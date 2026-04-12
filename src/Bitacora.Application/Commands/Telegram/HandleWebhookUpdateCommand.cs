using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.Application.Commands.Registro;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Handles incoming Telegram webhook updates (RF-TG-RUNTIME).
/// Fail-closed: missing consent, unknown session, invalid/expired codes, or unsafe state
/// are audited and silently denied (200 to Telegram to stop re-delivery).
/// </summary>
public readonly record struct HandleWebhookUpdateCommand(
    string? Payload,
    string? ChatId,
    Guid TraceId) : ICommand<HandleWebhookUpdateResponse>;

public sealed record HandleWebhookUpdateResponse(
    bool Accepted,
    string? ErrorCode,
    string? BotMessage);

public sealed class HandleWebhookUpdateCommandHandler(
    ITelegramSessionRepository sessionRepository,
    IConsentGrantRepository consentGrantRepository,
    IAccessAuditRepository accessAuditRepository,
    IPseudonymizationService pseudonymizationService,
    IBitacoraUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<HandleWebhookUpdateCommandHandler> logger)
    : ICommandHandler<HandleWebhookUpdateCommand, HandleWebhookUpdateResponse>
{
    public async ValueTask<HandleWebhookUpdateResponse> Handle(HandleWebhookUpdateCommand command, CancellationToken cancellationToken)
    {
        var (messageType, code, rawText) = ParsePayload(command.Payload);

        if (messageType == "start_with_code" && !string.IsNullOrWhiteSpace(command.ChatId) && !string.IsNullOrWhiteSpace(code))
        {
            var confirmResult = await mediator.Send(
                new ConfirmPairingCommand(code.Trim(), command.ChatId, command.TraceId),
                cancellationToken);

            await WriteAuditAsync(
                command.TraceId,
                command.ChatId,
                messageType,
                rawText,
                confirmResult.Success ? AuditOutcome.Ok : AuditOutcome.Denied,
                confirmResult.ErrorCode,
                null,
                cancellationToken);

            return new HandleWebhookUpdateResponse(confirmResult.Success, confirmResult.ErrorCode, confirmResult.UserMessage);
        }

        if (messageType == "start_without_code")
        {
            return new HandleWebhookUpdateResponse(true, null, "Enviá el código que aparece en la sección de Telegram de la web.");
        }

        // Fail-closed: reject empty chat_id
        if (string.IsNullOrWhiteSpace(command.ChatId))
        {
            logger.LogWarning("Telegram webhook received empty chat_id, trace {TraceId}", command.TraceId);
            await WriteAuditAsync(
                command.TraceId, null, messageType, rawText,
                AuditOutcome.Denied, "chat_id_missing", null, cancellationToken);
            return new HandleWebhookUpdateResponse(false, "TG_WEBHOOK_NO_CHAT_ID", null);
        }

        // Fail-closed: unknown session (no linked TelegramSession)
        var session = await sessionRepository.FindLinkedByChatIdAsync(command.ChatId, cancellationToken);
        if (session == null)
        {
            logger.LogWarning("Telegram webhook for unknown chat_id {ChatId}, trace {TraceId}",
                command.ChatId, command.TraceId);
            await WriteAuditAsync(
                command.TraceId, command.ChatId, messageType, rawText,
                AuditOutcome.Denied, "session_not_found", null, cancellationToken);
            // Return 200 to Telegram to stop re-delivery; no bot message (silent deny)
            return new HandleWebhookUpdateResponse(false, "TG_WEBHOOK_UNKNOWN_SESSION", "Primero vinculá tu cuenta desde la web.");
        }

        // Fail-closed: consent revoked or missing
        var consent = await consentGrantRepository.GetActiveByPatientAsync(session.PatientId, cancellationToken);
        if (consent == null)
        {
            logger.LogWarning(
                "Telegram webhook for patient {PatientId} with no active consent, trace {TraceId}",
                session.PatientId, command.TraceId);
            await WriteAuditAsync(
                command.TraceId, command.ChatId, messageType, rawText,
                AuditOutcome.Denied, "consent_revoked_or_missing", session.PatientId, cancellationToken);
            return new HandleWebhookUpdateResponse(false, "TG_WEBHOOK_NO_CONSENT", null);
        }

        // Route based on message type
        string? botMessage = null;

        if (messageType == "mood_input" && !string.IsNullOrWhiteSpace(rawText))
        {
            botMessage = await ProcessMoodInputAsync(session.PatientId, rawText, command.TraceId, cancellationToken);
        }
        else
        {
            // Generic fallback — no clinical data exposed
            botMessage = "Hola! Usa /start para vincular tu cuenta o escribe tu estado de animo.";
        }

        await WriteAuditAsync(
            command.TraceId, command.ChatId, messageType, rawText,
            AuditOutcome.Ok, null, session.PatientId, cancellationToken);

        return new HandleWebhookUpdateResponse(true, null, botMessage);
    }

    private static (string MessageType, string? Code, string? RawText) ParsePayload(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return ("unknown", null, null);
        }

        if (payload.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            var parts = payload.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var code = parts.Length > 1 ? parts[1].Trim() : null;
            return string.IsNullOrWhiteSpace(code)
                ? ("start_without_code", null, payload)
                : ("start_with_code", code, payload);
        }

        // Inline keyboard mood values: +3, +2, +1, 0, -1, -2, -3
        if (payload is "+3" or "+2" or "+1" or "0" or "-1" or "-2" or "-3")
        {
            return ("mood_input", null, payload);
        }

        return ("text_input", null, payload);
    }

    private async Task<string> ProcessMoodInputAsync(
        Guid patientId, string moodValue, Guid traceId, CancellationToken cancellationToken)
    {
        // Parse mood value: +3, +2, +1, 0, -1, -2, -3
        if (!int.TryParse(moodValue, out var score) || score is < -3 or > 3)
        {
            logger.LogWarning(
                "Invalid mood value {MoodValue} from patient {PatientId}, trace {TraceId}",
                moodValue, patientId, traceId);
            return "Valor invalido. Usa +3, +2, +1, 0, -1, -2 o -3.";
        }

        try
        {
            var createCommand = new CreateMoodEntryCommand(
                PatientId: patientId,
                ActorId: patientId,  // Telegram input is patient-initiated
                TraceId: traceId,
                Score: score,
                Channel: "telegram");

            var result = await mediator.Send(createCommand, cancellationToken);

            var dupMsg = result.IsDuplicate
                ? " (ya estaba registrado hace poco)."
                : ".";

            logger.LogInformation(
                "Telegram mood {Score} persisted for patient {PatientId}, entry {MoodEntryId}, duplicate={IsDuplicate}, trace {TraceId}",
                score, patientId, result.MoodEntryId, result.IsDuplicate, traceId);

            return $"Registrado: {score}{dupMsg}";
        }
        catch (BitacoraException ex) when (ex.StatusCode == 422)
        {
            logger.LogWarning(
                "Telegram mood duplicate check rejected {Score} for patient {PatientId}, trace {TraceId}",
                score, patientId, traceId);
            return "Ese valor ya lo registraste hace poco. Probá otro o espera unos minutos.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to persist Telegram mood {Score} for patient {PatientId}, trace {TraceId}",
                score, patientId, traceId);
            return "No pudimos registrarlo. Intentá de nuevo mas tarde.";
        }
    }

    private async Task WriteAuditAsync(
        Guid traceId,
        string? chatId,
        string messageType,
        string? rawText,
        AuditOutcome outcome,
        string? reason,
        Guid? patientId,
        CancellationToken cancellationToken)
    {
        var pseudonymId = !string.IsNullOrWhiteSpace(chatId)
            ? pseudonymizationService.CreatePseudonym(Guid.Empty) // chatId-based pseudonym via service
            : "tg:unknown";

        var audit = AccessAudit.Create(
            traceId,
            actorId: Guid.Empty,
            pseudonymId: pseudonymId,
            AuditActionType.TelegramAudit,
            resourceType: $"TelegramWebhook:{messageType}",
            resourceId: null,
            patientId,
            outcome,
            DateTime.UtcNow);

        await accessAuditRepository.AddAsync(audit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
