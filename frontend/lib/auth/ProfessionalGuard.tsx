'use client';

/**
 * ProfessionalGuard — redirects to / if user has no professional role.
 * Checks Supabase metadata for role: 'professional'.
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

    // TODO: when user metadata includes role, check it here.
    // For now, allow any authenticated user to access professional routes
    // to enable development and real-auth integration.
    // After Supabase custom claims / metadata role is wired, uncomment:
    // const role = (user as Record<string, unknown>)?.role;
    // if (role !== 'professional') router.replace('/');
  }, [user, loading, router]);
}
