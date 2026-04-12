# Decision Operativa — Hardening Exceptions Register + Go/No-Go

**Fecha:** 2026-04-10
**Pertenece a:** Wave-prod Phase 20 (Hardening Specs + Gates)
**Cierre de tarea:** T4

---

## Contexto

`Phase 20` endurece el canon contra la verdad del repo y cierra la fase documental previa al codigo. Este documento es el registro unico de excepciones aceptadas, atajos prohibidos y gates obligatorios que las fases de codigo (`30`, `31`, `40`, `41`) y la fase de validacion final (`60`) deben satisfacer antes de avanzar.

Este registro no introduce nuevos requisitos de producto. Solo opera reglas ya existentes en los outputs congelados de T1, T2 y T3, y en los mapas de decision de `wave-prod`.

**Fuente de los gates:** T1 (compliance invariants), T2 (contract freeze), T3 (runtime/rollout gates).

---

## Excepciones aceptadas (temporales, condicionadas)

Las siguientes condiciones son aceptadas como estado transitorio del repo y no constituyen drifting. Cada una tiene un gate de resolucion propio.

| Excepcion | Razon | Resolucion condicionada |
|-----------|-------|------------------------|
| `frontend/` no existe aun en runtime web completo | Ausencia de runtime web; trabajo diferido a Phase 40 | El gate de frontend exige que `frontend/` exista y que el slice objetivo tenga `UI-RFC + HANDOFF` completo antes de escribir codigo |
| `CareLink`, `BindingCode` no materializados en `src/` | Dominio diferido a Phase 30 segun el mapa canónico | No implementar estas entidades antes de Phase 30 sin cerrar antes su contrato en `CT-VINCULOS.md` |
| `TelegramSession`, `TelegramPairingCode` ausentes del modelo fisico | Contrato diferido en `CT-TELEGRAM-RUNTIME.md` | El bot Telegram puede existir en codigo solo despues de que el contrato esté activo y el invariante no-fuga esté verificado |
| `UX-VALIDATION-*` permanece en estado preparado | Regla de Phase 60: la validacion real requiere runtime web y/o Telegram funcional | No crear evidencia de validacion ficticia; esperar a Phase 60 con runtime real |
| BindingCode TTL y expiry behavior diferidos | Especificado en `09_contratos_tecnicos.md` pero sin implementacion en `src/` | Phase 31 no puede cerrar sin que el expiry se valide contra la migracion `InitialCore` o una migracion aditiva |
| Cascadas de revocacion de consentimiento (`RF-CON-011..013`) diferidas | Requieren `CareLink` y cache profesional operativos | Estas cascadas son deferred hasta que Phase 30 implemente los invariantes de revocation completos |
| Scheduling y envio real de recordatorios Telegram (`RF-TG-010..012`) diferidos | `ReminderWorker` registrado pero `SendTelegramMessageAsync` necesita integracion completa con Bot API | Phase 31 no puede cerrar sin integracion completa de envio al Bot API |
| Export streaming para datasets grandes (`RF-EXP-003`) diferido | No existe implementacion de streaming en el runtime actual | Phase 31 difiere este requerimiento hasta que la capa de lectura este estable |
| Modulo `SEC` completo (`RF-SEC-*`) diferido | Auditoria de acceso profesional y fail-closed de lectura no completados | Phase 50-hardening es el target para esta superficie |

---

## Atajos prohibidos (sin excepcion)

| Atajo | Por que esta prohibido |
|-------|----------------------|
| Implementar `CareLink`, `BindingCode` o `TelegramSession` antes de que sus contratos en `CT-VINCULOS.md` / `CT-TELEGRAM-RUNTIME.md` esten activos | El contrato define request/response, errores y fail-closed; saltarlo rompe el invariante de compliance |
| Crear `UX-VALIDATION-*` con evidencia ficticia o basada en diseno | Viola la regla de Phase 60: la validacion requiere runtime real; evidencia simulada anula el valor del documento |
| Mover cualquier slice a `validated` antes de Phase 60 | El canon UX/UI cierra en Phase 60 con evidencia de runtime; adelantar contaminaria la trazabilidad |
| Emitir respuestas HTTP o mensajes del bot Telegram que contengan `encrypted_payload`, `safe_projection` con datos clinicos, o cualquier campo derivable de registros del paciente | Invariante T3-15 / invariante no-fuga en `09_contratos_tecnicos.md` y `TECH-ROLLOUT-Y-OPERABILIDAD.md` |
| Exponer `patient_ref` como campo persistido | Es una proyeccion opaca de API; tratarlo como columna persistente rompera el modelo de datos |
| Saltar `ConsentRequiredMiddleware` en el pipeline para cualquier endpoint de escritura clinica | Es el hard gate de consentimiento; omitirlo viola la Ley 26.529 y el modelo de consentimiento |
| INSERTar, UPDATEar o DELETEar sobre `access_audits` fuera del patron append-only | Invariante de audit en `07_baseline_tecnica.md` y `CT-AUDIT.md`; cualquier otra operacion rompe la cadena regulatoria |
| Usar `frontend/` como target de build antes de Phase 40 | El directorio no existe todavia; cualquier pipeline que intente compilarlo fallara |
| Desplegar a produccion sin que `GET /health/ready` valide todos los componentes criticos | `07_baseline_tecnica.md` define el contrato de readiness; violarlo compromete la disponibilidad |
| Alterar `DataAccess:ApplyMigrationsOnStartup` en produccion | Existencia de migraciones explicitas es un invariante; cambiar este flag rompera el deployment controlado |
| Loguear `encrypted_payload`, `safe_projection` con datos clinicos, o cualquier identificador directo del paciente | Prohibido por T3-15 y el invariante no-fuga; viola Ley 25.326 y Ley 26.657 |
| Cambiar el orden del middleware pipeline de Program.cs sin validar fail-closed | El orden actual es: RateLimiter → TraceId → ApiException → Correlate → Auth → Authz → ConsentRequired; cualquier cambio debe ser validado contra T3-10 y T3-11 |

