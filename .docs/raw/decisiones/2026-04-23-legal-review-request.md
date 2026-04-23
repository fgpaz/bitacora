# Solicitud de review legal formal — R-P1-3 + contenido consent

**Fecha:** 2026-04-23
**Destinatario:** abogado/a del equipo con experiencia en regulación sanitaria argentina
**Origen:** closure `2026-04-23-login-flow-followups-closure.md` §8 follow-up P1
**Producto:** Bitácora (`bitacora.nuestrascuentitas.com`) — tool de registro personal de humor y bienestar para pacientes de salud mental.
**Scope del review:** validación formal del wording del flujo de consent y auditoría de cobertura del contenido informado contra Ley 26.529.

---

## 1. Contexto

Bitácora es un producto UI de self-tracking para pacientes de salud mental con opción de compartir con un profesional vinculado por invitación. Procesa datos sensibles bajo **Ley 25.326** (Protección de Datos Personales), **Ley 26.529** (Derechos del Paciente) y **Ley 26.657** (Salud Mental).

El 2026-04-23 se deployó un rediseño del flujo de login + dashboard que agregó un CTA secundario al consent (`"Ahora no"`), un mensaje post-decline y una nota de revocabilidad. Review interno pragmatic cerró el gap operativo pero documentó la necesidad de validación formal externa antes de expansión de producto.

Este documento consolida el pedido a abogado/a con 3 piezas:
- **Review del wording** (§3) del CTA secundario y mensajes asociados.
- **Auditoría de cobertura** (§4) del contenido del consent contra Art. 5 Ley 26.529.
- **Checklist de firma** (§6) para retornar.

---

## 2. Normas aplicables

- **Ley 25.326** — Protección de Datos Personales (datos sensibles Art. 2 + consentimiento Art. 5 + datos de salud Art. 8).
- **Ley 26.529** — Derechos del Paciente (autonomía Art. 2 inc. e + consent informado Art. 5 + forma del consent Art. 7 + revocabilidad Art. 10).
- **Ley 26.657** — Salud Mental (derecho al consent fehaciente Art. 7 inc. j + trato como sujeto Art. 7 inc. k).
- **Decreto 1089/2012** — reglamentario de Ley 26.529.

---

## 3. Review del wording de R-P1-3

### 3.1 CTA secundario de consent decline

**Archivo fuente:** `frontend/components/patient/consent/ConsentGatePanel.tsx:101-108`
**Label exacto:** `"Ahora no"`
**Comportamiento:** al click, redirige a `/?declined=1` sin borrar cookie de sesión. El paciente queda en landing con la sesión activa y puede volver a entrar al consent sin re-autenticar.

**Preguntas para abogado/a:**
1. ¿`"Ahora no"` como label de rechazo del consent cumple Art. 2 inc. e Ley 26.529 (derecho a rechazar sin expresión de causa)?
2. ¿Debería ser `"Rechazar"` o `"No acepto"` más explícito legalmente? ¿O `"Ahora no"` es preferible por sereno/no-estigmatizante?
3. ¿Mantener la sesión activa post-decline (sin borrar cookie) cumple Art. 7 Ley 26.529 sobre forma del consent? ¿O requerimos re-autenticación para considerarse decline "efectivo"?

### 3.2 Mensaje post-decline en landing

**Archivo fuente:** `frontend/app/page.tsx:21-24`
**Texto exacto:** `"Podés aceptar cuando quieras. Tu sesión sigue activa."`
**Accesibilidad:** `role="status"` con `aria-live="polite"`.

**Preguntas:**
4. ¿El mensaje cumple la reversibilidad del Art. 10 Ley 26.529 al afirmar "podés aceptar cuando quieras"?
5. ¿"Tu sesión sigue activa" es ambiguo desde perspectiva de minimización de datos (Ley 25.326 Art. 4)? ¿Requiere explicitar que no se guardó dato alguno?

### 3.3 Nota de revocabilidad

**Archivo fuente:** `frontend/components/patient/consent/ConsentGatePanel.tsx:97-99`
**Texto exacto:** `"Podés revocarlo cuando quieras desde Mi cuenta."`

