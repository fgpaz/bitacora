using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IDailyCheckinRepository
{
    ValueTask<DailyCheckin?> GetByPatientAndDateAsync(Guid patientId, DateOnly checkinDate, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyList<DailyCheckin>> GetByPatientAndDateRangeAsync(Guid patientId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    ValueTask AddAsync(DailyCheckin dailyCheckin, CancellationToken cancellationToken = default);
    void Update(DailyCheckin dailyCheckin);
}