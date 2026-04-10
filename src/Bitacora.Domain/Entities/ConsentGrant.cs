using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class ConsentGrant
{
    public Guid ConsentGrantId { get; private set; }
    public Guid PatientId { get; private set; }
    public string ConsentVersion { get; private set; } = string.Empty;
    public ConsentStatus Status { get; private set; }
    public DateTime? GrantedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ConsentGrant()
    {
    }

    private ConsentGrant(Guid patientId, string consentVersion, DateTime createdAtUtc)
    {
        ConsentGrantId = Guid.NewGuid();
        PatientId = patientId;
        ConsentVersion = consentVersion;
        Status = ConsentStatus.Granted;
        GrantedAt = createdAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public static ConsentGrant CreateGranted(Guid patientId, string consentVersion, DateTime createdAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(consentVersion))
        {
            throw new ArgumentException("Consent version is required.", nameof(consentVersion));
        }

        return new ConsentGrant(patientId, consentVersion.Trim(), createdAtUtc);
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        Status = ConsentStatus.Revoked;
        RevokedAt = revokedAtUtc;
    }
}
