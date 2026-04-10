using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class DailyCheckinRepository(AppDbContext dbContext) : IDailyCheckinRepository
{
    public async ValueTask<DailyCheckin?> GetByPatientAndDateAsync(Guid patientId, DateOnly checkinDate, CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyCheckins.FirstOrDefaultAsync(
            x => x.PatientId == patientId && x.CheckinDate == checkinDate,
            cancellationToken);
    }

    public async ValueTask AddAsync(DailyCheckin dailyCheckin, CancellationToken cancellationToken = default)
    {
        await dbContext.DailyCheckins.AddAsync(dailyCheckin, cancellationToken);
    }

    public void Update(DailyCheckin dailyCheckin)
    {
        dbContext.DailyCheckins.Update(dailyCheckin);
    }
}
