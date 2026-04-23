# W4 — Analytics stub + 4 eventos instrumentados

**Tipo:** code · **Subagente:** `ps-next-vercel` · **Duración estimada:** 30 min
**Salida:** 1 archivo nuevo + 4 callsites instrumentados. 0 deps nuevas.

---

## Contexto

Closure 2026-04-23 §6.3 flagged gap de analytics para validar el impacto del rediseño. Sin endpoint backend ni vendor instalado (grep confirmó 0 matches de mixpanel/amplitude/plausible/sentry/datadog).

**Decisión humana (AskUserQuestion):** stub con `console.info` + estructura lista para endpoint futuro.

**Invariantes:**
- 0 deps nuevas.
- 0 cruces zonas congeladas.
- 0 cambios de comportamiento funcional (solo side-effect de tracking).

---

## T4A — Crear `frontend/lib/analytics/track.ts`

```ts
/**
 * Analytics — tracking stub.
 *
 * Firma estable para instrumentar eventos del front sin dependencia
 * de vendor externo ni endpoint backend.
 *
 * Por ahora: console.info('[analytics]', event, props).
 * TODO: reemplazar con fetch('/api/analytics', { method: 'POST', body: ... })
 * o navigator.sendBeacon cuando el endpoint backend este disponible.
 *
 * Eventos definidos (2026-04-23 login flow redesign):
 * - time_to_cta_ready: ms desde dashboard ready hasta openDialog.
 * - ctr_rail_vs_checkin: which CTA del actionRail se clickea.
 * - logout_accidental_rate: logout con <3 min de uso post dashboard ready.
 * - decline_consent_rate: decline vs accept en consent.
 */

export type AnalyticsEvent =
  | 'time_to_cta_ready'
  | 'ctr_rail_vs_checkin'
  | 'logout_accidental_rate'
  | 'decline_consent_rate';

export interface AnalyticsProps {
  [key: string]: string | number | boolean | null | undefined;
}

export function track(event: AnalyticsEvent, props?: AnalyticsProps): void {
  if (typeof window === 'undefined') return;
  try {
    // eslint-disable-next-line no-console
    console.info('[analytics]', event, props ?? {});
    // TODO: cuando el endpoint backend este disponible:
    // navigator.sendBeacon('/api/analytics', JSON.stringify({ event, props, ts: Date.now() }));
  } catch {
    // Swallow — analytics nunca debe romper UX.
  }
}
```

---

## T4B — Instrumentar `time_to_cta_ready` y `ctr_rail_vs_checkin` en `Dashboard.tsx`

**`time_to_cta_ready`:** emitir al entrar en viewState=ready si es la primera transición ready. Track timestamp de cuando viewState cambió a ready, y al click de openDialog, emitir delta.

```ts
import { track } from '@/lib/analytics/track';

// Dentro del componente:
const readyAtRef = useRef<number | null>(null);

useEffect(() => {
  if (viewState === 'ready' && readyAtRef.current === null) {
    readyAtRef.current = performance.now();
  }
}, [viewState]);

function openDialog() {
  if (readyAtRef.current !== null) {
    const deltaMs = Math.round(performance.now() - readyAtRef.current);
    track('time_to_cta_ready', { source: 'ready', delta_ms: deltaMs });
    readyAtRef.current = null; // No doble-contar.
  } else {
    track('time_to_cta_ready', { source: 'empty_or_retry', delta_ms: null });
  }
  setDialogOpen(true);
}
```

**`ctr_rail_vs_checkin`:** emitir al click del primary (`"+ Nuevo registro"`) vs secondary (`"Check-in diario"` link).

```tsx
<button ... onClick={() => { track('ctr_rail_vs_checkin', { target: 'new_entry' }); openDialog(); }}>
  + Nuevo registro
</button>
<Link
  href="/registro/daily-checkin"
  className={styles.secondaryLink}
  onClick={() => track('ctr_rail_vs_checkin', { target: 'daily_checkin' })}
>
  Check-in diario
</Link>
```

Nota: no mover la logica de `openDialog` fuera del componente; solo envolver el callback del onClick.

---

## T4C — Instrumentar `logout_accidental_rate` en `ShellMenu.tsx` + `PatientPageShell.tsx`

El proxy de "accidental logout" es: logout con <3 minutos de uso post-dashboard-ready. Track `dashboardReadyAt` en PatientPageShell + delta al click de "Cerrar sesión".

**`PatientPageShell.tsx`:**

```ts
import { useEffect, useRef } from 'react';
import { track } from '../../lib/analytics/track';

// ...
const mountedAtRef = useRef<number>(performance.now());

async function handleLogout() {
  const uptimeMs = Math.round(performance.now() - mountedAtRef.current);
  track('logout_accidental_rate', {
    uptime_ms: uptimeMs,
    accidental: uptimeMs < 180_000, // <3 min proxy
  });
  await signOut();
}
```

Nota: el componente ya es `'use client'`. `performance.now()` es seguro en client.

---

## T4D — Instrumentar `decline_consent_rate` en `ConsentGatePanel.tsx`

```ts
import { track } from '@/lib/analytics/track';

async function handleAccept() {
  track('decline_consent_rate', { target: 'accept', version: consent.version });
  setSubmitting(true);
  try {
    await onAccept(consent.version);
  } finally {
    setSubmitting(false);
  }
}

function handleDecline() {
  track('decline_consent_rate', { target: 'decline', version: consent.version });
  router.push('/?declined=1');
}
```

---

## T4E — Verificación

```bash
npm --prefix frontend run typecheck   # exit 0
npm --prefix frontend run lint        # exit 0
```

**Grep zonas congeladas:**
```bash
git diff --name-only main..HEAD | grep -E "^frontend/(lib/auth/|app/api/|app/auth/|proxy\.ts|src/)"
# esperado: 0
```

**Grep deps nuevas:**
```bash
git diff main..HEAD -- frontend/package.json frontend/package-lock.json
# esperado: vacio
```

**Sanity test en navegador** (opcional, no bloqueante): abrir DevTools console, navegar al dashboard, clickear "+ Nuevo registro" → debe verse log `[analytics] time_to_cta_ready {source: "ready", delta_ms: <N>}`.

---

## T4F — Commit W4

```bash
git add frontend/lib/analytics/track.ts
git add frontend/components/patient/dashboard/Dashboard.tsx
git add frontend/components/patient/consent/ConsentGatePanel.tsx
git add frontend/components/ui/PatientPageShell.tsx

git commit -m "$(cat <<'EOF'
feat(w4-followups): analytics stub + 4 eventos instrumentados

Cierra follow-up #3 del closure login-flow-redesign 2026-04-23.

- lib/analytics/track.ts nuevo: firma track(event, props) -> console.info con TODO sendBeacon para endpoint futuro. 0 deps nuevas.
- Dashboard.tsx: instrumenta time_to_cta_ready (ms desde ready hasta openDialog) y ctr_rail_vs_checkin (new_entry vs daily_checkin).
- PatientPageShell.tsx: instrumenta logout_accidental_rate (proxy: uptime_ms < 3min post mount).
- ConsentGatePanel.tsx: instrumenta decline_consent_rate (accept vs decline con version).

typecheck + lint exit 0. 0 cruces zonas congeladas. 0 deps nuevas. Instrumentacion no-intrusiva (swallow + guard SSR).

- Gabriel Paz -
EOF
)"
```
