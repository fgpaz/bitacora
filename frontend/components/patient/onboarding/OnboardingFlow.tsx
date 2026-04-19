'use client';

/**
 * OnboardingFlow — orchestrates the ONB-first authenticated journey:
 * bootstrap -> consent -> bridge.
 * States: S01-HERO-INVITE | S02-AUTH-INTERSTITIAL | S03-CONSENT-* | S04-BRIDGE
 */
import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { PatientPageShell } from '@/components/ui/PatientPageShell';
import { AuthBootstrapInterstitial } from './AuthBootstrapInterstitial';
import { NextActionBridgeCard } from './NextActionBridgeCard';
import { ConsentGatePanel } from '@/components/patient/consent/ConsentGatePanel';
import {
  bootstrapPatient,
  getCurrentConsent,
  grantConsent,
  ConsentCurrentResponse,
} from '@/lib/api/client';
import { getAccessToken } from '@/lib/auth/client';

type Phase = 'auth' | 'consent' | 'bridge';

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
  const [error, setError] = useState<string | null>(null);
  const [errorCode, setErrorCode] = useState<string | undefined>();
  const [traceId, setTraceId] = useState<string | undefined>();
  const [loading, setLoading] = useState(true);

  // S02: resolve session + bootstrap
  useEffect(() => {
    async function resolve() {
      try {
        const token = await getAccessToken();
        if (!token) {
          window.location.href = '/ingresar';
          setLoading(false);
          return;
        }
        const data = await bootstrapPatient(inviteToken);
        setBootstrapData(data);
        if (!data.needsConsent) {
          setPhase('bridge');
        } else {
          setPhase('consent');
        }
      } catch (err: unknown) {
        const code = (err as { code?: string }).code;
        setErrorCode(code);
        if (code === 'ONB_001_JWT_INVALID' || code === 'ONB_001_JWT_EXPIRED') {
          setError('Tu sesión no es válida. Por favor, ingresá de nuevo.');
        } else {
          setError((err as Error).message ?? 'No se pudo iniciar sesión. Reintentá.');
        }
      } finally {
        setLoading(false);
      }
    }
    resolve();
  }, [inviteToken]);

  // S03: load consent
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
          setError('El servicio de consentimiento no está disponible en este momento.');
        } else {
          setError((err as Error).message ?? 'Error al cargar el consentimiento.');
        }
      }
    }
    loadConsent();
  }, [phase]);

  async function handleConsentAccept(version: string) {
    try {
      await grantConsent(version);
      setBootstrapData((prev) =>
        prev ? { ...prev, needsConsent: false } : null,
      );
      setPhase('bridge');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      const trace = (err as { trace_id?: string }).trace_id;
      setErrorCode(code);
      setTraceId(trace);
      if (code === 'CONSENT_VERSION_MISMATCH') {
        setError('La versión del consentimiento cambió. Por favor, revisalo de nuevo.');
      } else if (code === 'CONSENT_ALREADY_GRANTED') {
        setPhase('bridge');
      } else {
        setError((err as Error).message ?? 'Error al guardar el consentimiento.');
      }
    }
  }

  function handleConsentRetry() {
    setError(null);
    setErrorCode(undefined);
    setTraceId(undefined);
  }

  if (loading) {
    return (
      <PatientPageShell loading />
    );
  }

  if (error && phase === 'auth') {
    return (
      <PatientPageShell error={error} />
    );
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

  if (phase === 'bridge') {
    return (
      <PatientPageShell>
        <NextActionBridgeCard needsFirstEntry={bootstrapData?.needsConsent === false} />
      </PatientPageShell>
    );
  }

  // phase === 'consent'
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
        errorMessage={error ?? undefined}
        traceId={traceId}
      />
    </PatientPageShell>
  );
}
