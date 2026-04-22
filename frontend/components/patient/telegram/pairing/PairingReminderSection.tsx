'use client';

import Link from 'next/link';
import styles from './PairingReminderSection.module.css';

const TELEGRAM_BOT_URL = 'https://t.me/mi_bitacora_personal_bot';

type Props = {
  reminderHour: number;
  reminderMinute: number;
  configuredTime: string | null;
  savingSchedule: boolean;
  reminderConfigured: boolean;
  unlinkConfirmation: boolean;
  unlinking: boolean;
  onHourChange: (hour: number) => void;
  onMinuteChange: (minute: number) => void;
  onSaveReminder: () => void;
  onRequestUnlink: () => void;
  onConfirmUnlink: () => void;
  onCancelUnlink: () => void;
};

export function PairingReminderSection({
  reminderHour,
  reminderMinute,
  configuredTime,
  savingSchedule,
  reminderConfigured,
  unlinkConfirmation,
  unlinking,
  onHourChange,
  onMinuteChange,
  onSaveReminder,
  onRequestUnlink,
  onConfirmUnlink,
  onCancelUnlink,
}: Props) {
  return (
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
                onChange={(event) => onHourChange(Number(event.target.value))}
                className={styles.reminderSelect}
                disabled={savingSchedule}
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
                onChange={(event) => onMinuteChange(Number(event.target.value))}
                className={styles.reminderSelect}
                disabled={savingSchedule}
              >
                <option value={0}>00</option>
                <option value={30}>30</option>
              </select>
            </div>
          </div>

          <button
            type="button"
            className={styles.primaryBtn}
            onClick={onSaveReminder}
            disabled={savingSchedule}
            aria-busy={savingSchedule}
          >
            {savingSchedule ? 'Guardando...' : 'Guardar recordatorio'}
          </button>
        </div>
      </div>

      {reminderConfigured && (
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
            type="button"
            className={styles.unlinkBtn}
            onClick={onRequestUnlink}
            disabled={unlinking}
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
                type="button"
                className={styles.confirmBtn}
                onClick={onConfirmUnlink}
                disabled={unlinking}
                aria-busy={unlinking}
              >
                {unlinking ? 'Desvinculando...' : 'Desvincular'}
              </button>
              <button
                type="button"
                className={styles.secondaryBtn}
                onClick={onCancelUnlink}
                disabled={unlinking}
              >
                Mantener vínculo
              </button>
            </div>
          </div>
        )}
      </div>
    </>
  );
}
