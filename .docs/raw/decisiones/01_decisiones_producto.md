# Decisiones de Producto — Brainstorming T1

**Fecha:** 2026-04-08
**Contexto:** Decisiones de producto para el MVP de Bitacora, resueltas via brainstorming interactivo.

---

## Decisiones Cerradas Previamente (confirmadas)

| # | Decision | Valor | Justificacion |
|---|----------|-------|---------------|
| P0 | Nombre de trabajo | Bitacora | Metafora apropiada, reconocible en espanol |
| P1 | Escala de humor | -3..0..+3 (7 niveles) | Simplificacion valida del NIMH-LCM-p, elimina nivel severo que requiere evaluacion clinica |
| P2 | Onboarding MVP | Hibrido (profesional crea O paciente se auto-registra) | Soporta ambos flujos de ingreso |
| P3 | Scope MVP | Vertical-first, mood tracking | Form builder y tests clinicos diferidos a Fase 2 |
| P4 | Acceso profesional | Vinculo persistente + consentimiento revocable + auditoria | Mecanismo principal del MVP |
| P5 | Telegram MVP | Carga rapida + recordatorios | Gestion principal en web |
| P6 | Tracking diario | Sueno/actividad/ansiedad/irritabilidad/medicacion 1x/dia; humor 1+ por dia | Basado en NIMH-LCM-p adaptado |
| P7 | Fuera de MVP | Form builder, tests clinicos, IA, app nativa, multi-tedi operativo | Roadmap explicito |

## Decisiones Resueltas en T1

### D1: Codigo temporal para acceso one-off del profesional

**Decision:** Solo vinculo persistente en MVP. Codigo temporal queda para Roadmap.

**Opciones evaluadas:**
- (A) Solo vinculo persistente — **elegida**
- (B) Vinculo + codigo temporal
- (C) Solo codigo temporal

**Justificacion:** Menor superficie de ataque, 100% auditable, cumple Ley 25.326 sin ruta publica con datos sensibles. Las interconsultas se resuelven agregando otro profesional al vinculo. El codigo temporal agrega complejidad (endpoint publico, TTL, riesgo de sharing) sin valor suficiente para MVP.

---

### D2: Export de datos del paciente

**Decision:** CSV basico en MVP. PDF con graficos en Roadmap.

**Opciones evaluadas:**
- (A) CSV en MVP, PDF en Roadmap — **elegida**
- (B) CSV + PDF ambos en MVP
- (C) Todo export en Roadmap

**Justificacion:** CSV cumple portabilidad legal (Ley 25.326, derecho de acceso del titular) con esfuerzo minimo (1 endpoint). PDF con timeline longitudinal es el artefacto estrella pero requiere rendering server-side — se prioriza para Roadmap sin comprometer compliance.

---

### D3: Nombre del producto

**Decision:** "Bitacora" es el nombre user-facing definitivo.

**Opciones evaluadas:**
- (A) Bitacora como nombre definitivo — **elegida**
- (B) Bitacora como code name, nombre final TBD
- (C) Otro nombre

**Justificacion:** Palabra reconocida en espanol, metafora apropiada para diario/registro de salud. Funciona bien en URL (bitacora.nuestrascuentitas.com), bot de Telegram, y UI. Permite construir identidad de marca desde el MVP.

---

### D4: Consentimiento informado — timing

**Decision:** Hard gate obligatorio antes del primer registro de datos.

**Opciones evaluadas:**
- (A) Hard gate obligatorio — **elegida**
- (B) Soft gate diferido
- (C) Doble gate progresivo

**Justificacion:** La Ley 25.326 requiere consentimiento "previo" para datos sensibles. La Ley 26.657 refuerza proteccion especial para datos de salud mental. Un hard gate es la postura juridicamente mas segura: ningun dato de humor se almacena hasta que el paciente acepta el consentimiento informado. La friccion adicional en onboarding es aceptable dado el contexto clinico.

---

## Resumen de Todas las Decisiones de Producto (MVP)

| ID | Decision | Valor MVP |
|----|----------|-----------|
| P0 | Nombre | Bitacora (user-facing definitivo) |
| P1 | Escala | -3..0..+3 |
| P2 | Onboarding | Hibrido |
| P3 | Scope | Vertical mood tracking |
| P4 | Acceso profesional | Vinculo persistente solamente |
| P5 | Telegram | Carga rapida + recordatorios |
| P6 | Tracking | Sueno/actividad/ansiedad/irritabilidad/medicacion 1x/dia + humor N/dia |
| P7 | Fuera de MVP | Form builder, tests, IA, app nativa, multi-tedi |
| D1 | Codigo temporal | Roadmap (no MVP) |
| D2 | Export | CSV en MVP, PDF en Roadmap |
| D3 | Nombre | Bitacora (definitivo) |
| D4 | Consentimiento | Hard gate antes del primer registro |
