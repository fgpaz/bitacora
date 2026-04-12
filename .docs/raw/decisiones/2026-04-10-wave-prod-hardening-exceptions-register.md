# Decisión Operativa — Hardening Exceptions Register + Go/No-Go

**Fecha:** 2026-04-10
**Pertenece a:** Wave-prod Phase 20 (Hardening Specs + Gates)
**Cierre de tarea:** T4

---

## Contexto

`Phase 20` endurece el canon contra la verdad del repo y cierra la fase documental previa al código. Este documento es el registro único de excepciones aceptadas, atajos prohibidos y gates obligatorios que las fases de código (`30`, `31`, `40`, `41`) y la fase de validación final (`60`) deben satisfacer antes de avançar.

Este registro no introduce nuevos requisitos de producto. Solo opera reglas ya existentes en los outputs congelados de T1, T2 y T3, y en los mapas de decisión de `wave-prod`.

---

## Excepciones aceptadas (temporales, condicionadas)

Las siguientes condiciones son承认das como estado transitorio del repo y no constituyen drifting. Cada una tiene un gate de resolución propio.

| Excepción | Razón | Resolución condicionada |
|-----------|-------|------------------------|
| `frontend/` no existe en el repo | Ausencia de runtime web; trabajo diferido a Phase 40 | El gate de frontend exige que `frontend/` exista y que `ONB-001` tenga `UI-RFC + HANDOFF` completo antes de escribir código |
| `CareLink`, `BindingCode`, `TelegramSession` no materializados en `src/` | Dominio diferido a Phase 30/31 según el mapa canónico | No implementar这些 entidades antes de Phase 30 sin cerrar antes su contrato en `CT-VINCULOS.md` y `CT-TELEGRAM-RUNTIME.md` |
| Solo `ONB-001` tiene cadena completa `UI-RFC + HANDOFF` | Los slices `REG-001`, `REG-002` necesitan cierre pre-código; `VIN-*`, `VIS-*`, `EXP-001`, `CON-002`, `TG-001`, `TG-002` todavía carecen de `UI-RFC` | Ningún slice distinto de `ONB-001` puede avanzar a código hasta que su `UI-RFC` y su cadena de handoff estén completos |
| `UX-VALIDATION-*` permanece en estado preparado | Regla de Phase 60: la validación real requiere runtime web y/o Telegram funcional | No crear evidencia de validación ficticia; esperar a Phase 60 con runtime real |
| `TelegramSession` y `TelegramPairingCode` ausentes del modelo físico | Contrato diferido en `09_contratos/CT-TELEGRAM-RUNTIME.md` | El bot Telegram puede existir en código solo después de que el contrato esté activo y el invariante no-fuga esté verificado |
| BindingCode TTL y expiry behavior diferidos | Especificado en `09_contratos_tecnicos.md` pero sin implementación en `src/` | Phase 31 no puede cerrar sin que el expiry se valide contra la migración `InitialCore` o una migración aditiva |

---

## Atajos prohibidos (sin excepción)

| Atajo | Por qué está prohibido |
|-------|----------------------|
| Implementar `CareLink`, `BindingCode` o `TelegramSession` antes de que sus contratos en `CT-VINCULOS.md` / `CT-TELEGRAM-RUNTIME.md` estén activos | El contrato define request/response, errores y fail-closed; saltarlo rompe el invariante de compliance |
| Crear `UX-VALIDATION-*` con evidencia ficticia o basada en diseño | Viola la regla de Phase 60: la validación requiere runtime real; evidencia simulada anula el valor del documento |
| Mover cualquier slice a `validated` antes de Phase 60 | El canon UX/UI cierra en Phase 60 con evidencia de runtime; adelantar contaminaría la trazabilidad |
| Emitir respuestas HTTP o mensajes del bot Telegram que contengan `encrypted_payload`, `safe_projection` con datos clínicos, o cualquier campo derivable de registros del paciente | Invariante T3-15 / `no-fuga` en `09_contratos_tecnicos.md` y `TECH-ROLLOUT-Y-OPERABILIDAD.md` |
| Exponer `patient_ref` como campo persistido | Es una proyección opaca de API; tratarlo como columna persistente rompería el modelo de datos |
| Saltar `ConsentRequiredMiddleware` en el pipeline para cualquier endpoint de escritura clínica | Es el hard gate de consentimiento; omitirlo viola la Ley 26.529 y el modelo de consentimiento |
| INSERTar, UPDATEar o DELETEar sobre `access_audits` fuera del patrón append-only | Invariante de audit en `07_baseline_tecnica.md` y `CT-AUDIT.md`; cualquier otra operación rompe la cadena regulatoria |
| Usar `frontend/` como target de build antes de Phase 40 | El directorio no existe; cualquier pipeline que intente compilarlo fallará; además, el gate de readiness de frontend exige `ONB-001` completo |
| Desplegar a producción sin que `GET /health/ready` valide todos los componentes críticos | `07_baseline_tecnica.md` define el contract de readiness; violarlo compromete la disponibilidad |
| Alterar `DataAccess:ApplyMigrationsOnStartup` en producción | Existencia de migraciones explícitas es un invariante; cambiar este flag rompería el deployment controlado |

