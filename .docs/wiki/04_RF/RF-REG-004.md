# RF-REG-004: Verificar consent activo antes de registro

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-004 |
| Modulo | REG |
| Actor | Sistema (guardia de consent) |
| Flujo fuente | FL-REG-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- patient_id resuelto y autenticado via JWT o TelegramSession.
- Tabla ConsentGrant accesible en lectura.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT / TelegramSession | Requerido, existente |

## Proceso (Happy Path)
1. Consultar ConsentGrant WHERE patient_id = :patient_id AND status = 'granted'.
2. Si existe registro activo, continuar flujo.
3. Si no existe, abortar con 403 CONSENT_REQUIRED.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| consent_valid | bool | true si consent activo, false en caso contrario |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 403 | No existe ConsentGrant con status='granted' | {error: "CONSENT_REQUIRED"} |

## Casos especiales y variantes
- Consent revocado (status != 'granted'): mismo rechazo 403.
- Multiples ConsentGrant historicos: solo aplica el de status='granted' mas reciente.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | SELECT | status |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Consent activo permite continuar
  Given el patient tiene ConsentGrant con status='granted'
  When se verifica el consent
  Then consent_valid = true y el flujo continua

Scenario: Sin consent activo bloquea registro
  Given el patient no tiene ConsentGrant activo
  When se verifica el consent
  Then se retorna 403 CONSENT_REQUIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-004-01 | Patient con consent activo pasa la guardia | Positivo |
| TP-REG-004-02 | Patient sin consent retorna 403 | Negativo |
| TP-REG-004-03 | Patient con consent revocado retorna 403 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
