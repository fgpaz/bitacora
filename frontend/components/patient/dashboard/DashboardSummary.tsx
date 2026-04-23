'use client';

/**
 * DashboardSummary — resumen del estado del paciente en dos variantes:
 *   - `cards`: 3 stat cards (vista legada; se preserva para profesional).
 *   - `compact`: texto corrido sobrio (canon 10 §12.2 "evitar tableros de vigilancia").
 *
 * En el dashboard del paciente ready, usar variant='compact'.
 */
import { formatMoodScore } from '@/lib/formatters';
import styles from './DashboardSummary.module.css';

interface Props {
  totalEntries: number;
  avgMoodScore: number | null;
  lastEntryAt: string | null;
  variant?: 'cards' | 'compact';
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

function formatDaysAgo(isoString: string | null): string {
  if (!isoString) return '';
  try {
    const date = new Date(isoString.includes('T') ? isoString : `${isoString}T00:00:00`);
    const diffMs = Date.now() - date.getTime();
    const days = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    if (days <= 0) return 'hoy';
    if (days === 1) return 'ayer';
    return `hace ${days} días`;
  } catch {
    return '';
  }
}

export function DashboardSummary({
  totalEntries,
  avgMoodScore,
  lastEntryAt,
  variant = 'cards',
}: Props) {
  if (variant === 'compact') {
    const noun = totalEntries === 1 ? 'registro' : 'registros';
    const lastPhrase = lastEntryAt ? ` El último, ${formatDaysAgo(lastEntryAt)}.` : '';
    return (
      <p className={styles.compactSummary}>
        Llevás {totalEntries} {noun}.{lastPhrase}
      </p>
    );
  }

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
