using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBindingCodesAndCareLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "binding_codes",
                columns: table => new
                {
                    binding_code_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ttl_preset = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_binding_codes", x => x.binding_code_id);
                });

            migrationBuilder.CreateTable(
                name: "care_links",
                columns: table => new
                {
                    care_link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    can_view_data = table.Column<bool>(type: "boolean", nullable: false),
                    invited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    accepted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_care_links", x => x.care_link_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_binding_codes_code",
                table: "binding_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_binding_codes_professional_id_used_expires_at",
                table: "binding_codes",
                columns: new[] { "professional_id", "used", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_care_links_patient_id_professional_id",
                table: "care_links",
                columns: new[] { "patient_id", "professional_id" });

            migrationBuilder.CreateIndex(
                name: "IX_care_links_professional_id_status",
                table: "care_links",
                columns: new[] { "professional_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "binding_codes");

            migrationBuilder.DropTable(
                name: "care_links");
        }
    }
}
