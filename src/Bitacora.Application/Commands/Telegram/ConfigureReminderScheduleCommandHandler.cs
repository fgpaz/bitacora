using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Handler for ConfigureReminderScheduleCommand (RF-TG-010..012).
/// Creates or updates reminder configuration with timezone validation.
/// The UI sends a UTC schedule converted from patient-local Buenos Aires time (RF-TG-006).
/// </summary>
public sealed class ConfigureReminderScheduleCommandHandler(
    IReminderConfigRepository reminderConfigRepository,
    ITelegramSessionRepository telegramSessionRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<ConfigureReminderScheduleCommandHandler> logger)
    : IRequestHandler<ConfigureReminderScheduleCommand, ConfigureReminderScheduleResponse>
{
    private const string InvalidHourCode = "TG_006_INVALID_HOUR";
    private const string InvalidMinuteCode = "TG_006_INVALID_MINUTE";
    private const string InvalidTimezoneCode = "TG_006_INVALID_TIMEZONE";
    private const string NoActiveSessionCode = "TG_006_NO_ACTIVE_SESSION";

    public async ValueTask<ConfigureReminderScheduleResponse> Handle(
        ConfigureReminderScheduleCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Configuring reminder schedule, {HourUtc}:{MinuteUtc:D2} UTC, timezone={Timezone}",
            request.HourUtc, request.MinuteUtc, request.Timezone ?? "default");

        ValidateSchedule(request.HourUtc, request.MinuteUtc);

        var linkedSession = await telegramSessionRepository
            .FindLinkedByPatientIdAsync(request.PatientId, cancellationToken);

        if (linkedSession == null)
        {
            throw new BitacoraException(
                NoActiveSessionCode,
                "Telegram must be linked before configuring reminders.",
                403);
        }

        var resolvedTimezone = ValidateAndResolveTimezone(request.Timezone);

        // Get existing or create new reminder config.
        var existing = await reminderConfigRepository
            .FindByPatientIdAsync(request.PatientId, cancellationToken);

        ReminderConfig config = existing ?? ReminderConfig.CreateDefault(
            request.PatientId,
            request.HourUtc,
            request.MinuteUtc,
            resolvedTimezone);

        if (existing == null)
        {
            // Create new config with resolved timezone.
            await reminderConfigRepository.AddAsync(config, cancellationToken);
            logger.LogInformation(
                "Created new reminder config, {HourUtc}:{MinuteUtc:D2} {Timezone}",
                request.HourUtc, request.MinuteUtc, resolvedTimezone);
        }
        else
        {
            // Reschedule existing config with resolved timezone.
            config.ConfigureSchedule(request.HourUtc, request.MinuteUtc, resolvedTimezone, DateTime.UtcNow);
            await reminderConfigRepository.UpdateAsync(config, cancellationToken);
            logger.LogInformation(
                "Updated reminder config, {HourUtc}:{MinuteUtc:D2} {Timezone}",
                request.HourUtc, request.MinuteUtc, resolvedTimezone);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfigureReminderScheduleResponse(
            config.ReminderConfigId,
            config.HourUtc,
            config.MinuteUtc,
            config.ReminderTimezone,
            config.Enabled,
            config.NextFireAtUtc);
    }

    private static string ValidateAndResolveTimezone(string? timezone)
    {
        var trimmed = string.IsNullOrWhiteSpace(timezone)
            ? ReminderConfig.DefaultTimezone
            : timezone.Trim();

        if (trimmed.Equals(ReminderConfig.DefaultTimezone, StringComparison.Ordinal))
        {
            return ReminderConfig.DefaultTimezone;
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(trimmed, out var ianaTimeZoneId)
            && !string.IsNullOrWhiteSpace(ianaTimeZoneId)
            && IsValidTimeZoneId(ianaTimeZoneId))
        {
            return ianaTimeZoneId;
        }

        if (IsValidTimeZoneId(trimmed))
        {
            return trimmed;
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(trimmed, out var windowsTimeZoneId)
            && !string.IsNullOrWhiteSpace(windowsTimeZoneId)
            && IsValidTimeZoneId(windowsTimeZoneId))
        {
            return trimmed;
        }

        throw new BitacoraException(
            InvalidTimezoneCode,
            $"Timezone '{trimmed}' is not valid.",
            400);
    }

    private static bool IsValidTimeZoneId(string timezone)
    {
        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }

    private static void ValidateSchedule(int hourUtc, int minuteUtc)
    {
        if (hourUtc is < 0 or > 23)
        {
            throw new BitacoraException(
                InvalidHourCode,
                "Reminder hour must be an integer between 0 and 23.",
                400);
        }

        if (minuteUtc is not (0 or 30))
        {
            throw new BitacoraException(
                InvalidMinuteCode,
                "Reminder minute must be 0 or 30.",
                400);
        }
    }
}
