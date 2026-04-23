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
    public DbSet<BindingCode> BindingCodes => Set<BindingCode>();
    public DbSet<CareLink> CareLinks => Set<CareLink>();
    public DbSet<TelegramPairingCode> TelegramPairingCodes => Set<TelegramPairingCode>();
    public DbSet<TelegramSession> TelegramSessions => Set<TelegramSession>();
    public DbSet<ReminderConfig> ReminderConfigs => Set<ReminderConfig>();
    public DbSet<AnalyticsEvent> AnalyticsEvents => Set<AnalyticsEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.AuthSubject).HasColumnName("auth_subject").HasMaxLength(128).IsRequired();
            entity.Property(x => x.EncryptedEmail).HasColumnName("encrypted_email").IsRequired();
            entity.Property(x => x.EmailHash).HasColumnName("email_hash").HasMaxLength(128).IsRequired();
            entity.Property(x => x.KeyVersion).HasColumnName("key_version").IsRequired();
            entity.Property(x => x.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.SessionsRevokedAt).HasColumnName("sessions_revoked_at");
            entity.HasIndex(x => x.AuthSubject).HasDatabaseName("IX_users_auth_subject").IsUnique();
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

        modelBuilder.Entity<BindingCode>(entity =>
        {
            entity.ToTable("binding_codes");
            entity.HasKey(x => x.BindingCodeId);
            entity.Property(x => x.BindingCodeId).HasColumnName("binding_code_id");
            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(16).IsRequired();
            entity.Property(x => x.ProfessionalId).HasColumnName("professional_id").IsRequired();
            entity.Property(x => x.TtlPreset).HasColumnName("ttl_preset").HasMaxLength(8).IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
            entity.Property(x => x.Used).HasColumnName("used").IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.ProfessionalId, x.Used, x.ExpiresAt });
        });

        modelBuilder.Entity<CareLink>(entity =>
        {
            entity.ToTable("care_links");
            entity.HasKey(x => x.CareLinkId);
            entity.Property(x => x.CareLinkId).HasColumnName("care_link_id");
            entity.Property(x => x.ProfessionalId).HasColumnName("professional_id").IsRequired();
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.CanViewData).HasColumnName("can_view_data").IsRequired();
            entity.Property(x => x.InvitedAt).HasColumnName("invited_at").IsRequired();
            entity.Property(x => x.AcceptedAt).HasColumnName("accepted_at");
            entity.Property(x => x.RevokedAt).HasColumnName("revoked_at");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.PatientId, x.ProfessionalId });
            entity.HasIndex(x => new { x.ProfessionalId, x.Status });
        });

        modelBuilder.Entity<TelegramPairingCode>(entity =>
        {
            entity.ToTable("telegram_pairing_codes");
            entity.HasKey(x => x.TelegramPairingCodeId);
            entity.Property(x => x.TelegramPairingCodeId).HasColumnName("telegram_pairing_code_id");
            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(16).IsRequired();
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
            entity.Property(x => x.Used).HasColumnName("used").IsRequired();
            entity.Property(x => x.ConsumedAt).HasColumnName("consumed_at");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.PatientId, x.Used, x.ExpiresAt });
        });

        modelBuilder.Entity<TelegramSession>(entity =>
        {
            entity.ToTable("telegram_sessions");
            entity.HasKey(x => x.TelegramSessionId);
            entity.Property(x => x.TelegramSessionId).HasColumnName("telegram_session_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.ChatId).HasColumnName("chat_id").HasMaxLength(64).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(16).IsRequired();
            entity.Property(x => x.ConversationState).HasColumnName("conversation_state").HasConversion<int>().IsRequired();
            entity.Property(x => x.PendingMoodScore).HasColumnName("pending_mood_score");
            entity.Property(x => x.PendingFactorsJson).HasColumnName("pending_factors_json").HasMaxLength(512);
            entity.Property(x => x.LinkedAtUtc).HasColumnName("linked_at_utc").IsRequired();
            entity.Property(x => x.UnlinkedAtUtc).HasColumnName("unlinked_at_utc");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
            entity.HasIndex(x => x.ChatId)
                .IsUnique()
                .HasFilter("status = 'Linked'");
            entity.HasIndex(x => new { x.PatientId, x.Status });
        });

        modelBuilder.Entity<ReminderConfig>(entity =>
        {
            entity.ToTable("reminder_configs");
            entity.HasKey(x => x.ReminderConfigId);
            entity.Property(x => x.ReminderConfigId).HasColumnName("reminder_config_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.HourUtc).HasColumnName("hour_utc").IsRequired();
            entity.Property(x => x.MinuteUtc).HasColumnName("minute_utc").IsRequired();
            entity.Property(x => x.Enabled).HasColumnName("enabled").IsRequired();
            entity.Property(x => x.ReminderTimezone).HasColumnName("reminder_timezone").HasMaxLength(128).IsRequired();
            entity.Property(x => x.NextFireAtUtc).HasColumnName("next_fire_at_utc");
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.DisabledAtUtc).HasColumnName("disabled_at_utc");
            entity.HasIndex(x => new { x.PatientId }).IsUnique();
            entity.HasIndex(x => x.NextFireAtUtc);
        });

        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.ToTable("analytics_events");
            entity.HasKey(x => x.AnalyticsEventId);
            entity.Property(x => x.AnalyticsEventId).HasColumnName("analytics_event_id");
            entity.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(x => x.EventName).HasColumnName("event_name").HasMaxLength(64).IsRequired();
            entity.Property(x => x.PropsJson).HasColumnName("props_json").HasColumnType("jsonb");
            entity.Property(x => x.TraceId).HasColumnName("trace_id").IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            entity.HasIndex(x => new { x.EventName, x.CreatedAtUtc });
            entity.HasIndex(x => new { x.PatientId, x.CreatedAtUtc });
        });
    }
}

