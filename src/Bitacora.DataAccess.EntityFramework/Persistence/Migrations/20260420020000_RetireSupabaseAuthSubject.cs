using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence.Migrations;

public partial class RetireSupabaseAuthSubject : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_legacy_auth_subject'
                ) THEN
                    DROP INDEX "IX_users_legacy_auth_subject";
                END IF;

                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'legacy_auth_subject'
                ) THEN
                    ALTER TABLE users DROP COLUMN legacy_auth_subject;
                END IF;

                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'supabase_user_id'
                ) AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'auth_subject'
                ) THEN
                    ALTER TABLE users RENAME COLUMN supabase_user_id TO auth_subject;
                END IF;

                IF EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_supabase_user_id'
                ) AND NOT EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_auth_subject'
                ) THEN
                    ALTER INDEX "IX_users_supabase_user_id" RENAME TO "IX_users_auth_subject";
                END IF;

                IF NOT EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_auth_subject'
                ) THEN
                    CREATE UNIQUE INDEX "IX_users_auth_subject" ON users(auth_subject);
                END IF;
            END $$;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_auth_subject'
                ) AND NOT EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_supabase_user_id'
                ) THEN
                    ALTER INDEX "IX_users_auth_subject" RENAME TO "IX_users_supabase_user_id";
                END IF;

                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'auth_subject'
                ) AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'supabase_user_id'
                ) THEN
                    ALTER TABLE users RENAME COLUMN auth_subject TO supabase_user_id;
                END IF;

                IF NOT EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'users'
                      AND column_name = 'legacy_auth_subject'
                ) THEN
                    ALTER TABLE users ADD COLUMN legacy_auth_subject character varying(128);
                    UPDATE users
                    SET legacy_auth_subject = supabase_user_id
                    WHERE legacy_auth_subject IS NULL;
                END IF;

                IF NOT EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND indexname = 'IX_users_legacy_auth_subject'
                ) THEN
                    CREATE INDEX "IX_users_legacy_auth_subject" ON users(legacy_auth_subject);
                END IF;
            END $$;
            """);
    }
}
