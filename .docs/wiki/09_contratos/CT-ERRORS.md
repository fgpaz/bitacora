# CT-ERRORS: Catalogo de Errores Tipados

> Root: `09_contratos_tecnicos.md` — seccion Patron de errores.
> Este documento centraliza los errores reutilizados entre modulos. Los RF locales son fuente de verdad para codigos puntuales.

---

## Envolvente de Error

Todo error HTTP (excepto webhooks Telegram) sigue este envelope:

```json
{
  "error": {
    "code": "CODIGO",
    "message": "Mensaje accionable.",
    "trace_id": "uuid"
  }
}
```

### Reglas fundamentales

1. Todo error expone `code`, `message` y `trace_id`.
2. Los codigos son estables; no se reciclan para semanticas distintas.
3. Errores de seguridad/privacidad nunca revelan existencia de recursos.
4. **Fail-closed por defecto:** cualquier codigo no listado aqui retornado por un handler nuevo debe ser tratado como error interno (500).

### Webhook Telegram

Los handlers Telegram pueden retornar `HTTP 200` con error de negocio ya manejado. Esto evita reintentos de Telegram para errores que el bot ya resolvio con mensaje al usuario.

---

## Errores Transversales (activos)

| Codigo | HTTP | Uso canonico | Handler/Source |
|--------|------|--------------|----------------|
| UNAUTHORIZED | 401 | JWT ausente o claims obligatorios faltantes | JwtBearer middleware |
| FORBIDDEN | 403 | Actor autenticado fuera del rol esperado | Varios handlers |
| PATIENT_NOT_FOUND | 404 | JWT resolve a usuario local inexistente | Bootstrap, Consent |
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido | ConsentEndpoints |
| VALIDATION_ERROR | 422 | Error de validacion de request body | Varios handlers |
| NO_CONSENT_CONFIG | 503 | No hay consentimiento activo configurado | ConsentEndpoints |
| UNEXPECTED_ERROR | 500 | Excepcion no manejada | ApiExceptionMiddleware |

---

## Consentimiento y Onboarding (activos)

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| CONSENT_REQUIRED | 403 | Operacion bloqueada por falta de consentimiento vigente | ConsentRequiredMiddleware |
| CONSENT_VERSION_MISMATCH | 409 | El cliente envio version desactualizada | GrantConsentCommand |
| CONSENT_ALREADY_GRANTED | 409 | Ya existe grant vigente para esa version | GrantConsentCommand |
| ACCEPTED_FALSE | 422 | `accepted=false` en operacion de otorgamiento | GrantConsentCommand |
| CONFIRMED_FALSE | 422 | `confirmed=false` en operacion de revocacion | RevokeConsentCommand |
| NO_ACTIVE_CONSENT | 404 | No existe consentimiento activo para revocar | RevokeConsentCommand |
| REVOCATION_FAILED | 500 | La revocacion segura no pudo completarse | RevokeConsentCommand |
| ONB_001_JWT_INVALID | 401 | Token sin claims minimos para bootstrap | BootstrapPatientCommand |
| ONB_001_ENCRYPT_FAILED | 500 | Fallo al crear usuario local de forma segura | BootstrapPatientCommand |
| ONB_003_CONSENT_REQUIRED | 403 | Usuario `registered` intenta acceder a endpoints de datos | ConsentRequiredMiddleware |
| ONB_003_CONSENT_VERSION_MISSING | 400 | Consentimiento sin `version` en onboarding | GrantConsentCommand |
| ONB_003_ALREADY_GRANTED | 409 | Intento duplicado de otorgar consentimiento en onboarding | GrantConsentCommand |

---

