using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    ValueTask<User?> GetByAuthSubjectAsync(string authSubject, CancellationToken cancellationToken = default);
    ValueTask<User?> GetByEmailHashAsync(string emailHash, CancellationToken cancellationToken = default);
    ValueTask AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}
