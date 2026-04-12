using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class CareLinkRepository(AppDbContext dbContext) : ICareLinkRepository
{
    public async Task<CareLink?> GetByIdAsync(Guid careLinkId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CareLinks
            .FirstOrDefaultAsync(x => x.CareLinkId == careLinkId, cancellationToken);
    }

    public async Task<CareLink?> FindActiveByPatientAndProfessionalAsync(
        Guid patientId,
        Guid professionalId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CareLinks
            .FirstOrDefaultAsync(
                x => x.PatientId == patientId &&
                     x.ProfessionalId == professionalId &&
                     (x.Status == CareLinkStatus.Invited || x.Status == CareLinkStatus.Active),
                cancellationToken);
    }

    public async Task<IReadOnlyList<CareLink>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CareLinks
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CareLink>> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CareLinks
            .Where(x => x.ProfessionalId == professionalId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CareLink careLink, CancellationToken cancellationToken = default)
    {
        await dbContext.CareLinks.AddAsync(careLink, cancellationToken);
    }

    public Task UpdateAsync(CareLink careLink, CancellationToken cancellationToken = default)
    {
        dbContext.CareLinks.Update(careLink);
        return Task.CompletedTask;
    }
}
