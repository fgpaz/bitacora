# Prompt: Phase 40 — Frontend Bitácora 100% Producción
# target: claude-code
# pressure: aggressive
# generated: 2026-04-15
# task-size: large/risky/multi-module

---

## Misión

Completar, deployar y dejar 100% operativo el frontend de Bitácora en `bitacora.nuestrascuentitas.com`.

Hoy el backend está 100% funcional en producción. El frontend Next.js 16 existe en `frontend/` pero **nunca fue deployado**. La misión es cerrar Phase 40 por completo: completar las páginas faltantes, corregir la configuración de auth, deployar en Dokploy y verificar todo extremo a extremo.

**Repo:** `C:\repos\mios\humor`
**Fecha de referencia:** 2026-04-15

---

## Estado verificado al 2026-04-15 — NO reabrir estas decisiones

### Lo que existe y compila

| Artefacto | Estado |
|-----------|--------|
| `frontend/app/(patient)/registro/mood-entry/page.tsx` | ✅ Existe |
| `frontend/app/(patient)/registro/daily-checkin/page.tsx` | ✅ Existe |
| `frontend/app/(patient)/consent/page.tsx` | ✅ Existe |
| `frontend/app/(patient)/onboarding/page.tsx` | ✅ Existe |
| `frontend/app/(patient)/configuracion/telegram/page.tsx` | ✅ Existe — pairing code UI |
| `frontend/app/(profesional)/**` | ✅ Pacientes, detalle, invitaciones |
| `frontend/app/page.tsx` | ✅ Landing + magic-link entry |
| `frontend/middleware.ts` | ✅ Fail-closed, extrae role de JWT |
| `frontend/lib/auth/client.ts` | ✅ Magic link + getAccessToken + signOut |
| `frontend/lib/api/client.ts` | ✅ 8 endpoints paciente |
| `frontend/lib/api/professional.ts` | ✅ 8 endpoints profesional |
| `frontend/Dockerfile` | ✅ standalone output, Node 22, bake NEXT_PUBLIC_* en build |
| `infra/dokploy/bitacora-frontend.production.md` | ✅ App ID = `BRTMuvBfWtslXHnShtrnB` |

### Bug crítico de auth — NO ignorar

La documentación de Dokploy en `infra/dokploy/bitacora-frontend.production.md` referencia `NEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com`. **Esto es incorrecto.** La instancia GoTrue de Bitácora es `https://auth.bitacora.nuestrascuentitas.com`. Si el frontend apunta a `auth.tedi`, los JWT que emita serán inválidos para el backend de Bitácora.

**Valor correcto obligatorio:**
```
NEXT_PUBLIC_SUPABASE_URL=https://auth.bitacora.nuestrascuentitas.com
NEXT_PUBLIC_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiYW5vbiIsImlzcyI6InN1cGFiYXNlIiwiaWF0IjoxNzc2MjAwMDAwLCJleHAiOjE5Mjk4ODAwMDB9.6G7aHaCH2D2VKQ-sym_Fk1qk9kdXM9ocOiiLrKdbRhw
NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com
NODE_ENV=production
```

Todos estos valores viven en `infra/.env` — leer de ahí, NO inventarlos.

### Lo que falta para 100%

| Gap | Descripción | Prioridad |
|-----|-------------|-----------|
| Dashboard paciente | No existe `/dashboard` o `/registro/historial` — paciente no puede ver sus propios registros históricos | Alta |
| Página vinculos paciente | No existe `/configuracion/vinculos` — paciente no puede ingresar un binding code para aceptar invitación del profesional | Alta |
| Configuración recordatorios Telegram | La página `/configuracion/telegram` hace el pairing pero no permite configurar horario de recordatorio | Media |
| Logout explícito | No hay botón de logout en ninguna página. La sesión solo expira vía middleware. | Media |
| error.tsx + not-found.tsx | Páginas de error 404/500 para Next.js App Router. Sin ellas, errores rompen la UI. | Media |
| Health check API route | `/api/health` route para Dokploy/Traefik. Sin ella el load balancer no puede hacer health checks del frontend. | Alta |
| Deployment | El frontend nunca fue deployado a `bitacora.nuestrascuentitas.com` | Bloqueante |
| Env vars en Dokploy | Las env vars del frontend nunca fueron configuradas en Dokploy | Bloqueante |

---

## Skill de deployment — dokploy-cli

Para todas las operaciones de deploy en Dokploy usar `Skill("dokploy-cli")`. Scripts disponibles:
- Git Bash / Linux: `$HOME/.claude/skills/dokploy-cli/scripts/dkp.sh`
- Windows PowerShell: `$HOME\.claude\skills\dokploy-cli\scripts\dkp.ps1`

