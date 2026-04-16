namespace NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;

/// <summary>
/// Response for unlink telegram session operation.
/// </summary>
public sealed record UnlinkTelegramSessionResponse
{
    /// <summary>
    /// Gets the patient ID of the unlinked session.
    /// </summary>
    public required Guid PatientId { get; init; }

    /// <summary>
    /// Gets the unlink timestamp (UTC).
    /// </summary>
    public required DateTime UnlinkedAtUtc { get; init; }
}
