'use client';

/**
 * PatientPageShell — single-column editorial shell for patient-facing pages.
 * States: loading | ready | error
 */
import { ReactNode } from 'react';
import { useRouter } from 'next/navigation';
import { signOut } from '../../lib/auth/client';
import { ShellMenu, ShellMenuItem, ShellMenuSeparator } from './ShellMenu';
import styles from './PatientPageShell.module.css';

interface Props {
  children?: ReactNode;
  loading?: boolean;
  error?: string | null;
}

export function PatientPageShell({ children, loading, error }: Props) {
  const router = useRouter();

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
          <p>{error}</p>
        </div>
      </main>
    );
  }

  async function handleLogout() {
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
