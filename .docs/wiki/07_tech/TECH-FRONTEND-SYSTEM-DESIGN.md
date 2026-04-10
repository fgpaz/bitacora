# TECH-FRONTEND-SYSTEM-DESIGN — Sistema frontend de Bitácora

## Propósito

Este documento define la capa de diseño de sistema frontend para Bitácora en etapa `docs-first`.

No reemplaza `11_identidad_visual.md`, `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md`, `16_patrones_ui.md` ni futuros `UI-RFC-*`. Su función es traducir el canon global y las referencias Stitch a una gramática técnica reusable para la futura capa Next.js 16.

No autoriza implementación por sí solo. Tampoco equivale a validación UX real.

## Relación con el canon

Este documento depende de:

- `../02_arquitectura.md`
- `../04_RF.md`
- `../07_baseline_tecnica.md`
- `../09_contratos_tecnicos.md`
- `../11_identidad_visual.md`
- `../12_lineamientos_interfaz_visual.md`
- `../13_voz_tono.md`
- `../14_metodo_prototipado_validacion_ux.md`
- `../16_patrones_ui.md`
- `../21_matriz_validacion_ux.md`
- `../23_uxui/INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`

No debe contradecir:

- la prioridad de decisión del proyecto;
- la postura `no-dashboard` para experiencia paciente;
- el bloqueo estricto de consentimiento;
- la semántica de errores y `trace_id`;
- el uso de Stitch como autoridad visual primaria en esta etapa.

## Alcance y prioridades de decisión

La futura UI de Bitácora debe resolverse con este orden:

1. Security
2. Privacy
3. Correctness
4. Usability
5. Maintainability
6. Performance
7. Cost
8. Time-to-market

Esto aplica tanto a jerarquía visual como a comportamientos, estados y manejo de errores.

## Jerarquía de autoridad visual y documental

Durante esta etapa la precedencia queda así:

1. canon propio de Bitácora (`11`, `12`, `13`, `16`, `21`, `23_uxui`);
2. prototipos Stitch por estado cuando existan;
3. `PROTOTYPE-*.md` como dueño del alcance, inventario de frames y estados obligatorios;
4. HTML local consolidado solo como wrapper navegable o fallback documental, nunca como reemplazo silencioso de Stitch;
5. `awesome-design-md` solo como inspiración secundaria de orden documental y disciplina visual.

## Postura emocional y guardrails de producto

La UI debe leerse como:

- diario personal sobrio;
- interfaz clínica serena;
- producto que cuida sin vigilar;
- tecnología contenida, no exhibida.
- decisión editorial medida, sin ruido ornamental ni teatralidad visual.

La UI no debe verse como:

- dashboard SaaS genérico;
- consola analítica;
- app de hábitos celebratoria;
- experiencia hospitalaria fría;
- sistema de scoring conductual.

## Superficies base por actor

### Paciente web

- una sola dirección dominante;
- eje vertical claro;
- foco en tarea o lectura personal;
- bloques respirados;
- una acción dominante;
- acciones sensibles explícitas y contenidas;
- señales de confianza contextuales, sin persistir como paneles de control;
- continuidad visible pero discreta, sin convertir la pantalla en dashboard.

### Profesional web

- más densidad comparativa que la UI paciente;
- máximo dos zonas funcionales visibles en desktop;
- la lista o comparación nunca debe degradarse a dashboard duro;
- el contenido clínico sigue siendo legible antes que operativo;
- superficie profesional con la misma familia que el paciente, con un giro clínico y de soporte;
- continuidad visible pero discreta para lectura longitudinal y contexto terapéutico.

### Puente Telegram

- flujo corto, explicativo y sin jerga técnica;
- un paso dominante por pantalla;
- códigos, expiración y errores como estados guiados;
- continuidad clara hacia el bot.

## Sistema de tokens frontend

La implementación futura debe heredar esta familia semántica mínima:

Los valores concretos de paleta, tipografías, textura y materialidad viven en `../11_identidad_visual.md`.
Este documento no redefine esos valores: los traduce a roles de sistema y reglas de implementación.