Comandos clave para este plan:
```bash
DKP="$HOME/.claude/skills/dokploy-cli/scripts/dkp.sh"
$DKP doctor                                  # verificar conexión
$DKP status BRTMuvBfWtslXHnShtrnB           # ver estado del frontend
$DKP redeploy BRTMuvBfWtslXHnShtrnB         # deploy
$DKP domains BRTMuvBfWtslXHnShtrnB          # ver dominios configurados
```

Para configurar env vars del frontend, usar la API de Dokploy directamente o la UI de Dokploy en `http://54.37.157.93:3000`.

---

## Paso 1 obligatorio — Cargar contexto

Ejecutar `Skill("ps-contexto")` antes de cualquier otra acción. Leer específicamente:

- `.docs/wiki/01_alcance_funcional.md` — capacidades MVP
- `.docs/wiki/02_arquitectura.md` — decisión de prioridad y stack
- `.docs/wiki/07_baseline_tecnica.md` — Phase 40 row + invariantes T3-SEC-11 y T3-11
- `.docs/wiki/09_contratos_tecnicos.md` — contratos de autenticación y endpoints
- `infra/.env` — credenciales reales para configurar Dokploy
- `infra/dokploy/bitacora-frontend.production.md` — config de Dokploy (verificar y corregir)

---

## Paso 2 obligatorio — Exploración paralela

Despachar **5 subagentes `ps-explorer` en paralelo** antes de planificar. No reducir a menos de 5 — esta tarea es multi-módulo, cross-cutting, y tiene riesgo de auth drift.

```
Explorer 1: frontend/app/ — mapear todas las rutas existentes, sus estados (idle/submitting/success/error), y qué API calls hacen. Identificar exactamente qué le falta a cada página para ser completa.

Explorer 2: frontend/lib/ — auditar client.ts y auth/client.ts. Verificar que las URLs de GoTrue y API apuntan a los valores correctos (auth.bitacora vs auth.tedi). Identificar si hay hardcoded URLs incorrectas.

Explorer 3: frontend/components/ — listar todos los componentes, identificar cuáles están en uso y cuáles no. Verificar que TelegramPairingCard.tsx implementa el wizard completo (pairing + confirmación + status de vinculación).

Explorer 4: src/Bitacora.Api/Endpoints/ — verificar qué endpoints existen para: GET /telegram/session (para mostrar estado de vinculación), y si hay endpoint para configurar horario de recordatorio del bot. Esto define qué puede hacer la UI.

Explorer 5: infra/ — leer infra/.env (sin imprimir secretos), infra/dokploy/bitacora-frontend.production.md, y verificar si hay un script de deploy o runbook para el frontend en infra/runbooks/.
```

---

## Paso 3 — Brainstorming bloqueante

Ejecutar `Skill("brainstorming")` con foco en:

1. **Auth flow UX**: La app usa magic link (email OTP). ¿Es la experiencia correcta para el usuario de salud mental? Pros/cons vs password. Decisión debe quedar locked antes de planning.

2. **Dashboard paciente**: ¿Debe ser una nueva ruta `/dashboard` o una ruta de visualización dentro de `/registro/historial`? ¿Qué datos muestra? ¿Timeline propio usando el componente Timeline.tsx que existe para profesional?

3. **Wizard Telegram completo**: La página de pairing existe. ¿El wizard necesita más pasos (estado actual de vinculación, botón de desvincular, configuración de horario)? Definir el alcance exacto antes de implementar.

4. **Deployment order**: ¿Primero corregir env vars + deployar la app existente, o completar páginas faltantes primero? El orden importa porque puede haber usuarios reales.

---

## Paso 4 — Plan con writing-plans (obligatorio)

Ejecutar `Skill("writing-plans")` para generar el plan. El plan debe estar en `.docs/raw/plans/2026-04-15-phase40-frontend.md` con subdocumentos en `.docs/raw/plans/2026-04-15-phase40-frontend/`.

### Waves propuestas (ajustar según brainstorming)

**Wave 0 (secuencial — desbloquea todo lo demás):**
- T0a: Corregir `infra/dokploy/bitacora-frontend.production.md` con NEXT_PUBLIC_SUPABASE_URL correcto
- T0b: Configurar env vars en Dokploy usando `Skill("dokploy-cli")` (App ID: `BRTMuvBfWtslXHnShtrnB`) — usar `dkp.sh` o `dkp.ps1` según plataforma
- T0c: Primer deploy del frontend con `Skill("dokploy-cli")` → `redeploy BRTMuvBfWtslXHnShtrnB` para verificar que la infraestructura funciona

**Wave 1 (paralelo — páginas faltantes):**
- T1: Health check API route `/api/health/route.ts`
- T2: `error.tsx` y `not-found.tsx` en app root
- T3: Logout explícito — botón en PatientPageShell o nav
- T4: Dashboard paciente `/dashboard` — timeline propio reutilizando data del backend

