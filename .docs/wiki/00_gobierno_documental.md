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
  - id: scope
    label: Alcance funcional
    layer: "01"
    family: functional
    pack_stage: context
    paths:
      - .docs/wiki/01_alcance_funcional.md
  - id: architecture
    label: Arquitectura
    layer: "02"
    family: functional
    pack_stage: context
    paths:
      - .docs/wiki/02_arquitectura.md
  - id: flows
    label: Flujos funcionales
    layer: "03"
    family: functional
    pack_stage: context
    paths:
      - .docs/wiki/03_FL.md
      - .docs/wiki/03_FL/
  - id: requirements
    label: Requerimientos funcionales
    layer: "04"
    family: functional
    pack_stage: spec
    paths:
      - .docs/wiki/04_RF.md
      - .docs/wiki/04_RF/
  - id: data_model
    label: Modelo de datos
    layer: "05"
    family: functional
    pack_stage: spec
    paths:
      - .docs/wiki/05_modelo_datos.md
  - id: tests
    label: Pruebas y cobertura
    layer: "06"
    family: functional
    pack_stage: validation
    paths:
      - .docs/wiki/06_matriz_pruebas_RF.md
      - .docs/wiki/06_pruebas/
  - id: technical
    label: Baseline tecnica, modelo fisico y contratos
    layer: "07-09"
    family: technical
    pack_stage: implementation
    paths:
      - .docs/wiki/07_baseline_tecnica.md
      - .docs/wiki/07_tech/
      - .docs/wiki/08_modelo_fisico_datos.md
      - .docs/wiki/08_db/
      - .docs/wiki/09_contratos_tecnicos.md
      - .docs/wiki/09_contratos/
  - id: experience_global
    label: Experiencia global y marca
    layer: "10-16"
    family: experience
    pack_stage: experience
    paths:
      - .docs/wiki/10_manifiesto_marca_experiencia.md
      - .docs/wiki/11_identidad_visual.md
      - .docs/wiki/12_lineamientos_interfaz_visual.md
      - .docs/wiki/13_voz_tono.md
      - .docs/wiki/14_metodo_prototipado_validacion_ux.md
      - .docs/wiki/15_handoff_operacional_uxui.md
      - .docs/wiki/16_patrones_ui.md
  - id: ux_case
    label: Caso UX
    layer: "17-20"
    family: experience
    pack_stage: ux_case
    paths:
      - .docs/wiki/17_UXR.md
      - .docs/wiki/18_UXI.md
      - .docs/wiki/19_UJ.md
      - .docs/wiki/20_UXS.md
  - id: validation
    label: Validacion y operacion UX/UI
    layer: "21-23"
    family: validation
    pack_stage: validation
    paths:
      - .docs/wiki/21_matriz_validacion_ux.md
      - .docs/wiki/22_aprendizaje_ux_ui_spec_driven.md
      - .docs/wiki/23_uxui/
context_chain:
  - governance
  - scope
  - architecture
  - flows
  - requirements
closure_chain:
  - requirements
  - tests
  - technical
  - validation
audit_chain:
  - requirements
  - tests
  - technical
  - experience_global
  - validation
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
- El frontend (Next.js 16) está materializado y desplegado como superficie paciente/profesional.
- La wiki cubre toda la capa técnica (01-09) y tiene la capa UX/UI completa (10-23) como canon operativo.

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
