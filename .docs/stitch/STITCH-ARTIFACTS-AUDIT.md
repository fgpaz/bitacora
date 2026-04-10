# STITCH-ARTIFACTS-AUDIT

> Archivo derivado. Resume el estado estático de los artifacts Stitch y la preparación de cada slice frente al gate `strict Stitch only`.

| Slice | Config | Perfil | Design pack | Artifacts | Cobertura | Estado | Gap | Fallback | Recomendación |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| reg-001 | core-wave.prompts.json | patient | DESIGN.patient.md | 3 run(s) | 7/7 | full-candidate | Sin gap de cobertura visible. | acotado | Listo para auditoría visual manual. |
| reg-002 | core-wave.prompts.json | patient | DESIGN.patient.md | 7 run(s) | 7/7 | full-candidate | Sin gap de cobertura visible. | acotado | Listo para auditoría visual manual. |
| onb-001 | onb-001.prompts.json | patient | DESIGN.patient.md | 23 run(s) | 12/12 | full-candidate | Sin gap de cobertura visible. | acotado | Listo para auditoría visual manual. |
| con-002 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 1 run(s) | 2/4 | partial | Cobertura insuficiente: 2/4 estados full. | acotado | Reejecutar el slice y completar estados faltantes. |
| exp-001 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 8 run(s) | 3/5 | partial | Cobertura insuficiente: 3/5 estados full. | acotado | Reejecutar el slice y completar estados faltantes. |
| vin-002 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 3 run(s) | 2/5 | partial | Cobertura insuficiente: 2/5 estados full. | acotado | Reejecutar el slice y completar estados faltantes. |
| vin-003 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 1 run(s) | 1/4 | partial | Cobertura insuficiente: 1/4 estados full. | acotado | Reejecutar el slice y completar estados faltantes. |
| vin-004 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 1 run(s) | 3/5 | core-only | Cobertura parcial: 3/5 estados full. | acotado | Completar estados faltantes antes de abrir UI-RFC. |
| vis-001 | patient-wave-a.prompts.json | patient | legacy-pre-design-pack | 6 run(s) | 5/5 | full-candidate | La última corrida no registra design pack derivado. | acotado | Reejecutar bajo design pack derivado antes de auditar. |
| tg-001 | remaining-wave-b.prompts.json | telegram | legacy-pre-design-pack | 3 run(s) | 5/5 | full-candidate | La última corrida no registra design pack derivado. | acotado | Reejecutar bajo design pack derivado antes de auditar. |
| tg-002 | remaining-wave-b.prompts.json | telegram | legacy-pre-design-pack | 2 run(s) | 4/4 | full-candidate | La última corrida no registra design pack derivado. | acotado | Reejecutar bajo design pack derivado antes de auditar. |
| vin-001 | remaining-wave-b.prompts.json | professional | legacy-pre-design-pack | 3 run(s) | 5/5 | full-candidate | La última corrida no registra design pack derivado. | acotado | Reejecutar bajo design pack derivado antes de auditar. |
| vis-002 | remaining-wave-b.prompts.json | professional | legacy-pre-design-pack | 5 run(s) | 4/5 | partial | Cobertura insuficiente: 4/5 estados full. | acotado | Reejecutar el slice y completar estados faltantes. |

## Auditoría visual manual — Ola núcleo (`2026-04-09`)

La cobertura Stitch de `ONB-001`, `REG-001` y `REG-002` ya está completa, pero la revisión manual contra `VOICE-*`, `UXS-*`, `PROTOTYPE-*` y el canon global detectó drift suficiente como para no abrir todavía `UI-RFC-*`.

