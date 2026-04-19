'use client';

/**
 * PatientPageShell — single-column editorial shell for patient-facing pages.
 * States: loading | ready | error
 */
import { ReactNode } from 'react';
import { signOut } from '../../lib/auth/client';
import styles from './PatientPageShell.module.css';

interface Props {
  children?: ReactNode;
  loading?: boolean;
  error?: string | null;
}

export function PatientPageShell({ children, loading, error }: Props) {
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
      <div className={styles.content}>{children}</div>
      <button
        onClick={handleLogout}
        className={styles.logoutButton}
        title="Cerrar sesión"
      >
        Cerrar sesión
      </button>
    </main>
  );
}
