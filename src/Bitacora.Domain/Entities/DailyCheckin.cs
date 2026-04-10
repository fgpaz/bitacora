namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class DailyCheckin
{
    public Guid DailyCheckinId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateOnly CheckinDate { get; private set; }
    public byte[] EncryptedPayload { get; private set; } = [];
    public string SafeProjection { get; private set; } = "{}";
    public int KeyVersion { get; private set; }
    public DateTime EncryptedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private DailyCheckin()
    {
    }

    private DailyCheckin(
        Guid patientId,
        DateOnly checkinDate,
        byte[] encryptedPayload,
        string safeProjection,
        int keyVersion,
        DateTime encryptedAtUtc,
        DateTime createdAtUtc)
    {
        DailyCheckinId = Guid.NewGuid();
        PatientId = patientId;
        CheckinDate = checkinDate;
        EncryptedPayload = encryptedPayload;
        SafeProjection = safeProjection;
        KeyVersion = keyVersion;
        EncryptedAt = encryptedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public static DailyCheckin Create(
        Guid patientId,
        DateOnly checkinDate,
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

        return new DailyCheckin(patientId, checkinDate, encryptedPayload, safeProjection, keyVersion, encryptedAtUtc, createdAtUtc);
    }

    public void UpdatePayload(byte[] encryptedPayload, string safeProjection, int keyVersion, DateTime encryptedAtUtc, DateTime updatedAtUtc)
    {
        EncryptedPayload = encryptedPayload;
        SafeProjection = safeProjection;
        KeyVersion = keyVersion;
        EncryptedAt = encryptedAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }
}
