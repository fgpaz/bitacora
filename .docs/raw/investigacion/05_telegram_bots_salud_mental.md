# Telegram Bots para Salud Mental — Precedentes

## Bots existentes

| Bot | Repositorio | Caracteristicas clave |
|-----|------------|----------------------|
| **telegram-mood-tracker** | [twaslowski/telegram-mood-tracker](https://github.com/twaslowski/telegram-mood-tracker) | Tracker configurable, datos locales, integracion fluida |
| **MindMateBot** | [taj54/MindMateBot](https://github.com/taj54/MindMateBot) | Check-ins compasivos, mood tracking, tips de autocuidado |
| **MoodTrackerBot** | [pinae/MoodTrackerBot](https://github.com/pinae/MoodTrackerBot) | Pregunta 3 veces al dia, keyboard optimizado con descripciones numericas |
| **Faino** | (propietario) | Creado por psicologos profesionales, selecciona tecnicas apropiadas |

## Patrones UX que funcionan para registro rapido

1. **Keyboard inline** con opciones de humor (no texto libre) — reduce friccion a un tap
2. **Preguntas secuenciales** cortas (no un formulario largo de golpe) — max 5-6 pasos
3. **3 registros/dia** como frecuencia efectiva — manana, tarde, noche
4. **Emojis/iconos** para representar estados de animo — accesibilidad visual
5. **Confirmacion visual** inmediata del registro — feedback positivo

## Flujo de registro rapido propuesto para Bitacora

```
Bot: "Como te sentis ahora?"
[Keyboard inline: +3 | +2 | +1 | 0 | -1 | -2 | -3]

(Si es primer registro del dia)
Bot: "Cuantas horas dormiste anoche?"
[Keyboard: <4h | 4-6h | 6-8h | 8+h]

Bot: "Hiciste actividad fisica hoy?"
[Keyboard: Si | No]

Bot: "Tomaste la medicacion?"
[Keyboard: Si | No | No tomo]

Bot: "Registrado. Tu humor hoy: +1. Buen dia!"
```

## Alcance MVP del bot

**Confirmado:** carga rapida + recordatorios. La gestion principal queda en web.

El bot MVP incluye:
- Registro de humor (keyboard inline rapido)
- Registro de factores diarios (sueno, actividad, medicacion) — secuencial
- Recordatorios configurables (push a horarios elegidos)
- Confirmacion visual del registro

El bot MVP NO incluye:
- Consulta de graficos (eso es web)
- Gestion de cuenta o vinculo con profesional (eso es web)
- Form builder o tests clinicos
