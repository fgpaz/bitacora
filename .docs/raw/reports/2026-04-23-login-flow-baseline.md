# Login Flow Baseline — Consolidated Audit 2026-04-23

**Fecha:** 2026-04-23
**Base repo:** `main` @ `0bcb11e` (working tree limpio al inicio de la corrida)
**Scope:** landing (`/`) → ingresar → callback IdP → onboarding / consent → dashboard → modal registro → sesión expirada / logout / error.
**Método:** 5 `ps-explorer` en paralelo cross-checkeados + verificación directa de `tokens.css`, `middleware.ts` (ausente), `frontend/app/page.tsx`, rutas de registro.
**Modo:** read-only. Cero modificación de código en esta corrida.
**Compliance:** Ley 25.326 / 26.529 (consentimiento informado) / 26.657 (salud mental) — el audit NO propone cambios de storage, access control ni audit logging. Marcas `⚠ legal-review` cuando el hallazgo roce autonomía del paciente o derecho a rechazo.
**Enlaces:**
- Canon precedente: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`
- Baseline previo (frontend entero, no solo login flow): `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md`
- Critique previo: `.docs/raw/reports/2026-04-22-impeccable-critique.md`
- Reporte final de esta corrida: `.docs/raw/reports/2026-04-23-login-flow-audit.md`

---

## 1. Anti-Patterns Verdict

**¿Parece AI-generated? No.** El frontend de Bitácora es austero, editorial y sobrio. Evita las "AI slop tells": sin gradientes arcoíris, sin glassmorphism, sin hero metrics teatrales, sin card grids decorativas de 3 columnas con íconos. La tipografía es editorial-humanista (`Newsreader` + `Source Sans 3`). La paleta es cálida-contenida con acentos terracota, sin púrpuras tecnológicos.

**Sin embargo**, aparece un anti-patrón de otra clase: **dashboard-vigilancia sutil** en el hub de retorno. La combinación de `DashboardSummary` (3 stat cards con números grandes) + gráfico de barras positivo/negativo "Tus últimos días" + lista "Registros recientes" con badges de puntaje reproduce la estética de panel clínico que el manifiesto 10 prohíbe explícitamente. No es AI slop — es un SaaS-pattern heredado sin adaptación al canon.

**Tell específico**: un heading `"Mi historial"` (`Dashboard.tsx:22`) como primer texto visible para el paciente RECURRENTE. El canon 10 §5.1 exige `"Acá puedo registrar cómo estoy sin quedar expuesta."` como primera emoción. `"Mi historial"` instala frame de archivo/dossier antes que frame de refugio.

---

## 2. Executive Summary

### Totales por severidad

| Severidad | Count |
|-----------|-------|
| P0 — blocker | **4** |
| P1 — major | **15** |
| P2 — minor | **14** |
| P3 — delight | **3** |
| Positivos vigentes | **12** |
| `⚠ legal-review` | **1** (E2-F2) |
| `needs-prod-validation` | **3** |
| **Total hallazgos activos** | **36** |

### Top-3 problemas

1. **E3-F1 P0 — "+ Nuevo registro" está enterrado al final del scroll en estado `ready`**: `Dashboard.tsx:292-307`. Para el recurrente que abre el dashboard a las 23:47 en mobile, tiene que scrollear por encima de banner + DashboardSummary + gráfico + lista (600-700px) antes de ver el único botón que importa. Responde DIRECTAMENTE la queja del product owner.

2. **E3-F6 P0 — Logout 1-click sin confirmación y posicionado como primer elemento táctil**: `PatientPageShell.tsx:41-49` + `.module.css:62` (`align-self: flex-end`). Botón `"Cerrar sesión"` en esquina superior derecha, sin `aria-label` enriquecido (solo `title`, no accesible en touch), sin diálogo de confirmación. Primer touch target en el flujo DOM antes del contenido. Borrar sesión no es destructivo de datos, pero sí de continuidad de registro clínico — un tap accidental obliga a re-autenticar OIDC (fricción desproporcionada para usuario en vulnerabilidad).

3. **E5-F4 P0 — `app/global-error.tsx` NO existe**: cualquier excepción que bubble up al layout root renderiza pantalla cruda del runtime de Next.js sin branding, sin copy, sin CTA. Dead-end visual total. El canon 10 "refugio clínico sereno" se rompe cuando la aplicación falla al nivel más sensible.

### Verdict preliminar

**`needs-redesign`** para el dashboard como hub de retorno. Los otros surfaces (landing, onboarding, consent, modal, edge) están en `needs-refinement` — refinamientos quirúrgicos sobre base correcta. El verdict global lo impone el dashboard: es donde vive el dolor del product owner y donde ninguna micro-corrección va a resolver el problema estructural de jerarquía.

---

## 3. Mapa del journey

### Persona A — Paciente primera vez (via invitación)

```text
[/]                       OnboardingEntryHero variant=standard
  |                       (landing siempre pública; sin redirect cookie-viva)
  |
  | click "Ingresar"
  v
[/ingresar]               Route Handler → redirect Zitadel (SIN interstitial, sin feedback)
  |
  v
[Zitadel login]           (externo, fuera de scope)
  |
  v
[/auth/callback]          (bloqueada para auditar)
  |
  v
[/onboarding]             OnboardingFlow → AuthBootstrapInterstitial "Continuando con tu registro..."
  |                       → bootstrapPatient() retorna {needsConsent, resumePendingInvite}
  |
  | needsConsent=true → muestra ConsentGatePanel
  v
[/consent]                ConsentGatePanel: h2 "Consentimiento informado" → inviteHint
  |                       "Recordá que viniste a través de una invitación de tu profesional."
  |                       → decisionBar fixed con único CTA "Aceptar y continuar"
  |                       (⚠ SIN CTA de rechazo, SIN mención de revocación)
  |
  | click "Aceptar y continuar"
  v
