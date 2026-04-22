# Impeccable Critique — Frontend Bitácora
**Fecha:** 2026-04-22 · **Wave:** 1 · **Modo:** read-only

---

## Contexto de evaluación

Este reporte evalúa cinco puntos de calidad UX/story identificados en el baseline `2026-04-22-impeccable-audit-baseline.md` §7. La evaluación contrasta el código real de `frontend/` contra el canon emocional (10), los lineamientos de interfaz (12) y los patrones UI (16).

**Población objetivo:** personas potencialmente en vulnerabilidad emocional (usuarios psicológicos/psiquiátricos).
**Postura canónica requerida:** `refugio clínico sereno` — sin tableros de vigilancia, sin scoring language, sin tono motivacional, sin celebración.

---

## Punto 1 — Dashboard: densidad tipo tablero

### Lectura del código

`Dashboard.tsx` en estado `ready` (líneas 219-314) apila los siguientes bloques en secuencia vertical dentro de un `.stack` con `gap: var(--space-lg)`:

1. `<TelegramReminderBanner />` — banner de acción de configuración.
2. `<DashboardSummary />` — 3 tarjetas stat: "Registros totales", "Promedio de humor", "Último registro" (`DashboardSummary.tsx:37-54`).
3. `<section .trendPanel>` — gráfico de barras "Variabilidad diaria" con escala positivo/negativo y ejes visuales explícitos (línea media, columnas de track, etiquetas de fecha).
4. `<section aria-labelledby="recent-entries-heading">` — lista de hasta 10 entradas recientes, cada una con fecha + badge de puntaje.
5. `<section .actions>` — grupo de 2 CTAs: "＋ Nuevo registro" (primaria) + "Check-in diario" (secundaria).

En mobile (≤ 360px) los 5 bloques se apilan en columna única sin colapso, sin secciones expansibles y sin reordenamiento. El `.actions` pasa a grilla 2 columnas recién en `min-width: 480px`.

`DashboardSummary.module.css` no fue suministrado, pero `DashboardSummary.tsx` renderiza una `div.grid` de 3 `div.card` cada uno con `.label` y `.value`. La estructura es de tipo "stat card grid".

### Contraste con canon

**Canon 10 §12.2:** "evitar dashboards del paciente que parezcan tableros de vigilancia".
**Canon 12 §Densidad:** "la densidad nunca debe acercar una pantalla al lenguaje de dashboard".
**Canon 12 §Jerarquía visual:** "lo que nunca debe competir con el contenido principal" incluye "métricas decorativas" y "capas de estado con apariencia de monitoreo".
**Canon 16 §1 Shell editorial de una columna:** columna única, aire generoso, "una sola dirección dominante en mobile".

La combinación de 3 stat cards + gráfico de barras con eje positivo/negativo + lista de entradas con badge de puntaje en la misma pantalla produce un efecto de **panel de monitoreo** que el canon 10 marca explícitamente como anti-señal. El problema no está en cada bloque aislado sino en su coexistencia simultánea sin jerarquía narrativa: el gráfico "Variabilidad diaria" introduce un lenguaje de análisis de tendencias que en contexto clínico puede leerse como scoring continuo.

El estado `empty` muestra `DashboardSummary` incluso cuando hay 0 registros (`Dashboard.tsx:174-177`), lo que significa que el paciente ve "Registros totales: 0 / Promedio de humor: — / Último registro: —" antes de haber interactuado con el producto. Esta es la situación de mayor vulnerabilidad narrativa: el usuario en onboarding ya se enfrenta a la retícula de métricas antes de haber registrado nada.

**Sobre el nombre "Variabilidad diaria"** (`Dashboard.tsx:231`): el término es técnico-clínico, no cotidiano. Combina con el gráfico de barras positivo/negativo para producir la imagen de un panel de análisis de estado anímico, que es precisamente el lenguaje de vigilancia que el canon prohíbe.

