<!--
target_platform: codex
pressure: max-pressure
generated_at: 2026-04-20
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-20-fix-reminder-ui-hardening.md' | Set-Clipboard"
-->

# Nueva sesión Codex: corregir E2E Bitácora + hardening UI paciente

Estás en `C:\repos\mios\humor`. Trabajá sobre Bitácora, app clínica de registro de humor. Corregí todo lo encontrado en el E2E productivo del 2026-04-20 y revisá/rediseñá las partes UI que están incompletas o apenas funcionales, manteniendo privacidad clínica y trazabilidad estricta.

## Objetivo

1. Corregir el bug `#21`: guardar recordatorio de Telegram a las `22:00` desde `/configuracion/telegram` falla con `500 UNEXPECTED_ERROR`.
2. Resolver la deriva de contrato de horario: la UI selecciona hora local de Buenos Aires, pero hoy envía `{ hourUtc, minuteUtc, timezone }` sin convertir local -> UTC.
3. Evitar 500 ante input inválido: validar `hour 0..23`, `minute 0|30`, timezone válida/default Buenos Aires y sesión/vínculo Telegram activo según `RF-TG-006`.
4. Revisar y elevar la UI paciente porque hay superficies que no están realmente diseñadas: dashboard, configuración Telegram, registro de humor, daily check-in, shell/navegación/logout, estados loading/error/empty y responsive.
5. Sincronizar docs, tests, evidencia, issue `#21` y board si corresponde.

## Reglas obligatorias

- Empezá con `$ps-contexto`.
- Usá `$mi-lsp` antes de cualquier `rg`, glob o lectura masiva:
  - `mi-lsp workspace list`
  - `mi-lsp workspace status bitacora --format toon --full`
  - `mi-lsp nav governance --workspace bitacora --format toon`
  - Si `mi-lsp` reporta memoria stale o `next_hint`, seguí esa guía antes de confiar en el índice.
- Usá `$frontend-design` antes de los skills Impeccable.
- Corré `$impeccable-audit`, `$impeccable-adapt`, `$impeccable-bolder` y `$brainstorming`.
- Usá `$ps-asistente-wiki` porque probablemente haya que tocar contrato/docs/UX canon.
- Usá `$writing-plans` antes de ejecutar: esto es grande, riesgoso y multi-módulo.
- Cerrá cada batch relevante con `$ps-trazabilidad`.
- Cerrá la tarea final con `$ps-trazabilidad` y `$ps-auditar-trazabilidad`.
- No expongas ni guardes PII, `chat_id`, phone, username personal, cookies, auth codes, JWT, refresh tokens, PATs, passwords, bot tokens, session blobs, DB URIs ni payloads clínicos.
- No uses DB cleanup salvo bloqueo real y con confirmación explícita del usuario.
- No reabras ni cierres `#17`. Trabajá sobre `#21` y, si aplica, comentá `#15` solo con resumen sanitizado.

## Estado verificado a revalidar

- Producción: `https://bitacora.nuestrascuentitas.com`
- API: `https://api.bitacora.nuestrascuentitas.com`
- IdP: `https://id.nuestrascuentitas.com`
- Auth: Zitadel-only. No reintroducir Supabase Auth.
- Bot canónico: `@mi_bitacora_personal_bot`
- Cuenta QA Telegram: perfil `qa-alt`, cuenta dedicada. Usarla serialmente, nunca concurrente.
- Último E2E diario quedó AMBER, no GREEN.
- Evidencia E2E previa:
  - `.docs/raw/reports/2026-04-20-bitacora-daily-use-qa-alt.md`
  - `artifacts/e2e/2026-04-20-bitacora-daily-use-qa-alt/README.md`
  - `artifacts/e2e/2026-04-20-bitacora-daily-use-qa-alt/telegram-sanitized.json`
  - Screenshot clave: `artifacts/e2e/2026-04-20-bitacora-daily-use-qa-alt/05-reminder-2200-attempt.png`
