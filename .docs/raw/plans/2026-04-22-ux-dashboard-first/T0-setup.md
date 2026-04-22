# Task T0: Setup de rama para la feature

## Shared Context
**Goal:** Preparar el workspace sobre `main` con una rama dedicada `feature/ux-dashboard-first` para las waves siguientes.
**Stack:** Git, repo `C:\repos\mios\humor`, rama principal `main`.
**Architecture:** Monorepo con `frontend/` (Next.js) y `src/` (.NET). El plan sólo toca `frontend/` y `.docs/`.

## Locked Decisions
- Nombre de rama: `feature/ux-dashboard-first`.
- Base: `main` con `pull --ff-only` al momento de ejecutar.
- No se deben traer cambios de otras ramas ni `git stash pop` previos.

## Task Metadata
```yaml
id: T0
depends_on: []
agent_type: ps-worker
files:
  - read: .gitignore
complexity: low
done_when: "git rev-parse --abbrev-ref HEAD == feature/ux-dashboard-first && git status --porcelain vacío"
```

## Reference
N/A — es un paso de setup.

## Prompt
Sos un subagente ejecutando un paso atómico de setup. Trabajás en el repo `C:\repos\mios\humor` (Git Bash en Windows).

Tu objetivo: dejar el working tree limpio sobre una rama nueva `feature/ux-dashboard-first` salida de `main` actualizada. No modificás archivos de código en este paso.

Reglas duras:
1. NO uses `git push --force`, `git reset --hard`, `git checkout --` ni `git clean -f`.
2. Si hay cambios no commiteados al arrancar, DETENETE y reportá al orquestador el output de `git status -s` — no intentes "limpiar" por tu cuenta.
3. Si `main` no está en sync con remoto, DETENETE y reportá.

## Execution Procedure
1. Ejecutar `git status -s`. Si hay output no vacío, detenerse y reportar el status literal al orquestador.
2. Ejecutar `git fetch origin main` y luego `git switch main`.
3. Ejecutar `git pull --ff-only origin main`. Si falla, detenerse y reportar.
4. Ejecutar `git switch -c feature/ux-dashboard-first`.
5. Ejecutar `git status -s` de nuevo. Debe devolver vacío.
6. Ejecutar `git rev-parse --abbrev-ref HEAD`. Debe imprimir `feature/ux-dashboard-first`.

## Skeleton
```bash
git status -s
git fetch origin main
git switch main
git pull --ff-only origin main
git switch -c feature/ux-dashboard-first
git status -s
git rev-parse --abbrev-ref HEAD
```

## Verify
`git rev-parse --abbrev-ref HEAD` → `feature/ux-dashboard-first` y `git status -s` → vacío.

## Commit
(no hay commit en T0 — sólo setup; Wave 1 commitea por tarea).
