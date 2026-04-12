'use client';

/**
 * MoodScale — 7-value scale from -3 to +3.
 * States: default | selected | disabled
 */
import styles from './MoodScale.module.css';

interface Props {
  value: number | null;
  onChange: (v: number) => void;
  disabled?: boolean;
}

const SCORES = [-3, -2, -1, 0, 1, 2, 3];

export function MoodScale({ value, onChange, disabled }: Props) {
  return (
    <div
      className={styles.scale}
      role="radiogroup"
      aria-label="Escala de humor"
    >
      {SCORES.map((s) => (
        <button
          key={s}
          type="button"
          role="radio"
          aria-checked={value === s}
          className={`${styles.option} ${value === s ? styles.selected : ''}`}
          onClick={() => !disabled && onChange(s)}
          disabled={disabled}
          aria-label={`${s > 0 ? '+' : ''}${s}`}
        >
          <span className={styles.value}>{s > 0 ? `+${s}` : s}</span>
        </button>
      ))}
    </div>
  );
}
