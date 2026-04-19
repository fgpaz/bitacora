'use client';

/**
 * AuthBootstrapInterstitial — brief continuity screen between OIDC auth and bootstrap.
 * States: default | invite_context
 */
import styles from './AuthBootstrapInterstitial.module.css';

interface Props {
  variant?: 'default' | 'invite_context';
}

export function AuthBootstrapInterstitial({ variant = 'default' }: Props) {
  return (
    <div className={styles.root} aria-label="Continuando..." aria-busy="true">
      <div className={styles.indicator} aria-hidden="true" />
      {variant === 'invite_context' ? (
        <p className={styles.message}>Continuando con tu registro...</p>
      ) : (
        <p className={styles.message}>Continuando...</p>
      )}
    </div>
  );
}
