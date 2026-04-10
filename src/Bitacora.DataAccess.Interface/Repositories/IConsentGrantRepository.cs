using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IConsentGrantRepository
{
    ValueTask<ConsentGrant?> GetActiveByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    ValueTask<ConsentGrant?> GetLatestByPatientAndVersionAsync(Guid patientId, string consentVersion, CancellationToken cancellationToken = default);
    ValueTask AddAsync(ConsentGrant consentGrant, CancellationToken cancellationToken = default);
    void Update(ConsentGrant consentGrant);
}
