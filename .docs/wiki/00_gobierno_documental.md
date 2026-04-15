# 00. Gobierno Documental — Bitácora

## Propósito

Este documento establece la gobernanza documental del repo `humor` (Bitácora).
Si este documento no está perfectamente claro o es inválido, el trabajo spec-driven debe detenerse.

Bitácora es un clinical mood tracker que procesa datos de salud mental bajo Ley 25.326 (Protección de Datos Personales), Ley 26.529 (Derechos del Paciente) y Ley 26.657 (Salud Mental). Toda decisión de implementación, configuración de infraestructura o cambio en flujo de datos debe respetar estas tres leyes como requisito no negociable.

## Governance Source

```yaml
version: 1
profile: spec_backend
overlays:
  - spec_core
  - technical
numbering_recommended: true
hierarchy:
  - id: governance
    label: Gobierno documental
    layer: "00"
    family: functional
    pack_stage: governance
    paths:
      - .docs/wiki/00_gobierno_documental.md
context_chain:
  - governance
closure_chain:
  - governance
audit_chain:
  - governance
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

## Autoridad canónica

- `00_gobierno_documental.md` manda — es la fuente humana de verdad.
- `.docs/wiki/_mi-lsp/read-model.toml` es la proyección ejecutable y versionada.
- `mi-lsp nav governance` es la superficie primaria de diagnóstico.
- `CLAUDE.md` contiene las reglas operacionales de trabajo (no es fuente de gobernanza documental).

## Perfil: spec_backend

El proyecto usa perfil `spec_backend` porque:
- El backend (.NET 10) está completo y en producción.
- El frontend (Next.js 16) existe pero su deployment completo está diferido a Phase 40.
- La wiki cubre toda la capa técnica (01-09) y tiene la capa UX/UI completa (10-22) como anticipación.

## Estructura de la wiki

```
.docs/wiki/
  00_gobierno_documental.md        ← este archivo (governance authority)
  01_alcance_funcional.md          ← scope
  02_arquitectura.md               ← architecture
  03_FL.md + 03_FL/                ← functional flows
  04_RF.md + 04_RF/                ← requirements
  05_modelo_datos.md               ← data model
  06_matriz_pruebas_RF.md + 06_pruebas/  ← test plans
  07_baseline_tecnica.md + 07_tech/      ← technical baseline
  08_modelo_fisico_datos.md + 08_db/     ← physical data model
  09_contratos_tecnicos.md + 09_contratos/  ← technical contracts
  10-22 (UX/UI hardened canon)
  _mi-lsp/read-model.toml          ← executable projection (generated)
```

## Reglas de bloqueo

- Toda tarea arranca por gate de gobernanza.
- Si la gobernanza es ambigua, inválida, incompleta o stale, el repo entra en blocked mode.
- En blocked mode solo se permite diagnóstico y reparación.
- La prioridad del proyecto (Security > Privacy > Correctness > …) está en `02_arquitectura.md`.
