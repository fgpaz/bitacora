using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class CareLink
{
    public Guid CareLinkId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public Guid PatientId { get; private set; }
    public CareLinkStatus Status { get; private set; }
    public bool CanViewData { get; private set; }
    public DateTime InvitedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private CareLink()
    {
    }

    private CareLink(
        Guid careLinkId,
        Guid professionalId,
        Guid patientId,
        CareLinkStatus status,
        bool canViewData,
        DateTime invitedAt,
        DateTime? acceptedAt,
        DateTime? revokedAt,
        DateTime createdAtUtc)
    {
        CareLinkId = careLinkId;
        ProfessionalId = professionalId;
        PatientId = patientId;
        Status = status;
        CanViewData = canViewData;
        InvitedAt = invitedAt;
        AcceptedAt = acceptedAt;
        RevokedAt = revokedAt;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Creates a CareLink with can_view_data=false (RF-VIN-004 invariant).
    /// </summary>
    public static CareLink CreateInvited(
        Guid professionalId,
        Guid patientId,
        DateTime invitedAtUtc,
        DateTime createdAtUtc)
    {
        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException("Professional id is required.", nameof(professionalId));
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        return new CareLink(
            Guid.NewGuid(),
            professionalId,
            patientId,
            CareLinkStatus.Invited,
            canViewData: false,
            invitedAtUtc,
            acceptedAt: null,
            revokedAt: null,
            createdAtUtc);
    }

    /// <summary>
    /// Creates a CareLink directly in active status (auto-binding via BindingCode, RF-VIN-012).
    /// can_view_data=false by invariant T3-11.
    /// </summary>
    public static CareLink CreateActive(
        Guid professionalId,
        Guid patientId,
        DateTime nowUtc,
        DateTime createdAtUtc)
    {
        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException("Professional id is required.", nameof(professionalId));
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        return new CareLink(
            Guid.NewGuid(),
            professionalId,
            patientId,
            CareLinkStatus.Active,
            canViewData: false,
            nowUtc,
            acceptedAt: nowUtc,
            revokedAt: null,
            createdAtUtc);
    }

    public void Accept(DateTime acceptedAtUtc)
    {
        if (Status != CareLinkStatus.Invited)
        {
            throw new InvalidOperationException("Can only accept an invited CareLink.");
        }

        Status = CareLinkStatus.Active;
        AcceptedAt = acceptedAtUtc;
    }

    public void Revoke(DateTime revokedAtUtc, CareLinkStatus revokedStatus)
    {
        if (revokedAtUtc == default)
        {
            throw new ArgumentException("Revoked at is required.", nameof(revokedAtUtc));
        }

        if (Status != CareLinkStatus.Invited && Status != CareLinkStatus.Active)
        {
            throw new InvalidOperationException("Can only revoke an invited or active CareLink.");
        }

        Status = revokedStatus;
        RevokedAt = revokedAtUtc;
    }

    /// <summary>
    /// Only the patient owner may update can_view_data (RF-VIN-023).
    /// </summary>
    public void UpdateCanViewData(bool canViewData, DateTime utcNow)
    {
        if (utcNow == default)
        {
            throw new ArgumentException("UTC now is required.", nameof(utcNow));
        }

        if (Status != CareLinkStatus.Active)
        {
            throw new InvalidOperationException("Can only update can_view_data on an active CareLink.");
        }

        CanViewData = canViewData;
    }
}
