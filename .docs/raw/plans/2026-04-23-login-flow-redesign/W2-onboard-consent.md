# Wave 2 — Onboarding + Consent + Continuidad recurrente (Redesign 2026-04-23)

**Objetivo:** Cerrar 5 hallazgos P1 del audit que responden a la queja del product owner sobre la experiencia del recurrente y al gap de compliance (Ley 26.529) sobre consent. Al finalizar W2, el paciente con cookie viva que abre `/` ve un hero de retorno con CTA "Seguir registrando" (sin flash), el consent tiene CTA secundario "Ahora no" con salida respetuosa, el inviteHint está reubicado, la revocabilidad es visible, y el dashboard tiene heading de refugio + subtítulo de privacidad.

**Scope**: R-P1-1, R-P1-2, R-P1-3 (⚠ legal-review pre-deploy), R-P1-4, R-P1-8.

**Restricción absoluta**: Server Component en `app/page.tsx` NO importa nada de `lib/auth/*` — solo lee cookie `bitacora_session` (nombre literal) con `cookies()` de `next/headers`. Zero toque a `proxy.ts`.

---

## T2A — R-P1-1: Server Component en `app/page.tsx` + variant `"returning"`

**Skill líder:** `impeccable-onboard` — continuidad emocional para el recurrente.

