# DESIGN.md

> Archivo derivado y regenerable desde el wiki. No editar manualmente.
> Autoridad canónica: `.docs/wiki/11`, `.docs/wiki/12`, `.docs/wiki/13`, `.docs/wiki/16`, `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`.

## Overview
- Bitácora debe sentirse como **refugio personal** con tono **editorial íntimo** y desempate constante **claridad con calidez**.
- La historia que lidera es **registro personal**. La jerarquía visible debe seguir **contexto -> acción -> confianza**, con **una acción dominante + secundaria silenciosa** y señales de confianza **solo contextual**.
- La interfaz nace **mobile-first paciente + expansión selectiva profesional**, mantiene **aire generoso**, trabaja estados **integrados con escalamiento** y trata accesibilidad como **baseline explícito**.
- La traducción a Stitch debe ser **reglada con margen mínimo**. No se permiten reinterpretaciones que rompan la familia visual, la voz ni la lógica de control del producto.

## Colors
| Token | Dirección | Uso |
| --- | --- | --- |
| --brand-primary | verde salvia profundo #5E766E | identidad principal, acciones serenas, estados de continuidad |
| --brand-secondary | piedra rosada apagada #C8B7AE | apoyo visual, fondos secundarios, capas suaves |
| --brand-accent | terracota sobria #B67864 | énfasis puntual, highlight humano, detalles editoriales |
| --surface | marfil papel #F6F1EA | fondo base, superficies principales, sensación de cuaderno |
| --surface-muted | arena cálida #E8DED3 | contenedores, agrupaciones, zonas de apoyo |
| --foreground | tinta cálida #2E2A28 | texto principal, íconos, contraste estructural |
| --foreground-muted | grafito suave #655E59 | texto secundario, metadata, ayudas |
| --border-subtle | línea de papel #D7CCC1 | divisores, bordes livianos, campos tranquilos |

## Typography
| Rol | Familia canónica | Uso |
| --- | --- | --- |
| Display | Newsreader | títulos, frases de apertura, citas breves, momentos de presencia editorial |
| Body | Source Sans 3 | lectura principal, formularios, navegación, contenido de uso cotidiano |
| Mono | IBM Plex Mono | fechas, metadata, trazabilidad, datos técnicos o auditorables |

## Elevation
- Profundidad baja y silenciosa.
- Sombras mínimas y difusas; bordes sutiles antes que recortes duros.
- Superficies cálidas con textura editorial suave y brillo mínimo.
- El color acompaña la tarea; no debe convertirse en ruido dominante.

## Components
| Familia | Propósito | Estados mínimos |
| --- | --- | --- |
| PatientPageShell | shell editorial paciente | loading, ready, empty, error |
| ProfessionalPageShell | shell profesional sobria | loading, ready, empty, error |
| SectionHeader | encabezados de tarea o lectura | default |
| SensitiveContainer | consentimiento, acceso, revocación, exportación | default, warning, error |
| QuickValueField | gesto instantáneo como mood scale | default, submitting, error, confirm |
| GroupedCheckinForm | formularios breves por bloques | partial, conditional, ready, submitting, error, confirm |
| SaveRail | cierre de decisión o guardado | ready, disabled, submitting, error |
| InlineFeedback | confirmación factual o error localizado | confirm, error, info |
| AccessControlBlock | visibilidad, vínculo, revocación | default, changed, conflict, revoked |
| TimelinePanel | lectura longitudinal | loading, ready, empty, error |
| BindingCodePanel | auto-vinculación paciente-profesional por código | idle, code, invalid, expired, success, error |
| TelegramCodeBridgePanel | pairing Telegram y códigos temporales de canal | idle, code, expired, linked, error |
| TrustContextPanel | aviso puntual de confianza/consentimiento en tareas críticas | default, contextual_trust |

## Do's and Don'ts
### Do
- Mantener una acción dominante y una secundaria silenciosa en la misma pantalla.
- Usar aire generoso, profundidad baja y contenedores con suavidad contenida.
- Hacer visibles los límites de acceso solo cuando el contexto lo requiere.
- Conservar confirmaciones y vacíos en tono sereno y humano.
- Mantener contenido visible de ejemplo cotidiano y concreto.
- Usar la gramática de componentes ya definida en el sistema frontend y los patrones seed del canon, sin inventar familias paralelas.

### Don't
- No parecer dashboard SaaS, consola de monitoreo, app de wellness ni interfaz hospitalaria fría.
- No usar celebraciones, elogios, confeti, rebotes, pulsos persistentes ni CTAs que griten.
- No dejar paneles permanentes de confianza o privacidad compitiendo con el contenido principal.
- No introducir más de dos zonas funcionales visibles en desktop profesional.
- No usar el pack derivado como nueva autoridad editorial: siempre manda el wiki.

## Source of Truth
- `.docs/wiki/11_identidad_visual.md`
- `.docs/wiki/12_lineamientos_interfaz_visual.md`
- `.docs/wiki/13_voz_tono.md`
- `.docs/wiki/16_patrones_ui.md`
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
