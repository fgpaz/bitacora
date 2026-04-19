'use client';

import { createContext, useContext, useEffect, useState } from 'react';
import type { BitacoraRole, PublicSession } from './constants';

export interface SessionUser {
  id: string;
  email: string;
  role: BitacoraRole;
}

export interface SessionState {
  user: SessionUser | null;
  loading: boolean;
}

const SessionContext = createContext<SessionState>({
  user: null,
  loading: true,
});

export function SessionProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<SessionUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;

    fetch('/api/auth/session', { cache: 'no-store' })
      .then(async (response) => {
        if (!response.ok) return { user: null, expiresAt: null } satisfies PublicSession;
        return response.json() as Promise<PublicSession>;
      })
      .then((session) => {
        if (!mounted) return;
        setUser(session.user);
        setLoading(false);
      })
      .catch(() => {
        if (!mounted) return;
        setUser(null);
        setLoading(false);
      });

    return () => {
      mounted = false;
    };
  }, []);

  return (
    <SessionContext.Provider value={{ user, loading }}>
      {children}
    </SessionContext.Provider>
  );
}

export function useSession(): SessionState {
  return useContext(SessionContext);
}
