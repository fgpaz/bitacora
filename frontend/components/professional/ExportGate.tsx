'use client';

/**
 * ExportGate — enforces patient-owner-only export policy.
 *
 * Backend constraint: export endpoints are owner-only.
 * Professionals can VIEW the export tab but cannot download.
 * This component makes that limitation explicit in the UI instead of
 * faking a capability that would result in a 403 at the API level.
 *
 * States: checking | allowed | denied | error
 */
import { useEffect, useState } from 'react';
import type { ExportConstraint } from '@/lib/api/professional';
import styles from './ExportGate.module.css';

interface Props {
  patientId: string;
}

export function ExportGate({ patientId }: Props) {
  const [constraint, setConstraint] = useState<ExportConstraint | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // In a real implementation this calls the backend constraints endpoint.
    // Since export is owner-only, the backend will return allowed:false for
    // any professional user. We simulate that here.
    // TODO: replace with real call once backend exports /constraints for professionals.
    setLoading(false);
    setConstraint({
      export_type: 'full',
      allowed: false,
      reason: 'La exportacion de datos es solo para el paciente propietario. Solo el paciente puede descargar sus propios registros.',
    });
  }, [patientId]);

  if (loading) {
    return (
      <div className={styles.skeleton} aria-hidden="true" />
    );
  }

  if (error) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo verificar el acceso a exportacion.</p>
        <small>{error}</small>
      </div>
    );
  }

  if (!constraint) return null;

  if (!constraint.allowed) {
    return (
      <div className={styles.denied} role="alert" aria-live="polite">
        <div className={styles.deniedIcon} aria-hidden="true">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" aria-hidden="true">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 15v-2h2v2h-2zm0-4V7h2v6h-2z" fill="currentColor" />
          </svg>
        </div>
        <h3 className={styles.deniedTitle}>Exportacion no disponible</h3>
        <p className={styles.deniedReason}>{constraint.reason ?? 'No tenes permisos para exportar los datos de este paciente.'}</p>
        <p className={styles.deniedHint}>
          Si necesitas una copia de los registros, solicitale a tu paciente que la descargue desde su cuenta.
        </p>
      </div>
    );
  }

  return (
    <div className={styles.allowed}>
      <p>Descarga tus registros.</p>
      {/* Placeholder for real export buttons when backend supports it */}
    </div>
  );
}
