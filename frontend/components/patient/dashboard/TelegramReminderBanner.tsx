'use client';

/**
 * TelegramReminderBanner — dashboard nudge to link Telegram when not yet linked.
 * Hidden when session.linked === true or when the user dismissed it
 * within the last 30 days.
 */
import Link from 'next/link';
import { useEffect, useState } from 'react';
import { getTelegramSession } from '@/lib/api/client';
import styles from './TelegramReminderBanner.module.css';

const DISMISS_KEY = 'bitacora.telegram.banner.dismissedAt';
const DISMISS_WINDOW_MS = 30 * 24 * 60 * 60 * 1000;

function wasRecentlyDismissed(): boolean {
  if (typeof window === 'undefined') return false;
  const raw = window.localStorage.getItem(DISMISS_KEY);
  if (!raw) return false;
  const dismissedAt = Number.parseInt(raw, 10);
  if (Number.isNaN(dismissedAt)) return false;
  return Date.now() - dismissedAt < DISMISS_WINDOW_MS;
}

export function TelegramReminderBanner() {
  const [visible, setVisible] = useState(false);
  const [dismissAnnounce, setDismissAnnounce] = useState<string>('');

  useEffect(() => {
    let cancelled = false;
    async function check() {
      if (wasRecentlyDismissed()) return;
      try {
        const session = await getTelegramSession();
        if (!cancelled && session.linked === false) {
          setVisible(true);
        }
      } catch {
        // Silencioso: el banner es un nudge opcional, no bloquea UX.
      }
    }
    check();
    return () => {
      cancelled = true;
    };
  }, []);

  function handleDismiss() {
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(DISMISS_KEY, Date.now().toString());
    }
    setDismissAnnounce('Recordatorio descartado por 30 días.');
    setTimeout(() => setVisible(false), 100);
  }

  if (!visible) {
    return (
      <span role="status" aria-live="polite" className="visually-hidden">
        {dismissAnnounce}
      </span>
    );
  }

  return (
    <aside className={styles.banner} role="complementary" aria-label="Conectar Telegram">
      <div className={styles.content}>
        <p className={styles.title}>Recibí recordatorios por Telegram</p>
        <p className={styles.description}>Tarda un minuto. El recordatorio te llega a Telegram.</p>
      </div>
      <div className={styles.actions}>
        <Link href="/configuracion/telegram" className={styles.primary}>
          Conectar
        </Link>
        <button type="button" onClick={handleDismiss} className={styles.secondary} aria-label="Descartar recordatorio por 30 días">
          Ahora no
        </button>
      </div>
    </aside>
  );
}
