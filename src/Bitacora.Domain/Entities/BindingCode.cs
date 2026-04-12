using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class BindingCode
{
    public Guid BindingCodeId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public Guid ProfessionalId { get; private set; }
    public string TtlPreset { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool Used { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private BindingCode()
    {
    }

    private BindingCode(
        Guid bindingCodeId,
        string code,
        Guid professionalId,
        string ttlPreset,
        DateTime expiresAt,
        bool used,
        DateTime createdAtUtc)
    {
        BindingCodeId = bindingCodeId;
        Code = code;
        ProfessionalId = professionalId;
        TtlPreset = ttlPreset;
        ExpiresAt = expiresAt;
        Used = used;
        CreatedAtUtc = createdAtUtc;
    }

    public static BindingCode Create(
        string code,
        Guid professionalId,
        string ttlPreset,
        DateTime expiresAt,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException("Professional id is required.", nameof(professionalId));
        }

        if (string.IsNullOrWhiteSpace(ttlPreset))
        {
            throw new ArgumentException("TTL preset is required.", nameof(ttlPreset));
        }

        return new BindingCode(
            Guid.NewGuid(),
            code.Trim().ToUpperInvariant(),
            professionalId,
            ttlPreset.Trim(),
            expiresAt,
            used: false,
            createdAtUtc);
    }

    public void MarkAsUsed()
    {
        Used = true;
    }
}