**Wave 2 (paralelo — flujos Telegram y vinculos):**
- T5: Página `/configuracion/vinculos` — wizard para ingresar binding code y aceptar invitación del profesional
- T6: Mejorar `/configuracion/telegram` — agregar estado actual de vinculación + desvincular + (si el backend lo soporta) configuración de horario de recordatorio

**Wave 3 (secuencial — verificación e2e):**
- T7: Re-deploy con `Skill("dokploy-cli")` → `redeploy BRTMuvBfWtslXHnShtrnB` y esperar que el health check del dominio retorne 200
- T8: E2E web: login → consent → mood entry → daily checkin → telegram wizard → vinculos
- T9: Smoke del frontend desde `bitacora.nuestrascuentitas.com`

**Wave Final (trazabilidad):**
- T10: `ps-trazabilidad` — verificar sync FL/RF/TP con lo implementado
- T11: `ps-auditar-trazabilidad` — audit cross-documento

---

## Paso 5 — Ejecución con subagentes

Despachar según el plan generado. Agentes disponibles:

| Dominio | Subagente |
|---------|-----------|
| Frontend Next.js | `ps-next-vercel` |
| Backend .NET 10 | `ps-dotnet10` |
| Git, config, shell, infra | `ps-worker` |
| Deploy en Dokploy | `dokploy-cli` (`dkp.sh` / `dkp.ps1`) |
| Exploración | `ps-explorer` |
| Docs/wiki | `ps-docs` |
| Code review | `ps-code-reviewer` |

**Regla de despacho:** Wave 0 es secuencial (esperar que T0c confirme que el deploy funciona antes de Wave 1). Waves 1 y 2 son paralelas dentro de cada wave.

---

## Paso 6 — Cierre de trazabilidad

Ejecutar `Skill("ps-trazabilidad")` verificando:

**Cadena:** `00 → FL → RF → 07/08/09 → TP`

Específicamente:
- FL-ONB-01, FL-REG-01, FL-TG-01, FL-VIN-01 — flows web verificados en producción
- RF-ONB-*, RF-REG-001..005, RF-TG-003, RF-VIN-010..012 — requirements cubiertos por el frontend
- TP-ONB, TP-REG, TP-TG, TP-VIN, TP-VIS — test plans actualizados con evidencia web
- `07_baseline_tecnica.md` Phase 40 marcada como EJECUTADA
- T3-SEC-11 (frontend middleware role enforcement) — verificado en producción
- `09_contratos_tecnicos.md` — frontend como consumidor documentado

Luego ejecutar `Skill("ps-auditar-trazabilidad")` — audit final antes de cerrar Phase 40.

---

## Invariantes que NUNCA violar

1. **T3-SEC-11**: El middleware del frontend DEBE extraer `user_metadata.role` del JWT y forzar `/` si el rol no coincide con la ruta. No bypassear.
2. **Consent gate**: Ninguna ruta de paciente permite acceso a registro sin consentimiento activo. El backend lo enforza con 403, el frontend debe redirigir a `/consent`.
3. **No datos clínicos en logs**: Nunca loguear `encrypted_payload`, `safe_projection` con datos, ni `patient_id` directo. Solo `trace_id` y `pseudonym_id`.
4. **Auth domain correcto**: `NEXT_PUBLIC_SUPABASE_URL` DEBE apuntar a `auth.bitacora.nuestrascuentitas.com`. Verificar en código Y en Dokploy antes de cualquier deploy.
5. **Magic link redirect**: La URL de redirect del OTP debe apuntar a `https://bitacora.nuestrascuentitas.com/onboarding`.

---

## Artefactos de evidencia esperados

Guardar en `artifacts/e2e/2026-04-15-phase40/`:
- Screenshot de landing page en producción
- Screenshot de flujo de login completo (magic link)
- Screenshot de mood entry completada
- Screenshot de wizard Telegram (pairing + confirmación)
- Screenshot de dashboard del paciente (historial)
- Screenshot de configuración de vinculos
- `smoke-frontend.txt` — resultado del smoke test web

---

## Criterio de done

Phase 40 está cerrada cuando:
1. `https://bitacora.nuestrascuentitas.com` retorna HTTP 200 y muestra la landing page
2. Un usuario nuevo puede: recibir magic link → dar consent → registrar humor → ver su historial → vincular Telegram → recibir recordatorio del bot
3. `ps-trazabilidad` retorna 0 gaps abiertos
4. `ps-auditar-trazabilidad` retorna audit limpio
5. `07_baseline_tecnica.md` Phase 40 marcada como EJECUTADA con fecha 2026-04-15
