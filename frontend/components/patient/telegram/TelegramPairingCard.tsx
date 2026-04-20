'use client';

/**
 * TelegramPairingCard — guided setup for linking Telegram and configuring reminders.
 *
 * The UI keeps the patient-facing contract local to Buenos Aires time. The API
 * client converts the selected local time to UTC before persisting RF-TG-006.
 */
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import Link from 'next/link';
import {
  generatePairingCode,
  getReminderSchedule,
  getTelegramSession,
  setReminderSchedule as saveReminderScheduleApi,
  unlinkTelegram,
  type ReminderScheduleResponse,
  type TelegramPairingResponse,
  type TelegramSessionResponse,
} from '../../../lib/api/client';
import styles from './TelegramPairingCard.module.css';

const TTL_SECONDS = 15 * 60;
const BUENOS_AIRES_TZ = 'America/Argentina/Buenos_Aires';
const TELEGRAM_BOT_USERNAME = 'mi_bitacora_personal_bot';
const TELEGRAM_BOT_URL = `https://t.me/${TELEGRAM_BOT_USERNAME}`;

type LoadingState = 'checking' | 'ready' | 'unlinking' | 'saving_schedule';

export function TelegramPairingCard() {
  const [session, setSession] = useState<TelegramSessionResponse | null>(null);
  const [pairing, setPairing] = useState<TelegramPairingResponse | null>(null);
  const [secondsLeft, setSecondsLeft] = useState<number>(0);
  const [copied, setCopied] = useState(false);
  const [state, setState] = useState<LoadingState>('checking');
  const [generating, setGenerating] = useState(false);
  const [checkingLink, setCheckingLink] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [unlinkConfirmation, setUnlinkConfirmation] = useState(false);
  const [reminderSchedule, setReminderSchedule] = useState<ReminderScheduleResponse | null>(null);
  const [reminderHour, setReminderHour] = useState<number>(9);
  const [reminderMinute, setReminderMinute] = useState<number>(0);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const isLinked = session?.linked === true;
  const isExpired = pairing !== null && secondsLeft === 0;
  const commandText = pairing ? `/start ${pairing.code}` : '';
  const telegramStartUrl = pairing
    ? `${TELEGRAM_BOT_URL}?start=${encodeURIComponent(pairing.code)}`
    : TELEGRAM_BOT_URL;
  const configuredTime = reminderSchedule?.configured && reminderSchedule.hour !== null && reminderSchedule.minute !== null
    ? formatScheduleTime(reminderSchedule.hour, reminderSchedule.minute)
    : null;

  const loadReminder = useCallback(async () => {
    try {
      const schedule = await getReminderSchedule();
      setReminderSchedule(schedule);

      if (schedule.configured && schedule.hour !== null && schedule.minute !== null) {
        setReminderHour(schedule.hour);
        setReminderMinute(schedule.minute);
      }
    } catch {
      setNotice('No pudimos leer el recordatorio guardado. Podés revisar o guardar el horario de nuevo.');
    }
  }, []);

  const refreshSession = useCallback(async () => {
    const currentSession = await getTelegramSession();
    setSession(currentSession);

    if (currentSession.linked) {
      setPairing(null);
      setError(null);
      await loadReminder();
    }

    return currentSession;
  }, [loadReminder]);

  useEffect(() => {
    let cancelled = false;

    async function loadInitialState() {
      try {
        const currentSession = await getTelegramSession();
        if (cancelled) return;

        setSession(currentSession);
        if (currentSession.linked) {
          await loadReminder();
        }
      } catch {
        if (!cancelled) {
          setSession({ linked: false });
          setError('No pudimos leer el estado de Telegram. Intentá actualizar la página.');
        }
      } finally {
        if (!cancelled) {
          setState('ready');
        }
      }
    }

    void loadInitialState();
    return () => {
      cancelled = true;
    };
  }, [loadReminder]);

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

  useEffect(() => {
    if (!pairing || isExpired || isLinked) return;

    pollRef.current = setInterval(() => {
      refreshSession().catch(() => {
        // Manual check remains available; avoid noisy polling errors.
      });
    }, 5000);

    return () => {
      if (pollRef.current) clearInterval(pollRef.current);
    };
  }, [isExpired, isLinked, pairing, refreshSession]);

  const setupStatus = useMemo(() => {
    if (!isLinked) return 'Necesita vinculación';
    if (!reminderSchedule?.configured) return 'Telegram vinculado';
    return 'Listo para usar';
  }, [isLinked, reminderSchedule?.configured]);

  async function handleGenerate() {
    setGenerating(true);
    setError(null);
    setNotice(null);
    setCopied(false);
    try {
      const result = await generatePairingCode();
      setPairing(result);
      setSecondsLeft(TTL_SECONDS);
    } catch {
      setError('No pudimos generar el código. Intentá de nuevo.');
    } finally {
      setGenerating(false);
    }
  }

  async function handleCopyCommand() {
    if (!commandText) return;
    await navigator.clipboard.writeText(commandText);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  async function handleCheckLink() {
    setCheckingLink(true);
    setError(null);
    setNotice(null);
    try {
      const currentSession = await refreshSession();
      if (!currentSession.linked) {
        setNotice('Todavía no vemos la vinculación. En Telegram enviá el mensaje exacto que aparece acá.');
      }
    } catch {
      setError('No pudimos comprobar la vinculación. Intentá de nuevo.');
    } finally {
      setCheckingLink(false);
    }
  }

  async function handleUnlink() {
    setState('unlinking');
    setError(null);
    setNotice(null);
    try {
      await unlinkTelegram();
      setSession({ linked: false });
      setPairing(null);
      setReminderSchedule(null);
      setUnlinkConfirmation(false);
    } catch {
      setError('No pudimos desvincular Telegram. Intentá de nuevo.');
    } finally {
      setState('ready');
    }
  }

  async function handleSaveReminder() {
    setState('saving_schedule');
    setError(null);
    setNotice(null);
    try {
      const result = await saveReminderScheduleApi(reminderHour, reminderMinute, BUENOS_AIRES_TZ);
      setReminderSchedule(result);
      setNotice(`Recordatorio guardado para las ${formatScheduleTime(reminderHour, reminderMinute)}, hora de Buenos Aires.`);
    } catch {
      setError('No pudimos guardar el horario. Intentá de nuevo.');
    } finally {
      setState('ready');
    }
  }

  if (state === 'checking') {
    return (
      <div className={styles.card} aria-busy="true" aria-label="Cargando configuración de Telegram">
        <div className={styles.skeleton} aria-hidden="true" />
      </div>
    );
  }

  return (
    <section className={styles.card} aria-labelledby="telegram-title">
      <div className={styles.header}>
        <p className={styles.eyebrow}>Telegram</p>
        <h2 id="telegram-title" className={styles.title}>Configurar el bot</h2>
        <p className={styles.description}>
          Usá Telegram para registrar tu estado de ánimo y recibir un recordatorio diario. El bot no muestra tus registros ni datos clínicos.
        </p>
        <p className={isLinked ? styles.readyBadge : styles.pendingBadge}>{setupStatus}</p>
      </div>

      {error && (
        <p className={styles.error} role="alert">
          {error}
        </p>
      )}

      {notice && (
        <p className={styles.notice} role="status">
          {notice}
        </p>
      )}

      {!isLinked ? (
        <div className={styles.flow} aria-label="Pasos para vincular Telegram">
          <div className={styles.step}>
            <span className={styles.stepMarker}>1</span>
            <div>
              <h3 className={styles.stepTitle}>Generá un código</h3>
              <p className={styles.stepText}>El código vence en 15 minutos y sirve solo para vincular tu cuenta.</p>
            </div>
          </div>

          {!pairing && (
            <button
              className={styles.primaryBtn}
              onClick={handleGenerate}
              disabled={generating}
              aria-busy={generating}
            >
              {generating ? 'Generando...' : 'Generar código'}
            </button>
          )}

          {pairing && !isExpired && (
            <>
              <div className={styles.commandBox}>
                <p className={styles.codeLabel}>Enviá este mensaje al bot. Vence en {formatTime(secondsLeft)}.</p>
                <code className={styles.commandText}>{commandText}</code>
                <div className={styles.actionGrid}>
                  <button className={styles.secondaryBtn} onClick={handleCopyCommand}>
                    {copied ? 'Mensaje copiado' : 'Copiar mensaje'}
                  </button>
                  <a className={styles.primaryBtn} href={telegramStartUrl} target="_blank" rel="noopener noreferrer">
                    Abrir Telegram
                  </a>
                </div>
              </div>

              <div className={styles.step}>
                <span className={styles.stepMarker}>2</span>
                <div>
                  <h3 className={styles.stepTitle}>Confirmá la vinculación</h3>
                  <p className={styles.stepText}>
                    Si Telegram no completa el mensaje solo, pegá el texto copiado en el chat con @{TELEGRAM_BOT_USERNAME}.
                  </p>
                </div>
              </div>

              <button
                className={styles.secondaryBtn}
                onClick={handleCheckLink}
                disabled={checkingLink}
                aria-busy={checkingLink}
              >
                {checkingLink ? 'Comprobando...' : 'Ya envié el mensaje'}
              </button>
            </>
          )}

          {isExpired && (
            <div className={styles.expiredBlock} role="status">
              <p className={styles.expiredText}>El código venció. Generá uno nuevo y enviá el nuevo mensaje al bot.</p>
              <button
                className={styles.primaryBtn}
                onClick={handleGenerate}
                disabled={generating}
                aria-busy={generating}
              >
                {generating ? 'Generando...' : 'Generar nuevo código'}
              </button>
            </div>
          )}
        </div>
      ) : (
        <>
          <div className={styles.flow}>
            <div className={styles.step}>
              <span className={styles.stepMarker}>1</span>
              <div>
                <h3 className={styles.stepTitle}>Telegram quedó vinculado</h3>
                <p className={styles.stepText}>Ya podés escribirle al bot para registrar tu humor desde Telegram.</p>
              </div>
            </div>

            <div className={styles.section}>
              <div className={styles.sectionHeader}>
                <h3 className={styles.sectionTitle}>Recordatorio diario</h3>
                {configuredTime && (
                  <p className={styles.savedTime}>Actual: {configuredTime}, hora de Buenos Aires</p>
                )}
              </div>
              <p className={styles.hint}>Elegí a qué hora querés que el bot te recuerde registrar tu estado de ánimo.</p>

              <div className={styles.reminderControls}>
                <div className={styles.reminderGroup}>
                  <label htmlFor="reminder-hour" className={styles.reminderLabel}>
                    Hora
                  </label>
                  <select
                    id="reminder-hour"
                    value={reminderHour}
                    onChange={(event) => setReminderHour(Number(event.target.value))}
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
                    Minuto
                  </label>
                  <select
                    id="reminder-minute"
                    value={reminderMinute}
                    onChange={(event) => setReminderMinute(Number(event.target.value))}
                    className={styles.reminderSelect}
                    disabled={state === 'saving_schedule'}
                  >
                    <option value={0}>00</option>
                    <option value={30}>30</option>
                  </select>
                </div>
              </div>

              <button
                className={styles.primaryBtn}
                onClick={handleSaveReminder}
                disabled={state === 'saving_schedule'}
                aria-busy={state === 'saving_schedule'}
              >
                {state === 'saving_schedule' ? 'Guardando...' : 'Guardar recordatorio'}
              </button>
            </div>
          </div>

          {reminderSchedule?.configured && (
            <div className={styles.nextStep}>
              <h3 className={styles.sectionTitle}>Ya quedó listo</h3>
              <p className={styles.hint}>
                El próximo paso es probar el bot con un registro breve. Podés volver a cambiar el horario cuando quieras.
              </p>
              <div className={styles.actionGrid}>
                <a className={styles.primaryBtn} href={TELEGRAM_BOT_URL} target="_blank" rel="noopener noreferrer">
                  Probar el bot
                </a>
                <Link className={styles.secondaryBtn} href="/dashboard">
                  Volver al inicio
                </Link>
              </div>
            </div>
          )}

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
                  Se cortará la conexión con el bot. Tus registros existentes no se eliminan.
                </p>
                <div className={styles.actionGrid}>
                  <button
                    className={styles.confirmBtn}
                    onClick={handleUnlink}
                    disabled={state === 'unlinking'}
                    aria-busy={state === 'unlinking'}
                  >
                    {state === 'unlinking' ? 'Desvinculando...' : 'Desvincular'}
                  </button>
                  <button
                    className={styles.secondaryBtn}
                    onClick={() => setUnlinkConfirmation(false)}
                    disabled={state === 'unlinking'}
                  >
                    Mantener vínculo
                  </button>
                </div>
              </div>
            )}
          </div>
        </>
      )}
    </section>
  );
}

function formatTime(seconds: number) {
  const minutes = Math.floor(seconds / 60).toString().padStart(2, '0');
  const remainingSeconds = (seconds % 60).toString().padStart(2, '0');
  return `${minutes}:${remainingSeconds}`;
}

function formatScheduleTime(hour: number, minute: number) {
  return `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
}