| Familia | Base canónica | Regla |
| --- | --- | --- |
| color de marca | `--brand-primary`, `--brand-secondary`, `--brand-accent` | color atmosférico con acentos medidos, nunca como ruido dominante |
| superficies | `--surface`, `--surface-muted` | fondos cálidos con textura suave editorial, mate y legible |
| tipografía | `Newsreader`, `Source Sans 3`, `IBM Plex Mono` | `Newsreader` editorial medida para ritmo de lectura; sans para operaciones |
| radios | `radius-sm`, `radius-md`, `radius-lg` | contenedores con suavidad contenida, sin carácter decorativo |
| borde | `border-base`, `border-subtle` | separar sin rigidez |
| elevación | `shadow-none`, `shadow-soft` | profundidad mínima |
| foco | `focus-ring` | visible, accesible, sin brillo agresivo |
| motion | transiciones suaves y cortas | movimiento suave y funcional; sin rebotes ni teatralidad |

### Reglas de tokenización

- las decisiones semánticas deben mapearse primero a rol, no a valor crudo;
- el color no puede ser el único portador de estado;
- cualquier tema o variante futura debe preservar contraste AA como mínimo;
- no se introducen tokens “techno” o “dashboard” fuera del canon.
- iconografía funcional con calidez mínima, sin metáforas emocionales pesadas;
- textura suave editorial debe permanecer por defecto en superficies base, sin saturación de gradientes;
- continuidad y vínculos deben materializarse con señales de bajo contraste emocional.

## Gramática de layout

### Shell paciente

- una columna dominante;
- ancho de lectura moderado;
- encabezado breve;
- secuencia vertical continua;
- rail o sidebar solo cuando no compita con la tarea principal.

### Shell profesional

- zona principal de trabajo + apoyo secundario opcional;
- la información comparativa debe respirar;
- tablas o resúmenes densos requieren alivio visual y estados bien separados.

### Shell de acción sensible

- contenedor explícito;
- consecuencia visible;
- CTA primaria inequívoca;
- destructivas separadas;
- reversible cuando aplique.

### Shell de lectura longitudinal

- gráfico o contenido principal primero;
- filtros y exportación en segundo plano;
- vacío y error claramente distinguibles.

### Shell de puente externo

- instrucción central;
- evidencia del siguiente paso;
- códigos y vencimientos con visibilidad alta;
- soporte de reintento sin dramatismo.

## Gramática de componentes

La futura implementación debe organizarse alrededor de estas primitivas de sistema:

| Familia | Propósito | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell editorial paciente | loading, ready, empty, error |
| `ProfessionalPageShell` | shell profesional sobria | loading, ready, empty, error |
| `SectionHeader` | encabezados de tarea o lectura | default |
| `SensitiveContainer` | consentimiento, acceso, revocación, exportación | default, warning, error |
| `QuickValueField` | gesto instantáneo como mood scale | default, submitting, error, confirm |
| `GroupedCheckinForm` | formularios breves por bloques | partial, conditional, ready, submitting, error, confirm |
| `SaveRail` | cierre de decisión o guardado | ready, disabled, submitting, error |
| `InlineFeedback` | confirmación factual o error localizado | confirm, error, info |
| `AccessControlBlock` | visibilidad, vínculo, revocación | default, changed, conflict, revoked |
| `TimelinePanel` | lectura longitudinal | loading, ready, empty, error |
| `BindingCodePanel` | auto-vinculación paciente-profesional por código | idle, code, invalid, expired, success, error |
| `TelegramCodeBridgePanel` | pairing Telegram y códigos temporales de canal | idle, code, expired, linked, error |
| `TrustContextPanel` | aviso puntual de confianza/consentimiento en tareas críticas | default, contextual_trust |

Estos nombres son de sistema y sirven como gramática común. El nombre de implementación puede refinarse después, pero no debería redefinir la familia visual.

`BindingCodePanel` y `TelegramCodeBridgePanel` no deben colapsarse en una sola primitive, porque responden a contratos, errores y consecuencias de acceso diferentes.

## Estados de interacción y estados críticos

Toda `UI-RFC` futura debe trabajar con esta taxonomía común:

