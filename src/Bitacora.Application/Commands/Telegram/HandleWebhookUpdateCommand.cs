using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mediator;
using Microsoft.Extensions.Configuration;
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
/// Handles incoming Telegram webhook updates (RF-REG-010..015).
/// Fail-closed: missing consent, unknown session, invalid/expired codes, or unsafe state
/// are audited and silently denied (200 to Telegram to stop re-delivery).
/// Sequential factors flow (RF-REG-013): mood score -> sleep -> physical -> social ->
/// anxiety -> irritability -> medication -> (medication_time) -> DailyCheckin.
/// </summary>
public readonly record struct HandleWebhookUpdateCommand(
    string? Payload,
    string? ChatId,
    Guid TraceId,
    string? CallbackQueryId) : ICommand<HandleWebhookUpdateResponse>;

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
    IConfiguration configuration,
    ILogger<HandleWebhookUpdateCommandHandler> logger)
    : ICommandHandler<HandleWebhookUpdateCommand, HandleWebhookUpdateResponse>
{
    // Transient dictionary to accumulate factors between Telegram messages.
    // Key: chat_id, Value: accumulated factor values.
    private static readonly Dictionary<string, TelegramFactorAccumulator> _factorAccumulators = new();

    // System actor ID used for Telegram audit records where no authenticated patient is known.
    // Distinct from Guid.Empty (which AccessAudit.Create rejects) and from any real patient ID.
    private static readonly Guid TelegramBotActorId = new("b07acc00-0000-0000-b070-000000000000");

    public async ValueTask<HandleWebhookUpdateResponse> Handle(HandleWebhookUpdateCommand command, CancellationToken cancellationToken)
    {
        // Immediately acknowledge callback_query to dismiss Telegram button spinner.
        // Safe to call unconditionally — returns early if CallbackQueryId is null or whitespace.
        await AnswerCallbackQueryAsync(command.CallbackQueryId, command.TraceId, cancellationToken);

        var (messageType, code, rawText) = ParsePayload(command.Payload);

        // ── RF-REG-014: /start CODE routing ─────────────────────────────────
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
                patientId: null,
                cancellationToken);

            // Always reply to Telegram (for pairing confirmation message)
            if (!string.IsNullOrWhiteSpace(confirmResult.UserMessage))
            {
                await SendTelegramMessageAsync(command.ChatId, confirmResult.UserMessage, command.TraceId, cancellationToken);
            }

            return new HandleWebhookUpdateResponse(confirmResult.Success, confirmResult.ErrorCode, confirmResult.UserMessage);
        }

        if (messageType == "start_without_code")
        {
            var reply = "Enviá el codigo que aparece en la seccion de Telegram de la web.";
            await SendTelegramMessageAsync(command.ChatId!, reply, command.TraceId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }

        // ── RF-REG-011: session resolution ────────────────────────────────────
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
        var session = await sessionRepository.GetByChatIdAsync(command.ChatId, cancellationToken);
        if (session == null)
        {
            logger.LogWarning("Telegram webhook for unknown Telegram session, trace {TraceId}",
                command.TraceId);
            await WriteAuditAsync(
                command.TraceId, command.ChatId, messageType, rawText,
                AuditOutcome.Denied, "session_not_found", null, cancellationToken);
            // Return 200 to Telegram; silent deny (no bot message) per CT-TELEGRAM-RUNTIME invariant
            return new HandleWebhookUpdateResponse(false, "TG_WEBHOOK_UNKNOWN_SESSION", null);
        }

        // ── RF-REG-015: consent check ─────────────────────────────────────────
        var consent = await consentGrantRepository.GetActiveByPatientAsync(session.PatientId, cancellationToken);
        if (consent == null)
        {
            logger.LogWarning(
                "Telegram webhook for linked session with no active consent, trace {TraceId}",
                command.TraceId);
            await WriteAuditAsync(
                command.TraceId, command.ChatId, messageType, rawText,
                AuditOutcome.Denied, "consent_revoked_or_missing", session.PatientId, cancellationToken);

            // RF-REG-015: send consent request message
            var consentReply = "Para registrar tu humor necesitamos tu consentimiento. " +
                               "Por favor vincula tu cuenta desde la web para continuar.";
            await SendTelegramMessageAsync(command.ChatId, consentReply, command.TraceId, cancellationToken);
            return new HandleWebhookUpdateResponse(false, "TG_WEBHOOK_NO_CONSENT", consentReply);
        }

        // ── RF-REG-013: sequential factors flow ───────────────────────────────
        if (session.ConversationState != TelegramConversationState.Idle)
        {
            return await HandleFactorInputAsync(session, rawText ?? string.Empty, messageType, command.TraceId, cancellationToken);
        }

        // ── RF-REG-012: mood score input ──────────────────────────────────────
        if (messageType == "mood_input" && !string.IsNullOrWhiteSpace(rawText))
        {
            return await HandleMoodInputAsync(session, rawText, command.TraceId, cancellationToken);
        }

        // ── Generic fallback ──────────────────────────────────────────────────
        var fallback = "Usa /start para vincular tu cuenta o escribe tu estado de animo (por ejemplo +2 o -1).";
        await SendTelegramMessageAsync(session.ChatId, fallback, command.TraceId, cancellationToken);

        await WriteAuditAsync(
            command.TraceId, command.ChatId, messageType, rawText,
            AuditOutcome.Ok, null, session.PatientId, cancellationToken);

        return new HandleWebhookUpdateResponse(true, null, fallback);
    }

    /// <summary>
    /// RF-REG-012: Records mood score, initiates sequential factors flow.
    /// </summary>
    private async ValueTask<HandleWebhookUpdateResponse> HandleMoodInputAsync(
        TelegramSession session,
        string moodValue,
        Guid traceId,
        CancellationToken cancellationToken)
    {
        // Parse mood value: +3, +2, +1, 0, -1, -2, -3
        if (!int.TryParse(moodValue, out var score) || score is < -3 or > 3)
        {
            logger.LogWarning(
                "Invalid mood value {MoodValue} from session {SessionId}, trace {TraceId}",
                moodValue, session.TelegramSessionId, traceId);
            var reply = "Valor invalido. Usa +3, +2, +1, 0, -1, -2 o -3.";
            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            await WriteAuditAsync(traceId, session.ChatId, "mood_input", moodValue,
                AuditOutcome.Denied, "invalid_mood_value", session.PatientId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }

        try
        {
            // Create MoodEntry via existing command (RF-REG-012)
            var createMoodCmd = new CreateMoodEntryCommand(
                PatientId: session.PatientId,
                ActorId: session.PatientId,
                TraceId: traceId,
                Score: score,
                Channel: "telegram");

            var moodResult = await mediator.Send(createMoodCmd, cancellationToken);

            logger.LogInformation(
                "Telegram mood persisted, entry {MoodEntryId}, duplicate={IsDuplicate}, trace {TraceId}",
                moodResult.MoodEntryId, moodResult.IsDuplicate, traceId);

            // Initialize factor accumulator
            var accumulator = new TelegramFactorAccumulator { MoodScore = score };
            _factorAccumulators[session.ChatId] = accumulator;

            // RF-REG-013 step 1: ask for sleep hours with inline keyboard
            var reply = (moodResult.IsDuplicate ? "Tu registro ya estaba guardado hace poco." : "Registro guardado.") +
                        "\n\n" +
                        "Ahora contame un poco más:\n" +
                        "¿Cuántas horas dormiste anoche?";

            // Advance session conversation state
            var nowUtc = DateTime.UtcNow;
            session.AdvanceToAwaitingFactors(score, JsonSerializer.Serialize(accumulator), nowUtc);
            await sessionRepository.UpdateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                BuildSleepKeyboard());
            await WriteAuditAsync(traceId, session.ChatId, "mood_input", moodValue,
                AuditOutcome.Ok, null, session.PatientId, cancellationToken);

            return new HandleWebhookUpdateResponse(true, null, reply);
        }
        catch (BitacoraException ex) when (ex.StatusCode == 422)
        {
            logger.LogWarning(
                "Telegram mood duplicate check rejected, trace {TraceId}",
                traceId);
            var reply = "Ese valor ya lo registraste hace poco. Proba otro o espera unos minutos.";
            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            await WriteAuditAsync(traceId, session.ChatId, "mood_input", moodValue,
                AuditOutcome.Denied, "duplicate_mood", session.PatientId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to persist Telegram mood, trace {TraceId}",
                traceId);
            var reply = "No pudimos registrarlo. Intenta de nuevo mas tarde.";
            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            await WriteAuditAsync(traceId, session.ChatId, "mood_input", moodValue,
                AuditOutcome.Denied, "persist_error", session.PatientId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }
    }

    /// <summary>
    /// RF-REG-013: Handles factor inputs as part of the sequential conversation.
    /// </summary>
    private async ValueTask<HandleWebhookUpdateResponse> HandleFactorInputAsync(
        TelegramSession session,
        string rawText,
        string messageType,
        Guid traceId,
        CancellationToken cancellationToken)
    {
        // Retrieve or initialize accumulator; re-hydrate from DB if lost on server restart
        if (!_factorAccumulators.TryGetValue(session.ChatId, out var accumulator))
        {
            accumulator = !string.IsNullOrWhiteSpace(session.PendingFactorsJson)
                ? JsonSerializer.Deserialize<TelegramFactorAccumulator>(session.PendingFactorsJson)
                  ?? new TelegramFactorAccumulator()
                : new TelegramFactorAccumulator();
            _factorAccumulators[session.ChatId] = accumulator;
        }

        var nowUtc = DateTime.UtcNow;
        string reply;
        var state = session.ConversationState;

        try
        {
            switch (state)
            {
                // ── Sleep hours ────────────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorSleep:
                {
                    if (!TryParseSleepHours(rawText, out var sleepHours))
                    {
                        reply = "Formato invalido. Cuantas horas dormiste anoche? (0-24, ejemplo: 7.5)";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.SleepHours = sleepHours;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    reply = "Tuviste actividad fisica hoy?";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                        BuildYesNoKeyboard());
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }

                // ── Physical activity ─────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorPhysical:
                {
                    if (!TryParseYesNo(rawText, out var physical))
                    {
                        reply = "Respuesta invalida. Tuviste actividad fisica hoy?";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                            BuildYesNoKeyboard());
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.PhysicalActivity = physical;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    reply = "Tuviste actividad social hoy?";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                        BuildYesNoKeyboard());
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }

                // ── Social activity ───────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorSocial:
                {
                    if (!TryParseYesNo(rawText, out var social))
                    {
                        reply = "Respuesta invalida. Tuviste actividad social hoy?";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                            BuildYesNoKeyboard());
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.SocialActivity = social;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    reply = "Sentiste ansiedad hoy?";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                        BuildYesNoKeyboard());
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }

                // ── Anxiety ───────────────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorAnxiety:
                {
                    if (!TryParseYesNo(rawText, out var anxiety))
                    {
                        reply = "Respuesta invalida. Sentiste ansiedad hoy?";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                            BuildYesNoKeyboard());
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.Anxiety = anxiety;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    reply = "Sentiste irritabilidad hoy?";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                        BuildYesNoKeyboard());
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }

                // ── Irritability ───────────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorIrritability:
                {
                    if (!TryParseYesNo(rawText, out var irritability))
                    {
                        reply = "Respuesta invalida. Sentiste irritabilidad hoy?";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                            BuildYesNoKeyboard());
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.Irritability = irritability;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    reply = "Tomaste medicacion hoy?";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                        BuildYesNoKeyboard());
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }

                // ── Medication ─────────────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorMedication:
                {
                    if (!TryParseYesNo(rawText, out var medication))
                    {
                        reply = "Respuesta invalida. Tomaste medicacion hoy?";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken,
                            BuildYesNoKeyboard());
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.MedicationTaken = medication;
                    session.AdvanceToNextFactor(JsonSerializer.Serialize(accumulator), nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    if (medication)
                    {
                        reply = "A que hora? (HH:mm, ejemplo: 08:00)";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    // No medication — complete check-in immediately
                    return await CompleteCheckinAsync(session, accumulator, traceId, cancellationToken);
                }

                // ── Medication time ────────────────────────────────────────────
                case TelegramConversationState.AwaitingFactorMedicationTime:
                {
                    if (!TryParseTimeOnly(rawText, out var medTime))
                    {
                        reply = "Formato invalido. A que hora? (HH:mm, ejemplo: 08:00)";
                        await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
                        return new HandleWebhookUpdateResponse(true, null, reply);
                    }

                    accumulator.MedicationTime = medTime;
                    return await CompleteCheckinAsync(session, accumulator, traceId, cancellationToken);
                }

                default:
                {
                    // Unknown state — reset
                    session.ResetToIdle(nowUtc);
                    await sessionRepository.UpdateAsync(session, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    _factorAccumulators.Remove(session.ChatId);

                    reply = "Ups, perdimos el hilo. Escribi tu estado de animo para comenzar de nuevo.";
                    await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
                    return new HandleWebhookUpdateResponse(true, null, reply);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Factor input error for session {SessionId}, state {State}, trace {TraceId}",
                session.TelegramSessionId, state, traceId);

            session.ResetToIdle(nowUtc);
            await sessionRepository.UpdateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _factorAccumulators.Remove(session.ChatId);

            reply = "Algo salio mal. Escribi tu estado de animo para comenzar de nuevo.";
            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }
    }

    /// <summary>
    /// Completes the daily check-in after all factors are collected (RF-REG-013).
    /// </summary>
    private async ValueTask<HandleWebhookUpdateResponse> CompleteCheckinAsync(
        TelegramSession session,
        TelegramFactorAccumulator accumulator,
        Guid traceId,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        try
        {
            var checkinCmd = new CreateDailyCheckinCommand(
                PatientId: session.PatientId,
                MoodScore: accumulator.MoodScore!.Value,
                SleepHours: accumulator.SleepHours,
                PhysicalActivity: accumulator.PhysicalActivity,
                SocialActivity: accumulator.SocialActivity,
                Anxiety: accumulator.Anxiety,
                Irritability: accumulator.Irritability,
                MedicationTaken: accumulator.MedicationTaken,
                MedicationTime: accumulator.MedicationTime,
                TraceId: traceId,
                Channel: "telegram");

            var checkinResult = await mediator.Send(checkinCmd, cancellationToken);

            var actionWord = checkinResult.Created ? "Registro completo." : "Check-in actualizado.";
            var reply = $"{actionWord}\n\nYa podés verlo en tu historial web.";

            logger.LogInformation(
                "Telegram DailyCheckin persisted, id {DailyCheckinId}, created={Created}, trace {TraceId}",
                checkinResult.DailyCheckinId, checkinResult.Created, traceId);

            // Reset session and accumulator
            session.ResetToIdle(nowUtc);
            await sessionRepository.UpdateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _factorAccumulators.Remove(session.ChatId);

            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            await WriteAuditAsync(traceId, session.ChatId, "checkin_complete", null,
                AuditOutcome.Ok, null, session.PatientId, cancellationToken);

            return new HandleWebhookUpdateResponse(true, null, reply);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to complete Telegram DailyCheckin, trace {TraceId}",
                traceId);

            session.ResetToIdle(nowUtc);
            await sessionRepository.UpdateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _factorAccumulators.Remove(session.ChatId);

            var reply = "No pudimos guardar los factores. Tu humor quedo registrado; podes reintentar los factores mas tarde.";
            await SendTelegramMessageAsync(session.ChatId, reply, traceId, cancellationToken);
            return new HandleWebhookUpdateResponse(true, null, reply);
        }
    }

    // ── Telegram API ─────────────────────────────────────────────────────────────

    private async Task SendTelegramMessageAsync(
        string chatId, string message, Guid traceId, CancellationToken cancellationToken,
        object? replyMarkup = null)
    {
        var token = configuration["TELEGRAM_BOT_TOKEN"] ?? configuration["Telegram:BotToken"];
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("Telegram bot token missing; message not sent, trace {TraceId}", traceId);
            return;
        }

        await SendViaTelegramApiAsync(token, chatId, message, traceId, cancellationToken, replyMarkup);
    }

    /// <summary>
    /// Calls Telegram answerCallbackQuery to dismiss the button spinner immediately.
    /// Safe to call with null — returns early without error.
    /// </summary>
    private async Task AnswerCallbackQueryAsync(
        string? callbackQueryId, Guid traceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(callbackQueryId)) return;

        var token = configuration["TELEGRAM_BOT_TOKEN"] ?? configuration["Telegram:BotToken"];
        if (string.IsNullOrWhiteSpace(token)) return;

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            await client.PostAsJsonAsync(
                $"https://api.telegram.org/bot{token}/answerCallbackQuery",
                new { callback_query_id = callbackQueryId },
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "answerCallbackQuery failed for id {CallbackQueryId}, trace {TraceId}",
                callbackQueryId, traceId);
        }
    }

    /// <summary>
    /// Inline keyboard for sleep hours selection (4–9 h), used after mood score is recorded.
    /// Callback data matches strings accepted by TryParseSleepHours.
    /// </summary>
    private static object BuildSleepKeyboard() => new
    {
        inline_keyboard = new[]
        {
            new object[]
            {
                new { text = "4h", callback_data = "4" },
                new { text = "5h", callback_data = "5" },
                new { text = "6h", callback_data = "6" },
                new { text = "7h", callback_data = "7" },
                new { text = "8h", callback_data = "8" },
                new { text = "9h", callback_data = "9" },
            }
        }
    };

    /// <summary>
    /// Inline keyboard for binary yes/no questions (si/no factors).
    /// Callback data matches strings accepted by TryParseYesNo.
    /// </summary>
    private static object BuildYesNoKeyboard() => new
    {
        inline_keyboard = new[]
        {
            new object[]
            {
                new { text = "Sí", callback_data = "si" },
                new { text = "No", callback_data = "no" },
            }
        }
    };

    private async Task<bool> SendViaTelegramApiAsync(
        string token,
        string chatId,
        string message,
        Guid traceId,
        CancellationToken cancellationToken,
        object? replyMarkup = null)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        // Telegram rejects reply_markup:null with 400 "object expected as reply markup".
        // Use WhenWritingNull so the field is omitted entirely when no keyboard is needed.
        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var delays = new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) };
        var attempt = 0;

        while (true)
        {
            try
            {
                var response = await client.PostAsJsonAsync(
                    $"https://api.telegram.org/bot{token}/sendMessage",
                    new { chat_id = chatId, text = message, reply_markup = replyMarkup },
                    jsonOptions,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                    return true;

                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    logger.LogWarning(
                        "Telegram API client error {StatusCode} (no retry), trace {TraceId}",
                        response.StatusCode, traceId);
                    return false;
                }

                logger.LogWarning(
                    "Telegram API returned {StatusCode}, attempt {Attempt}, trace {TraceId}",
                    response.StatusCode, attempt + 1, traceId);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(
                    "Telegram API request failed: {Message}, attempt {Attempt}, trace {TraceId}",
                    ex.Message, attempt + 1, traceId);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(
                    "Telegram API request timed out, attempt {Attempt}, trace {TraceId}",
                    attempt + 1, traceId);
            }

            if (attempt >= delays.Length)
            {
                logger.LogWarning(
                    "Telegram API retry limit reached, trace {TraceId}",
                    traceId);
                return false;
            }

            await Task.Delay(delays[attempt], cancellationToken);
            attempt++;
        }
    }

    // ── Payload parsing ─────────────────────────────────────────────────────────

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

    // ── Factor parsers ─────────────────────────────────────────────────────────

    private static bool TryParseSleepHours(string input, out decimal sleepHours)
    {
        // Accept "7", "7.5", "7,5", "07:30" (as 7.5h) formats
        var normalized = input.Trim().Replace(',', '.');

        if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var hours) &&
            hours >= 0 && hours <= 24)
        {
            sleepHours = hours;
            return true;
        }

        // Try HH:mm format (minutes as fractional hours)
        if (TimeOnly.TryParse(input.Trim(), out var t))
        {
            sleepHours = t.Hour + t.Minute / 60m;
            return sleepHours <= 24;
        }

        sleepHours = default;
        return false;
    }

    private static bool TryParseYesNo(string input, out bool value)
    {
        var normalized = input.Trim().ToLowerInvariant();
        switch (normalized)
        {
            case "si" or "sí" or "s" or "yes" or "y" or "1" or "true":
                value = true;
                return true;
            case "no" or "n" or "0" or "false":
                value = false;
                return true;
            default:
                value = default;
                return false;
        }
    }

    private static bool TryParseTimeOnly(string input, out TimeOnly time)
    {
        // Try "HH:mm" format first
        if (TimeOnly.TryParse(input.Trim(), out time))
            return true;

        // Try "H:mm" without leading zero
        if (TimeOnly.TryParseExact(input.Trim(), ["H:mm", "HHmm", "Hmm"], CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            return true;

        return false;
    }

    // ── Audit ──────────────────────────────────────────────────────────────────

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
            ? pseudonymizationService.CreatePseudonym(Guid.Empty)
            : "tg:unknown";

        var audit = AccessAudit.Create(
            traceId,
            actorId: patientId ?? TelegramBotActorId,
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

/// <summary>
/// Transient accumulator for Telegram sequential factors (not persisted — kept in memory).
/// Serialized to JSON and stored in TelegramSession.PendingFactorsJson.
/// </summary>
internal sealed class TelegramFactorAccumulator
{
    public int? MoodScore { get; set; }
    public decimal SleepHours { get; set; }
    public bool PhysicalActivity { get; set; }
    public bool SocialActivity { get; set; }
    public bool Anxiety { get; set; }
    public bool Irritability { get; set; }
    public bool MedicationTaken { get; set; }
    public TimeOnly? MedicationTime { get; set; }
}
