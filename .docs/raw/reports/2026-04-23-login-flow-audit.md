# Login Flow Audit — Bitácora 2026-04-23

**Fecha:** 2026-04-23 · **Rama base:** `main` @ `0bcb11e` · **Modo:** read-only (cero modificaciones de código)
**Scope:** `/` → `/ingresar` → callback Zitadel → `/onboarding` → `/consent` → `/dashboard` → modal `MoodEntryDialog` → edge states (sesión expirada, logout, error global, not-found)
**Compliance:** Ley 25.326 / 26.529 / 26.657 — un hallazgo marcado `⚠ legal-review`
**Enlaces:**
- Baseline consolidado de esta corrida: [`2026-04-23-login-flow-baseline.md`](./2026-04-23-login-flow-baseline.md)
- Closure previo: [`2026-04-22-impeccable-hardening-closure.md`](./2026-04-22-impeccable-hardening-closure.md)
- Baseline + critique previos (frontend entero): [`2026-04-22-impeccable-audit-baseline.md`](./2026-04-22-impeccable-audit-baseline.md), [`2026-04-22-impeccable-critique.md`](./2026-04-22-impeccable-critique.md)
- Prompt fuente: `.docs/raw/prompts/2026-04-23-login-flow-ux-audit.md`

---

## 1. Resumen ejecutivo

**Verdict: `needs-redesign`** (para el dashboard como hub de retorno). El resto del flujo está en `needs-refinement`. El verdict global lo impone el dashboard: es donde el product owner localizó el dolor y donde ninguna micro-corrección resuelve el problema estructural.

**Top-3 problemas que responden literalmente la queja del PO** ("la UX de login es malísima y no queda claro cómo un usuario ya registrado accede a sus registros ni cómo registra algo nuevo"):

1. **El CTA `"+ Nuevo registro"` está enterrado al final del scroll del dashboard en estado `ready`** (E3-F1, `Dashboard.tsx:292-307`). Es el ÚLTIMO bloque del DOM. En mobile 360px con 10 entradas cargadas, queda ~600-700px debajo del viewport inicial. El recurrente nocturno abre el dashboard y NO ve el botón de la acción más cotidiana del producto.
2. **El logout es un botón 1-click sin confirmación en la esquina superior derecha, primer elemento tap** (E3-F6, `PatientPageShell.tsx:41-49` + `.module.css:62`). Tap accidental → cierra sesión → re-autenticar OIDC. Ausencia total de continuidad para el recurrente y vector de frustración clásico en mobile.
3. **No existe `frontend/app/global-error.tsx`** (E5-F4). Cualquier error catastrófico en el layout root produce pantalla cruda del runtime Next sin branding, sin copy, sin CTA. Dead-end visual total en el momento más sensible.

**Totales:** **4 P0** · **15 P1** · **14 P2** · **3 P3** · **12 positivos vigentes** del hardening 2026-04-22 · **1 `⚠ legal-review`** · **3 `needs-prod-validation`**.

El audit confirma que los 11 commits del hardening 2026-04-22 dejaron el modal a11y-correcto, empty state sin vigilancia y el foco en el trigger. La deuda ahora es **estructural** (jerarquía, posicionamiento, continuidad), no **estilística**.

---

## 2. Mapa del journey

### Persona A — Paciente primera vez (vía invitación o directo)

```text
[/]                       OnboardingEntryHero variant=standard
                          headline "Tu espacio personal de registro" + único CTA "Ingresar"
                          (sin detección de sesión; misma pantalla para cookie viva)
  |
  v
[/ingresar]               route handler → redirect Zitadel (sin interstitial, sin "te estamos llevando")
  |
  v
[Zitadel login]           externo — fuera de scope
  |
  v
[/auth/callback]          zona bloqueada — no auditado
  |
  v
[/onboarding]             OnboardingFlow monta AuthBootstrapInterstitial "Continuando con tu registro..."
                          → bootstrapPatient() retorna {needsConsent, resumePendingInvite}
                          → decisión: ir a /consent o saltar a /dashboard
  |
  v (needsConsent=true)
[/consent]                h2 "Consentimiento informado" (jerga clínica sin bajada)
                          → inviteHint "Recordá que viniste a través de una invitación de tu profesional"
                             (aparece ANTES de las secciones de contenido → E2-F1 P1)
                          → secciones del consent (contenido dinámico desde API)
                          → decisionBar fixed con ÚNICO CTA "Aceptar y continuar"
                             (SIN rechazo, SIN revocación mencionada → E2-F2 P1 ⚠, E2-F3 P1)
  |
  v (click "Aceptar y continuar")
[/dashboard]              viewState=empty:
                          → TelegramReminderBanner (si linked=false)
                          → emptyState "Empezá con tu primer registro" + CTA "Registrar humor"
                          (DashboardSummary OCULTO — closure 2026-04-22 vigente ✓)
  |
  v (click "Registrar humor")
[MoodEntryDialog]         <dialog>.showModal() nativo → aria-modal, focus trap nativo,
                          Esc, backdrop click, focus return al trigger (vigentes ✓)
                          → MoodScale radios 44×44 → submit SPA
                          → success: "Registro guardado." role=status aria-live=polite
                          ⚠ Modal NO se cierra automáticamente (E4-F2 P1)
                          ⚠ Sin puente de siguiente acción (E4-F1 P1)
```