**En mobile ≤ 360px:** el `trendChart` tiene `min-height: 156px` con `grid-template-columns: repeat(var(--trend-count), ...)`, lo que en 10 entradas genera columnas muy estrechas (aprox. 30px cada una). El resultado es un gráfico de barras comprimido que, lejos de desaparecer, se torna ilegible pero visualmente denso — peor que escondido: confuso.

### Contradicción detectada con el baseline

El baseline §7.1 sugiere "mover las 3 tarjetas stat a vista expansible `<details>` en mobile" y "renombrar 'Variabilidad diaria' → 'Cómo te fue estos días'". Estas recomendaciones son correctas pero **insuficientes**: la contradicción central no es el nombre del gráfico sino la composición completa en estado `empty` que ya instala la retícula de métricas. El baseline no menciona el caso `empty` como superficie de riesgo. **Se documenta como contradicción:** el baseline trata la densidad como problema del estado `ready` solamente; el código muestra que el estado `empty` también expone las 3 tarjetas stat.

### Veredict: `rediseñar`

**Qué cambiar:**

1. **Estado `empty`:** eliminar `DashboardSummary` de la vista `empty`. El onboarding debe priorizar la acción de primer registro ("Empezá con tu primer registro" + CTA), no métricas vacías. El shell debe sentirse como invitación, no como tablero vacío.
2. **Estado `ready`:** desacoplar visualmente `DashboardSummary` del flujo principal. Opciones ordenadas de menor a mayor cambio: (a) mover las 3 tarjetas a una sección colapsable `<details>` o detrás de un toggle silencioso; (b) convertirlas en texto corrido en lugar de cards stat individuales ("Llevas 12 registros. El último fue el 15 abr."); (c) moverlas a una vista secundaria accesible desde el dashboard pero no visible por defecto.
3. **"Variabilidad diaria":** renombrar a texto cotidiano. El baseline sugiere "Cómo te fue estos días" — es correcto. El gráfico de barras con eje positivo/negativo es aceptable en concepto pero requiere que el label no active lenguaje de scoring.
4. **Mobile ≤ 360px:** el `trendChart` debe colapsar o simplificarse a un resumen textual cuando `--trend-count` genere columnas < 24px. Opciones: limitar a los últimos 5 registros en mobile, o reemplazar el gráfico por un texto de resumen ("Últimos 10 días: 6 registros") en breakpoint ≤ 360px.
5. **`aria-label="Puntaje de humor"` / "Sin puntaje"** (`Dashboard.tsx:287-288`): esto se solapa con el punto de microcopy del baseline §1.4, pero tiene implicancia de story: el badge de "Sin puntaje" en cada entrada de la lista refuerza el lenguaje de scoring en cada fila. Reemplazar por "Sin dato" o eliminar el badge cuando sea nulo.

**Skill responsable:** `impeccable-distill` para la reestructuración narrativa del dashboard (T8/W3); `impeccable-onboard` para el estado `empty` sin métricas (T9/W3).

**file:line afectados:**
- `frontend/components/patient/dashboard/Dashboard.tsx:169-213` (estado empty — remove DashboardSummary)
- `frontend/components/patient/dashboard/Dashboard.tsx:222-228` (estado ready — DashboardSummary, considerar colapso)
- `frontend/components/patient/dashboard/Dashboard.tsx:231` (label "Variabilidad diaria")
- `frontend/components/patient/dashboard/Dashboard.tsx:287-288` (scoreBadge + "Sin puntaje")
- `frontend/components/patient/dashboard/Dashboard.module.css:170-176` (trendChart mobile)

---

## Punto 2 — TelegramPairingCard: CTAs simultáneos en estado `pairing active`

### Lectura del código

`TelegramPairingCard.tsx:272-306` en estado `pairing && !isExpired`:

```
actionGrid
  ├─ <button .secondaryBtn> "Copiar mensaje" / "Mensaje copiado"
  └─ <a .primaryBtn href={telegramStartUrl}> "Abrir Telegram"

(fuera del actionGrid, a continuación)
step marker "2" + stepTitle "Confirmá la vinculación"
<button .secondaryBtn> "Ya envié el mensaje"
```

Los tres elementos son visibles simultáneamente. La jerarquía CSS marca `.primaryBtn` para "Abrir Telegram" y `.secondaryBtn` para los otros dos, lo que técnicamente cumple el criterio de "un primario". Sin embargo, la proximidad visual entre "Copiar mensaje" y "Abrir Telegram" dentro de `actionGrid` (mismo contenedor, disposición side-by-side) los hace competir como pares de igual peso perceptual, en lugar de jerarquía primaria + silenciosa.

"Ya envié el mensaje" actúa como un tercer CTA de progresión de estado. En la estructura actual aparece **después** del marcador de paso 2 y de la instrucción, lo que significa que el paso instruccional (texto) queda atrapado entre los dos CTAs de la fase anterior y el CTA de confirmación. El ritmo de lectura es: acción → acción → instrucción → acción. El orden canónico debería ser: instrucción → acción primaria → acción secundaria → confirmación separada.

### Contraste con canon

**Canon 12 §Jerarquía de CTA:** "una acción primaria máxima + una acción secundaria clara + acciones terciarias discretas". La disposición actual tiene 2 acciones en el mismo nivel visual (primaria + secundaria en grilla horizontal) más una tercera acción del mismo nivel `.secondaryBtn` a continuación.

**Canon 12 §Formularios:** "agrupación por sentido, no por rigidez técnica". El `actionGrid` agrupa por presentación (botones de misma fila) pero mezcla dos intenciones semánticas distintas: (a) ayudar a completar el mensaje en Telegram (copiar/abrir) y (b) confirmar que ya se completó (ya envié).

**Canon 10 §5.2 capa conductual:** la persona "nunca debería preguntarse... '¿Estoy aceptando algo sin darme cuenta?'". Tres CTAs simultáneos en un flujo de vinculación técnica generan fricción de decisión innecesaria para un usuario potencialmente en vulnerabilidad.

### Veredict: `refinar`

La jerarquía visual CSS existe (primaryBtn vs secondaryBtn) pero la agrupación espacial la anula. No es un rediseño de flujo sino un reordenamiento de bloques dentro de la estructura ya existente.

**Qué cambiar:**

1. Separar en dos bloques con espacio visual claro entre ellos:
   - **Bloque A** (fase "ir a Telegram"): instrucción de código + CTA primaria "Abrir Telegram" + CTA terciaria "Copiar mensaje" (texto o botón muy silencioso, no en grilla de igual peso).
   - **Bloque B** (fase "confirmar"): separador visual o `margin-top` generoso + texto breve de orientación + botón secundario "Ya envié el mensaje".
2. "Copiar mensaje" debe descender de `.secondaryBtn` a comportamiento terciario: texto plano con subrayado, o bien botón outline de menor peso visual. No debe estar en grilla horizontal con "Abrir Telegram".
3. El marcador de paso "2" y el `stepTitle "Confirmá la vinculación"` deben preceder al botón "Ya envié el mensaje" sin quedar intercalados entre los CTAs del bloque A.

**Skill responsable:** `impeccable-polish` para el reordenamiento y la degradación visual de "Copiar mensaje" (T10/W4).

**file:line afectados:**
- `frontend/components/patient/telegram/TelegramPairingCard.tsx:274-306` (estructura actionGrid + step 2 + checkLink button)
- `frontend/components/patient/telegram/TelegramPairingCard.module.css` (eventual ajuste de `.actionGrid` para permitir apilado vertical en este contexto específico)

---

## Punto 3 — ConsentGatePanel: presencia profesional antes del control

### Lectura del código

`ConsentGatePanel.tsx:47-51`:

```tsx
{resumeInvite && (
  <p className={styles.inviteHint}>
    Recorda que viniste a traves de una invitacion de tu profesional.
  </p>
)}
```

