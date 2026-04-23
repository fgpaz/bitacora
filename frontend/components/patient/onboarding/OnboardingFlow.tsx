'use client';

/**
 * OnboardingFlow — authenticated entry gate. Runs only when the user needs
 * consent; otherwise redirects to /dashboard.
 * Phases: auth (bootstrap loader) -> consent (gate panel) -> redirect.
 */
import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { PatientPageShell } from '@/components/ui/PatientPageShell';
import { AuthBootstrapInterstitial } from './AuthBootstrapInterstitial';
import { ConsentGatePanel } from '@/components/patient/consent/ConsentGatePanel';
import {
  bootstrapPatient,
  getCurrentConsent,
  grantConsent,
  ConsentCurrentResponse,
} from '@/lib/api/client';
import { getAccessToken } from '@/lib/auth/client';
import { formatUserFacingError, type UserFacingError } from '@/lib/errors/user-facing';

type Phase = 'auth' | 'consent';

interface BootstrapData {
  userId: string;
  needsConsent: boolean;
  resumePendingInvite: boolean;
}

export function OnboardingFlow() {
  const searchParams = useSearchParams();
  const inviteToken = searchParams.get('invite_token') ?? undefined;

  const [phase, setPhase] = useState<Phase>('auth');
  const [bootstrapData, setBootstrapData] = useState<BootstrapData | null>(null);
  const [consent, setConsent] = useState<ConsentCurrentResponse | null>(null);
  const [error, setError] = useState<UserFacingError | null>(null);
  const [errorCode, setErrorCode] = useState<string | undefined>();
  const [traceId, setTraceId] = useState<string | undefined>();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function resolve() {
      try {
        const token = await getAccessToken();
        if (!token) {
          window.location.assign('/ingresar');
          return;
        }
        const data = await bootstrapPatient(inviteToken);
        setBootstrapData(data);
        if (!data.needsConsent) {
          window.location.assign('/dashboard');
          return;
        }
        setPhase('consent');
      } catch (err: unknown) {
        const code = (err as { code?: string }).code;
        setErrorCode(code);
        if (code === 'ONB_001_JWT_INVALID' || code === 'ONB_001_JWT_EXPIRED') {
          setError(formatUserFacingError({ code: 'SESSION_EXPIRED' }));
        } else {
          setError(
            formatUserFacingError(err, {
              fallback: {
                title: 'No pudimos iniciar tu sesión.',
                description: 'Probá de nuevo en unos minutos.',
              },
            }),
          );
        }
      } finally {
        setLoading(false);
      }
    }
    resolve();
  }, [inviteToken]);

  useEffect(() => {
    if (phase !== 'consent') return;
    async function loadConsent() {
      try {
        const data = await getCurrentConsent();
        setConsent(data);
      } catch (err: unknown) {
        const code = (err as { code?: string }).code;
        const trace = (err as { trace_id?: string }).trace_id;
        setErrorCode(code);
        setTraceId(trace);
        if (code === 'NO_CONSENT_CONFIG') {
          setError(formatUserFacingError({ code: 'NO_CONSENT_CONFIG' }));
        } else {
          setError(
            formatUserFacingError(err, {
              fallback: {
                title: 'No pudimos cargar el consentimiento.',
                description: 'Probá de nuevo en unos minutos.',
              },
            }),
          );
        }
      }
    }
    loadConsent();
  }, [phase]);

  async function handleConsentAccept(version: string) {
    try {
      await grantConsent(version);
      window.location.assign('/dashboard');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      const trace = (err as { trace_id?: string }).trace_id;
      setErrorCode(code);
      setTraceId(trace);
      if (code === 'CONSENT_VERSION_MISMATCH') {
        setError(formatUserFacingError({ code: 'CONSENT_VERSION_MISMATCH' }));
      } else if (code === 'CONSENT_ALREADY_GRANTED') {
        window.location.assign('/dashboard');
      } else {
        setError(
          formatUserFacingError(err, {
            fallback: {
              title: 'No pudimos guardar el consentimiento.',
              description: 'Probá de nuevo en unos minutos.',
            },
          }),
        );
      }
    }
  }

  function handleConsentRetry() {
    setError(null);
    setErrorCode(undefined);
    setTraceId(undefined);
  }

  if (loading) {
    return <PatientPageShell loading />;
  }

  if (error && phase === 'auth') {
    return <PatientPageShell error={error} />;
  }

  if (phase === 'auth') {
    return (
      <PatientPageShell>
        <AuthBootstrapInterstitial
          variant={bootstrapData?.resumePendingInvite ? 'invite_context' : 'default'}
        />
      </PatientPageShell>
    );
  }

  if (!consent) {
    return <PatientPageShell loading />;
  }

  return (
    <PatientPageShell>
      <ConsentGatePanel
        consent={consent}
        resumeInvite={bootstrapData?.resumePendingInvite}
        onAccept={handleConsentAccept}
        onRetry={handleConsentRetry}
        errorCode={errorCode}
        errorMessage={error?.description}
        traceId={traceId}
      />
    </PatientPageShell>
  );
}
