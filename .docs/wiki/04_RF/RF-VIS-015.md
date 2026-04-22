# RF-VIS-015: Dashboard paciente con inline mood entry

## Execution Sheet
- Modulo: VIS
- Trigger: Paciente autenticado accede a `/dashboard` con `consent_granted = true`
- Actor: Paciente (autenticado)
- Prioridad PDP: Correctness → Usability

## Precondiciones detalladas
- Sesion activa: `bitacora_session` cookie valida
- `ConsentGrant` en estado `granted` para el paciente
- El paciente puede tener cero o mas `MoodEntry` previos (ambos estados visibles en dashboard)
- Backend sin cambios: los endpoints de registro y visualizacion son los mismos que definen RF-REG-001..005

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| score | Seleccion del paciente via `MoodScale` | Entero en rango -3..+3 |
| session | `bitacora_session` cookie | Identifica al paciente autenticado |
| refreshNonce | Estado interno de `Dashboard` | Trigger de refresco post-guardado |

## Proceso (Happy Path)

### Estado empty (ningun MoodEntry previo)
1. `Dashboard.tsx` carga y detecta lista de registros vacia.
2. Muestra CTA "Registrar humor" (empty state).
3. Paciente presiona CTA → `MoodEntryDialog` se abre como `<dialog>` nativo modal.
4. `MoodEntryForm` (modo embedded) renderiza `MoodScale` dentro del dialog.
5. Paciente selecciona score y confirma.
6. `MoodEntryForm` hace `POST /api/v1/mood-entries` con el score seleccionado.
7. API retorna `201 Created` con el `MoodEntry` creado.
8. `onSaved` callback del dialog dispara actualizacion de `refreshNonce`.
9. `Dashboard.tsx` re-fetcha el historial con el nuevo nonce.
10. Modal se cierra; dashboard muestra el primer registro en el historial.

### Estado ready (uno o mas MoodEntry previos)
1. `Dashboard.tsx` carga y muestra el historial de registros.
2. Boton "+ Nuevo registro" disponible en la UI.
3. Paciente presiona "+ Nuevo registro" → flujo identico desde paso 3 del estado empty.

## Outputs
- `MoodEntry` persistido en `bitacora_db` (cifrado, RF-REG-003).
- Dashboard actualizado sin navegacion ni recarga completa de pagina.
- Modal cerrado, historial visible con el nuevo registro.

## Errores tipados
| Codigo | HTTP | Tratamiento UI |
|--------|------|----------------|
| `CONSENT_REQUIRED` | 403 | Muestra bloque de consent en lugar del formulario; redirige a `/consent` |
| `ENCRYPTION_FAILURE` | 500 | `InlineFeedback` con `variant=error` y `trace_id` visible |
| `INVALID_SCORE` | 422 | `InlineFeedback` con `variant=error` localizado en el campo score |
| `SESSION_EXPIRED` | 401 | Redireccion a `/ingresar`; sin perdida de contexto de navegacion |

## Casos especiales y variantes
- **Cierre sin guardar**: presionar Escape, click en backdrop o boton "Cerrar" cierra el modal sin side-effects. No se crea `MoodEntry`.
- **Doble envio**: el boton de confirmar entra en estado `submitting` post-click; no permite doble accion confusa.
- **TelegramReminderBanner**: visible encima del historial si `TelegramSession.linked === false`. Dismissible por 30 dias via `localStorage['bitacora.telegram.banner.dismissedAt']`. No afecta el flujo de registro.
- **Banner ya dismissed**: no visible aunque `linked === false` si el dismiss sigue vigente (< 30 dias).

## Impacto en modelo de datos
- INSERT en `mood_entries` (sin cambios de schema respecto a RF-REG-001..003)
- No hay impacto en tablas nuevas; toda la logica de persistencia es la misma del modulo REG

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Empty state CTA abre modal
  Given paciente autenticado con consent_granted=true y sin MoodEntry previos
  When accede a /dashboard
  Then ve CTA "Registrar humor"
  When presiona CTA
  Then MoodEntryDialog se abre como modal

