# Pre-Push Guard Runbook — Bitacora XP-on-main

**Script:** `infra/git/Invoke-PrePushGuard.ps1`
**Skill:** `ps-pre-push` (en `C:\Users\fgpaz\.agents\skills\ps-pre-push\SKILL.md`)
**Policy:** CLAUDE.md + AGENTS.md seccion 11 "XP-on-main Workflow".

---

## Cuando ejecutarlo

**Siempre antes de `git push origin main`** en el workflow XP-on-main:

1. `ps-trazabilidad` debe estar completo.
2. `ps-auditar-trazabilidad` debe estar completo para cambios grandes/riesgosos, multi-modulo, policy-changing, o closure de waves.
3. Tests aplicables verdes (frontend typecheck + lint + e2e; backend `dotnet build`).
4. `Invoke-PrePushGuard.ps1` debe retornar `Approved` o `Approved with waiver` (exit 0).
5. `git push origin main`.

Nunca pushear con el guard `Blocked`. No fuerce push como workaround.

---

## Invocacion basica

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 `
  -ExpectedScope frontend,canon-docs `
  -TraceabilityEvidence .docs/raw/reports/2026-04-23-<task>-closure.md
```

Scopes reconocidos (alias aceptados entre parentesis):

| Scope | Cubre | Alias |
|-------|-------|-------|
| `policy` | `CLAUDE.md`, `AGENTS.md`, `SUBAGENTS.md` | `agents`, `claude` |
| `git-tooling` | `infra/git/*` | `tooling`, `prepush` |
| `shared-skill` | `.claude/skills/*` local | `skills`, `skill` |
| `canon-docs` | `.docs/wiki/*`, `.docs/templates/*` | `docs`, `wiki` |
| `evidence-docs` | `.docs/raw/reports`, `.docs/raw/decisiones`, `.docs/raw/investigacion` | `evidence`, `closure`, `reports`, `decisiones` |
| `raw-scratch` | `.docs/raw/plans`, `.docs/raw/prompts` | `scratch`, `raw` |
| `migrations-infra` | `infra/migrations/*` | `migrations`, `sql` |
| `secrets-infra` | `infra/.env*`, `infra/secrets*` | `secrets` |
| `infra` | resto de `infra/` | `ops` |
| `scripts-infra` | `scripts/*` | `scripts`, `tooling` |
| `ci` | `.github/*` | `github`, `actions` |
| `frontend` | `frontend/*` excepto frozen | `web`, `next`, `web-next` |
| `backend` | `src/Bitacora.*`, `src/Shared*`, `src/TelegramBotAdapter` | `api`, `service`, `dotnet`, `net` |
| `tests` | `tests/`, `test/` | `e2e`, `qa` |
| `frozen-auth` | `frontend/lib/auth/*`, `frontend/app/auth/*` | `auth` (requiere waiver) |
| `frozen-api` | `frontend/app/api/*` | `api-proxy` (requiere waiver) |
| `frozen-proxy` | `frontend/proxy.ts` | `proxy` (requiere waiver) |
| `frozen-src` | `frontend/src/*` | `src-frontend` (requiere waiver) |

---

## Modos de invocacion

### Dry-run (recomendado como pre-check local)

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 -ExpectedScope frontend -TraceabilityEvidence <path> -DryRun
```

No hace push; solo corre checks + emite verdict.

### JSON para automatizacion

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 -ExpectedScope frontend -TraceabilityEvidence <path> -Json
```

Devuelve `PrePushGuardReport` con schema `1.0-bitacora`.

### Waiver explicito

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 `
  -WaiverReason "Policy patch chat-approved; self-contained" `
  -ExpectedScope policy
```

Waiver resuelve ausencia de issue/card, evidence doc dedicado o verificacion de board. **NO** resuelve:
- fast-forward rechazado contra `origin/main`,
- scope no declarado en `-ExpectedScope`,
- archivos peligrosos untracked bajo `frontend/test-results`, `.next`, etc.,
- frozen surfaces sin waiver,
- raw paths sin categorizacion.

### Shared skill sync

Si el diff toca `.claude/skills/<name>` o `~/.agents/skills/<name>`:

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 `
  -ExpectedScope shared-skill,git-tooling `
  -SharedSkillName ps-pre-push `
  -TraceabilityEvidence <closure-path>
```

El guard compara `C:\Users\fgpaz\.agents\skills\<name>` vs `C:\repos\buho\assets\skills\<name>` byte a byte.

---

## Que checkea el guard

1. `git fetch origin main` exitoso.
2. `origin/main` es ancestor de `HEAD` (fast-forward natural, no force-push).
3. Scope declarado cubre todos los paths cambiados.
4. Frozen surfaces (`frontend/lib/auth/*`, etc.) bloqueados sin waiver.
5. `.docs/raw/` uncategorized bloqueado; raw-scratch (plans/prompts) warning.
6. Traceability evidence: `.docs/raw/reports/*-closure.md`, `.docs/raw/decisiones/`, `.docs/raw/investigacion/`, `.docs/auditoria/`, `.docs/planificacion/`, o `.docs/wiki/06_*`.
7. Shared-skill source/mirror byte-identical cuando shared-skill esta en scope.
8. Issue/card: warning (no-blocker) mientras Bitacora no tenga Project V2 configurado.
9. Dangerous untracked artifacts bajo `frontend/test-results`, `frontend/.next`, `frontend/coverage`, `src/*/bin`, `src/*/obj`, `artifacts/` (no e2e).

---

## Verdicts

- **`Approved`** (exit 0): safe to `git push origin main`.
- **`Approved with waiver`** (exit 0): safe with documented waiver context.
- **`Blocked`** (exit 2): NO pushear. Fix cada blocker y re-run.

---

## Troubleshooting

### "Base ref origin/main is missing after fetch"

Chequear conectividad a `github.com`. El guard hace `git fetch origin main --prune`; si falla, ningun safety check sirve.

### "Changed path is outside ExpectedScope"

Agregar el scope correspondiente a `-ExpectedScope`. Los alias (ver tabla) permiten ser laxo; p.ej. `infra` cubre tambien `infra/git`, y `docs` cubre `canon-docs`.

### "Critical surface requires ExpectedScope declaration"

Se toco un surface sensible (policy, shared-skill, git-tooling, canon-docs, migrations-infra, secrets-infra, ci, frozen-*) sin declarar scope. Agregar al parametro.

### "Frozen surface touched without waiver"

Se toco `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`, `frontend/proxy.ts` o `frontend/src/*`. Requiere waiver explicito con `-WaiverReason` y justificacion humana.

### "Uncategorized raw artifact under .docs/raw/"

Mover el archivo a `.docs/raw/reports/`, `.docs/raw/decisiones/`, `.docs/raw/investigacion/`, `.docs/raw/plans/` o `.docs/raw/prompts/`. Los roots sueltos estan vedados.

### "Traceability evidence missing"

Pasar `-TraceabilityEvidence` apuntando a un closure doc (ej: `.docs/raw/reports/YYYY-MM-DD-<task>-closure.md`). Si el diff ya incluye un closure doc bajo las paths reconocidas, el guard lo detecta automaticamente (`DetectedInDiff`).

### "Dangerous untracked artifact"

Los outputs de tests (`frontend/test-results`, `playwright-report`, `.next`) y builds .NET (`bin`, `obj`) no deben pushearse. Agregar al `.gitignore` correspondiente o mover a `artifacts/e2e/<YYYY-MM-DD>-<task-slug>/` segun la artifact hygiene rule de CLAUDE.md seccion 9.2.

---

## Inicializar configuracion de board (opcional)

Bitacora no tiene Project V2 configurado hoy. Si se agrega, crear `.pj-crear-tarjeta.conf` en la raiz del repo:

```conf
PJ_REPO=fgpaz/bitacora
PJ_PROJECT_OWNER=fgpaz
PJ_PROJECT_NUMBER=1
PJ_PROJECT_STATUS_FIELD=Status
```

El guard pasa de "warning sin issue" a "verificar board card" automaticamente.

---

## Mantenimiento

- Source-of-truth: este runbook + `Invoke-PrePushGuard.ps1`.
- Cuando se agrega nuevo scope o frozen surface en CLAUDE.md seccion 11, actualizar la tabla de scopes aqui + el `switch -Regex` del script en `Get-AffectedSurface` y el hash `$aliases` de `Test-ScopeMatch`.
- El script usa PowerShell 5.1+ y Git disponible en PATH.
