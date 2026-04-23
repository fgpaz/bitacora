'use client';

/**
 * DashboardSummary — patient dashboard statistics cards.
 * Displays total entries, average mood score, and last entry timestamp.
 */
import { formatMoodScore } from '@/lib/formatters';
import styles from './DashboardSummary.module.css';

interface Props {
  totalEntries: number;
  avgMoodScore: number | null;
  lastEntryAt: string | null;
}

function formatDate(isoString: string | null): string {
  if (!isoString) return '—';
  try {
    const date = new Date(isoString.includes('T') ? isoString : `${isoString}T00:00:00`);
    return date.toLocaleDateString('es-AR', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  } catch {
    return '—';
  }
}

export function DashboardSummary({ totalEntries, avgMoodScore, lastEntryAt }: Props) {
  return (
    <div className={styles.grid}>
      <div className={styles.card}>
        <p className={styles.label}>Registros totales</p>
        <p className={styles.value}>{totalEntries}</p>
      </div>

      <div className={styles.card}>
        <p className={styles.label}>Promedio de humor</p>
        <p className={styles.value}>
          {formatMoodScore(avgMoodScore, { decimals: 1 })}
        </p>
      </div>

      <div className={styles.card}>
        <p className={styles.label}>Último registro</p>
        <p className={styles.dateValue}>{formatDate(lastEntryAt)}</p>
      </div>
    </div>
  );
}
