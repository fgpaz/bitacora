'use client';

/**
 * DailyCheckinForm — REG-002: daily factor check-in with grouped blocks.
 * States: idle | submitting | success | error | consent | session
 */
import { useState } from 'react';
import Link from 'next/link';
import { upsertDailyCheckin, DailyCheckinRequest } from '@/lib/api/client';
import { getAccessToken } from '@/lib/auth/client';
import { PatientPageShell } from '@/components/ui/PatientPageShell';
import { InlineFeedback } from '@/components/ui/InlineFeedback';
import styles from './DailyCheckinForm.module.css';

type Phase = 'idle' | 'submitting' | 'success' | 'error' | 'consent' | 'session';

interface FormData {
  sleep_hours: number | null;
  physical_activity: boolean;
  social_activity: boolean;
  anxiety: boolean;
  irritability: boolean;
  medication_taken: boolean;
  medication_time: string;
}

export function DailyCheckinForm() {
  const [form, setForm] = useState<FormData>({
    sleep_hours: null,
    physical_activity: false,
    social_activity: false,
    anxiety: false,
    irritability: false,
    medication_taken: false,
    medication_time: '',
  });
  const [phase, setPhase] = useState<Phase>('idle');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [traceId, setTraceId] = useState<string | undefined>();
  const [blockErrors, setBlockErrors] = useState<Record<string, string>>({});

  const allFilled =
    form.sleep_hours !== null &&
    form.physical_activity !== undefined &&
    form.social_activity !== undefined &&
    form.anxiety !== undefined &&
    form.irritability !== undefined &&
    form.medication_taken !== undefined &&
    (!form.medication_taken || form.medication_time !== '');

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!allFilled) return;

    setPhase('submitting');
    setErrorMessage(null);
    setBlockErrors({});

    const payload: DailyCheckinRequest = {
      sleep_hours: form.sleep_hours ?? 0,
      physical_activity: form.physical_activity,
      social_activity: form.social_activity,
      anxiety: form.anxiety,
      irritability: form.irritability,
      medication_taken: form.medication_taken,
      ...(form.medication_taken && form.medication_time
        ? { medication_time: form.medication_time }
        : {}),
    };

    try {
      const token = await getAccessToken();
      if (!token) {
        setPhase('session');
        return;
      }
      await upsertDailyCheckin(payload);
      setPhase('success');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      const trace = (err as { trace_id?: string }).trace_id;
      setTraceId(trace);
      if (code === 'CONSENT_REQUIRED') {
        setPhase('consent');
      } else if (code === 'INVALID_SLEEP_HOURS') {
        setBlockErrors({ sleep: 'Las horas de sueno deben estar entre 0 y 24.' });
        setPhase('error');
      } else if (code === 'MISSING_MEDICATION_TIME') {
        setBlockErrors({ medication: 'Completá el horario de medicacion.' });
        setPhase('error');
      } else if (code === 'ENCRYPTION_FAILURE') {
        setErrorMessage('No se pudo guardar. Intentá de nuevo.');
        setPhase('error');
      } else if (code === 'UNAUTHORIZED') {
        setPhase('session');
      } else {
        setErrorMessage('Ocurrio un error. Intentá de nuevo.');
        setPhase('error');
      }
    }
  }

  if (phase === 'consent') {
    return (
      <PatientPageShell>
        <div className={styles.redirectState}>
          <p className={styles.redirectText}>
            Necesitas aceptar el consentimiento para continuar.
          </p>
          <Link href="/consent" className={styles.redirectLink}>
            Ir al consentimiento
          </Link>
        </div>
      </PatientPageShell>
    );
  }

  if (phase === 'session') {
    return (
      <PatientPageShell>
        <div className={styles.redirectState}>
          <p className={styles.redirectText}>Tu sesion caduco. Por favor, ingresá de nuevo.</p>
          <Link href="/ingresar" className={styles.redirectLink}>Ingresar</Link>
        </div>
      </PatientPageShell>
    );
  }

  if (phase === 'success') {
    return (
      <PatientPageShell>
        <div className={styles.successState} role="status" aria-live="polite">
          <p className={styles.successText}>Check-in guardado.</p>
          <Link href="/" className={styles.backLink}>Volver al inicio</Link>
        </div>
      </PatientPageShell>
    );
  }

  return (
    <PatientPageShell>
      <form onSubmit={handleSubmit} className={styles.form}>
        <h1 className={styles.headline}>Completá tu check-in de hoy</h1>

        {/* Sleep block */}
        <div className={`${styles.block} ${blockErrors.sleep ? styles.blockError : ''}`}>
          <label className={styles.blockLabel} htmlFor="sleep_hours">
            Horas de sueno
          </label>
          <input
            id="sleep_hours"
            type="number"
            min={0}
            max={24}
            step={0.5}
            className={styles.numberInput}
            value={form.sleep_hours ?? ''}
            onChange={(e) =>
              setForm((f) => ({ ...f, sleep_hours: parseFloat(e.target.value) || null }))
            }
            aria-describedby={blockErrors.sleep ? 'sleep-error' : undefined}
          />
          {blockErrors.sleep && (
            <span id="sleep-error" className={styles.blockErrorText}>{blockErrors.sleep}</span>
          )}
        </div>

        {/* Boolean blocks */}
        {([
          { key: 'physical_activity', label: 'Actividad fisica' },
          { key: 'social_activity', label: 'Actividad social' },
          { key: 'anxiety', label: 'Ansiedad' },
          { key: 'irritability', label: 'Irritabilidad' },
        ] as const).map(({ key, label }) => (
          <div key={key} className={styles.block}>
            <span className={styles.blockLabel}>{label}</span>
            <div className={styles.boolRow} role="group" aria-label={label}>
              <button
                type="button"
                className={`${styles.boolBtn} ${form[key] ? styles.boolSelected : ''}`}
                onClick={() => setForm((f) => ({ ...f, [key]: true }))}
              >
                Si
              </button>
              <button
                type="button"
                className={`${styles.boolBtn} ${!form[key] ? styles.boolSelected : ''}`}
                onClick={() => setForm((f) => ({ ...f, [key]: false }))}
              >
                No
              </button>
            </div>
          </div>
        ))}

        {/* Medication block */}
        <div className={styles.block}>
          <span className={styles.blockLabel}>¿Tomaste medicacion hoy?</span>
          <div className={styles.boolRow} role="group" aria-label="Medicacion">
            <button
              type="button"
              className={`${styles.boolBtn} ${form.medication_taken ? styles.boolSelected : ''}`}
              onClick={() => setForm((f) => ({ ...f, medication_taken: true }))}
            >
              Si
            </button>
            <button
              type="button"
              className={`${styles.boolBtn} ${!form.medication_taken ? styles.boolSelected : ''}`}
              onClick={() => setForm((f) => ({ ...f, medication_taken: false, medication_time: '' }))}
            >
              No
            </button>
          </div>

          {form.medication_taken && (
            <div className={styles.medicationTime}>
              <label className={styles.blockLabel} htmlFor="medication_time">
                Horario aproximado
              </label>
              <input
                id="medication_time"
                type="time"
                className={styles.timeInput}
                value={form.medication_time}
                onChange={(e) => setForm((f) => ({ ...f, medication_time: e.target.value }))}
                aria-describedby={blockErrors.medication ? 'med-error' : undefined}
              />
              {blockErrors.medication && (
                <span id="med-error" className={styles.blockErrorText}>{blockErrors.medication}</span>
              )}
            </div>
          )}
        </div>

        {phase === 'error' && errorMessage && (
          <InlineFeedback variant="error" message={errorMessage} traceId={traceId} />
        )}

        <div className={styles.submitBar}>
          <button
            type="submit"
            className={styles.submitBtn}
            disabled={!allFilled || phase === 'submitting'}
            aria-busy={phase === 'submitting'}
          >
            {phase === 'submitting' ? 'Guardando...' : 'Guardar check-in'}
          </button>
        </div>
      </form>
    </PatientPageShell>
  );
}
