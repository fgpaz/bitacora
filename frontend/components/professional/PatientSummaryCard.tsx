'use client';

/**
 * PatientSummaryCard — displays summary stats for a patient.
 */
import { PatientSummary } from '@/lib/api/professional';
import styles from './PatientSummaryCard.module.css';

interface Props {
  summary: PatientSummary;
}

export function PatientSummaryCard({ summary }: Props) {
  const avgScore =
    summary.avg_mood_score != null
      ? summary.avg_mood_score.toFixed(1)
      : '—';

  return (
    <section className={styles.card} aria-labelledby="summary-heading">
      <h2 id="summary-heading" className={styles.heading}>Resumen del paciente</h2>
      <dl className={styles.grid}>
        <div className={styles.stat}>
          <dt className={styles.statLabel}>Registros totales</dt>
          <dd className={styles.statValue}>{summary.total_entries}</dd>
        </div>
        <div className={styles.stat}>
          <dt className={styles.statLabel}>Promedio de humor</dt>
          <dd className={styles.statValue}>{avgScore}</dd>
        </div>
        <div className={styles.stat}>
          <dt className={styles.statLabel}>Ultimo registro</dt>
          <dd className={styles.statValue}>
            {summary.last_entry_at
              ? new Date(summary.last_entry_at).toLocaleDateString('es-AR', { dateStyle: 'medium' })
              : '—'}
          </dd>
        </div>
        <div className={styles.stat}>
          <dt className={styles.statLabel}>Vinculado desde</dt>
          <dd className={styles.statValue}>
            {new Date(summary.created_at).toLocaleDateString('es-AR', { dateStyle: 'medium' })}
          </dd>
        </div>
      </dl>
    </section>
  );
}