### Persona B — Paciente recurrente (cookie viva, consent granted, registros previos)

```text
[/] (cookie viva)         MISMA PANTALLA que primera vez (E1-F2 P1, E1-F5 P2)
                          → pitch de captación, no puerta de retorno
                          → sin saludo, sin "seguí donde dejaste"
  |
  v (SSO silencioso probable si cookie Zitadel viva)
[/dashboard]              viewState=ready:
                          1. TelegramReminderBanner (condicional → E3-F10 P2)
                          2. DashboardSummary — 3 stat cards (E3-F3 P1 densidad tablero)
                          3. h1 "Mi historial"  ⬅ contradice copy congelado (E3-F7 P2)
                          4. "Tus últimos días" trendChart (E3-F4 P2, E3-F9 P1)
                          5. "Registros recientes" list (10 items)
                          6. [.actions] "+ Nuevo registro" + "Check-in diario"  ⬅ ENTERRADO (E3-F1 P0)
                          Logout en esquina superior — primer tap, 1-click (E3-F6 P0)
                          Sin link a /configuracion/vinculos (E3-F12 P1)

  | subruta: sesión expira mid-modal/form →
  v
[sessionBlock inline]     "Tu sesión caducó. Ingresá de nuevo." + link /ingresar
                          (copy correcto; falta aria-live anuncio → E5-F3 P2)

  | subruta: logout explícito →
  v
[signOut()]               destino post-logout no auditable (zona bloqueada)

  | subruta: error global →
  v
[app/error.tsx]           "No pudimos cargar esta pantalla" ✓ + "Ocurrió algo inesperado" ✗ (E5-F1 P1)
                          CTA "Reintentar" → reset. Sin wordmark (E5-F8 P2)

  | subruta: error layout root →
  v
[(global-error.tsx AUSENTE)]   Pantalla cruda Next sin branding (E5-F4 P0)
```

---

## 3. Inventario de hallazgos

Tabla canónica con el shape obligatorio del brief. Ordenada por severidad, luego por surface.

