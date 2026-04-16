using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

/// <inheritdoc />
public partial class AddReminderTimezoneColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "reminder_timezone",
            table: "reminder_configs",
            type: "text",
            nullable: false,
            defaultValue: "Etc/UTC");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "reminder_timezone",
            table: "reminder_configs");
    }
}
