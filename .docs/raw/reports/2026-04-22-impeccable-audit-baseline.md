# Impeccable Audit Baseline — Frontend Bitácora

**Fecha:** 2026-04-22
**Rama base:** `main` @ `1c1ac50`
**Scope:** `frontend/` (UI-only, sin tocar auth/contratos/dominio)
**Método:** 3 `ps-explorer` en paralelo cross-checked + verificación de `tokens.css` + revisión de canon (`10-16`, `23_uxui`, `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`)
**Compliance:** Ley 25.326 / 26.529 / 26.657 — cambios UI-only no alteran storage, access control, consent flows ni audit logging.

## Cómo leer este documento

- **Dimensiones impeccable** cubiertas: accessibility (a11y), performance, theming (tokens + DS), responsive, microcopy (voz/tono), visual-quality, state-coverage.
- **Severidad:** `bloqueante` (regla 9.1 ortografía o WCAG AA claro) > `alto` > `medio` > `bajo`.
- **Confianza:** `alta` cuando ≥2 explorers coinciden; `media` cuando 1 explorer reporta con `file:line`.
- **Fuera de scope:** `frontend/proxy.ts`, `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`.
- **Copy congelado** (NO modificar): "Ingresar", "Tu espacio personal de registro", "Empezá con tu primer registro", "Registrar humor", "Nuevo registro", "+ Nuevo registro", "Check-in diario", "Recibí recordatorios por Telegram", "Conectar", "Ahora no", "Registro guardado.", "Check-in guardado.".

---

## 1. Microcopy y voz (regla 9.1 + canon 13_voz_tono)

### 1.1 Ortografía sistemática — tildes faltantes (bloqueante)

La regla 9.1 (`CLAUDE.md`) es obligatoria: tildes y signos inversos no negociables. Violaciones cross-checked por Explorer A y B:

| Archivo:línea | Texto actual | Corrección |
|---|---|---|
| `frontend/app/layout.tsx:15` | `title: "Bitacora"` | `"Bitácora"` |
| `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:13` | `Bitacora` (wordmark) | `Bitácora` |
| `frontend/components/ui/ProfessionalShell.tsx:44` | `Bitacora Pro` | `Bitácora Pro` |
| `frontend/components/patient/consent/ConsentGatePanel.tsx:49` | `"Recorda que viniste a traves de una invitacion de tu profesional."` | `"Recordá que viniste a través de una invitación de tu profesional."` |
| `frontend/components/patient/consent/ConsentGatePanel.tsx:55` | `Version` | `Versión` |
| `frontend/app/(profesional)/.../invitaciones/page.tsx:12` | `"Ingresa el correo electronico..."` | `"Ingresá el correo electrónico..."` |
| `frontend/components/professional/InviteForm.tsx:35-111,95` | `Invitacion`, `invitacion`, `Se envio`, `aparecera`, `ID de invitacion`, `vinculo`, `ya esta`, `electronico`, `No se pudo enviar la invitacion` | todas con tilde |
| `frontend/components/professional/PatientList.tsx:61,63,73,80,144,145` | `Paginacion`, `Pagina N de M`, `Pagina anterior/siguiente`, `No tenes pacientes vinculados todavia`, `Invita a alguien desde la seccion de invitaciones` | `Paginación`, `Página`, `Página anterior/siguiente`, `No tenés pacientes vinculados todavía`, `Invitá a alguien desde la sección de invitaciones` |
| `frontend/components/professional/PatientDetail.tsx:65` | `Ultimo registro` | `Último registro` |
| `frontend/components/professional/PatientSummaryCard.tsx:32` | `Ultimo registro` | `Último registro` |
| `frontend/components/professional/ExportGate.tsx:271,287` | `exportacion`, `No tenes permisos` | `exportación`, `No tenés permisos` |

### 1.2 Tuteo en capa profesional (bloqueante)

Canon 13 exige voseo rioplatense sobrio. La superficie profesional no tiene excepción documentada.

| Archivo:línea | Actual | Corrección |
|---|---|---|
| `frontend/app/(profesional)/.../invitaciones/page.tsx:12` | `Ingresa` | `Ingresá` |
| `frontend/components/professional/PatientList.tsx:145` | `Invita` | `Invitá` |

