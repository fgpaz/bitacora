# RF-EXP-003: Stream response para datasets grandes

## Execution Sheet
- Modulo: EXP
- Mecanismo: IAsyncEnumerable<CsvRow> → response HTTP chunked
- Actor: Sistema (invocado por RF-EXP-001)
- Prioridad PDP: Correctness > Usability
- Estado: **Diferido — streaming CSV no implementado.** El endpoint `GET /api/v1/export/patient-summary` retorna un DTO JSON que se bufferea completamente en memoria antes de enviarse.

## Precondiciones detalladas
- El endpoint de exportacion no debe buffear el dataset completo en memoria
- Descifrado por registro (RF-EXP-002) ocurre a medida que se itera
- Si el descifrado falla en cualquier punto del stream, el stream se cierra con error

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| query_stream | IAsyncEnumerable<RawRecord> | Stream de registros desde DB |
| from / to | date | Parametros del rango (ya validados) |

## Proceso (Happy Path)
1. Abrir conexion DB con query tipo CURSOR o similar (no `ToList()`)
2. Escribir header CSV al stream de response inmediatamente
3. Iterar `await foreach (var record in queryStream)`:
   a. Invocar RF-EXP-002 para descifrar record
   b. Serializar a fila CSV
   c. Escribir fila al stream de response con `await writer.WriteLineAsync(row)`
   d. Hacer `await writer.FlushAsync()` periodicamente (cada 50 filas)
4. Cerrar stream al finalizar iteracion

## Outputs
- HTTP response chunked transfer encoding
- El cliente comienza a recibir datos antes de que el servidor termine de procesar
- No hay JSON envelope; solo CSV puro

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| EXP_003_STREAM_INTERRUPTED | Conexion cliente cerrada; abortar iteracion silenciosamente |
| EXP_003_DECRYPT_FAILED | Propagado desde RF-EXP-002; cerrar stream con error |

## Casos especiales y variantes
- Cliente desconecta a mitad del stream: detectar via `CancellationToken`, no loguear como error
- Error de descifrado a mitad del stream: ya se enviaron filas parciales; cerrar stream y loguear
- Dataset vacio: solo se envia header y se cierra stream (HTTP 200, no error)

## Impacto en modelo de datos
- Solo lectura. El query debe usar iteracion server-side (no cargar todo en RAM)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Export de 10000 registros no satura memoria
  Given un paciente con 10000 mood_entries
  When GET /api/v1/export/csv
  Then el servidor nunca tiene mas de 200 registros en RAM simultaneamente
  And el cliente recibe el CSV completo

Scenario: Error de descifrado en fila 500 cierra el stream
  Given la fila 500 tiene key_version desconocida
  When el stream llega a esa fila
  Then el stream se cierra y el cliente recibe respuesta truncada

Scenario: Cliente desconecta a mitad del stream
  Given un export en progreso
  When el cliente cierra la conexion
  Then el servidor cancela la iteracion sin loguear error
```

## Trazabilidad de tests
- UT: EXP003_Iterator_DoesNotCallToList
- IT: EXP003_LargeDataset_LowMemoryFootprint
- IT: EXP003_ClientDisconnect_CancelledGracefully

## Sin ambiguedades pendientes
- `FlushAsync` cada 50 filas es el intervalo por defecto; no configurable por cliente
- Una vez que el header CSV fue enviado, el HTTP status code ya es 200; errores posteriores truncan el stream
