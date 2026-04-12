'use client';

/**
 * ProfessionalShell — top-navigation shell for professional-facing pages.
 * States: loading | ready
 */
import { ReactNode } from 'react';
import Link from 'next/link';
import styles from './ProfessionalShell.module.css';

interface Props {
  children?: ReactNode;
  loading?: boolean;
}

export function ProfessionalShell({ children, loading }: Props) {
  if (loading) {
    return (
      <div className={styles.shell} aria-busy="true" aria-label="Cargando">
        <div className={styles.skeleton} aria-hidden="true" />
      </div>
    );
  }

  return (
    <div className={styles.shell}>
      <header className={styles.header}>
        <nav className={styles.nav} aria-label="Navegacion profesional">
          <Link href="/profesional/pacientes" className={styles.navLink}>
            Pacientes
          </Link>
          <Link href="/profesional/invitaciones" className={styles.navLink}>
            Invitaciones
          </Link>
        </nav>
        <div className={styles.wordmark}>Bitacora Pro</div>
      </header>
      <main className={styles.main}>
        <div className={styles.content}>{children}</div>
      </main>
    </div>
  );
}