### 1.3 Copy vago en errores (medio, canon 13 §"Errores")

Canon 13 exige errores concretos, no genéricos. Fallbacks identificados:

- `frontend/components/patient/mood/MoodEntryForm.tsx:57` → `"Ocurrió un error. Intentá de nuevo."` → sugerir `"No pudimos guardar el registro. Probá de nuevo."`
- `frontend/components/patient/checkin/DailyCheckinForm.tsx:96` → mismo fallback genérico.
- `frontend/app/error.tsx:14-17` → `"Algo salió mal" / "Ocurrió un error inesperado. Intentá de nuevo."` → reemplazar por texto concreto (`"No pudimos completar la acción..."`).

### 1.4 Scoring/dashboard language (medio)

Canon 13 prefiere `estado de ánimo` sobre `puntaje`:

- `Dashboard.tsx:287` → `aria-label="Puntaje de humor"` → `aria-label="Estado de ánimo"`.
- `Dashboard.tsx:288` → `"Sin puntaje"` → `"Sin registro"` o `"Sin dato"`.
- `DashboardSummary.tsx:44` → `"Promedio de humor"` → aceptable si se enmarca como "Tendencia" / "Resumen"; evaluar.

### 1.5 Rozaduras menores (bajo)

- `OnboardingEntryHero.tsx:30-32` → repetición de "tranquilidad" dos veces en la misma frase.
- `TelegramReminderBanner.tsx:60` → `"Tarda un minuto y te ayuda a no olvidar tu registro."` roza el push paternalista; evaluar refraseo neutral (ej. `"Tarda un minuto. El recordatorio aparece en Telegram."`).
- `BindingCodeForm.tsx:90` → `"Invitación aceptada ✓"` — el check es decoración celebratoria; canon 13 exige confirmación sin celebración. Eliminar `✓`.
- `ConsentGatePanel.tsx:49` → hint de invitación introduce presencia profesional antes del control del paciente (canon 10 §10.1 "si una pantalla pone primero 'Tu profesional verá tus registros' va contra el manifiesto"). Mantener el hint es aceptable por contexto de onboarding, pero su tono debe revisarse junto al `impeccable-clarify`.

---

## 2. Accessibility (WCAG 2.1 AA)

### 2.1 ARIA y semántica (alto)

| # | Archivo:línea | Problema | Fix |
|---|---|---|---|
| A1 | `components/ui/InlineFeedback.tsx:19` | `role="alert"` combinado con `aria-live="polite"` — role implica assertive | `aria-live` condicional por variant, o eliminar uno de los dos |
| A2 | `components/patient/consent/ConsentGatePanel.tsx:58-61` | `<section role="listitem">` — combinación inválida, `<section>` es landmark | cambiar a `<div role="listitem">` o a `<ul>/<li>` |
| A3 | `components/patient/checkin/DailyCheckinForm.tsx:178-213` | botones "Sí"/"No" sin `aria-pressed` — estado invisible para AT | `aria-pressed={form[key] === true/false}` |
| A4 | `components/ui/PatientPageShell.tsx:41-48` | logout button sin `type="button"` → peligro submit si se anida en form | agregar `type="button"` |
| A5 | `components/patient/mood/MoodEntryForm.tsx:100` + `MoodEntryDialog.tsx:55` | `h1` dentro del `<dialog>` que ya tiene `h2` — jerarquía invertida cuando `embedded` | en variante embedded cambiar `<h1>` por `<p>` con `role="heading" aria-level="3"` |
| A6 | `components/patient/dashboard/Dashboard.tsx:287-290` | `aria-label="Puntaje de humor"` genérico — no diferencia entre entradas al listar | enriquecer con fecha: `aria-label={`Estado de ánimo: ${formatMoodScore(v)}, ${dateStr}`}` |

### 2.2 Foco y teclado (alto)