[/dashboard]              empty state: "Empezá con tu primer registro" + CTA "Registrar humor"
  |                       DashboardSummary OCULTO (closure 2026-04-22 vigente).
  |                       Banner Telegram visible si linked=false.
  |
  | click "Registrar humor"
  v
[MoodEntryDialog]         <dialog> nativo showModal() — aria-modal, Esc, backdrop click,
                          focus trap nativo. MoodScale 44×44. Submit SPA.
                          Success: role=status aria-live=polite "Registro guardado."
                          ⚠ Modal NO se cierra automáticamente — queda abierto
                          indefinidamente sin puente de siguiente acción (E4-F1/F2).
```

### Persona B — Paciente recurrente

```text
[/] (cookie viva)         OnboardingEntryHero variant=standard, idéntica a primera vez.
  |                       SIN redirect, SIN saludo, SIN señal "ya estás adentro".
  |                       Ausencia total de continuidad — P1 E1-F2/F5.
  |
  | click "Ingresar"
  v
[Zitadel SSO silencioso]  (externo, probable auto-redirect si cookie Zitadel viva)
  |
  v
[/auth/callback] → [/onboarding] → bootstrapPatient() retorna needsConsent=false
  |                                → window.location.assign('/dashboard')
  v
[/dashboard]              viewState=ready. Orden DOM:
                          1. TelegramReminderBanner (solo si unlinked)
                          2. DashboardSummary — "Registros totales" + "Promedio de humor"
                             + "Último registro" (peso tipo tablero, E3-F3 P1)
                          3. Heading h1="Mi historial" (E3-F7: viola copy congelado)
                          4. section "Tus últimos días" (trendChart — columnas <24px
                             en 360px con 10 entradas, E3-F9 P1)
                          5. section "Registros recientes" (lista 10 items con badges)
                          6. section .actions → "+ Nuevo registro" + "Check-in diario"
                                                  (⬅ enterrado al fin, E3-F1 P0)
                          Logout en esquina superior derecha — primer elemento tap,
                          1-click sin confirm (E3-F6 P0).
                          Sin link a /configuracion/vinculos (E3-F12 P1).

  | Subruta: sesión expira mientras edita mood →
  v
[inline sessionBlock]     "Tu sesión caducó. Ingresá de nuevo." + <a href="/ingresar">
                          (MoodEntryForm.tsx:79, DailyCheckinForm.tsx:122).
                          Copy correcto canon 13. Sin aria-live en el contenedor padre.

  | Subruta: logout explícito →
  v
[signOut() externo]       destino post-logout no auditable (zona bloqueada).

  | Subruta: error global →
  v
[app/error.tsx]           Título "No pudimos cargar esta pantalla" (OK).
                          Sub "Ocurrió algo inesperado." (E5-F1 P1 — regresión parcial
                          canon 13: prohíbe "algo inesperado").
                          CTA "Reintentar" → reset(). Sin branding header.

  | Subruta: error en layout root →
  v
