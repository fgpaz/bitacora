'use client';

/**
 * InviteForm — form to send a patient invite by email.
 * States: idle | submitting | success | conflict | error
 *
 * UI-RFC-VIN-001 states:
 * S01-DEFAULT  → idle, empty form
 * S01-READY   → idle, valid email, CTA enabled (handled by button disabled prop)
 * S01-SUBMITTING → submitting, disabled form
 * S02-SUCCESS → 201 Created, PendingStatusCard
 * S03-CONFLICT → 409 ALREADY_EXISTS, ConflictResolutionCard
 */
import Link from 'next/link';
import { useState } from 'react';
import { createProfessionalInvite, InviteResponse } from '@/lib/api/professional';
import { InlineFeedback } from '@/components/ui/InlineFeedback';
import styles from './InviteForm.module.css';

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('es-AR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  });
}

function truncateId(id: string): string {
  return id.length > 8 ? `${id.slice(0, 8)}…` : id;
}

function PendingStatusCard({ invite, email }: { invite: InviteResponse; email: string }) {
  return (
    <div className={styles.card} role="status" aria-live="polite">
      <h3 className={styles.cardTitle}>Invitacion enviada</h3>
      <p className={styles.cardText}>
        Se envio una invitacion a <strong>{email}</strong>. Cuando se registre, aparecera en tu
        lista de pacientes.
      </p>
      <dl className={styles.meta}>
        <div className={styles.metaRow}>
          <dt className={styles.metaKey}>ID de invitacion</dt>
          <dd className={styles.metaValue}>{truncateId(invite.invite_id)}</dd>
        </div>
        <div className={styles.metaRow}>
          <dt className={styles.metaKey}>Enviada el</dt>
          <dd className={styles.metaValue}>{formatDate(invite.created_at)}</dd>
        </div>
      </dl>
    </div>
  );
}

function ConflictResolutionCard({ email }: { email: string }) {
  return (
    <div className={`${styles.card} ${styles.cardConflict}`} role="alert" aria-live="assertive">
      <h3 className={styles.cardTitle}>Este paciente ya tiene una invitacion o vinculo</h3>
      <p className={styles.cardText}>
        El correo <strong>{email}</strong> ya esta vinculado a tu cuenta o tiene una invitacion
        pendiente. No es necesario enviar otra.
      </p>
      <Link href="/profesional/pacientes" className={styles.linkButton}>
        Ver pacientes
      </Link>
    </div>
  );
}

export function InviteForm() {
  const [email, setEmail] = useState('');
  const [status, setStatus] = useState<'idle' | 'submitting' | 'success' | 'conflict' | 'error'>(
    'idle',
  );
  const [inviteData, setInviteData] = useState<InviteResponse | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!email) return;
    setStatus('submitting');
    setErrorMsg(null);
    setInviteData(null);

    try {
      const invite = await createProfessionalInvite({ email });
      setInviteData(invite);
      setStatus('success');
      setEmail('');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      if (code === 'ALREADY_EXISTS') {
        setStatus('conflict');
      } else {
        setStatus('error');
        setErrorMsg(err instanceof Error ? err.message : 'No se pudo enviar la invitacion.');
      }
    }
  }

  if (status === 'success' && inviteData) {
    return <PendingStatusCard invite={inviteData} email={inviteData.email} />;
  }

  if (status === 'conflict') {
    return <ConflictResolutionCard email={email} />;
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