| # | Archivo:línea | Problema | Fix |
|---|---|---|---|
| F1 | `styles/globals.css:23-26` vs módulos (6 archivos) | globals define `outline: 2px solid`; CSS Modules sobreescriben con `outline: none; box-shadow: --focus-ring` — en Windows High Contrast Mode el box-shadow es ignorado → foco invisible | mantener `outline: 2px solid var(--brand-primary); outline-offset: 2px` y sumar `box-shadow` como complemento |
| F2 | `components/patient/checkin/DailyCheckinForm.module.css:56-59,112-115` | inputs con `:focus` sin `:focus-visible` — foco permanente en mouse-users | eliminar `:focus` genérico, dejar solo `:focus-visible` con outline real |
| F3 | `components/patient/dashboard/Dashboard.tsx:113-119` + `MoodEntryDialog.tsx` | el modal no restaura foco al trigger al cerrar (si se abrió por click sin foco previo el foco queda en `<body>`) | `triggerRef` en el botón `+ Nuevo registro`, llamar `triggerRef.current?.focus()` en `closeDialog()` |
| F4 | `components/patient/dashboard/MoodEntryDialog.tsx:46-52` | falta `aria-modal="true"` explícito (compatibilidad NVDA antiguas) | agregar atributo |

### 2.3 Contraste (alto)

| # | Origen | Combinación sospechosa | Fix |
|---|---|---|---|
| C1 | `styles/tokens.css:14-15` | `--foreground-muted #655E59` sobre `--surface-muted #E8DED3` — ratio estimado ~3.5:1 < 4.5 | oscurecer `--foreground-muted` a ~`#4A4440` (verificar con herramienta de contraste; mantener coherencia con identidad cálida) |
| C2 | `components/patient/consent/ConsentGatePanel.module.css:5-9` | `--brand-accent #B67864` como color de texto en `.inviteHint` sobre `--surface` — ratio ~2.8:1 | usar `--foreground-muted` (o lo que resulte tras C1) para el hint; reservar `--brand-accent` para énfasis no-texto |

### 2.4 Use of color (alto)

- `components/patient/checkin/DailyCheckinForm.module.css:31-33` → estado de error en bloque comunicado solo por `border-left-color`. Complementar con ícono o texto "⚠ Campo con error" cerca del heading del bloque.

### 2.5 Touch targets (medio)

| # | Archivo:línea | Problema | Fix |
|---|---|---|---|
| T1 | `ConsentGatePanel.module.css:71-83` | `.acceptBtn` sin `min-height` → ~40px < 44px (CTA legal crítico) | `min-height: 44px` |
| T2 | `ConsentGatePanel.module.css:90-101` | `.retryBtn` ~32px | `min-height: 44px` |
| T3 | `TelegramReminderBanner.module.css:45` | `min-height: 40px` en primary/secondary | subir a 44px |

### 2.6 Status messages y dismiss (medio)

- `TelegramReminderBanner.tsx:66` → `"Ahora no"` no comunica duración del dismiss (30 días). Agregar `aria-label="Descartar recordatorio por 30 días"` sin cambiar el texto visible (que es copy congelado).
- `TelegramReminderBanner.tsx:47-52` → al descartar, el banner desaparece del DOM sin anuncio ARIA. Emitir región `aria-live="polite"` con "Recordatorio descartado" antes de ocultar.
- `InviteForm.tsx:125-130` → submit sin `aria-busy` durante submitting (inconsistente con el resto del codebase).

### 2.7 Formularios (medio)

- `DailyCheckinForm.tsx:150-165` → input `sleep_hours` sin `required`/`aria-required`.

---

## 3. Performance, bundle y Next.js

### 3.1 `"use client"` en page.tsx (alto)

8 de 8 `page.tsx` funcionales están marcados `'use client'` en línea 1 cuando podrían delegar sólo el componente interactivo:

- `app/(patient)/consent/page.tsx:1`
- `app/(patient)/onboarding/page.tsx:1`
- `app/(patient)/dashboard/page.tsx:1` (**verificar** — Dashboard pide data client-side, pero el shell + metadata pueden ser RSC)
- `app/(patient)/registro/daily-checkin/page.tsx:1`
- `app/(patient)/registro/mood-entry/page.tsx:1`
- `app/(patient)/configuracion/telegram/page.tsx:1`
- `app/(patient)/configuracion/vinculos/page.tsx:1`
- `app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx:1`

Impacto: bundle cliente mayor, metadata/shell no RSC, TBT peor.

### 3.2 `@fontsource` vs `next/font` (medio)

