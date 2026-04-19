using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async ValueTask<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async ValueTask<User?> GetByAuthSubjectAsync(string authSubject, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.AuthSubject == authSubject, cancellationToken);
    }

    public async ValueTask<User?> GetByEmailHashAsync(string emailHash, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .OrderBy(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(x => x.EmailHash == emailHash, cancellationToken);
    }

    public async ValueTask AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        dbContext.Users.Update(user);
    }
}
