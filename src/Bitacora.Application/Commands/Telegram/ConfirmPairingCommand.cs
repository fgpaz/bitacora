using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Confirms Telegram pairing when a user sends /start CODE to the bot (RF-TG-002).
/// Validates the code, checks chat_id uniqueness, creates TelegramSession.
/// This is called from the Telegram webhook handler (not a REST endpoint).
/// </summary>
public readonly record struct ConfirmPairingCommand(
    string Code,
    string ChatId,
    Guid TraceId) : ICommand<ConfirmPairingResponse>;

public sealed record ConfirmPairingResponse(
    bool Success,
    string? ErrorCode,
    string UserMessage);

public sealed class ConfirmPairingCommandHandler(
    ITelegramPairingCodeRepository pairingCodeRepository,
    ITelegramSessionRepository sessionRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<ConfirmPairingCommandHandler> logger)
    : ICommandHandler<ConfirmPairingCommand, ConfirmPairingResponse>
{
    public async ValueTask<ConfirmPairingResponse> Handle(ConfirmPairingCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
        {
            return new ConfirmPairingResponse(false, "TG_002_CODE_INVALID", "Codigo invalido o expirado.");
        }

        if (string.IsNullOrWhiteSpace(command.ChatId))
        {
            return new ConfirmPairingResponse(false, "TG_002_CODE_INVALID", "Codigo invalido o expirado.");
        }

        var nowUtc = DateTime.UtcNow;

        // 1. Find valid pairing code
        var pairingCode = await pairingCodeRepository.FindValidByCodeAsync(command.Code.Trim(), cancellationToken);
        if (pairingCode == null)
        {
            logger.LogWarning("Telegram pairing code not found or expired: {Code}, trace {TraceId}",
                MaskCode(command.Code), command.TraceId);
            return new ConfirmPairingResponse(false, "TG_002_CODE_INVALID", "Codigo invalido o expirado.");
        }

        // 2. Check if code is expired
        if (!pairingCode.IsValid(nowUtc))
        {
            logger.LogWarning("Telegram pairing code expired: {Code}, trace {TraceId}",
                MaskCode(command.Code), command.TraceId);
            return new ConfirmPairingResponse(false, "TG_002_CODE_EXPIRED", "Codigo invalido o expirado.");
        }

        // 3. Check chat_id is not already linked to another patient (RF-TG-003 invariant)
        var existingChatSession = await sessionRepository.FindLinkedByChatIdAsync(command.ChatId, cancellationToken);
        if (existingChatSession != null)
        {
            // If same patient already has this chat linked, consider it success (idempotent)
            if (existingChatSession.PatientId == pairingCode.PatientId)
            {
                return new ConfirmPairingResponse(true, null, "Cuenta vinculada. Ya podes registrar tu humor desde aca.");
            }

            logger.LogWarning("Telegram chat_id {ChatId} already linked to another patient, trace {TraceId}",
                command.ChatId, command.TraceId);
            return new ConfirmPairingResponse(false, "TG_002_CHAT_DUPLICATE", "Este Telegram ya esta vinculado a otra cuenta.");
        }

        // 4. Consume the code and create session in same transaction
        pairingCode.MarkAsUsed(nowUtc);
        var session = TelegramSession.CreateLinked(pairingCode.PatientId, command.ChatId, nowUtc);

        await pairingCodeRepository.UpdateAsync(pairingCode, cancellationToken);
        await sessionRepository.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Telegram session {SessionId} created for patient {PatientId}, chat_id {ChatId}, trace {TraceId}",
            session.TelegramSessionId, pairingCode.PatientId, command.ChatId, command.TraceId);

        return new ConfirmPairingResponse(true, null, "Cuenta vinculada. Ya podes registrar tu humor desde aca.");
    }

    /// <summary>Logs code without exposing full value.</summary>
    private static string MaskCode(string code) => code.Length > 4 ? $"***{code[^4..]}" : "****";
}
