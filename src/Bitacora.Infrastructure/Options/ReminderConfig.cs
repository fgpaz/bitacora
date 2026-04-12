using Microsoft.Extensions.Options;

namespace NuestrasCuentitas.Bitacora.Infrastructure.Options;

/// <summary>
/// Reminder schedule configuration used by the reminder hosted service.
/// Loaded from appsettings.json section "ReminderConfig".
/// </summary>
public sealed class ReminderConfig
{
    public const string SectionName = "ReminderConfig";

    /// <summary>Interval in minutes between reminder scheduler ticks. Default: 1.</summary>
    public int TickIntervalMinutes { get; set; } = 1;

    /// <summary>Default hour (0-23) for daily reminder if patient has no preferred time.</summary>
    public int DefaultHourUtc { get; set; } = 9;

    /// <summary>Default minute for daily reminder.</summary>
    public int DefaultMinuteUtc { get; set; } = 0;

    /// <summary>Maximum retry attempts when sending a reminder fails.</summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>Base delay in seconds before retrying a failed reminder.</summary>
    public int RetryBaseDelaySeconds { get; set; } = 30;
}