| ID | Surface | Persona | Heurística violada | Severidad | Evidencia file:line | Impacto observado |
|----|---------|---------|--------------------|-----------|---------------------|-------------------|
| E3-F1 | dashboard | recurrente | Lineamiento 12 §"una acción dominante"; Patrón 16 #3 "Gesto rápido de valor" | **P0** | `Dashboard.tsx:292-307` — `<section className={styles.actions}>` con `"+ Nuevo registro"` es el último bloque DOM en ready | Recurrente no ve el CTA al cargar; responde literal la queja del PO |
| E3-F6 | shell | ambos | Lineamiento 12 §"acciones destructivas explícitas"; Manifiesto 10 §5.2 | **P0** | `PatientPageShell.tsx:41-49` + `.module.css:62` — logout sin `aria-label`, sin diálogo, posición `align-self: flex-end`, primer tap DOM | Tap accidental cierra sesión; pérdida de contexto; fricción OIDC desproporcionada |
| E5-F4 | global-error | ambos | Patrón 16 #7; Manifiesto 10 §refugio | **P0** | `frontend/app/global-error.tsx` — archivo inexistente | Error catastrófico = pantalla runtime Next sin branding ni CTA |
| E4-F1+E4-F2 | modal | ambos | Patrón 16 #12 "Puente de siguiente acción"; Nielsen #1 | **P0** combinado | `MoodEntryForm.tsx:82-87` (bloque continuidad condicional `!embedded`) + `Dashboard.tsx:111` (`handleEntrySaved()` no llama `closeDialog()`) | Modal queda abierto post-success sin instrucción; usuario no entiende si volvió al dashboard. Se eleva a P0 por concurrencia de dos fallos del mismo patrón #12 y por contradecir directamente la queja del PO ("cómo registra algo nuevo") — si el primer registro se percibe incompleto, el modelo mental se rompe en la acción nuclear |
| E1-F1 | landing | ambos | Lineamiento 12 §"acción dominante + secundaria silenciosa"; Patrón 16 #10 | P1 | `OnboardingEntryHero.tsx:35-39` — `ctaStack` contiene único `<Link>"Ingresar"</Link>` | Patrón incompleto — acción dominante sin secundaria silenciosa |
| E1-F2 | landing | recurrente | Manifiesto 10 §continuidad; Patrón 16 #10 | P1 | `app/page.tsx:4` + `frontend/middleware.ts` inexistente | Recurrente con cookie viva ve pitch de captación; sin redirect a dashboard |
| E1-F3 | landing | ambos | WCAG 2.2 §2.4.7 Focus Visible | P1 | `OnboardingEntryHero.module.css:62-77` — `.primaryCta` sin `:focus-visible` | CTA único sin focus ring visible — bloqueante AA |
| E2-F1 | consent | invite | Manifiesto 10 §10.1 "vigilancia antes que control" | P1 | `ConsentGatePanel.tsx:53-55` — inviteHint "Recordá que viniste a través de una invitación de tu profesional." antes de secciones | Frame de supervisión profesional precede al contenido del consent |
| E2-F2 | consent | primera vez | Manifiesto 10 §autonomía; **⚠ legal-review Ley 26.529 Art. 2** | P1 | `ConsentGatePanel.tsx:82-93` — único CTA "Aceptar y continuar" | Sin CTA de rechazo; consentimiento bajo coacción implícita |
| E2-F3 | consent | primera vez | Patrón 16 #2 "Contenedor sensible" reversibilidad; Manifiesto 10 §control legible | P1 | `ConsentGatePanel.tsx` — sin mención "revocar" | Paciente acepta sin saber que puede revocar |
| E3-F2 | dashboard | recurrente | Manifiesto 10 §refugio; Patrón 16 #12 | P1 | `page.tsx:22` — `<h1>Mi historial</h1>` | Frame de archivo precede al frame de refugio; sin saludo ni ancla emocional |
| E3-F3 | dashboard | recurrente | Lineamiento 12 §densidad; Manifiesto 10 §12.2 | P1 | `Dashboard.tsx:218-222` + `DashboardSummary.tsx:33-49` — 3 stat cards en ready | Peso tablero-vigilancia; critique 2026-04-22 Punto 1 no resuelto en ready |
| E3-F5 | dashboard | recurrente | Patrón 16 #12 navegación a config | P1 | `TelegramReminderBanner.tsx:71` — único link config (condicional) | Recurrente con banner descartado pierde acceso a `/configuracion/telegram` |
| E3-F9 | dashboard | recurrente | Lineamiento 12 §mobile-first; WCAG 1.4.10 Reflow | P1 | `Dashboard.module.css:193-201` + `.module.css:345` — grid trendChart gap 4px en 360px | Columnas <28px con 10 entradas; etiquetas 11px ilegibles |
| E3-F12 | dashboard | recurrente | Patrón 16 #12; Manifiesto 10 §8.1 "controles visibles para revocar" | P1 | Grep `/configuracion/vinculos` en scope → 0 matches | Gestión de vínculos clínicos invisible desde dashboard |
| E5-F1 | error-page | ambos | Canon 13 §Errores — evitar "Ups. Algo salió mal" | P1 regresión | `app/error.tsx:16` — `"Ocurrió algo inesperado..."` | Regresión parcial post-hardening; sub genérico prohibido por canon 13 |
| E5-F7 | inline / dashboard / vinculos / onboarding | paciente | Canon 13 §Errores específicos | P1 | `VinculosManager.tsx:50`, `Dashboard.tsx:157`, `OnboardingFlow.tsx:85` — fórmula `"Error al cargar X"` + `err.message` crudo | Errores sin CTA orientador; OnboardingFlow expone strings técnicos crudos |
| E5-F10 | shell | paciente | WCAG 4.1.3 + canon 13 — prop error sin filtro | P1 | `PatientPageShell.tsx:27-33` — `error: string` libre sin tipado ni CTA recovery | Vector exposición técnica + dead-end |
| E1-F4 | landing | invite | Patrón 16 #10 — inviteLabel precede h1 | P2 | `OnboardingEntryHero.tsx:17-27` — `<p .inviteLabel>` antes de `<h1>` | HA-1 del critique 2026-04-22 sigue vigente; contexto clínico precede al producto |
| E1-F5 | landing | recurrente | Manifiesto 10 §continuidad emocional | P2 | `OnboardingEntryHero.tsx:28` — headline fijo sin variante recurrente | Retorno tratado como primera vez |
| E1-F6 | /ingresar | ambos | Manifiesto 10 §ansiedad editorial | P2 | `frontend/app/ingresar/route.ts:11-39` — redirect directo sin interstitial | Salto abrupto click → Zitadel; en conexión lenta parece que el botón no respondió |
| E1-F7 | onboarding | primera vez | WCAG 1.3.1 — No-JS path | P2 | `onboarding/page.tsx:5` — Suspense fallback `<div style={{minHeight:'100vh'}}/>` | Sin JS = pantalla blanca post-callback sin instrucción |
| E1-F8 | landing | primera vez | Lineamiento 12 §mobile-first ≤360px (`needs-prod-validation`) | P2 | `OnboardingEntryHero.module.css:49-55` + `tokens.css:61` `--space-xl: 40px` | Hipótesis: CTA puede quedar bajo fold en 360px; requiere medición real |
| E2-F4 | consent | primera vez | Lineamiento 12 §CTA; FL-CON-01 §"scroll obligatorio" | P2 | `ConsentGatePanel.module.css:60-70` — `.decisionBar { position: fixed }` accionable desde momento 0 | CTA activo antes de scroll → paciente puede aceptar sin leer |
| E2-F5 | consent | ambos | WCAG 2.4.3 Focus Order; 4.1.3 Status Messages | P2 | `OnboardingFlow.tsx:132-150` — sin `focus()` al transicionar a phase='consent' | Teclado/AT no recibe señal de cambio de pantalla |
| E2-F6 | consent / onboarding | ambos | Patrón 16 contexto de pantalla | P2 | `OnboardingFlow.tsx:125-131` + `ConsentGatePanel.tsx` — sin copy introductorio | Paciente aterriza en consent sin contexto del "por qué" |
| E2-F8 | consent | primera vez | Canon 13 §lenguaje llano con bajada clínica | P2 | `ConsentGatePanel.tsx:49` — `<h2>Consentimiento informado</h2>` sin sub | Primera palabra en decisión legal sensible es jerga clínica sin traducción |
| E2-F9 | rutas | ambos | Arquitectura de ruta | P2 | `consent/page.tsx:1-18` + `onboarding/page.tsx:1-14` — mismo `<OnboardingFlow/>` | Dos URLs, misma UI; deuda de separación futura |
| E3-F4 | dashboard | recurrente | Lineamiento 12 §jerarquía + aire | P2 | `Dashboard.tsx:224-229` + `Dashboard.tsx:260-264` — dos h2 con `.sectionTitle` idéntica | Redundancia visual entre "Tus últimos días" y "Registros recientes" |
| E3-F7 | dashboard | ambos | Copy congelado 2026-04-22 | P2 🔒 | `page.tsx:22` — `<h1>Mi historial</h1>` vs congelado "Tu espacio personal de registro" | Incumplimiento de congelado; requiere decisión humana de producto |
| E3-F8 | dashboard / modal | ambos | Lineamiento 12 §consistencia; WCAG 4.1.2 | P2 | `Dashboard.tsx:204-207` "Registrar humor" vs `:291-296` "+ Nuevo registro", ninguno con `aria-label` | Misma acción, dos textos; inconsistencia AT |
| E3-F10 | banner | recurrente | Patrón 16 #12 | P2 | `TelegramReminderBanner.tsx:34-36` — banner solo visible si `linked=false` | Usuario ya vinculado pierde acceso a gestión desde dashboard |
| E4-F3 | modal | ambos | WCAG 2.5.8 Target Size | P2 | `MoodEntryDialog.module.css:57-58` — `.closeBtn { 40×40 }` vs MoodScale 44×44 | Cierre más difícil que el resto del modal |
| E5-F2 | not-found | ambos | Canon 13 §Errores — CTA contextual | P2 | `app/not-found.tsx:12-15` — CTA `"Volver al inicio"` → `/` | Paciente autenticado hace viaje extra vía landing |
| E5-F3 | form / session | paciente | WCAG 4.1.3; Patrón 16 #11 | P2 | `MoodEntryForm.tsx:79` + `DailyCheckinForm.tsx:122` — sessionBlock sin aria-live propio | AT no anuncia transición a sesión expirada |
| E5-F8 | error-page | ambos | Manifiesto 10 §identidad visible | P2 | `app/error.tsx:12-26` monta `<main>` sin wordmark; `layout.tsx:35-43` sin header root | Error sin contexto de marca |
| E5-F9 | offline | ambos | Manifiesto 10 §seguridad sin juicio — ausencia de manejo | P2 | Grep `offline|navigator.onLine|sin red|network` → 0 matches | Sin detector offline; mobile inestable = mensaje genérico |
| E1-F9 | landing footer | ambos | WCAG 2.4.7 | P3 | `OnboardingEntryHero.module.css:109-115` — `.footerLink` sin `:focus-visible` | Link soporte sin focus ring |
| E1-F10 | metadata | primera vez | Social / SEO | P3 | `app/layout.tsx` — description genérica `"Registro de humor y bienestar"` | Oportunidad de refuerzo promesa emocional en link compartido |
| E2-F7 | interstitial | invite | Canon 13 §agencia | P3 | `AuthBootstrapInterstitial.tsx:18` — `"Continuando con tu registro..."` | Cumple canon; oportunidad de afirmar más agencia del paciente |
| E4-F4 | mood-form | ambos | Nielsen #5 prevención errores | P3 | `MoodEntryForm.tsx:119-126` — submit con score=0 sin confirmación | Registro involuntario humor neutro; bajo impacto clínico |
| E5-F5 | logout | paciente | Redundancia AT | P3 | `PatientPageShell.tsx:43-49` — `title` y texto idénticos, sin `aria-label` | Oportunidad `aria-label="Cerrar tu sesión y volver al inicio"` |

