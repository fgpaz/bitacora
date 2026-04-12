'use client';

/**
 * OnboardingEntryHero — public ONB-first landing hero.
 * Variants: standard | invite | invite_fallback
 */
import Link from 'next/link';
import styles from './OnboardingEntryHero.module.css';

interface Props {
  variant?: 'standard' | 'invite' | 'invite_fallback';
  professionalName?: string;
  onStart?: () => void;
}

export function OnboardingEntryHero({ variant = 'standard', professionalName, onStart }: Props) {
  return (
    <div className={styles.hero}>
      <header className={styles.header}>
        <span className={styles.wordmark}>Bitacora</span>
      </header>

      <div className={styles.body}>
        {variant === 'invite' && professionalName && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompanamiento de {professionalName}
          </p>
        )}
        {variant === 'invite_fallback' && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompanamiento profesional
          </p>
        )}

        <h1 className={styles.headline}>
          Tu espacio personal de registro
        </h1>

        <p className={styles.sub}>
          Un lugar tranquilo para llevar tu registro de humor y bienestar,
          con la tranquilidad de que tus datos son privados.
        </p>

        <div className={styles.ctaStack}>
          {onStart ? (
            <button type="button" className={styles.primaryCta} onClick={onStart}>
              Empezar ahora
            </button>
          ) : (
            <Link href="/onboarding" className={styles.primaryCta}>
              Empezar ahora
            </Link>
          )}
        </div>

        <p className={styles.privacyNote}>
          La privacidad de tus datos es fundamental. Nadie mas puede ver lo que registras.
        </p>
      </div>

      <footer className={styles.footer}>
        <p className={styles.footerText}>¿Ya tienes cuenta?</p>
        <Link href="/ingresar" className={styles.footerLink}>Ingresar</Link>
      </footer>
    </div>
  );
}
