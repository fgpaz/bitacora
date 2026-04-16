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
/// Timezone lookup follows platform conventions: Windows TZID, IANA fallback, Argentina default.
/// </summary>
public sealed class ConfigureReminderScheduleCommandHandler(
    IReminderConfigRepository reminderConfigRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<ConfigureReminderScheduleCommandHandler> logger)
    : IRequestHandler<ConfigureReminderScheduleCommand, ConfigureReminderScheduleResponse>
{
    public async ValueTask<ConfigureReminderScheduleResponse> Handle(
        ConfigureReminderScheduleCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Configuring reminder schedule for {PatientId}, {HourUtc}:{MinuteUtc:D2} UTC, timezone={Timezone}",
            request.PatientId, request.HourUtc, request.MinuteUtc, request.Timezone ?? "default");

        // Validate timezone if provided.
        string resolvedTimezone = "Etc/UTC";
        if (!string.IsNullOrWhiteSpace(request.Timezone))
        {
            resolvedTimezone = ValidateAndResolveTimezone(request.Timezone);
        }

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
                "Created new reminder config for {PatientId}, {HourUtc}:{MinuteUtc:D2} {Timezone}",
                request.PatientId, request.HourUtc, request.MinuteUtc, resolvedTimezone);
        }
        else
        {
            // Reschedule existing config with resolved timezone.
            config.Reschedule(request.HourUtc, request.MinuteUtc, resolvedTimezone, DateTime.UtcNow);
            await reminderConfigRepository.UpdateAsync(config, cancellationToken);
            logger.LogInformation(
                "Updated reminder config for {PatientId}, {HourUtc}:{MinuteUtc:D2} {Timezone}",
                request.PatientId, request.HourUtc, request.MinuteUtc, resolvedTimezone);
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

    private static string ValidateAndResolveTimezone(string timezone)
    {
        TimeZoneInfo? tz = null;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            tz = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(z => z.HasIanaId && z.Id == timezone);
            if (tz == null)
            {
                tz = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(z => z.Id.Contains("Argentina", StringComparison.OrdinalIgnoreCase));
            }
        }

        if (tz == null)
        {
            throw new BitacoraException(
                code: "INVALID_TIMEZONE",
                message: $"Timezone '{timezone}' is not valid. Could not find system timezone.",
                statusCode: 400);
        }

        return tz.Id;
    }
}
