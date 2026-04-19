# Runbook: Professional Smoke User

## Propósito

El usuario profesional smoke permite ejecutar los tests del módulo VIN que requieren un profesional activo, específicamente VIN-P03 (binding code flow).

> Estado post Wave B: este runbook queda en transición. Las credenciales GoTrue previas no son runtime activo y no deben documentarse ni regenerarse en plaintext. El reemplazo debe crear usuarios de prueba en Zitadel y obtener tokens por flujo OIDC controlado, sin imprimir JWTs.

## Credenciales

Las credenciales viven en Infisical via `mi-key-cli`. No registrar passwords, PATs, JWTs ni secrets en este archivo.

## Cómo recrear si se pierde

Crear o reparar el usuario en la organización Zitadel `bitacora` usando el admin flow aprobado para Teslita. El usuario debe tener rol `professional` en el project Bitacora (`369306332534145382`). Guardar cualquier secreto de prueba solo en Infisical.

## Verificación

Para verificar que el usuario funciona:

- Iniciar sesión por OIDC en `https://bitacora.nuestrascuentitas.com/ingresar`.
- Validar que el rol profesional permita acceder a `/profesional`.
- Validar bootstrap/API con un token real obtenido por flujo seguro; no pegar el token en logs ni evidencia.

## Historial

- 2026-04-15: Usuario creado para habilitar GAP-03 (VIN-P03 test case)
- 2026-04-19: Wave B cambió el runtime activo a Zitadel. GoTrue queda legacy/rollback.
