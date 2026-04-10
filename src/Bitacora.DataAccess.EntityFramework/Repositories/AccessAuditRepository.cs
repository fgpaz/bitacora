using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class AccessAuditRepository(AppDbContext dbContext) : IAccessAuditRepository
{
    public async ValueTask AddAsync(AccessAudit accessAudit, CancellationToken cancellationToken = default)
    {
        await dbContext.AccessAudits.AddAsync(accessAudit, cancellationToken);
    }
}
