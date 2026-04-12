using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class MoodEntryRepository(AppDbContext dbContext) : IMoodEntryRepository
{
    public async ValueTask<MoodEntry?> FindDuplicateAsync(Guid patientId, int score, DateTime sinceUtc, CancellationToken cancellationToken = default)
    {
        var candidates = await dbContext.MoodEntries
            .AsNoTracking()
            .Where(x => x.PatientId == patientId && x.CreatedAtUtc >= sinceUtc)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            using var document = JsonDocument.Parse(candidate.SafeProjection);
            if (document.RootElement.TryGetProperty("mood_score", out var scoreElement) &&
                scoreElement.TryGetInt32(out var currentScore) &&
                currentScore == score)
            {
                return candidate;
            }
        }

        return null;
    }

    public async ValueTask<IReadOnlyList<MoodEntry>> GetByPatientAndDateRangeAsync(
        Guid patientId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
        var toDateTime = to.ToDateTime(TimeOnly.MaxValue);

        return await dbContext.MoodEntries
            .AsNoTracking()
            .Where(x => x.PatientId == patientId && x.CreatedAtUtc >= fromDateTime && x.CreatedAtUtc <= toDateTime)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask AddAsync(MoodEntry moodEntry, CancellationToken cancellationToken = default)
    {
        await dbContext.MoodEntries.AddAsync(moodEntry, cancellationToken);
    }
}