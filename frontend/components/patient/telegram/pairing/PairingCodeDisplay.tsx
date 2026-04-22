'use client';

import styles from './PairingCodeDisplay.module.css';

const TELEGRAM_BOT_USERNAME = 'mi_bitacora_personal_bot';

type Props = {
  commandText: string;
  telegramStartUrl: string;
  secondsLeft: number;
  isExpired: boolean;
  copied: boolean;
  checkingLink: boolean;
  generating: boolean;
  onCopy: () => void;
  onCheckLink: () => void;
  onGenerate: () => void;
};

function formatTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60).toString().padStart(2, '0');
  const remaining = (seconds % 60).toString().padStart(2, '0');
  return `${minutes}:${remaining}`;
}

export function PairingCodeDisplay({
  commandText,
  telegramStartUrl,
  secondsLeft,
  isExpired,
  copied,
  checkingLink,
  generating,
  onCopy,
  onCheckLink,
  onGenerate,
}: Props) {
  if (isExpired) {
    return (
      <div className={styles.expiredBlock} role="status">
        <p className={styles.expiredText}>El código venció. Generá uno nuevo y enviá el nuevo mensaje al bot.</p>
        <button
          type="button"
          className={styles.primaryBtn}
          onClick={onGenerate}
          disabled={generating}
          aria-busy={generating}
        >
          {generating ? 'Generando...' : 'Generar nuevo código'}
        </button>
      </div>
    );
  }

  return (
    <>
      <div className={styles.commandBox}>
        <p className={styles.codeLabel}>Enviá este mensaje al bot. Vence en {formatTime(secondsLeft)}.</p>
        <code className={styles.commandText}>{commandText}</code>
        <div className={styles.actionGrid}>
          <button type="button" className={styles.secondaryBtn} onClick={onCopy}>
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
        type="button"
        className={styles.secondaryBtn}
        onClick={onCheckLink}
        disabled={checkingLink}
        aria-busy={checkingLink}
      >
        {checkingLink ? 'Comprobando...' : 'Ya envié el mensaje'}
      </button>
    </>
  );
}
