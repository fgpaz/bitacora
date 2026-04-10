<!--
target: codex
pressure: aggressive
generated: 2026-04-08
-->

# Bitácora — Nueva sesión para entrada a UI bajo waiver explícito

## Mission

Start a new Codex session to open the UI layer of Bitácora from the current repo state.

This session is `docs/UI first`, not product implementation.

Its job is to:

1. absorb the explicit waiver that allows UI work before real `UX-VALIDATION`;
2. harden the global UI entry layer;
3. descend into slice-level UI specs in the agreed order;
4. use `Stitch` as the primary visual authority;
5. use several subagents on every non-trivial task;
6. leave the repo ready so the following session can begin code with minimal ambiguity.

## Verify the repo before trusting this prompt

Treat this prompt as a restart aid, not as authority.
Verify the workspace first and trust the repo over this text if they disagree.

Verified repo state on `2026-04-08`:

- the workspace is currently `docs-first`; there is no committed app tree such as `frontend/`, `src/`, `app/`, `pages/`, or `components/` at repo root;
- the active visual surface lives in `.docs/wiki/23_uxui/PROTOTYPE/` and `artifacts/stitch/`;
- all `13` visible MVP slices already exist in `PROTOTYPE`;
- `.docs/wiki/23_uxui/UI-RFC/` does not exist yet;
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` does not exist yet;
- `.docs/wiki/23_uxui/HANDOFF-*` families do not exist yet;
- Stitch tooling is active in `.docs/stitch/` and already has per-slice generation scripts;
- real UX validation is still pending and remains deferred;
- the explicit UI-entry waiver already exists in:
  - `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
  - `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`

## Mandatory skill stack

Use:

- `$ps-contexto` — mandatory first action
- `$brainstorming` — mandatory before planning or hard documentation decisions
- `$ps-asistente-wiki` — mandatory to confirm the exact documentary phase and next UI doc step
- `$writing-plans` — mandatory because this is large, risky, and multi-step
- `$ps-trazabilidad` — mandatory per batch and at final closure
- `$ps-auditar-trazabilidad` — mandatory final audit because the session is multi-family and high-drift risk

Use `$mi-lsp` only if real code under `src/` or equivalent is discovered.
If no implementation code exists, say that explicitly and do not invent semantic code navigation where none applies.

## Mandatory exploration block before planning or writing

Before planning or creating any document:

1. Run `$ps-contexto`.
2. Read `AGENTS.md` and `CLAUDE.md`.
3. Dispatch **minimum 5 explorer subagents in parallel in a single message**.
4. If `ps-explorer` hits model limits, immediately redispatch with equivalent explorer-capable agents; do not drop below 5 parallel probes.
5. Cross-check the probes before proceeding.

Minimum probe objectives:

1. confirm the real state of `.docs/wiki/23_uxui/` and whether `UI-RFC` / `HANDOFF-*` are still absent;
2. inventory Stitch assets in `artifacts/stitch/` and note any slice/state limitations already documented;
3. inspect representative `PROTOTYPE-*` docs from patient, professional, and Telegram slices to extract repeated visual/interaction patterns;
4. verify whether any real frontend/application code exists or whether the session must remain fully `docs-first`;
5. inspect technical anchors that will constrain UI specs: `04_RF`, `07/08/09`, and current visual/system docs.

If two probes contradict each other, launch a sixth tiebreaker probe before trusting the result.

Also inspect the external inspiration source:

- `https://github.com/VoltAgent/awesome-design-md`

Use it as a primary-source inspiration repo, not as authority over Bitácora.

## Required workflow

Follow this sequence strictly:

1. `$ps-contexto`
2. mandatory exploration block
3. `$ps-asistente-wiki`
4. `$brainstorming`
5. `$writing-plans`
6. execute in waves, using several subagents for every non-trivial task
7. `$ps-trazabilidad` per batch
8. `$ps-trazabilidad` final
9. `$ps-auditar-trazabilidad`

## Read first

- `AGENTS.md`
- `CLAUDE.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/11_identidad_visual.md`
- `.docs/wiki/12_lineamientos_interfaz_visual.md`
- `.docs/wiki/13_voz_tono.md`
- `.docs/wiki/14_metodo_prototipado_validacion_ux.md`
- `.docs/wiki/15_handoff_operacional_uxui.md`
- `.docs/wiki/16_patrones_ui.md`
- `.docs/wiki/21_matriz_validacion_ux.md`
- `.docs/wiki/22_aprendizaje_ux_ui_spec_driven.md`
- `.docs/wiki/23_uxui/INDEX.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`
- `.docs/stitch/README.md`
- `.docs/stitch/package.json`

