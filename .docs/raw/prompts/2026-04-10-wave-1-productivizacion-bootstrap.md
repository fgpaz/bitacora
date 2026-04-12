<!-- target: codex | pressure: aggressive | generated: 2026-04-10 -->

# Wave 1 de productivización — Prompt de arranque

```text
Iniciá una nueva sesión de Codex para crear la cartera completa de planes que lleve Bitácora desde su estado actual hasta un estado “productivo”.

Esta es una sesión de planificación, no de implementación.

Usá:
- $ps-contexto — obligatorio como primer paso
- $mi-lsp — obligatorio para explorar código bajo src/
- $brainstorming — obligatorio antes de planificar
- $ps-asistente-wiki — para verificar si queda algún gate documental real antes de planificar
- $writing-plans — para persistir la wave y los subplanes en disco
- $ps-trazabilidad — obligatorio para cerrar la tarea
- $ps-auditar-trazabilidad — obligatorio porque esta tarea es grande, riesgosa y multi-módulo

No confíes ciegamente en este prompt. Verificá el repo real primero y nombrá cualquier contradicción.

Misión exacta
- Crear una wave completa de planes futuros bajo `.docs/raw/plans/` para dejar Bitácora encaminado a productivo.
- El usuario pidió explícitamente formato `wave-1/` con todos los planes a seguir adentro.
- Si el convenio normal de `writing-plans` entra en tensión con ese pedido, conservá ambas cosas:
  - un plan índice date-prefixed en `.docs/raw/plans/2026-04-10-wave-1-productivizacion.md`
  - una carpeta operativa `.docs/raw/plans/wave-1/` con dentro todos los planes downstream que sigan desde acá
- No implementes código en esta sesión. Solo dejá la wave y los planes listos, guardados y commiteados.

Workflow obligatorio
1. $ps-contexto
2. Exploración obligatoria: mínimo 5 ps-explorer en paralelo + mi-lsp bajo `src/`
3. $brainstorming
4. $ps-asistente-wiki
5. $writing-plans
6. Guardar todos los planes en disco y hacer commit solo de artefactos de planning
7. $ps-trazabilidad
8. $ps-auditar-trazabilidad

Exploración obligatoria antes de planificar
- Usá `mi-lsp` como herramienta primaria para navegación semántica bajo `src/`.
- Despachá mínimo 5 exploraciones paralelas con objetivos distintos. No las hagas secuenciales.
- Objetivos mínimos:
  1. Verificar la superficie implementada hoy en `src/Bitacora.Api` y contrastarla contra `04_RF.md`, `07_baseline_tecnica.md` y `09_contratos_tecnicos.md`.
  2. Verificar qué módulos del canon siguen sin runtime efectivo: `Vinculos`, `Visualizacion`, `Telegram`, `Export`, frontend Next.js y deploy Dokploy.
  3. Verificar el estado real de pruebas: `06_matriz_pruebas_RF.md`, `06_pruebas/TP-*.md`, `src/Bitacora.Tests`, y cualquier smoke o harness existente.
  4. Verificar el estado real del frente visual: `23_uxui/INDEX.md`, `23_uxui/UI-RFC/UI-RFC-INDEX.md`, `.docs/stitch/STITCH-ARTIFACTS-AUDIT.md`, `.docs/stitch/DESIGN*.md`.
  5. Verificar requisitos de entorno y operación: PostgreSQL, Supabase Auth compartido, Dokploy, variables sensibles, migraciones, observabilidad y secretos.
  6. Verificar si existe cualquier código frontend, bot Telegram o infraestructura ya arrancada fuera de `src/Bitacora.Api`.
- Si 2 exploraciones se contradicen, resolvelo antes de escribir los planes.

Estado verificado que debés preservar (10 de abril de 2026)
- El repositorio ya fue publicado en `git@github.com:fgpaz/bitacora.git`.
- La rama activa es `main`.
- El worktree debería estar limpio al inicio de la sesión.
- Existe backend `.NET 10` runnable en `src/` con `Wave 1` implementada para `Auth`, `Consent`, `Registro` y `Seguridad`.
- Endpoints implementados hoy:
  - `POST /api/v1/auth/bootstrap`
  - `GET /api/v1/consent/current`
  - `POST /api/v1/consent`
  - `DELETE /api/v1/consent/current`
  - `POST /api/v1/mood-entries`
  - `POST /api/v1/daily-checkins`
  - `GET /health`
- Persistencia materializada hoy:
  - `User`
  - `ConsentGrant`
  - `MoodEntry`
  - `DailyCheckin`
  - `PendingInvite`
  - `AccessAudit`
  - `EncryptionKeyVersion`
- `Bitacora.Tests` existe como scaffold, pero no hay suite real material todavía.
- La capa UX/UI global ya está cerrada; no la reabras.
- `ONB-001`, `REG-001` y `REG-002` ya tienen cobertura Stitch completa pero siguen bloqueados para `UI-RFC` por hallazgos visuales.
- El resto de slices sigue entre `pendiente de auditoría` y `requiere rerun con design pack derivado`.
- `UX-VALIDATION` real sigue diferido hasta que exista producto funcionando; no planifiques validación con usuarios como condición previa de buildout.

Fuentes primarias que debés leer primero
- `AGENTS.md`
- `CLAUDE.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/04_RF.md`
- `.docs/wiki/05_modelo_datos.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/08_modelo_fisico_datos.md`
- `.docs/wiki/09_contratos_tecnicos.md`
- `.docs/wiki/23_uxui/INDEX.md`
- `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md`
- `.docs/stitch/STITCH-ARTIFACTS-AUDIT.md`
- `src/README.md`

Objetivo de salida de esta sesión
- Dejar una wave maestra de productivización con todos los planes necesarios para seguir desde acá sin volver a pensar el roadmap base.
- El resultado esperado no es un único plan suelto: es un índice maestro + una carpeta `wave-1/` con los planes siguientes.
- Cada plan downstream debe estar listo para ejecución posterior por waves.

Cobertura mínima que esos planes deben incluir
- Backend restante:
  - `Vinculos`
  - `Visualizacion`
  - `Export`
  - endurecimiento de `Consent` en cascadas diferidas
  - cierre de seams de seguridad/audit/queries profesionales
- Frontend:
  - bootstrap Next.js 16
  - auth compartida con Supabase
  - app shell mínima
  - onboarding/consent/registro paciente
  - slices profesionales y visualización según dependencias reales
- Telegram:
  - pairing
  - sesiones
  - reminder scheduler
  - registro conversacional
- Infra y operación:
  - PostgreSQL local/dev/prod
  - Dokploy
  - secretos
  - observabilidad
  - migraciones/bootstrap
  - backups y readiness mínima
- QA y release readiness:
  - smokes
  - contract checks
  - e2e mínimos
  - criterios de salida a staging/prod
- Visual unblock como frente aparte:
  - Stitch recovery
  - reruns pendientes
  - auditoría visual manual
  - condiciones exactas para abrir `UI-RFC-*`

Decisiones cerradas que NO debés reabrir
- Prioridad del proyecto: `Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > Time-to-market`.
- Monolito modular `.NET 10` como backend del MVP.
- Auth con Supabase compartido.
- PostgreSQL dedicada.
- Cifrado `encrypted_payload + safe_projection`.
- Fail-closed para seguridad y auditoría.
- `trace_id` end-to-end.
- La capa visual global ya está cerrada.
- La deuda de `UX-VALIDATION` no se resuelve ahora; primero hay que tener código funcionando.
- `UI-RFC` no se abre por slice mientras Stitch siga bloqueado.

Severity rules
- Si el repo contradice este prompt, confiá en el repo y nombrá la contradicción.
- Si encontrás doble autoridad documental, tratala como defecto real.
- Si encontrás un módulo del canon sin runtime ni plan futuro explícito, tratá eso como gap real.
- Si un plan presupone frontend o Telegram ya existentes cuando no existen, corregilo.
- Si una secuencia de planes deja sin dueño a deploy, secretos, migraciones o observabilidad, corregilo.
- Si intentás saltar de planificación a implementación en esta sesión, eso es drift.

Boundaries
- Planeamiento solamente.
- No escribir código de runtime.
- No reabrir manifesto, identidad, lineamientos visuales ni voz global.
- No crear `UX-VALIDATION-*` falsos.
- No crear `UI-RFC-*` todavía.
- No mezclar esta wave con handoff UX/UI final; eso sigue gated por validación/código y por Stitch.

Naming y estructura esperada
- Crear:
  - `.docs/raw/plans/2026-04-10-wave-1-productivizacion.md`
  - `.docs/raw/plans/wave-1/`
- Dentro de `wave-1/`, dejar planes downstream suficientes para continuar. Como mínimo, una familia de planes por estos frentes:
  - backend restante
  - frontend MVP
  - telegram runtime
  - infra/ops/deploy
  - QA/release readiness
  - visual unblock + Stitch/UI-RFC gates
- Si necesitás subdividir más, hacelo. Pero no dejes ningún frente crítico afuera.

Reglas específicas para $writing-plans
- Persistí los archivos a disco.
- Hacé commit solo de los artefactos de planificación creados en esta sesión.
- Si el plan supera 8 tareas o varios frentes, usá estructura de waves y subdocumentos de forma estricta.
- Como esta sesión sigue en planning-only, no ejecutes ninguna wave. Solo dejala lista.

Cierre esperado
- Devolvé un `<proposed_plan>` breve con:
  - nombre de la wave maestra
  - lista de planes creados dentro de `wave-1/`
  - riesgos principales detectados
  - criterio de orden de ejecución recomendado
- Cerrá con $ps-trazabilidad y $ps-auditar-trazabilidad.
- No termines hasta confirmar que todos los archivos del plan existen en disco.
```
