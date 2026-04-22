<!--
target_platform: codex
pressure: aggressive
generated_at: 2026-04-20
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-20-bitacora-e2e-diario-qa-alt.md' | Set-Clipboard"
-->

# Misión

Estás en `C:\repos\mios\humor`. Ejecutá un E2E productivo completo de Bitácora como uso diario común de un paciente real: login web con Zitadel, consentimiento/onboarding si aplica, pairing fresco con Telegram usando el perfil `qa-alt`, registro desde Telegram, visualización web, registro web, configuración de recordatorio, logout y verificación fail-closed. Tratá `qa-alt` como cuenta QA dedicada y no expongas identificadores personales.

## Estado verificado que podés usar, pero debés revalidar

- Bitácora está en producción con Zitadel-only: `https://bitacora.nuestrascuentitas.com`, API `https://api.bitacora.nuestrascuentitas.com`, IdP `https://id.nuestrascuentitas.com`.
- Último commit relevante: `a6ae1e0 docs(security): redact telegram qa evidence`.
- Wave B `#17` está `Closed` y Project V2 `Done`; epic `#15` sigue `Open/Doing`.
- Bot canónico: `@mi_bitacora_personal_bot`.
- `mi-telegram-cli auth status --profile qa-alt --json` estaba `Authorized` el 2026-04-20. Revalidalo antes de usarlo.
- Evidencia vigente previa: `.docs/raw/reports/2026-04-20-qa-dev-full-smoke.md` y `artifacts/e2e/2026-04-20-qa-dev-full-smoke/`.

## Reglas inviolables

- Empezá con `$ps-contexto`.
- Usá `$mi-lsp` antes de grep/glob: `mi-lsp workspace list`, `mi-lsp workspace status bitacora --format toon --full`, `mi-lsp nav governance --workspace bitacora --format toon`.
- Después despachá en paralelo al menos 3 `ps-explorer` antes de planificar:
  1. Web patient journey: rutas protegidas, login/session, dashboard, registro, configuración Telegram.
  2. Telegram runtime: pairing, bot username, no-fuga, comandos `mi-telegram-cli`, contratos `CT-TELEGRAM-RUNTIME`.
  3. Evidence/privacy: dónde guardar screenshots/reportes sin PII, qué docs ya son vigentes.
- Agregá Explorer 4 si Playwright/auth interactiva falla o hay sesión rara. Agregá Explorer 5 si smoke/Dokploy/API runtime contradice docs.
- Corré `$brainstorming` antes de ejecutar. Lockear: datos sintéticos vs valores reales, fresh pairing via UI, evidencia redacted, alcance patient-only, rollback si pairing falla.
- Si el trabajo se vuelve multi-batch, usá `$writing-plans`. Si solo corrés E2E sin fixes, no hace falta plan largo.
- Cerrá con `$ps-trazabilidad`; usá `$ps-auditar-trazabilidad` si tocás código/docs, encontrás un bug crítico, o actualizás issues/board.

## Privacidad y seguridad

- No imprimas ni guardes `chat_id`, phone, username personal, cookies, auth codes, JWT, refresh tokens, PATs, passwords, bot tokens, session blobs, DB URIs ni payloads clínicos.
- No uses DB cleanup salvo bloqueo real de pairing y solo si la UI no permite desvincular. Si fuera inevitable, pedí confirmación explícita y usá variables efímeras, nunca valores hardcodeados.
- No copies respuestas completas de Telegram si contienen valores clínicos. El contrato vigente exige confirmaciones genéricas.
- Usá datos sintéticos por defecto. Si el usuario insiste en valores reales, ingresarlos solo en la UI/Telegram y redactar toda evidencia visual/textual.

## Preflight obligatorio

1. `git status --short` debe estar limpio salvo untracked históricos docs/prompts conocidos.
2. `git log -5 --oneline` debe incluir `a6ae1e0` o posterior.
3. `pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1` debe pasar.
4. `mi-telegram-cli auth status --profile qa-alt --json` debe ser `Authorized`.
5. Resolver el peer con comillas PowerShell: `@mi_bitacora_personal_bot`. No corras comandos concurrentes contra `qa-alt`.

## Ejecución E2E diaria

Crear evidencia en `artifacts/e2e/2026-04-20-bitacora-daily-use-qa-alt/`.