Este bloque se renderiza **antes** del `<header>` con el título "Consentimiento informado" cuando el prop `resumeInvite={true}`. Es el primer elemento visible de la pantalla de consentimiento para usuarios que llegaron vía invitación.

Adicionalmente: el texto tiene errores ortográficos graves (baseline §1.1): "Recorda" → "Recordá", "a traves" → "a través", "invitacion" → "invitación". Esos son bugs bloqueantes per regla 9.1, pero están fuera del scope de este punto (los cubre `impeccable-clarify`).

### Contraste con canon

**Canon 10 §10.1 Ejemplos de uso:** "Si una pantalla pone primero 'Tu profesional verá tus registros', va contra el manifiesto porque instala vigilancia antes que control." El hint actual no dice exactamente eso, pero su función es equivalente: instalar la presencia profesional en el campo visual del paciente *antes* de que el paciente lea las secciones del consentimiento y antes de que ejerza su decisión de aceptar o rechazar.

**Canon 10 §3 Decisiones madre:** "Presencia profesional: condicional y explícita". El hint no es presencia del profesional en sí, pero sí recuerda al paciente que llegó a través de un tercero antes de darle el control de decidir. La asimetría de información es: el sistema sabe que hubo una invitación; el paciente recibe ese dato como primer input antes de leer qué está consintiéndose.

**Canon 10 §2 Tesis de experiencia:** "Primero protege al paciente. Después, y solo si el paciente lo decide, habilita una presencia profesional explícita."

**Canon 12 §Jerarquía visual:** "lo que debe liderar" es el contenido personal y el significado de la pantalla — en este caso, el consentimiento informado. Un hint de contexto de invitación no es contenido personal: es recordatorio de procedencia.

### Evaluación del argumento de onboarding contextual

El baseline §1.5 registra este punto como: "Mantener el hint es aceptable por contexto de onboarding, pero su tono debe revisarse junto al impeccable-clarify."

Ese argumento tiene peso funcional: el usuario que llega vía invitación puede confundirse sobre por qué está en una pantalla de consentimiento sin ningún contexto. Un hint de procedencia reduce esa confusión. El argumento es legítimo.

Sin embargo, la implementación lo contradice en dos dimensiones:

1. **Posición:** al aparecer *antes* del encabezado "Consentimiento informado", el hint precede la explicación del consentimiento mismo. El orden correcto para sostener canon 10 sería: primero el contexto de la pantalla ("Consentimiento informado"), luego la aclaración de procedencia como nota contextual subordinada, no introductoria.

2. **Tono:** "Recordá que viniste a través de una invitación de tu profesional" usa el imperativo "Recordá" que tiene una carga levemente prescriptiva. En un espacio de consentimiento informado, el tono debería ser descriptivo ("Llegaste a través de una invitación de tu profesional"), no imperativo.

**Conclusión:** el hint en sí no es eliminable en MVP porque resuelve un problema real de contexto. Pero su posición (antes del encabezado) y su tono (imperativo prescriptivo) violan el principio de "paciente primero" del canon. Es un problema de jerarquía de elementos, no de existencia del elemento.

### Veredict: `refinar`

**Qué cambiar:**

1. Mover el `inviteHint` al interior del panel *después* del `<header>` y *antes* de las secciones del consentimiento (`ConsentGatePanel.tsx:53`). El encabezado "Consentimiento informado" debe ser el primer elemento visible.
2. Cambiar el tono de imperativo a descriptivo: "Llegaste a través de una invitación de [profesional / de un profesional]." — neutral, informativo, no prescriptivo.
3. La corrección ortográfica (tildes) es prerequisito bloqueante antes de cualquier ajuste de tono (cubierto por `impeccable-clarify`).

**Skill responsable:** `impeccable-clarify` para corrección ortográfica + tono (T7/W2); el reordenamiento del elemento puede ir en `impeccable-clarify` como ajuste de posición de elemento, o en `impeccable-onboard` si se trabaja el flujo de onboarding vía invitación de forma integral.