Scenario: Modal guarda y refresca sin navegar
  Given MoodEntryDialog abierto con MoodScale lista
  When paciente selecciona score y confirma
  Then POST /api/v1/mood-entries retorna 201
  And el modal se cierra
  And el dashboard muestra el nuevo registro sin recarga completa de pagina

Scenario: Cierre sin guardar no tiene side-effects
  Given MoodEntryDialog abierto
  When paciente presiona Escape o backdrop o boton Cerrar
  Then el modal se cierra
  And ningun MoodEntry es creado

Scenario: Error CONSENT_REQUIRED muestra bloque de consent
  Given sesion activa pero consent_granted=false
  When el modal intenta enviar POST /api/v1/mood-entries
  Then API retorna 403 CONSENT_REQUIRED
  And la UI muestra bloque de consent con camino claro a /consent

Scenario: Error tecnico muestra InlineFeedback con trace_id
  Given POST /api/v1/mood-entries retorna 500 ENCRYPTION_FAILURE con trace_id
  When el modal recibe el error
  Then muestra InlineFeedback variant=error con el trace_id visible

Scenario: Banner Telegram visible con linked=false
  Given paciente autenticado con TelegramSession.linked=false
  And sin dismiss vigente en localStorage
  When accede a /dashboard
  Then TelegramReminderBanner es visible

Scenario: Banner Telegram oculto con linked=true
  Given paciente autenticado con TelegramSession.linked=true
  When accede a /dashboard
  Then TelegramReminderBanner no se renderiza

Scenario: Banner dismissed persiste 30 dias
  Given paciente presiona dismiss en TelegramReminderBanner
  When vuelve a /dashboard dentro de 30 dias
  Then TelegramReminderBanner no se renderiza aunque linked=false
```

## Trazabilidad de tests
- TP-VIS-06: modal de nuevo registro abre, guarda y refresca sin salir del dashboard
- TP-VIS-07: banner Telegram aparece con `linked=false`, oculto con `linked=true`, dismiss persiste 30 dias

## Componentes involucrados
| Componente | Ruta | Rol |
|-----------|------|-----|
| `Dashboard` | `frontend/components/patient/dashboard/Dashboard.tsx` | orquestacion del estado empty/ready y refresco |
| `MoodEntryDialog` | `frontend/components/patient/dashboard/MoodEntryDialog.tsx` | modal nativo `<dialog>` que embebe `MoodEntryForm` |
| `MoodEntryForm` | `frontend/components/patient/mood/MoodEntryForm.tsx` | formulario de registro en modo embedded |
| `TelegramReminderBanner` | `frontend/components/patient/dashboard/TelegramReminderBanner.tsx` | banner dismissible de vinculacion Telegram |

## Dependencias
- FL-ONB-01 (flujo de onboarding; post-consent va al dashboard)
- FL-REG-01 (flujo de registro de MoodEntry; backend sin cambios)
- FL-VIS-01 (flujo de visualizacion del historial del paciente)
- RF-REG-001..005 (backend de registro; este RF solo define la superficie UI)

## Sin ambiguedades pendientes
- El modal es un `<dialog>` nativo del browser; no un portal React ni una ruta separada
- `refreshNonce` es un entero de estado local que se incrementa post-save; no hay WebSocket ni polling
- `TelegramReminderBanner` consulta `getTelegramSession()` en el mismo render del dashboard; el resultado no bloquea la carga del historial
- El primer registro desde el dashboard cumple RF-ONB-004 (primer MoodEntry post-consent) y RF-ONB-005 (transicion a estado `active`) sin requerir el flujo de Bridge Card que fue eliminado

## Cambios recientes

- 2026-04-22: RF creado como parte de la decision "dashboard-first". Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
