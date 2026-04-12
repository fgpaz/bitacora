'use client';

import { useSession } from '@/lib/auth/SessionContext';
import { ProfessionalShell } from '@/components/ui/ProfessionalShell';
import { useProfessionalGuard } from '@/lib/auth/ProfessionalGuard';

function LoadingFallback() {
  return <ProfessionalShell loading />;
}

export default function ProfesionalLayout({ children }: { children: React.ReactNode }) {
  useProfessionalGuard();
  const { loading } = useSession();

  return (
    <ProfessionalShell loading={loading}>
      {children}
    </ProfessionalShell>
  );
}
