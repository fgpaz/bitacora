using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using Shared.Contract.Export;
using Shared.Contract.Visualizacion;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Export;

/// <summary>
/// Produces a full structured export of a patient's mood and check-in data for a date range.
/// Privacy-aware: only safe projections. Owner-only (patientId comes from authenticated session).
/// </summary>
public readonly record struct ExportPatientSummaryQuery(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    Guid ActorId,
    Guid TraceId) : IQuery<PatientExportDto>;

public sealed class ExportPatientSummaryQueryHandler(
    IMoodEntryRepository moodEntryRepository,
    IDailyCheckinRepository dailyCheckinRepository)
    : IQueryHandler<ExportPatientSummaryQuery, PatientExportDto>
{
    public async ValueTask<PatientExportDto> Handle(
        ExportPatientSummaryQuery query,
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

        var entries = new List<PatientExportEntryDto>();
        var anxietyDays = 0;
        var irritabilityDays = 0;
        var medicationTakenDays = 0;
        var sleepHours = new List<decimal>();
        var moodScores = new List<int>();

        foreach (var date in allDates)
        {
            var mood = moodByDate[date].FirstOrDefault();
            var checkin = checkinByDate[date].FirstOrDefault();

            MoodExportDto? moodDto = null;
            if (mood is not null)
            {
                int? score = null;
                using (var doc = JsonDocument.Parse(mood.SafeProjection))
                {
                    if (doc.RootElement.TryGetProperty("mood_score", out var el) && el.ValueKind == JsonValueKind.Number)
                    {
                        score = el.GetInt32();
                        moodScores.Add(el.GetInt32());
                    }
                }
                moodDto = new MoodExportDto(score, mood.CreatedAtUtc);
            }

            CheckinExportDto? checkinDto = null;
            if (checkin is not null)
            {
                decimal? sleep = null;
                bool? physical = null, social = null, anxiety = null, irritability = null, medication = null;

                using (var doc = JsonDocument.Parse(checkin.SafeProjection))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("sleep_hours", out var sh) && sh.ValueKind == JsonValueKind.Number)
                    {
                        sleep = sh.GetDecimal();
                        sleepHours.Add(sh.GetDecimal());
                    }
                    if (root.TryGetProperty("physical_activity", out var pa))
                        physical = pa.ValueKind == JsonValueKind.True ? true : pa.ValueKind == JsonValueKind.False ? false : null;
                    if (root.TryGetProperty("social_activity", out var sa))
                        social = sa.ValueKind == JsonValueKind.True ? true : sa.ValueKind == JsonValueKind.False ? false : null;
                    if (root.TryGetProperty("anxiety", out var ax))
                    {
                        anxiety = ax.ValueKind == JsonValueKind.True ? true : ax.ValueKind == JsonValueKind.False ? false : null;
                        if (anxiety == true) anxietyDays++;
                    }
                    if (root.TryGetProperty("irritability", out var ir))
                    {
                        irritability = ir.ValueKind == JsonValueKind.True ? true : ir.ValueKind == JsonValueKind.False ? false : null;
                        if (irritability == true) irritabilityDays++;
                    }
                    if (root.TryGetProperty("medication_taken", out var mt))
                    {
                        medication = mt.ValueKind == JsonValueKind.True ? true : mt.ValueKind == JsonValueKind.False ? false : null;
                        if (medication == true) medicationTakenDays++;
                    }
                }

                TimeOnly? medTime = null;
                if (checkin is not null)
                {
                    // medication_time is stored in the entity, read if present
                    // The SafeProjection may contain it; for now leave as null since DailyCheckin entity
                    // does not expose a direct property for this
                }

                checkinDto = new CheckinExportDto(sleep, physical, social, anxiety, irritability, medication, medTime);
            }

            entries.Add(new PatientExportEntryDto(date, moodDto, checkinDto));
        }

        var summary = new PatientExportSummaryDto(
            allDates.Count,
            moodEntries.Count,
            dailyCheckins.Count,
            moodScores.Count > 0 ? (decimal)moodScores.Average() : null,
            sleepHours.Count > 0 ? sleepHours.Average() : null,
            anxietyDays,
            irritabilityDays,
            medicationTakenDays);

        return new PatientExportDto(
            query.PatientId,
            query.From,
            query.To,
            DateTime.UtcNow,
            summary,
            entries);
    }
}