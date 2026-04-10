# CT-ERRORS: Catalogo de errores tipados reutilizados

> Root: `09_contratos_tecnicos.md`.

Este documento centraliza los errores reutilizados entre modulos. Los RF siguen siendo la fuente de verdad para codigos estrictamente locales de un endpoint o handler puntual.

## Reglas

1. Todo error expone `error.code`, `error.message` y `trace_id`.
2. Los codigos son estables; no se reciclan para semanticas distintas.
3. En handlers Telegram puede existir `HTTP 200` con error de negocio ya manejado mediante mensaje al usuario.

## Consentimiento y onboarding

| Codigo | HTTP | Uso canonico |
|--------|------|--------------|
| UNAUTHORIZED | 401 | JWT ausente o claims obligatorios faltantes |
| FORBIDDEN | 403 | El actor autenticado no cumple el rol esperado |
| PATIENT_NOT_FOUND | 404 | El JWT no resuelve un paciente local existente |
| INVALID_BODY | 400 | El endpoint esperaba JSON y no pudo leerlo |
| NO_CONSENT_CONFIG | 503 | No hay consentimiento activo configurado |
| CONSENT_REQUIRED | 403 | Operacion bloqueada por falta de consentimiento vigente |
| CONSENT_VERSION_MISMATCH | 409 | El cliente envio una version de consentimiento desactualizada |
| CONSENT_ALREADY_GRANTED | 409 | Ya existe grant vigente para esa version |
| ACCEPTED_FALSE | 422 | El cliente envio `accepted=false` en una operacion de otorgamiento |
| CONFIRMED_FALSE | 422 | El cliente envio `confirmed=false` en una revocacion |
| NO_ACTIVE_CONSENT | 404 | No existe consentimiento activo para revocar |
| REVOCATION_FAILED | 500 | La revocacion segura no pudo completarse |
| ONB_003_CONSENT_REQUIRED | 403 | Usuario `registered` intenta acceder a endpoints de datos |
| ONB_003_CONSENT_VERSION_MISSING | 400 | Consentimiento sin `version` en onboarding |
| ONB_003_ALREADY_GRANTED | 409 | Intento duplicado de otorgar consentimiento en onboarding |
| ONB_001_JWT_INVALID | 401 | El token no trae claims minimos para bootstrap |
| ONB_001_ENCRYPT_FAILED | 500 | Fallo al crear el usuario local de forma segura |

## Vinculos y acceso profesional

| Codigo | HTTP | Uso canonico |
|--------|------|--------------|
| CARELINK_EXISTS | 409 | Ya existe vinculo activo o invitado para la dupla profesional-paciente |
| PENDING_INVITE_EXISTS | 409 | Ya existe `PendingInvite` vigente para el mismo profesional y email hash |
| LINK_NOT_FOUND | 404 | `CareLink` inexistente |
| INVALID_STATUS | 409 | El estado del `CareLink` no permite la operacion solicitada |
| FORBIDDEN | 403 | El actor autenticado no es owner del recurso o no tiene permiso |
| VIS_011_NOT_LINKED | 403 | Resumen profesional sin `CareLink` visible |
| VIS_012_NOT_LINKED | 403 | Alertas profesionales sin `CareLink` visible |

## Binding codes

| Codigo | HTTP | Uso canonico |
|--------|------|--------------|
| INVALID_BINDING_CODE_TTL | 422 | `ttl_preset` fuera del catalogo permitido |
| BINDING_CODE_NOT_FOUND | 404 | BindingCode inexistente |
| BINDING_CODE_EXPIRED | 410 | BindingCode expirado |
| BINDING_CODE_ALREADY_USED | 409 | BindingCode ya consumido |
| CODE_GENERATION_FAILED | 500 | No se pudo generar un codigo unico |

## Telegram

| Codigo | HTTP | Uso canonico |
|--------|------|--------------|
| SESSION_NOT_LINKED | 200 | Webhook Telegram sin `TelegramSession` activa |
| TG_001_UNAUTHORIZED | 401 | JWT invalido al generar pairing code |
| TG_001_CONSENT_REQUIRED | 403 | Pairing code solicitado sin consentimiento vigente |
| TG_002_CODE_INVALID | 200 | `/start` con codigo invalido o formato incorrecto |
| TG_002_CODE_EXPIRED | 200 | `/start` con codigo vencido |
| TG_002_CHAT_DUPLICATE | 200 | `chat_id` ya vinculado a otra cuenta |

## Registro, export y seguridad

| Codigo | HTTP | Uso canonico |
|--------|------|--------------|
| INVALID_SCORE | 422 | `mood_score` fuera de rango |
| VALIDATION_ERROR | 422 | Error de validacion en `daily-checkins` |
| INVALID_SLEEP_HOURS | 422 | `sleep_hours` fuera de rango |
| MISSING_MEDICATION_TIME | 422 | `medication_taken=true` sin horario |
| INVALID_TIME_FORMAT | 422 | Hora de medicacion mal formada |
| ENCRYPTION_FAILURE | 500 | Fallo al cifrar o clave ausente |
| KEY_VERSION_INVALID | 500 | `key_version` inexistente o no utilizable |
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir `AccessAudit` |
| EXP_001_RANGE_INVALID | 400 | Rango invalido en export CSV |
| EXP_001_DECRYPT_FAILED | 500 | Fallo al descifrar durante export |

## Convenciones de mensaje

- Usar mensajes accionables y breves para el usuario final.
- En errores de seguridad o privacidad evitar revelar existencia de recursos salvo donde el contrato ya lo haga explicito.
- En errores webhook/Telegram privilegiar guidance al usuario y `HTTP 200` al proveedor cuando el caso de negocio ya fue absorbido.
