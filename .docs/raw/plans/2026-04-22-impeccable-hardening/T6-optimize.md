# Task T6: impeccable-optimize (next/font + RSC + prefers-reduced-motion local)

## Shared Context
**Goal:** Migrar fuentes a `next/font/google`, convertir page.tsx a Server Components donde sea seguro, sumar `prefers-reduced-motion` local en skeletons con shimmer/spin.
**Stack:** Next.js 16 App Router, `next/font/google`, RSC.
**Architecture:** Cambios a nivel layout, page.tsx y CSS de skeleton.

## Locked Decisions
- **NO tocar** `frontend/app/auth/**`, `frontend/app/api/**`, `frontend/proxy.ts`, `frontend/lib/auth/*`.
- Migrar `@fontsource` → `next/font/google` en `frontend/app/layout.tsx`. Variables CSS deben ser `--font-display`, `--font-body`, `--font-mono` (mismos nombres actuales).
- Familias: `Newsreader` (weights 400, 500, 600), `Source Sans 3` (weights 400, 500, 600), `IBM Plex Mono` (weight 400). `subsets: ['latin']`. `display: 'swap'`.
- **Eliminar** los 9 imports `@fontsource/*` del layout.tsx.
- **NO eliminar** las dependencias `@fontsource/*` de `package.json` en esta wave — queda como follow-up para evitar conflictos de lock file. Agregar comentario en commit.
- page.tsx → RSC: sólo convertir los que son wrappers delegando a componente `use client`. NO convertir los que usan `useEffect`/`useState` inline.
- Shimmer local `prefers-reduced-motion`: agregar regla específica que reemplace `animation: shimmer ...` por `animation: none` cuando el usuario lo pide.
- **NO eliminar** la regla global `tokens.css:58-64` — es backstop; las reglas locales son complementarias.

## Task Metadata
```yaml
id: T6
depends_on: [T5]
agent_type: ps-next-vercel
files:
  - modify: frontend/app/layout.tsx
  - modify: frontend/styles/tokens.css
  - modify: frontend/app/(patient)/consent/page.tsx
  - modify: frontend/app/(patient)/onboarding/page.tsx
  - modify: frontend/app/(patient)/registro/daily-checkin/page.tsx
  - modify: frontend/app/(patient)/registro/mood-entry/page.tsx
  - modify: frontend/app/(patient)/configuracion/telegram/page.tsx
  - modify: frontend/app/(patient)/configuracion/vinculos/page.tsx
  - modify: frontend/app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx
  - modify: frontend/components/patient/dashboard/Dashboard.module.css:42-45
  - modify: frontend/components/patient/onboarding/AuthBootstrapInterstitial.module.css:16
  - modify: frontend/components/patient/telegram/TelegramPairingCard.module.css:24
  - modify: frontend/components/patient/vinculos/VinculosManager.module.css:19
  - modify: frontend/components/ui/PatientPageShell.module.css:37-41
complexity: high
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e && npm run build exit 0 AND grep -rn '@fontsource' frontend/app/layout.tsx | wc -l == 0"
```

## Reference
- Next.js 16 `next/font/google`: https://nextjs.org/docs/app/api-reference/components/font
- Baseline §3.1, §3.2, §3.5.

## Prompt
Ejecutás migración + conversión RSC cuidadosa.

### 6.1 — Migrar fuentes en `layout.tsx`
Abrí `frontend/app/layout.tsx`. Estado actual (línea 1-20 aprox):
```tsx
import '@fontsource/newsreader/latin-400.css';
import '@fontsource/newsreader/latin-500.css';
// ...
import '@fontsource/source-sans-3/latin-400.css';
// ...
import '@/styles/tokens.css';
import '@/styles/globals.css';
```

Reemplazá los 9 imports `@fontsource/*` por:
```tsx
import { Newsreader, Source_Sans_3, IBM_Plex_Mono } from 'next/font/google';

const newsreader = Newsreader({
  subsets: ['latin'],
  weight: ['400', '500', '600'],
  display: 'swap',
  variable: '--font-display',
  style: ['normal', 'italic'],
});

const sourceSans = Source_Sans_3({
  subsets: ['latin'],
  weight: ['400', '500', '600'],
  display: 'swap',
  variable: '--font-body',
});

const plexMono = IBM_Plex_Mono({
  subsets: ['latin'],
  weight: ['400'],
  display: 'swap',
  variable: '--font-mono',
});
```

En el `<html>` o `<body>` del layout, agregá la prop `className`:
```tsx
<html lang="es" className={`${newsreader.variable} ${sourceSans.variable} ${plexMono.variable}`}>
  <body>{children}</body>
</html>
```

