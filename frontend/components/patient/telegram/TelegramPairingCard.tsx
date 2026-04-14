'use client';

/**
 * TelegramPairingCard — generates and displays a BIT-XXXXX pairing code
 * for the patient to send to @bitacorav2_bot via /start <code>.
 *
 * Implements RF-TG-001 (frontend side): pairing code generation, display,
 * copy-to-clipboard, countdown timer, and expiry/regenerate flow.
 */
import { useEffect, useRef, useState } from 'react';
import {
  generatePairingCode,
  getTelegramSession,
  type TelegramPairingResponse,
  type TelegramSessionResponse,
} from '../../../lib/api/client';
import styles from './TelegramPairingCard.module.css';

const TTL_SECONDS = 15 * 60; // 15 minutes

export function TelegramPairingCard() {
  const [session, setSession] = useState<TelegramSessionResponse | null>(null);
  const [pairing, setPairing] = useState<TelegramPairingResponse | null>(null);
  const [secondsLeft, setSecondsLeft] = useState<number>(0);
  const [copied, setCopied] = useState(false);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  // Load current session status on mount
  useEffect(() => {
    getTelegramSession()
      .then(setSession)
      .catch(() => setSession({ linked: false }))
      .finally(() => setLoading(false));
  }, []);

  // Countdown timer when a pairing code is active
  useEffect(() => {
    if (!pairing) return;
    const expiresAt = new Date(pairing.expires_at).getTime();
    const tick = () => {
      const diff = Math.max(0, Math.floor((expiresAt - Date.now()) / 1000));
      setSecondsLeft(diff);
    };
    tick();
    timerRef.current = setInterval(tick, 1000);
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [pairing]);

  async function handleGenerate() {
    setGenerating(true);
    setError(null);
    try {
      const result = await generatePairingCode();
      setPairing(result);
      setSecondsLeft(TTL_SECONDS);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al generar el código';
      setError(msg);
    } finally {
      setGenerating(false);
    }
  }

  async function handleCopy() {
    if (!pairing) return;
    await navigator.clipboard.writeText(pairing.code);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  function formatTime(seconds: number) {
    const m = Math.floor(seconds / 60).toString().padStart(2, '0');
    const s = (seconds % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
  }

  if (loading) {
    return (
      <div className={styles.card} aria-busy="true" aria-label="Cargando">
        <div className={styles.skeleton} aria-hidden="true" />
      </div>
    );
  }

  if (session?.linked) {
    return (
      <div className={styles.card} role="status">
        <p className={styles.linkedBadge}>Telegram vinculado</p>
        <p className={styles.hint}>
          Tu cuenta está conectada a{' '}
          <a
            href="https://t.me/bitacorav2_bot"
            target="_blank"
            rel="noopener noreferrer"
            className={styles.botLink}
          >
            @bitacorav2_bot
          </a>
          . Podés registrar tu humor directamente desde el bot.
        </p>
      </div>
    );
  }

  const isExpired = pairing !== null && secondsLeft === 0;

  return (
    <div className={styles.card}>
      <h2 className={styles.title}>Vincular Telegram</h2>
      <p className={styles.description}>
        Generá un código único y enviáselo al bot{' '}
        <a
          href="https://t.me/bitacorav2_bot"
          target="_blank"
          rel="noopener noreferrer"
          className={styles.botLink}
        >
          @bitacorav2_bot
        </a>{' '}
        con el comando <code>/start TU_CÓDIGO</code>.
      </p>

      {error && (
        <p className={styles.error} role="alert">
          {error}
        </p>
      )}

      {!pairing && (
        <button
          className={styles.generateBtn}
          onClick={handleGenerate}
          disabled={generating}
          aria-busy={generating}
        >
          {generating ? 'Generando...' : 'Generar código de vinculación'}
        </button>
      )}

      {pairing && !isExpired && (
        <div className={styles.codeBlock}>
          <p className={styles.codeLabel}>Tu código (válido por {formatTime(secondsLeft)}):</p>
          <div className={styles.codeRow}>
            <span className={styles.code} aria-label={`Código de vinculación: ${pairing.code}`}>
              {pairing.code}
            </span>
            <button
              className={styles.copyBtn}
              onClick={handleCopy}
              aria-label="Copiar código"
            >
              {copied ? 'Copiado' : 'Copiar'}
            </button>
          </div>
          <p className={styles.instruction}>
            Abrí{' '}
            <a
              href={`https://t.me/bitacorav2_bot?start=${pairing.code}`}
              target="_blank"
              rel="noopener noreferrer"
              className={styles.botLink}
            >
              el bot en Telegram
            </a>{' '}
            y enviá el código. El bot te confirmará la vinculación.
          </p>
        </div>
      )}

      {isExpired && (
        <div className={styles.expiredBlock} role="status">
          <p className={styles.expiredText}>Código expirado.</p>
          <button
            className={styles.generateBtn}
            onClick={handleGenerate}
            disabled={generating}
            aria-busy={generating}
          >
            {generating ? 'Generando...' : 'Generar nuevo código'}
          </button>
        </div>
      )}
    </div>
  );
}
