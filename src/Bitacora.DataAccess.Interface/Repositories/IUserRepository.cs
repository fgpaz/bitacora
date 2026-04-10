using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    ValueTask<User?> GetBySupabaseUserIdAsync(string supabaseUserId, CancellationToken cancellationToken = default);
    ValueTask AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}