[(NO global-error.tsx)]   Pantalla cruda del runtime. Dead-end sin marca. (E5-F4 P0)
```

---

## 4. Inventario de hallazgos (consolidado)

Tabla canónica — ordenada por severidad, luego por surface.

| ID | Surface | Persona | Heurística violada | Severidad | Evidencia file:line | Impacto observado |
|----|---------|---------|--------------------|-----------|---------------------|-------------------|
| **E3-F1** | dashboard | recurrente | Lineamiento 12 §"una acción dominante"; Patrón 16 #3 "Gesto rápido de valor"; Manifiesto 10 §4.1 "registrar sin friccion" | **P0** | `Dashboard.tsx:292-307` — `<section className={styles.actions}>` con botón `"+ Nuevo registro"` es el ÚLTIMO bloque del DOM en `ready`. Orden: banner → DashboardSummary → trendPanel → lista recientes → `actions`. En mobile 360px con 10 entradas, el CTA está ~600-700px debajo del top. | Recurrente nocturno en mobile NO VE el CTA de registro al cargar. Responde literalmente la queja del PO: "no queda claro cómo registra algo nuevo". |
| **E3-F6** | shell | ambos | Lineamiento 12 §"acciones destructivas explícitas"; canon 10 §5.2 capa conductual ("¿Estoy aceptando algo sin darme cuenta?") | **P0** | `PatientPageShell.tsx:41-49` + `.module.css:62` (`align-self: flex-end`). `<button type="button" title="Cerrar sesión">` sin `aria-label`, sin diálogo de confirmación, posicionado en esquina superior derecha — PRIMER elemento tap en orden DOM antes de `<div className={styles.content}>`. | Toque accidental en mobile cierra sesión sin cancel. Re-autenticar OIDC con cookie Zitadel probablemente viva = fricción de login completo. Pérdida de contexto de registro en curso (si sesión cae en medio del modal). |
| **E5-F4** | global-error | ambos | Patrón 16 #7 "Error recuperable localizado"; canon 10 §"refugio sereno" | **P0** | Glob `frontend/app/global-error.tsx` → sin resultados. | Cualquier fallo en `<Providers>` o `layout.tsx` renderiza pantalla cruda del runtime Next.js sin branding "Bitácora", sin copy, sin CTA. Dead-end visual total en el momento de mayor vulnerabilidad (cualquier error catastrófico). |
| **E1-F1** | landing | ambos | Lineamiento 12 §"acción dominante + secundaria silenciosa"; Patrón 16 #10 "Hero contextual adaptativo" | **P1** | `OnboardingEntryHero.tsx:35-39` — `ctaStack` contiene ÚNICAMENTE `<Link href="/ingresar">Ingresar</Link>`. No existe CTA secundaria. | El hero tiene una acción dominante pero NINGUNA secundaria silenciosa. No hay "Conocer más", "Ver ejemplo", "Entrar con invitación". Una acción única sola no cumple el patrón — le falta el par silencioso que respalda la confianza. |
| **E1-F2** | landing | recurrente | Manifiesto 10 §continuidad; UX pattern "hero contextual adaptativo" | **P1** | `frontend/app/page.tsx:4` — `<OnboardingEntryHero variant="standard" />` siempre. `frontend/middleware.ts` **no existe** (verificado). Ninguna detección de sesión a nivel de `/`. | Paciente con cookie viva que carga `bitacora.nuestrascuentitas.com` ve la misma pitch de captación que un visitante anónimo. Tiene que volver a tocar "Ingresar" y confiar en SSO silencioso para volver al hub. Fricción y ansiedad de "¿no guardó mi sesión?". |
| **E1-F3** | landing | ambos | WCAG 2.2 §2.4.7 Focus Visible; canon 11 `focus-ring` | **P1** | `OnboardingEntryHero.module.css:62-77` — `.primaryCta` NO define `:focus-visible`. Solo `:hover` en línea 79. El token `--focus-ring` del canon 11 no está aplicado al CTA principal de entrada. | Usuario de teclado navegando por la landing no tiene indicador visible de foco sobre el único CTA accionable. Bloqueante WCAG AA en el punto de entrada de todo el producto. |
| **E2-F1** | consent | primera vez (invite) | Manifiesto 10 §10.1 "Si una pantalla pone primero 'Tu profesional verá tus registros' va contra el manifiesto porque instala vigilancia antes que control" | **P1** | `ConsentGatePanel.tsx:53-55` — `<p className={styles.inviteHint}>"Recordá que viniste a través de una invitación de tu profesional."</p>` aparece INMEDIATAMENTE después del `<header>` y ANTES de las secciones del consent. El paciente lee "profesional" antes de leer qué acepta. | Instala el frame de supervisión profesional como primer contexto de agencia, contradice explícitamente §10.1. (El critique 2026-04-22 Punto 3 pidió reposicionar — quedó a medias: el h2 precede al hint, pero el hint sigue antes del contenido.) |
| **E2-F2** | consent | primera vez | Manifiesto 10 §7.2 capa conductual; **⚠ legal-review Ley 26.529 Art. 2** autonomía del paciente | **P1** | `ConsentGatePanel.tsx:82-93` — ÚNICO botón `"Aceptar y continuar"`. No existe `"No acepto"`, `"Rechazar"`, link de salida, ni opción "volver más tarde". `decisionBar` es un único CTA sin alternativa visible. | El consentimiento parece puerta forzada, no decisión. Ley 26.529 Art. 2 exige que el consentimiento informado incluya posibilidad real de rechazo. El UI puede generar consentimiento bajo coacción implícita ("si no acepto no entro nunca"). **Requiere revisión legal antes de decidir fix.** |
| **E2-F3** | consent | primera vez | Patrón 16 #2 "Contenedor sensible" — reversibilidad visible; Manifiesto 10 §"control legible" | **P1** | `ConsentGatePanel.tsx` (archivo completo) — cero menciones de "revocar" o variantes. No hay link "Podés revocarlo cuando quieras", ni texto explicativo de reversibilidad. | El paciente acepta sin saber que puede revocar. Contradice canon 10 §8.1 "controles visibles para activar, desactivar o revocar" como señal de confianza obligatoria. |
| **E3-F2** | dashboard | recurrente | Manifiesto 10 §refugio sereno, ancla emocional de retorno; Patrón 16 #12 "Puente de siguiente acción" | **P1** | `frontend/app/(patient)/dashboard/page.tsx:22` — `<h1>Mi historial</h1>`. No aparece "Bienvenido de vuelta", fecha de hoy, ni copy congelado `"Tu espacio personal de registro"`. La landing prometió esto último pero el dashboard no lo cumple. | Recurrente ve inmediatamente frame de archivo ("historial") antes que frame de refugio. La promesa "tu espacio seguro" no se sostiene en la pantalla donde el usuario pasa más tiempo. |
| **E3-F3** | dashboard | recurrente | Lineamiento 12 §"la densidad nunca debe acercar una pantalla al lenguaje de dashboard"; Manifiesto 10 §12.2 "evitar dashboards del paciente que parezcan tableros de vigilancia" | **P1** | `Dashboard.tsx:218-222` — `<DashboardSummary />` en estado `ready` sin condición. `DashboardSummary.tsx:33-49` — 3 `<div className="card">` con labels `"Registros totales"`, `"Promedio de humor"`, `"Último registro"` y valores 2rem. Antes del heading de "Tus últimos días" y de la lista. | Peso visual tipo tablero en el primer viewport del recurrente. El critique 2026-04-22 Punto 1 identificó este problema; el closure lo resolvió para empty state pero NO para ready. |
| **E3-F5** | dashboard | recurrente | Patrón 16 #12 "Puente de siguiente acción" — navegación a config | **P1** | `TelegramReminderBanner.tsx:71` — único `<Link href="/configuracion/telegram">` en el árbol del dashboard. El banner se oculta si `session.linked === true` (línea 34) o si está descartado en localStorage por 30 días. No hay link a `/configuracion/vinculos` en ningún archivo en scope. | Recurrente con Telegram ya vinculado (o con banner descartado) no puede descubrir la configuración de recordatorios/vínculos desde el dashboard. La única ruta es tipear URL directo. Esto rompe la experiencia de "cuidarse sin pensar". |
| **E3-F9** | dashboard | recurrente | Lineamiento 12 §responsive mobile-first; WCAG 1.4.10 Reflow | **P1** | `Dashboard.module.css:193-201` — `grid-template-columns: repeat(var(--trend-count), minmax(0, 1fr))` con `gap: var(--space-xs)` (4px). En 360px: 328px útiles / 10 columnas - 9×4px gaps = ~28.8px por columna. `.trendDay` en 11px mono + 28px columna → overflow-wrap any → etiquetas ilegibles. El `gap` sube a `--space-sm` solo a partir de 480px (`Dashboard.module.css:345`), no a 360px. | Critique 2026-04-22 Punto 1 no resuelto: etiquetas de fecha ilegibles en 360px con muchas entradas. Usuario cansado en mobile ve un gráfico comprimido e incomprensible. |
| **E3-F12** | dashboard | recurrente | Patrón 16 #12 "Puente de siguiente acción" — gestión de vínculos invisible | **P1** | Búsqueda `/configuracion/vinculos` en archivos en scope → 0 matches. No hay link, tab, ni botón. | La ruta de gestión de vínculos clínicos (revocar profesional, activar compartición) está completamente invisible desde el hub principal. Rompe canon 10 §8.1 "controles visibles para activar, desactivar o revocar". Al recurrente le quedan ocultas las acciones más sensibles. |
| **E4-F1** | modal | ambos | Patrón 16 #12 "Puente de siguiente acción"; Nielsen #1 Visibilidad del estado | **P1** | `MoodEntryForm.tsx:82-87` — bloque de continuidad (`"Completar check-in diario"` + `"Volver al dashboard"`) condicionado a `{!embedded && ...}`. En modal (embedded=true), success muestra SOLO `"Registro guardado."` sin puente. | El modal no le dice al paciente qué viene después del guardado. Queda atrapado visualmente en éxito sin indicación de cierre ni confirmación de que el dashboard se actualizó. |
| **E4-F2** | modal | ambos | Nielsen #1 Visibilidad del estado; Patrón 16 #6 "Confirmación factual breve" | **P1** | `Dashboard.tsx:111` — `handleEntrySaved()` llama solo a `setRefreshNonce((n) => n + 1)`. NO invoca `closeDialog()`. El modal permanece abierto en estado success indefinidamente. | Usuario debe descubrir que el `×` cierra el modal. En contexto clínico genera confusión sobre si necesita otra acción. Rompe la economía de "un gesto rápido de valor" (patrón 16 #3). |
| **E5-F1** | error-page | ambos | Canon 13 §Errores — evitar genéricos; ejemplo prohibido `"Ups. Algo salió mal."` | **P1** (regresión) | `frontend/app/error.tsx:16` — `"Ocurrió algo inesperado. Probá recargar la página o volver en unos minutos."` El título (línea 14 `"No pudimos cargar esta pantalla"`) es correcto. El sub usa la fórmula genérica prohibida. | Regresión parcial post-hardening 2026-04-22 (el closure mencionó corrección del error page). La mitad del par título/descripción viola canon 13. |
| **E5-F7** | inline / dashboard / vinculos / onboarding | paciente | Canon 13 §Errores "decir qué pasó + qué puede hacer"; mensaje expuesto crudo al usuario | **P1** | `VinculosManager.tsx:50` — `'Error al cargar los vínculos'`. `Dashboard.tsx:157` — `"Error al cargar el historial"` + `window.location.reload()`. `OnboardingFlow.tsx:85` — `(err as Error).message ?? 'Error al cargar el consentimiento.'` (expone mensaje técnico crudo si `err.message` existe). | Errores sin CTA orientador. Peor: `OnboardingFlow.tsx:85` puede exponer al paciente strings como "Network Error" o "500 Internal Server" generados por fetch. Viola canon 13 + regla de seguridad de no exponer detalles técnicos. |
| **E5-F10** | shell | paciente | WCAG 4.1.3 + canon 13 — errores sin filtro; falta CTA recovery | **P1** | `PatientPageShell.tsx:27-33` — `<div className={styles.errorState} role="alert"><p>{error}</p></div>`. El prop `error` es string libre sin tipado ni canon. Sin botón reintento ni link `/ingresar`. | Cualquier consumidor puede pasar mensaje técnico crudo y se renderiza directamente. Vector de exposición + dead-end sin CTA. |
| **E2-F4** | consent | primera vez | Lineamiento 12 §CTA calmado; Patrón 16 #5 "Rail final de guardado" — no garantiza lectura | **P2** | `ConsentGatePanel.module.css:60-70` — `.decisionBar { position: fixed; bottom: 0; }`. El botón `"Aceptar y continuar"` es accionable desde momento 0, antes de cualquier scroll. | El paciente puede aceptar sin leer. En dispositivos pequeños puede solapar la última sección. No hay scroll obligatorio visible (FL-CON-01 lo exige funcionalmente). |
| **E2-F5** | consent | primera vez | WCAG 2.4.3 Focus Order; 4.1.3 Status Messages | **P2** | `OnboardingFlow.tsx:132-150` — al transicionar de `phase='auth'` a `phase='consent'` no hay `useEffect` que haga `focus()` sobre el `<h2>` ni sobre nada del `ConsentGatePanel`. El foco queda en posición residual. | Usuario de teclado / AT no recibe señal de que la pantalla cambió. Debe navegar ciegamente. |
| **E2-F6** | consent / onboarding | ambos | Patrón 16 #7 "Error recuperable"; canon 13 contexto de pantalla | **P2** | `OnboardingFlow.tsx:125-131` — mientras carga consent muestra `<PatientPageShell loading />` sin texto. `ConsentGatePanel.tsx` no contiene frase del tipo "Para registrar tu estado de ánimo necesitamos tu autorización explícita". | El "por qué" de la pantalla es invisible. Paciente aterriza en consent frío sin contexto. |
| **E2-F8** | consent | primera vez | Canon 13 §lenguaje llano vs clínico | **P2** | `ConsentGatePanel.tsx:49` — `<h2 className={styles.title}>Consentimiento informado</h2>`. Sin subtítulo humano que traduzca el término. | Primera palabra que lee el paciente en la decisión legal más sensible es terminología medico-legal. Canon 13 permite clínico cuando mejora precisión, pero exige bajada humana para reducir ansiedad. |
| **E2-F9** | rutas / arquitectura | ambos | Arquitectura de ruta | **P2** | `consent/page.tsx:1-18` + `onboarding/page.tsx:1-14` — ambas rutas montan el mismo `<OnboardingFlow />` idénticamente. Comentario en `consent/page.tsx:4` dice "Delegates..." pero no hay sub-estado diferenciado. | Dos URLs, misma UI. Deuda arquitectónica: el día que haya diferencia de estado recurrente vs primera vez, el refactor va a requerir separar lógica. |
| **E3-F4** | dashboard | recurrente | Lineamiento 12 §"aire generoso" + jerarquía semántica | **P2** | `Dashboard.tsx:224-229` h2 `"Tus últimos días"` y `Dashboard.tsx:260-264` h2 `"Registros recientes"` — mismo class `.sectionTitle`, misma jerarquía tipográfica, escasos `var(--space-lg)` de distancia. | Dos h2 consecutivos con peso idéntico pueden leerse como redundantes. ¿Qué hace cada uno que el otro no? |
| **E3-F7** | dashboard | ambos | Copy congelado 2026-04-22 "Tu espacio personal de registro" | **P2** | `page.tsx:22` — `<h1>Mi historial</h1>`. El string `"Tu espacio personal de registro"` NO aparece en ningún archivo en scope. Metadata title `"Mi historial | Bitácora"` (`page.tsx:13`) coherente con h1 pero no con canon. | Copy congelado incumplido en el heading de entrada. (Nota: el congelado está documentado en el baseline previo y en la decision doc 2026-04-22; requiere decisión humana si se actualiza el congelado o se migra el h1.) |
| **E3-F8** | dashboard / modal | ambos | Lineamiento 12 §consistencia de jerarquía; WCAG 4.1.2 | **P2** | `Dashboard.tsx:204-207` estado empty → botón texto `"Registrar humor"` (copy congelado). `Dashboard.tsx:291-296` estado ready → botón `"+ Nuevo registro"`. Misma acción, dos textos distintos. Ninguno tiene `aria-label`. | Inconsistencia confunde a AT y a usuarios que comparan estados. Solo empty respeta copy congelado. |
| **E3-F10** | banner | recurrente | Patrón 16 #12 | **P2** | `TelegramReminderBanner.tsx:34-36` — banner solo visible si `session.linked === false`. Para `linked === true` no hay ningún CTA/link de gestión en el mismo slot. | Usuario que ya vinculó Telegram pierde acceso visible a gestionar/cambiar su config desde dashboard. |
| **E4-F3** | modal | ambos | WCAG 2.5.8 Target Size (AA WCAG 2.2) | **P2** | `MoodEntryDialog.module.css:57-58` — `.closeBtn { min-width: 40px; min-height: 40px }`. Por debajo del mínimo 44×44 declarado en MoodScale. Inconsistente dentro del mismo modal. | Close button más difícil de tocar que el resto del modal. Problema para dedos grandes / temblor fino (común en población objetivo). |
| **E5-F2** | not-found | ambos | Canon 13 §Errores — CTA contextual | **P2** | `frontend/app/not-found.tsx:12-15` — CTA `"Volver al inicio"` → `/`. Paciente autenticado con URL incorrecta aterriza en landing pública, no en dashboard. | El CTA no diferencia anónimo vs autenticado. Paciente autenticado hace un viaje extra vía landing → ingresar → SSO para volver al hub. |
| **E5-F3** | form / session | paciente | WCAG 4.1.3 Status Messages; Patrón 16 #11 | **P2** | `MoodEntryForm.tsx:79` + `DailyCheckinForm.tsx:122` — copy `"Tu sesión caducó. Ingresá de nuevo."` + `<a href="/ingresar">`. Correcto en copy, pero el `sessionBlock` se monta sin `role="alert"` ni `aria-live` propio; el contenedor padre solo tiene `role=alert` en su estado error. | AT no anuncia automáticamente la transición a "sesión expirada" dentro del form. Usuario con screen reader puede quedarse intentando llenar el form sin saber que ya no es posible. |
| **E5-F8** | error-page | ambos | Manifiesto 10 §identidad visible; Patrón 16 #7 | **P2** | `app/error.tsx:12-26` monta `<main>` directo sin wordmark "Bitácora". `layout.tsx:35-43` no incluye header en root (solo `<Providers>`). | Paciente ve un panel de error sin saber que está en Bitácora. Rompe continuidad de marca y aumenta sensación de "se rompió todo". |
| **E5-F9** | offline | ambos | Canon 10 §seguridad sin juicio — falta manejo | **P2** | Grep `offline|navigator.onLine|sin red|network` en `frontend/**/*.tsx` → 0 resultados. | Sin detección de red. Con conexión inestable el form falla con mensaje genérico (peor: con `err.message` crudo por E5-F7). No hay "Estás sin conexión, tu registro se guardará cuando vuelvas". |
| **E1-F4** | landing | invite | Patrón 16 #10 "Hero contextual adaptativo" — inviteLabel precede h1 | **P2** | `OnboardingEntryHero.tsx:17-27` — para `variant="invite"` / `"invite_fallback"`, `<p className={styles.inviteLabel}>...</p>` renderiza antes del `<h1>`. Hallazgo adicional HA-1 del critique 2026-04-22 sigue vigente. | Contexto de invitación se impone visualmente al headline del producto. Misma lógica que E2-F1: "instala vigilancia antes que control". Severidad menor aquí porque no hay decisión de consent en juego. |
| **E1-F5** | landing | recurrente | Manifiesto 10 §continuidad emocional | **P2** | `OnboardingEntryHero.tsx:28` — headline único `"Tu espacio personal de registro"`. Sub (línea 31-34) explica producto desde cero. Sin variante de bienvenida para recurrente. | Recurrente recibe pitch de captación en cada retorno. Fricción editorial innecesaria. |
| **E1-F6** | /ingresar | ambos | Manifiesto 10 §ansiedad editorial — feedback de estado | **P2** | `frontend/app/ingresar/route.ts:11-39` (route handler) — redirect directo a Zitadel sin interstitial, sin mensaje. | Salto visual abrupto entre click "Ingresar" y formulario de Zitadel. En conexión lenta parece que el botón no respondió. Sin "Te estamos llevando al ingreso seguro". |
| **E1-F7** | onboarding | primera vez | WCAG 1.3.1 — No-JS path | **P2** | `onboarding/page.tsx:5` — Suspense fallback es `<div style={{ minHeight: '100vh', background: 'var(--surface)' }} />`. Sin JS = pantalla vacía sin instrucción ni enlace. | En agentes low-JS / errores de hidratación, el paciente ve pantalla en blanco post-callback. |
| **E1-F8** | landing | primera vez | Lineamiento 12 §mobile-first ≤360px | **P2** (`needs-prod-validation`) | `OnboardingEntryHero.module.css:49-55` — `.sub { margin-bottom: var(--space-xl) }`. `tokens.css:61` — `--space-xl: 40px`. Sin media query ≤360px. | Hipótesis: en pantallas muy compactas el CTA puede quedar bajo el fold. Requiere validación en viewport real. |
| **E5-F5** | logout | paciente | Redundancia accesibilidad | **P3** (positivo parcial) | `PatientPageShell.tsx:43-49` — `type="button"` ✓, `title="Cerrar sesión"` ✓. Pero `title` y texto visible son idénticos. No hay `aria-label` descriptivo que agregue valor. | No es hallazgo bloqueante. Oportunidad: `aria-label="Cerrar tu sesión y volver al inicio"` agrega contexto sin violar canon. |
| **E2-F7** | interstitial | primera vez invite | Canon 13 §agencia del paciente | **P3** | `AuthBootstrapInterstitial.tsx:18` — variante invite: `"Continuando con tu registro..."`. Cumple canon (neutro, no menciona profesional). Oportunidad sin bloqueo. | Podría afirmar más la agencia: `"Preparando tu espacio..."`. |
| **E1-F9** | footer | ambos | WCAG 2.4.7 Focus Visible | **P3** | `OnboardingEntryHero.module.css:109-115` — `.footerLink` sin `:focus-visible`, solo `:hover`. | Enlace "Contactar soporte" sin focus ring. Severidad menor (soporte es secundario) pero relevante para usuario con problemas de acceso + teclado. |
| **E1-F10** | landing | primera vez | Metadata SEO / social preview | **P3** | `app/layout.tsx` — `<title>Bitácora</title>`, description `"Registro de humor y bienestar"`. Sin refuerzo de "espacio privado de salud mental". | Oportunidad no explotada. Link compartido en WhatsApp no refuerza promesa emocional. |
| **E4-F4** | mood-form | ambos | Nielsen #5 Prevención de errores | **P3** | `MoodEntryForm.tsx:119-126` — submit habilitado con cualquier score incluyendo `0` (valor válido pero visualmente indistinguible del estado inicial). | Registro involuntario de humor neutro. Bajo impacto clínico real (0 es dato válido), pero oportunidad de confirmación suave. |

---

## 5. Patterns & systemic issues

1. **Ausencia de continuidad emocional para el recurrente**. Aparece en E1-F2, E1-F5, E3-F2, E3-F7. El producto trata cada visita como primera vez. No hay saludo, ni fecha de hoy, ni "seguí donde dejaste". El canon 10 lo exige pero el código no lo implementa.

2. **Jerarquía invertida en superficies sensibles**. Aparece en E1-F4 (inviteLabel antes de h1) y E2-F1 (inviteHint antes de secciones de consent). Patrón repetido: la presencia profesional/contexto clínico precede al contenido del paciente. Canon 10 §10.1 lo prohíbe explícitamente.

3. **CTAs principales mal posicionados**. Aparece en E3-F1 (CTA registro enterrado) y E3-F6 (logout prominente). El peso visual está invertido: el acto cotidiano de valor (registrar) es el último del DOM; el acto raro y con fricción (cerrar sesión) es el primero. Canon 12 §jerarquía de CTA viola los dos.

4. **Mensajes genéricos en errores**. Aparece en E5-F1, E5-F7, E5-F10, E3 (error de dashboard). `"Ocurrió algo inesperado"`, `"Error al cargar X"`, `err.message` crudo. Canon 13 los prohíbe y exige fórmula concreta. Regresión parcial post-hardening + consumidores que pasan strings sin filtro.

5. **Configuración invisible para el recurrente**. E3-F5 + E3-F10 + E3-F12. La única ruta a `/configuracion/telegram` es el banner (condicional). `/configuracion/vinculos` es totalmente invisible. Rompe canon 10 §8.1 "controles visibles para activar, desactivar o revocar".

6. **Focus management inconsistente**. E1-F3 (landing CTA), E1-F9 (footer link), E2-F5 (transición a consent), E5-F3 (sessionBlock sin aria-live). El focus ring token existe (`--focus-ring`) pero no siempre se aplica; las transiciones de fase no manejan foco.

7. **Hero contextual adaptativo (patrón 16 #10) incompleto**. E1-F1 (sin secundaria silenciosa) + E1-F4 (inviteLabel orden) + E1-F5 (sin variante recurrente). El patrón requiere "misma estructura, cambio fuerte de copy"; hoy hay variantes pero no contemplan el caso recurrente.

---

## 6. Positive findings vigentes (del closure 2026-04-22)

El hardening del 2026-04-22 dejó 12 ítems correctamente implementados que deben **preservarse**:

1. `DashboardSummary` oculto en empty state — `Dashboard.tsx:174` comentario explícito canon 10.
2. `aria-modal="true"` en `MoodEntryDialog.tsx:51`.
3. `role="dialog"` nativo con `<dialog>.showModal()` → focus trap automático.
4. `aria-labelledby="mood-entry-dialog-title"` → `<h2 id="mood-entry-dialog-title">` (`MoodEntryDialog.tsx:52+57`).
5. Focus return al trigger via `triggerRef + requestAnimationFrame` (`Dashboard.tsx:107-109`).
6. `role="radiogroup" / role="radio" / aria-checked` en `MoodScale.tsx:20,26,27`.
7. `aria-busy` durante submit en `MoodEntryForm.tsx:122` + `DailyCheckinForm.tsx:249`.
8. `role="status" aria-live="polite"` en success blocks (`MoodEntryForm.tsx:79`).
9. `aria-pressed` en boolean buttons de `DailyCheckinForm.tsx:175,181,207,214`.
10. `aria-required="true"` en `sleep_hours` (`DailyCheckinForm.tsx:163`).
11. `InlineFeedback` sin conflicto `role=alert + aria-live=polite` — ahora condicional por variant (`InlineFeedback.tsx:19-21`).
12. `prefers-reduced-motion` local en `MoodEntryDialog.module.css:19-21` + tokens + canon 11 sync con radios 4/8/12 reales + `--foreground-muted: #4A4440` para AA.

