using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

/// <inheritdoc />
public partial class AddTelegramSessionsAndPairingCodes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "telegram_pairing_codes",
            columns: table => new
            {
                telegram_pairing_code_id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                used = table.Column<bool>(type: "boolean", nullable: false),
                consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_telegram_pairing_codes", x => x.telegram_pairing_code_id);
            });

        migrationBuilder.CreateTable(
            name: "telegram_sessions",
            columns: table => new
            {
                telegram_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                chat_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                linked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                unlinked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_telegram_sessions", x => x.telegram_session_id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_telegram_pairing_codes_code",
            table: "telegram_pairing_codes",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_telegram_pairing_codes_patient_id_used_expires_at",
            table: "telegram_pairing_codes",
            columns: new[] { "patient_id", "used", "expires_at" });

        migrationBuilder.CreateIndex(
            name: "IX_telegram_sessions_chat_id",
            table: "telegram_sessions",
            column: "chat_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_telegram_sessions_patient_id_status",
            table: "telegram_sessions",
            columns: new[] { "patient_id", "status" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "telegram_pairing_codes");
        migrationBuilder.DropTable(name: "telegram_sessions");
    }
}
