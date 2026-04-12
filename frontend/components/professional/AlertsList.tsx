'use client';

/**
 * AlertsList — displays patient alerts with severity indicators.
 * Alerts are read-only for the professional role.
 */
import { useEffect, useState } from 'react';
import { getPatientAlerts } from '@/lib/api/professional';
import type { Alert } from '@/lib/api/professional';
import styles from './AlertsList.module.css';

interface Props {
  patientId: string;
}

function AlertRow({ alert }: { alert: Alert }) {
  return (
    <div className={`${styles.row} ${styles[alert.severity]}`} role="listitem">
      <div className={styles.rowHeader}>
        <span className={`${styles.severityBadge} ${styles[alert.severity]}`}>
          {alert.severity === 'high' ? 'Alto' : alert.severity === 'medium' ? 'Medio' : 'Bajo'}
        </span>
        <span className={styles.alertType}>{alert.type}</span>
        <time className={styles.alertDate}>
          {new Date(alert.created_at).toLocaleDateString('es-AR', { dateStyle: 'medium' })}
        </time>
      </div>
      <p className={styles.message}>{alert.message}</p>
    </div>
  );
}

export function AlertsList({ patientId }: Props) {
  const [alerts, setAlerts] = useState<Alert[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getPatientAlerts(patientId)
      .then((res) => setAlerts(res.alerts))
      .catch((err: Error) => setError(err.message));
  }, [patientId]);

  if (error) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudieron cargar las alertas.</p>
        <small>{error}</small>
      </div>
    );
  }

  if (alerts === null) {
    return (
      <div className={styles.skeleton}>
        {Array.from({ length: 3 }).map((_, i) => (
          <div key={i} className={styles.skeletonRow} aria-hidden="true" />
        ))}
      </div>
    );
  }

  if (alerts.length === 0) {
    return (
      <p className={styles.empty}>
        No hay alertas activas para este paciente.
      </p>
    );
  }

  return (
    <section aria-labelledby="alerts-heading">
      <h2 id="alerts-heading" className={styles.heading}>
        Alertas
        <span className={styles.count}>{alerts.length}</span>
      </h2>
      <div className={styles.list} role="list">
        {alerts.map((a) => (
          <AlertRow key={a.id} alert={a} />
        ))}
      </div>
    </section>
  );
}