Then read a representative set of slice prototypes:

- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-001.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-001.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-001.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-001.md`

## Locked decisions — do not reopen

- Real UX validation will happen later, once functional code exists.
- Do not create `UX-VALIDATION-*` now.
- `Stitch` is the primary visual authority for this stage.
- Local prototype HTML must remain based on what Stitch generated, not reinterpret it arbitrarily.
- `awesome-design-md` is a secondary inspiration source for system/document clarity, not a replacement for Bitácora canon.
- In the first global UI documents, ask all the rigorous questions needed.
- Once slice-level specs begin, ask only questions that are strictly necessary to resolve a real contract/design ambiguity.
- This session is `docs + UI contracts only`; do not implement product code.
- Every non-trivial task in the session must use several subagents.
- The slice order is frozen as:
  1. `ONB-001`
  2. `REG-001`
  3. `REG-002`
  4. `VIN-002`
  5. `VIN-004`
  6. `VIN-003`
  7. `CON-002`
  8. `VIS-001`
  9. `EXP-001`
  10. `VIN-001`
  11. `VIS-002`
  12. `TG-001`
  13. `TG-002`

## Boundaries

- Docs only.
- No product implementation.
- No `HANDOFF-*`.
- No fake validation.
- No new authority outside the canon + Stitch + explicit waiver.
- Do not create a repo-root `DESIGN.md` as a competing source of truth.
- If a support artifact inspired by `awesome-design-md` is useful, keep it clearly secondary and non-canonical.
- Do not touch `07/08/09` unless a real UI-level contradiction forces it.

## Working rule for questions

### Global UI entry phase

Use the full `$brainstorming` rigor.
Close the important decisions about:

- UI communication priority
- pattern hierarchy
- system visual direction
- token and component grammar
- responsive philosophy
- state treatment
- accessibility posture
- how Stitch references are translated into implementable contracts

### Slice spec phase

Once the global layer is stable, only ask a question if the answer changes one of these materially:

- component boundaries
- major visual hierarchy
- state model
- backend contract expectations
- accessibility behavior
- cross-slice reuse

If the question is cosmetic and does not change a contract, decide and move on.

## Session objective by waves

### Wave 0 — absorb the waiver into the UI entry

Goal:

- confirm with `$ps-asistente-wiki` that the project is intentionally entering UI under waiver;
- make sure the next UI docs do not create double authority with the existing UX-validation gate;
- expose any contradiction instead of smoothing it over.

### Wave 1 — close the global UI entry

Practical target:

- harden `.docs/wiki/16_patrones_ui.md`;
- create `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`;
- create `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md`;
- define how Stitch feeds the UI layer;
- define the translation from patterns to tokens, layout, components, states, motion, and responsive behavior;
- define how slice-level `UI-RFC-*` should cite prototype, voice, UXS, and technical anchors.

Do not let this wave turn into decorative theory.
The output must be directly useful for the following slice specs and for code in the next session.

### Wave 2 — descend into slice-level UI specs

If Wave 1 is stable, begin the first slice specs.

Priority target for this session:

1. `ONB-001`
2. `REG-001`
3. `REG-002`

If clarity and time still hold:

4. `VIN-002`
5. `VIN-004`

Do not force breadth over quality.
It is better to leave the repo with a strong UI entry plus a few solid core `UI-RFC-*` docs than many shallow ones.

## Severity rules

- If the repo contradicts this prompt, trust the repo and name the contradiction.
- If a skill points to the old canon, treat that as drift and adapt to the real repo structure.
- If a Stitch reference conflicts with canon docs, stop and resolve the conflict explicitly.
- If a slice-level UI contract depends on backend details that are still ambiguous, mark the dependency instead of hiding it in UI wording.
- If external inspiration pushes toward a style that conflicts with accepted Bitácora Stitch outputs, reject the external style.
- Treat double authority as a real defect.
- Treat “we can define that later in code” as a real defect during this session.

## End condition

Do not end the session until:

- the global UI entry is explicitly closed or blocked with named reasons;
- the repo contains the new or hardened global UI docs it truly earned;
- at least the first core slice specs are opened if Wave 1 closed cleanly;
- `$ps-trazabilidad` has run;
- `$ps-auditar-trazabilidad` has run;
- the session leaves an explicit statement of what is ready for the following code session and what remains pending.
