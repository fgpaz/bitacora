'use client';

/**
 * Timeline — chronological feed of patient entries.
 * Supports pagination.
 */
import { useEffect, useState } from 'react';
import { getPatientTimeline } from '@/lib/api/professional';
import type { TimelineEntry } from '@/lib/api/professional';
import styles from './Timeline.module.css';

interface Props {
  patientId: string;
}

function EntryRow({ entry }: { entry: TimelineEntry }) {
  const date = new Date(entry.created_at).toLocaleDateString('es-AR', {
    dateStyle: 'medium',
    timeStyle: 'short',
  });

  if (entry.entry_type === 'mood_entry') {
    const score = (entry.data as { score?: number }).score;
    return (
      <div className={styles.entry}>
        <span className={styles.entryType}>Registro de humor</span>
        <span className={styles.entryDate}>{date}</span>
        {score != null && (
          <span className={`${styles.score} ${score >= 0 ? styles.positive : styles.negative}`}>
            {score > 0 ? `+${score}` : score}
          </span>
        )}
      </div>
    );
  }

  if (entry.entry_type === 'daily_checkin') {
    const d = entry.data as Record<string, boolean | string | number>;
    return (
      <div className={styles.entry}>
        <span className={styles.entryType}>Check-in diario</span>
        <span className={styles.entryDate}>{date}</span>
        {d.anxiety && <span className={styles.flag}>Ansiedad</span>}
        {d.irritability && <span className={styles.flag}>Irritabilidad</span>}
      </div>
    );
  }

  return (
    <div className={styles.entry}>
      <span className={styles.entryType}>{entry.entry_type}</span>
      <span className={styles.entryDate}>{date}</span>
    </div>
  );
}

export function Timeline({ patientId }: Props) {
  const [entries, setEntries] = useState<TimelineEntry[] | null>(null);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    getPatientTimeline(patientId, page)
      .then((res) => {
        setEntries(res.entries);
        setTotal(res.total);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [patientId, page]);

  if (error) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo cargar el historial.</p>
        <small>{error}</small>
      </div>
    );
  }

  if (loading && entries === null) {
    return (
      <div className={styles.skeleton}>
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className={styles.skeletonRow} aria-hidden="true" />
        ))}
      </div>
    );
  }

  if (!entries || entries.length === 0) {
    return (
      <p className={styles.empty}>Este paciente aun no tiene registros.</p>
    );
  }

  return (
    <section aria-labelledby="timeline-heading">
      <h2 id="timeline-heading" className={styles.heading}>Historial</h2>
      <div className={styles.list}>
        {entries.map((e) => (
          <EntryRow key={e.id} entry={e} />
        ))}
      </div>
      {total > page * 20 && (
        <button
          className={styles.loadMore}
          onClick={() => setPage((p) => p + 1)}
          disabled={loading}
        >
          {loading ? 'Cargando...' : 'Cargar mas'}
        </button>
      )}
    </section>
  );
}
