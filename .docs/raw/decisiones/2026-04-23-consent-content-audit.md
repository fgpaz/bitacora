# Auditoría del contenido del consent — Art. 5 Ley 26.529

**Fecha:** 2026-04-23
**Tipo:** docs-only, auditoría interna
**Scope:** cobertura del contenido semántico del consent vigente (`version: 2026-04-09.v1`) contra los 6 incisos del Art. 5 Ley 26.529.
**Verdict:** `requiere-ampliación-sugerida` — 2/6 incisos bien cubiertos, 4/6 parcial o ausente. Sin red flags hard dada la naturaleza del producto (self-tracking complementario, no intervención clínica directa), pero se recomiendan secciones adicionales.

---

## 1. Contenido actual del consent

**Fuente:** `src/Bitacora.Api/appsettings.json` bloque `Consent`, servido por endpoint `GET /api/v1/consent/current`.

### Metadato

- **Version:** `2026-04-09.v1`

### ActiveText

> "Necesitamos tu consentimiento informado para guardar tu registro clínico personal, protegerlo y permitir la continuidad terapéutica autorizada."

### Sections

1. **Id:** `data-use` — **Title:** `Uso de tus datos`
   > "Tus registros se usan para tu seguimiento clínico y solo se comparten cuando vos lo autorizás."

2. **Id:** `security` — **Title:** `Protección y trazabilidad`
   > "La información se cifra, seudonimiza cuando corresponde y deja trazas de acceso para auditoría."

3. **Id:** `rights` — **Title:** `Control y revocación`
   > "Podés retirar el consentimiento y volver a otorgarlo más adelante, con impacto sobre nuevos registros y accesos futuros."

---

## 2. Norma aplicable — Art. 5 Ley 26.529

> **Art. 5° — Definición. Entiéndese por consentimiento informado la declaración de voluntad suficiente efectuada por el paciente, o por sus representantes legales en su caso, emitida luego de recibir, por parte del profesional interviniente, información clara, precisa y adecuada con respecto a:**
> **a)** Su estado de salud;
> **b)** El procedimiento propuesto, con especificación de los objetivos perseguidos;
> **c)** Los beneficios esperados del procedimiento;
> **d)** Los riesgos, molestias y efectos adversos previsibles;
> **e)** La especificación de los procedimientos alternativos y sus riesgos, beneficios y perjuicios en relación con el procedimiento propuesto;
> **f)** Las consecuencias previsibles de la no realización del procedimiento propuesto o de los alternativos especificados.

---

## 3. Adaptación del Art. 5 a Bitácora

Bitácora **no es intervención médica directa** ni procedimiento clínico. Es una **herramienta de self-tracking** que puede compartirse voluntariamente con un profesional vinculado. Por lo tanto:

- El "procedimiento" en el sentido del Art. 5 se adapta a: "acto de registrar datos de salud mental personales, almacenarlos y opcionalmente compartirlos con profesional vinculado".
- El "profesional interviniente" del Art. 5 se adapta a: sistema Bitácora como responsable del tratamiento de datos + profesional vinculado si hay invite activo.
- Los incisos clínicos (a estado de salud, b procedimiento) aplican parcialmente; los incisos operativos (c beneficios, d riesgos, e alternativas, f consecuencias) aplican más directamente.

**Premisa de la auditoría:** pedimos cobertura "informacional suficiente" de los 6 incisos con adaptación, no 1:1 clínico.

---

## 4. Cobertura por inciso

### 4.1 Inciso a) Estado de salud

**Cobertura actual:** IMPLÍCITA

El consent asume que la persona está en condición de salud mental que justifica usar Bitácora (el producto es explícitamente para pacientes), pero **no lo enuncia ni lo pregunta**.

**Análisis:** Bitácora no diagnostica ni categoriza el estado de salud del paciente. Solo registra self-reports. No requiere conocer ni afirmar el estado de salud para funcionar. Este inciso del Art. 5 aplica a intervenciones donde el profesional determina el estado; Bitácora no lo hace.

**Gap:** bajo — no requiere cobertura adicional.
**Recomendación:** ninguna.

### 4.2 Inciso b) Procedimiento propuesto + objetivos

**Cobertura actual:** PARCIAL

- **Cubierto:** `ActiveText` dice "guardar tu registro clínico personal, protegerlo y permitir la continuidad terapéutica autorizada".
- **Sección `data-use`** reitera: "registros se usan para tu seguimiento clínico y solo se comparten cuando vos lo autorizás".

