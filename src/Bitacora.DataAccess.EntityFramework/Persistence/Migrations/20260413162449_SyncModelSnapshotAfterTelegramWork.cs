using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelSnapshotAfterTelegramWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op migration: keeps the model snapshot in sync with the existing
            // migration chain so fresh databases can be migrated safely.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op migration: snapshot sync only.
        }
    }
}
