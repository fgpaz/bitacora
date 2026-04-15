using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using Shared.Contract.Visualizacion;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Visualizacion;

/// <summary>
/// Returns the combined timeline (mood entries + daily check-ins) for a patient
/// within a date range, ordered chronologically. Privacy-aware: only safe projections.
/// </summary>
public readonly record struct GetPatientTimelineQuery(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    Guid ActorId,
    Guid TraceId) : IQuery<GetPatientTimelineResponse>;

public sealed record GetPatientTimelineResponse(IReadOnlyList<TimelineDayDto> Days);

public sealed class GetPatientTimelineQueryHandler(
    IMoodEntryRepository moodEntryRepository,
    IDailyCheckinRepository dailyCheckinRepository)
    : IQueryHandler<GetPatientTimelineQuery, GetPatientTimelineResponse>
{
    public async ValueTask<GetPatientTimelineResponse> Handle(
        GetPatientTimelineQuery query,
        CancellationToken cancellationToken)
    {
        var moodEntries = await moodEntryRepository.GetByPatientAndDateRangeAsync(
            query.PatientId, query.From, query.To, cancellationToken);

        var dailyCheckins = await dailyCheckinRepository.GetByPatientAndDateRangeAsync(
            query.PatientId, query.From, query.To, cancellationToken);

        var moodByDate = moodEntries
            .Select(e => new { Date = DateOnly.FromDateTime(e.CreatedAtUtc), Entry = e })
            .ToLookup(x => x.Date, x => x.Entry);

        var checkinByDate = dailyCheckins
            .ToLookup(c => c.CheckinDate, c => c);

        var allDates = Enumerable
            .Range(0, query.To.DayNumber - query.From.DayNumber + 1)
            .Select(offset => query.From.AddDays(offset))
            .ToList();

        var timeline = allDates.Select(date =>
        {
            var moodEntry = moodByDate[date].FirstOrDefault();
            var checkin = checkinByDate[date].FirstOrDefault();

            return new TimelineDayDto(
                date,
                moodEntry is not null ? ToDto(moodEntry) : null,
                checkin is not null ? ToDto(checkin) : null);
        }).ToList();

        return new GetPatientTimelineResponse(timeline);
    }

    private static MoodEntryDto ToDto(MoodEntry entry)
    {
        int? score = null;
        using var doc = JsonDocument.Parse(entry.SafeProjection);
        if (doc.RootElement.TryGetProperty("mood_score", out var el) && el.ValueKind == JsonValueKind.Number)
        {
            score = el.GetInt32();
        }

        return new MoodEntryDto(entry.MoodEntryId, score, entry.CreatedAtUtc);
    }

    private static DailyCheckinDto ToDto(DailyCheckin checkin)
    {
        decimal? sleep = null;
        bool? physical = null, social = null, anxiety = null, irritability = null, medication = null;

        using var doc = JsonDocument.Parse(checkin.SafeProjection);
        var root = doc.RootElement;

        if (root.TryGetProperty("sleep_hours", out var sh) && sh.ValueKind == JsonValueKind.Number)
            sleep = sh.GetDecimal();
        if (root.TryGetProperty("has_physical", out var pa) && pa.ValueKind == JsonValueKind.True)
            physical = true;
        else if (pa.ValueKind == JsonValueKind.False)
            physical = false;
        if (root.TryGetProperty("has_social", out var sa) && sa.ValueKind == JsonValueKind.True)
            social = true;
        else if (sa.ValueKind == JsonValueKind.False)
            social = false;
        if (root.TryGetProperty("has_anxiety", out var ax) && ax.ValueKind == JsonValueKind.True)
            anxiety = true;
        else if (ax.ValueKind == JsonValueKind.False)
            anxiety = false;
        if (root.TryGetProperty("has_irritability", out var ir) && ir.ValueKind == JsonValueKind.True)
            irritability = true;
        else if (ir.ValueKind == JsonValueKind.False)
            irritability = false;
        if (root.TryGetProperty("has_medication", out var mt) && mt.ValueKind == JsonValueKind.True)
            medication = true;
        else if (mt.ValueKind == JsonValueKind.False)
            medication = false;

        return new DailyCheckinDto(
            checkin.DailyCheckinId,
            checkin.CheckinDate,
            sleep,
            physical,
            social,
            anxiety,
            irritability,
            medication);
    }
}