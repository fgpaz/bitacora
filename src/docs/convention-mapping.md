# Mapeo de Convenciones: landing -> Minimal API

Prerrequisito recomendado:
- Completar [Onboarding Dia 1](./first-day-onboarding.md).

## Mapa Visual
```mermaid
flowchart LR
    A[landingNomenclaturaArquetipo]
    A --> B[Api/Controllers]
    A --> C[Api/Automapper]
    A --> D[Application/Commands]
    A --> E[Application/Queries]
    A --> F[Application/Dtos]

    B --> B2[Api/Endpoints/<Dominio>]
    C --> C2[Api/Mappers]
    D --> D2[Se mantiene]
    E --> E2[Se mantiene]
    F --> F2[Se mantiene]
```

## Convenciones que se Mantienen
- `Api/ViewModels/<Controller>/<Operacion>/Input|Output`
- `Application/Commands/<Dominio>`
- `Application/Queries/<Dominio>`
- `Application/Dtos/<Operacion>/Input|Output`
- `DataAccess.*`, `Domain`, `EventBus`, `Infrastructure`, `Shared.*`

## Convenciones Adaptadas
- `Api/Controllers/*` -> `Api/Endpoints/<Dominio>/*`
- `Api/Automapper/*` -> `Api/Mappers/*` (Mapperly)

## Ejemplo de Nombres
- Operacion: `EditarUsuario`
- Endpoint: `MapPost("/api/v1/usuario/editar-usuario", ...)`
- Command: `EditarUsuarioCommand`
- Query: `GetUsuarioQuery`
