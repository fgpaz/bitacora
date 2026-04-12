using MassTransit;

namespace Shared.Contract.Events;

/// <summary>
/// Published when a patient accepts a care link invitation and the link becomes Active.
/// Used by other services to grant/refresh professional access to patient data.
/// </summary>
[EntityName("CareLinkActivated")]
public sealed record CareLinkActivatedEvent(
    Guid CareLinkId,
    Guid ProfessionalId,
    Guid PatientId,
    bool CanViewData,
    DateTime ActivatedAtUtc,
    Guid CorrelationId);

/// <summary>
/// Published when a care link is revoked (by patient or by consent revocation).
/// Other services should immediately revoke any derived access grants.
/// </summary>
[EntityName("CareLinkRevoked")]
public sealed record CareLinkRevokedEvent(
    Guid CareLinkId,
    Guid ProfessionalId,
    Guid PatientId,
    string RevocationReason,
    DateTime RevokedAtUtc,
    Guid CorrelationId);

/// <summary>
/// Published when a patient updates the can_view_data flag on an active care link.
/// </summary>
[EntityName("CareLinkCanViewDataUpdated")]
public sealed record CareLinkCanViewDataUpdatedEvent(
    Guid CareLinkId,
    Guid ProfessionalId,
    Guid PatientId,
    bool CanViewData,
    DateTime UpdatedAtUtc,
    Guid CorrelationId);
