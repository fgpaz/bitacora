'use client';

/**
 * ConsentRevocationPanel — UI dedicada para revocación del consent activo.
 *
 * Slice 23_uxui: CON-002. UXS-CON-002 §S02 "Revisión del impacto antes de revocar".
 *
 * Estados: default | submitting | success | error_retryable.
 * Ley 26.529 Art. 10 revocabilidad + Art. 2 inc. e) autonomía.
 */

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { getCurrentConsent, revokeConsent, type ConsentCurrentResponse } from '@/lib/api/client';
import { formatUserFacingError, type UserFacingError } from '@/lib/errors/user-facing';
import styles from './ConsentRevocationPanel.module.css';

type Phase = 'loading' | 'default' | 'submitting' | 'success' | 'error';

export function ConsentRevocationPanel() {
  const router = useRouter();
  const [phase, setPhase] = useState<Phase>('loading');
  const [consent, setConsent] = useState<ConsentCurrentResponse | null>(null);
  const [error, setError] = useState<UserFacingError | null>(null);
  const [revokedAt, setRevokedAt] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    async function load() {
      try {
        const data = await getCurrentConsent();
        if (cancelled) return;
        setConsent(data);
        if (data.patientStatus === 'revoked') {
          setPhase('success');
        } else if (data.patientStatus === 'granted') {
          setPhase('default');
        } else {
          setPhase('default');
        }
      } catch (err: unknown) {
        if (cancelled) return;
        setError(
          formatUserFacingError(err, {
            fallback: {
              title: 'No pudimos cargar tu consentimiento.',
              description: 'Probá de nuevo en unos minutos.',
            },
          }),
        );
        setPhase('error');
      }
    }
    load();
    return () => {
      cancelled = true;
    };
  }, []);

  async function handleRevoke() {
    setPhase('submitting');
    setError(null);
    try {
      const result = await revokeConsent(true);
      setRevokedAt(result.revokedAtUtc);
      setPhase('success');
    } catch (err: unknown) {
      setError(
        formatUserFacingError(err, {
          fallback: {
            title: 'No pudimos revocar este consentimiento.',
            description: 'Probá de nuevo.',
          },
        }),
      );
      setPhase('error');
    }
  }

  function handleKeep() {
    router.push('/dashboard');
  }

  if (phase === 'loading') {
    return (
      <div className={styles.panel} aria-busy="true" aria-label="Cargando consentimiento">
        <div className={styles.skeleton} aria-hidden="true" />
      </div>
    );
  }

  if (phase === 'error' && !consent) {
    return (
      <div className={styles.panel}>
        <div className={styles.errorState} role="alert">
          <p className={styles.errorTitle}>{error?.title ?? 'No pudimos cargar tu consentimiento.'}</p>
          <p className={styles.errorDescription}>{error?.description ?? 'Probá de nuevo en unos minutos.'}</p>
          <button
            type="button"
            className={styles.secondaryBtn}
            onClick={() => router.push('/dashboard')}
          >
            Volver al dashboard
          </button>
        </div>
      </div>
    );
  }

  if (phase === 'success') {
    return (
      <div className={styles.panel}>
        <header className={styles.header}>
          <h1 className={styles.title}>Consentimiento revocado</h1>
        </header>
        <p className={styles.successText} role="status" aria-live="polite">
          {revokedAt
            ? 'Listo. Tu consentimiento quedó revocado.'
            : 'Tu consentimiento ya estaba revocado.'}
        </p>
        <p className={styles.impactNote}>
          Nuevos registros quedan suspendidos y los profesionales vinculados pierden acceso a tus datos.
          Podés volver a otorgar el consentimiento cuando quieras.
        </p>
        <div className={styles.successActions}>
          <button
            type="button"
            className={styles.primaryBtn}
            onClick={() => router.push('/dashboard')}
          >
            Volver al dashboard
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.panel}>
      <header className={styles.header}>
        <h1 className={styles.title}>Revocar consentimiento</h1>
        {consent && <p className={styles.version}>Versión activa {consent.version}</p>}
      </header>

      <section className={styles.impact} aria-labelledby="impact-heading">
        <h2 id="impact-heading" className={styles.sectionTitle}>Qué pasa si seguís</h2>
        <ul className={styles.impactList}>
          <li>Se suspende el registro de nuevos datos en Bitácora.</li>
          <li>Los profesionales vinculados pierden acceso a tus datos.</li>
          <li>Tu historial anterior queda guardado; no se borra.</li>
          <li>Podés volver a otorgar el consentimiento cuando quieras.</li>
        </ul>
      </section>

      {error && phase === 'error' && (
        <div className={styles.inlineError} role="alert">
          <p className={styles.errorTitle}>{error.title}</p>
          <p className={styles.errorDescription}>{error.description}</p>
        </div>
      )}

      <div className={styles.decisionBar}>
        <button
          type="button"
          className={styles.secondaryBtn}
          onClick={handleKeep}
          disabled={phase === 'submitting'}
        >
          Conservar consentimiento
        </button>
        <button
          type="button"
          className={styles.revokeBtn}
          onClick={handleRevoke}
          disabled={phase === 'submitting'}
          aria-busy={phase === 'submitting'}
        >
          {phase === 'submitting' ? 'Revocando…' : 'Revocar consentimiento'}
        </button>
      </div>
    </div>
  );
}
