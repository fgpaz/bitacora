'use client';

import { PatientList } from '@/components/professional/PatientList';

export default function PacientesPage() {
  return (
    <section>
      <h1 style={{ fontFamily: 'var(--font-display)', fontSize: '1.5rem', marginBottom: 'var(--space-lg)', color: 'var(--foreground)' }}>
        Mis pacientes
      </h1>
      <PatientList />
    </section>
  );
}
