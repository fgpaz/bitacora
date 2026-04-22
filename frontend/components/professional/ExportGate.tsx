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
 *
 * UI-RFC-EXP-001 allowed sub-states:
 * - S01-DEFAULT  : period selector visible, CTA enabled
 * - S01-PERIOD   : period changed, scope text updated
 * - S02-GENERATING: brief loading on CTA while preparing download
 * - S02-SUCCESS  : browser handles download (no special UI)
 * - S03-ERROR    : error with retry
 */
import { useEffect, useState, useCallback } from 'react';
import {
  getExportConstraints,
  downloadExportCsv,
  type ExportConstraint,
  type PeriodPreset,
  type PeriodSelection,
} from '@/lib/api/professional';
import styles from './ExportGate.module.css';

interface Props {
  patientId: string;
}

type AllowedState = 'default' | 'period' | 'generating' | 'error';

/* ─── Helpers ─────────────────────────────────────────────────────────── */

function toDateInputValue(d: Date): string {
  return d.toISOString().split('T')[0]!;
}

function today(): Date {
  const d = new Date();
  d.setHours(12, 0, 0, 0);
  return d;
}

function daysAgo(n: number): Date {
  const d = today();
  d.setDate(d.getDate() - n);
  return d;
}

function presetRange(preset: PeriodPreset): { from: Date; to: Date } {
  switch (preset) {
    case '7d':  return { from: daysAgo(6),  to: today() };
    case '30d': return { from: daysAgo(29), to: today() };
    case '90d': return { from: daysAgo(89), to: today() };
    default:    return { from: daysAgo(6),  to: today() };
  }
}

function formatRange(from: string, to: string): string {
  const f = new Date(from + 'T12:00:00');
  const t = new Date(to   + 'T12:00:00');
  const fmt = { day: '2-digit', month: 'short', year: 'numeric' } as const;
  return `${f.toLocaleDateString('es-AR', fmt)} – ${t.toLocaleDateString('es-AR', fmt)}`;
}

/* ─── Preset buttons ─────────────────────────────────────────────────── */

const PRESETS: { label: string; value: PeriodPreset }[] = [
  { label: 'Últimos 7 días',  value: '7d'  },
  { label: 'Últimos 30 días', value: '30d' },
  { label: 'Últimos 90 días', value: '90d' },
  { label: 'Personalizado',   value: 'custom' },
];

/* ─── ExportActionBar ────────────────────────────────────────────────── */

interface ExportActionBarProps {
  selection: PeriodSelection;
  scopeText: string;
  downloadState: 'idle' | 'generating' | 'error';
  errorMessage: string | null;
  onDownload: () => void;
  onRetry: () => void;
}

function ExportActionBar({
  selection,
  scopeText,
  downloadState,
  errorMessage,
  onDownload,
  onRetry,
}: ExportActionBarProps) {
  return (
    <div className={styles.actionBar}>
      <ExportScopeBlock scopeText={scopeText} />
      <div className={styles.actionRow}>
        {downloadState === 'error' ? (
          <div className={styles.errorInline} role="alert">
            <span>{errorMessage}</span>
            <button className={styles.retryBtn} onClick={onRetry} type="button">
              Reintentar
            </button>
          </div>
        ) : (
          <button
            className={styles.downloadBtn}
            onClick={onDownload}
            disabled={downloadState === 'generating'}
            type="button"
            aria-busy={downloadState === 'generating'}
          >
            {downloadState === 'generating' ? 'Preparando...' : 'Descargar CSV'}
          </button>
        )}
      </div>
    </div>
  );
}

/* ─── ExportScopeBlock ────────────────────────────────────────────────── */

interface ExportScopeBlockProps {
  scopeText: string;
}

function ExportScopeBlock({ scopeText }: ExportScopeBlockProps) {
  return (
    <div className={styles.scopeBlock} aria-live="polite" aria-atomic="true">
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" aria-hidden="true" className={styles.scopeIcon}>
        <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
      </svg>
      <span className={styles.scopeText}>{scopeText}</span>
    </div>
  );
}

/* ─── PeriodSelector ──────────────────────────────────────────────────── */

interface PeriodSelectorProps {
  selection: PeriodSelection;
  onChange: (s: PeriodSelection) => void;
}

