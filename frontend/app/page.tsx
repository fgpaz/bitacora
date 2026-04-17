'use client';

import { OnboardingEntryHero } from '@/components/patient/onboarding/OnboardingEntryHero';
import { signInWithMagicLink } from '@/lib/auth/client';
import { useState } from 'react';

export default function HomePage() {
  const [email, setEmail] = useState('');
  const [sent, setSent] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function handleEmailChange(value: string) {
    setEmail(value);
    if (error) setError(null);
  }

  async function handleStart() {
    const trimmed = email.trim();
    if (!trimmed) {
      setError('Ingresá tu correo electronico.');
      return;
    }
    setSubmitting(true);
    const { error: authError } = await signInWithMagicLink(trimmed);
    setSubmitting(false);
    if (authError) {
      setError('No se pudo enviar el enlace. Intentá de nuevo.');
    } else {
      setSent(true);
    }
  }

  if (sent) {
    return (
      <main style={{ minHeight: '100vh', background: 'var(--surface)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 'var(--space-lg)' }}>
        <div style={{ maxWidth: 480, textAlign: 'center' }}>
          <h1 style={{ fontFamily: 'var(--font-display)', fontSize: '1.5rem', marginBottom: 'var(--space-md)', color: 'var(--foreground)' }}>
            Revisá tu correo
          </h1>
          <p style={{ fontFamily: 'var(--font-body)', color: 'var(--foreground-muted)' }}>
            Te enviamos un enlace magico a <strong>{email}</strong>. Hacé click en el enlace para continuar.
          </p>
        </div>
      </main>
    );
  }

  return (
    <OnboardingEntryHero
      variant="standard"
      onStart={handleStart}
      email={email}
      onEmailChange={handleEmailChange}
      error={error}
      submitting={submitting}
    />
  );
}
