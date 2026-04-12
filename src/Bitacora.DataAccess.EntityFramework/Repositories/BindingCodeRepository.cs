using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class BindingCodeRepository(AppDbContext dbContext) : IBindingCodeRepository
{
    public async Task<BindingCode?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await dbContext.BindingCodes
            .FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<BindingCode?> FindActiveByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BindingCodes
            .FirstOrDefaultAsync(x => x.ProfessionalId == professionalId && !x.Used && x.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task AddAsync(BindingCode bindingCode, CancellationToken cancellationToken = default)
    {
        await dbContext.BindingCodes.AddAsync(bindingCode, cancellationToken);
    }

    public Task UpdateAsync(BindingCode bindingCode, CancellationToken cancellationToken = default)
    {
        dbContext.BindingCodes.Update(bindingCode);
        return Task.CompletedTask;
    }
}
