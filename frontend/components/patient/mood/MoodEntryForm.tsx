'use client';

/**
 * MoodEntryForm — REG-001: mood score capture and submission.
 * States: S02-DEFAULT | S02-SUBMITTING | S02-SUCCESS | S02-ERROR | S02-CONSENT | S02-SESSION
 */
import Link from 'next/link';
import { useState } from 'react';
import { createMoodEntry } from '@/lib/api/client';
import { getAccessToken } from '@/lib/auth/client';
import { MoodScale } from './MoodScale';
import { InlineFeedback } from '@/components/ui/InlineFeedback';
import { PatientPageShell } from '@/components/ui/PatientPageShell';
import styles from './MoodEntryForm.module.css';

type Phase = 'idle' | 'submitting' | 'success' | 'error' | 'consent' | 'session';

export function MoodEntryForm() {
  const [score, setScore] = useState<number | null>(null);
  const [phase, setPhase] = useState<Phase>('idle');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [traceId, setTraceId] = useState<string | undefined>();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (score === null) return;

    setPhase('submitting');
    setErrorMessage(null);

    try {
      const token = await getAccessToken();
      if (!token) {
        setPhase('session');
        return;
      }
      await createMoodEntry(score);
      setPhase('success');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      const trace = (err as { trace_id?: string }).trace_id;
      setTraceId(trace);
      if (code === 'CONSENT_REQUIRED') {
        setPhase('consent');
      } else if (code === 'INVALID_SCORE') {
        setErrorMessage('El valor elegido no es válido. Intentá de nuevo.');
        setPhase('error');
      } else if (code === 'ENCRYPTION_FAILURE') {
        setErrorMessage('No se pudo guardar el registro. Intentá de nuevo.');
        setPhase('error');
      } else if (code === 'UNAUTHORIZED' || code === 'ONB_001_JWT_INVALID') {
        setPhase('session');
      } else {
        setErrorMessage('Ocurrió un error. Intentá de nuevo.');
        setPhase('error');
      }
    }
  }

  if (phase === 'consent') {
    return (
      <PatientPageShell>
        <div className={styles.consentState}>
          <p className={styles.consentText}>
            Necesitás aceptar el consentimiento antes de registrar tu humor.
          </p>
          <Link href="/consent" className={styles.consentLink}>
            Ir al consentimiento
          </Link>
        </div>
      </PatientPageShell>
    );
  }

  if (phase === 'session') {
    return (
      <PatientPageShell>
        <div className={styles.consentState}>
          <p className={styles.consentText}>
            Tu sesión caducó. Ingresá de nuevo.
          </p>
          <a href="/ingresar" className={styles.consentLink}>
            Ingresar
          </a>
        </div>
      </PatientPageShell>
    );
  }

  if (phase === 'success') {
    return (
      <PatientPageShell>
        <div className={styles.successState} role="status" aria-live="polite">
          <p className={styles.successText}>Registro guardado.</p>
          <Link href="/registro/daily-checkin" className={styles.nextLink}>
            Completar check-in diario
          </Link>
          <Link href="/" className={styles.backLink}>Volver al inicio</Link>
        </div>
      </PatientPageShell>
    );
  }

  return (
    <PatientPageShell>
      <form onSubmit={handleSubmit} className={styles.form}>
        <h1 className={styles.headline}>¿Cómo te sentís ahora?</h1>

        <MoodScale
          value={score}
          onChange={setScore}
          disabled={phase === 'submitting'}
        />

        {phase === 'error' && errorMessage && (
          <InlineFeedback
            variant="error"
            message={errorMessage}
            traceId={traceId}
          />
        )}

        <div className={styles.submitArea}>
          <button
            type="submit"
            className={styles.submitBtn}
            disabled={score === null || phase === 'submitting'}
            aria-busy={phase === 'submitting'}
          >
            {phase === 'submitting' ? 'Guardando…' : 'Guardar'}
          </button>
        </div>
      </form>
    </PatientPageShell>
  );
}
