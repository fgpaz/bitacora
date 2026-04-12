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

    private PendingInvite(
        Guid pendingInviteId,
        Guid professionalId,
        string inviteeEmailHash,
        string inviteToken,
        PendingInviteStatus status,
        DateTime expiresAt,
        DateTime? consumedAt,
        DateTime createdAtUtc)
    {
        PendingInviteId = pendingInviteId;
        ProfessionalId = professionalId;
        InviteeEmailHash = inviteeEmailHash;
        InviteToken = inviteToken;
        Status = status;
        ExpiresAt = expiresAt;
        ConsumedAt = consumedAt;
        CreatedAtUtc = createdAtUtc;
    }

    public static PendingInvite Create(
        Guid professionalId,
        string inviteeEmailHash,
        string inviteToken,
        DateTime expiresAt,
        DateTime createdAtUtc)
    {
        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException("Professional id is required.", nameof(professionalId));
        }

        if (string.IsNullOrWhiteSpace(inviteeEmailHash))
        {
            throw new ArgumentException("Invitee email hash is required.", nameof(inviteeEmailHash));
        }

        if (string.IsNullOrWhiteSpace(inviteToken))
        {
            throw new ArgumentException("Invite token is required.", nameof(inviteToken));
        }

        return new PendingInvite(
            Guid.NewGuid(),
            professionalId,
            inviteeEmailHash,
            inviteToken,
            PendingInviteStatus.Issued,
            expiresAt,
            consumedAt: null,
            createdAtUtc);
    }

    public void MarkAsConsumed(DateTime consumedAtUtc)
    {
        Status = PendingInviteStatus.Consumed;
        ConsumedAt = consumedAtUtc;
    }
}