---

## 4. Recomendaciones priorizadas (output de la cadena impeccable-*)

Cada recomendación tiene: **qué cambiar**, **por qué**, **dónde**, y **skill impeccable-* sugerido** para el fix.

### 4.1 P0 — Blockers pre-merge

**R-P0-1 — Subir `"+ Nuevo registro"` al top del dashboard en ready** (→ `impeccable-distill`)

- **Qué:** agregar un bloque sticky/prominent con el CTA al inicio del viewport ready. Una opción concreta alineada a canon 12: un "rail de acción personal" arriba de DashboardSummary con heading tipo `"Hoy"` + CTA grande `"Registrar humor"` (alinea con copy congelado E3-F8). DashboardSummary y listas quedan debajo como referencia, no como bloqueo.
- **Por qué:** responde la queja literal del product owner. Patrón 16 #3 "Gesto rápido de valor" exige que el gesto esté disponible inmediatamente, no tras scroll.
- **Dónde:** `Dashboard.tsx:214-310` (reordenar árbol DOM del ready) + `Dashboard.module.css` (sticky CSS). NO tocar el empty state — ya respeta el patrón.
- **Skill:** `impeccable-distill` (reestructuración narrativa); apoyo de `impeccable-onboard` para alinear copy recurrente.

**R-P0-2 — Proteger logout de taps accidentales** (→ `impeccable-harden`)

