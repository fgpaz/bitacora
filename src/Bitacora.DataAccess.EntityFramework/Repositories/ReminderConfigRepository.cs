using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class ReminderConfigRepository(AppDbContext dbContext) : IReminderConfigRepository
{
    public async Task<ReminderConfig?> GetByIdAsync(Guid reminderConfigId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ReminderConfigs
            .FirstOrDefaultAsync(x => x.ReminderConfigId == reminderConfigId, cancellationToken);
    }

    public async Task<ReminderConfig?> FindByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ReminderConfigs
            .FirstOrDefaultAsync(x => x.PatientId == patientId, cancellationToken);
    }

    public async Task AddAsync(ReminderConfig reminderConfig, CancellationToken cancellationToken = default)
    {
        await dbContext.ReminderConfigs.AddAsync(reminderConfig, cancellationToken);
    }

    public Task UpdateAsync(ReminderConfig reminderConfig, CancellationToken cancellationToken = default)
    {
        dbContext.ReminderConfigs.Update(reminderConfig);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<ReminderConfig>> GetDueRemindersAsync(DateTime asOfUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.ReminderConfigs
            .Where(x => x.Enabled && x.NextFireAtUtc != null && x.NextFireAtUtc <= asOfUtc)
            .ToListAsync(cancellationToken);
    }
}