**file:line afectados:**
- `frontend/components/patient/consent/ConsentGatePanel.tsx:47-51` (posición del inviteHint — mover a línea 57, después del `</header>`)
- `frontend/components/patient/consent/ConsentGatePanel.tsx:49` (tono del texto — cambiar imperativo por descriptivo + corrección ortográfica)

---

## Punto 4 — PatientPageShell: sin navegación formal

### Lectura del código

`PatientPageShell.tsx:40-51`:

```tsx
<main className={styles.shell}>
  <button onClick={handleLogout} className={styles.logoutButton} title="Cerrar sesión">
    Cerrar sesión
  </button>
  <div className={styles.content}>{children}</div>
</main>
```

El shell tiene exactamente un elemento de cromo: un botón "Cerrar sesión" flotante. No hay header con identificación del producto, no hay indicador de sección actual, no hay navegación entre secciones paciente.

Las secciones paciente existentes en el routing son:
- `/dashboard`
- `/registro/daily-checkin`
- `/registro/mood-entry`
- `/configuracion/telegram`
- `/configuracion/vinculos`
- `/consent`

Ninguna de estas secciones tiene un elemento de orientación estructural compartido más allá del shell.

### Contraste con canon

**Canon 12 §Navegación:** "estable, simple, predecible, sin ruido" + "pocas decisiones visibles a la vez" + "jerarquía clara entre sección actual y secciones vecinas".

Un botón "Cerrar sesión" suelto no viola la regla de "pocas decisiones" — en MVP con una sola acción de cromo, es la manifestación más minimalista posible. El problema no es exceso sino ausencia: el usuario no tiene señal de dónde está.

**Canon 12 §Composición:** "la mirada se mantiene enfocada en un eje claro". Sin indicador de ubicación, el foco del paciente depende enteramente del contenido de la página, sin apoyo estructural.

**Canon 16 §1 Shell editorial de una columna:** "eje vertical claro". El shell sí implementa la columna, pero no la ancla contextualmente.

### Evaluación de si es bloqueante en MVP

La pregunta es correcta y el baseline §7.3 la responde: "no bloqueante en MVP". Este análisis coincide. Razones:

1. El flujo principal paciente en MVP es lineal: consent → dashboard → registro. La sección de configuración de Telegram es accesible desde el banner, no desde navegación.
2. El botón "Cerrar sesión" suelto es la implementación mínima funcional. No viola ninguna regla de canon, solo deja incumplida la regla de orientación.
3. Agregar navegación formal ahora implicaría definir el mapa de secciones paciente, que probablemente crezca en waves futuras. Una nav parcial podría convertirse en deuda de mantenimiento.

**Sin embargo**, hay una dimensión emocional que el baseline subestima: para usuarios en vulnerabilidad emocional, la desorientación leve ("¿en qué sección estoy?") suma carga cognitiva innecesaria. El canon 12 §Accesibilidad señala que "mensajes y estados [deben ser] compatibles con cansancio visual o atención reducida". La falta de indicador de ubicación es compatible con MVP pero incompatible con el tono de refugio a largo plazo.

### Veredict: `mantener` (con condición explícita de crecimiento)

**Condición:** aceptable en MVP mientras el flujo paciente sea lineal con ≤ 3 secciones principales. En cuanto el flujo crezca (W5+), debe agregarse indicador de ubicación mínimo: título del producto en header + nombre de sección activa. No necesariamente navegación lateral.

**Skill responsable:** cuando corresponda ejecutar, `impeccable-polish` puede agregar el indicador de ubicación como parte del shell. No requiere wave dedicada.

**file:line afectados (cuando se ejecute):**
- `frontend/components/ui/PatientPageShell.tsx:40-51` (agregar header con wordmark + sección activa vía prop)
- `frontend/components/ui/PatientPageShell.module.css` (estilos del header)

