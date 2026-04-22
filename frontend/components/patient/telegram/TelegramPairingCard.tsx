'use client';

/**
 * TelegramPairingCard — guided setup for linking Telegram and configuring reminders.
 *
 * The UI keeps the patient-facing contract local to Buenos Aires time. The API
 * client converts the selected local time to UTC before persisting RF-TG-006.
 */
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
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
import { PairingCodeDisplay } from './pairing/PairingCodeDisplay';
import { PairingInstructions } from './pairing/PairingInstructions';
import { PairingReminderSection } from './pairing/PairingReminderSection';
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
          {!pairing && (
            <PairingInstructions
              generating={generating}
              onGenerate={handleGenerate}
            />
          )}

          {pairing && (
            <PairingCodeDisplay
              commandText={commandText}
              telegramStartUrl={telegramStartUrl}
              secondsLeft={secondsLeft}
              isExpired={isExpired}
              copied={copied}
              checkingLink={checkingLink}
              generating={generating}
              onCopy={handleCopyCommand}
              onCheckLink={handleCheckLink}
              onGenerate={handleGenerate}
            />
          )}
        </div>
      ) : (
        <PairingReminderSection
          reminderHour={reminderHour}
          reminderMinute={reminderMinute}
          configuredTime={configuredTime}
          savingSchedule={state === 'saving_schedule'}
          reminderConfigured={reminderSchedule?.configured ?? false}
          unlinkConfirmation={unlinkConfirmation}
          unlinking={state === 'unlinking'}
          onHourChange={setReminderHour}
          onMinuteChange={setReminderMinute}
          onSaveReminder={handleSaveReminder}
          onRequestUnlink={() => setUnlinkConfirmation(true)}
          onConfirmUnlink={handleUnlink}
          onCancelUnlink={() => setUnlinkConfirmation(false)}
        />
      )}
    </section>
  );
}

function formatScheduleTime(hour: number, minute: number) {
  return `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
}
