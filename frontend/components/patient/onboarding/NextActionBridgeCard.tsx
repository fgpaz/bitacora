'use client';

/**
 * NextActionBridgeCard — post-consent bridge to first mood entry.
 * Shown in S04-BRIDGE state.
 */
import Link from 'next/link';
import styles from './NextActionBridgeCard.module.css';

interface Props {
  needsFirstEntry?: boolean;
}

export function NextActionBridgeCard({ needsFirstEntry = true }: Props) {
  return (
    <div className={styles.card} role="status" aria-live="polite">
      <p className={styles.confirmText}>Consentimiento registrado.</p>
      {needsFirstEntry ? (
        <Link href="/registro/mood-entry" className={styles.cta}>
          Hacer mi primer registro
        </Link>
      ) : (
        <p className={styles.note}>Ya puedes usar Bitacora.</p>
      )}
      <Link href="/configuracion/telegram" className={styles.secondaryLink}>
        Vincular Telegram (opcional)
      </Link>
    </div>
  );
}