`frontend/app/layout.tsx:2-9` importa fuentes via `@fontsource/*` packages. No garantiza `font-display: swap` explícito, pierde subsetting dinámico de `next/font/google`. El LCP de la landing (hero text con `Newsreader`) es sensible a esto.

Acción: migrar las 3 familias a `next/font/google` con `display: 'swap'` y `subsets: ['latin']`. Declarar variables CSS desde la API de next/font (`variable: '--font-display'`, etc.). La operación es UI-only, sin tocar auth/contratos.

### 3.3 Componentes monolíticos (alto/medio)

- `TelegramPairingCard.tsx` — 455 líneas, mezcla polling, display de código, expiración. `impeccable-extract` puede partirlo en `PairingCodeDisplay` + `PairingStatusPoller` + `PairingInstructions`.
- `Timeline.tsx` (profesional) — 403 líneas, SVG chart calculado en cada render sin `useMemo`. Envolver `moodPoints`, `scaleX`, `paths` con `useMemo([entries, preset])`.
- `Dashboard.tsx` — 315 líneas, fetches paralelos OK, pero transformaciones de lista sin memoización.

### 3.4 `next.config.mjs` (bajo)

- `output: 'standalone'` presente (correcto para Dokploy).
- Headers actuales: `X-Frame-Options`, `X-Content-Type-Options`, `X-XSS-Protection`, `Referrer-Policy`, `Permissions-Policy`.
- **Ausentes:** `Content-Security-Policy`, `Strict-Transport-Security`. **NO actuar** — zona auth/seguridad congelada, sólo se anota como deuda.
- `compress: true` implícito pero no explicitado; `experimental.optimizeCss` no está. Esfuerzo-beneficio bajo dado que el stack es lean (ver 3.6).

### 3.5 Motion y `prefers-reduced-motion` (medio)

`tokens.css:58-64` aplica `animation-duration: 0.01ms !important` globalmente, lo cual neutraliza pero en un loop infinite genera ciclos de paint vacíos. Componentes con shimmer sin `@media (prefers-reduced-motion: reduce) { animation: none; }` local:

- `Dashboard.module.css:42-45`
- `AuthBootstrapInterstitial.module.css:16` (spin)
- `TelegramPairingCard.module.css:24`
- `VinculosManager.module.css:19`
- `PatientPageShell.module.css:37-41`

(Los módulos profesionales — AlertsList, ExportGate, PatientList, Timeline, ProfessionalShell — sí tienen la regla local.)

### 3.6 Stack lean (positivo)

Sólo 5 dependencias prod (`next`, `react`, `react-dom`, 3×`@fontsource`). Cero lodash, moment, chart libs. Tree-shaking saludable. No proponer nuevas dependencias.

---

## 4. Theming y tokens del DS

### 4.1 **Divergencia crítica** tokens.css vs `11_identidad_visual.md` (alto)

| Token | Canon 11 §"Tokens base" | `styles/tokens.css:32-35` | Delta |
|---|---|---|---|
| `--radius-sm` | 8px | 4px | **-4px** |
| `--radius-md` | 14px | 8px | **-6px** |
| `--radius-lg` | 20px | 12px | **-8px** |

Decisión requerida: actualizar `tokens.css` al canon (implica pasada visual por todos los CSS Modules para revisar que no se rompan bordes) **o** actualizar `11_identidad_visual.md` con los valores implementados.  *Recomendado:* conservar tokens actuales y hacer un PR de canon sync dentro de `impeccable-normalize` — menos riesgo de regresiones visuales.

### 4.2 Tokens faltantes con fallbacks hardcodeados (alto)

