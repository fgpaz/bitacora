# Task T5: impeccable-extract (componentes reutilizables + memoización)

## Shared Context
**Goal:** Partir componentes monolíticos (TelegramPairingCard 455 líneas) y sumar `useMemo` a computaciones caras (Timeline SVG, Dashboard transformaciones).
**Stack:** Next.js 16 + React 19.
**Architecture:** Extraer hijos presentacionales; mantener estado y side-effects en el padre.

## Locked Decisions
- `TelegramPairingCard.tsx` se parte en 3 subcomponentes presentational en `frontend/components/patient/telegram/pairing/`:
  - `PairingCodeDisplay.tsx` — muestra el código generado + countdown + botón "Copiar mensaje".
  - `PairingInstructions.tsx` — bloque instructivo inicial ("Generá un código", copy estático).
  - `PairingReminderSection.tsx` — configurador de recordatorio + botón "Desvincular Telegram".
- El estado (polling, generating, unlinking, saving_schedule) queda en el padre `TelegramPairingCard`.
- NO se extraen hooks ni lógica de API. Sólo JSX presentational + props.
- `Timeline.tsx` suma `useMemo` para `moodPoints`, `scaleX`, paths SVG.
- `Dashboard.tsx` suma `useMemo` para el array de entries transformadas del trendChart.
- NO se toca `OnboardingFlow.tsx` (el estado machine auth → consent → redirect es crítico y tocarlo puede romper el flow congelado).

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-next-vercel
files:
  - create: frontend/components/patient/telegram/pairing/PairingCodeDisplay.tsx
  - create: frontend/components/patient/telegram/pairing/PairingCodeDisplay.module.css
  - create: frontend/components/patient/telegram/pairing/PairingInstructions.tsx
  - create: frontend/components/patient/telegram/pairing/PairingInstructions.module.css
  - create: frontend/components/patient/telegram/pairing/PairingReminderSection.tsx
  - create: frontend/components/patient/telegram/pairing/PairingReminderSection.module.css
  - modify: frontend/components/patient/telegram/TelegramPairingCard.tsx
  - modify: frontend/components/patient/telegram/TelegramPairingCard.module.css
  - modify: frontend/components/professional/Timeline.tsx
  - modify: frontend/components/patient/dashboard/Dashboard.tsx
complexity: high
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0 AND wc -l frontend/components/patient/telegram/TelegramPairingCard.tsx < 280"
```

## Reference
- Baseline §3.3 (monolíticos) y §4.3 (magic spacing en cases).
- React 19 docs `useMemo`: https://react.dev/reference/react/useMemo

## Prompt
Sos un ejecutor write. La extracción es **presentational only** — no movés lógica de side-effects.

### 5.1 — Crear `PairingCodeDisplay.tsx`
En `frontend/components/patient/telegram/pairing/PairingCodeDisplay.tsx`:
```tsx
'use client';

import { MouseEvent } from 'react';
import styles from './PairingCodeDisplay.module.css';

type Props = {
  pairingMessage: string;
  timeRemaining: string;
  isExpired: boolean;
  isCopied: boolean;
  onCopy: () => void;
  onOpenTelegram: () => void;
  onConfirm: () => void;
  onRegenerate: () => void;
  checkingPairing: boolean;
};

