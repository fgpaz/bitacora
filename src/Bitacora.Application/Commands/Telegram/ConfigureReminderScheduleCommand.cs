using Mediator;
using NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Command to configure or reschedule reminders for authenticated patient (RF-TG-006).
/// HourUtc and MinuteUtc are persisted UTC fields; timezone preserves patient-local display context.
/// </summary>
public sealed record ConfigureReminderScheduleCommand(
    Guid PatientId,
    int HourUtc,
    int MinuteUtc,
    string? Timezone,
    Guid TraceId) : IRequest<ConfigureReminderScheduleResponse>;