**Archivos:**
- `frontend/app/page.tsx` — convertir a Server Component async con `cookies()`.
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` — agregar branch `variant === 'returning'`.
- `frontend/components/patient/onboarding/OnboardingEntryHero.module.css` — estilo opcional del branch returning (puede reusar `.primaryCta`).

**Evidencia base (Explorer 3 + Explorer 5):**
- `app/page.tsx:4`: `<OnboardingEntryHero variant="standard" />` fijo.
- `lib/auth/constants.ts:1`: `SESSION_COOKIE = 'bitacora_session'` (confirmado por Explorer 5).
- `proxy.ts:36`: `request.cookies.get(SESSION_COOKIE)?.value` — confirma que la cookie es observable server-side.
- OnboardingEntryHero tiene variants `standard | invite | invite_fallback` — agregamos un cuarto: `returning`.

**Cambio concreto:**
1. `app/page.tsx` convierte a async Server Component que lee `cookies()`:
   ```tsx
   import { cookies } from 'next/headers';
   import { OnboardingEntryHero } from '@/components/patient/onboarding/OnboardingEntryHero';

   export default async function Home() {
     const cookieStore = await cookies();
     const hasSession = cookieStore.has('bitacora_session');
     return <OnboardingEntryHero variant={hasSession ? 'returning' : 'standard'} />;
   }
   ```
   **Nota crítica**: se hardcodea el string literal `'bitacora_session'` para evitar el import de `lib/auth/constants` (zona congelada).

2. `OnboardingEntryHero.tsx` extiende el type de `variant`:
   ```tsx
   interface Props {
     variant: 'standard' | 'invite' | 'invite_fallback' | 'returning';
     professionalName?: string;
   }
   ```

3. Agregar branch:
   ```tsx
   {variant === 'returning' && (
     <>
       <h1 className={styles.headline}>Volviste.</h1>
       <p className={styles.sub}>Seguir donde dejaste.</p>
       <div className={styles.ctaStack}>
         <Link href="/dashboard" className={styles.primaryCta}>
           Seguir registrando
         </Link>
       </div>
     </>
   )}
   ```

**Canon referenciado:**
- Manifesto 10 §continuidad emocional ("no debe sentirse primera vez cada vez").
- Canon 13 §voz humana, sereno ("Volviste." es sobrio; evita "¡Hola de nuevo!" celebratorio).
- Patrón 16 #10 "Hero contextual adaptativo" — misma estructura, cambio fuerte de copy.

**Checkpoint T2A:**
- [ ] `app/page.tsx` es async Server Component.
- [ ] `cookies()` importado de `next/headers` (NO `lib/auth/*`).
- [ ] Cookie name hardcoded como string literal `'bitacora_session'` (anotado con comentario que referencia la constante upstream).
- [ ] `variant="returning"` renderiza h1 "Volviste." + sub + CTA "Seguir registrando" → `/dashboard`.
- [ ] Render server-side verificado: curl con cookie devuelve HTML con "Volviste.".
- [ ] Sin flash de landing (HTML ya viene con variant correcto).

**Commit propuesto:**
```
feat(w2-onboard): R-P1-1 continuidad recurrente via Server Component en app/page.tsx

app/page.tsx ahora es async Server Component y lee la cookie bitacora_session
con cookies() de next/headers. Si la cookie esta presente, renderiza
OnboardingEntryHero variant="returning" con h1 "Volviste." + CTA "Seguir
registrando" -> /dashboard. Si no, mantiene variant="standard".

Render server-side -> cero flash de landing para el recurrente (mismo UX
objetivo que middleware-based, pero sin tocar proxy.ts que esta en zona
congelada). La cookie name se hardcode como string literal para evitar import
de lib/auth/constants (tambien zona congelada).

Responde audit 2026-04-23 findings E1-F2 P1 + E1-F5 P2. Manifesto 10
§continuidad emocional + Canon 13 §voz sereno + Patron 16 #10 "Hero contextual
adaptativo".

Zero toque a lib/auth/*, app/api/*, proxy.ts.

- Gabriel Paz -
```

---

## T2B — R-P1-2: Reubicar inviteHint en consent + inviteLabel en hero

**Skill líder:** `impeccable-onboard`.

**Archivos:**
- `frontend/components/patient/consent/ConsentGatePanel.tsx` — mover `inviteHint` después del `.sections`.
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` — mover `inviteLabel` bajo `h1` (solo para variants invite / invite_fallback).

**Evidencia base (Explorer 3):**
- `ConsentGatePanel.tsx:52-65`: inviteHint aparece antes del `<div className={styles.sections}>`.
- `OnboardingEntryHero.tsx:17-28`: inviteLabel aparece antes del `<h1>`.

**Cambio concreto:**
1. En `ConsentGatePanel.tsx`:
   - Quitar el `<p className={styles.inviteHint}>` de su posición actual (después de `<header>`).
   - Reposicionarlo dentro o inmediatamente después del `<div className={styles.sections}>`, antes del decisionBar.
2. En `OnboardingEntryHero.tsx`:
   - El inviteLabel se renderiza bajo el `<h1>`, entre el h1 y el sub.

**Canon referenciado:**
- Manifesto 10 §10.1 — "instala vigilancia antes que control" aplica al inviteHint/inviteLabel que hoy preceden.
- Patrón 16 #10 — mismo fix que critique 2026-04-22 HA-1 pendiente.

**Checkpoint T2B:**
- [ ] `ConsentGatePanel.tsx`: inviteHint aparece después del `.sections`.
- [ ] `OnboardingEntryHero.tsx`: inviteLabel aparece después del h1 en variants invite / invite_fallback.
- [ ] Copy literal preservado (solo se mueve de posición).

**Commit propuesto:**
```
refactor(w2-onboard): R-P1-2 reubicar inviteHint post-sections y inviteLabel post-h1

En ConsentGatePanel.tsx el inviteHint ("Recorda que viniste a traves de una
invitacion de tu profesional.") ahora aparece DESPUES del bloque .sections,
antes del decisionBar. El paciente lee primero el contenido del consent y
luego el contexto de la invitacion.

En OnboardingEntryHero.tsx el inviteLabel se renderiza bajo el h1 "Tu espacio
personal de registro" en lugar de antes, para que la promesa del producto
preceda al contexto de invitacion.

Responde audit 2026-04-23 E2-F1 P1 + E1-F4 P2. Manifesto 10 §10.1 ("instala
vigilancia antes que control") + critique 2026-04-22 HA-1 pendiente.

Copy literal preservado (solo reposicionamiento).

- Gabriel Paz -
```

---

## T2C — R-P1-3: CTA "Ahora no" en consent ⚠ legal-review pre-deploy

**Skill líder:** `impeccable-harden`.

**Archivos:**
- `frontend/components/patient/consent/ConsentGatePanel.tsx` — agregar botón secundario + handler de decline.
- `frontend/components/patient/consent/ConsentGatePanel.module.css` — estilo del botón secundario + jerarquía visual.

**Evidencia base (Explorer 3):**
- `ConsentGatePanel.tsx:87-99`: solo `<button className={styles.acceptBtn}>Aceptar y continuar</button>`.
- `.module.css:59-69`: decisionBar `position: fixed bottom: 0`.

**Cambio concreto:**
1. Agregar botón secundario en el decisionBar:
   ```tsx
   <div className={styles.decisionBar}>
     <button
       type="button"
       onClick={handleDecline}
       className={styles.declineBtn}
     >
       Ahora no
     </button>
     <button
       type="submit"
       onClick={handleAccept}
       className={styles.acceptBtn}
     >
       {submitting ? 'Guardando...' : 'Aceptar y continuar'}
     </button>
   </div>
   ```

2. `handleDecline`:
   ```tsx
   function handleDecline() {
     // No borrar cookie (sesion sigue activa).
     // Redirigir a / con query param ?declined=1 para que app/page.tsx NO redirija
     // al dashboard durante esta navegacion (mitiga R4 del plan maestro).
     router.push('/?declined=1');
   }
   ```
   **IMPORTANTE**: `app/page.tsx` en T2A debe respetar `searchParams.declined === '1'` y renderizar `variant="standard"` o una variante neutral con mensaje sereno. Ajuste adicional en T2A o sub-ajuste aquí:
   ```tsx
   export default async function Home({
     searchParams,
   }: {
     searchParams: Promise<{ declined?: string }>;
   }) {
     const params = await searchParams;
     const cookieStore = await cookies();
     const hasSession = cookieStore.has('bitacora_session');
     const declined = params.declined === '1';
     const variant = declined ? 'standard' : hasSession ? 'returning' : 'standard';
     return <OnboardingEntryHero variant={variant} />;
   }
   ```

3. Mensaje de decline visible (inline en `OnboardingEntryHero` si `declined === true`, o en landing como banner):
   - Opción más simple: que `handleDecline` haga `alert` o setee un estado local con mensaje antes del redirect. Preferido: redirect con query param y el hero muestra un `role=status aria-live=polite` con `"Podés aceptar cuando quieras. Tu sesión sigue activa."`. El hero acepta un prop opcional `message?: string` para esto.

4. Jerarquía visual CSS: `.declineBtn` debe ser **secundario silencioso** (menos peso que `.acceptBtn`, canon 12 §CTA). Borde o texto, sin fondo terracota.

**Canon referenciado:**
- Manifesto 10 §7.2 capa conductual + §autonomía del paciente.
- Ley 26.529 Art. 2 (Argentina) — autonomía informada exige rechazo posible.
- Canon 13 §mensajes sensibles ("Podés aceptar cuando quieras. Tu sesión sigue activa." cumple: sereno, respetuoso, sin dramatizar).
- Patrón 16 #5 "Rail final de guardado" — par CTA con secundario silencioso.

**⚠ Flag legal pre-deploy:**
El commit message DEBE incluir explícitamente:
> `CAVEAT: deploy a produccion requiere validacion del equipo legal sobre el wording del CTA secundario y del mensaje de decline. Hard gate funcional de RF-CON-003 preservado (sin consent no hay registro); el CTA "Ahora no" solo ofrece salida respetuosa sin romper el gate.`

**Checkpoint T2C:**
- [ ] `ConsentGatePanel.tsx` tiene ambos botones en el decisionBar.
- [ ] `handleDecline` redirige a `/?declined=1` sin borrar cookie.
- [ ] `app/page.tsx` respeta `?declined=1` para NO redirigir al dashboard.
- [ ] Jerarquía visual CSS: `acceptBtn` primario + `declineBtn` secundario silencioso.
- [ ] Commit message contiene CAVEAT legal.
- [ ] Mensaje de post-decline visible para el paciente.

**Commit propuesto:**
```
feat(w2-harden): R-P1-3 CTA secundario Ahora no en consent con salida respetuosa

Agrega boton secundario "Ahora no" al decisionBar del ConsentGatePanel que
redirige a /?declined=1 sin borrar la cookie de sesion. app/page.tsx respeta
el query param para NO redirigir al dashboard durante la navegacion declinada,
y muestra mensaje sereno "Podes aceptar cuando quieras. Tu sesion sigue
activa." (canon 13 §mensajes sensibles).

Jerarquia visual: acceptBtn dominante (primario) + declineBtn secundario
silencioso (canon 12 §CTA + Patron 16 #5). Hard gate funcional de RF-CON-003
preservado: sin consent no hay acceso a endpoints de datos.

Responde audit 2026-04-23 finding E2-F2 P1 + Ley 26.529 Art. 2 (autonomia
informada). Manifesto 10 §7.2 capa conductual + §autonomia del paciente.

CAVEAT: deploy a produccion requiere validacion del equipo legal sobre el
wording del CTA secundario y del mensaje de decline. No mergear a main sin
revisar el texto con abogado.

Zero toque a zonas congeladas. handleDecline usa router.push estandar sin
invocar signOut ni lib/auth.

- Gabriel Paz -
```

---

## T2D — R-P1-4: Revocabilidad visible en consent

**Skill líder:** `impeccable-clarify`.

**Archivos:**
- `frontend/components/patient/consent/ConsentGatePanel.tsx` — agregar `<p className={styles.revocationNote}>` cerca del decisionBar.
- `frontend/components/patient/consent/ConsentGatePanel.module.css` — estilo sobrio.

**Evidencia base (Explorer 3):**
- `ConsentGatePanel.tsx` completo: cero menciones de "revocar" o reversibilidad.

**Cambio concreto:**
Agregar texto breve antes del `.decisionBar` (encima de los botones):
```tsx
<p className={styles.revocationNote}>
  Podés revocarlo cuando quieras desde Mi cuenta.
</p>
```

(Nota: el link directo a `/configuracion/vinculos` llegará en R-P1-6 Wave 3. Por ahora solo texto orientador que instala el frame de reversibilidad.)

**Canon referenciado:**
- Canon 13 §estados sensibles — "Hacer visible la reversibilidad cuando exista".
- Manifesto 10 §8.1 — "controles visibles para activar, desactivar o revocar".

**Checkpoint T2D:**
- [ ] Texto `"Podés revocarlo cuando quieras desde Mi cuenta."` visible sobre el decisionBar.
- [ ] Estilo sereno (muted, no alerta).

**Commit propuesto:**
```
feat(w2-clarify): R-P1-4 revocabilidad explicita en consent

Agrega parrafo sobrio encima del decisionBar: "Podes revocarlo cuando quieras
desde Mi cuenta.". El paciente ahora lee explicitamente que el consent es
reversible antes de aceptar.

Responde audit 2026-04-23 finding E2-F3 P1. Canon 13 §estados sensibles +
Manifesto 10 §8.1 "controles visibles para activar, desactivar o revocar".

El link directo a /configuracion/vinculos llegara en R-P1-6 Wave 3 via
ShellMenu; por ahora solo texto orientador que instala el frame de
reversibilidad.

- Gabriel Paz -
```

---

## T2E — R-P1-8: Heading dashboard saludo contextual + subtítulo

**Skill líder:** `impeccable-onboard`.

**Archivos:**
- `frontend/app/(patient)/dashboard/page.tsx` — cambiar h1.
- `frontend/app/(patient)/dashboard/page.module.css` (o el CSS que estile el heading) — ajustar si el h1 es más largo.

**Evidencia base (Explorer 1):**
- `app/(patient)/dashboard/page.tsx:22`: `<h1 id="dashboard-heading" className={styles.heading}>Mi historial</h1>`.

**Cambio concreto:**
```tsx
<h1 id="dashboard-heading" className={styles.heading}>
  Hola. Acá está lo que registraste.
</h1>
<p className={styles.subtitle}>
  Solo vos ves lo que registrás. Tus datos son privados.
</p>
```

CSS debe:
- Permitir `clamp()` tipográfico para el h1 más largo en 360px.
- Subtítulo con `var(--foreground-muted)` y tipografía ligera.
- Metadata title puede mantener `'Mi historial | Bitácora'` (no confunde al usuario y mantiene SEO).

**Canon referenciado:**
- Manifesto 10 §5.1 — "Acá puedo registrar cómo estoy sin quedar expuesta." Frame de refugio.
- Canon 13 §tono sereno, sin elogio, sin coach.

**Checkpoint T2E:**
- [ ] h1 es `"Hola. Acá está lo que registraste."`.
- [ ] Subtítulo es `"Solo vos ves lo que registrás. Tus datos son privados."`.
- [ ] 360px no desborda (clamp aplicado).
- [ ] Metadata title preservada (`"Mi historial | Bitácora"`).
- [ ] E2E selectors text-based adaptados si alguno esperaba el literal "Mi historial".

**Commit propuesto:**
```
feat(w2-onboard): R-P1-8 heading dashboard saludo contextual + subtitulo de privacidad

Reemplaza h1 "Mi historial" por saludo contextual "Hola. Aca esta lo que
registraste." + subtitulo "Solo vos ves lo que registras. Tus datos son
privados." El frame emocional cambia de archivo a refugio (canon 10 §5.1).

Decision humana cerrada en brainstorming 2026-04-23: el copy nuevo esta
alineado al canon 13 (sereno, humano, no-coach) y absorbe parte del congelado
sobre privacidad sin duplicar literal con el landing.

Responde audit 2026-04-23 finding E3-F2 + E3-F7 P1 (needs-human-decision
resuelta). Manifesto 10 §5.1 frame de refugio + Canon 13 §tono sereno.

CSS usa clamp() para evitar desbordar en 360px.

Metadata title preservada como "Mi historial | Bitacora" para no romper SEO.

- Gabriel Paz -
```

---

## T2F — Cierre W2: review + typecheck + lint + e2e + grep zonas congeladas

**Skill líder:** `ps-code-reviewer` + `ps-worker`.

**Pasos:**
1. Dispatch `ps-code-reviewer` con diff completo W2 (5 commits).
2. Correr `cd frontend && npm run typecheck && npm run lint && npm run test:e2e`.
3. Grep zonas congeladas en archivos tocados W2.
4. Verificar que el specs `landing.spec.ts` siga pasando. El test assert sobre el headline `"Tu espacio personal de registro"` — con Server Component detectando cookie, el test por defecto (sin cookie) sigue viendo `variant="standard"` y el headline congelado. Sin cambios de selector necesarios.
5. Reportar + checkpoint humano.

**Done when:**
- ps-code-reviewer APROBADO.
- typecheck + lint + 8/8 e2e verde.
- Grep zonas congeladas = 0.
- Caveat legal de R-P1-3 documentado en el commit.

---

## Checkpoint humano W2

Reportar:
- Cadena de commits W2.
- Diff summary.
- Verdict ps-code-reviewer.
- Tests: 8/8 verde.
- ⚠ Flag legal R-P1-3: texto actual + recordatorio de validación legal antes de deploy.
- Esperar OK humano.

---

## Zonas congeladas — verificación

Archivos W2 permitidos:
- `frontend/app/page.tsx` ✓
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` + `.module.css` ✓
- `frontend/components/patient/consent/ConsentGatePanel.tsx` + `.module.css` ✓
- `frontend/app/(patient)/dashboard/page.tsx` + CSS module del heading ✓

Ningún import desde `lib/auth/*` en los archivos anteriores. Cookie name se lee de `next/headers` con string literal.
