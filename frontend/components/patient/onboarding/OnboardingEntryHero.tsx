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
  email?: string;
  onEmailChange?: (value: string) => void;
  error?: string | null;
  submitting?: boolean;
}

export function OnboardingEntryHero({
  variant = 'standard',
  professionalName,
  onStart,
  email = '',
  onEmailChange,
  error,
  submitting = false,
}: Props) {
  return (
    <div className={styles.hero}>
      <header className={styles.header}>
        <span className={styles.wordmark}>Bitacora</span>
      </header>

      <div className={styles.body}>
        {variant === 'invite' && professionalName && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompañamiento de {professionalName}
          </p>
        )}
        {variant === 'invite_fallback' && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompañamiento profesional
          </p>
        )}

        <h1 className={styles.headline}>
          Tu espacio personal de registro
        </h1>

        <p className={styles.sub}>
          Un lugar tranquilo para llevar tu registro de humor y bienestar,
          con la tranquilidad de que tus datos son privados.
        </p>

        {onStart ? (
          <form
            className={styles.form}
            onSubmit={(event) => {
              event.preventDefault();
              if (!submitting) onStart();
            }}
            noValidate
          >
            <label className={styles.emailLabel} htmlFor="onboarding-email">
              Tu correo electrónico
            </label>
            <input
              id="onboarding-email"
              name="email"
              type="email"
              autoComplete="email"
              inputMode="email"
              required
              className={styles.emailInput}
              value={email}
              onChange={(event) => onEmailChange?.(event.target.value)}
              aria-invalid={Boolean(error)}
              aria-describedby={error ? 'onboarding-email-error' : undefined}
              placeholder="tu@correo.com"
              disabled={submitting}
            />
            {error && (
              <p id="onboarding-email-error" role="alert" className={styles.errorMessage}>
                {error}
              </p>
            )}
            <div className={styles.ctaStack}>
              <button
                type="submit"
                className={styles.primaryCta}
                disabled={submitting}
              >
                {submitting ? 'Abriendo...' : 'Empezar ahora'}
              </button>
            </div>
          </form>
        ) : (
          <div className={styles.ctaStack}>
            <Link href="/onboarding" className={styles.primaryCta}>
              Empezar ahora
            </Link>
          </div>
        )}

        <p className={styles.privacyNote}>
          La privacidad de tus datos es fundamental. Nadie más puede ver lo que registrás.
        </p>
      </div>

      <footer className={styles.footer}>
        <p className={styles.footerText}>¿Ya tienes cuenta?</p>
        <a href="/ingresar" className={styles.footerLink}>Ingresar</a>
      </footer>
    </div>
  );
}
