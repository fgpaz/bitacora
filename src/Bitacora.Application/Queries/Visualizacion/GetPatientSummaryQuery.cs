using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using Shared.Contract.Visualizacion;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Visualizacion;

/// <summary>
/// Returns an aggregated summary of mood and check-in data for a patient within a date range.
/// Privacy-aware: only safe projections.
/// </summary>
public readonly record struct GetPatientSummaryQuery(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    Guid ActorId,
    Guid TraceId) : IQuery<PatientSummaryDto>;

public sealed class GetPatientSummaryQueryHandler(
    IMoodEntryRepository moodEntryRepository,
    IDailyCheckinRepository dailyCheckinRepository)
    : IQueryHandler<GetPatientSummaryQuery, PatientSummaryDto>
{
    public async ValueTask<PatientSummaryDto> Handle(
        GetPatientSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var moodEntries = await moodEntryRepository.GetByPatientAndDateRangeAsync(
            query.PatientId, query.From, query.To, cancellationToken);

        var dailyCheckins = await dailyCheckinRepository.GetByPatientAndDateRangeAsync(
            query.PatientId, query.From, query.To, cancellationToken);

        var totalDays = query.To.DayNumber - query.From.DayNumber + 1;
        var daysWithMood = moodEntries.Count;
        var daysWithCheckin = dailyCheckins.Count;

        var moodScores = new List<int>();
        var sleepHours = new List<decimal>();
        var anxietyDays = 0;
        var irritabilityDays = 0;
        var medicationTakenDays = 0;

        foreach (var entry in moodEntries)
        {
            using var doc = JsonDocument.Parse(entry.SafeProjection);
            if (doc.RootElement.TryGetProperty("mood_score", out var el) && el.ValueKind == JsonValueKind.Number)
            {
                moodScores.Add(el.GetInt32());
            }
        }

        foreach (var checkin in dailyCheckins)
        {
            using var doc = JsonDocument.Parse(checkin.SafeProjection);
            var root = doc.RootElement;

            if (root.TryGetProperty("sleep_hours", out var sh) && sh.ValueKind == JsonValueKind.Number)
                sleepHours.Add(sh.GetDecimal());

            if (root.TryGetProperty("has_anxiety", out var ax) && ax.ValueKind == JsonValueKind.True)
                anxietyDays++;
            if (root.TryGetProperty("has_irritability", out var ir) && ir.ValueKind == JsonValueKind.True)
                irritabilityDays++;
            if (root.TryGetProperty("has_medication", out var mt) && mt.ValueKind == JsonValueKind.True)
                medicationTakenDays++;
        }

        decimal? avgMood = moodScores.Count > 0 ? (decimal)moodScores.Average() : null;
        decimal? avgSleep = sleepHours.Count > 0 ? sleepHours.Average() : null;

        return new PatientSummaryDto(
            query.PatientId,
            query.From,
            query.To,
            totalDays,
            daysWithMood,
            daysWithCheckin,
            avgMood,
            avgSleep,
            anxietyDays,
            irritabilityDays,
            medicationTakenDays);
    }
}