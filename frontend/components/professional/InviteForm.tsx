'use client';

/**
 * InviteForm — form to send a patient invite by email.
 * States: idle | submitting | success | error
 */
import { useState } from 'react';
import { createProfessionalInvite } from '@/lib/api/professional';
import { InlineFeedback } from '@/components/ui/InlineFeedback';
import styles from './InviteForm.module.css';

export function InviteForm() {
  const [email, setEmail] = useState('');
  const [status, setStatus] = useState<'idle' | 'submitting' | 'success' | 'error'>('idle');
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!email) return;
    setStatus('submitting');
    setErrorMsg(null);

    try {
      await createProfessionalInvite({ email });
      setStatus('success');
      setEmail('');
    } catch (err: unknown) {
      setStatus('error');
      setErrorMsg(err instanceof Error ? err.message : 'No se pudo enviar la invitacion.');
    }
  }

  if (status === 'success') {
    return (
      <InlineFeedback
        variant="confirm"
        message={`Invitacion enviada a ${email}. Cuando se registre, aparecera en tu lista de pacientes.`}
      />
    );
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form} noValidate>
      <label htmlFor="invite-email" className={styles.label}>
        Correo electronico del paciente
      </label>
      <div className={styles.row}>
        <input
          id="invite-email"
          type="email"
          className={styles.input}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="paciente@ejemplo.com"
          required
          disabled={status === 'submitting'}
          autoComplete="email"
        />
        <button
          type="submit"
          className={styles.button}
          disabled={status === 'submitting' || !email}
        >
          {status === 'submitting' ? 'Enviando...' : 'Invitar'}
        </button>
      </div>
      {status === 'error' && errorMsg && (
        <InlineFeedback variant="error" message={errorMsg} />
      )}
    </form>
  );
}
