using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; private set; }
    public string AuthSubject { get; private set; } = string.Empty;
    public string? LegacyAuthSubject { get; private set; }
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
        string authSubject,
        byte[] encryptedEmail,
        string emailHash,
        int keyVersion,
        UserRole role,
        UserStatus status,
        DateTime createdAtUtc)
    {
        UserId = userId;
        AuthSubject = authSubject;
        EncryptedEmail = encryptedEmail;
        EmailHash = emailHash;
        KeyVersion = keyVersion;
        Role = role;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public static User CreatePatient(
        string authSubject,
        byte[] encryptedEmail,
        string emailHash,
        int keyVersion,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(authSubject))
        {
            throw new ArgumentException("Auth subject is required.", nameof(authSubject));
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
            authSubject.Trim(),
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

    public void LinkAuthSubject(string authSubject)
    {
        if (string.IsNullOrWhiteSpace(authSubject))
        {
            throw new ArgumentException("Auth subject is required.", nameof(authSubject));
        }

        var normalized = authSubject.Trim();
        if (string.Equals(AuthSubject, normalized, StringComparison.Ordinal))
        {
            return;
        }

        LegacyAuthSubject ??= AuthSubject;
        AuthSubject = normalized;
    }
}