**Logout correcto estructuralmente** (E5-F5): `type="button"` + `title` presentes. El problema es el posicionamiento + ausencia de confirmación (E3-F6), no el markup.

---

## 7. Recommendations preliminares por prioridad

### Inmediato (P0 — pre-merge blockers)

- **E3-F1** — subir `"+ Nuevo registro"` al TOP del dashboard en ready. Patrón 16 #3 "gesto rápido de valor" debe ser primer CTA visible. Recomendado: bloque sticky/prominent arriba de DashboardSummary o absorber DashboardSummary dentro del mismo CTA.
- **E3-F6** — dos caminos posibles (decisión humana): (a) agregar diálogo de confirmación "¿Cerrar tu sesión?" o (b) mover logout a overflow menu / footer del shell + agregar `aria-label` descriptivo. La segunda es más alineada con canon 12 "pocas decisiones visibles" + "acciones destructivas separadas".
- **E5-F4** — crear `frontend/app/global-error.tsx` con wordmark Bitácora + copy canon 13 + CTA "Volver al inicio". Sin dependencias nuevas.

### Corto plazo (P1 — esta sesión de implementación)

- **E1-F2 + E1-F5** — agregar `frontend/middleware.ts` que detecte cookie de sesión y redirija `/` → `/dashboard`. (Cruza zona auth — requiere coordinación con owner del shield auth para no romper callback.)
- **E2-F1** — reubicar inviteHint DESPUÉS de las secciones del consent, no antes. O trasladar a confirmación post-accept.
- **E2-F2** ⚠ legal-review — agregar CTA secundario `"No acepto"` con resultado visible (paciente vuelve a landing con mensaje "Podés aceptar cuando quieras, sin presión").
- **E2-F3** — agregar texto explícito de revocación en consent: "Podés revocarlo cuando quieras desde configuración".
- **E3-F3** — ocultar o compactar `DashboardSummary` en ready (patrón `<details>` colapsable + texto corrido fue la recomendación del critique 2026-04-22).
- **E3-F5 + E3-F12** — agregar sección de configuración visible en shell (nav mínima) o "Configurar" link en dashboard footer.
- **E3-F9** — colapsar trendChart a resumen textual en 360px o limitar a 5 entradas en mobile.
- **E4-F1 + E4-F2** — cerrar modal automáticamente tras success + toast `aria-live` "Registro guardado" en dashboard.
- **E5-F1** — reemplazar sub de `app/error.tsx` por copy canon: `"No pudimos completar la acción. Probá en unos minutos o volvé al inicio."`.
- **E5-F7** — envelopar todos los errores de componentes con función `formatUserFacingError(err)` que mapee técnico → canon 13. No exponer `err.message` crudo.
- **E5-F10** — tipar prop `error` de PatientPageShell como `{ title: string; description?: string; retry?: () => void }`.

