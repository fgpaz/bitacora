# Task T1: GAP-04 — Agregar HTTP header Retry-After en respuesta 429

## Shared Context
**Goal:** Resolver GAP-04: la respuesta 429 del rate limiter no incluye el header HTTP `Retry-After`, requerido por T3-RL-01.
**Stack:** .NET 10, ASP.NET Core rate limiter (`Microsoft.AspNetCore.RateLimiting`)
**Architecture:** `Program.cs` registra el rate limiter con `AddRateLimiter`. El callback `OnRejected` ya existe y escribe el body JSON. Solo falta agregar el header HTTP.

## Locked Decisions
- El valor de `Retry-After` es `60` (segundos fijos) — no dinámico.
- No cambiar el body JSON existente (ya incluye `"retryAfter":60`).
- No cambiar la política, el límite, ni el order del middleware en el pipeline.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Api/Program.cs:135-142
complexity: low
done_when: "dotnet build src/Bitacora.Api/ exits 0; curl concurrent test produce 429 con header Retry-After: 60"
```

## Reference
`src/Bitacora.Api/Program.cs:135-142` — bloque `OnRejected` actual:
```csharp
options.OnRejected = async (context, cancellationToken) =>
{
    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    context.HttpContext.Response.ContentType = "application/json";
    await context.HttpContext.Response.WriteAsync(
        """{"error":"RATE_LIMIT_EXCEEDED","message":"Demasiadas solicitudes. Intentá de nuevo en un minuto.","retryAfter":60}""",
        cancellationToken);
};
```

## Prompt
Abre `src/Bitacora.Api/Program.cs`. Localiza el bloque `options.OnRejected = async (context, cancellationToken) =>` (alrededor de las líneas 135-142).

El bloque actual escribe el status 429 y un body JSON, pero NO agrega el HTTP header `Retry-After`. Tu única tarea es agregar UNA línea:

```csharp
context.HttpContext.Response.Headers.RetryAfter = "60";
```

Esta línea debe ir DESPUÉS de `context.HttpContext.Response.ContentType = "application/json";` y ANTES del `WriteAsync`.

NO toques ninguna otra parte del archivo. NO cambies las políticas de rate limiting. NO cambies el body JSON.

Tras el edit, corre `dotnet build src/Bitacora.Api/ --no-restore` y confirma que retorna `Build succeeded`.

## Execution Procedure
1. Abrir `src/Bitacora.Api/Program.cs` y leer las líneas 130-145 para confirmar el bloque `OnRejected`.
2. Usar Edit tool para insertar `context.HttpContext.Response.Headers.RetryAfter = "60";` en la posición correcta.
3. Verificar que el bloque queda así:
   ```csharp
   options.OnRejected = async (context, cancellationToken) =>
   {
       context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
       context.HttpContext.Response.ContentType = "application/json";
       context.HttpContext.Response.Headers.RetryAfter = "60";
       await context.HttpContext.Response.WriteAsync(
           """{"error":"RATE_LIMIT_EXCEEDED","message":"Demasiadas solicitudes. Intentá de nuevo en un minuto.","retryAfter":60}""",
           cancellationToken);
   };
   ```
4. Correr `dotnet build src/Bitacora.Api/ --no-restore`.
5. Si build falla: revisar si el tipo de `Headers.RetryAfter` requiere casting. En ASP.NET Core la propiedad es `StringValues` — asignar `"60"` (string) es correcto.
6. Reportar si el archivo no coincide con lo esperado o si el build falla con error diferente al tipo.

## Skeleton
```csharp
options.OnRejected = async (context, cancellationToken) =>
{
    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    context.HttpContext.Response.ContentType = "application/json";
    context.HttpContext.Response.Headers.RetryAfter = "60";  // ← NUEVA LÍNEA
    await context.HttpContext.Response.WriteAsync(
        """{"error":"RATE_LIMIT_EXCEEDED","message":"Demasiadas solicitudes. Intentá de nuevo en un minuto.","retryAfter":60}""",
        cancellationToken);
};
```

## Verify
`dotnet build src/Bitacora.Api/ --no-restore` → `Build succeeded`

## Commit
`fix(rate-limit): add Retry-After: 60 HTTP header to 429 responses (T3-RL-01)`