| Estado | Significado | Regla de UX |
| --- | --- | --- |
| `loading` | la data o pantalla se está resolviendo | skeleton o placeholder sereno |
| `ready` | la tarea puede ejecutarse | acción visible y clara |
| `submitting` | la acción ya fue tomada | no permitir doble acción confusa |
| `confirm` | la operación terminó bien | confirmación factual, sin elogio |
| `empty` | no hay datos para mostrar | orientar sin dramatizar |
| `error` | fallo recuperable o técnico | explicar, ubicar y ofrecer siguiente paso |
| `locked` | bloqueo estricto de acceso o consentimiento | visibilizar causa y vía de resolución |
| `expired` | código o sesión temporal vencidos | reset guiado, no alarmista |
| `revoked` | acceso o vínculo ya no están activos | lenguaje claro y no punitivo |
| `conflict` | el estado actual impide repetir la acción | explicar contexto, evitar duplicación |
| `session_expired` | sesión web caducada | reingreso digno y continuidad explícita |
| `contextual_trust` | consentimiento activo o señal de acceso puntual | aparece junto a la acción, no en bloque permanente |
| `continuity_soft` | continuidad entre pasos de la jornada | marca discreta de retorno y progreso no intrusivo |

## Motion y microinteracciones

- transiciones cortas y suaves;
- fade y desplazamiento breve como recursos base;
- no usar rebotes, pulsos, confeti ni énfasis lúdico;
- acciones sensibles deben sentirse estables, no “heroicas”;
- siempre respetar `prefers-reduced-motion`.

## Accesibilidad

La futura capa frontend debe asumir como baseline:

- contraste AA mínimo;
- foco visible en todos los elementos interactivos;
- etiquetas por encima del campo;
- estados no dependientes solo de color;
- navegación por teclado completa;
- orden de foco coherente;
- mensajes de error cercanos al punto afectado;
- gráficos con resumen textual complementario;
- timers y vencimientos comprensibles sin depender solo de animación;
- toque cómodo en mobile.

## Integración con Stitch

### Regla general

Stitch es la fuente visual primaria de esta etapa (`strict Stitch only`).
El `DESIGN.md` que Stitch consume vive en `.docs/stitch/` como artefacto derivado y regenerable desde el wiki; no introduce una autoridad visual paralela.

### Reglas obligatorias

- cada `UI-RFC-*` debe citar rutas exactas de Stitch por estado obligatorio;
- el inventario de estados viene de `PROTOTYPE-*`, no de la memoria del implementador;
- si el slice está bajo `strict Stitch only`, el HTML local no alcanza para abrir `UI-RFC`;
- el HTML local puede sobrevivir como wrapper de revisión, nunca como autoridad visual paralela;
- cualquier conflicto entre Stitch y canon bloquea el slice hasta resolución explícita.

### Traducción de Stitch con margen mínimo

- traducir decisiones globales a la gramática de tokens/layout/componentes sin inventar nuevas superficies o estados fuera de `PROTOTYPE-*`;
- sincronizar primero el design system del proyecto con el `DESIGN.md` derivado antes de evaluar cobertura por slice;
- sostener explícitamente:
  - tipografía editorial medida;
  - color atmosférico con acentos medidos;
  - movimiento suave y funcional;
  - contenedores con suavidad contenida;
  - fondos con textura suave editorial;
  - iconografía funcional con calidez mínima;
  - superficie profesional con la misma familia, con giro clínico;
  - confianza solo contextual, continuidad visible pero discreta;
  - una acción dominante por contexto.
- si el artefacto Stitch muestra más elementos que los contratos permiten, priorizar la restricción funcional y documentar la diferencia como deuda de diseño, nunca como implementación.

## Contratos de API para frontend

### Convenciones generales

- autenticación vía `Authorization: Bearer <access_token>`;
- resolución de identidad por JWT de Supabase;
- API versionada en `/api/v1`;
- `patient_ref` opaco para vistas profesionales;
- todos los errores comparten envelope con `trace_id`.

### Expectativas por dominio

| Dominio | Expectativa frontend |
| --- | --- |
| onboarding/auth | bootstrap desde JWT y continuidad de contexto |
| consentimiento | bloqueo estricto explícito antes de registrar o vincular |
| mood entries | gesto rápido, feedback inmediato y confirmación factual |
| daily checkins | grupos cortos, condicionales visibles, guardado final claro |
| care links | explicar vínculo, acceso y reversibilidad |
| professional dashboard | lectura comparativa sin tono de monitoreo |
| telegram pairing | estados de código, expiración y vínculo ya activo |
| export | acción sensible, explícita y reversible solo donde aplique |

