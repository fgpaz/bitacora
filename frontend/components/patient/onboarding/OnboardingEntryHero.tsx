import Link from 'next/link';
import styles from './OnboardingEntryHero.module.css';

interface Props {
  variant?: 'standard' | 'invite' | 'invite_fallback';
  professionalName?: string;
}

export function OnboardingEntryHero({ variant = 'standard', professionalName }: Props) {
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

        <h1 className={styles.headline}>Tu espacio personal de registro</h1>

        <p className={styles.sub}>
          Un lugar tranquilo para llevar tu registro de humor y bienestar,
          con la tranquilidad de que tus datos son privados.
        </p>

        <div className={styles.ctaStack}>
          <Link href="/ingresar" className={styles.primaryCta}>
            Ingresar
          </Link>
        </div>

        <p className={styles.privacyNote}>
          La privacidad de tus datos es fundamental. Nadie más puede ver lo que registrás.
        </p>
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
