using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

/// <inheritdoc />
public partial class AddTelegramConversationState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "conversation_state",
            table: "telegram_sessions",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "pending_mood_score",
            table: "telegram_sessions",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "pending_factors_json",
            table: "telegram_sessions",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_at_utc",
            table: "telegram_sessions",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "conversation_state", table: "telegram_sessions");
        migrationBuilder.DropColumn(name: "pending_mood_score", table: "telegram_sessions");
        migrationBuilder.DropColumn(name: "pending_factors_json", table: "telegram_sessions");
        migrationBuilder.DropColumn(name: "updated_at_utc", table: "telegram_sessions");
    }
}
