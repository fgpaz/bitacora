using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_audits",
                columns: table => new
                {
                    audit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pseudonym_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    action_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    resource_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: true),
                    outcome = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_audits", x => x.audit_id);
                });

            migrationBuilder.CreateTable(
                name: "consent_grants",
                columns: table => new
                {
                    consent_grant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_grants", x => x.consent_grant_id);
                });

            migrationBuilder.CreateTable(
                name: "daily_checkins",
                columns: table => new
                {
                    daily_checkin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checkin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    encrypted_payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    safe_projection = table.Column<string>(type: "jsonb", nullable: false),
                    key_version = table.Column<int>(type: "integer", nullable: false),
                    encrypted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_checkins", x => x.daily_checkin_id);
                });

            migrationBuilder.CreateTable(
                name: "encryption_key_versions",
                columns: table => new
                {
                    key_version = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encryption_key_versions", x => x.key_version);
                });

            migrationBuilder.CreateTable(
                name: "mood_entries",
                columns: table => new
                {
                    mood_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    safe_projection = table.Column<string>(type: "jsonb", nullable: false),
                    key_version = table.Column<int>(type: "integer", nullable: false),
                    encrypted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mood_entries", x => x.mood_entry_id);
                });

            migrationBuilder.CreateTable(
                name: "pending_invites",
                columns: table => new
                {
                    pending_invite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invitee_email_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    invite_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_invites", x => x.pending_invite_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supabase_user_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    encrypted_email = table.Column<byte[]>(type: "bytea", nullable: false),
                    email_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    key_version = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sessions_revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.InsertData(
                table: "encryption_key_versions",
                columns: new[] { "key_version", "created_at_utc", "is_active" },
                values: new object[] { 1, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Utc), true });

            migrationBuilder.CreateIndex(
                name: "IX_access_audits_patient_id_created_at_utc",
                table: "access_audits",
                columns: new[] { "patient_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_consent_grants_patient_id_consent_version",
                table: "consent_grants",
                columns: new[] { "patient_id", "consent_version" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_checkins_patient_id_checkin_date",
                table: "daily_checkins",
                columns: new[] { "patient_id", "checkin_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mood_entries_patient_id_created_at_utc",
                table: "mood_entries",
                columns: new[] { "patient_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_pending_invites_professional_id_invitee_email_hash_status",
                table: "pending_invites",
                columns: new[] { "professional_id", "invitee_email_hash", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_users_email_hash",
                table: "users",
                column: "email_hash");

            migrationBuilder.CreateIndex(
                name: "IX_users_supabase_user_id",
                table: "users",
                column: "supabase_user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_audits");

            migrationBuilder.DropTable(
                name: "consent_grants");

            migrationBuilder.DropTable(
                name: "daily_checkins");

            migrationBuilder.DropTable(
                name: "encryption_key_versions");

            migrationBuilder.DropTable(
                name: "mood_entries");

            migrationBuilder.DropTable(
                name: "pending_invites");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
