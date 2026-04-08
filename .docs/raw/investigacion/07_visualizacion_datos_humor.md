# Visualizacion de Datos de Humor

## Tipos de graficos clinicamente utiles

### 1. Timeline longitudinal (estilo NIMH-LCM)
- Eje X = dias (con scroll horizontal para periodos largos)
- Eje Y = -3 a +3
- Linea de humor con color por zona (rojo para depresion intensa, verde para eutimia, amarillo para mania)
- Marcadores para eventos significativos (cambio de medicacion, sesion con profesional)
- **Prioridad:** alta — es el grafico principal del producto

### 2. Heatmap calendario
- Vista mensual con colores por intensidad de humor
- Similar a GitHub contribution graph
- Permite detectar patrones semanales (ej: lunes siempre bajo)
- **Prioridad:** media — complemento visual util

### 3. Correlacion sueno-humor
- Scatter plot o dual-axis chart
- Eje X = horas de sueno, Eje Y = humor promedio del dia
- Permite al paciente y profesional ver si hay correlacion
- **Prioridad:** media — insight clinico valioso

### 4. Barras de factores
- Actividad fisica, social, medicacion como barras apiladas bajo el timeline
- Permite ver factores asociados al humor de cada dia
- **Prioridad:** media — complemento del timeline

### 5. Resumen mensual PDF
- Para compartir con profesional como documento imprimible
- Incluye timeline + estadisticas resumen + notas
- **Prioridad:** alta si export PDF entra en MVP

### 6. Dashboard del profesional
- Vista multi-paciente con alertas (ej: paciente X lleva 3 dias en -3)
- Ranking de pacientes que requieren atencion
- **Prioridad:** alta para el profesional — diferenciador vs competencia

## Validacion clinica

**ChronoRecord** (20 anos de uso clinico) confirma que el charting digital longitudinal es superior al papel para detectar patrones en trastornos afectivos.

**Fuente:** [ChronoRecord 20 Years - PMC](https://pmc.ncbi.nlm.nih.gov/articles/PMC10484643/)

## Tecnologias de visualizacion a evaluar

- **Recharts** (React): simple, declarativo, buen soporte de responsive
- **Chart.js** (vanilla): ligero, amplia comunidad
- **D3.js**: maximo control, curva de aprendizaje alta
- **Nivo** (React + D3): bonito por defecto, buen heatmap calendar

La decision tecnologica se toma en la fase de arquitectura (T4).
