'use client';

/**
 * BindingCodeForm — accepts a binding code from a professional to establish a care link.
 *
 * Props:
 *  - onSuccess: callback after successful code acceptance (triggers parent refetch)
 *
 * States:
 *  - idle: form ready
 *  - submitting: POST /api/v1/vinculos/accept in flight
 *  - success: code accepted, display confirmation
 *  - error: validation or API error
 */
import { useState } from 'react';
import { acceptBindingCode } from '../../../lib/api/client';
import styles from './VinculosForm.module.css';

interface BindingCodeFormProps {
  onSuccess: () => void;
}

export function BindingCodeForm({ onSuccess }: BindingCodeFormProps) {
  const [code, setCode] = useState('');
  const [state, setState] = useState<'idle' | 'submitting' | 'success' | 'error'>('idle');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCode(e.target.value.toUpperCase());
    if (state === 'error') {
      setState('idle');
      setErrorMessage(null);
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!code.trim()) return;

    setState('submitting');
    setErrorMessage(null);

    try {
      await acceptBindingCode(code);
      setState('success');
      setCode('');
      setTimeout(() => {
        setState('idle');
        onSuccess();
      }, 400);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al aceptar el código';
      setErrorMessage(msg);
      setState('error');
    }
  };

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.fieldGroup}>
        <label htmlFor="binding-code" className={styles.label}>
          Código de invitación
        </label>
        <input
          id="binding-code"
          type="text"
          placeholder="Ej: BIT-ABC12"
          value={code}
          onChange={handleChange}
          disabled={state === 'submitting' || state === 'success'}
          className={state === 'error' ? styles.inputError : styles.input}
          aria-describedby={errorMessage ? 'code-error' : undefined}
          maxLength={50}
        />
        {errorMessage && (
          <p id="code-error" className={styles.errorMessage} role="alert">
            {errorMessage}
          </p>
        )}
      </div>

      <button
        type="submit"
        disabled={!code.trim() || state === 'submitting' || state === 'success'}
        className={styles.submitButton}
        aria-busy={state === 'submitting'}
      >
        {state === 'submitting' && 'Aceptando...'}
        {state === 'success' && 'Invitación aceptada'}
        {state === 'idle' && 'Aceptar invitación'}
        {state === 'error' && 'Aceptar invitación'}
      </button>
    </form>
  );
}
