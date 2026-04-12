'use client';

import { SessionProvider } from '@/lib/auth/SessionContext';
import { ReactNode } from 'react';

export function Providers({ children }: { children: ReactNode }) {
  return <SessionProvider>{children}</SessionProvider>;
}