**Preguntas:**
6. ¿La promesa `"desde Mi cuenta"` es suficiente UI cumpliendo Art. 10 Ley 26.529 cuando hoy el item `"Consentimiento"` en el menú "Mi cuenta" existe como slice pendiente? **Nota:** al momento del review, la UI dedicada de revocación se implementa en esta misma sesión (wave W7). El backend ya soporta la revocación via `DELETE /api/v1/consent/current`.
7. ¿Necesitamos agregar el canal alternativo de revocación (email a soporte) como backup dentro del texto, o la ruta UI es suficiente?

---

## 4. Auditoría de cobertura del contenido del consent contra Art. 5 Ley 26.529

Ver documento hermano `.docs/raw/decisiones/2026-04-23-consent-content-audit.md` con el análisis completo de los 6 incisos del Art. 5 contra el contenido actual del consent (version `2026-04-09.v1`).

**Resumen ejecutivo:**
- Cobertura de Art. 5: 2/6 incisos bien cubiertos, 4/6 parcial o ausente.
- Gap más relevante: Art. 5 inc. c) beneficios esperados, inc. d) riesgos/efectos adversos, inc. e) procedimientos alternativos, inc. f) consecuencias de no realización.
- Mitigación: Bitácora NO es intervención clínica stricto sensu; es herramienta complementaria de self-tracking. Art. 5 aplica con adaptación pero no 1:1.

**Preguntas para abogado/a:**
8. ¿La adaptación del Art. 5 a un producto de self-tracking (no intervención médica directa) es defendible legalmente? ¿O cada inciso debe estar cubierto textualmente?
9. Si requerimos cobertura completa: ¿pueden sugerirse textos para los 4 incisos faltantes que mantengan el tono sereno canon 13 sin caer en jerga legal?
10. ¿La versión del consent (`2026-04-09.v1`) requiere ser incrementada tras agregar los incisos faltantes, incluso si solo se agrega texto? ¿Qué implica para los consents ya firmados por pacientes existentes? ¿Requiere re-aceptación?

---

## 5. Cobertura adicional — Ley 26.657

Bitácora es un producto en el dominio de salud mental. Art. 7 inc. j Ley 26.657:
> el usuario tiene derecho a no ser objeto de investigaciones clínicas ni tratamientos experimentales sin un consentimiento fehaciente.

**Preguntas:**
11. ¿El producto califica como "tratamiento" bajo Ley 26.657, o como "herramienta de apoyo" fuera del scope? Esto afecta la rigurosidad del consent requerido.
12. Art. 7 inc. k trato como sujeto, no objeto: ¿el wording cumple o tiene residuo paternalista?

---

## 6. Checklist de firma del abogado/a

Al terminar el review, firmar a continuación de cada ítem:

- [ ] Wording de `"Ahora no"` aprobado / requiere cambios: `____________`
- [ ] Wording de mensaje post-decline aprobado / requiere cambios: `____________`
- [ ] Wording de revocationNote aprobado / requiere cambios: `____________`
- [ ] Cobertura de Art. 5 Ley 26.529 defendible como está / requiere textos adicionales sugeridos: `____________`
- [ ] Ley 26.657 Art. 7 inc. j cumplido como está / requiere ajuste: `____________`
- [ ] Version del consent debe incrementarse: sí / no
- [ ] Consents existentes requieren re-aceptación: sí / no
- [ ] Observaciones adicionales: `____________`

**Firma:** `____________`
**Fecha:** `____________`
**Matrícula profesional:** `____________`

---

## 7. Cómo enviar este documento al abogado

1. Compartir este doc + `.docs/raw/decisiones/2026-04-23-consent-content-audit.md` + `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md` (review interno previo) por canal seguro.
2. Agendar reunión de 30min para discutir las 12 preguntas.
3. Recibir checklist firmado.
4. Si hay cambios: abrir tarea en el backlog con wave code + legal-ratification y ejecutar antes de próxima expansión regulatoria (otras jurisdicciones, GDPR/HIPAA/LGPD).
5. Archivar la respuesta firmada en `.docs/raw/decisiones/` como `2026-XX-XX-legal-review-signed.md`.

---

## 8. Trazabilidad

- Closure previo: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` §6.1.
- Closure followups: `.docs/raw/reports/2026-04-23-login-flow-followups-closure.md` §2 follow-up #1.
- Review interno pragmatic: `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md`.
- Auditoría Art. 5: `.docs/raw/decisiones/2026-04-23-consent-content-audit.md`.

**Estado:** template preparado. Pendiente envío a abogado/a humano.