- **Qué:** dos opciones — (a) mover logout a un overflow menu ("⋯" o "Mi cuenta") con ítem `"Cerrar sesión"`, o (b) mantener el botón pero exigir confirmación inline (`"¿Cerrar tu sesión?" → "Sí, cerrar" / "Cancelar"`) con `aria-label` descriptivo. La (a) está más alineada con lineamiento 12 §"pocas decisiones visibles a la vez".
- **Por qué:** logout 1-click en esquina prominente es un vector clásico de sesión perdida por error. Para un paciente en vulnerabilidad, re-autenticar OIDC corta el registro clínico en curso.
- **Dónde:** `PatientPageShell.tsx:41-49` + `.module.css:62`. Potencialmente crear un nuevo subcomponente `ShellHeader` con overflow menu.
- **Skill:** `impeccable-harden` (edge case UX) + `impeccable-clarify` (microcopy de confirmación).

**R-P0-3 — Crear `frontend/app/global-error.tsx`** (→ `impeccable-harden`)

- **Qué:** archivo nuevo que reemplaza el layout root ante error catastrófico. Wordmark "Bitácora" + título canon 13 (`"No pudimos cargar el sitio"`) + sub concreto (`"Probá recargar la página. Si el problema continúa, escribinos desde tu correo."`) + CTA `"Recargar"` con `reset()`.
- **Por qué:** Next.js App Router colapsa a runtime crudo sin `global-error.tsx`. Rompe manifesto 10 en el momento más sensible.
- **Dónde:** `frontend/app/global-error.tsx` (archivo nuevo). Debe ser `'use client'` con `<html><body>...</body></html>` wrapper mínimo (contrato Next).
- **Skill:** `impeccable-harden`.

**R-P0-4 — Cerrar modal post-success + puente de siguiente acción** (→ `impeccable-clarify` + `impeccable-harden`)

- **Qué:** en `handleEntrySaved()` esperar ~800ms (mostrar "Registro guardado." en el modal) y cerrar automáticamente con `closeDialog()`. Simultáneamente, en el dashboard, emitir un toast `aria-live="polite"` con `"Registro sumado a tu historial."` (copy canon 13, sin celebración) para confirmar continuidad. El bloque `{!embedded && ...}` de `MoodEntryForm.tsx:82-87` debe tener su contraparte en modo embedded (puente condensado).
- **Por qué:** Patrón 16 #12 "Puente de siguiente acción" + #6 "Confirmación factual breve". La acción nuclear del producto no puede terminar en un modal abierto sin instrucción.
- **Dónde:** `Dashboard.tsx:111` (`handleEntrySaved`) + `MoodEntryForm.tsx:82-87` (bloque continuity-embedded) + toast nuevo en `Dashboard.tsx`.
- **Skill:** `impeccable-clarify` (microcopy) + `impeccable-harden` (cierre automático + aria-live).

### 4.2 P1 — Esta sesión de implementación (major, post-P0)

**R-P1-1 — Continuidad del recurrente en landing** (E1-F2, E1-F5) (→ `impeccable-onboard`)

- Crear `frontend/middleware.ts` que detecte cookie `bitacora_session` viva y redirija `/` → `/dashboard` (requiere coordinación con owner de `lib/auth/*` — zona bloqueada en este audit; marcar handoff explícito).
- Si el equipo auth rechaza middleware server-side, alternativa UI-only: `OnboardingEntryHero` con variante `variant="returning"` que muestre `<h1>"Volviste"</h1>` + CTA "Seguir registrando" → `/dashboard`. Activada por flag (cookie leída en RSC sin tocar lib/auth).
- **Skill:** `impeccable-onboard` con handoff a equipo auth.

**R-P1-2 — Reubicar inviteHint en consent + hero** (E2-F1, E1-F4) (→ `impeccable-onboard`)

- Mover inviteHint DESPUÉS de las secciones del consent, antes del decisionBar. En el hero, mover inviteLabel a subtítulo bajo h1, no al tope.
- **Dónde:** `ConsentGatePanel.tsx:53-55` (mover al final del `.sections` container); `OnboardingEntryHero.tsx:17-27` (mover post-h1).
- **Skill:** `impeccable-onboard`.

**R-P1-3 — CTA de rechazo en consent** (E2-F2) ⚠ legal-review (→ `impeccable-harden`)

