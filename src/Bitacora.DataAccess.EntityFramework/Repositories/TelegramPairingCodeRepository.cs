using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class TelegramPairingCodeRepository(AppDbContext dbContext) : ITelegramPairingCodeRepository
{
    public async Task<TelegramPairingCode?> FindValidByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        return await dbContext.TelegramPairingCodes
            .FirstOrDefaultAsync(
                x => x.Code == code.ToUpperInvariant() &&
                     !x.Used &&
                     x.ExpiresAt > nowUtc,
                cancellationToken);
    }

    public async Task<TelegramPairingCode?> FindActiveByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        return await dbContext.TelegramPairingCodes
            .FirstOrDefaultAsync(
                x => x.PatientId == patientId &&
                     !x.Used &&
                     x.ExpiresAt > nowUtc,
                cancellationToken);
    }

    public async Task AddAsync(TelegramPairingCode pairingCode, CancellationToken cancellationToken = default)
    {
        await dbContext.TelegramPairingCodes.AddAsync(pairingCode, cancellationToken);
    }

    public Task UpdateAsync(TelegramPairingCode pairingCode, CancellationToken cancellationToken = default)
    {
        dbContext.TelegramPairingCodes.Update(pairingCode);
        return Task.CompletedTask;
    }
}