**Gap:**
- No explicita que Bitácora es una **herramienta web + Telegram** con múltiples canales de registro.
- No detalla que la **continuidad terapéutica autorizada** requiere aceptar una invitación de un profesional específico.
- No aclara qué pasa durante el período sin profesional vinculado (registros siguen, pero nadie más los ve).

**Recomendación:**
- Agregar en sección `data-use` o nueva sección un texto tipo:
  > "Bitácora guarda tu registro en tu perfil personal y permite compartirlo con un profesional únicamente si aceptás su invitación. Podés registrar desde la web o desde Telegram y siempre sos vos quien decide quién ve tus datos."

### 4.3 Inciso c) Beneficios esperados

**Cobertura actual:** AUSENTE

No se enuncian beneficios esperados en ninguna sección.

**Análisis:** el Art. 5 exige que se comunique qué beneficios trae participar. En self-tracking, los beneficios son:
- Tener registro propio de tu estado emocional.
- Ayuda al profesional a acompañar con información real si vos autorizás el vínculo.
- Mejor continuidad de cuidado entre sesiones.

**Gap:** alto — debería haber sección explícita.

**Recomendación nueva sección:**
- **Id:** `benefits` — **Title:** `Para qué te sirve`
  > "Llevar tu registro te ayuda a ver cómo estuviste en distintos momentos y a compartirlo con un profesional solo si vos lo decidís. Bitácora no reemplaza tu tratamiento; lo acompaña si te sirve."

### 4.4 Inciso d) Riesgos, molestias, efectos adversos previsibles

**Cobertura actual:** AUSENTE

Solo hay sección `security` sobre cifrado/seudonimización/trazabilidad, que mitigan algunos riesgos pero no los enuncian.

**Análisis:** los riesgos razonables en un producto de self-tracking digital de datos de salud mental son:
- Brecha de seguridad de la plataforma (mitigación: cifrado + seudonimización).
- Acceso no autorizado si el dispositivo del paciente queda comprometido.
- Malinterpretación del registro por un profesional si se comparte (registros descontextualizados).
- Dependencia emocional del acto de registrar.
- Confusión si la persona registra estados extremos sin seguimiento profesional activo.

**Gap:** alto — es la ausencia más problemática desde Art. 5.

**Recomendación nueva sección:**
- **Id:** `risks` — **Title:** `Qué tener en cuenta`
  > "Los datos viajan cifrados y quedan protegidos, pero ningún sistema digital es infalible. Tu profesional solo accede si lo autorizás. Si registrás algo que te preocupa y no tenés un profesional vinculado, pedí ayuda directamente: Bitácora es una herramienta, no un canal de emergencia."

### 4.5 Inciso e) Procedimientos alternativos

**Cobertura actual:** AUSENTE

No se mencionan alternativas al uso de Bitácora.

**Análisis:** alternativas razonables:
- Llevar registro en papel / agenda personal.
- Usar otras aplicaciones de self-tracking.
- No llevar registro formal.
- Trabajar solo en consulta con tu profesional sin registro digital.

**Gap:** medio — no es común en apps incluir esto, pero el Art. 5 lo exige.

**Recomendación nueva sección:**
- **Id:** `alternatives` — **Title:** `Otras opciones`
  > "Podés llevar tu registro en papel, en otra app o no registrar nada. Bitácora es una opción más. Si no te sirve, podés borrarla: tu tratamiento no depende de esta herramienta."

### 4.6 Inciso f) Consecuencias de no realización

**Cobertura actual:** PARCIAL

- **Sección `rights`** menciona que la revocación tiene "impacto sobre nuevos registros y accesos futuros", pero no explicita qué pasa si nunca aceptás el consent desde el principio.
- Al no aceptar: no se puede registrar. No hay pérdida de otros servicios.

**Gap:** bajo — pero se puede clarificar.

**Recomendación (ampliación de `rights` o nueva):**
- **Id:** `no-consent` — **Title:** `Si elegís no seguir`
  > "Si decidís no aceptar, no vas a poder registrar en Bitácora, pero tu sesión sigue activa y podés volver cuando quieras. No perdés acceso a otros servicios ni afecta tu relación con un profesional."

---

## 5. Resumen por cobertura

