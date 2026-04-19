'use client';

/**
 * ProfessionalGuard - redirects to / if user has no professional role.
 */
import { useSession } from '@/lib/auth/SessionContext';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export function useProfessionalGuard() {
  const { user, loading } = useSession();
  const router = useRouter();

  useEffect(() => {
    if (loading) return;
    if (!user) {
      router.replace('/');
      return;
    }

    if (user.role !== 'professional') {
      router.replace('/');
    }
  }, [user, loading, router]);
}