- Issue creado:
  - `#21` Reminder schedule save fails with 500 on 22:00 from Telegram settings
  - Severity actual: Medium
  - Labels: `bug`, `qa-report`
- Project V2 sync vía `pj-crear-tarjeta` falló antes porque `gh` no tenía auth de Projects. Reintentá si el entorno ya está autenticado; si no, reportá el bloqueo sin inventar sincronización.

## Exploración obligatoria en paralelo

Después de `$ps-contexto` y `$mi-lsp`, despachá al menos estos 5 `ps-explorer` en paralelo antes de planificar:

1. Backend reminder:
   - Revisar `RF-TG-006`, endpoint `PUT /api/v1/telegram/reminder-schedule`, handler, entidad `ReminderConfig`, middleware de errores y tests existentes.
   - Devolver evidencia `file:line` sobre causa probable del 500, validaciones faltantes y contrato real.
2. Frontend reminder/timezone:
   - Revisar `frontend/components/patient/telegram/TelegramPairingCard.tsx`, su CSS y `frontend/lib/api/client.ts`.
   - Confirmar cómo se elige hora local, qué payload se manda, cómo se muestra respuesta y cómo se maneja error.
3. UI paciente:
   - Auditar dashboard, mood entry, daily check-in, shell/logout, onboarding/consent si aplica, estados loading/error/empty.
   - Buscar hardcoded colors, textos sin tildes, layouts frágiles, touch targets, overlap móvil, estados no diseñados.
4. UX/UI canon y handoff:
   - Revisar `.docs/wiki/10_manifiesto_marca_experiencia.md`, `11_identidad_visual.md`, `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md`, `16_patrones_ui.md`, `.docs/wiki/23_uxui/INDEX.md` y docs TG/REG/VIS/ONB vinculados.
   - Devolver drift entre canon y UI implementada, y qué docs requieren sync.
5. QA/runtime/evidencia:
   - Revisar el reporte E2E, README de evidencia, `#21`, `#15`, comandos smoke, Playwright y Telegram CLI.
   - Proponer matriz mínima de verificación post-fix con evidencia sanitizada.

Si Playwright/auth interactiva falla o aparece sesión rara, agregar Explorer 6 para auth/session. Si smoke/Dokploy/API contradice docs, agregar Explorer 7.

## Decisiones a bloquear en brainstorming

- Alcance patient-only. No rediseñar admin ni features no relacionadas.
- Datos sintéticos por defecto. Si se usan datos reales, toda evidencia visual/textual debe quedar redactada.
- Telegram no-leak invariant: el bot no debe ecoar valores clínicos, factores, medicación, `chat_id`, `patient_id` ni payloads.
- La UI muestra horario local de Buenos Aires para paciente; el contrato debe ser explícito y consistente.
- Recomendación inicial: preservar almacenamiento/contrato UTC si ya es canónico, pero convertir hora local -> UTC antes de enviar o en un boundary claro del backend. Si cambiás contrato a campos locales, actualizá RF/CT/API/client/tests.
- `$impeccable-bolder` no significa UI ruidosa: en este producto significa más diseñada, contenida, clara, distintiva y confiable. Evitá gamificación, glassmorphism, gradientes morados/azules dominantes, estética marketing o mensajes motivacionales invasivos.
- La experiencia debe sentirse como “calma clínica y contención”, con seguridad sin juicio, control reversible y lenguaje sobrio.

## Trabajo esperado

### Fix `#21`

- Encontrar causa raíz del `500` en recordatorio.
- Agregar validación backend si falta:
  - `hour` entero `0..23`
  - `minute` solo `0` o `30`
  - timezone válida o default `America/Argentina/Buenos_Aires`
  - vínculo/sesión Telegram activo si `RF-TG-006` lo exige
- Convertir errores a respuestas tipadas, nunca `UNEXPECTED_ERROR` para input normal.
- Resolver contrato local/UTC en frontend/backend/documentación.
- Agregar tests enfocados de dominio/handler/endpoint según el patrón del repo.