| Inciso | Descripción | Cobertura actual | Gap | Prioridad |
|--------|-------------|------------------|-----|-----------|
| a) | Estado de salud | Implícita | Bajo | ignora (no aplica) |
| b) | Procedimiento + objetivos | Parcial | Medio | ampliar texto existente |
| c) | Beneficios esperados | AUSENTE | Alto | **agregar sección** |
| d) | Riesgos, molestias, efectos adversos | AUSENTE | Alto | **agregar sección** |
| e) | Procedimientos alternativos | AUSENTE | Medio | **agregar sección** |
| f) | Consecuencias de no realización | Parcial | Bajo | ampliar texto existente |

**Score de cobertura:** 2/6 bien cubiertos + 2/6 parcial + 2/6 ausentes.

---

## 6. Propuesta de consent ampliado v2026-XX-XX

Si el equipo decide cerrar los gaps, el consent ampliado quedaría con 6 secciones en lugar de 3:

```json
{
  "ActiveVersion": "2026-XX-XX.v2",
  "ActiveText": "Necesitamos tu consentimiento informado para guardar tu registro personal de humor y bienestar y, si vos lo autorizás, permitir continuidad terapéutica con un profesional vinculado.",
  "Sections": [
    { "Id": "data-use", "Title": "Uso de tus datos", "Body": "Tus registros se guardan en tu perfil y solo se comparten con un profesional si aceptás su invitación. Podés registrar desde la web o Telegram." },
    { "Id": "benefits", "Title": "Para qué te sirve", "Body": "Llevar tu registro te ayuda a ver cómo estuviste en distintos momentos y compartirlo con un profesional solo si vos lo decidís. Bitácora no reemplaza tu tratamiento; lo acompaña si te sirve." },
    { "Id": "risks", "Title": "Qué tener en cuenta", "Body": "Los datos viajan cifrados y quedan protegidos, pero ningún sistema digital es infalible. Tu profesional solo accede si lo autorizás. Si registrás algo que te preocupa y no tenés profesional vinculado, pedí ayuda directamente: Bitácora es una herramienta, no un canal de emergencia." },
    { "Id": "alternatives", "Title": "Otras opciones", "Body": "Podés llevar tu registro en papel, en otra app o no registrar nada. Bitácora es una opción más. Si no te sirve, podés borrarla: tu tratamiento no depende de esta herramienta." },
    { "Id": "security", "Title": "Protección y trazabilidad", "Body": "La información se cifra, seudonimiza cuando corresponde y deja trazas de acceso para auditoría." },
    { "Id": "rights", "Title": "Control y revocación", "Body": "Podés retirar el consentimiento y volver a otorgarlo más adelante. Si decidís no aceptar, no vas a poder registrar pero tu sesión sigue activa y no afecta otros servicios ni tu relación con un profesional." }
  ]
}
```

**Cambios requeridos para aplicar:**
1. Incrementar `ActiveVersion` a `2026-XX-XX.v2`.
2. Agregar las 3 nuevas secciones (`benefits`, `risks`, `alternatives`).
3. Actualizar secciones `data-use` y `rights` con los textos ampliados.
4. Evaluar si los consents existentes (de version `2026-04-09.v1`) requieren re-aceptación. El sistema ya detecta `CONSENT_VERSION_MISMATCH` y fuerza re-revisión; esto cubriría el escenario naturalmente.

---

## 7. Decisión operativa

Este documento es insumo para review legal formal. **No se modifica `appsettings.json` en esta sesión.**

**Razones:**
- El contenido del consent es responsabilidad del equipo clínico-legal, no UX.
- Aumentar la versión del consent fuerza re-aceptación de pacientes existentes (efecto no trivial).
- Los textos propuestos son borradores UX; el abogado puede ajustarlos.

**Acciones siguientes:**
1. Este doc + `.docs/raw/decisiones/2026-04-23-legal-review-request.md` se envían al abogado/a del equipo (W6 completa esta entrega).
2. Abogado/a firma el checklist y decide si ampliar contenido.
3. Si aprueba ampliación: sesión futura dedicada para implementar los textos en `appsettings.json` + validar con equipo clínico + incrementar versión + manejar re-aceptación de pacientes existentes.

---

## 8. Trazabilidad

- **Origen:** closure `2026-04-23-login-flow-followups-closure.md` §8 follow-up P2 #4.
- **Fuente contenido:** `src/Bitacora.Api/appsettings.json:23-43`.
- **Endpoint:** `GET /api/v1/consent/current` (`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs:18`).
- **Normativa:** Ley 26.529 Art. 5 (texto completo en §2).
- **Documento hermano:** `.docs/raw/decisiones/2026-04-23-legal-review-request.md`.

**Estado:** auditoría completada, recomendaciones documentadas. Pendiente decisión legal externa + implementación si aprobada.
