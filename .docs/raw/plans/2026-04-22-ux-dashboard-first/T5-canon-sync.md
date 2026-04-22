# Task T5: Sync del canon UX/UI hardened

## Shared Context
**Goal:** Propagar la decisión "dashboard-first" por toda la wiki SDD hardened (13 archivos afectados). El canon debe reflejar que `S04-BRIDGE`, `NextActionBridgeCard` y `signInWithMagicLink` ya no existen en producción.
**Stack:** Markdown bajo `.docs/wiki/`. Sin código. Tildes y signos de apertura mandatorios.
**Architecture:** Wiki governance sync validada por `mi-lsp` (profile `spec_backend`). Cambios UI-only no afectan datos ni contratos técnicos; sí afectan RF/FL/UXS/TP y design system.

## Locked Decisions
- Decision doc anchor: `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. Todos los docs modificados deben linkear ahí.
- Se crea `RF-VIS-015` nuevo: "Dashboard paciente inline-entry".
- En `UXS-ONB-001`: marcar `S04-BRIDGE` como deprecado desde 2026-04-22 con reemplazo `S05-DASHBOARD-EMPTY`.
- En `UI-RFC-ONB-001`: remover `NextActionBridgeCard` del contrato, agregar sección "Componentes fuera de alcance (movidos a Dashboard)".
- Crear contrato nuevo `UI-RFC-VIS-001` para `MoodEntryDialog` + `TelegramReminderBanner` (si el formato hardened lo requiere; caso contrario agregar como sección en `UI-RFC-ONB-001`).
- En `TP-ONB`: eliminar casos de Bridge Card. En `TP-VIS`: agregar casos para modal y banner.
- En `04_RF.md`: agregar entrada para `RF-VIS-015`.

## Task Metadata
```yaml
id: T5
depends_on: [T1, T2, T3]
agent_type: ps-docs
files:
  - modify: .docs/wiki/03_FL/FL-ONB-01.md
  - modify: .docs/wiki/04_RF.md
  - create: .docs/wiki/04_RF/RF-VIS-015.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-ONB-001.md
  - modify: .docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md
  - modify: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md
  - modify: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-ONB-001.md
  - modify: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-ONB-001.md
  - modify: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-ONB-001.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md
  - modify: .docs/wiki/06_pruebas/TP-ONB.md
  - modify: .docs/wiki/06_pruebas/TP-VIS.md
  - modify: .docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md
  - modify: .docs/wiki/07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md
complexity: high
done_when: "grep -rln 'NextActionBridgeCard\\|S04-BRIDGE\\|signInWithMagicLink\\|magic link' .docs/wiki/ → 0 matches; RF-VIS-015 existe y está indexado"
```

## Reference
- Decision doc: `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md` (contexto completo).
- Código de referencia: `frontend/components/patient/dashboard/{MoodEntryDialog,TelegramReminderBanner}.tsx`, `frontend/app/page.tsx`, `frontend/components/patient/onboarding/OnboardingEntryHero.tsx`, `frontend/components/patient/onboarding/OnboardingFlow.tsx`.
- Plantilla RF existente: `04_RF/RF-VIS-010.md` o `RF-VIS-014.md` (copiar estructura).

## Prompt
Sos `ps-docs`. Tu tarea es propagar una decisión de UX ya tomada a través del canon SDD hardened. NO discutís la decisión; la reflejás.

Reglas duras:
- Mantener la terminología del canon (nomenclatura de estados, RF IDs, formato de secciones).
- Cada doc modificado debe incluir un link al decision doc `2026-04-22-dashboard-first-post-login.md` en la sección "Cambios recientes" o equivalente.
- NO borrar secciones completas; marcar deprecados con un bloque `> **Deprecado 2026-04-22**: ...` y dejar la historia.
- NO duplicar contenido entre docs; cada doc cumple un rol único en el canon.
- Todo en español con tildes. Evitar emojis (política CLAUDE.md §9).
- Si un archivo tiene estructura que no reconocés, DETENETE y reportá en vez de improvisar.

## Execution Procedure
1. Leer el decision doc completo antes de tocar nada.
2. Para cada archivo de la lista `files.modify`:
   - Leer el doc actual. Identificar las secciones que mencionan `NextActionBridgeCard`, `S04-BRIDGE`, `magic link`, `Revisá tu correo` o el flujo viejo post-consent.
   - Hacer la edición mínima: reemplazar o marcar deprecado, agregar link al decision doc.
3. Crear `.docs/wiki/04_RF/RF-VIS-015.md` con el Skeleton A. Título: "Dashboard paciente con inline mood entry".
4. Editar `.docs/wiki/04_RF.md` para agregar la entrada nueva en el índice, preservando orden alfanumérico.
5. Ejecutar:
   ```bash
   grep -rln 'NextActionBridgeCard\|S04-BRIDGE\|signInWithMagicLink' .docs/wiki/
   ```
   Debe devolver 0 matches (salvo los bloques `> **Deprecado**:` que son textual-histórico explícito — si hay, documentarlos en el commit message).
6. Ejecutar `mi-lsp nav governance --workspace bitacora --format toon`. Debe seguir en sync (`sync: in_sync`).
7. Commit.

## Skeleton

Skeleton A — `.docs/wiki/04_RF/RF-VIS-015.md`:
```markdown
# RF-VIS-015: Dashboard paciente con inline mood entry