### Medio plazo (P2 — siguiente wave)

- **E1-F1** — agregar CTA secundaria "Tengo un código de invitación" o similar (depende de producto).
- **E1-F3, E1-F9, E2-F5** — revisión sistemática de `:focus-visible` en todos los elementos interactivos del login flow.
- **E1-F7** — loading state con texto humano para no-JS path ("Cargando tu espacio..." como `<noscript>`).
- **E2-F4** — scroll obligatorio o bloqueo CTA hasta `scroll-bottom` (FL-CON-01 lo exige).
- **E2-F6** — contexto introductorio en consent ("Para registrar necesitamos tu autorización").
- **E2-F8** — subtítulo humano bajo "Consentimiento informado".
- **E2-F9** — evaluar colapsar `/consent` en ruta o diferenciar semánticamente.
- **E3-F4** — jerarquía visual distinta entre "Tus últimos días" y "Registros recientes" (o absorber una en otra).
- **E3-F7 + E3-F8** — alinear h1 + trigger con copy congelado O actualizar canon (decisión humana).
- **E3-F10** — slot "Configurar recordatorios" fijo en dashboard (no condicional).
- **E4-F3** — `closeBtn` 40→44px.
- **E5-F2** — CTA contextual en not-found según sesión.
- **E5-F3** — `aria-live` en sessionBlock de forms.
- **E5-F8** — header con wordmark en `app/error.tsx`.
- **E5-F9** — detector offline con banner + cola local de registros.
- **E1-F4** — reordenar inviteLabel después de h1 (mismo fix que E2-F1 para hero).

