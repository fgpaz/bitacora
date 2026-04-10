using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IMoodEntryRepository
{
    ValueTask<MoodEntry?> FindDuplicateAsync(Guid patientId, int score, DateTime sinceUtc, CancellationToken cancellationToken = default);
    ValueTask AddAsync(MoodEntry moodEntry, CancellationToken cancellationToken = default);
}