---

## Go / No-Go antes de codigo backend (Phase 30/31)

### Go

- `T1` Cerro y las invariantes de privacidad, consentimiento, auditoria y retencion estan expresadas en `02_arquitectura.md`, `05_modelo_datos.md`, `07_baseline_tecnica.md`, `07_tech/TECH-CIFRADO.md` y `09_contratos/CT-AUDIT.md`.
- `T2` Cerro y los contratos publicos de auth, error y consentimiento estan congelados en `09_contratos_tecnicos.md`, `CT-AUTH.md`, `CT-ERRORS.md`, `CT-VINCULOS.md`, `CT-VISUALIZACION-Y-EXPORT.md` y `CT-TELEGRAM-RUNTIME.md`.
- `T3` Cerro y las reglas de runtime y rollout estan expresadas en `07_baseline_tecnica.md` y `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`.
- Los RF impacted por el codigo que se va a escribir estan sincronizados con los contratos: `RF-VIN-*`, `RF-TG-*`, `RF-VIS-*`, `RF-EXP-*` segun corresponda al modulo que se ataca.
- `mi-lsp workspace status humor --format toon` confirma que el workspace `humor` esta indexado y los simbolos del modulo objetivo son localizables.
- La superficie implementada en `src/` no se reutiliza para expandir modulos no cubiertos por contrato activo.
- Los secretos (`BITACORA_PSEUDONYM_SALT`, `BITACORA_ENCRYPTION_KEY`, `SUPABASE_JWT_SECRET`) estan disponibles en el entorno de ejecucion.

### No-Go

- Falta cualquiera de los documentos de T1, T2 o T3.
- El modulo objetivo tiene RF que no tienen contrato tecnico correspondiente en `09_contratos/` o cuyo contrato dice "canonico diferido".
- `mi-lsp` no puede localizar simbolos del modulo en el workspace `humor`.
- Se pretende implementar `CareLink`, `TelegramSession` o `BindingCode` sin que `CT-VINCULOS.md` o `CT-TELEGRAM-RUNTIME.md` haya pasado de "canonico diferido" a "activo".
- Secrets faltantes en el entorno de desarrollo.

---

## Go / No-Go antes de codigo frontend (Phase 40/41)

### Go

- `frontend/` existe en el repo y es compilable.
- El slice que se va a construir tiene `UI-RFC` completo y todos los documentos de la cadena de handoff (`HANDOFF-SPEC`, `HANDOFF-ASSETS`, `HANDOFF-MAPPING`, `HANDOFF-VISUAL-QA`) en estado existente.
- Los contratos de respuesta del backend para ese slice estan definidos en `09_contratos_tecnicos.md` y sus detail docs como "activos".
- El diseño de tokens, componentes y reglas de implementacion esta disponible en `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` para los estados de ese slice.
- `ONB-001` es el unico slice que hoy satisface todos estos requisitos; los demas slices tienen diferentes niveles de completitud en su cadena de handoff.
- Los secretos para el frontend (`NEXT_PUBLIC_API_BASE_URL`, variables de Supabase) estan configurados en el entorno.

### No-Go

- `frontend/` no existe todavia.
- El slice no tiene `UI-RFC` o alguno de los documentos de handoff faltantes.
- El contrato de respuesta del backend para ese flujo no esta en `09_contratos_tecnicos.md` como "activo" (no "canonico diferido").
- Se va a generar codigo que accede a endpoints diferidos (`VIN-*`, `VIS-*`, `EXP-001`, `TG-*`) antes de que esos endpoints esten implementados en runtime.
- `NEXT_PUBLIC_API_BASE_URL` no esta definido en `frontend/.env.local`.

