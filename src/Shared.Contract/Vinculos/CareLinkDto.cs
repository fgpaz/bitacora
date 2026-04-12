namespace Shared.Contract.Vinculos;

/// <summary>
/// Response DTO for an active care link returned to the API layer.
/// </summary>
public sealed record CareLinkDto(
    Guid CareLinkId,
    Guid ProfessionalId,
    string ProfessionalDisplayName,
    Guid PatientId,
    string Status,
    bool CanViewData,
    DateTime InvitedAt,
    DateTime? AcceptedAt,
    DateTime? RevokedAt);

/// <summary>
/// Response DTO for a binding code generated for a professional.
/// </summary>
public sealed record BindingCodeDto(
    Guid BindingCodeId,
    string Code,
    Guid ProfessionalId,
    string TtlPreset,
    DateTime ExpiresAt,
    DateTime CreatedAtUtc);