---

## Go / No-Go antes de código backend

### Go

- `T1` cerró y las invariantes de privacidad, consentimiento, auditoría y retención están expresadas en `02_arquitectura.md`, `05_modelo_datos.md`, `07_baseline_tecnica.md`, `07_tech/TECH-CIFRADO.md` y `09_contratos/CT-AUDIT.md`.
- `T2` cerró y los contratos públicos de auth, error y consentimiento están congelados en `09_contratos_tecnicos.md`, `CT-AUTH.md`, `CT-ERRORS.md`, `CT-VINCULOS.md`, `CT-VISUALIZACION-Y-EXPORT.md` y `CT-TELEGRAM-RUNTIME.md`.
- `T3` cerró y las reglas de runtime y rollout están expresadas en `07_baseline_tecnica.md` y `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`.
- Los RF impacted por el código que se va a escribir están sincronizados con los contratos: `RF-VIN-*`, `RF-TG-*`, `RF-VIS-*`, `RF-EXP-*` según corresponda al módulo que se ataca.
- `mi-lsp workspace status humor --format toon` confirma que el workspace `humor` está indexado y los símbolos del módulo objetivo son localizables.
- La поверхность implementada en `src/` no se reutiliza para expandir módulos no cubiertos por contrato activo.

### No-Go

- Falta cualquiera de los documentos de T1, T2 o T3.
- El módulo objetivo tiene RF que no tienen contrato técnico correspondiente en `09_contratos/` o cuyo contrato dice "canónico diferido".
- `mi-lsp` no puede localizar símbolos del módulo en el workspace `humor`.
- Se pretende implementar `CareLink`, `TelegramSession` o `BindingCode` sin que `CT-VINCULOS.md` o `CT-TELEGRAM-RUNTIME.md` haya pasado de "canónico diferido" a "activo".

---

## Go / No-Go antes de código frontend

### Go

- `frontend/` existe en el repo y es compilable.
- El slice que se va a construir tiene `UI-RFC` completo y todos los documentos de la cadena de handoff (`HANDOFF-SPEC`, `HANDOFF-ASSETS`, `HANDOFF-MAPPING`, `HANDOFF-VISUAL-QA`) en estado existente.
- Los contratos de respuesta del backend para ese slice están definidos en `09_contratos_tecnicos.md` y sus detail docs.
- El diseño de tokens, componentes y reglas de implementación está disponible en `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` para los estados de ese slice.
- `ONB-001` es el único slice que hoy satisface estos requisitos; `REG-001` y `REG-002` están bloqueados hasta que su cierre pre-código coincida con el nivel de `ONB-001`.

### No-Go

- `frontend/` no existe todavía.
- El slice no tiene `UI-RFC` o alguno de los documentos de handoff faltantes.
- El contrato de respuesta del backend para ese flujo no está en `09_contratos_tecnicos.md` como "activo" (no "canónico diferido").
- Se va a generar código que accede a endpoints diferidos (`VIN-*`, `VIS-*`, `EXP-001`, `TG-*`) antes de que esos endpoints estén implementados en runtime.

---

## Go / No-Go antes de validación final

### Go

- Todos los slices visibles tienen `UI-RFC` y cadena de handoff completa.
- El backend implementa la superficie completa listada en `09_contratos_tecnicos.md` como "implementada" y "canónica diferida" — ambas cubiertas por contrato activo.
- El invariante no-fuga de Telegram está implementado y verificado: ninguna respuesta del bot contiene `encrypted_payload`, `safe_projection` con datos clínicos o información derivable.
- `infra/smoke/backend-smoke.ps1` pasa contra el runtime deployado.
- `GET /health/ready` retorna `200 OK` con estado `ready`.
- Los secretos (`BITACORA_PSEUDONYM_SALT`, `BITACORA_ENCRYPTION_KEY`, `SUPABASE_JWT_SECRET`) están configurados en el entorno de staging o producción.
- La migración `InitialCore` o su equivalente aditiva fue aplicada exitosamente contra la base de datos target.

### No-Go

- Existe algún slice visible sin `UX-VALIDATION-*` basada en evidencia real de runtime.
- El smoke test de backend falla o no puede ejecutarse.
- `GET /health/ready` retorna algo distinto de `200 OK` con `ready=true`.
- El invariante no-fuga no fue verificado con prueba concreta (no basta con que el documento lo afirme).
- Secrets faltantes en el entorno de validación.
- Migraciones no aplicadas o aplicadas con errores.

---

## Vinculación cruzada

Este documento debe estar linked desde:

- `.docs/plans/wave-prod/INDEX.md` — portafolio activo
- `.docs/wiki/07_baseline_tecnica.md` — baseline técnico
- `.docs/wiki/09_contratos_tecnicos.md` — índice de contratos

---

*Fuente: outputs T1/T2/T3 de `.docs/plans/wave-prod/20-hardening-specs-gates/`, mapa de gaps de `.docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md` y `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`, middleware verificado en `src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs`.*