### Largo plazo (P3 — nice-to-have)

- **E1-F6** — interstitial breve entre click Ingresar y Zitadel.
- **E1-F10** — metadata OG enriquecida.
- **E4-F4** — confirmación visual suave para mood=0.
- **E2-F7** — copy más afirmativo en interstitial invite.
- **E5-F5** — aria-label descriptivo en logout.

---

## 8. Suggested impeccable-* commands para fix

| Skill | Hallazgos asignados | Racional |
|-------|---------------------|----------|
| `impeccable-distill` | E3-F1, E3-F3, E3-F4, E3-F7, E3-F8 | Rediseño del dashboard — simplificar narrativa, subir CTA, absorber DashboardSummary. |
| `impeccable-onboard` | E1-F4, E1-F5, E1-F7, E2-F1, E2-F6, E2-F8, E3-F2 | Primera vez + retorno emocional del recurrente + contexto de consent. |
| `impeccable-harden` | E2-F2, E2-F3, E2-F4, E2-F5, E3-F6, E5-F1, E5-F2, E5-F4, E5-F7, E5-F8, E5-F9, E5-F10 | Resiliencia + compliance + edge cases — bloque más grande del audit. |
| `impeccable-clarify` | E3-F5, E3-F12, E4-F1, E4-F2, E5-F3 | Copy y microcopy — puente siguiente acción + visibilidad de config. |
| `impeccable-normalize` | E1-F3, E1-F9, E2-F5, E4-F3 | Focus ring + touch targets — aplicación sistemática de tokens existentes. |
| `impeccable-adapt` | E3-F9, E1-F8 | Responsive ≤360px. |
| `impeccable-delight` | E4-F4, E2-F7, E5-F5, E1-F6 | Micro-momentos sobrios, sin violar manifesto. |
| `impeccable-polish` | E1-F1, E2-F9, E3-F10, E1-F10 | Detalles finales, jerarquía de secundarias, metadata. |
| **NO aplica** | — | `impeccable-bolder` (canon prohíbe), `impeccable-colorize` (canon prohíbe), `impeccable-extract`/`impeccable-optimize`/`impeccable-adapt` en modo profundo (fuera de scope de esta corrida). |

