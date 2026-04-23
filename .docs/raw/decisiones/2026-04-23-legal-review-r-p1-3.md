# Legal-review R-P1-3 — wording consent decline + revocationNote

**Fecha:** 2026-04-23
**Rama:** `feature/login-flow-followups-2026-04-23`
**Tipo de revisión:** docs-only, pragmatic legal-review interno sin abogado formal
**Scope:** wording introducido por R-P1-3 en el rediseño del login flow 2026-04-23, ya merged a `main` (commit `5d91158`) y deployado.
**Normativa de referencia:** Ley 26.529 (Derechos del Paciente) + Ley 25.326 (Protección de Datos Personales) + Ley 26.657 (Salud Mental).
**Verdict:** `resuelto-sin-cambios-pending-formal-legal-opinion`.

---

## 1. Motivación

El closure `2026-04-23-login-flow-redesign-closure.md` §6.1 dejó R-P1-3 como follow-up bloqueante de compliance hasta validación legal. El humano autorizó realizar el review interno con recomendación explícita de no sustituir una validación formal externa. Este documento deja la decisión operativa trazable y documenta el análisis hecho contra los tres artículos relevantes.

## 2. Wording bajo revisión (literal)

### 2.1 CTA secundario de consent decline

**Archivo:** `frontend/components/patient/consent/ConsentGatePanel.tsx:101-108`

```tsx
<button
  type="button"
  className={styles.declineBtn}
  onClick={handleDecline}
  disabled={submitting}
>
  Ahora no
</button>
```

Handler `handleDecline` (líneas 44-48): `router.push('/?declined=1')` sin borrar cookie de sesión.

### 2.2 Mensaje post-decline en landing

**Archivo:** `frontend/app/page.tsx:21-24`

```tsx
const variant = declined ? 'standard' : hasSession ? 'returning' : 'standard';
const message = declined
  ? 'Podés aceptar cuando quieras. Tu sesión sigue activa.'
  : undefined;
```

Renderizado en `OnboardingEntryHero.tsx:67-71`:

```tsx
{message && (
  <p className={styles.heroMessage} role="status" aria-live="polite">
    {message}
  </p>
)}
```

### 2.3 Nota de revocabilidad en consent

**Archivo:** `frontend/components/patient/consent/ConsentGatePanel.tsx:97-99`

```tsx
<p className={styles.revocationNote}>
  Podés revocarlo cuando quieras desde Mi cuenta.
</p>
```

Consistencia UI: `ShellMenu` default label = `"Mi cuenta"` (`ShellMenu.tsx:20`). La ruta declarada existe conceptualmente como entry point del menú, pero no hay una página dedicada de revocación de consent como slice autónomo (ver §4 Gap observado).

---

## 3. Análisis por norma

### 3.1 Ley 26.529 — Derechos del Paciente

**Art. 2 inc. e) Autonomía de la voluntad:**
> El paciente tiene derecho a aceptar o rechazar determinadas terapias o procedimientos médicos o biológicos, con o sin expresión de causa, como así también a revocar posteriormente su manifestación de la voluntad.

**Evaluación del wording:**
- CTA `"Ahora no"`: ofrece salida expresa sin exigir expresión de causa. **Cumple.**
- Sin coacción implícita: no hay timeout contado, no hay bloqueos de navegación, no hay warnings amenazantes. **Cumple.**
- Mensaje post-decline `"Podés aceptar cuando quieras. Tu sesión sigue activa."`: reafirma la reversibilidad inmediata y la persistencia de la sesión sin re-autenticar. **Cumple.**

**Observación:** el hard gate funcional del RF-CON-003 (sin consent no hay registro de mood) se preserva después del decline — la sesión sigue activa pero el paciente no puede acceder al registro hasta aceptar. Esto NO viola Art. 2 inc. e: la persona mantiene la capacidad libre de aceptar o rechazar, y el rechazo no conlleva sanción ni pérdida de estado técnico. Solo limita el acceso al servicio que depende del consent.

**Art. 5º Definición consent informado:**
> Declaración de voluntad (...) emitida luego de recibir por parte del profesional actuante, información clara, precisa y adecuada con respecto a (...) su propósito (...) beneficios esperados (...) riesgos, molestias y efectos adversos previsibles (...) la especificación de los procedimientos alternativos (...) y las consecuencias previsibles de la no realización (...).

**Evaluación:** Art. 5 exige contenido del consent, no wording del CTA de rechazo. El contenido del consent se entrega vía `consent.sections` (dinámicas desde API `/api/v1/consent/current`). Este review no audita el contenido del consent sino solo los CTAs de decisión. **Fuera de scope inmediato.** (Auditar contenido del consent es trabajo aparte.)

**Art. 10º Revocabilidad:**
> La decisión del paciente en cuanto a consentir o rechazar las terapias o procedimientos médicos o biológicos puede ser revocada.

**Evaluación del wording:**
- revocationNote `"Podés revocarlo cuando quieras desde Mi cuenta."`: explicita la revocabilidad e indica dónde ejecutarla. **Cumple en principio.**
- Riesgo residual: la promesa `"desde Mi cuenta"` apunta a `ShellMenu` (trigger `"Mi cuenta"`), cuyos items actuales son `Recordatorios`, `Vínculos` y `Cerrar sesión`. **No existe un item específico de revocación de consent** (el slice `CON-002` del canon 23_uxui está listo a nivel UI-RFC + HANDOFF pero la implementación frontend aún no está materializada). Este gap NO invalida la promesa verbal (la revocación backend existe y es accesible via UI cuando se implemente CON-002), pero sí crea una expectativa UI no cumplida al 2026-04-23.

**Recomendación operativa:** agregar en el closure follow-up explícito para implementar la ruta UI de revocación (slice `CON-002`) antes del próximo release mayor. No bloquea este merge.

