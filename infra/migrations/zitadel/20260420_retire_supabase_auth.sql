BEGIN;

DROP INDEX IF EXISTS "IX_users_legacy_auth_subject";
DROP INDEX IF EXISTS "IX_users_supabase_user_id";

DO $$
BEGIN
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
        ALTER TABLE public.users RENAME COLUMN supabase_user_id TO auth_subject;
    ELSIF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'users'
          AND column_name = 'supabase_user_id'
    ) AND EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'users'
          AND column_name = 'auth_subject'
    ) THEN
        UPDATE public.users
        SET auth_subject = supabase_user_id
        WHERE auth_subject IS NULL
          AND supabase_user_id IS NOT NULL;

        ALTER TABLE public.users DROP COLUMN supabase_user_id;
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'users'
          AND column_name = 'legacy_auth_subject'
    ) THEN
        ALTER TABLE public.users DROP COLUMN legacy_auth_subject;
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM public.users
        WHERE auth_subject IS NULL
    ) THEN
        RAISE EXCEPTION 'Cannot retire Supabase auth: users.auth_subject contains null values.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM public.users
        WHERE auth_subject IS NOT NULL
        GROUP BY auth_subject
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION 'Cannot retire Supabase auth: duplicate users.auth_subject values detected.';
    END IF;
END $$;

ALTER TABLE public.users ALTER COLUMN auth_subject SET NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS "IX_users_auth_subject"
    ON public.users (auth_subject);

COMMIT;
