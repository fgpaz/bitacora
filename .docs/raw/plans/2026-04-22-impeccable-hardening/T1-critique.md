# Task T1: impeccable-critique (read-only UX critique)

## Shared Context
**Goal:** Evaluar la efectividad de diseño del frontend actual antes de modificar código.
**Stack:** Next.js 16 + React 19 + CSS Modules.
**Architecture:** UI-only audit sobre `frontend/` con foco en jerarquía visual, story, information architecture y alineación emocional con canon 10/11.

## Locked Decisions
- Output único: reporte markdown en `.docs/raw/reports/2026-04-22-impeccable-critique.md`.
- Zero code changes. Si el executor modifica archivos fuera del reporte, es defecto.
- No cubrir a11y/performance/tokens: eso está en waves posteriores (T2-T10). Foco: jerarquía, story, postura emocional, densidad, consistencia editorial.
- Usa el baseline `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md` como input, NO lo dupliques.

## Task Metadata
```yaml
id: T1
depends_on: [T0]
agent_type: ps-code-reviewer
files:
  - read: frontend/app/**/*.tsx
  - read: frontend/components/**/*.tsx
  - read: .docs/raw/reports/2026-04-22-impeccable-audit-baseline.md
  - read: .docs/wiki/10_manifiesto_marca_experiencia.md
  - read: .docs/wiki/11_identidad_visual.md
  - read: .docs/wiki/12_lineamientos_interfaz_visual.md
  - read: .docs/wiki/16_patrones_ui.md
  - create: .docs/raw/reports/2026-04-22-impeccable-critique.md
complexity: low
done_when: "test -f .docs/raw/reports/2026-04-22-impeccable-critique.md && git diff --stat frontend/ | wc -l == 0"
```

## Reference
- Baseline consolidado: `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md` §7 (visual quality, jerarquía) y §8 (alineado).
- Canon emocional: `.docs/wiki/10_manifiesto_marca_experiencia.md` §4-§8 (decisiones madre, personalidad, postura de confianza, anti-señales).
- Canon interfaz: `.docs/wiki/12_lineamientos_interfaz_visual.md` §"Jerarquía visual", §"Densidad y ritmo", §"Jerarquía de CTA".
- Skill base: invocar `Skill("impeccable-critique")` con el frontend completo como contexto.

## Prompt
Sos un agente read-only. Tu único producto es un reporte markdown. NO modificas código. NO tocás tests. NO tocás canon wiki.

Contexto de negocio: Bitácora es un clinical mood tracker para personas potencialmente en vulnerabilidad emocional (usuarios psicológicos/psiquiátricos). El canon 10 establece una postura "refugio clínico sereno" con anti-señales explícitas: "evitar dashboards del paciente que parezcan tableros de vigilancia", "sin celebración", "sin scoring language", "sin tono motivacional".

Tu tarea: evaluar la efectividad de diseño del frontend actual desde la perspectiva UX/story, validando o refutando estos 5 puntos del baseline:

1. **Dashboard densidad tipo tablero** (baseline §7.1). El archivo `frontend/components/patient/dashboard/Dashboard.tsx` combina `DashboardSummary` (3 tarjetas stat) + trendChart + entryList + grupo de acciones. ¿Esto viola canon 10 ("sin tableros de vigilancia")? ¿Cómo se compara con el patrón "Shell editorial de una columna" (canon 16 §1)? ¿Qué recomendás en mobile ≤360px?

2. **TelegramPairingCard — CTAs simultáneos** (baseline §7.2). En estado `pairing active` hay 3 CTAs visibles: "Copiar mensaje" + "Abrir Telegram" + "Ya envié el mensaje" (`TelegramPairingCard.tsx:279-301`). Canon 12 §"Jerarquía de CTA" exige "una primaria + secundaria silenciosa + terciaria discreta". ¿Qué jerarquía propondrías?

3. **ConsentGatePanel — presencia profesional antes del control** (baseline §1.5). El texto `"Recordá que viniste a través de una invitación de tu profesional"` (ConsentGatePanel.tsx:49) aparece antes de que el paciente dé consentimiento. Canon 10 §"Ejemplos de uso" marca como anti-señal instalar vigilancia antes de control. ¿Se justifica mantener el hint por onboarding contextual, o es redundante?

4. **PatientPageShell sin navegación formal** (baseline §7.3). Solo un botón "Cerrar sesión" suelto, sin header ni indicador de ubicación. Canon 12 §"Navegación" pide "estable, simple, predecible". ¿Bloqueante en MVP o aceptable?

5. **Hero landing (`OnboardingEntryHero`) — jerarquía de confianza**. Evaluá si el orden headline → CTA → privacidad → soporte respeta canon 16 §10 "Hero contextual adaptativo". ¿La frase "Un lugar tranquilo para llevar tu registro..." (con "tranquilidad" duplicada) sabotea la sensación de serenidad? ¿Qué recomendás sin tocar el copy congelado "Tu espacio personal de registro" + "Ingresar"?

Reglas de salida:
- Formato markdown, 500-800 líneas.
- Un veredict por cada uno de los 5 puntos: `mantener`, `refinar`, `rediseñar`, o `fuera de scope`.
- Por cada `refinar` / `rediseñar`, specificar:
  - qué cambiarías (sin redactar el código — eso es W3/W8/W9).
  - qué skill posterior lo debería ejecutar (harden / extract / onboard / polish / distill / quieter).
  - file:line que se tocaría.
- Zero cambios de código. Zero cambios en wiki. Solo reporte en `.docs/raw/reports/2026-04-22-impeccable-critique.md`.
- Cerrá con un `## Resumen por skill de waves posteriores` mapeando tus recomendaciones a T3-T10 del plan.

## Execution Procedure
1. Leé el baseline completo: `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md`.
2. Leé canon `10_manifiesto_marca_experiencia.md`, `12_lineamientos_interfaz_visual.md`, `16_patrones_ui.md`.
3. Abrí Dashboard.tsx, TelegramPairingCard.tsx, ConsentGatePanel.tsx, PatientPageShell.tsx, OnboardingEntryHero.tsx — solo lectura con Read tool.
4. Redactá el reporte evaluando los 5 puntos con veredict + recomendaciones.
5. Persistí el reporte con Write en `.docs/raw/reports/2026-04-22-impeccable-critique.md`.
6. Verificá `git status --short frontend/` devuelve vacío (cero cambios en código).
7. Si algún punto del baseline parece contradicho por el código real, documentálo como "contradicción detectada" en el reporte — no modifiques el baseline.

## Skeleton
```markdown
# Impeccable Critique — Frontend Bitácora
**Fecha:** 2026-04-22 · **Wave:** 1 · **Modo:** read-only

## Veredict por punto
### 1. Dashboard densidad tipo tablero
**Veredict:** refinar
**Qué cambiar:** ...
**Skill responsable:** impeccable-distill (T + wave)
**file:line:** frontend/components/patient/dashboard/Dashboard.tsx:231-266

...

## Resumen por skill de waves posteriores
| Skill | Hallazgos asignados |
|---|---|
| impeccable-harden | ... |
| impeccable-distill | ... |
...
```

## Verify
`test -f .docs/raw/reports/2026-04-22-impeccable-critique.md && git diff --stat frontend/ | wc -l` → `0`

## Commit
`docs(impeccable-critique): UX critique de jerarquía y densidad en frontend paciente`
