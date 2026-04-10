# 20 — Índice UXS por slice

## Propósito

Este documento funciona como índice/puente de la capa `UXS`.

Los contratos UX críticos por caso viven en `23_uxui/UXS/*` y toman el paso más sensible o decisivo de cada slice.

## Regla operativa

- cada slice visible debe tener un `UXS-<SLICE>.md` dueño del paso crítico;
- `UXS` no reemplaza `VOICE` ni `UJ`, pero aterriza estructura, estados, copy y aceptación;
- no bajar a prototipo un slice que todavía no tenga su `UXS` principal.

## Documentos activos

| ID | Caso | Paso crítico | Estado | Documento |
| --- | --- | --- | --- | --- |
| `ONB-001` | Onboarding invitado del paciente hasta primer MoodEntry | `S03` — Consentimiento como explicitación de control | Prototype cerrado | `23_uxui/UXS/UXS-ONB-001.md` |
| `REG-001` | Registro rápido de humor vía web | `S02` — Selección y confirmación inmediata del humor | Case UX cerrado | `23_uxui/UXS/UXS-REG-001.md` |
| `REG-002` | Registro de factores diarios vía web | `S03` — Revisión final y envío del check-in | Case UX cerrado | `23_uxui/UXS/UXS-REG-002.md` |
| `VIN-001` | Emisión de invitación profesional a paciente | `S02` — Revisión y emisión de invitación | Case UX cerrado | `23_uxui/UXS/UXS-VIN-001.md` |
| `VIN-002` | Auto-vinculación paciente a profesional por código | `S02` — Ingreso y validación del código | Case UX cerrado | `23_uxui/UXS/UXS-VIN-002.md` |
| `VIN-003` | Revocación de vínculo por paciente | `S02` — Confirmación de revocación del vínculo | Case UX cerrado | `23_uxui/UXS/UXS-VIN-003.md` |
| `VIN-004` | Gestión de acceso profesional por paciente | `S02` — Cambio del estado de acceso | Case UX cerrado | `23_uxui/UXS/UXS-VIN-004.md` |
| `CON-002` | Revocación de consentimiento | `S02` — Revisión del impacto antes de revocar | Case UX cerrado | `23_uxui/UXS/UXS-CON-002.md` |
| `VIS-001` | Timeline longitudinal del paciente | `S02` — Lectura inicial y ajuste de período | Case UX cerrado | `23_uxui/UXS/UXS-VIS-001.md` |
| `VIS-002` | Dashboard multi-paciente del profesional | `S02` — Primera lectura del dashboard | Case UX cerrado | `23_uxui/UXS/UXS-VIS-002.md` |
| `EXP-001` | Exportación CSV del paciente | `S02` — Confirmación del alcance y disparo de descarga | Case UX cerrado | `23_uxui/UXS/UXS-EXP-001.md` |
| `TG-001` | Vinculación de cuenta Telegram | `S01` — Generación y handoff del código | Case UX cerrado | `23_uxui/UXS/UXS-TG-001.md` |
| `TG-002` | Recordatorio y registro conversacional por Telegram | `S02` — Respuesta al recordatorio | Case UX cerrado | `23_uxui/UXS/UXS-TG-002.md` |

---

**Estado:** índice raíz de la familia `UXS`.
**Siguiente capa gobernada:** `23_uxui/UXS/UXS-INDEX.md`.
