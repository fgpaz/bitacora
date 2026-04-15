# Task T3: GOV — Crear 00_gobierno_documental.md

## Shared Context
**Goal:** Resolver GOV: el archivo `.docs/wiki/00_gobierno_documental.md` no existe, bloqueando `ps-contexto` y `mi-lsp nav governance`.
**Stack:** Markdown + YAML embebido + TOML (projection)
**Architecture:** El skill `crear-gobierno-documental` existe en `C:\Users\fgpaz\.claude\skills\crear-gobierno-documental\`. El template de referencia está en `references/governance-template.md` dentro de ese skill.

## Locked Decisions
- Usar el skill `crear-gobierno-documental` — NO crear el archivo manualmente.
- Perfil: `spec_backend` (Bitácora es backend-first con UX/UI diferido a Phase 40).
- Workspace alias para mi-lsp: `bitacora` o `humor` (validar con `mi-lsp workspace list`).

## Task Metadata
```yaml
id: T3
depends_on: []
agent_type: ps-worker
files:
  - create: .docs/wiki/00_gobierno_documental.md
  - create: .docs/wiki/_mi-lsp/read-model.toml
complexity: low
done_when: "mi-lsp nav governance retorna sin bloqueo; .docs/wiki/00_gobierno_documental.md existe con YAML válido"
```

## Reference
Skill de referencia: `C:\Users\fgpaz\.claude\skills\crear-gobierno-documental\SKILL.md`
Template: `C:\Users\fgpaz\.claude\skills\crear-gobierno-documental\references\governance-template.md`

## Prompt
El archivo `.docs/wiki/00_gobierno_documental.md` no existe. `mi-lsp workspace status` reporta governance como BLOCKED.

Tu tarea es invocar `Skill(crear-gobierno-documental)` para crear el gobierno documental de Bitácora.

Antes de invocar el skill, ten a mano este contexto para responder las preguntas del skill:
- **Nombre del proyecto:** Bitácora
- **Descripción:** Clinical mood tracker — procesa datos de salud mental bajo Ley 25.326, 26.529 y 26.657 (Argentina)
- **Perfil governance:** `spec_backend` (backend completo, frontend diferido a Phase 40)
- **Overlays activos:** `spec_core`, `technical`
- **Wiki path:** `.docs/wiki/`
- **Workspace alias mi-lsp:** verificar con `mi-lsp workspace list` antes de asumir (`bitacora` o `humor`)
- **Archivos presentes en wiki (01-22):** todos existentes; falta solo 00

Después de que el skill genere el documento:
1. Verificar que `.docs/wiki/00_gobierno_documental.md` existe y contiene un bloque YAML con `version`, `profile`, `hierarchy`.
2. Verificar que `.docs/wiki/_mi-lsp/read-model.toml` fue creado.
3. Correr `mi-lsp nav governance --workspace <alias> --format toon` para confirmar que ya no está BLOCKED.

## Execution Procedure
1. Correr `mi-lsp workspace list` para verificar el alias correcto del workspace.
2. Invocar `Skill(crear-gobierno-documental)` con el contexto de Bitácora.
3. Responder las preguntas del skill usando los valores del Prompt.
4. Verificar existencia: `ls .docs/wiki/00_gobierno_documental.md`.
5. Verificar existencia: `ls .docs/wiki/_mi-lsp/read-model.toml`.
6. Correr `mi-lsp nav governance --workspace bitacora --format toon` (o el alias correcto).
7. Si governance sigue bloqueado: leer el mensaje de error y reportar exactamente qué campo del YAML falta.

## Skeleton
```yaml
# Embedded en 00_gobierno_documental.md:
version: 1
profile: spec_backend
overlays:
  - spec_core
  - technical
hierarchy:
  - id: governance
    label: Gobierno documental
    layer: "00"
    family: functional
    pack_stage: governance
    paths:
      - .docs/wiki/00_gobierno_documental.md
context_chain: [governance]
closure_chain: [governance]
audit_chain: [governance]
blocking_rules:
  - missing_human_governance_doc
  - missing_governance_yaml
  - invalid_governance_schema
  - projection_out_of_sync
  - workspace_index_stale
projection:
  output: .docs/wiki/_mi-lsp/read-model.toml
  format: toml
  auto_sync: true
  versioned: true
```

## Verify
`mi-lsp nav governance --workspace bitacora --format toon` → sin mensaje BLOCKED

## Commit
`docs(gov): create 00_gobierno_documental.md and _mi-lsp/read-model.toml (GOV)`
