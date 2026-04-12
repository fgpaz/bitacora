'use client';

/**
 * Session context for the patient.
 * Holds the Supabase access token and user metadata.
 * Token is resolved by the Supabase JS client after magic-link / OAuth login.
 */

import { createClient, SupabaseClient } from '@supabase/supabase-js';
import { createContext, useContext, useEffect, useState } from 'react';

export interface SessionUser {
  id: string;
  email: string;
}

export interface SessionState {
  supabase: SupabaseClient;
  user: SessionUser | null;
  loading: boolean;
}

// Placeholder until provider initialises — the real client is created lazily inside SessionProvider
const PLACEHOLDER_CLIENT = {} as SupabaseClient;

const SessionContext = createContext<SessionState>({
  supabase: PLACEHOLDER_CLIENT,
  user: null,
  loading: true,
});

export function SessionProvider({ children }: { children: React.ReactNode }) {
  const [supabase] = useState<SupabaseClient>(() => {
    const url = process.env.NEXT_PUBLIC_SUPABASE_URL;
    const key = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;
    if (!url || !key) {
      // During build prerender this will not execute; provider always mounts in browser
      return PLACEHOLDER_CLIENT;
    }
    return createClient(url, key);
  });
  const [user, setUser] = useState<SessionUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Get initial session (checks cookies/localStorage via Supabase client)
    supabase.auth.getSession().then(({ data }) => {
      if (data.session?.user) {
        setUser({ id: data.session.user.id, email: data.session.user.email ?? '' });
      }
      setLoading(false);
    });

    // Listen for auth state changes
    const { data: { subscription } } = supabase.auth.onAuthStateChange((_event, session) => {
      setUser(session?.user ? { id: session.user.id, email: session.user.email ?? '' } : null);
      setLoading(false);
    });

    return () => subscription.unsubscribe();
  }, [supabase]);

  return (
    <SessionContext.Provider value={{ supabase, user, loading }}>
      {children}
    </SessionContext.Provider>
  );
}

export function useSession(): SessionState {
  return useContext(SessionContext);
}
