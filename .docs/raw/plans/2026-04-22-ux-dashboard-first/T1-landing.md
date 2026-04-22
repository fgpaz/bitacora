# Task T1: Landing pública con un único CTA "Ingresar"

## Shared Context
**Goal:** Landing `/` declarativa, sin form de email, sin estado transitorio "Revisá tu correo". Un solo `Link` primario `Ingresar` que va a `/ingresar` (OIDC+PKCE server-side).
**Stack:** Next.js 16 App Router, React 19, TS, CSS Modules.
**Architecture:** `frontend/app/page.tsx` renderiza `OnboardingEntryHero`. `OnboardingEntryHero` ya soporta dos ramas (form vs Link); este task elimina la rama form.

## Locked Decisions
- CTA primario label: `Ingresar` (no "Empezar ahora" ni "Iniciar sesión con Zitadel").
- Destino del CTA: `/ingresar` (el route handler server-side gestiona state/nonce/verifier PKCE).
- Se elimina `signInWithMagicLink` de `frontend/lib/auth/client.ts`. Único entrypoint restante: `signInWithZitadel`.
- `OnboardingEntryHero` conserva las variantes `invite` y `invite_fallback` (labels `inviteLabel`). No se tocan.
- Footer `¿Problemas para acceder? Contactar soporte` (mailto) se conserva.
- Todo el texto en español con tildes y signos de apertura (`¿`, `¡`). No ASCII-izar.

## Task Metadata
```yaml
id: T1
depends_on: [T0]
agent_type: ps-next-vercel
files:
  - modify: frontend/app/page.tsx
  - modify: frontend/components/patient/onboarding/OnboardingEntryHero.tsx
  - modify: frontend/components/patient/onboarding/OnboardingEntryHero.module.css
  - modify: frontend/lib/auth/client.ts
  - read: frontend/app/ingresar/route.ts
complexity: low
done_when: "cd frontend && npm run typecheck && npm run lint exits 0, y grep -r signInWithMagicLink frontend/ no encuentra matches"
```

## Reference
- `frontend/app/ingresar/route.ts` — confirma que /ingresar implementa OIDC+PKCE server-side y respeta `login_hint` opcional. NO modificar.
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` (versión actual en `main`) — la rama Link que queda es la que hoy vive en líneas 98-104.

## Prompt
Sos `ps-next-vercel`. Tu objetivo es simplificar la landing pública de Bitácora eliminando el falso flujo de magic link y dejando un único CTA `Ingresar`.

Bases:
- Bitácora es una app de salud mental. Copy debe ser honesto y claro. No prometer correos que no se envían.
- OIDC+PKCE se negocia server-side en `/ingresar` (ya existe). La landing sólo debe linkear ahí.
- No agregar Suspense, no agregar validación de formulario, no agregar Zod. Es una página declarativa.
- No introducir dependencias nuevas. No tocar CLAUDE.md.

Si al abrir cualquier archivo su estructura difiere de la descripta acá, DETENETE y reportá el diff.

## Execution Procedure
1. Validar workspace: `cd C:\repos\mios\humor\frontend && ls app/page.tsx components/patient/onboarding/OnboardingEntryHero.tsx lib/auth/client.ts`. Si falta alguno, detener y reportar.
2. Usar `mi-lsp nav refs signInWithMagicLink --workspace bitacora --format toon` (fallback `rg "signInWithMagicLink" frontend/`) para listar call sites.
3. Reescribir `frontend/app/page.tsx` con el Skeleton A.
4. Reescribir `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` con el Skeleton B. Props: sólo `variant` + `professionalName`. Sin form.
5. Editar `frontend/components/patient/onboarding/OnboardingEntryHero.module.css`: eliminar las clases `.form`, `.emailLabel`, `.emailInput`, `.errorMessage`. Dejar el resto intacto.
6. Editar `frontend/lib/auth/client.ts`: eliminar la función `signInWithMagicLink` completa (y su firma). Conservar `signInWithZitadel`, `signOut`, `getAccessToken`.
7. Re-grep `signInWithMagicLink`. Debe devolver 0 matches.
8. Ejecutar `npm run typecheck` y `npm run lint` desde `frontend/`. Ambos deben exitear 0.
9. Commit (ver sección Commit).

## Skeleton

Skeleton A — `frontend/app/page.tsx`:
```tsx
import { OnboardingEntryHero } from '@/components/patient/onboarding/OnboardingEntryHero';

export default function HomePage() {
  return <OnboardingEntryHero variant="standard" />;
}
```

Skeleton B — `frontend/components/patient/onboarding/OnboardingEntryHero.tsx`:
```tsx
import Link from 'next/link';
import styles from './OnboardingEntryHero.module.css';

interface Props {
  variant?: 'standard' | 'invite' | 'invite_fallback';
  professionalName?: string;
}

export function OnboardingEntryHero({ variant = 'standard', professionalName }: Props) {
  return (
    <div className={styles.hero}>
      <header className={styles.header}>
        <span className={styles.wordmark}>Bitacora</span>
      </header>

      <div className={styles.body}>
        {variant === 'invite' && professionalName && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompañamiento de {professionalName}
          </p>
        )}
        {variant === 'invite_fallback' && (
          <p className={styles.inviteLabel}>
            Registro inicial con acompañamiento profesional
          </p>
        )}

        <h1 className={styles.headline}>Tu espacio personal de registro</h1>

        <p className={styles.sub}>
          Un lugar tranquilo para llevar tu registro de humor y bienestar,
          con la tranquilidad de que tus datos son privados.
        </p>

        <div className={styles.ctaStack}>
          <Link href="/ingresar" className={styles.primaryCta}>
            Ingresar
          </Link>
        </div>

        <p className={styles.privacyNote}>
          La privacidad de tus datos es fundamental. Nadie más puede ver lo que registrás.
        </p>
      </div>

      <footer className={styles.footer}>
        <p className={styles.footerText}>¿Problemas para acceder?</p>
        <a href="mailto:soporte@nuestrascuentitas.com" className={styles.footerLink}>
          Contactar soporte
        </a>
      </footer>
    </div>
  );
}
```

Skeleton C — edición puntual en `frontend/lib/auth/client.ts`:
```ts
// Eliminar la función signInWithMagicLink completa.
// Dejar únicamente:
export async function signInWithZitadel(email?: string): Promise<{ error: string | null }> { /* ... */ }
export async function signOut(): Promise<void> { /* ... */ }
export async function getAccessToken(): Promise<string | null> { /* ... */ }
```

## Verify
```bash
cd frontend
grep -r "signInWithMagicLink" . --include="*.ts" --include="*.tsx"   # expected: no matches
npm run typecheck                                                    # expected: exit 0
npm run lint                                                         # expected: exit 0
```

## Commit
```
feat(ux): landing con un único CTA "Ingresar" sin falso magic link
```
