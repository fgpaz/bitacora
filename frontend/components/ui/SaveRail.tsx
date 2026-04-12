'use client';

/**
 * SaveRail — bottom action bar for form submission.
 * States: disabled | enabled | loading
 */
import { ReactNode } from 'react';
import styles from './SaveRail.module.css';

interface Props {
  children: ReactNode;
  disabled?: boolean;
  loading?: boolean;
  onClick?: () => void;
}

export function SaveRail({ children, disabled, loading }: Props) {
  return (
    <div className={styles.rail} role="group" aria-label="Acciones">
      <button
        type="submit"
        className={styles.cta}
        disabled={disabled || loading}
        aria-busy={loading}
      >
        {loading ? 'Guardando...' : children}
      </button>
    </div>
  );
}
