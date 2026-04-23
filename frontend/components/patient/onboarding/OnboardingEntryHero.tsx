import Link from 'next/link';
import styles from './OnboardingEntryHero.module.css';

interface Props {
  variant?: 'standard' | 'invite' | 'invite_fallback' | 'returning';
  professionalName?: string;
  message?: string;
}

export function OnboardingEntryHero({ variant = 'standard', professionalName, message }: Props) {
  const isReturning = variant === 'returning';

  return (
    <div className={styles.hero}>
      <header className={styles.header}>
        <span className={styles.wordmark}>Bitácora</span>
      </header>

      <div className={styles.body}>
        {isReturning ? (
          <>
            <h1 className={styles.headline}>Volviste.</h1>
            <p className={styles.sub}>Seguí donde dejaste.</p>

            <div className={styles.ctaStack}>
              <Link href="/dashboard" className={styles.primaryCta}>
                Seguir registrando
              </Link>
            </div>

            <p className={styles.privacyNote}>
              Solo vos ves lo que registrás. Tus datos son privados.
            </p>
          </>
        ) : (
          <>
            <h1 className={styles.headline}>Tu espacio personal de registro</h1>

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

            <p className={styles.sub}>
              Un lugar sobrio para llevar tu registro de humor y bienestar,
              con la seguridad de que tus datos son privados.
            </p>

            <div className={styles.ctaStack}>
              <Link href="/ingresar" className={styles.primaryCta}>
                Ingresar
              </Link>
            </div>

            <p className={styles.privacyNote}>
              Solo vos ves lo que registrás. Tus datos son privados.
            </p>
          </>
        )}

        {message && (
          <p className={styles.heroMessage} role="status" aria-live="polite">
            {message}
          </p>
        )}
      </div>

      <footer className={styles.footer}>
        <p className={styles.footerText}>¿Problemas para acceder?</p>
        <a href="mailto:soporte@nuestrascuentitas.com" className={styles.footerLink}>
          Contactar soporte
        </a>
      </footer>
    </div>
  );
}
