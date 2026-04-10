using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Context;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;

public sealed class AppDbContext : DbContext
{
    private readonly Guid? _currentPatientId;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentPatientContextAccessor currentPatientContextAccessor)
        : base(options)
    {
        _currentPatientId = currentPatientContextAccessor.PatientId;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();
    public DbSet<DailyCheckin> DailyCheckins => Set<DailyCheckin>();
    public DbSet<ConsentGrant> ConsentGrants => Set<ConsentGrant>();
    public DbSet<PendingInvite> PendingInvites => Set<PendingInvite>();
    public DbSet<AccessAudit> AccessAudits => Set<AccessAudit>();
    public DbSet<EncryptionKeyVersion> EncryptionKeyVersions => Set<EncryptionKeyVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.SupabaseUserId).HasColumnName("supabase_user_id").HasMaxLength(128).IsRequired();
            entity.Property(x => x.EncryptedEmail).HasColumnName("encrypted_email").IsRequired();
            entity.Property(x => x.EmailHash).HasColumnName("email_hash").HasMaxLength(128).IsRequired();
            entity.Property(x => x.KeyVersion).HasColumnName("key_version").IsRequired();
            entity.Property(x => x.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.SessionsRevokedAt).HasColumnName("sessions_revoked_at");
            entity.HasIndex(x => x.SupabaseUserId).IsUnique();
            entity.HasIndex(x => x.EmailHash);
        });

        modelBuilder.Entity<MoodEntry>(entity =>
        {
            entity.ToTable("mood_entries");
            entity.HasKey(x => x.MoodEntryId);
            entity.Property(x => x.MoodEntryId).HasColumnName("mood_entry_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.EncryptedPayload).HasColumnName("encrypted_payload").IsRequired();
            entity.Property(x => x.SafeProjection).HasColumnName("safe_projection").HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.KeyVersion).HasColumnName("key_version").IsRequired();
            entity.Property(x => x.EncryptedAt).HasColumnName("encrypted_at").IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.PatientId, x.CreatedAtUtc });
            entity.HasQueryFilter(x => _currentPatientId == null || x.PatientId == _currentPatientId);
        });

        modelBuilder.Entity<DailyCheckin>(entity =>
        {
            entity.ToTable("daily_checkins");
            entity.HasKey(x => x.DailyCheckinId);
            entity.Property(x => x.DailyCheckinId).HasColumnName("daily_checkin_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.CheckinDate).HasColumnName("checkin_date").IsRequired();
            entity.Property(x => x.EncryptedPayload).HasColumnName("encrypted_payload").IsRequired();
            entity.Property(x => x.SafeProjection).HasColumnName("safe_projection").HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.KeyVersion).HasColumnName("key_version").IsRequired();
            entity.Property(x => x.EncryptedAt).HasColumnName("encrypted_at").IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
            entity.HasIndex(x => new { x.PatientId, x.CheckinDate }).IsUnique();
            entity.HasQueryFilter(x => _currentPatientId == null || x.PatientId == _currentPatientId);
        });

        modelBuilder.Entity<ConsentGrant>(entity =>
        {
            entity.ToTable("consent_grants");
            entity.HasKey(x => x.ConsentGrantId);
            entity.Property(x => x.ConsentGrantId).HasColumnName("consent_grant_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.ConsentVersion).HasColumnName("consent_version").HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.GrantedAt).HasColumnName("granted_at");
            entity.Property(x => x.RevokedAt).HasColumnName("revoked_at");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.PatientId, x.ConsentVersion });
        });

        modelBuilder.Entity<PendingInvite>(entity =>
        {
            entity.ToTable("pending_invites");
            entity.HasKey(x => x.PendingInviteId);
            entity.Property(x => x.PendingInviteId).HasColumnName("pending_invite_id");
            entity.Property(x => x.ProfessionalId).HasColumnName("professional_id").IsRequired();
            entity.Property(x => x.InviteeEmailHash).HasColumnName("invitee_email_hash").HasMaxLength(128).IsRequired();
            entity.Property(x => x.InviteToken).HasColumnName("invite_token").HasMaxLength(256).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
            entity.Property(x => x.ConsumedAt).HasColumnName("consumed_at");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.ProfessionalId, x.InviteeEmailHash, x.Status });
        });

        modelBuilder.Entity<AccessAudit>(entity =>
        {
            entity.ToTable("access_audits");
            entity.HasKey(x => x.AuditId);
            entity.Property(x => x.AuditId).HasColumnName("audit_id");
            entity.Property(x => x.TraceId).HasColumnName("trace_id").IsRequired();
            entity.Property(x => x.ActorId).HasColumnName("actor_id").IsRequired();
            entity.Property(x => x.PseudonymId).HasColumnName("pseudonym_id").HasMaxLength(128).IsRequired();
            entity.Property(x => x.ActionType).HasColumnName("action_type").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.ResourceType).HasColumnName("resource_type").HasMaxLength(64).IsRequired();
            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id");
            entity.Property(x => x.Outcome).HasColumnName("outcome").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.PatientId, x.CreatedAtUtc });
        });

        modelBuilder.Entity<EncryptionKeyVersion>(entity =>
        {
            entity.ToTable("encryption_key_versions");
            entity.HasKey(x => x.KeyVersion);
            entity.Property(x => x.KeyVersion).HasColumnName("key_version");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
            entity.HasData(new EncryptionKeyVersion(1, new DateTime(2026, 4, 9, 0, 0, 0, DateTimeKind.Utc), true));
        });
    }
}

