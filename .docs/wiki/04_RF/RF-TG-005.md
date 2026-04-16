# RF-TG-005: Desvincular sesion Telegram desde UI web

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-TG-005 |
| Modulo | TG |
| Endpoint | DELETE /api/v1/telegram/session |
| Actor | Paciente (UI web + API) |
| Prioridad | Usabilidad |
| Estado | **Implementado backend + frontend** via `UnlinkTelegramSessionCommand` + `TelegramPairingCard.tsx` (Phase 40, 2026-04-16) |

## Precondiciones detalladas
- JWT valido con `User.status=active`.
- Existe un `TelegramSession` activo (linked) para el `patient_id`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT | Extraido del claim; sesion activa requerida |

## Proceso (Happy Path)
1. Extraer `patient_id` del JWT.
2. Buscar `TelegramSession` con `status=linked` para ese paciente (`FindLinkedByPatientIdAsync`).
3. Si no existe, lanzar `BitacoraException("TG_SESSION_NOT_FOUND", 404)`.
4. Invocar `session.Unlink(nowUtc)` — soft delete: setea `unlinked_at`, cambia estado a `unlinked`.
5. `UpdateAsync` + `SaveChangesAsync` en transaccion.
6. Retornar `{ patient_id, unlinked_at_utc }` con HTTP 200.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| PatientId | uuid | ID del paciente desvinculado |
| UnlinkedAtUtc | timestamp | UTC del momento de desvinculacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| TG_SESSION_NOT_FOUND | 404 | No existe sesion activa | `{error: "TG_SESSION_NOT_FOUND"}` |
| TG_005_UNAUTHORIZED | 401 | JWT invalido o ausente | `{error: "TG_005_UNAUTHORIZED"}` |

## Casos especiales y variantes
- Soft delete: el registro `TelegramSession` NO se elimina fisicamente; se marca con `unlinked_at` y `status=unlinked`.
- Los recordatorios asociados quedan inactivos al desvincularse la sesion.
- Si el mismo `chat_id` re-vincula en el futuro, se crea una nueva sesion.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| TelegramSession | UPDATE | unlinked_at=now(), status='unlinked' |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente desvincula Telegram desde UI web
  Given paciente autenticado con sesion Telegram activa
  When DELETE /api/v1/telegram/session
  Then HTTP 200 con { patient_id, unlinked_at_utc }
  And TelegramSession.status = 'unlinked'
  And TelegramSession no eliminado fisicamente (soft delete)

Scenario: DELETE sin sesion activa
  Given paciente autenticado sin sesion Telegram vinculada
  When DELETE /api/v1/telegram/session
  Then HTTP 404 con error="TG_SESSION_NOT_FOUND"
```

## Trazabilidad
| Elemento | Referencia |
|----------|-----------|
| Flujo fuente | FL-TG-01 |
| Test plan | TP-TG (TG-P05, TG-N04) |
| Comando backend | `UnlinkTelegramSessionCommand` |
| Endpoint | `TelegramEndpoints.cs` MapDelete("/session") |
| Componente frontend | `TelegramPairingCard.tsx` (unlinkSection) |
| API client | `frontend/lib/api/client.ts` → `unlinkTelegram()` |
| Contrato tecnico | `09_contratos_tecnicos.md` — DELETE /api/v1/telegram/session |