---

## 9. Preguntas abiertas / follow-ups

Cosas fuera de scope que surgieron durante el audit y requieren decisión o validación:

1. **🔒 needs-human-decision — E2-F2 consent sin CTA de rechazo**: ¿El producto quiere permitir rechazo con mensaje "podés aceptar más tarde" (recomendado) o mantener hard gate actual con fricción reducida? Impacta Ley 26.529. Requiere review legal.
2. **🔒 needs-human-decision — E3-F7 h1 "Mi historial" vs copy congelado**: ¿Se actualiza el canon 2026-04-22 al nuevo h1 o se revierte al congelado? Ambas son válidas; necesita decisión de producto.
3. **needs-prod-validation — E1-F2 middleware/redirect cookie viva**: confirmar en prod si Zitadel SSO silencioso efectivamente re-auth sin fricción o si requiere interacción.
4. **needs-prod-validation — E1-F8 CTA bajo fold en 360px**: medir en dispositivos reales (no solo DevTools).
5. **needs-prod-validation — E3-F6 logout accidental**: telemetría de `signOut()` vs `handleEntrySaved()` — ¿hay tasa de sesiones cerradas en mid-modal que evidencie el problema?
6. **Fuera de scope auth — E5-F5**: confirmar destino post-`signOut()` (a `/`, `/ingresar`, o IdP). Si es IdP directo, paciente ve Zitadel sin branding Bitácora tras logout.
7. **Fuera de scope auth — E5-session**: verificar si hay interceptor HTTP global que detecte 401 o si cada componente maneja el estado independientemente (E5-F3 sugiere el segundo).
8. **Gap traceability** — no existe `UJ-ONB-*` ni `UXS-ONB-*` en `.docs/wiki/23_uxui/` para el flujo login+dashboard+modal de esta auditoría. Recomendar `crear-journey-ux` y `crear-spec-ux` post-fix para dejar canon trazable.

---

## 10. Métricas proxy

- Hallazgos activos: **36**
- Surfaces tocados: **8** (landing, /ingresar, onboarding, consent, dashboard, shell, modal, edge-error)
- Heurísticas únicas violadas: **14** (manifesto §10.1, §5.2, §12.2, §refugio sereno; lineamiento §CTA, §densidad, §mobile-first, §acciones destructivas; patrón 16 #3, #10, #11, #12; WCAG 2.4.7, 4.1.3)
- Hallazgos por persona: primera vez **13**, recurrente **15**, ambos **8**
- Closure 2026-04-22 vigente: **12** ítems (sin regresiones estructurales); regresiones detectadas: **1** (E5-F1 parcial)

---

*Baseline consolidado del audit read-only 2026-04-23. Siguiente artefacto: `.docs/raw/reports/2026-04-23-login-flow-audit.md` con veredictos impeccable-critique/harden/normalize/onboard/delight/polish aplicados por surface.*
