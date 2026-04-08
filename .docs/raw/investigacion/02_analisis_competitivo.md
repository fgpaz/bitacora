# Analisis Competitivo — Apps de Mood Tracking

## Tabla comparativa

| App | Fortalezas | Debilidades | Sharing con profesional |
|-----|-----------|-------------|------------------------|
| **eMoods** | Escala bipolar (highs/lows), sueno, medicacion, reporte PDF mensual, datos 100% locales | Solo movil, no web, sin formularios custom | PDF por email al doctor |
| **Daylio** | UX micro-diario excelente, charts bonitos, actividades por iconos | No orientado a clinica, sin escala bipolar, no sharing nativo | Export CSV/charts manual |
| **Bearable** | Tracking holistico (humor+sintomas+estilo de vida), correlaciones automaticas | Complejidad abrumadora, freemium agresivo | Resumenes semanales/mensuales |
| **MoodChart.co** | Disenado para psiquiatras, timeline longitudinal clinico | Solo web, sin bot, sin auto-reporte del paciente | Acceso directo del psiquiatra |
| **Moodfit** | PHQ-9/GAD-7 integrados, CBT tools | Mas terapia que tracking | Limitado |

**Fuentes:**
- [eMoods Features](https://emoodtracker.com/features)
- [eMoods Compare](https://emoodtracker.com/other-mood-trackers)
- [Best Mood Tracking Apps 2026 - LifeStance Health](https://lifestance.com/blog/best-mood-tracking-apps-therapists-top-choices-2026/)
- [Top Mood Tracker Apps 2026 - Clustox](https://www.clustox.com/blog/mood-tracker-apps/)

## Gaps que Bitacora puede llenar

- **Ninguna app combina web + Telegram bot** para registro
- **Ninguna tiene formularios configurables** por el profesional
- **El sharing es primitivo** (PDF email, CSV export) — no hay acceso en tiempo real seguro
- **Falta modelo multi-tenant** donde el profesional administre pacientes
- **No hay apps en espanol** disenadas para el contexto clinico argentino/LATAM

## Implicaciones para el diseno

- La combinacion web + Telegram es un diferenciador real y no existe en el mercado.
- El modelo de sharing con vinculo persistente y consentimiento revocable supera a todos los competidores.
- El contexto argentino/LATAM en espanol es un nicho desatendido.
- La UX de Daylio (micro-diario rapido) y la escala de eMoods (bipolar con highs/lows) son los mejores referentes a combinar.
