namespace NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

/// <summary>
/// Request to configure reminder schedule for authenticated patient (RF-TG-006).
/// HourUtc and MinuteUtc are UTC fields; the patient UI converts local Buenos Aires time before sending.
/// </summary>
public sealed record ConfigureReminderScheduleRequest(
    int HourUtc,
    int MinuteUtc,
    string? Timezone = null);
