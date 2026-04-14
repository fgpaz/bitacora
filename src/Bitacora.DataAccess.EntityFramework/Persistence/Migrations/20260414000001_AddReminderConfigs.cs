using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

/// <inheritdoc />
public partial class AddReminderConfigs : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "reminder_configs",
            columns: table => new
            {
                reminder_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                hour_utc = table.Column<int>(type: "integer", nullable: false),
                minute_utc = table.Column<int>(type: "integer", nullable: false),
                enabled = table.Column<bool>(type: "boolean", nullable: false),
                next_fire_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                disabled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_reminder_configs", x => x.reminder_config_id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_reminder_configs_next_fire_at_utc",
            table: "reminder_configs",
            column: "next_fire_at_utc");

        migrationBuilder.CreateIndex(
            name: "IX_reminder_configs_patient_id",
            table: "reminder_configs",
            column: "patient_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "reminder_configs");
    }
}