## Objetivo
Permitir al paciente ver su historial de registros de humor y cargar un nuevo registro sin salir del dashboard, usando un modal accesible.

## Contexto
Decisión: `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
Reemplaza el patrón previo `S04-BRIDGE` + ruta separada `/registro/mood-entry`.

## Actores
- Paciente autenticado con `consent_granted = true`.

## Precondiciones
- Sesión activa (`bitacora_session` cookie válida).
- Consent activo.

## Postcondiciones
- Si guarda un registro: nuevo `MoodEntry` creado; dashboard refresca timeline+summary sin navegar.
- Si cierra el modal sin guardar: sin side-effects.

## Criterios de aceptación
1. En `/dashboard` empty state, el CTA "Registrar humor" abre el modal "Nuevo registro".
2. En `/dashboard` ready state, el CTA "+ Nuevo registro" abre el mismo modal.
3. El modal se cierra con Escape, click en backdrop, click en botón "Cerrar".
4. Tras guardar exitosamente, el dashboard recarga timeline+summary y cierra el modal.
5. Si el backend devuelve `CONSENT_REQUIRED`, el modal muestra el bloque de consent y link a `/consent`.
6. Si el backend devuelve `ENCRYPTION_FAILURE` o `INVALID_SCORE`, el modal queda abierto con `InlineFeedback` de error + `trace_id`.

## Componentes
- `frontend/components/patient/dashboard/MoodEntryDialog.tsx`
- `frontend/components/patient/mood/MoodEntryForm.tsx` (variante `embedded`)
- `frontend/components/patient/dashboard/Dashboard.tsx` (integración)

## Dependencias
- FL-ONB-01, FL-REG-01, FL-VIS-01.
- RF-REG-001..005 (contrato backend sin cambios).

## Test plan
Ver `TP-VIS.md` casos TP-VIS-06 y TP-VIS-07.

## Cambios recientes
- 2026-04-22 — creación, derivada de la decisión "dashboard-first".
```

## Verify
```bash
grep -rln 'NextActionBridgeCard\|S04-BRIDGE\|signInWithMagicLink' .docs/wiki/   # 0 matches (o sólo bloques histórico-deprecado)
ls .docs/wiki/04_RF/RF-VIS-015.md                                               # existe
grep -c 'RF-VIS-015' .docs/wiki/04_RF.md                                        # ≥ 1
mi-lsp nav governance --workspace bitacora --format toon | head -20             # sync: in_sync
```

## Commit
```
docs(canon): sync wiki con decisión dashboard-first (RF-VIS-015, UXS/UI-RFC/TP)
```
