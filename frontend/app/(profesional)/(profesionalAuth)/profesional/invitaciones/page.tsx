'use client';

import { InviteForm } from '@/components/professional/InviteForm';

export default function InvitacionesPage() {
  return (
    <section>
      <h1 style={{ fontFamily: 'var(--font-display)', fontSize: '1.5rem', marginBottom: 'var(--space-md)', color: 'var(--foreground)' }}>
        Invitar paciente
      </h1>
      <p style={{ fontFamily: 'var(--font-body)', color: 'var(--foreground-muted)', marginBottom: 'var(--space-lg)' }}>
        Ingresa el correo electronico de tu paciente. Le enviaremos un enlace para que se registre y quede vinculado a tu cuenta.
      </p>
      <InviteForm />
    </section>
  );
}