---

## Punto 5 — OnboardingEntryHero: jerarquía de confianza y duplicación de "tranquilidad"

### Lectura del código

`OnboardingEntryHero.tsx`:

```
<div .hero>
  <header .header>
    <span .wordmark>Bitacora</span>          ← [1] identidad
  </header>
  <div .body>
    [inviteLabel? — solo en variantes invite/invite_fallback]
    <h1 .headline>Tu espacio personal de registro</h1>    ← [2] propuesta de valor
    <p .sub>                                               ← [3] sub-headline
      Un lugar tranquilo para llevar tu registro de humor y bienestar,
      con la tranquilidad de que tus datos son privados.
    </p>
    <div .ctaStack>
      <Link .primaryCta href="/ingresar">Ingresar</Link>  ← [4] CTA primaria
    </div>
    <p .privacyNote>                                       ← [5] señal de confianza
      La privacidad de tus datos es fundamental. Nadie más puede ver lo que registrás.
    </p>
  </div>
  <footer .footer>
    <p .footerText>¿Problemas para acceder?</p>           ← [6] soporte
    <a .footerLink>Contactar soporte</a>
  </footer>
</div>
```

El orden de lectura es: identidad → propuesta de valor → sub-headline → CTA → privacidad → soporte.

### Evaluación del orden contra canon 16 §10

**Canon 16 §10 Hero contextual adaptativo:** "una sola acción dominante" + "soporte de confianza subordinado a la historia principal". El patrón especifica que el soporte de confianza debe ser **subordinado**: visible pero no compitiendo con la historia principal.

El orden implementado coloca la nota de privacidad (`privacyNote`) **después** del CTA primaria. Eso la hace subordinada visualmente al flujo de acción, que es correcto. El soporte (footer) es el último elemento, también correcto.

**La jerarquía de confianza está bien ordenada.** El análisis del baseline sobre este punto es correcto: headline → CTA → privacidad → soporte respeta el patrón §10.

### Evaluación de la duplicación de "tranquilidad"

`OnboardingEntryHero.tsx:31-33`:

```
Un lugar tranquilo para llevar tu registro de humor y bienestar,
con la tranquilidad de que tus datos son privados.
```

"tranquilo" y "tranquilidad" en la misma oración de 2 cláusulas. La repetición es detectada por el baseline §1.5 como "bajo".

**Impacto en la postura de serenidad del canon:** la repetición del campo semántico no genera una sensación de serenidad sino de insistencia en la serenidad, lo cual paradójicamente contradice la serenidad auténtica. El canon 10 §4.3 arquetipo "refugio clínico sereno" implica que la calma no se declara repetidamente: se demuestra. Un diseño que necesita decir dos veces "tranquilo/tranquilidad" en una frase exhibe ansiedad editorial, no calma.

Sin embargo, el copy congelado aplica a partes específicas: "Tu espacio personal de registro" (h1) está congelado. El sub-headline `<p .sub>` NO está en la lista de copy congelado del baseline §8.1 / §10 "Riesgos, invariantes y congelados". Por tanto, el sub-headline es modificable.

**Copy congelado verificado:** la lista de congelados es:
"Ingresar", "Tu espacio personal de registro", "Empezá con tu primer registro", "Registrar humor", "Nuevo registro", "+ Nuevo registro", "Check-in diario", "Recibí recordatorios por Telegram", "Conectar", "Ahora no", "Registro guardado.", "Check-in guardado."

El texto del sub-headline y de `privacyNote` no están en esa lista.

### Veredict: `refinar`

**Qué cambiar:**

1. **Sub-headline:** eliminar la duplicación "tranquilo" / "tranquilidad". La reescritura no debe tocar el h1 congelado. Opciones de dirección (sin dictar el copy exacto — eso es W4):
   - Reemplazar la segunda instancia por el concepto de privacidad directamente: "Un lugar sereno para llevar tu registro de humor y bienestar. Tus datos son privados y solo vos los controlás."
   - O comprimir: "Un lugar para llevar tu registro de humor y bienestar, con plena privacidad de tus datos."
   - La dirección es: separar "serenidad" de "privacidad" en dos conceptos diferenciados, no usar "privacidad" como prueba de "tranquilidad".