- `frontend/components/professional/ExportGate.module.css:125,163`, `Timeline.module.css:177,198,217` → usan `var(--border)` — no existe en tokens. Reemplazar por `var(--border-subtle)`.
- `frontend/components/professional/InviteForm.module.css:84` → `var(--semantic-warning, #d97706)` — token inexistente + fallback hex. Reemplazar por `var(--status-warning-border)` o definir `--semantic-warning: var(--status-warning)`.
- `frontend/components/patient/dashboard/MoodEntryDialog.module.css:18` → `var(--shadow-strong, 0 24px 48px rgba(16,24,40,0.18))` — token inexistente con fallback crudo. Definir `--shadow-strong` en tokens.css y eliminar el fallback rgba.
- `frontend/components/patient/dashboard/MoodEntryDialog.module.css:17` → `var(--radius-lg, 16px)` — fallback 16px contradice el token real (12px). Eliminar el fallback.
- `frontend/components/patient/dashboard/MoodEntryDialog.module.css:11` → `background-color: rgba(16, 24, 40, 0.55)` para el backdrop. Definir `--overlay-backdrop`.
- `frontend/components/professional/Timeline.module.css:191,232` → `color: #fff` hardcodeado en botones activos. Definir `--foreground-on-brand: #FFFFFF` y usarlo.

### 4.3 Magic numbers de spacing (medio)

Valores en rem/pixel crudo fuera de la escala `--space-*`:

- `components/patient/vinculos/VinculosList.module.css:13,27,154` → `padding: 2rem`, `1.25rem`, `1rem`.
- `components/patient/vinculos/VinculosManager.module.css:52` → `padding: 1rem`.
- `components/professional/AlertsList.module.css:18,61`, `PatientList.module.css:56,159`, `Timeline.module.css:52,212,228` → paddings de 1-4px crudos. Aceptables en chips pequeños como exception tipográfica documentada.

### 4.4 Tipografía fluida (medio)

`clamp()` usado sólo en `OnboardingEntryHero.module.css:42`. El resto de headings tiene tamaños rígidos en rem. Extender a una escala fluida documentada en `tokens.css`.

### 4.5 Fuentes (bajo)

- `Newsreader` sin variantes 600/700 cargadas (layout.tsx:2-4). Si algún módulo usa `font-weight: 600` con `--font-display`, genera faux bold.

---

## 5. Responsive

### 5.1 Breakpoints fragmentados (medio)

5 breakpoints distintos en uso sin inventario canónico:
- `max-width: 359px` (1)
- `min-width: 375px` (1)
- `min-width: 480px` (2) + `max-width: 480px` (1)
- `min-width: 560px` (2)
- `min-width: 768px` (2)

Definir en `tokens.css`: `--bp-sm: 480px`, `--bp-md: 768px`, `--bp-lg: 1024px` (o custom media queries con PostCSS). Unificar progresivamente.

### 5.2 Mobile-first: sin regresiones detectadas

Canon 12 pide "1 columna mobile-first por defecto". Los CSS Modules revisados respetan la base mobile-first. Sin hallazgos blokeantes en pantallas ≤ 360px.

---

## 6. State coverage

Por canon `TECH-FRONTEND-SYSTEM-DESIGN.md §"Estados de interacción"`, cada componente debería cubrir los estados relevantes. Gaps detectados (no todos son bloqueantes, muchos no aplican según rol):

| Componente | Estados faltantes relevantes |
|---|---|
| Dashboard | `session_expired` (hoy redirige sin feedback visible), `conflict`, `locked` |
| MoodEntryDialog | `error` (hoy lo delega al form, OK — pero no hay feedback a nivel dialog) |
| TelegramReminderBanner | `error` (hoy silenciado deliberadamente por comentario; evaluar si expone errores de `getTelegramSession`) |
| ConsentGatePanel | `session_expired`, `expired`, `revoked`, `locked` |
| PatientPageShell | `session_expired`, `locked` |

Recomendación: `impeccable-harden` para los flows `session_expired` y `error silencioso`.

---

## 7. Visual quality y jerarquía

### 7.1 Dashboard densidad tipo tablero (alto)

`Dashboard.tsx` combina `DashboardSummary` (3 tarjetas) + trendChart ("Variabilidad diaria") + entryList ("Registros recientes") + grupo de acciones. Canon 10 §Anti-señales: "evitar dashboards del paciente que parezcan tableros de vigilancia". Canon 12: "la densidad nunca debe acercar una pantalla al lenguaje de dashboard".

Acciones sugeridas:
- Considerar ocultar `DashboardSummary` cuando haya <7 registros (empty/early state), o desmarcarla visualmente del trend + recent.
- Mover las 3 tarjetas stat a vista expandible (`<details>`) en mobile.
- Renombrar `"Variabilidad diaria"` → `"Cómo te fue estos días"` (más humano).

