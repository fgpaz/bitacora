using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

/// <inheritdoc />
public partial class AllowTelegramRelinkAfterUnlink : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DROP INDEX IF EXISTS ix_telegram_sessions_chat_id;
            DROP INDEX IF EXISTS "IX_telegram_sessions_chat_id";
            """);

        migrationBuilder.CreateIndex(
            name: "IX_telegram_sessions_chat_id",
            table: "telegram_sessions",
            column: "chat_id",
            unique: true,
            filter: "status = 'Linked'");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DROP INDEX IF EXISTS ix_telegram_sessions_chat_id;
            DROP INDEX IF EXISTS "IX_telegram_sessions_chat_id";
            """);

        migrationBuilder.CreateIndex(
            name: "IX_telegram_sessions_chat_id",
            table: "telegram_sessions",
            column: "chat_id",
            unique: true);
    }
}
