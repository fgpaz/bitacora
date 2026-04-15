# Task T2: GAP-05 — Fix key mismatch en mapeo de safe_projection en timeline y summary

## Shared Context
**Goal:** Resolver GAP-05: los handlers de timeline y summary buscan claves incorrectas en `safe_projection` JSONB, por lo que todos los factores binarios de DailyCheckin devuelven `null`.
**Stack:** .NET 10, `System.Text.Json`, PostgreSQL JSONB
**Architecture:** `safe_projection` se almacena con claves `has_*` (ej: `has_medication`). Los handlers de visualización leen claves sin prefijo (ej: `medication_taken`). Es un key mismatch puro — sin cambio de modelo ni de DB.

## Locked Decisions
- NO cambiar el schema de `safe_projection` en DB (ya tiene los datos correctos con claves `has_*`).
- NO cambiar el UpsertDailyCheckinCommandHandler (ya escribe correctamente con claves `has_*`).
- Cambiar SOLO los dos query handlers de visualización para que lean las claves correctas.
- Los DTOs de respuesta NO cambian (ya usan `PhysicalActivity`, `MedicationTaken`, etc.).

## Task Metadata
```yaml
id: T2
depends_on: []
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Application/Queries/Visualizacion/GetPatientTimelineQuery.cs:75-115
  - modify: src/Bitacora.Application/Queries/Visualizacion/GetPatientSummaryQuery.cs:54-68
  - read: src/Bitacora.Application/Commands/Registro/UpsertDailyCheckinCommand.cs:71-81
complexity: low
done_when: "dotnet build exits 0; ambos archivos usan has_physical, has_social, has_anxiety, has_irritability, has_medication"
```

## Reference
**Cómo se escribe safe_projection (correcto — NO cambiar):**
`src/Bitacora.Application/Commands/Registro/UpsertDailyCheckinCommand.cs:71-81`
```csharp
var safeProjection = JsonSerializer.Serialize(new
{
    sleep_hours = command.SleepHours,
    has_physical = command.PhysicalActivity,
    has_social = command.SocialActivity,
    has_anxiety = command.Anxiety,
    has_irritability = command.Irritability,
    has_medication = command.MedicationTaken
});
```

**Cómo se lee ACTUALMENTE (incorrecto — lo que hay que cambiar):**
`src/Bitacora.Application/Queries/Visualizacion/GetPatientTimelineQuery.cs:101-104`
```csharp
if (root.TryGetProperty("medication_taken", out var mt) && mt.ValueKind == JsonValueKind.True)
    medication = true;
else if (mt.ValueKind == JsonValueKind.False)
    medication = false;
```

## Prompt
Hay un key mismatch entre cómo se ESCRIBE `safe_projection` y cómo se LEE. La tabla de correcciones obligatorias es:

| Clave buscada actualmente (INCORRECTA) | Clave correcta en safe_projection |
|---------------------------------------|----------------------------------|
| `"physical_activity"` | `"has_physical"` |
| `"social_activity"` | `"has_social"` |
| `"anxiety"` | `"has_anxiety"` |
| `"irritability"` | `"has_irritability"` |
| `"medication_taken"` | `"has_medication"` |

**Archivo 1:** `src/Bitacora.Application/Queries/Visualizacion/GetPatientTimelineQuery.cs`
- Leer las líneas 75-115 (método `ToDto` o similar que deserializa safe_projection de DailyCheckin)
- Reemplazar TODAS las ocurrencias de las claves incorrectas por las correctas
- `sleep_hours` ya es correcto — NO cambiar

**Archivo 2:** `src/Bitacora.Application/Queries/Visualizacion/GetPatientSummaryQuery.cs`
- Leer las líneas 54-68 (método que agrega contadores de factores binarios)
- Aplicar las mismas correcciones de claves

NO cambies ninguna otra parte. NO cambies los DTOs de respuesta. NO cambies la lógica de lectura de `mood_score` (esa clave es correcta tal cual).

Tras los edits, correr `dotnet build src/Bitacora.Application/ --no-restore` y confirmar `Build succeeded`.

## Execution Procedure
1. Abrir `src/Bitacora.Application/Queries/Visualizacion/GetPatientTimelineQuery.cs` y leer líneas 75-115.
2. Identificar cada `TryGetProperty("clave_incorrecta", ...)` para los 5 factores binarios.
3. Usar Edit tool para corregir cada clave: reemplazar `"physical_activity"` → `"has_physical"`, `"social_activity"` → `"has_social"`, `"anxiety"` → `"has_anxiety"`, `"irritability"` → `"has_irritability"`, `"medication_taken"` → `"has_medication"`.
4. Abrir `src/Bitacora.Application/Queries/Visualizacion/GetPatientSummaryQuery.cs` y leer líneas 54-68.
5. Aplicar las mismas correcciones de claves en ese archivo.
6. Correr `dotnet build src/Bitacora.Application/ --no-restore`.
7. Parar y reportar si algún archivo tiene una estructura de lectura diferente a la esperada (ej: usa LINQ en vez de TryGetProperty — en ese caso, reportar el patrón exacto).

## Skeleton
```csharp
// ANTES (incorrecto):
if (root.TryGetProperty("medication_taken", out var mt) && mt.ValueKind == JsonValueKind.True)
    medication = true;

// DESPUÉS (correcto):
if (root.TryGetProperty("has_medication", out var mt) && mt.ValueKind == JsonValueKind.True)
    medication = true;

// Aplicar el mismo patrón para:
// "physical_activity"  → "has_physical"
// "social_activity"    → "has_social"
// "anxiety"            → "has_anxiety"
// "irritability"       → "has_irritability"
```

## Verify
`dotnet build src/Bitacora.Application/ --no-restore` → `Build succeeded`

## Commit
`fix(visualizacion): correct safe_projection key names in timeline and summary handlers (GAP-05)`
