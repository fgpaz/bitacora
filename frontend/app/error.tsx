'use client';

import styles from './AppState.module.css';

export default function Error({
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <main className={styles.statePage}>
      <section className={styles.panel} aria-labelledby="app-error-title">
        <h1 id="app-error-title" className={styles.title}>Algo salió mal</h1>
        <p className={styles.text}>
          Ocurrió un error inesperado. Intentá de nuevo.
        </p>
        <button
          onClick={reset}
          className={styles.primaryButton}
        >
          Reintentar
        </button>
      </section>
    </main>
  );
}
