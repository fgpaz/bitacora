using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface ICareLinkRepository
{
    Task<CareLink?> GetByIdAsync(Guid careLinkId, CancellationToken cancellationToken = default);
    Task<CareLink?> FindActiveByPatientAndProfessionalAsync(Guid patientId, Guid professionalId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CareLink>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CareLink>> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default);
    Task AddAsync(CareLink careLink, CancellationToken cancellationToken = default);
    Task UpdateAsync(CareLink careLink, CancellationToken cancellationToken = default);
}