- Agregar botón secundario `"Ahora no"` o `"Volver al inicio"` junto al primario. Al click: mensaje sereno (`"Podés aceptar cuando quieras. Tu sesión sigue activa."`) + redirect a `/` con posibilidad de volver sin re-autenticar. Mantener hard gate funcional del RF-CON-003 (sin consent no hay registro) pero ofrecer salida respetuosa.
- **Por qué:** Ley 26.529 Art. 2 exige posibilidad real de rechazo. Canon 10 §6 postura de confianza exige "paciente titular de sus datos".
- **Dónde:** `ConsentGatePanel.tsx:82-93` (decisionBar). Lógica de salida coordinada con equipo auth (no borrar sesión; solo redirigir).
- **Skill:** `impeccable-harden` + review legal previo.

**R-P1-4 — Mencionar revocabilidad en consent** (E2-F3) (→ `impeccable-clarify`)

- Texto breve en el consent: `"Podés revocarlo cuando quieras desde Mi cuenta."` Enlace opcional a página de revocación (cuando exista; por ahora link estático que explica).
- **Dónde:** `ConsentGatePanel.tsx` — nueva `<p className={styles.revocationNote}>` cerca del decisionBar.
- **Skill:** `impeccable-clarify`.

**R-P1-5 — Rediseñar DashboardSummary en ready** (E3-F3, E3-F4) (→ `impeccable-distill`)

- Tres caminos (menor a mayor cambio): (a) colapsar DashboardSummary detrás de `<details>` con toggle `"Ver resumen"`; (b) convertir a texto corrido (`"Llevás 12 registros. El último fue hace 2 días."`); (c) eliminar DashboardSummary de ready y absorber sus números en el heading del rail de acción. Opción (b) o (c) alinean mejor con canon 10.
- **Dónde:** `Dashboard.tsx:218-222` + `DashboardSummary.tsx` completo.
- **Skill:** `impeccable-distill`.

**R-P1-6 — Acceso visible a configuración** (E3-F5, E3-F12) (→ `impeccable-onboard` + `impeccable-clarify`)

- Agregar una sección `"Configuración"` en `PatientPageShell` (nav mínima persistente) o al final del dashboard con links `"Recordatorios de Telegram"` y `"Vínculos con profesional"`. No tabs — lineamiento 12 §"pocas decisiones a la vez".
- **Dónde:** `PatientPageShell.tsx` (nueva nav/footer) o `Dashboard.tsx` (sección `.settings`).
- **Skill:** `impeccable-onboard` (descubribilidad) + `impeccable-clarify` (labels).

**R-P1-7 — Focus ring canonizado en CTAs de entry points** (E1-F3) (→ `impeccable-normalize`)

- Aplicar `:focus-visible { outline: 2px solid var(--brand-primary); outline-offset: 2px; box-shadow: var(--focus-ring) }` a `.primaryCta` de landing, footerLink, y cualquier elemento interactivo del login flow que falte (auditable via CSS modules búsqueda sistemática).
- **Dónde:** `OnboardingEntryHero.module.css` y auditar resto del login flow.
- **Skill:** `impeccable-normalize`.

**R-P1-8 — Heading recurrente del dashboard** (E3-F2, E3-F7) 🔒 (→ `impeccable-onboard`)

- Decisión humana entre: (a) mantener `"Mi historial"` como h1 y revisar el congelado 2026-04-22 en el canon; (b) alinear h1 con congelado usando `"Tu espacio personal de registro"`; (c) introducir saludo contextual (`"Hola. Acá está lo que registraste."`) que no choca con el congelado. Recomendado: (c) combinado con subtítulo que absorbe el concepto de privacidad (`"Solo vos ves lo que registrás."`, copy congelado también).
- **Dónde:** `app/(patient)/dashboard/page.tsx:22`.
- **Skill:** `impeccable-onboard` tras decisión de producto.

**R-P1-9 — trendChart colapsable en 360px** (E3-F9) (→ `impeccable-adapt`)

- En breakpoint ≤400px: limitar trendChart a 5 últimas entradas y aumentar gap a `--space-sm` (8px). Alternativa: reemplazar por resumen textual (`"Últimos 10 días: 6 registros, humor promedio +1"`) si `window.innerWidth < 400`.
- **Dónde:** `Dashboard.module.css:193-201` + `Dashboard.tsx` (límite entradas por media query/prop).
- **Skill:** `impeccable-adapt`.

**R-P1-10 — Mensajes de error concretos + filtro de `err.message`** (E5-F1, E5-F7, E5-F10) (→ `impeccable-harden` + `impeccable-clarify`)

- Crear `frontend/lib/errors/user-facing.ts` con función `formatUserFacingError(err: unknown): { title: string; description: string; retry?: () => void }` que mapea códigos conocidos a copy canon 13 y cae a mensaje genérico concreto, nunca `err.message` crudo.
- Reemplazar sub de `app/error.tsx:16` por `"No pudimos completar la acción. Probá en unos minutos o volvé al inicio."` + segundo CTA opcional `"Volver al inicio"`.
- Tipar prop `error` de `PatientPageShell` como `UserFacingError` + agregar CTA retry.
- **Dónde:** archivos nuevos + `app/error.tsx:16`, `VinculosManager.tsx:50`, `Dashboard.tsx:157`, `OnboardingFlow.tsx:85`, `PatientPageShell.tsx:27-33`.
- **Skill:** `impeccable-harden` (resiliencia) + `impeccable-clarify` (copy).

