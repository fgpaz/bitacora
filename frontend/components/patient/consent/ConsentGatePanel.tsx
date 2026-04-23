'use client';

/**
 * ConsentGatePanel — consent reading and acceptance panel.
 * States: ready | reminder | version_conflict | service_error | submitting
 */
import { useState } from 'react';
import { ConsentCurrentResponse } from '@/lib/api/client';
import { InlineFeedback } from '@/components/ui/InlineFeedback';
import styles from './ConsentGatePanel.module.css';

interface Props {
  consent: ConsentCurrentResponse;
  resumeInvite?: boolean;
  onAccept: (version: string) => Promise<void>;
  onRetry?: () => void;
  errorCode?: string;
  errorMessage?: string;
  traceId?: string;
}

export function ConsentGatePanel({
  consent,
  resumeInvite = false,
  onAccept,
  onRetry,
  errorCode,
  errorMessage,
  traceId,
}: Props) {
  const [submitting, setSubmitting] = useState(false);

  async function handleAccept() {
    setSubmitting(true);
    try {
      await onAccept(consent.version);
    } finally {
      setSubmitting(false);
    }
  }

  const isVersionConflict = errorCode === 'CONSENT_VERSION_MISMATCH';
  const isServiceError = errorCode === 'NO_CONSENT_CONFIG' || errorCode === 'AUDIT_WRITE_FAILED';

  return (
    <div className={styles.panel}>
      <header className={styles.header}>
        <h2 className={styles.title}>Consentimiento informado</h2>
        <p className={styles.version}>Versión {consent.version}</p>
      </header>

      <div className={styles.sections} role="list" aria-label="Secciones del consentimiento">
        {consent.sections.map((section) => (
          <div key={section.id} className={styles.section} role="listitem">
            <h3 className={styles.sectionTitle}>{section.title}</h3>
            <p className={styles.sectionContent}>{section.content}</p>
          </div>
        ))}
      </div>

      {resumeInvite && (
        <p className={styles.inviteHint}>
          Recordá que viniste a través de una invitación de tu profesional.
        </p>
      )}

      {errorMessage && (
        <InlineFeedback
          variant="error"
          message={errorMessage}
          traceId={isServiceError ? traceId : undefined}
        />
      )}

      {isServiceError && onRetry && (
        <button type="button" className={styles.retryBtn} onClick={onRetry}>
          Reintentar
        </button>
      )}

      {isVersionConflict && (
        <button type="button" className={styles.retryBtn} onClick={onRetry}>
          Volver a revisar
        </button>
      )}

      {!isServiceError && !isVersionConflict && (
        <div className={styles.decisionBar}>
          <button
            type="button"
            className={styles.acceptBtn}
            onClick={handleAccept}
            disabled={submitting}
            aria-busy={submitting}
          >
            {submitting ? 'Guardando...' : 'Aceptar y continuar'}
          </button>
        </div>
      )}
    </div>
  );
}
