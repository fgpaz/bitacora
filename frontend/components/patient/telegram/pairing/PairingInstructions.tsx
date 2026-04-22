'use client';

import styles from './PairingInstructions.module.css';

type Props = {
  generating: boolean;
  onGenerate: () => void;
};

export function PairingInstructions({ generating, onGenerate }: Props) {
  return (
    <>
      <div className={styles.step}>
        <span className={styles.stepMarker}>1</span>
        <div>
          <h3 className={styles.stepTitle}>Generá un código</h3>
          <p className={styles.stepText}>El código vence en 15 minutos y sirve solo para vincular tu cuenta.</p>
        </div>
      </div>

      <button
        type="button"
        className={styles.primaryBtn}
        onClick={onGenerate}
        disabled={generating}
        aria-busy={generating}
      >
        {generating ? 'Generando...' : 'Generar código'}
      </button>
    </>
  );
}
