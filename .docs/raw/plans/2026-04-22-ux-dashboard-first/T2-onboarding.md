# Task T2: `OnboardingFlow` sin Bridge Card, redirect a `/dashboard`

## Shared Context
**Goal:** Eliminar la fase `S04-BRIDGE` del onboarding. El flujo se reduce a `auth → consent (condicional) → /dashboard`. Borrar el componente `NextActionBridgeCard` y su CSS.
**Stack:** Next.js 16 App Router, React 19 hooks.
**Architecture:** `OnboardingFlow` se monta en `/onboarding` vía `frontend/app/(patient)/onboarding/page.tsx`. Hoy pasa por 3 fases; este task deja sólo 2 y redirige fuera.

## Locked Decisions
- Si `bootstrap.needsConsent === false` al iniciar, `OnboardingFlow` hace `window.location.assign('/dashboard')` y no renderiza nada.
- Tras `grantConsent(version)` exitoso, también hace `window.location.assign('/dashboard')`. Usamos hard redirect (no `router.push`) para rehidratar cookies limpias.
- Se elimina `NextActionBridgeCard.tsx` + `NextActionBridgeCard.module.css`.
- Se preserva el comportamiento de error (`ONB_001_JWT_INVALID`, `CONSENT_ALREADY_GRANTED`, `CONSENT_VERSION_MISMATCH`, `NO_CONSENT_CONFIG`).
- `AuthBootstrapInterstitial` sigue siendo el spinner mientras se resuelve el bootstrap.

## Task Metadata
```yaml
id: T2
depends_on: [T0]
agent_type: ps-next-vercel
files:
  - modify: frontend/components/patient/onboarding/OnboardingFlow.tsx
  - delete: frontend/components/patient/onboarding/NextActionBridgeCard.tsx
  - delete: frontend/components/patient/onboarding/NextActionBridgeCard.module.css
  - read: frontend/app/(patient)/onboarding/page.tsx
  - read: frontend/components/patient/onboarding/AuthBootstrapInterstitial.tsx
  - read: frontend/components/patient/consent/ConsentGatePanel.tsx
complexity: medium
done_when: "grep -rn 'NextActionBridgeCard\\|S04-BRIDGE' frontend/ → 0 matches; typecheck + lint verdes"
```

## Reference
- `frontend/components/patient/onboarding/OnboardingFlow.tsx` (main branch) — línea 149 tiene el bug conceptual `needsFirstEntry={bootstrapData?.needsConsent === false}`. Eliminar junto con toda la fase `bridge`.
- `frontend/components/patient/consent/ConsentGatePanel.tsx` — contratos de `onAccept(version)` y `onRetry`. No modificar.

## Prompt
Sos `ps-next-vercel`. Tu tarea es simplificar `OnboardingFlow` para que sea un gate de consent puro: si no hace falta consent, redirige a `/dashboard`; si hace falta, muestra el panel de consent y al aceptar redirige a `/dashboard`.

Reglas duras:
- No modificar `ConsentGatePanel`, `AuthBootstrapInterstitial`, `PatientPageShell`, ni `bootstrapPatient`/`grantConsent` (API client).
- No agregar fetch adicional de Telegram en este componente. Ese nudge vive en el dashboard (T3).
- Usar `window.location.assign('/dashboard')` en lugar de `router.push('/dashboard')` para garantizar rehidratación de cookies tras consent.
- Si el archivo `OnboardingFlow.tsx` no matchea la estructura descripta en Reference (ej. alguien cambió nombres de props de hooks), DETENETE y reportá.

## Execution Procedure
1. Abrir `frontend/components/patient/onboarding/OnboardingFlow.tsx`. Confirmar que define `type Phase = 'auth' | 'consent' | 'bridge'` y un `phase === 'bridge'` render.
2. Reemplazar todo el contenido del archivo con el Skeleton A.
3. Borrar los archivos:
   - `frontend/components/patient/onboarding/NextActionBridgeCard.tsx`
   - `frontend/components/patient/onboarding/NextActionBridgeCard.module.css`
4. Correr `grep -rn "NextActionBridgeCard\|S04-BRIDGE" frontend/` (o `mi-lsp nav refs NextActionBridgeCard`). Debe devolver 0 matches en código.
5. `cd frontend && npm run typecheck && npm run lint`. Ambos exit 0.
6. Commit.

## Skeleton

Skeleton A — `frontend/components/patient/onboarding/OnboardingFlow.tsx`:
```tsx
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
  const [error, setError] = useState<string | null>(null);
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
      window.location.assign('/dashboard');
    } catch (err: unknown) {
      const code = (err as { code?: string }).code;
      const trace = (err as { trace_id?: string }).trace_id;
      setErrorCode(code);
      setTraceId(trace);
      if (code === 'CONSENT_VERSION_MISMATCH') {
        setError('La versión del consentimiento cambió. Por favor, revisalo de nuevo.');
      } else if (code === 'CONSENT_ALREADY_GRANTED') {
        window.location.assign('/dashboard');
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

  if (loading) return <PatientPageShell loading />;

  if (error && phase === 'auth') return <PatientPageShell error={error} />;

  if (phase === 'auth') {
    return (
      <PatientPageShell>
        <AuthBootstrapInterstitial
          variant={bootstrapData?.resumePendingInvite ? 'invite_context' : 'default'}
        />
      </PatientPageShell>
    );
  }

  if (!consent) return <PatientPageShell loading />;

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
```

## Verify
```bash
cd frontend
grep -rn "NextActionBridgeCard\|S04-BRIDGE" . --include="*.ts" --include="*.tsx"   # expected: 0 matches
ls components/patient/onboarding/NextActionBridgeCard.tsx 2>&1                     # expected: No such file
npm run typecheck                                                                  # expected: exit 0
npm run lint                                                                       # expected: exit 0
```

## Commit
```
refactor(onboarding): eliminar Bridge Card, redirect directo a /dashboard
```
