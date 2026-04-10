# VOICE — Índice de specs de voz por slice

## Propósito

Este índice lista las specs de voz por slice visible.

Cada `VOICE-*` traduce la base global de `13_voz_tono.md` al caso concreto ya definido por `UXI` y `UJ`.

## Documentos activos

| ID | Caso | Cobertura | Estado | Siguiente artefacto |
| --- | --- | --- | --- | --- |
| `VOICE-ONB-001` | Onboarding invitado del paciente hasta primer MoodEntry | `S01..S05`, con foco sensible en `S01` y `S03` | activo y adelantado | `../PROTOTYPE/PROTOTYPE-ONB-001.md` |
| `VOICE-REG-001` | Registro rápido de humor vía web | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-REG-001.md` |
| `VOICE-REG-002` | Registro de factores diarios vía web | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-REG-002.md` |
| `VOICE-VIN-001` | Emisión de invitación profesional a paciente | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIN-001.md` |
| `VOICE-VIN-002` | Auto-vinculación paciente a profesional por código | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIN-002.md` |
| `VOICE-VIN-003` | Revocación de vínculo por paciente | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIN-003.md` |
| `VOICE-VIN-004` | Gestión de acceso profesional por paciente | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIN-004.md` |
| `VOICE-CON-002` | Revocación de consentimiento | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-CON-002.md` |
| `VOICE-VIS-001` | Timeline longitudinal del paciente | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIS-001.md` |
| `VOICE-VIS-002` | Dashboard multi-paciente del profesional | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-VIS-002.md` |
| `VOICE-EXP-001` | Exportación CSV del paciente | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-EXP-001.md` |
| `VOICE-TG-001` | Vinculación de cuenta Telegram | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-TG-001.md` |
| `VOICE-TG-002` | Recordatorio y registro conversacional por Telegram | framing verbal del slice y su paso crítico | activo | `../UXS/UXS-TG-002.md` |

## Regla de uso

- crear un nuevo `VOICE-*` cuando un slice visible tenga sensibilidad verbal propia;
- no duplicar en `VOICE-*` decisiones globales ya cerradas en `13_voz_tono.md`;
- no bajar todavía layout ni comportamiento detallado: eso pertenece a `UXS`.

---

**Estado:** índice activo de voz por caso.
**Slices cubiertos:** `ONB-001`, `REG-001`, `REG-002`, `VIN-001`, `VIN-002`, `VIN-003`, `VIN-004`, `CON-002`, `VIS-001`, `VIS-002`, `EXP-001`, `TG-001`, `TG-002`.
**Siguiente capa gobernada:** `../UXS/*` y, cuando aplique, `../PROTOTYPE/*`.
