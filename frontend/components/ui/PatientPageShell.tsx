'use client';

/**
 * PatientPageShell — single-column editorial shell for patient-facing pages.
 * States: loading | ready | error
 */
import { ReactNode, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { signOut } from '../../lib/auth/client';
import type { UserFacingError } from '../../lib/errors/user-facing';
import { track } from '../../lib/analytics/track';
import { ShellMenu, ShellMenuItem, ShellMenuSeparator } from './ShellMenu';
import styles from './PatientPageShell.module.css';

interface Props {
  children?: ReactNode;
  loading?: boolean;
  error?: UserFacingError | null;
}

export function PatientPageShell({ children, loading, error }: Props) {
  const router = useRouter();
  const mountedAtRef = useRef<number | null>(null);

  useEffect(() => {
    mountedAtRef.current = performance.now();
  }, []);

  if (loading) {
    return (
      <main className={styles.shell} aria-busy="true" aria-label="Cargando">
        <div className={styles.skeleton} aria-hidden="true" />
      </main>
    );
  }

  if (error) {
    return (
      <main className={styles.shell}>
        <div className={styles.errorState} role="alert">
          <p className={styles.errorTitle}>{error.title}</p>
          <p className={styles.errorDescription}>{error.description}</p>
          {error.retry && (
            <button type="button" className={styles.errorRetry} onClick={error.retry}>
              Reintentar
            </button>
          )}
        </div>
      </main>
    );
  }

  async function handleLogout() {
    if (mountedAtRef.current !== null) {
      const uptimeMs = Math.round(performance.now() - mountedAtRef.current);
      track('logout_accidental_rate', {
        uptime_ms: uptimeMs,
        accidental: uptimeMs < 180_000,
      });
    } else {
      track('logout_accidental_rate', { uptime_ms: null, accidental: null });
    }
    await signOut();
  }

  return (
    <main className={styles.shell}>
      <header className={styles.shellHeader}>
        <ShellMenu>
          <ShellMenuItem onClick={() => router.push('/configuracion/telegram')}>
            Recordatorios
          </ShellMenuItem>
          <ShellMenuItem onClick={() => router.push('/configuracion/vinculos')}>
            Vínculos
          </ShellMenuItem>
          <ShellMenuItem onClick={() => router.push('/configuracion/consent')}>
            Consentimiento
          </ShellMenuItem>
          <ShellMenuSeparator />
          <ShellMenuItem onClick={handleLogout} variant="destructive">
            Cerrar sesión
          </ShellMenuItem>
        </ShellMenu>
      </header>
      <div className={styles.content}>{children}</div>
    </main>
  );
}
