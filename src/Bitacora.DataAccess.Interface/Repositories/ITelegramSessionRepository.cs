using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

/// <summary>
/// Repository for TelegramSession persistence (RF-TG-001..003).
/// </summary>
public interface ITelegramSessionRepository
{
    /// <summary>Gets a Telegram session by its ID.</summary>
    Task<TelegramSession?> GetByIdAsync(Guid telegramSessionId, CancellationToken cancellationToken = default);

    /// <summary>Finds a linked session by patient ID.</summary>
    Task<TelegramSession?> FindLinkedByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    /// <summary>Finds a linked session by Telegram chat ID.</summary>
    Task<TelegramSession?> FindLinkedByChatIdAsync(string chatId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new Telegram session.</summary>
    Task AddAsync(TelegramSession session, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Telegram session.</summary>
    Task UpdateAsync(TelegramSession session, CancellationToken cancellationToken = default);
}
