using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;
using System.Net.Http;
using System.Net.Http.Json;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Sends a scheduled Telegram reminder to a linked patient (RF-TG-010..012).
/// Fail-closed: if consent is revoked, session unlinked, or scheduling fails,
/// the reminder is silenced and rescheduled without exposing clinical data.
/// </summary>
public readonly record struct SendReminderCommand(
    Guid ReminderConfigId,
    Guid TraceId) : ICommand<SendReminderResponse>;

public sealed record SendReminderResponse(
    bool Sent,
    string? ErrorCode,
    DateTime? NextFireAtUtc);

public sealed class SendReminderCommandHandler(
    IConfiguration configuration,
    IReminderConfigRepository reminderConfigRepository,
    ITelegramSessionRepository sessionRepository,
    IConsentGrantRepository consentGrantRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IPseudonymizationService pseudonymizationService,
    ILogger<SendReminderCommandHandler> logger)
    : ICommandHandler<SendReminderCommand, SendReminderResponse>
{
    public async ValueTask<SendReminderResponse> Handle(SendReminderCommand command, CancellationToken cancellationToken)
    {
        if (command.ReminderConfigId == Guid.Empty)
        {
            throw new BitacoraException("REMINDER_CONFIG_ID_REQUIRED", "Reminder config id is required.", 400);
        }

        // 1. Load reminder config
        var reminderEntity = await reminderConfigRepository.GetByIdAsync(command.ReminderConfigId, cancellationToken);
        if (reminderEntity == null)
        {
            logger.LogWarning("ReminderConfig {ReminderConfigId} not found, trace {TraceId}",
                command.ReminderConfigId, command.TraceId);
            return new SendReminderResponse(false, "REMINDER_CONFIG_NOT_FOUND", null);
        }

        // 2. Fail-closed: skip if disabled
        if (!reminderEntity.Enabled)
        {
            logger.LogInformation("ReminderConfig {ReminderConfigId} is disabled, skipping", command.ReminderConfigId);
            return new SendReminderResponse(false, "REMINDER_DISABLED", null);
        }

        // 3. Consent gate (fail-closed)
        var consent = await consentGrantRepository.GetActiveByPatientAsync(reminderEntity.PatientId, cancellationToken);
        if (consent == null)
        {
            logger.LogWarning(
                "SendReminder patient {PatientId} has no active consent, silencing reminder {ReminderConfigId}, trace {TraceId}",
                reminderEntity.PatientId, command.ReminderConfigId, command.TraceId);

            reminderEntity.Disable(DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditAsync(command.TraceId, reminderEntity.PatientId, AuditOutcome.Denied, "consent_missing", cancellationToken);
            return new SendReminderResponse(false, "REMINDER_NO_CONSENT", null);
        }

        // 4. Session gate (fail-closed)
        var session = await sessionRepository.FindLinkedByPatientIdAsync(reminderEntity.PatientId, cancellationToken);
        if (session == null || session.Status != TelegramSessionStatus.Linked)
        {
            logger.LogWarning(
                "SendReminder patient {PatientId} has no linked Telegram session, silencing reminder {ReminderConfigId}, trace {TraceId}",
                reminderEntity.PatientId, command.ReminderConfigId, command.TraceId);

            reminderEntity.Disable(DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditAsync(command.TraceId, reminderEntity.PatientId, AuditOutcome.Denied, "session_unlinked", cancellationToken);
            return new SendReminderResponse(false, "REMINDER_NO_SESSION", null);
        }

        // 5. Send Telegram reminder — no clinical data per CT-TELEGRAM-RUNTIME invariant
        var botMessage = "Es hora de registrar tu humor del dia. Abre la app para registrarlo.";
        var sent = await SendTelegramMessageAsync(session.ChatId, botMessage, command.TraceId, cancellationToken);

        if (!sent)
        {
            logger.LogWarning(
                "Failed to send reminder for patient {PatientId}, trace {TraceId}",
                reminderEntity.PatientId, command.TraceId);
            return new SendReminderResponse(false, "REMINDER_SEND_FAILED", null);
        }

        // 6. Advance to next fire time
        reminderEntity.AdvanceNextFire(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(command.TraceId, reminderEntity.PatientId, AuditOutcome.Ok, null, cancellationToken);

        logger.LogInformation(
            "Reminder sent for patient {PatientId}, next fire at {NextFireAt}, trace {TraceId}",
            reminderEntity.PatientId, reminderEntity.NextFireAtUtc, command.TraceId);

        return new SendReminderResponse(true, null, reminderEntity.NextFireAtUtc);
    }

    private Task<bool> SendTelegramMessageAsync(
        string chatId, string message, Guid traceId, CancellationToken cancellationToken)
    {
        var token = configuration["TELEGRAM_BOT_TOKEN"] ?? configuration["Telegram:BotToken"];
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("Telegram bot token missing; reminder send skipped, trace {TraceId}", traceId);
            return Task.FromResult(false);
        }

        return SendViaTelegramApiAsync(token, chatId, message, traceId, cancellationToken);
    }

    private async Task<bool> SendViaTelegramApiAsync(
        string token,
        string chatId,
        string message,
        Guid traceId,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var response = await client.PostAsJsonAsync(
            $"https://api.telegram.org/bot{token}/sendMessage",
            new { chat_id = chatId, text = message },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Telegram API returned {StatusCode} for reminder send, trace {TraceId}",
                response.StatusCode,
                traceId);
            return false;
        }

        return true;
    }

    private async Task WriteAuditAsync(
        Guid traceId, Guid patientId, AuditOutcome outcome, string? reason, CancellationToken cancellationToken)
    {
        var pseudonymId = pseudonymizationService.CreatePseudonym(patientId);

        var audit = AccessAudit.Create(
            traceId,
            actorId: Guid.Empty,
            pseudonymId: pseudonymId,
            AuditActionType.TelegramAudit,
            resourceType: "TelegramReminder",
            resourceId: null,
            patientId,
            outcome,
            DateTime.UtcNow);

        await accessAuditRepository.AddAsync(audit, cancellationToken);
    }
}