function PeriodSelector({ selection, onChange }: PeriodSelectorProps) {
  const handlePreset = (preset: PeriodPreset) => {
    if (preset === 'custom') {
      onChange({ preset, from: selection.from, to: selection.to });
    } else {
      const { from, to } = presetRange(preset);
      onChange({ preset, from: toDateInputValue(from), to: toDateInputValue(to) });
    }
  };

  const handleFrom = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange({ preset: 'custom', from: e.target.value, to: selection.to });
  };

  const handleTo = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange({ preset: 'custom', from: selection.from, to: e.target.value });
  };

  return (
    <div className={styles.periodSelector} role="group" aria-label="Seleccionar período">
      <div className={styles.presets}>
        {PRESETS.map(({ label, value }) => (
          <button
            key={value}
            className={`${styles.presetBtn} ${selection.preset === value ? styles.presetBtnActive : ''}`}
            onClick={() => handlePreset(value)}
            type="button"
            aria-pressed={selection.preset === value}
          >
            {label}
          </button>
        ))}
      </div>
      {selection.preset === 'custom' && (
        <div className={styles.customRange}>
          <label className={styles.dateLabel} htmlFor="period-from">Desde</label>
          <input
            id="period-from"
            type="date"
            className={styles.dateInput}
            value={selection.from}
            onChange={handleFrom}
            max={selection.to}
          />
          <label className={styles.dateLabel} htmlFor="period-to">Hasta</label>
          <input
            id="period-to"
            type="date"
            className={styles.dateInput}
            value={selection.to}
            onChange={handleTo}
            min={selection.from}
            max={toDateInputValue(today())}
          />
        </div>
      )}
    </div>
  );
}

/* ─── Main component ──────────────────────────────────────────────────── */

export function ExportGate({ patientId }: Props) {
  const [constraint, setConstraint] = useState<ExportConstraint | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [allowedState, setAllowedState] = useState<AllowedState>('default');
  const [downloadError, setDownloadError] = useState<string | null>(null);

  // Period selection state — initialised to 7d
  const [selection, setSelection] = useState<PeriodSelection>(() => {
    const { from, to } = presetRange('7d');
    return { preset: '7d', from: toDateInputValue(from), to: toDateInputValue(to) };
  });

  useEffect(() => {
    getExportConstraints(patientId)
      .then((c) => {
        setConstraint(c);
        setLoading(false);
      })
      .catch((err) => {
        setError(err instanceof Error ? err.message : 'Error desconocido');
        setLoading(false);
      });
  }, [patientId]);

  const scopeText = `Registros del ${formatRange(selection.from, selection.to)}`;

  const handlePeriodChange = useCallback((s: PeriodSelection) => {
    setSelection(s);
    setAllowedState('period');
    setDownloadError(null);
  }, []);

  const handleDownload = useCallback(async () => {
    setAllowedState('generating');
    setDownloadError(null);
    try {
      await downloadExportCsv(patientId, selection.from, selection.to);
      setAllowedState('default');
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Error al preparar la descarga';
      setDownloadError(msg);
      setAllowedState('error');
    }
  }, [patientId, selection]);

  const handleRetry = useCallback(() => {
    setDownloadError(null);
    setAllowedState('default');
  }, []);

  if (loading) {
    return <div className={styles.skeleton} aria-hidden="true" />;
  }

  if (error) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo verificar el acceso a exportación.</p>
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
        <h3 className={styles.deniedTitle}>Exportación no disponible</h3>
        <p className={styles.deniedReason}>{constraint.reason ?? 'No tenés permisos para exportar los datos de este paciente.'}</p>
        <p className={styles.deniedHint}>
          Si necesitas una copia de los registros, solicitale a tu paciente que la descargue desde su cuenta.
        </p>
      </div>
    );
  }

  // ── Allowed state ────────────────────────────────────────────────────

  const isGenerating = allowedState === 'generating';
  const isError     = allowedState === 'error';

  return (
    <div className={styles.allowed}>
      <div className={styles.header}>
        <h2 className={styles.title}>Exportar registros</h2>
        <p className={styles.subtitle}>Descargate una copia de tus registros en formato CSV.</p>
      </div>

      <PeriodSelector selection={selection} onChange={handlePeriodChange} />

      <div className={styles.selectedRange} aria-live="polite">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" aria-hidden="true" className={styles.rangeIcon}>
          <path d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
        <span>{scopeText}</span>
      </div>

      <ExportActionBar
        selection={selection}
        scopeText={scopeText}
        downloadState={isGenerating ? 'generating' : isError ? 'error' : 'idle'}
        errorMessage={downloadError}
        onDownload={handleDownload}
        onRetry={handleRetry}
      />
    </div>
  );
}
