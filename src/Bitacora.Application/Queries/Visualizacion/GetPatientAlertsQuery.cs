using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using Shared.Contract.Visualizacion;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Visualizacion;

public readonly record struct GetPatientAlertsQuery(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    Guid ActorId,
    Guid TraceId) : IQuery<IReadOnlyList<PatientAlertDto>>;

public sealed class GetPatientAlertsQueryHandler(IMoodEntryRepository moodEntryRepository)
    : IQueryHandler<GetPatientAlertsQuery, IReadOnlyList<PatientAlertDto>>
{
    public async ValueTask<IReadOnlyList<PatientAlertDto>> Handle(GetPatientAlertsQuery query, CancellationToken cancellationToken)
    {
        var moodEntries = await moodEntryRepository.GetByPatientAndDateRangeAsync(
            query.PatientId,
            query.From,
            query.To,
            cancellationToken);

        var lowMoodDates = moodEntries
            .Select(entry =>
            {
                using var doc = JsonDocument.Parse(entry.SafeProjection);
                if (!doc.RootElement.TryGetProperty("mood_score", out var scoreEl) || scoreEl.ValueKind != JsonValueKind.Number)
                {
                    return (Date: (DateOnly?)null, Score: (int?)null);
                }

                return (Date: (DateOnly?)DateOnly.FromDateTime(entry.CreatedAtUtc), Score: (int?)scoreEl.GetInt32());
            })
            .Where(x => x.Date is not null && x.Score is <= -2)
            .Select(x => x.Date!.Value)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        if (lowMoodDates.Count < 3)
        {
            return [];
        }

        var streak = 1;
        DateOnly? lastDate = null;
        DateOnly? triggerDate = null;

        foreach (var date in lowMoodDates)
        {
            if (lastDate is not null && date.DayNumber == lastDate.Value.DayNumber + 1)
            {
                streak++;
            }
            else
            {
                streak = 1;
            }

            if (streak >= 3)
            {
                triggerDate = date;
            }

            lastDate = date;
        }

        if (triggerDate is null)
        {
            return [];
        }

        return
        [
            new PatientAlertDto(
                "LOW_MOOD_STREAK",
                "high",
                "Se detectó una racha de humor bajo sostenida.",
                triggerDate.Value,
                streak)
        ];
    }
}
