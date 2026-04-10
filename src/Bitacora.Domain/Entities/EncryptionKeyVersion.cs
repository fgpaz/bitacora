namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class EncryptionKeyVersion
{
    public int KeyVersion { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsActive { get; private set; }

    private EncryptionKeyVersion()
    {
    }

    public EncryptionKeyVersion(int keyVersion, DateTime createdAtUtc, bool isActive)
    {
        KeyVersion = keyVersion;
        CreatedAtUtc = createdAtUtc;
        IsActive = isActive;
    }
}