## Manejo de errores y estrategia de fallback

La UI debe mapear explícitamente los códigos de contrato más relevantes:

| Código | Tratamiento UI esperado |
| --- | --- |
| `CONSENT_REQUIRED` | estado `locked` con camino claro a consentimiento |
| `CONSENT_VERSION_MISMATCH` | conflicto bloqueante con actualización de flujo |
| `CARELINK_EXISTS` | conflicto contextual, no error genérico |
| `BINDING_CODE_NOT_FOUND` | error localizado o estado inválido |
| `BINDING_CODE_EXPIRED` | estado `expired` con regeneración |
| `BINDING_CODE_ALREADY_USED` | conflicto no destructivo |
| `SESSION_NOT_LINKED` | estado guiado de puente, no toast ambiguo |
| `AUDIT_WRITE_FAILED` | error técnico bloqueante con `trace_id` visible |
| `ENCRYPTION_FAILURE` | error técnico bloqueante con `trace_id` visible |
| `NO_CONSENT_CONFIG` | indisponibilidad operativa clara, no redacción clínica |

### Regla de fallback

- priorizar reintento o siguiente paso claro;
- nunca ocultar `trace_id` cuando el contrato ya lo devuelve;
- no exponer detalles internos ni vocabulario de backend al usuario;
- no degradar un error de seguridad a mensaje vago.

## Requerimientos de seguridad, privacidad y corrección aplicados al frontend

- no asumir lectura de payload clínico sensible fuera del contrato;
- no revelar PII o accesos innecesarios en UI profesional;
- siempre nombrar explícitamente quién puede ver qué;
- revocación y cambio de acceso deben sentirse inmediatos y concretos;
- no inventar heurísticas clínicas del lado cliente;
- las vistas profesionales deben conservar opacidad sobre `patient_ref`.

## Responsive

- mobile-first para paciente y puente Telegram;
- desktop-first solo donde el caso profesional realmente lo exija;
- máximo dos zonas funcionales en desktop para trabajo profesional;
- evitar layouts SaaS de tres columnas por defecto;
- las prioridades de contenido no deben cambiar entre mobile y desktop.

## Checklist previa a UI-RFC

Antes de abrir un `UI-RFC-*` de slice, debe cumplirse:

1. `VOICE-*`, `UXS-*` y `PROTOTYPE-*` estables para el slice;
2. estado del slice visible en `21_matriz_validacion_ux.md` y `23_uxui/INDEX.md`;
3. cobertura Stitch suficiente según el gate vigente;
4. referencias exactas por estado obligatorio;
5. mapeo de errores y estados críticos cerrado;
6. no contradicción con `11`, `12`, `13` o `16`.

## Riesgos, deudas y supuestos abiertos

### Riesgos activos

- la dispensa puede generar retrabajo cuando exista validación real;
- el gate `strict Stitch only` puede bloquear la apertura de slices núcleo;
- la UI profesional corre riesgo de deslizarse a dashboard si no se disciplina desde `UI-RFC`.

### Bloqueos actuales de la primera ola

- `ONB-001`: bloqueado por cobertura Stitch incompleta de estados obligatorios;
- `REG-001`: bloqueado porque no existen artefactos Stitch del slice;
- `REG-002`: bloqueado porque no existen artefactos Stitch del slice.

### Supuestos

- la próxima sesión podrá comenzar código solo cuando exista al menos un `UI-RFC-*` de slice no bloqueado;
- si cambian contratos de error, auth o consentimiento, este documento debe resincronizarse antes de implementación.

---

**Estado:** baseline técnico frontend inicial, válido para entrada a UI docs-first bajo dispensa.
**Precedencia:** depende de `11`, `12`, `13`, `16`, `04`, `07`, `09`, `21`, `23_uxui` y decisiones de dispensa.
**Siguiente capa gobernada:** `23_uxui/UI-RFC/UI-RFC-INDEX.md` y futuros `UI-RFC-*`.
