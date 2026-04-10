using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class PendingInviteRepository(AppDbContext dbContext) : IPendingInviteRepository
{
    public async ValueTask<PendingInvite?> FindResumableByTokenAndEmailHashAsync(
        string inviteToken,
        string emailHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.PendingInvites.FirstOrDefaultAsync(
            x => x.InviteToken == inviteToken &&
                 x.InviteeEmailHash == emailHash &&
                 x.Status == PendingInviteStatus.Issued &&
                 x.ExpiresAt >= nowUtc,
            cancellationToken);
    }
}
