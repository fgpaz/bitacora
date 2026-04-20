namespace NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

/// <summary>
/// Response after configuring reminder schedule (RF-TG-006).
/// </summary>
public sealed record ConfigureReminderScheduleResponse(
    Guid ReminderConfigId,
    int HourUtc,
    int MinuteUtc,
    string ReminderTimezone,
    bool Enabled,
    DateTime? NextFireAtUtc);