| Slice | Resultado | Hallazgos principales | Decisión operativa |
| --- | --- | --- | --- |
| `ONB-001` | no aprueba | consentimiento demasiado pesado y bilingüe; continuidad posterior al consentimiento demasiado lírica; cierre final más solemne de lo permitido | reejecutar Stitch con ajuste de copy y jerarquía antes de abrir `UI-RFC-ONB-001` |
| `REG-001` | no aprueba | estado de sesión expirada sobredimensionado; confirmación con exceso de acciones y señales de confianza no contextuales | reejecutar Stitch con ajuste de estados sensibles antes de abrir `UI-RFC-REG-001` |
| `REG-002` | no aprueba | drift bilingüe en entrada (`Daily Factors`, `Complete`); confirmación final demasiado ceremonial y criptográfica | reejecutar Stitch con ajuste de idioma y confirmación antes de abrir `UI-RFC-REG-002` |

### Notas de detalle

- `ONB-001`
  - contradice la pauta de `silencio útil antes, control explícito en consentimiento, silencio útil después`;
  - el bloque de consentimiento conserva headings en inglés (`Total Privacy`, `Ownership`, `Ethical Use`) y gana demasiado peso para una pausa breve;
  - el cierre usa framing más poético de lo permitido (`Tu primera reflexión...`, `Ir al refugio`).
- `REG-001`
  - la entrada y la escala principal sí respetan la dirección `pregunta directa, cero interpretación`;
  - el desvío aparece en `S02-SESSION`, donde el estado explica de más seguridad y cifrado;
  - el desvío se repite en `S03-CONFIRM`, donde aparecen acciones adicionales y acceso profesional como si fueran el foco del cierre.
- `REG-002`
  - `S02-PARTIAL`, `S02-MEDICATION` y `S03-ERROR` están bastante alineados con el canon;
  - el principal gap está en `S01-ENTRY`, que mezcla inglés con español y rompe la voz del slice;
  - `S03-CONFIRM` debería ser más breve y factual, sin convertir el cierre en una explicación criptográfica.

### Rework pass posterior (`2026-04-09`, tarde)

Se ejecutó una pasada de corrección sobre:

- `.docs/stitch/stitch-mode-a.mjs`
- `.docs/stitch/core-wave.prompts.json`
- `.docs/stitch/onb-001.prompts.json`

Resultado observable:

- `REG-001`
  - `S02-SESSION` mejoró y ya usa reingreso breve y digno;
  - `S03-CONFIRM` mejoró y quedó factual;
  - `S02-DEFAULT` sigue sin cierre visual aceptable porque las últimas reejecuciones terminaron con `Resource has been exhausted` antes de devolver HTML descargable.
- `REG-002`
  - hubo mejora parcial en prompts para `S01-ENTRY` y `S03-CONFIRM`;
  - las últimas reejecuciones estrictas también quedaron afectadas por `Resource has been exhausted`, así que el último HTML verificable todavía no alcanza nivel de apertura.
- `ONB-001`
  - la tanda intermedia corrigió parte del drift más grave: headings ingleses, copy poético de expiración, auth demasiado lírica y confirmación excesivamente ornamental;
  - todavía quedan desvíos en consentimiento listo / conflicto y en compactación general del paso;
  - la última tanda correctiva también quedó bloqueada por `Resource has been exhausted`.

Conclusión operativa de esta rework pass:

- la ola núcleo avanzó materialmente;
- `REG-001`, `REG-002` y `ONB-001` siguen **bloqueados** para apertura de `UI-RFC-*`;
- el siguiente movimiento ya no es más ajuste local del runner, sino reintentar corridas Stitch cuando vuelva a haber cuota disponible y reauditar contra los mismos criterios.

Screens que todavía requieren rerun útil después del fix del runner:

- `REG-001`
  - `s02-default`
- `REG-002`
  - `s01-entry`
  - `s03-confirm`
- `ONB-001`
  - `s03-ready`
  - `s03-version-conflict`
  - `s05-confirm`

Nota operativa:

- desde esta pasada, `.docs/stitch/stitch-mode-a.mjs` ya trata `isError: true` y respuestas sin artifacts descargables como fallas reales;
- también endurece la auditoría estática para no contar `*.result.json` sin `html/png` útil como cobertura válida.