## Registro (activos)

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| INVALID_SCORE | 422 | `mood_score` fuera de rango [-3, 3] | CreateMoodEntryCommand |
| VALIDATION_ERROR | 422 | Error de validacion en daily-checkins | CreateOrUpdateDailyCheckinCommand |
| INVALID_SLEEP_HOURS | 422 | `sleep_hours` fuera de rango | CreateOrUpdateDailyCheckinCommand |
| MISSING_MEDICATION_TIME | 422 | `medication_taken=true` sin horario | CreateOrUpdateDailyCheckinCommand |
| INVALID_TIME_FORMAT | 422 | Hora de medicacion mal formada | CreateOrUpdateDailyCheckinCommand |

---

## Auditoria y Cifrado (activos)

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir AccessAudit (fail-closed) | AccessAuditMiddleware, handlers |
| ENCRYPTION_FAILURE | 500 | Fallo de cifrado o clave no disponible | CryptographyService |
| KEY_VERSION_INVALID | 500 | `key_version` inexistente o no utilizable | CryptographyService |
| PSEUDONYM_SALT_MISSING | 500 | Variable BITACORA_PSEUDONYM_SALT no disponible (fail-closed) | PseudonymizationService |

---

## Vinculos — Implementado en Wave 30

> Modulo VIN existe en runtime. Estos codigos son **activos** y alineados con `RevokeCareLinkCommand`, `AcceptCareLinkCommand`, y `UpdateCareLinkCanViewDataCommand`.

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| BINDING_CODE_REQUIRED | 400 | `bindingCode` vacio o ausente en accept | AcceptCareLinkCommand |
| BINDING_CODE_INVALID_OR_EXPIRED | 410 | BindingCode no existe, usado, o vencido | AcceptCareLinkCommand |
| CARE_LINK_ALREADY_EXISTS | 409 | Ya existe CareLink activo con el profesional | AcceptCareLinkCommand |
| CONFIRMED_FALSE | 422 | Revocacion sin `confirmed=true` | RevokeCareLinkCommand |
| CARE_LINK_ID_REQUIRED | 400 | `careLinkId` vacio en revocation | RevokeCareLinkCommand |
| CARE_LINK_NOT_FOUND | 404 | CareLink inexistente | RevokeCareLinkCommand, UpdateCareLinkCanViewDataCommand |
| NOT_YOUR_CARE_LINK | 403 | Actor no es owner del vinculo | RevokeCareLinkCommand, UpdateCareLinkCanViewDataCommand |
| CARE_LINK_NOT_REVOKABLE | 422 | CareLink no esta en estado `invited` o `active` | RevokeCareLinkCommand |
| REVOKE_CARE_LINK_FAILED | 500 | Fallo transaccional durante revocacion | RevokeCareLinkCommand |
| ACCEPT_CARE_LINK_FAILED | 500 | Fallo transaccional durante aceptacion | AcceptCareLinkCommand |
| PROFESSIONAL_ACCESS_DENIED | 403 | Profesional intenta acceder sin `can_view_data=true` | ProfessionalDataAccessAuthorizer |

### Binding Codes

| Codigo | HTTP | Uso canonico | Estado |
|--------|------|--------------|--------|
| BINDING_CODE_NOT_FOUND | 404 | BindingCode inexistente | Activo (verificado en handler) |
| BINDING_CODE_EXPIRED | 410 | BindingCode vencido | Activo (verificado en handler) |
| BINDING_CODE_ALREADY_USED | 409 | BindingCode ya consumido | Activo (verificado en handler) |
| CODE_GENERATION_FAILED | 500 | No se pudo generar codigo unico | Activo |

### Superficie canonica diferida

| Codigo | HTTP | Uso canonico | Estado |
|--------|------|--------------|--------|
| CARELINK_EXISTS | 409 | Ya existe vinculo activo o invitado para la dupla | Diferido |
| PENDING_INVITE_EXISTS | 409 | Ya existe PendingInvite vigente para el mismo email hash | Diferido |
| LINK_NOT_FOUND | 404 | CareLink inexistente | Diferido |
| INVALID_STATUS | 409 | El estado del CareLink no permite la operacion | Diferido |
| INVALID_EMAIL_FORMAT | 422 | `patient_email` invalido | Diferido |

