namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class MoodEntry
{
    public Guid MoodEntryId { get; private set; }
    public Guid PatientId { get; private set; }
    public byte[] EncryptedPayload { get; private set; } = [];
    public string SafeProjection { get; private set; } = "{}";
    public int KeyVersion { get; private set; }
    public DateTime EncryptedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private MoodEntry()
    {
    }

    private MoodEntry(
        Guid patientId,
        byte[] encryptedPayload,
        string safeProjection,
        int keyVersion,
        DateTime encryptedAtUtc,
        DateTime createdAtUtc)
    {
        MoodEntryId = Guid.NewGuid();
        PatientId = patientId;
        EncryptedPayload = encryptedPayload;
        SafeProjection = safeProjection;
        KeyVersion = keyVersion;
        EncryptedAt = encryptedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public static MoodEntry Create(
        Guid patientId,
        byte[] encryptedPayload,
        string safeProjection,
        int keyVersion,
        DateTime encryptedAtUtc,
        DateTime createdAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        if (encryptedPayload.Length == 0)
        {
            throw new ArgumentException("Encrypted payload is required.", nameof(encryptedPayload));
        }

        if (string.IsNullOrWhiteSpace(safeProjection))
        {
            throw new ArgumentException("Safe projection is required.", nameof(safeProjection));
        }

        return new MoodEntry(patientId, encryptedPayload, safeProjection, keyVersion, encryptedAtUtc, createdAtUtc);
    }
}