### 7.2 TelegramPairingCard — CTAs simultáneos (alto)

En estado `pairing active` aparecen 3 CTAs: "Copiar mensaje" + "Abrir Telegram" + "Ya envié el mensaje" (`TelegramPairingCard.tsx:279-301`). Canon 12 exige "una acción dominante + secundaria silenciosa". Agrupar por jerarquía:
- primaria: "Abrir Telegram"
- secundaria: "Copiar mensaje"
- separada (debajo, ritmo distinto): "Ya envié el mensaje → Verificar"

### 7.3 PatientPageShell sin navegación formal (bajo)

Botón "Cerrar sesión" aislado sin header ni indicador de ubicación. A medida que crezcan secciones paciente, desorienta. No bloqueante en MVP.

### 7.4 Delay teatral en BindingCodeForm (medio)

`setTimeout(() => { setState('idle'); onSuccess(); }, 1500)` en `BindingCodeForm.tsx:46-49`. Canon 12 §"Motion no permitido": "delays teatrales". Reducir a 400ms o resolver sincrónicamente con toast.

---

## 8. Estado de lo alineado (positivo — no tocar)

### 8.1 Copy congelado alineado

Todos los labels post-decisión dashboard-first (2026-04-22) están en código y matchean literalmente con el canon:
- `"Ingresar"` (OnboardingEntryHero.tsx:36)
- `"Tu espacio personal de registro"` (OnboardingEntryHero.tsx:28)
- `"Empezá con tu primer registro"` (Dashboard.tsx:198)
- `"Registrar humor"` (Dashboard.tsx:205)
- `"+ Nuevo registro"` (Dashboard.tsx:301)
- `"Check-in diario"` (Dashboard.tsx:304)
- `"Nuevo registro"` (MoodEntryDialog.tsx:55)
- `"Recibí recordatorios por Telegram"` (TelegramReminderBanner.tsx:59)
- `"Conectar"` / `"Ahora no"` (TelegramReminderBanner.tsx:64,66)
- `"Registro guardado."` / `"Check-in guardado."` (MoodEntryForm.tsx:86 / DailyCheckinForm.tsx:133)

### 8.2 Componentes bien implementados

- `MoodScale.tsx` — `role="radiogroup" + role="radio" + aria-checked` correcto.
- `MoodEntryDialog` — Escape + backdrop click + `aria-labelledby` OK.
- `InlineFeedback` — role/variant pattern (aunque con conflicto aria-live ya señalado).
- `AlertsList`, `ExportGate`, `PatientList`, `Timeline`, `ProfessionalShell` — tienen `prefers-reduced-motion` local.
- `<html lang="es">` presente.
- Todos los inputs críticos con `<label htmlFor>`.
- Skeleton loaders con `aria-hidden="true"`.
- `aria-busy` en la mayoría de submits.

### 8.3 Infra lean

- `next.config.mjs` con `output: 'standalone'` + 5 headers de seguridad.
- 5 dependencias prod (zero bloat).
- `tokens.css` con custom properties canónicas (salvo los 3 radios señalados).
- `@fontsource` subido por npm, no Google Fonts CDN.

---

## 9. Mapping hallazgos → skills `impeccable-*`

