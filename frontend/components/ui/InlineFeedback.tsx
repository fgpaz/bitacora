'use client';

/**
 * InlineFeedback — contextual message for confirmations and errors.
 * Variants: info | error | confirm
 */
import styles from './InlineFeedback.module.css';

interface Props {
  variant: 'info' | 'error' | 'confirm';
  message: string;
  traceId?: string;
}

export function InlineFeedback({ variant, message, traceId }: Props) {
  return (
    <div
      className={`${styles.root} ${styles[variant]}`}
      role={variant === 'error' ? 'alert' : 'status'}
      aria-live={variant === 'error' ? 'assertive' : 'polite'}
    >
      <span className={styles.message}>{message}</span>
      {traceId && (
        <span className={styles.trace} aria-label={`Referencia: ${traceId}`}>
          {traceId}
        </span>
      )}
    </div>
  );
}