En `tokens.css`, las reglas de `--font-display`, `--font-body`, `--font-mono` deben seguir funcionando como fallback. Actualmente dicen:
```css
--font-display: 'Newsreader', Georgia, serif;
```
Estas son **fallback** por si next/font no cargó. El class-based override del `<html>` gana en especificidad. Mantener la declaración actual sin cambios.

### 6.2 — Convertir page.tsx a RSC donde sea seguro
Para cada page.tsx listado, chequear si tiene `useEffect`, `useState`, `useRef`, contextos o handlers. **Solo convertir a RSC (eliminar `'use client'`) si es un wrapper puro que delega al componente `use client`**.

Casos específicos:

#### `frontend/app/(patient)/consent/page.tsx`
Si el contenido es algo como:
```tsx
'use client';
import { OnboardingFlow } from '@/components/...';
export default function ConsentPage() { return <OnboardingFlow />; }
```
Convertir a RSC eliminando `'use client'`. OnboardingFlow es client, la page puede ser server.

#### `frontend/app/(patient)/onboarding/page.tsx`
Mismo patrón — eliminar `'use client'` si es wrapper puro.

#### `frontend/app/(patient)/registro/mood-entry/page.tsx` y `daily-checkin/page.tsx`
Si son wrappers de `MoodEntryForm` / `DailyCheckinForm` — convertir a RSC.

#### `frontend/app/(patient)/configuracion/telegram/page.tsx` y `vinculos/page.tsx`
Mismo patrón — convertir si son wrappers puros.

#### `frontend/app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx`
Si es wrapper puro — convertir. Si usa inline style con `var(--font-display)` (baseline lo mencionó), mantener el inline style (Server Component puede usarlo) pero convertir a RSC.

**NO convertir** `dashboard/page.tsx` si tiene `useEffect`. Verificar primero.

**Regla de decisión:** si después de eliminar `'use client'` el archivo compila sin errores con `npm run typecheck`, la conversión es segura. Si rompe, revertir la conversión de ese archivo.

### 6.3 — Shimmer local `prefers-reduced-motion`
Para cada archivo listado (5 CSS Modules), buscar la regla `animation: shimmer ...infinite` y agregar inmediatamente después:
```css
@media (prefers-reduced-motion: reduce) {
  .skeleton,
  .summarySkeleton,
  .entrySkeleton,
  .spinner,
  .checkingShimmer,
  .revokingShimmer {
    animation: none;
  }
}
```
Ajustar los selectores al naming real del archivo.

Específicamente:
- `Dashboard.module.css` — skeletons en `.summarySkeleton`, `.entrySkeleton`.
- `AuthBootstrapInterstitial.module.css:16` — `.spinner` (animación `spin`).
- `TelegramPairingCard.module.css:24` — `.checkingShimmer` (o nombre similar).
- `VinculosManager.module.css:19` — shimmer para revoking.
- `PatientPageShell.module.css:37-41` — shimmer loading.

## Execution Procedure
1. Migrar fuentes en layout.tsx (paso 6.1).
2. `cd frontend && npm run typecheck` — el build debe pasar.
3. Para cada page.tsx (paso 6.2):
   - Leer el archivo; si es wrapper puro, eliminar `'use client'`.
   - `npm run typecheck` tras cada conversión.
   - Si rompe, revertir ESE archivo y continuar con los demás.
4. Shimmer local (paso 6.3) en los 5 archivos.
5. Verify final:
   ```bash
   cd frontend && npm run typecheck && npm run lint && npm run test:e2e && npm run build
   grep -rn '@fontsource' frontend/app/layout.tsx  # esperar 0
   ```
6. Si `npm run build` falla por next/font, revisar que las variantes (weight) y `subsets` estén correctos.

## Skeleton
```tsx
// layout.tsx nuevo:
import { Newsreader, Source_Sans_3, IBM_Plex_Mono } from 'next/font/google';
import '@/styles/tokens.css';
import '@/styles/globals.css';

const newsreader = Newsreader({ subsets: ['latin'], weight: ['400','500','600'], display: 'swap', variable: '--font-display', style: ['normal','italic'] });
const sourceSans = Source_Sans_3({ subsets: ['latin'], weight: ['400','500','600'], display: 'swap', variable: '--font-body' });
const plexMono = IBM_Plex_Mono({ subsets: ['latin'], weight: ['400'], display: 'swap', variable: '--font-mono' });

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es" className={`${newsreader.variable} ${sourceSans.variable} ${plexMono.variable}`}>
      <body>{children}</body>
    </html>
  );
}
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e && npm run build
grep -rn '@fontsource' frontend/app/layout.tsx         # 0
grep -c "'use client'" frontend/app/**/page.tsx | tail  # al menos 3 deberían ser 0
```

## Commit
`perf(impeccable-optimize): migrar a next/font, RSC en page wrappers, reduced-motion local`
