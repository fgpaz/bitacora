'use client';

/**
 * TelegramPairingCard — generates and displays a BIT-XXXXX pairing code
 * for the patient to send to @mi_bitacora_personal_bot via /start <code>.
 *
 * Implements RF-TG-001 (frontend side): pairing code generation, display,
 * copy-to-clipboard, countdown timer, expiry/regenerate flow, unlinking,
 * and reminder schedule configuration.
 */
import { useEffect, useRef, useState } from 'react';
import {
  generatePairingCode,
  getTelegramSession,
  unlinkTelegram,
  setReminderSchedule as saveReminderScheduleApi,
  type TelegramPairingResponse,
  type TelegramSessionResponse,
  type ReminderScheduleResponse,
} from '../../../lib/api/client';
import styles from './TelegramPairingCard.module.css';

const TTL_SECONDS = 15 * 60; // 15 minutes
const BUENOS_AIRES_TZ = 'America/Argentina/Buenos_Aires';
const TELEGRAM_BOT_USERNAME = 'mi_bitacora_personal_bot';

type LoadingState = 'checking' | 'linked' | 'unlinked' | 'unlinking' | 'saving_schedule';

export function TelegramPairingCard() {
  const [session, setSession] = useState<TelegramSessionResponse | null>(null);
  const [pairing, setPairing] = useState<TelegramPairingResponse | null>(null);
  const [secondsLeft, setSecondsLeft] = useState<number>(0);
  const [copied, setCopied] = useState(false);
  const [state, setState] = useState<LoadingState>('checking');
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [unlinkConfirmation, setUnlinkConfirmation] = useState(false);
  const [reminderSchedule, setReminderSchedule] = useState<ReminderScheduleResponse | null>(null);
  const [reminderHour, setReminderHour] = useState<number>(9);
  const [reminderMinute, setReminderMinute] = useState<number>(0);
  const [reminderSaved, setReminderSaved] = useState(false);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  // Load current session status on mount
  useEffect(() => {
    getTelegramSession()
      .then((s) => {
        setSession(s);
        setState(s.linked ? 'linked' : 'unlinked');
      })
      .catch(() => {
        setSession({ linked: false });
        setState('unlinked');
      });
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

  function formatScheduleTime(hour: number, minute: number) {
    return `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
  }

  async function handleUnlink() {
    setState('unlinking');
    setError(null);
    try {
      await unlinkTelegram();
      setSession({ linked: false });
      setState('unlinked');
      setPairing(null);
      setUnlinkConfirmation(false);
      setReminderSchedule(null);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al desvincular Telegram';
      setError(msg);
      setState('linked');
    }
  }

  async function handleSaveReminder() {
    setState('saving_schedule');
    setError(null);
    try {
      const result = await saveReminderScheduleApi(reminderHour, reminderMinute, BUENOS_AIRES_TZ);
      setReminderSchedule(result);
      setReminderSaved(true);
      setTimeout(() => setReminderSaved(false), 3000);
      setState('linked');
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al guardar el horario';
      setError(msg);
      setState('linked');
    }
  }

  if (state === 'checking') {
    return (
      <div className={styles.card} aria-busy="true" aria-label="Cargando">
        <div className={styles.skeleton} aria-hidden="true" />
      </div>
    );
  }

  // Linked state: show status, unlink button, and reminder config
  if (state === 'linked' || (session?.linked && state !== 'unlinked')) {
    return (
      <div className={styles.card} role="status">
        {/* Status badge */}
        <p className={styles.linkedBadge}>Telegram vinculado</p>
        <p className={styles.hint}>
          Tu cuenta está conectada a{' '}
          <a
            href={`https://t.me/${TELEGRAM_BOT_USERNAME}`}
            target="_blank"
            rel="noopener noreferrer"
            className={styles.botLink}
          >
            @{TELEGRAM_BOT_USERNAME}
          </a>
          . Podés registrar tu humor directamente desde el bot.
        </p>

        {/* Reminder configuration section */}
        <div className={styles.section}>
          <h3 className={styles.sectionTitle}>Configurar recordatorio</h3>
          <div className={styles.reminderControls}>
            <div className={styles.reminderGroup}>
              <label htmlFor="reminder-hour" className={styles.reminderLabel}>
                Hora:
              </label>
              <select
                id="reminder-hour"
                value={reminderHour}
                onChange={(e) => setReminderHour(Number(e.target.value))}
                className={styles.reminderSelect}
                disabled={state === 'saving_schedule'}
              >
                {Array.from({ length: 24 }, (_, i) => (
                  <option key={i} value={i}>
                    {i.toString().padStart(2, '0')}
                  </option>
                ))}
              </select>
            </div>

            <div className={styles.reminderGroup}>
              <label htmlFor="reminder-minute" className={styles.reminderLabel}>
                Minuto:
              </label>
              <select
                id="reminder-minute"
                value={reminderMinute}
                onChange={(e) => setReminderMinute(Number(e.target.value))}
                className={styles.reminderSelect}
                disabled={state === 'saving_schedule'}
              >
                <option value={0}>00</option>
                <option value={30}>30</option>
              </select>
            </div>
          </div>

          {reminderSaved && (
            <p className={styles.reminderSuccess} role="status">
              Recordatorio configurado para las {formatScheduleTime(reminderHour, reminderMinute)}
            </p>
          )}

          <button
            className={styles.saveReminderBtn}
            onClick={handleSaveReminder}
            disabled={state === 'saving_schedule'}
            aria-busy={state === 'saving_schedule'}
          >
            {state === 'saving_schedule' ? 'Guardando...' : 'Guardar horario'}
          </button>
        </div>

        {/* Unlink section */}
        <div className={styles.unlinkSection}>
          {!unlinkConfirmation ? (
            <button
              className={styles.unlinkBtn}
              onClick={() => setUnlinkConfirmation(true)}
              disabled={state === 'unlinking'}
            >
              Desvincular Telegram
            </button>
          ) : (
            <div className={styles.unlinkConfirmation}>
              <p className={styles.confirmText}>
                ¿Estás seguro? Se cortará la conexión con el bot.
              </p>
              <div className={styles.confirmButtons}>
                <button
                  className={styles.confirmBtn}
                  onClick={handleUnlink}
                  disabled={state === 'unlinking'}
                  aria-busy={state === 'unlinking'}
                >
                  {state === 'unlinking' ? 'Desvinculando...' : 'Confirmar'}
                </button>
                <button
                  className={styles.cancelBtn}
                  onClick={() => setUnlinkConfirmation(false)}
                  disabled={state === 'unlinking'}
                >
                  Cancelar
                </button>
              </div>
            </div>
          )}
        </div>

        {error && (
          <p className={styles.error} role="alert">
            {error}
          </p>
        )}
      </div>
    );
  }

  const isExpired = pairing !== null && secondsLeft === 0;

  // Unlinked state: show pairing wizard
  return (
    <div className={styles.card}>
      <h2 className={styles.title}>Vincular Telegram</h2>
      <p className={styles.description}>
        Generá un código único y enviáselo al bot{' '}
        <a
          href={`https://t.me/${TELEGRAM_BOT_USERNAME}`}
          target="_blank"
          rel="noopener noreferrer"
          className={styles.botLink}
        >
          @{TELEGRAM_BOT_USERNAME}
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
              href={`https://t.me/${TELEGRAM_BOT_USERNAME}?start=${pairing.code}`}
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
