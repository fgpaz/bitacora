# Task T2: Functional Canon Sync

## Shared Context
**Goal:** Align flows, requirements, and semantic data model with the verified repo truth and pending MVP scope.  
**Stack:** Markdown wiki, .NET 10 backend, PostgreSQL, `mi-lsp`.  
**Architecture:** Functional docs must distinguish current runtime from deferred slices without deleting the target MVP.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/03_FL.md
  - modify: .docs/wiki/03_FL/FL-*.md
  - modify: .docs/wiki/04_RF.md
  - modify: .docs/wiki/04_RF/RF-*.md
  - modify: .docs/wiki/05_modelo_datos.md
  - read: .docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md
complexity: high
done_when: ".docs/wiki/03_FL.md, .docs/wiki/04_RF.md, impacted FL/RF docs, and .docs/wiki/05_modelo_datos.md separate implemented truth from pending work without contradiction"
```

## Reference
`.docs/wiki/03_FL/FL-ONB-01.md` — existing patient flow structure.  
`.docs/wiki/04_RF/RF-VIN-*.md` — vínculo requirements to preserve as target scope.  
`.docs/wiki/05_modelo_datos.md` — canonical entity model that must reflect current vs deferred materialization.

## Prompt
Use the gap map from T1 as the editorial guide. Before editing, rerun `mi-lsp workspace list --format toon`, validate the workspace with `mi-lsp workspace status "/mnt/c/repos/mios/humor" --format toon`, then use `mi-lsp nav search "PendingInvite" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`, `mi-lsp nav search "ConsentGrant" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`, and `mi-lsp nav search "DailyCheckin" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon` to confirm which entities and behaviors are currently materialized. If `mi-lsp` returns `hint` or `next_hint`, follow that rerun guidance before falling back. Then update `.docs/wiki/03_FL.md`, impacted `FL-*` files, `.docs/wiki/04_RF.md`, impacted `RF-*` files, and `.docs/wiki/05_modelo_datos.md` so they do three things at once: preserve the MVP target, mark implemented truth accurately, and flag not-yet-built slices as pending rather than implied runtime. Keep the canon in Spanish, keep IDs stable, and do not renumber the RF/FL sets. When a slice depends on future code (`CareLink`, professional views, Telegram sessions, `frontend/`), say so explicitly.

## Skeleton
```md
## Estado actual
## Alcance pendiente
## Dependencias para implementación
```

## Verify
`git diff -- .docs/wiki/03_FL .docs/wiki/04_RF .docs/wiki/05_modelo_datos.md` -> functional canon explicitly marks current truth and deferred work

## Commit
`docs(spec): normalize functional canon for wave-prod`
