using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IPendingInviteRepository
{
    ValueTask<PendingInvite?> FindResumableByTokenAndEmailHashAsync(string inviteToken, string emailHash, DateTime nowUtc, CancellationToken cancellationToken = default);
}
