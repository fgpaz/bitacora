# Formularios Clinicos Configurables

> **Nota:** Esta capacidad esta FUERA del MVP. Se documenta como insumo para Fase 2.

## Soluciones existentes

| Solucion | Tipo | Fortalezas | Debilidades |
|----------|------|-----------|-------------|
| **Jotform** | Builder de formularios sin codigo | Facil de usar, muchos templates | No clinico, no integrado a tracking |
| **ICANotes** | EHR con formularios | 150+ formularios pre-configurados para salud mental | Costoso, cerrado, no LATAM |
| **Formsort** | Templates clinicos | PHQ-9, GAD-7 validados | SaaS externo, no self-hosted |

## Diseno para Fase 2

### Form builder simple para profesionales
- Campos tipo: escala (numerica), si/no (booleano), texto libre, numero, hora/rango horario
- Drag-and-drop o lista secuencial de campos
- Preview antes de asignar

### Templates pre-cargados
- Mood chart diario (el formulario fijo del MVP, ahora como template)
- PHQ-9 (Patient Health Questionnaire-9)
- GAD-7 (Generalized Anxiety Disorder-7)
- Escala de ansiedad personalizada
- Tests de unico uso (sin periodicidad)

### Periodicidad configurable
- Diario (default para mood tracking)
- Semanal
- Mensual
- Unico (test de una sola vez, como PHQ-9 de ingreso)

### Recordatorios automaticos
- Push web (notificacion del navegador)
- Mensaje Telegram (si el paciente tiene bot vinculado)
- Horarios configurados por el paciente o el profesional

### Scoring automatico
- Para escalas estandarizadas (PHQ-9, GAD-7) con puntuacion y categorizacion
- Visualizacion de tendencia del score a lo largo del tiempo

## Implicaciones para la arquitectura MVP

Aunque el form builder es Fase 2, la arquitectura del MVP debe contemplar el "seam" de evolucion:
- El formulario fijo del MVP debe modelarse internamente como un formulario con schema fijo
- La entidad DailyCheckin debe poder evolucionar a una entidad FormResponse generica
- El modelo de datos debe soportar campos dinamicos sin romper el schema existente
