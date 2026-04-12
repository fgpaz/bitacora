using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

/// <summary>
/// Repository for TelegramPairingCode persistence (RF-TG-001..003).
/// </summary>
public interface ITelegramPairingCodeRepository
{
    /// <summary>Finds a valid (not used, not expired) pairing code by its value.</summary>
    Task<TelegramPairingCode?> FindValidByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Finds the active (not used, not expired) pairing code for a patient, if any.</summary>
    Task<TelegramPairingCode?> FindActiveByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new pairing code.</summary>
    Task AddAsync(TelegramPairingCode pairingCode, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing pairing code.</summary>
    Task UpdateAsync(TelegramPairingCode pairingCode, CancellationToken cancellationToken = default);
}