2. **`privacyNote`:** "La privacidad de tus datos es fundamental. Nadie más puede ver lo que registrás." — este texto está bien orientado pero "es fundamental" tiene un tono levemente declarativo que roza la insistencia. El baseline lo señala como rozadura. Sin embargo, dado que el sub-headline ya cubriría privacidad tras el cambio del punto 1, la `privacyNote` podría simplificarse a una confirmación más escueta o eliminarse si el sub-headline la absorbe. Eso evita redundancia de privacidad después del CTA.

3. **Wordmark:** `OnboardingEntryHero.tsx:13` tiene "Bitacora" sin tilde (baseline §1.1 bloqueante). Debe corregirse a "Bitácora". Esto es prerequisito antes de cualquier ajuste editorial de este componente.

**Sobre la variante `invite`:** el `inviteLabel` en variantes invite/invite_fallback aparece *antes* del h1 (líneas 17-22). La misma tensión que en el punto 3 (presencia profesional antes del control del paciente) aplica aquí: el inviteLabel presenta el acompañamiento profesional antes de la propuesta de valor personal. En el hero de onboarding esto es más tolerable que en consent (porque no hay una decisión de consentimiento en juego), pero el principio de "paciente primero" sugeriría que el inviteLabel debería ser secundario al headline, no anterior. Este hallazgo no estaba en los 5 puntos originales — se documenta como hallazgo adicional.

**Skill responsable:** `impeccable-clarify` para wordmark + corrección ortográfica de "Bitacora"; `impeccable-polish` para el sub-headline (eliminación de duplicación + revisión de privacyNote).

**file:line afectados:**
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:13` (wordmark sin tilde)
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:31-33` (sub-headline con duplicación)
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:41-43` (privacyNote — revisar si absorberla en sub-headline)
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:17-22` (inviteLabel antes de h1 — hallazgo adicional no en scope original)

---

## Hallazgos adicionales detectados durante la lectura

Estos hallazgos no estaban en los 5 puntos, pero emergen del código real y contradicen o complementan el baseline:

### HA-1: Dashboard estado `empty` expone `DashboardSummary` con ceros (contradice baseline)

**Contradicción detectada.** El baseline §7.1 analiza la densidad solo en estado `ready`. El código muestra que el estado `empty` también renderiza `DashboardSummary` (`Dashboard.tsx:174-177`). Un paciente en su primera visita ve "Registros totales: 0 / Promedio de humor: — / Último registro: —" antes de haber registrado nada. Esto instala la retícula de métricas en el momento de mayor vulnerabilidad onboarding.

### HA-2: `formatMoodScore` retorna "sin puntaje" en minúsculas en `Dashboard.tsx:39`

`formatMoodScore` (línea 39) retorna `'sin puntaje'` en minúsculas. Ese string se renderiza en el `scoreBadge` (línea 288). La capitalización inconsistente con el resto de los textos de la UI no es crítica, pero es rozadura editorial. El baseline §1.4 menciona la frase en mayúsculas ("Sin puntaje") como candidata a reemplazar por "Sin dato" — el código tiene la versión en minúsculas, confirmando que el baseline lo vio con capitalización distinta. No es contradicción grave pero sí inconsistencia.

### HA-3: `TelegramPairingCard` en estado `ready` (ya vinculado) tiene estructura diferente a lo analizado en baseline

El baseline §7.2 analiza el estado `pairing active` (CTAs de vinculación). La lectura del código muestra que en estado `isLinked` (ya vinculado, líneas 322-442), la estructura es más ordenada: 1 step informativo + sección de recordatorio + acciones de recordatorio + sección "Ya quedó listo" con "Probar el bot" / "Volver al inicio" + sección de desvinculación. Este estado **no viola** el patrón de jerarquía de CTAs. El problema de CTAs simultáneos es exclusivo del estado `pairing active`, confirmando el alcance del baseline.

