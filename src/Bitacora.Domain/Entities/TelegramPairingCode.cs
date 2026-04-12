using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// A temporary pairing code used to link a Telegram chat to a patient account (RF-TG-001..003).
/// Format: BIT-XXXXX (5 alphanumeric chars). TTL: 15 minutes.
/// One active code per patient at a time; generating a new one invalidates the previous.
/// </summary>
public sealed class TelegramPairingCode
{
    public Guid TelegramPairingCodeId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool Used { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private TelegramPairingCode()
    {
    }

    private TelegramPairingCode(
        Guid telegramPairingCodeId,
        string code,
        Guid patientId,
        DateTime expiresAt,
        bool used,
        DateTime? consumedAt,
        DateTime createdAtUtc)
    {
        TelegramPairingCodeId = telegramPairingCodeId;
        Code = code;
        PatientId = patientId;
        ExpiresAt = expiresAt;
        Used = used;
        ConsumedAt = consumedAt;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Creates a new pairing code with format BIT-XXXXX and 15-minute TTL.
    /// </summary>
    public static TelegramPairingCode Create(
        string code,
        Guid patientId,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        const int ttlMinutes = 15;
        var expiresAt = createdAtUtc.AddMinutes(ttlMinutes);

        return new TelegramPairingCode(
            Guid.NewGuid(),
            code.Trim().ToUpperInvariant(),
            patientId,
            expiresAt,
            used: false,
            consumedAt: null,
            createdAtUtc);
    }

    /// <summary>
    /// Marks the code as consumed atomically.
    /// </summary>
    public void MarkAsUsed(DateTime consumedAtUtc)
    {
        if (Used)
        {
            throw new InvalidOperationException("Pairing code has already been used.");
        }

        Used = true;
        ConsumedAt = consumedAtUtc;
    }

    /// <summary>
    /// Returns true if the code is valid: not used and not expired.
    /// </summary>
    public bool IsValid(DateTime nowUtc) => !Used && ExpiresAt > nowUtc;
}