---

## Telegram — Implementado (Phase 31+)

> Los endpoints de Telegram REST (pairing, session, webhook) estan materializados.

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| SESSION_NOT_LINKED | 200 | Webhook sin sesion vinculada (ya manejado con mensaje) | HandleWebhookUpdateCommand |
| TG_001_UNAUTHORIZED | 401 | JWT invalido al generar pairing code | GeneratePairingCodeCommand |
| TG_001_CONSENT_REQUIRED | 403 | Pairing code solicitado sin consentimiento vigente | GeneratePairingCodeCommand |
| TG_002_CODE_INVALID | 200 | `/start` con codigo invalido o formato incorrecto | HandleWebhookUpdateCommand |
| TG_002_CODE_EXPIRED | 200 | `/start` con codigo vencido | HandleWebhookUpdateCommand |
| TG_002_CHAT_DUPLICATE | 200 | `chat_id` ya vinculado a otra cuenta | HandleWebhookUpdateCommand |
| TG_003_ALREADY_LINKED | 200 | Chat ya tiene sesion activa | HandleWebhookUpdateCommand |
| TG_006_NO_ACTIVE_SESSION | 403 | Configuracion de recordatorio sin TelegramSession linked | ConfigureReminderScheduleCommandHandler |
| TG_006_INVALID_HOUR | 400 | `hourUtc` fuera de 0..23 | ConfigureReminderScheduleCommandHandler |
| TG_006_INVALID_MINUTE | 400 | `minuteUtc` distinto de 0 o 30 | ConfigureReminderScheduleCommandHandler |
| TG_006_INVALID_TIMEZONE | 400 | Timezone invalido o no soportado | ConfigureReminderScheduleCommandHandler |
| PAIRING_CODE_GENERATION_FAILED | 500 | Colision de codigo tras 5 intentos | GeneratePairingCodeCommand |
| PATIENT_ID_REQUIRED | 400 | patient_id vacio | Telegram handlers |
| FORBIDDEN | 403 | Secret token de webhook invalido | TelegramEndpoints webhook |

---

## Visualizacion y Export — Implementado (Phase 31+)

> Endpoints de visualizacion paciente y profesional implementados. Export CSV owner-only implementado.

| Codigo | HTTP | Uso canonico | Handler |
|--------|------|--------------|---------|
| INVALID_DATE_RANGE | 400 | `from > to` en consulta de timeline, summary, export | VisualizacionEndpoints, ExportEndpoints |
| PROFESSIONAL_ACCESS_DENIED | 403 | Acceso a dashboard/alertas sin CareLink con `can_view_data=true` | ProfessionalDataAccessAuthorizer |
| UNAUTHORIZED | 401 | JWT invalido o expirado | JwtBearer middleware |
| VIS_001_RANGE_TOO_LARGE | 400 | Rango > 90 dias; usar paginacion | VisualizacionEndpoints (future) |

### Deferred

| Codigo | HTTP | Uso canonico | Estado |
|--------|------|--------------|--------|
| EXP_001_DECRYPT_FAILED | 500 | Fallo al descifrar durante export (fail-closed) | Diferido |
| VIS_001_RANGE_TOO_LARGE | 400 | Rango > 90 dias (paginacion) | Diferido |

---

## Convenciones de Mensaje

- Mensajes accionables y breves para el usuario final.
- Errores de seguridad/privacidad: no revelar existencia de recursos salvo donde el contrato ya lo haga explicito.
- Errores webhook/Telegram: privilegiar guidance al usuario y `HTTP 200` al proveedor cuando el caso de negocio ya fue absorbido.
- No incluir datos derivables de la estructura de la base de datos en mensajes de error.

---

## Sincronizacion

Cambios en este catalogo fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion Patron de errores)
- Los RF asociados a cada codigo si cambia semantica o HTTP status
- Los TP correspondientes si cambia la expectativa de error
