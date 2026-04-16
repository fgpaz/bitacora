namespace NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

/// <summary>
/// Request to configure reminder schedule for authenticated patient (RF-TG-010..012).
/// </summary>
public sealed record ConfigureReminderScheduleRequest(
    int HourUtc,
    int MinuteUtc,
    string? Timezone = null);