---

## Go / No-Go antes de runtime hardening (Phase 50)

### Go

- Phase 30 y Phase 31 estan materializadas con smoke passing.
- Todos los endpoints listados en `09_contratos_tecnicos.md` como "implementados" estan operativos en runtime.
- El invariante no-fuga de Telegram esta implementado y verificado: ninguna respuesta del bot contiene `encrypted_payload`, `safe_projection` con datos clinicos o informacion derivable.
- Los smoke tests `infra/smoke/backend-smoke.ps1` pasan contra el runtime deployado.
- `GET /health/ready` retorna `200 OK` con estado `ready`.
- Los secretos (`BITACORA_PSEUDONYM_SALT`, `BITACORA_ENCRYPTION_KEY`, `SUPABASE_JWT_SECRET`) estan configurados en el entorno de staging o produccion.
- La migracion `InitialCore` o su equivalente aditiva fue aplicada exitosamente contra la base de datos target.
- ReminderWorker esta registrado y activo; `SendTelegramMessageAsync` hace POST real a Telegram Bot API.

### No-Go

- Phase 30 o Phase 31 no tienen smoke passing.
- Existe algun endpoint en `09_contratos_tecnicos.md` marcado como "implementado" que no esta operativo en runtime.
- El invariante no-fuga no fue verificado con prueba concreta (no basta con que el documento lo afirme).
- `GET /health/ready` retorna algo distinto de `200 OK` con `ready=true`.
- Secrets faltantes en el entorno de validacion.
- Migraciones no aplicadas o aplicadas con errores.
- `TELEGRAM_BOT_TOKEN` no esta presente en el entorno de runtime.

---

## Go / No-Go antes de validacion final (Phase 60)

### Go

- Todos los slices visibles tienen `UI-RFC` y cadena de handoff completa.
- El backend implementa la superficie completa listada en `09_contratos_tecnicos.md` como "implementada" y "canonica diferida" — ambas cubiertas por contrato activo.
- El invariante no-fuga de Telegram esta implementado y verificado con prueba concreta.
- `infra/smoke/backend-smoke.ps1` pasa contra el runtime deployado.
- `GET /health/ready` retorna `200 OK` con estado `ready`.
- Los secretos estan configurados en el entorno de staging o produccion.
- La migracion `InitialCore` o su equivalente aditiva fue aplicada exitosamente contra la base de datos target.
- Existe un plan de cuentas de test Telegram reproducibles para `TG-001` y `TG-002`.

### No-Go

- Existe algun slice visible sin `UX-VALIDATION-*` basada en evidencia real de runtime.
- El smoke test de backend falla o no puede ejecutarse.
- `GET /health/ready` retorna algo distinto de `200 OK` con `ready=true`.
- El invariante no-fuga no fue verificado con prueba concreta.
- Secrets faltantes en el entorno de validacion.
- Migraciones no aplicadas o aplicadas con errores.
- `UX-VALIDATION-*` con evidencia ficticia o basada en diseno existe en algun slice.

---

## Referencia cruzada

| Gate | Regla fuente |
|------|-------------|
| ConsentRequiredMiddleware como hard gate | T1 / `07_baseline_tecnica.md` invariante 9 |
| No-fuga Telegram | T1 / `09_contratos_tecnicos.md` invariante 4 |
| Append-only AccessAudit | T1 / `09_contratos_tecnicos.md` invariante 2 |
| PseudonymizationService fail-closed | T1 / `07_baseline_tecnica.md` invariante 11 / T3-12 |
| Encryption key fail-closed | T1 / `07_baseline_tecnica.md` invariante 10 / T3-14 |
| Rate limiting fail-closed | T3 / `07_baseline_tecnica.md` T3-RL-01 |
| Consent revocado corta recordatorio | T3 / `07_baseline_tecnica.md` T3-RL-03 |
| 1 recordatorio por paciente por dia | T3 / `07_baseline_tecnica.md` T3-RL-02 |
| Export CSV owner-only | T1 / `09_contratos_tecnicos.md` invariante 5 |
| Supresion irreversible | T1 / `07_baseline_tecnica.md` invariante 11 |
| `patient_ref` opaco, no persistido | T1 / `07_baseline_tecnica.md` invariante 7 |
| Migrations explicitas | T3 / `07_baseline_tecnica.md` invariante 8 |

---

## Vinculacion cruzada

Este documento debe estar linked desde:

- `.docs/plans/wave-prod/INDEX.md` — portafolio activo
- `.docs/wiki/07_baseline_tecnica.md` — baseline tecnico
- `.docs/wiki/09_contratos_tecnicos.md` — indice de contratos

---

*Fuente: outputs T1/T2/T3 de `.docs/plans/wave-prod/20-hardening-specs-gates/`, mapa de gaps de `.docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md` y `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`, middleware verificado en `src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs`.*