---

## Tabla de veredictos

| Punto | Componente | Veredict | Severidad canon | Skill responsable |
|---|---|---|---|---|
| 1 | Dashboard densidad | `rediseñar` | alta — viola canon 10 §12.2 y canon 12 §densidad | `impeccable-distill` + `impeccable-onboard` |
| 2 | TelegramPairingCard CTAs | `refinar` | media — viola canon 12 §jerarquía CTA | `impeccable-polish` |
| 3 | ConsentGatePanel inviteHint | `refinar` | media — viola canon 10 §10.1 en posición | `impeccable-clarify` + `impeccable-onboard` |
| 4 | PatientPageShell navegación | `mantener` (condicional) | baja — MVP aceptable | `impeccable-polish` (W5+) |
| 5 | OnboardingEntryHero serenidad | `refinar` | baja-media — duplicación editorial | `impeccable-clarify` + `impeccable-polish` |

**Conteo:** 0 `mantener` incondicional · 3 `refinar` · 1 `rediseñar` · 1 `mantener` condicional.

---

## Resumen por skill de waves posteriores

| Skill | Hallazgos asignados | Wave estimada |
|---|---|---|
| `impeccable-clarify` | P3/HA-2 wordmark "Bitacora" (H.5), tono imperativo ConsentGatePanel (P3), correcciones ortográficas prerequisito (P3, P5) | T7 / W2 |
| `impeccable-distill` | P1 rediseño Dashboard densidad en estado `ready`: colapso DashboardSummary, rename "Variabilidad diaria", badge "Sin puntaje" → "Sin dato" | T8 / W3 |
| `impeccable-onboard` | P1 rediseño Dashboard estado `empty` sin métricas vacías; P3 reordenamiento inviteHint después de header en ConsentGatePanel; P5 HA inviteLabel en hero invite | T9 / W3 |
| `impeccable-polish` | P2 TelegramPairingCard reordenamiento CTAs + degradación "Copiar mensaje"; P4 PatientPageShell indicador de ubicación (W5+); P5 sub-headline duplicación + privacyNote | T10 / W4 |
| `impeccable-harden` | No hallazgos nuevos en este critique. Los de baseline §6 (state coverage session_expired/error) permanecen asignados a harden | T6 / W2 |
| `impeccable-normalize` | No hallazgos nuevos en este critique. Los de baseline §4 (tokens, radios) permanecen asignados a normalize | T5 / W2 |
| `impeccable-extract` | No hallazgos nuevos. TelegramPairingCard monolítico permanece asignado a extract (baseline §3.3) | T4 / W2 |

---

## Contradicciones detectadas con el baseline

| ID | Descripción | Baseline afectado |
|---|---|---|
| CD-1 | Estado `empty` del Dashboard también expone `DashboardSummary` con ceros — el baseline §7.1 solo analiza estado `ready` | baseline §7.1 |
| CD-2 | `formatMoodScore` retorna "sin puntaje" en minúsculas; el baseline §1.4 cita "Sin puntaje" en mayúsculas | baseline §1.4 |

**Nota:** estas contradicciones están documentadas aquí. No se modifica el baseline (regla del task).

---

## Assessment

**Ready to merge:** No aplica (reporte read-only, no hay diff de código).

**Razonamiento:** Los 5 puntos evaluados confirman el diagnóstico del baseline con una diferencia crítica: el punto más grave (dashboard densidad) tiene alcance mayor al documentado — el estado `empty` también viola el canon. Los 3 `refinar` son ejecutables en waves cortas. El único `rediseñar` (Dashboard) requiere decisión de jerarquía narrativa antes de codificar, lo que lo convierte en el input principal para `impeccable-distill` y `impeccable-onboard`.
