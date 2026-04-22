'use client';

/**
 * PatientDetail — tabbed view: Summary | Timeline | Alerts | Export
 */
import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { getPatientSummary } from '@/lib/api/professional';
import { PatientSummaryCard } from '@/components/professional/PatientSummaryCard';
import { Timeline } from '@/components/professional/Timeline';
import { AlertsList } from '@/components/professional/AlertsList';
import { ExportGate } from '@/components/professional/ExportGate';
import styles from './PatientDetail.module.css';

type Tab = 'resumen' | 'historial' | 'alertas' | 'exportar';

export function PatientDetail() {
  const { patientId } = useParams<{ patientId: string }>();
  const [tab, setTab] = useState<Tab>('resumen');
  const [summary, setSummary] = useState<Awaited<ReturnType<typeof getPatientSummary>> | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);

  useEffect(() => {
    if (!patientId) return;
    getPatientSummary(patientId)
      .then(setSummary)
      .catch((err: Error) => setLoadError(err.message));
  }, [patientId]);

  if (loadError) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo cargar el detalle del paciente.</p>
        <small>{loadError}</small>
      </div>
    );
  }

  return (
    <div>
      <Link href="/profesional/pacientes" className={styles.back}>
        Volver a pacientes
      </Link>

      {summary && <PatientSummaryCard summary={summary} />}

      <nav className={styles.tabs} aria-label="Secciones del paciente">
        {(['resumen', 'historial', 'alertas', 'exportar'] as Tab[]).map((t) => (
          <button
            key={t}
            className={`${styles.tab} ${tab === t ? styles.activeTab : ''}`}
            onClick={() => setTab(t)}
            aria-selected={tab === t}
            role="tab"
          >
            {t === 'resumen' ? 'Resumen' : t === 'historial' ? 'Historial' : t === 'alertas' ? 'Alertas' : 'Exportar'}
          </button>
        ))}
      </nav>

      <div role="tabpanel">
        {tab === 'resumen' && summary && (
          <p className={styles.tabNote}>
            Resumen general del paciente. Último registro:{' '}
            {summary.last_entry_at
              ? new Date(summary.last_entry_at).toLocaleDateString('es-AR', { dateStyle: 'long' })
              : 'sin registros'}
          </p>
        )}
        {tab === 'historial' && <Timeline patientId={patientId!} />}
        {tab === 'alertas' && <AlertsList patientId={patientId!} />}
        {tab === 'exportar' && <ExportGate patientId={patientId!} />}
      </div>
    </div>
  );
}