### UI hardening

- Ejecutar `$impeccable-audit` y guardar reporte sanitizado, preferentemente:
  - `.docs/raw/reports/2026-04-20-bitacora-ui-audit.md`
  - o `artifacts/ui-audit/2026-04-20-bitacora-patient-ui/README.md`
- Aplicar `$impeccable-adapt` para mobile `320`, `375`, tablet y desktop:
  - sin overflow, sin overlap, touch targets adecuados, controles estables, texto que entra.
- Aplicar `$impeccable-bolder` bajo el canon:
  - Telegram settings no debe verse como formulario genérico sin diseño.
  - Dashboard debe tener jerarquía clara, estados completos y lectura cotidiana.
  - Mood entry y daily check-in deben sentirse parte de la misma experiencia.
  - Shell/logout/protected states deben ser sobrios y consistentes.
- Corregir español visible: tildes, signos de apertura, copy claro y no paternalista.
- Reemplazar colores hardcodeados por tokens/patrones existentes cuando sea viable.

### Docs y trazabilidad

Usá `$ps-asistente-wiki` para decidir exactamente qué sincronizar. Como mínimo revisá:

- `.docs/wiki/04_RF/RF-TG-006.md`
- `.docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md`
- `.docs/wiki/09_contratos_tecnicos.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `.docs/wiki/23_uxui/INDEX.md` y handoffs TG/REG/VIS si la UI cambia.

Actualizá docs solo si el contrato, estados, UX o tests realmente cambian. No hagas churn documental.

## Verificación mínima

Preflight:

```powershell
git status --short --untracked-files=all
git log -5 --oneline
pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1
mi-telegram-cli auth status --profile qa-alt --json
```

Tests/build:

```powershell
dotnet test .\src\Bitacora.sln --configuration Release
Push-Location .\frontend; npm run typecheck; npm run lint; npm run build; Pop-Location
```

E2E post-fix:

- Crear evidencia en `artifacts/e2e/2026-04-20-bitacora-reminder-ui-fix/` o fecha actual si corresponde.
- Login por Zitadel sin pedir credenciales por chat.
- Ir a `/configuracion/telegram`.
- Guardar recordatorio `22:00` y validar feedback UI.
- Confirmar que el vínculo Telegram no se rompe.
- Logout y fail-closed básico.
- Si necesitás Telegram CLI, usar `qa-alt` serialmente y evidencia sanitizada.
- Capturar screenshots desktop y mobile de las superficies UI tocadas.
- Verificar con Playwright que no haya overlap/overflow en mobile.

## Issues/board

- Actualizá `#21` con comentario sanitizado y evidencia. Cerralo solo si el fix y E2E pasan.
- Si se toca el Project V2, usar `$pj-crear-tarjeta`/`pj-crear-tarjeta --status-sync` cuando `gh` tenga permisos. Si no, reportar bloqueo exacto.
- Comentá `#15` solo con resumen sanitizado si el cierre es relevante para el epic.

## Criterios GREEN

- Guardar recordatorio `22:00` desde UI funciona sin 500.
- El contrato horario queda consistente y documentado.
- Inputs inválidos no producen 500.
- Telegram permanece vinculado.
- UI paciente queda auditada y las superficies tocadas están diseñadas, responsive y alineadas al canon.
- Tests/build/smoke pasan o quedan bloqueos explícitos.
- Evidencia no contiene secretos, tokens, PII, `chat_id` ni payload clínico.
- `$ps-trazabilidad` y `$ps-auditar-trazabilidad` quedan ejecutados antes del cierre.

## Entrega final esperada

Responder con:

- Cambios de código y docs, con paths.
- Resultado de tests/build/smoke/E2E.
- Link o path de evidencia UI/E2E.
- Estado de `#21` y board.
- Riesgos residuales o bloqueos, si quedan.
