using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IAccessAuditRepository
{
    ValueTask AddAsync(AccessAudit accessAudit, CancellationToken cancellationToken = default);
}
