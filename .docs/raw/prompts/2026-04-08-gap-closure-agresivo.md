<!-- target: codex | pressure: max-pressure | generated: 2026-04-08 -->

# Cierre agresivo de gaps documentales — Proyecto Bitacora

## Mision

Cerrar TODOS los gaps de trazabilidad y consistencia en la documentacion SDD del proyecto Bitacora (mood tracker clinico). El proyecto tiene documentacion completa de 01 a 09 pero NO tiene matriz de pruebas (06) ni planes de test (TP-*). Ademas, los 56 RF fueron generados por agentes paralelos y pueden tener inconsistencias entre si, con los flujos, o con el modelo de datos.

**Esto es una sesion de auditoria y cierre. NO es una sesion de implementacion de codigo.**

## Primer paso obligatorio

```
$ps-contexto
```

Lee AGENTS.md primero. Luego carga:
- `.docs/wiki/01_alcance_funcional.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/03_FL.md`
- `.docs/wiki/04_RF.md`
- `.docs/wiki/05_modelo_datos.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/08_modelo_fisico_datos.md`
- `.docs/wiki/09_contratos_tecnicos.md`

## Exploracion obligatoria (minimo 5 subagentes en paralelo)

Antes de planificar, lanza MINIMO 5 subagentes ps-explorer en UN SOLO mensaje:

1. ps-explorer: "Lee TODOS los FL en .docs/wiki/03_FL/ y lista los RF candidatos que cada flujo declara. Compara con 04_RF.md."
2. ps-explorer: "Lee TODOS los RF en .docs/wiki/04_RF/ y verifica que cada RF tiene las 11 secciones obligatorias completas (Execution Sheet, Precondiciones, Inputs, Proceso, Outputs, Errores tipados, Casos especiales, Impacto modelo datos, Gherkin, Trazabilidad tests, Sin ambiguedades). Lista los que tienen secciones vacias o faltantes."
3. ps-explorer: "Lee 05_modelo_datos.md y compara cada entidad/campo con los que se mencionan en los RF de .docs/wiki/04_RF/. Reporta entidades mencionadas en RF que no estan en 05, y entidades en 05 que ningun RF referencia."
4. ps-explorer: "Lee 09_contratos_tecnicos.md y compara la tabla de endpoints con los endpoints declarados en los RF. Reporta endpoints en RF que faltan en 09, y endpoints en 09 sin RF asociado."
5. ps-explorer: "Lee los detail docs (07_tech/TECH-*.md, 08_db/DB-*.md, 09_contratos/CT-*.md) y verifica que los invariantes y decisiones criticas (cifrado, fail-closed, pseudonimizacion, consent default false, audit append-only) son consistentes con lo que dicen los RF."

## Workflow

Esta sesion sigue el flujo **Large/Risky**:

1. `$ps-contexto` — cargar contexto completo
2. `$brainstorming` — identificar gaps criticos vs cosmeticos
3. `$writing-plans` — generar plan de cierre con tareas por prioridad
4. Ejecucion por waves con `$ps-trazabilidad` por batch
5. `$ps-trazabilidad` — cierre final
6. `$ps-auditar-trazabilidad` — auditoria read-only final

## Tareas de cierre esperadas

### P0 — Criticos (deben cerrarse en esta sesion)

1. **Crear 06_matriz_pruebas_RF.md** — indice de trazabilidad RF → TP. Una fila por RF con TP-ID, escenario positivo, escenario negativo.
2. **Crear 06_pruebas/TP-*.md** — un archivo de test plan por modulo (TP-REG.md, TP-CON.md, TP-VIN.md, TP-VIS.md, TP-EXP.md, TP-TG.md, TP-SEC.md, TP-ONB.md). Cada TP referencia los RF del modulo con escenarios Gherkin expandidos.
3. **Verificar consistencia FL → RF** — cada FL debe tener todos sus RF candidatos reflejados en 04_RF/. Si hay RF faltantes, crearlos. Si hay RF huerfanos (sin FL), documentar por que.
4. **Verificar consistencia 05_modelo_datos ↔ RF** — cada campo mencionado en los RF debe existir en 05. Si hay drift, actualizar 05.
5. **Verificar consistencia 09_contratos ↔ RF** — cada endpoint en los RF debe estar en 09. Si falta, agregar.

