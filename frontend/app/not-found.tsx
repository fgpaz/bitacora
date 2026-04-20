import Link from 'next/link';
import styles from './AppState.module.css';

export default function NotFound() {
  return (
    <main className={styles.statePage}>
      <section className={styles.panel} aria-labelledby="not-found-title">
        <p className={styles.kicker}>404</p>
        <h1 id="not-found-title" className={styles.title}>Página no encontrada</h1>
        <p className={styles.text}>La ruta no está disponible o cambió de lugar.</p>
        <Link
          href="/"
          className={styles.secondaryLink}
        >
          Volver al inicio
        </Link>
      </section>
    </main>
  );
}
