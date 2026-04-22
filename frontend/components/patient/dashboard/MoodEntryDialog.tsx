'use client';

/**
 * MoodEntryDialog — modal wrapper for MoodEntryForm used from the dashboard.
 * Uses the native <dialog> element so Escape and backdrop cancel work for free.
 */
import { useEffect, useRef } from 'react';
import { MoodEntryForm } from '@/components/patient/mood/MoodEntryForm';
import styles from './MoodEntryDialog.module.css';

interface Props {
  open: boolean;
  onClose: () => void;
  onSaved: () => void;
}

export function MoodEntryDialog({ open, onClose, onSaved }: Props) {
  const dialogRef = useRef<HTMLDialogElement | null>(null);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (open && !dialog.open) {
      dialog.showModal();
    } else if (!open && dialog.open) {
      dialog.close();
    }
  }, [open]);

  function handleCancel(event: React.SyntheticEvent<HTMLDialogElement>) {
    event.preventDefault();
    onClose();
  }

  function handleBackdropClick(event: React.MouseEvent<HTMLDialogElement>) {
    if (event.target === dialogRef.current) {
      onClose();
    }
  }

  function handleSaved() {
    onSaved();
  }

  return (
    <dialog
      ref={dialogRef}
      className={styles.dialog}
      onCancel={handleCancel}
      onClick={handleBackdropClick}
      aria-labelledby="mood-entry-dialog-title"
    >
      <div className={styles.panel}>
        <div className={styles.header}>
          <h2 id="mood-entry-dialog-title" className={styles.title}>Nuevo registro</h2>
          <button
            type="button"
            className={styles.closeBtn}
            onClick={onClose}
            aria-label="Cerrar"
          >
            ×
          </button>
        </div>
        <div className={styles.body}>
          <MoodEntryForm embedded onSaved={handleSaved} />
        </div>
      </div>
    </dialog>
  );
}