export function PairingCodeDisplay(props: Props) {
  const {
    pairingMessage, timeRemaining, isExpired, isCopied,
    onCopy, onOpenTelegram, onConfirm, onRegenerate, checkingPairing,
  } = props;

  if (isExpired) {
    return (
      <div className={styles.expired} role="alert">
        <p>El código venció. Generá uno nuevo y enviá el nuevo mensaje al bot.</p>
        <button type="button" className={styles.primaryBtn} onClick={onRegenerate}>
          Generar nuevo código
        </button>
      </div>
    );
  }

  return (
    <div className={styles.codeBlock}>
      <p className={styles.instruction}>Enviá este mensaje al bot. Vence en {timeRemaining}.</p>
      <pre className={styles.codeText}>{pairingMessage}</pre>
      <div className={styles.actionGrid}>
        <button type="button" className={styles.secondaryBtn} onClick={onCopy}>
          {isCopied ? 'Mensaje copiado' : 'Copiar mensaje'}
        </button>
        <button type="button" className={styles.primaryBtn} onClick={onOpenTelegram}>
          Abrir Telegram
        </button>
      </div>
      <div className={styles.confirmRow}>
        <h3 className={styles.confirmTitle}>Confirmá la vinculación</h3>
        <button
          type="button"
          className={styles.tertiaryBtn}
          onClick={onConfirm}
          disabled={checkingPairing}
        >
          {checkingPairing ? 'Comprobando…' : 'Ya envié el mensaje'}
        </button>
      </div>
    </div>
  );
}
```
CSS asociado en `PairingCodeDisplay.module.css` copia los estilos relevantes de `TelegramPairingCard.module.css` sin duplicar la hoja completa.

### 5.2 — Crear `PairingInstructions.tsx`
Componente estático presentational con el copy `"Generá un código"` + `"El código vence en 15 minutos..."` + botón `Generar código`. Props: `{ onGenerate: () => void; generating: boolean; }`. CSS local.

### 5.3 — Crear `PairingReminderSection.tsx`
Contiene el configurador de horario + botón "Probar el bot" + botón "Volver al inicio" + bloque "Desvincular Telegram" con confirm inline. Props:
```tsx
type Props = {
  schedule: string;
  onScheduleChange: (val: string) => void;
  onSaveSchedule: () => void;
  savingSchedule: boolean;
  onUnlink: () => void;
  unlinking: boolean;
  showUnlinkConfirm: boolean;
  onRequestUnlink: () => void;
  onCancelUnlink: () => void;
  error: string | null;
  savedScheduleMessage: string | null;
};
```

### 5.4 — Refactor `TelegramPairingCard.tsx`
- Eliminar los bloques JSX ahora en los 3 children.
- Importar los 3 children y renderizarlos en los branches apropiados de la máquina de estados (idle, pairing, paired).
- Mantener useEffect, fetch, polling, setState.
- Objetivo: ≤250 líneas de código.

### 5.5 — `Timeline.tsx` — `useMemo` para SVG
Abrí `frontend/components/professional/Timeline.tsx`. Localizá la función que calcula `moodPoints` / `scaleX` / paths SVG. Envolvela:
```tsx
const moodPoints = useMemo(() => {
  return entries.map((entry) => {
    const x = scaleX(entry.occurred_at);
    const y = scaleY(entry.mood_score);
    return { x, y, entry };
  });
}, [entries, preset, customFrom, customTo]);
```
Si hay cálculos auxiliares (minScore/maxScore), también envolver. Verificar con `npm run typecheck` después.

### 5.6 — `Dashboard.tsx` — `useMemo` para trend bars
Abrí `Dashboard.tsx`. Localizá el mapeo que produce las `trendBar` visibles. Envolvé la transformación en `useMemo([entries])`.

## Execution Procedure
1. Crear los 3 archivos `pairing/*` (paso 5.1-5.3) con Write.
2. Refactor `TelegramPairingCard.tsx` (paso 5.4): editar múltiples bloques JSX. Preservar todos los `useEffect`, handlers, fetches. Verificar `wc -l frontend/components/patient/telegram/TelegramPairingCard.tsx` ≤250 al terminar.
3. Edit Timeline.tsx y Dashboard.tsx con `useMemo` (pasos 5.5-5.6).
4. Verify final:
   ```bash
   cd frontend && npm run typecheck && npm run lint && npm run test:e2e
   wc -l frontend/components/patient/telegram/TelegramPairingCard.tsx
   ```
5. Si el build falla o un spec rompe, NO mergear; reportar al humano.

## Skeleton
Ya provisto en 5.1.

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
wc -l frontend/components/patient/telegram/TelegramPairingCard.tsx
```
Ambos: exit 0 + línea ≤250.

## Commit
`refactor(impeccable-extract): split TelegramPairingCard y useMemo en Timeline/Dashboard`