1. Abrir browser con Playwright en `https://bitacora.nuestrascuentitas.com/ingresar`.
2. Login por Zitadel. Si hay passwordless/MFA/passkey, dejar que el usuario lo complete en browser visible. Nunca pedir credenciales por chat.
3. Confirmar sesión: `/dashboard` carga, `/api/auth/session` devuelve usuario, proxy backend autenticado funciona.
4. Si aparece consentimiento/onboarding, completarlo como paciente. Si ya existe, registrar estado.
5. Ir a `/configuracion/telegram`.
6. Si Telegram ya está vinculado, capturar estado y luego hacer fresh pairing desde UI usando `Desvincular` solo para `qa-alt`. Si no hay botón o hay riesgo de desvincular una cuenta no-QA, detenerse y reportar.
7. Generar pairing code `BIT-XXXXX` desde UI.
8. En PowerShell, enviar el pairing:
   ```powershell
   mi-telegram-cli messages send --profile qa-alt --peer "@mi_bitacora_personal_bot" --text "/start BIT-XXXXX" --json
   mi-telegram-cli messages wait --profile qa-alt --peer "@mi_bitacora_personal_bot" --timeout 30 --json
   ```
   Si se ejecuta desde Git Bash, usar `MSYS_NO_PATHCONV=1`.
9. Verificar en web que `/configuracion/telegram` muestra vinculado y bot canónico.
10. Registrar un check-in Telegram sintético de bajo riesgo:
    - mood: `+1`
    - sleep: `7h`
    - physical: `Sí`
    - social: `No`
    - anxiety: `No`
    - irritability: `No`
    - medication: `No`
    Usar `messages send`, `messages wait`, y `messages press-button` por índice. Si el CLI cambia sintaxis, consultar `mi-telegram-cli messages --help` y documentar el cambio.
11. Validar que las respuestas del bot sean confirmation-only y no repitan score, sueño, factores, medicación, `chat_id`, `patient_id` ni payloads.
12. Volver a `/dashboard` y confirmar que el registro Telegram aparece con fecha local correcta (`America/Buenos_Aires`) y sin shift UTC.
13. Crear un registro web común desde `/registro/mood-entry` y un daily check-in desde `/registro/daily-checkin` con datos sintéticos distintos.
14. Volver al dashboard: validar totals, promedio, última entrada, timeline/listado y estado empty/error/loading si aparece.
15. En `/configuracion/telegram`, cambiar horario de recordatorio a un valor razonable y guardar. Validar feedback UI y que no se rompe el vínculo.
16. Logout. Confirmar que rutas protegidas redirigen/fallan cerrado y que `/api/backend/...` sin sesión da 401.
17. Login nuevamente y confirmar persistencia del vínculo Telegram y visualización de registros.

## Evidencia requerida

- `README.md` en el directorio de evidencia con pasos, resultado y comandos.
- Screenshots mínimos: login completado/dashboard, configuración Telegram vinculada, dashboard después de Telegram, dashboard después de registro web, logout/protected redirect.
- Un JSON/MD sanitizado con mensajes Telegram: solo tipo de respuesta, botón usado por índice y verdict; no valores personales si se usaron reales.
- Reporte opcional en `.docs/raw/reports/2026-04-20-bitacora-daily-use-qa-alt.md` si el E2E queda GREEN o encuentra gaps.

## Criterios GREEN

- Zitadel login/logout funciona.
- `qa-alt` queda pareado por `/start BIT-XXXXX`.
- Telegram registra check-in y el dashboard web lo muestra.
- Web permite registro diario común y dashboard refleja el cambio.
- Bot no ecoa valores clínicos ni identificadores.
- Sin Supabase Auth reintroducido.
- Evidencia no contiene secretos, tokens, `chat_id`, PII ni payload clínico.

## Si encontrás un bug

Clasificalo: Critical si fuga datos/PII/secreto o auth fail-open; High si impide login, pairing, registro o visualización; Medium si rompe UX diaria sin pérdida de datos; Low si es copy/polish. No cierres `#17` ni lo reabras. Comentá `#15` solo con resumen sanitizado si el smoke termina o si hay un bloqueo real. Para bugs nuevos, crear issue nuevo con evidencia sanitizada.
