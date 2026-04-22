# Prompt: Sync canon UX/UI — dashboard-first post-login

**Precondición:** leer
`.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md` antes de tocar
nada.

## Objetivo
Propagar la decisión "Dashboard-first post-login (eliminar Bridge Card)" por
todo el canon UX/UI hardened para que el código y la wiki queden in-sync.

## Contexto (ya implementado en código)
- Se eliminó `NextActionBridgeCard.tsx` y la fase `S04-BRIDGE`.
- `/onboarding` ahora sólo corre si falta consent; caso contrario redirige a
  `/dashboard`.
- Landing pública tiene un único CTA `Ingresar` (sin email, sin magic-link).
- `/dashboard` integra `MoodEntryDialog` (modal) y `TelegramReminderBanner`
  (sólo visible si `getTelegramSession().linked === false`).

## Tareas concretas

1. **`.docs/wiki/23_uxui/UXS/UXS-ONB-001.md`**
   - Marcar la sección `S04-BRIDGE` como **deprecada desde 2026-04-22**.
   - Reemplazar por `S05-DASHBOARD-EMPTY` (el nuevo landing autenticado).
   - Actualizar las pantallas "Pantalla siguiente" en cada sección.

2. **`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md`**
   - Remover el componente `NextActionBridgeCard` del contrato.
   - Agregar `MoodEntryDialog` y `TelegramReminderBanner` como contrato nuevo
     (o derivar a un `UI-RFC-VIS-001`).

3. **`.docs/wiki/23_uxui/HANDOFF-*/`** (SPEC, VISUAL-QA, MAPPING, ASSETS)
   - Eliminar referencias a `NextActionBridgeCard`.
   - Agregar handoff para los nuevos componentes de dashboard.

4. **`.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md`**
   - Quitar strings de Bridge Card.
   - Sumar copy de empty-state y banner Telegram.

5. **`.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md`**
   - Reescribir la narrativa del último paso: ya no hay Bridge Card, se entra
     a dashboard directo.

6. **`.docs/wiki/06_pruebas/TP-ONB.md`**
   - Remover casos que validan Bridge Card.
   - Agregar caso "post-consent redirige a /dashboard".

7. **`.docs/wiki/06_pruebas/TP-VIS.md`**
   - Agregar casos:
     - `MoodEntryDialog` abre, guarda, cierra, refresca historial.
     - `TelegramReminderBanner` visibilidad condicionada a
       `session.linked === false`.
     - Dismiss del banner persiste 30 días.

8. **`.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`**
   - Actualizar el map de componentes (quitar NextActionBridgeCard, sumar
     MoodEntryDialog y TelegramReminderBanner).

9. **`.docs/wiki/07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`**
   - Documentar el rollout y métricas a observar: tasa de completitud
     post-consent sin Bridge Card, tasa de vinculación Telegram con banner
     dismissible.

10. **`.docs/wiki/04_RF.md` y nueva `RF-VIS-015`**
    - Nuevo RF `RF-VIS-015-dashboard-paciente-inline-entry.md`: cargar mood
      entry desde el dashboard sin abandonar la vista.
    - Actualizar índice `04_RF.md`.

## Cierre
- Correr `Skill(ps-trazabilidad)` para validar cross-links entre FL/RF/UXS/TP.
- Correr `Skill(ps-auditar-trazabilidad)` por ser cambio multi-módulo.
- Commit con mensaje: `docs: sync canon UX para decision dashboard-first`.