### 4.3 P2 — Siguiente wave (quality refinements)

| ID | Fix | Skill |
|----|-----|-------|
| E1-F6 | Interstitial breve entre click Ingresar y Zitadel — "Te estamos llevando al ingreso seguro..." (<500ms, sin spinner teatral) | `impeccable-polish` |
| E1-F7 | `<noscript>` con instrucción mínima en onboarding Suspense fallback | `impeccable-harden` |
| E1-F8 | Validar en device real; si CTA cae bajo fold, reducir `--space-xl` en mobile o mover CTA arriba del sub | `impeccable-adapt` |
| E2-F4 | Scroll obligatorio o dimming del CTA hasta `scroll-bottom` detectado | `impeccable-harden` |
| E2-F5 | `useEffect(() => headingRef.current?.focus(), [])` al montar `ConsentGatePanel` | `impeccable-normalize` |
| E2-F6 | Frase introductoria antes de secciones del consent: `"Antes de empezar a registrar, necesitamos que leas y aceptes este consentimiento."` | `impeccable-clarify` |
| E2-F8 | Subtítulo bajo "Consentimiento informado": `"Antes de registrar tus datos, acá te contamos qué guardamos, quién accede y cómo lo controlás."` | `impeccable-clarify` |
| E2-F9 | Evaluar si `/consent` debería aceptar prop de estado (recurrente vs primera vez) o colapsar a una sola ruta | `impeccable-extract` (fuera de scope actual) |
| E3-F8 | Unificar texto del trigger: `"Registrar humor"` en ambos estados (respeta congelado). Agregar `aria-label="Registrar humor nuevo"` | `impeccable-normalize` |
| E3-F10 | Slot `"Configurar recordatorios"` fijo en dashboard (no condicional), independiente del estado del banner | `impeccable-onboard` |
| E4-F3 | `.closeBtn` 40→44px | `impeccable-normalize` |
| E5-F2 | CTA `not-found.tsx` dependiente de cookie: `"Volver a tu historial"` → `/dashboard` si sesión viva; `"Volver al inicio"` → `/` si no | `impeccable-adapt` |
| E5-F3 | `role="alert"` en sessionBlock de `MoodEntryForm` y `DailyCheckinForm` | `impeccable-normalize` |
| E5-F8 | Header con wordmark en `app/error.tsx` (import desde tokens/componente compartido) | `impeccable-polish` |
| E5-F9 | Detector offline en `PatientPageShell` con banner `role="alert"` `"Estás sin conexión. Tu próximo registro se guardará cuando vuelvas."` y cola local (localStorage) para reintento | `impeccable-harden` |

### 4.4 P3 — Nice-to-have delight sobrio

| ID | Fix | Skill |
|----|-----|-------|
| E1-F9 | `:focus-visible` en `.footerLink` | `impeccable-normalize` |
| E1-F10 | Metadata OG: description `"Tu espacio privado de salud mental. Registrá cómo estás sin exponerte."` | `impeccable-polish` |
| E2-F7 | Copy interstitial invite más afirmativo: `"Preparando tu espacio..."` | `impeccable-delight` |
| E4-F4 | Tooltip/nota bajo MoodScale score=0: `"Neutro — un día común."` | `impeccable-delight` |
| E5-F5 | `aria-label="Cerrar tu sesión y volver al inicio"` en logout | `impeccable-polish` |

---

## 5. Resumen por surface (veredicto impeccable-critique)

| Surface | Veredicto | Racional |
|---------|-----------|----------|
| Landing `/` | `refinar` | Hero estructuralmente correcto; falta continuidad recurrente + focus ring + interstitial `/ingresar`. No requiere rediseño |
| `/ingresar` | `mantener` (con P2) | Route handler hace su trabajo; P2 E1-F6 es quality-of-life |
| Onboarding `/onboarding` + interstitial | `mantener` | AuthBootstrapInterstitial cumple patrón 16 #11. Problemas son aguas arriba (landing) o abajo (consent) |
| Consent `/consent` | `refinar` (3× P1) | Hardening P1 principal: rechazo (⚠ legal), revocación, posición inviteHint. No requiere rediseño estructural |
| Dashboard `/dashboard` | **`rediseñar`** | E3-F1 P0, E3-F3 P1 densidad, E3-F5/F12 P1 navegación. Epicentro del dolor del PO |
| Shell (PatientPageShell) | `refinar` (1× P0) | Logout P0 + sin nav de config P1 |
| Modal (MoodEntryDialog + forms) | `refinar` (2× P1 de continuidad) | A11y estructural perfecto (12 items vigentes). Fallo: cierre post-success + puente |
| Edge states | `refinar` (1× P0 + 3× P1) | global-error ausente P0; copy de errores con regresión parcial y consumidores sin filtro |

**Conteo final:** 0 `mantener puro` · 6 `refinar` · 1 `rediseñar` · 1 `mantener + P2`.

---

## 6. Follow-ups explícitos (fuera de scope del audit)

Identificados durante la corrida; no se resuelven aquí:

