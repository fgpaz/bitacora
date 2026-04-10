using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; private set; }
    public string SupabaseUserId { get; private set; } = string.Empty;
    public byte[] EncryptedEmail { get; private set; } = [];
    public string EmailHash { get; private set; } = string.Empty;
    public int KeyVersion { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SessionsRevokedAt { get; private set; }

    private User()
    {
    }

    private User(
        Guid userId,
        string supabaseUserId,
        byte[] encryptedEmail,
        string emailHash,
        int keyVersion,
        UserRole role,
        UserStatus status,
        DateTime createdAtUtc)
    {
        UserId = userId;
        SupabaseUserId = supabaseUserId;
        EncryptedEmail = encryptedEmail;
        EmailHash = emailHash;
        KeyVersion = keyVersion;
        Role = role;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public static User CreatePatient(
        string supabaseUserId,
        byte[] encryptedEmail,
        string emailHash,
        int keyVersion,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(supabaseUserId))
        {
            throw new ArgumentException("Supabase user id is required.", nameof(supabaseUserId));
        }

        if (encryptedEmail.Length == 0)
        {
            throw new ArgumentException("Encrypted email is required.", nameof(encryptedEmail));
        }

        if (string.IsNullOrWhiteSpace(emailHash))
        {
            throw new ArgumentException("Email hash is required.", nameof(emailHash));
        }

        return new User(
            Guid.NewGuid(),
            supabaseUserId.Trim(),
            encryptedEmail,
            emailHash,
            keyVersion,
            UserRole.Patient,
            UserStatus.Registered,
            createdAtUtc);
    }

    public void MarkConsentGranted()
    {
        if (Status == UserStatus.Registered)
        {
            Status = UserStatus.ConsentGranted;
        }
    }

    public void MarkActive()
    {
        if (Status == UserStatus.ConsentGranted)
        {
            Status = UserStatus.Active;
        }
    }

    public void RevokeSessions(DateTime revokedAtUtc)
    {
        SessionsRevokedAt = revokedAtUtc;
    }
}
