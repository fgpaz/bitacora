using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class ConsentGrantRepository(AppDbContext dbContext) : IConsentGrantRepository
{
    public async ValueTask<ConsentGrant?> GetActiveByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ConsentGrants
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.Status == ConsentStatus.Granted, cancellationToken);
    }

    public async ValueTask<ConsentGrant?> GetLatestByPatientAndVersionAsync(Guid patientId, string consentVersion, CancellationToken cancellationToken = default)
    {
        return await dbContext.ConsentGrants
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.ConsentVersion == consentVersion, cancellationToken);
    }

    public async ValueTask AddAsync(ConsentGrant consentGrant, CancellationToken cancellationToken = default)
    {
        await dbContext.ConsentGrants.AddAsync(consentGrant, cancellationToken);
    }

    public void Update(ConsentGrant consentGrant)
    {
        dbContext.ConsentGrants.Update(consentGrant);
    }
}