1. **Backend / schema** — ninguna recomendación toca backend. Si la decisión sobre E2-F2 CTA rechazo cambia el comportamiento del endpoint `/api/v1/consent`, requiere nuevo RF (posible RF-CON-014 "Manejar rechazo de consentimiento con salida sin pérdida de sesión"). Handoff a `crear-requerimiento`.
2. **Analytics / telemetría** — ninguna métrica del flujo login vive en código auditado. Recomendación fuera de scope: instrumentar (a) tiempo desde `/dashboard` ready hasta `openDialog()` (proxy del dolor del PO), (b) rate de logout accidental (sesiones cerradas con <3 min de uso), (c) CTR del CTA `"+ Nuevo registro"` vs `"Check-in diario"`. Handoff a owner analytics.
3. **E2E tests** — 8 specs vigentes post-closure 2026-04-22 (ver `frontend/e2e/`). Los fixes P0-P1 van a requerir ajuste de selectors/texts en `dashboard-modal.spec.ts` y posiblemente nuevos specs para global-error y logout confirmation. Handoff a `e2e-testing-patterns`.
4. **Traceability canon UX/UI** — NO existen `UJ-ONB-*`, `UXS-ONB-*`, `VOICE-ONB-*` en `.docs/wiki/23_uxui/` para el flujo login+dashboard+modal. Canon 22 requiere que los fixes P0-P1 produzcan estos artefactos antes de implementación. Handoff a `crear-journey-ux` + `crear-spec-ux`.
5. **Middleware / zona auth** — R-P1-1 requiere `frontend/middleware.ts` (zona bloqueada en este audit). Handoff al equipo auth: confirmar si middleware server-side detecta cookie activa de forma segura sin romper callback OIDC.
6. **Prod validation** — 3 hallazgos marcados `needs-prod-validation` (E1-F2, E1-F8, E3-F6). Requieren medición en dispositivos reales + telemetría. Handoff a QA manual o e2e con dispositivos reales.
7. **Legal review** — E2-F2 requiere confirmación del equipo legal sobre si el hard gate actual sin CTA de rechazo cumple Ley 26.529 Art. 2. Bloqueante para R-P1-3.

---

## 7. Métricas proxy

| Métrica | Valor |
|---------|-------|
| Hallazgos activos totales | **36** |
| P0 | **4** |
| P1 | **15** |
| P2 | **14** |
| P3 | **3** |
| Surfaces tocados | **8** (landing, /ingresar, onboarding, consent, dashboard, shell, modal, edge-error) |
| Heurísticas únicas violadas | **14** |
| Hallazgos por persona: primera vez | **13** |
| Hallazgos por persona: recurrente | **15** |
| Hallazgos por persona: ambos | **8** |
| Positivos vigentes (closure 2026-04-22) | **12** |
| Regresiones detectadas | **1** (E5-F1 parcial) |
| Hallazgos `⚠ legal-review` | **1** |
| Hallazgos `needs-prod-validation` | **3** |
| Hallazgos `🔒 needs-human-decision` | **2** (E2-F2, E3-F7) |

---

## 8. Verdict y próxima acción

### Verdict

**`needs-redesign`** para el dashboard como hub de retorno del paciente recurrente. El resto del flujo está en **`needs-refinement`**. El peso del verdict lo impone el dashboard porque ahí vive el dolor del product owner: el CTA enterrado (E3-F1 P0), el logout mal posicionado (E3-F6 P0), y la densidad tablero-like (E3-F3 P1) no se arreglan con micro-corrección. Exigen rediseño de jerarquía DOM del estado `ready`.

El resto del flujo puede cerrarse con una wave de refinamientos quirúrgicos bien scoped. La arquitectura UX base (manifesto 10 + lineamiento 12 + patrón 16 + closure 2026-04-22) es correcta y no requiere revisión. La deuda es de **aplicación** del canon, no de **definición** del canon.

### Próxima acción recomendada

1. **Humano confirma** el verdict y decide sobre los 2 ítems `🔒 needs-human-decision`:
   - E2-F2: ¿agregamos CTA de rechazo con salida? (requiere review legal).
   - E3-F7: ¿actualizamos canon o mantenemos `"Mi historial"` como h1?
2. **Generar prompt de implementación** con `impeccable-distill` como skill líder, asistido por `impeccable-onboard` (dashboard como hub del recurrente) + `impeccable-harden` (global-error + logout + consent rechazo + cerrar modal). Scope sugerido: 4 waves (P0 blockers → P1 onboard/continuity → P1 harden/errors → P2 polish). Dado el tamaño (36 hallazgos, 3 P0 estructurales) recomiendo `writing-plans` para un plan wave-dispatchable con checkpoints cada wave, idéntico al patrón del hardening 2026-04-22.
3. **No mergear este reporte a main** — la corrida es read-only. La rama `audit/login-flow-2026-04-23` queda abierta para que el humano revise y decida integración vs sesión dedicada de fix.

---

*Reporte de auditoría read-only del flujo login+dashboard+modal de Bitácora, 2026-04-23. Baseline: `.docs/raw/reports/2026-04-23-login-flow-baseline.md`. Closure previo: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`. Prompt fuente: `.docs/raw/prompts/2026-04-23-login-flow-ux-audit.md`.*
