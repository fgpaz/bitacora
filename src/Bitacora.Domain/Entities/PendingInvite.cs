using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class PendingInvite
{
    public Guid PendingInviteId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public string InviteeEmailHash { get; private set; } = string.Empty;
    public string InviteToken { get; private set; } = string.Empty;
    public PendingInviteStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private PendingInvite()
    {
    }
}
