# NuestrasCuentitas.Bitacora

Microservicio generado con `sandinas-ms10` (.NET 10 full skeleton).

## Documentacion Operativa Incluida
- [Onboarding Dia 1](./docs/first-day-onboarding.md)
- [Flujo Diario y Buenas Practicas](./docs/daily-workflow-and-coding-practices.md)
- [Mapeo de Convenciones (landing -> Minimal API)](./docs/convention-mapping.md)

## Ruta Sugerida para Empezar
1. Leer [Onboarding Dia 1](./docs/first-day-onboarding.md).
2. Ejecutar comandos base y levantar la API.
3. Seguir [Flujo Diario y Buenas Practicas](./docs/daily-workflow-and-coding-practices.md).
4. Aplicar [Mapeo de Convenciones](./docs/convention-mapping.md) en cada cambio.

## Comandos Base
```powershell
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run --project Bitacora.Api
```

## Configuracion Local Minima
- PostgreSQL local accesible con la cadena `ConnectionStrings:BitacoraDb`.
- `ZITADEL_AUTHORITY` y `ZITADEL_AUDIENCE` para validar JWT RS256 via metadata/JWKS.
- `BITACORA_ENCRYPTION_KEY` o `Encryption:Key` con 32 bytes exactos para AES-256-GCM.
- `BITACORA_PSEUDONYM_SALT` o `Pseudonymization:Salt` para seudonimizacion.
- `Consent:*` vive en `appsettings*.json` y versiona el consentimiento activo.

El proyecto incluye placeholders de desarrollo en `Bitacora.Api/appsettings*.json` y un ejemplo de variables en `Bitacora.Api/.env.example`. `SUPABASE_JWT_SECRET` queda solo para rollback de builds anteriores al cutover Zitadel.

## Runtime Local
```powershell
dotnet build Bitacora.sln
dotnet ef migrations add InitialCore --project Bitacora.DataAccess.EntityFramework --startup-project Bitacora.Api --output-dir Persistence/Migrations
dotnet run --project Bitacora.Api
```

`DataAccess:ApplyMigrationsOnStartup` queda en `false` por defecto para no bloquear el arranque si PostgreSQL no esta disponible todavia.

## Nota de Mensajeria
- `EventBusSettings:HostAddress` se entrega vacio por defecto para que el micro pueda arrancar sin RabbitMQ local.
- Para habilitar MassTransit + RabbitMQ, definir `EventBusSettings` en `appsettings*.json` o variables de entorno.