### 3.2 Ley 25.326 — Protección de Datos Personales

**Art. 5º Consentimiento:** exige consent previo, expreso, informado y por escrito.

**Evaluación:**
- El flow de consent se ejecuta antes de cualquier recolección de datos sensibles de salud mental (Art. 7º § datos sensibles). **Cumple.**
- El hard gate funcional preservado garantiza que no hay registro de datos sin consent. **Cumple.**
- El CTA secundario `"Ahora no"` no afecta el cumplimiento del Art. 5: si el paciente declina, no se recoge dato alguno.

**Art. 6º Información previa:** sin cambios por R-P1-3 (el contenido del consent sigue intacto).

**Art. 11º Cesión:** sin cambios por R-P1-3. La cesión a profesional vinculado se mantiene como flow separado (slice `VIN-*`) con sus propios controles.

### 3.3 Ley 26.657 — Salud Mental

Art. 7º inc. j) derecho del usuario al consentimiento informado con posibilidad de rechazo:
> (...) su derecho a no ser objeto de investigaciones clínicas ni tratamientos experimentales sin un consentimiento fehaciente (...) y con pleno conocimiento de su alcance.

**Evaluación:** el wording `"Ahora no"` + `"Podés aceptar cuando quieras"` + revocationNote son coherentes con el espíritu del Art. 7 inc. j: derecho a no ser tratado sin consent fehaciente y posibilidad real de no aceptar. **Cumple.**

Art. 7º inc. k) derecho a que en el tratamiento no sea considerado un objeto pasivo:
- El framing sereno, sin dramatizar, sin infantilizar, sostiene este derecho. **Cumple.**

---

## 4. Red flags detectados

**Ninguno hard.** El wording sostiene los principios de autonomía, reversibilidad, consent informado y revocabilidad.

### Red flags soft (observaciones)

1. **Promesa UI de revocación sin destino concreto.** `"Podés revocarlo cuando quieras desde Mi cuenta."` dirige a `ShellMenu`, pero el item de revocación no existe como UI dedicada. Mitigación: implementar slice `CON-002` (ya prepared a nivel handoff en `.docs/wiki/23_uxui/UJ/UJ-CON-002.md` y relacionados) antes del próximo release mayor. No bloquea este merge porque la promesa backend sí está implementada.

2. **Ausencia de confirmación post-decline.** Al clickear `"Ahora no"`, la redirección es inmediata sin diálogo de confirmación. Esto ES correcto por diseño — agregar un diálogo de confirmación al decline sería paternalista y violaría el espíritu del Art. 2 inc. e (autonomía sin coacción). El mensaje post-decline en landing cumple el rol de feedback. **Mantener como está.**

3. **"Mi cuenta" como label con alcance limitado.** Hoy `ShellMenu` muestra solo `Recordatorios`, `Vínculos`, `Cerrar sesión`. Si un paciente lee la revocationNote y navega a "Mi cuenta" esperando ver una opción de "Revocar consentimiento" y no la encuentra, se genera fricción. Mitigación: agregar al menos un tooltip o sub-item `"Revocar consentimiento"` (item disabled con `title="Próximamente"`) o un redirect a un email de soporte de revocación, como puente hasta que CON-002 esté materializado. **Recomendación pero no bloqueante.**

---

## 5. Recomendación de validación formal

Este review es pragmatic y fue autorizado por el humano bajo la instrucción `"hacer vos review"`. **NO sustituye una validación formal de abogado/a con experiencia en regulación sanitaria argentina.**

**Pasos siguientes recomendados (fuera del scope de esta sesión):**

1. **Enviar este documento + los 3 bloques de wording a un/a abogado/a del equipo para validación formal** antes de cualquier campaña de captación masiva o expansión del producto a otro contexto regulatorio.
2. **Considerar que Ley 26.529 y Ley 26.657** son de aplicación nacional argentina. Si el producto se abre a otras jurisdicciones (por ejemplo, provincias con reglamentación adicional o terceros países bajo GDPR/HIPAA/LGPD), este review debe re-ejecutarse con normativa específica.
3. **Agregar en el backlog** el slice UI de revocación (`CON-002`) como prioridad siguiente del roadmap. La promesa UI `"desde Mi cuenta"` debe tener contraparte real antes del próximo release mayor.
4. **Auditar el contenido semántico del consent** (secciones dinámicas desde API `/api/v1/consent/current`) contra el Art. 5º Ley 26.529 como trabajo separado.

---

## 6. Decisión operativa

**R-P1-3 se cierra como `resuelto-sin-cambios-pending-formal-legal-opinion`.**

- No se modifica el código de `ConsentGatePanel.tsx` ni `app/page.tsx` ni `OnboardingEntryHero.tsx` en esta wave.
- El wording queda deployado en producción (commit `5d91158` ya mergeado al 2026-04-23).
- El closure update `2026-04-23-login-flow-followups-closure.md` documenta este verdict + los 3 red flags soft como follow-ups remanentes.
- La validación formal externa queda como `P1 post-merge` en el backlog del proyecto.

---

## 7. Trazabilidad

- Commit del wording original: `3035ebc` (W2 del rediseño 2026-04-23).
- Documento fuente: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` §6.1.
- Audit origen: `.docs/raw/reports/2026-04-23-login-flow-audit.md` E2-F2 (P1 `⚠ legal-review`).
- Canon 23_uxui deltas 2026-04-23: UXS-ONB-001, VOICE-ONB-001, UJ-ONB-001 (en esta misma wave W1).

---

**Estado:** decisión legal pragmatic documentada. Validación formal externa pendiente.
**Siguiente acción:** enviar a abogado/a del equipo cuando esté disponible.
