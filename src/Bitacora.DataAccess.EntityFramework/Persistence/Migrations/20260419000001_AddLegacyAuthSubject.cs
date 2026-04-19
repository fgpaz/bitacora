using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

public partial class AddLegacyAuthSubject : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "legacy_auth_subject",
            table: "users",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.Sql("""
            UPDATE users
            SET legacy_auth_subject = supabase_user_id
            WHERE legacy_auth_subject IS NULL;
            """);

        migrationBuilder.CreateIndex(
            name: "IX_users_legacy_auth_subject",
            table: "users",
            column: "legacy_auth_subject");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE users
            SET supabase_user_id = legacy_auth_subject
            WHERE legacy_auth_subject IS NOT NULL;
            """);

        migrationBuilder.DropIndex(
            name: "IX_users_legacy_auth_subject",
            table: "users");

        migrationBuilder.DropColumn(
            name: "legacy_auth_subject",
            table: "users");
    }
}
