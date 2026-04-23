# W2 — PatientPageShell strict `UserFacingError`

**Tipo:** code · **Subagente:** `ps-next-vercel` · **Duración estimada:** 20-30 min
**Salida:** 2 archivos modificados (PatientPageShell.tsx + OnboardingFlow.tsx).

---

## Contexto

El closure 2026-04-23 §6.4 flagged la prop `error?: string | null` de `PatientPageShell` como deuda. El único callsite vivo es `OnboardingFlow.tsx:122` (error branch). El contrato `UserFacingError` vive en `frontend/lib/errors/user-facing.ts` con `formatUserFacingError(err, options)`.

**Invariantes:**
- `OnboardingFlow.tsx` ya usa `formatUserFacingError()` (ver commit `15985d2` W3-harden+clarify).
- `PatientPageShell` tiene 3 imports del error branch: loading / error / ready. Solo el error branch cambia.
- Zonas congeladas: NO tocar `lib/auth/*`, `app/api/*`, `app/auth/*`, `proxy.ts`, `src/**`.

---

## T2A — Migrar `PatientPageShell.tsx`

**Antes:**
```ts
interface Props {
  children?: ReactNode;
  loading?: boolean;
  error?: string | null;
}

// ...
if (error) {
  return (
    <main className={styles.shell}>
      <div className={styles.errorState} role="alert">
        <p>{error}</p>
      </div>
    </main>
  );
}
```

**Después:**
```ts
import type { UserFacingError } from '../../lib/errors/user-facing';

interface Props {
  children?: ReactNode;
  loading?: boolean;
  error?: UserFacingError | null;
}

// ...
if (error) {
  return (
    <main className={styles.shell}>
      <div className={styles.errorState} role="alert">
        <p className={styles.errorTitle}>{error.title}</p>
        <p className={styles.errorDescription}>{error.description}</p>
        {error.retry && (
          <button
            type="button"
            className={styles.errorRetry}
            onClick={error.retry}
          >
            Reintentar
          </button>
        )}
      </div>
    </main>
  );
}
```

**CSS asociado** (`PatientPageShell.module.css`): si no existen `.errorTitle`, `.errorDescription`, `.errorRetry`, agregarlos con estilos sobrios (manifiesto 10 §refugio, sin celebración). Reutilizar variables tokens existentes: `--space-md`, `--text-primary`, `--text-muted`, `--brand-primary`, `--focus-ring`.

---

## T2B — Migrar `OnboardingFlow.tsx`

**Callsites de `setError(string)` a migrar a `UserFacingError`:**

- L60: `setError('Tu sesión no es válida. Por favor, ingresá de nuevo.')` → usar `formatUserFacingError({ code: 'SESSION_EXPIRED' })`.
- L62: `setError('No pudimos iniciar tu sesión. Probá de nuevo en unos minutos.')` → usar `formatUserFacingError(err)` (fallback genérico), con retry = () => window.location.reload().
- L83: `setError('El servicio de consentimiento no está disponible en este momento.')` → `formatUserFacingError({ code: 'NO_CONSENT_CONFIG' }, { retry: handleConsentRetry })`.
- L85: `setError('No pudimos cargar el consentimiento. Probá de nuevo en unos minutos.')` → `formatUserFacingError(err, { retry: handleConsentRetry })`.
- L102: `setError('La versión del consentimiento cambió. Por favor, revisalo de nuevo.')` → `formatUserFacingError({ code: 'CONSENT_VERSION_MISMATCH' }, { retry: handleConsentRetry })`.
- L106: `setError('No pudimos guardar el consentimiento. Probá de nuevo en unos minutos.')` → `formatUserFacingError(err)` con fallback concreto.

**Cambios en state:**

```ts
const [error, setError] = useState<string | null>(null);
// →
const [error, setError] = useState<UserFacingError | null>(null);
```

**Render:**

```tsx
// Antes
if (error && phase === 'auth') {
  return <PatientPageShell error={error} />;
}

// Después: mismo, ahora error es UserFacingError
```

```tsx
// Antes
<ConsentGatePanel ... errorMessage={error ?? undefined} />

// Después: extraer el description del UserFacingError
<ConsentGatePanel ... errorMessage={error?.description} />
```

Nota: `ConsentGatePanel.errorMessage` sigue aceptando string; no la migramos en esta wave porque tiene otros callers (InlineFeedback) y expandiría el scope. Se pasa `error?.description` como mejor aproximación al mensaje que antes venía como string.

---

## T2C — Verificación

```bash
npm --prefix frontend run typecheck    # exit 0
npm --prefix frontend run lint         # exit 0
```

**Grep zonas congeladas:**
```bash
git diff --name-only main..HEAD | grep -E "^frontend/(lib/auth/|app/api/|app/auth/|proxy\.ts|src/)"
# esperado: 0 líneas
```

---

## T2D — Commit W2

```bash
git add frontend/components/ui/PatientPageShell.tsx
git add frontend/components/ui/PatientPageShell.module.css
git add frontend/components/patient/onboarding/OnboardingFlow.tsx

git commit -m "$(cat <<'EOF'
refactor(w2-followups): PatientPageShell prop error strict UserFacingError

Cierra follow-up #4 del closure login-flow-redesign 2026-04-23.

- PatientPageShell.Props.error: string | null -> UserFacingError | null.
- Render error branch: title + description + boton retry opcional.
- OnboardingFlow.tsx: 6 callsites de setError migrados via formatUserFacingError (SESSION_EXPIRED, NO_CONSENT_CONFIG, CONSENT_VERSION_MISMATCH y fallbacks).
- CSS: agregados .errorTitle, .errorDescription, .errorRetry en PatientPageShell.module.css.
- ConsentGatePanel.errorMessage recibe error?.description (compat minima sin expandir scope).

typecheck + lint exit 0. 0 cruces zonas congeladas. 0 deps nuevas.

- Gabriel Paz -
EOF
)"
```
