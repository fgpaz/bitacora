'use client';

/**
 * MoodEntryForm — REG-001: mood score capture and submission.
 * Variants: page (default, standalone route) | embedded (inside a dialog).
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

interface Props {
  embedded?: boolean;
  onSaved?: () => void;
}

export function MoodEntryForm({ embedded = false, onSaved }: Props = {}) {
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
      if (onSaved) onSaved();
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
        setErrorMessage('No pudimos guardar el registro. Probá de nuevo.');
        setPhase('error');
      }
    }
  }

  const consentBlock = (
    <div className={styles.consentState}>
      <p className={styles.consentText}>
        Necesitás aceptar el consentimiento antes de registrar tu humor.
      </p>
      <Link href="/consent" className={styles.consentLink}>
        Ir al consentimiento
      </Link>
    </div>
  );

  const sessionBlock = (
    <div className={styles.consentState}>
      <p className={styles.consentText}>Tu sesión caducó. Ingresá de nuevo.</p>
      <a href="/ingresar" className={styles.consentLink}>Ingresar</a>
    </div>
  );

  const successBlock = (
    <div className={styles.successState} role="status" aria-live="polite">
      <p className={styles.successText}>Registro guardado.</p>
      {!embedded && (
        <>
          <Link href="/registro/daily-checkin" className={styles.nextLink}>
            Completar check-in diario
          </Link>
          <Link href="/dashboard" className={styles.backLink}>Volver al dashboard</Link>
        </>
      )}
    </div>
  );

  const formBlock = (
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
  );

  const content =
    phase === 'consent' ? consentBlock :
    phase === 'session' ? sessionBlock :
    phase === 'success' ? successBlock :
    formBlock;

  if (embedded) return content;
  return <PatientPageShell>{content}</PatientPageShell>;
}
