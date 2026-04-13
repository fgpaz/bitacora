using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class TelegramSessionRepository(AppDbContext dbContext) : ITelegramSessionRepository
{
    public async Task<TelegramSession?> GetByIdAsync(Guid telegramSessionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TelegramSessions
            .FirstOrDefaultAsync(x => x.TelegramSessionId == telegramSessionId, cancellationToken);
    }

    public async Task<TelegramSession?> FindLinkedByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TelegramSessions
            .FirstOrDefaultAsync(
                x => x.PatientId == patientId && x.Status == TelegramSessionStatus.Linked,
                cancellationToken);
    }

    public async Task<TelegramSession?> FindLinkedByChatIdAsync(string chatId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TelegramSessions
            .FirstOrDefaultAsync(
                x => x.ChatId == chatId && x.Status == TelegramSessionStatus.Linked,
                cancellationToken);
    }

    public async Task<TelegramSession?> GetByChatIdAsync(string chatId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TelegramSessions
            .FirstOrDefaultAsync(
                x => x.ChatId == chatId && x.Status == TelegramSessionStatus.Linked,
                cancellationToken);
    }

    public async Task AddAsync(TelegramSession session, CancellationToken cancellationToken = default)
    {
        await dbContext.TelegramSessions.AddAsync(session, cancellationToken);
    }

    public Task UpdateAsync(TelegramSession session, CancellationToken cancellationToken = default)
    {
        dbContext.TelegramSessions.Update(session);
        return Task.CompletedTask;
    }
}