| Skill | Cluster de hallazgos | Archivos tocados (estimado) |
|---|---|---|
| `impeccable-audit` | baseline (este documento) | read-only |
| `impeccable-critique` | 7.1 Dashboard densidad, 7.2 TelegramPairingCard CTAs, flujo completo paciente | evaluación UX sin diff |
| `impeccable-harden` | 1.3 errores vagos, 6 state coverage (session_expired/conflict/locked), 2.6 status messages dismiss + `aria-live` | 8-10 archivos |
| `impeccable-normalize` | 4.1 radios canon-vs-implementación, 4.2 tokens faltantes (`--border`, `--semantic-warning`, `--shadow-strong`, `--foreground-on-brand`, `--overlay-backdrop`), 4.3 magic spacing, 5.1 breakpoints | 10-15 archivos CSS |
| `impeccable-extract` | TelegramPairingCard→3 subcomponentes, Dashboard helpers, banner genérico reutilizable, dialog genérico | 4-6 archivos nuevos |
| `impeccable-clarify` | 1.1 tildes, 1.2 tuteo, 1.3 copy vago, 1.4 scoring language, 2.5/2.6 aria-labels descriptivos, 7.3 nav shell | 12-15 archivos |
| `impeccable-optimize` | 3.1 `"use client"` → RSC, 3.2 next/font, 3.3 useMemo/useCallback, 3.5 reduced-motion local | 10-12 archivos |
| `impeccable-adapt` | 5.1 breakpoints fragmentados, 4.4 tipografía fluida con clamp, 2.5 touch targets mobile | 8-10 archivos CSS |
| `impeccable-onboard` | 7.1 Dashboard vacío con 0 registros (ya hay empty state; refinar), empty state más sereno | 2-3 archivos |
| `impeccable-polish` | 1.5 rozaduras menores, 7.4 delay teatral, consistencia final | 5-7 archivos |
| `impeccable-delight` | micro-interacciones sutiles respetando `prefers-reduced-motion` (muy acotado por canon 10 postura sobria) | ≤3 archivos |
| `impeccable-colorize` | N/A (canon pide sobriedad; `impeccable-quieter` más aplicable si algo ) | — |
| `impeccable-bolder` | N/A | — |
| `impeccable-quieter` | eventual — si la pasada de normalize deja algo muy cargado | ≤3 archivos |
| `impeccable-distill` | 7.2 densidad Dashboard (si critique lo confirma) | 1-2 archivos |

---

## 10. Riesgos, invariantes y congelados

**No tocar:**
- `frontend/app/auth/callback/route.ts`, `frontend/lib/auth/server.ts`, `frontend/proxy.ts`.
- Cualquier endpoint en `src/` (backend .NET).
- Copy congelado post dashboard-first (ver §8.1).
- `frontend/lib/api/client.ts` — shape congelado; sólo limpieza de campos huérfanos con `grep` confirmado.
- `Content-Security-Policy`, `HSTS` — zona auth/security no en scope.
- Dependencias npm — no agregar nuevas.

**Invariantes de proceso:**
- NUNCA `taskkill`, `--no-verify`, `--amend` sobre empujados, `push --force main`.
- Artefactos efímeros a `tmp/` o `artifacts/e2e/2026-04-22-<slug>/`.
- Cross-platform: condicionar por `process.platform` si aplica.

**Tests:** Playwright 8 specs verdes. Cualquier regresión bloquea la wave.

---

## 11. Propuestas de orden de ejecución (pendiente confirmación humana en Paso 4)

El canon de Bitácora ranquea: **Security → Privacy → Correctness → Usability → Maintainability → Performance → Cost → TTM**. Para este hardening UI-only:

- **A11y-first** es la opción alineada con la priority list (Correctness + Usability sobre Performance), y con el compliance salud (usuarios potencialmente en vulnerabilidad, cansancio atencional).
- **Perf-first** atacaría bundle/LCP antes de corregir accesibilidad, lo cual contradice el orden canónico.
- **Visual-first** (colorize/delight/bolder) está desalineado con canon 10 postura sobria — se descarta salvo como polish final acotado.

La decisión de ordenamiento va a `AskUserQuestion` en el próximo paso.

---

## 12. Evidencia cross-check

- **Explorer A** (estado frontend vs canon): 19 componentes auditados, evidencia `file:line` en cada hallazgo.
- **Explorer B** (a11y/ARIA/foco/contraste): 20+ hallazgos con WCAG ref.
- **Explorer C** (perf/tokens/DS): 15 hallazgos priorizados high/med/low.
- **Lectura directa** de `frontend/styles/tokens.css` para confirmar divergencia radios-canon.

Cross-references confirmados en ≥2 explorers:
- Tildes sistemáticas (A ∩ B)
- `#fff` / `--semantic-warning` hardcode (A ∩ C)
- Shimmer sin reduced-motion local (A ∩ B ∩ C)
- TelegramPairingCard monolítico (A ∩ C)
- Dashboard densidad dashboard-like (A: canon match)

---

**Estado:** baseline `impeccable-audit` cerrado.
**Siguiente paso:** `Skill("brainstorming")` + `AskUserQuestion` para fijar orden de ejecución de las skills `impeccable-*` antes de `writing-plans`.