### P1 — Importantes (cerrar si el tiempo permite)

6. **Revisar Gherkin en cada RF** — verificar que cada RF tiene al menos 1 escenario positivo y 1 negativo. Si falta, completar.
7. **Verificar errores tipados** — cada RF debe tener errores tipados con codigo, HTTP, trigger y respuesta concretos. Sin genericos.
8. **Verificar que CLAUDE.md y AGENTS.md** tienen paths correctos (03_FL/ no FL/, 04_RF/ no RF/).

### P2 — Cosmeticos

9. **Verificar links entre docs** — que las referencias cruzadas apunten a archivos que existen.
10. **Verificar ortografia en docs en espanol** — tildes y ene en textos user-facing.

## Decisiones cerradas (NO reabrir)

| Decision | Valor | Fuente |
|----------|-------|--------|
| PDP | Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > TTM | 02_arquitectura.md |
| Cifrado | encrypted_payload + safe_projection (patron BuhoSalud) | T3-5 revisada |
| Auth | Supabase Auth compartida | plan inicial |
| DB | bitacora_db dedicada en mismo PostgreSQL | T3-4 |
| Servicios | Monolito modular .NET 10 | T3-2 |
| Telegram | Webhook prod + long-polling dev | T3-3 |
| RLS | EF Core Global Query Filters (app-level) | T3-6 |
| Consent | Hard gate antes del primer registro | D4 |
| Acceso profesional | Solo vinculo persistente (sin codigo temporal) | D1 |
| Export | CSV en MVP, PDF en Roadmap | D2 |
| Nombre | Bitacora (definitivo) | D3 |
| Fail-closed | Todo gate de seguridad falla cerrado | T3-10 |
| Audit | Append-only, pseudonym_id en logs, actor_id solo en audit | T3-8, T3-9 |
| Retencion | Crisis 5 anos, audit 2 anos, supresion por anonimizacion | T3-12 |

## Boundaries

- **Docs only.** NO escribir codigo fuente.
- **NO crear archivos fuera de .docs/wiki/** (excepto el plan en .docs/raw/plans/).
- **NO modificar decisiones cerradas.**
- **NO agregar features nuevas.** Solo cerrar gaps en la documentacion existente.
- Si encontras una contradiccion grave entre docs, reportala con `request_user_input` antes de resolverla.

## Severidad

- **Drift entre RF y modelo de datos** = P0, corregir inmediatamente.
- **RF sin Gherkin** = P0, completar.
- **Endpoint en RF sin entrada en 09** = P0, agregar.
- **FL sin mapping completo a RF** = P0, crear RF faltantes.
- **06_matriz_pruebas inexistente** = P0, crear desde cero.
- **Doble autoridad** (dos docs dicen cosas distintas sobre el mismo tema) = STOP, reportar al usuario.
- **Fail-open en RF de seguridad** = STOP, corregir con maxima prioridad.

## Archivos clave para esta sesion

```
.docs/wiki/
├── 01_alcance_funcional.md
├── 02_arquitectura.md
├── 03_FL.md + 03_FL/ (15 flujos)
├── 04_RF.md + 04_RF/ (56 RF)
├── 05_modelo_datos.md (10 entidades)
├── 06_matriz_pruebas_RF.md          ← CREAR
├── 06_pruebas/TP-*.md               ← CREAR (8 archivos)
├── 07_baseline_tecnica.md
├── 07_tech/TECH-CIFRADO.md
├── 07_tech/TECH-TELEGRAM.md
├── 08_modelo_fisico_datos.md
├── 08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md
├── 09_contratos_tecnicos.md
├── 09_contratos/CT-AUTH.md
└── 09_contratos/CT-AUDIT.md
```
