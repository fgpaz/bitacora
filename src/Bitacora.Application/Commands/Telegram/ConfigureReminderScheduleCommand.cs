using Mediator;
using NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Command to configure or reschedule reminders for authenticated patient (RF-TG-010..012).
/// Supports timezone configuration to convert UTC schedule to patient-local timezone.
/// </summary>
public sealed record ConfigureReminderScheduleCommand(
    Guid PatientId,
    int HourUtc,
    int MinuteUtc,
    string? Timezone,
    Guid TraceId) : IRequest<ConfigureReminderScheduleResponse>;
