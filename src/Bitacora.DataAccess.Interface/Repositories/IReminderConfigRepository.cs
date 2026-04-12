using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

/// <summary>
/// Repository for ReminderConfig persistence (RF-TG-010..012).
/// </summary>
public interface IReminderConfigRepository
{
    /// <summary>Gets a reminder config by its ID.</summary>
    Task<ReminderConfig?> GetByIdAsync(Guid reminderConfigId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new reminder config.</summary>
    Task AddAsync(ReminderConfig reminderConfig, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing reminder config.</summary>
    Task UpdateAsync(ReminderConfig reminderConfig, CancellationToken cancellationToken = default);

    /// <summary>Returns all enabled reminder configs with NextFireAtUtc on or before the given UTC now.</summary>
    Task<IReadOnlyList<ReminderConfig>> GetDueRemindersAsync(DateTime asOfUtc, CancellationToken cancellationToken = default);
}