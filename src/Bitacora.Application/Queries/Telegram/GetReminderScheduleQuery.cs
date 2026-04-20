using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Telegram;

/// <summary>
/// Returns the authenticated patient's Telegram reminder schedule without exposing Telegram PII.
/// </summary>
public readonly record struct GetReminderScheduleQuery(
    Guid PatientId,
    Guid TraceId) : IQuery<GetReminderScheduleResponse>;

public sealed record GetReminderScheduleResponse(
    bool Configured,
    Guid? ReminderConfigId,
    int? HourUtc,
    int? MinuteUtc,
    string? ReminderTimezone,
    bool? Enabled,
    DateTime? NextFireAtUtc);

public sealed class GetReminderScheduleQueryHandler(
    IReminderConfigRepository reminderConfigRepository)
    : IQueryHandler<GetReminderScheduleQuery, GetReminderScheduleResponse>
{
    public async ValueTask<GetReminderScheduleResponse> Handle(
        GetReminderScheduleQuery query,
        CancellationToken cancellationToken)
    {
        var config = await reminderConfigRepository.FindByPatientIdAsync(query.PatientId, cancellationToken);

        if (config == null)
        {
            return new GetReminderScheduleResponse(
                Configured: false,
                ReminderConfigId: null,
                HourUtc: null,
                MinuteUtc: null,
                ReminderTimezone: null,
                Enabled: null,
                NextFireAtUtc: null);
        }

        return new GetReminderScheduleResponse(
            Configured: true,
            ReminderConfigId: config.ReminderConfigId,
            HourUtc: config.HourUtc,
            MinuteUtc: config.MinuteUtc,
            ReminderTimezone: config.ReminderTimezone,
            Enabled: config.Enabled,
            NextFireAtUtc: config.NextFireAtUtc);
    }
}
