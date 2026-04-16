# Stitch Mode A para Bitacora

Este directorio deja el setup operativo de Stitch para Bitácora con un `DESIGN.md` derivado del wiki y regenerable.

`Mode A` significa conexión directa por `StreamableHTTP` con OAuth. No usa proxy MCP ni el comando `tool` del CLI roto.

## Qué usa este setup

- credenciales OAuth de `~/.stitch-mcp`
- el proyecto GCP configurado en `~/.stitch-mcp/config/configurations/config_default`
- un pack `DESIGN.md` regenerable desde:
  - `../wiki/11_identidad_visual.md`
  - `../wiki/12_lineamientos_interfaz_visual.md`
  - `../wiki/13_voz_tono.md`
  - `../wiki/16_patrones_ui.md`
  - `../wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- prompts por slice alineados con `VOICE`, `UXS` y `PROTOTYPE`

## Pack de diseño derivado

El pack versionado que Stitch consume vive en este mismo directorio:

- `DESIGN.md`
- `DESIGN.patient.md`
- `DESIGN.professional.md`
- `DESIGN.telegram.md`
- `DESIGN.manifest.json`

La autoridad sigue estando en el wiki. Estos archivos son derivados y pueden regenerarse con:

```powershell
npm run build-design-pack
```

## Primer uso

1. Instalar dependencias:

```powershell
cd C:\repos\mios\humor\.docs\stitch
npm install
```

2. Verificar auth local:

```powershell
npm run check-auth
```

3. Listar proyectos disponibles:

```powershell
npm run list-projects
```

4. Regenerar el pack derivado:

```powershell
npm run build-design-pack
```

5. Generar el preset principal del slice:

```powershell
npm run generate:onb001
```

## Comandos útiles

Generar preset core:

```powershell
node stitch-mode-a.mjs generate-onb-001 --preset core
```

Generar preset completo:

```powershell
node stitch-mode-a.mjs generate-onb-001 --preset full
```

Usar un proyecto existente:

```powershell
node stitch-mode-a.mjs generate-onb-001 --project 123456789
```

Forzar otro modelo o dispositivo:

```powershell
node stitch-mode-a.mjs generate-onb-001 --preset core --model GEMINI_3_FLASH --device MOBILE
```

Sincronizar design system del slice:

```powershell
npm run sync-design:onb001
```

Auditar artifacts existentes:

```powershell
npm run audit-artifacts
```

## Salidas

Las salidas se guardan en:

`artifacts/stitch/onb-001/<timestamp>/`

Cada corrida escribe:

- `run.json` con metadata de la corrida
- `project.json` si se pudo snapshotear el proyecto para `apply_design_system`
- `apply-design-system.json`, `apply-design-system.skipped.json` o `apply-design-system.error.json`
- un `*.result.json` por pantalla
- un `*.html` por pantalla cuando Stitch devuelve `htmlCode.downloadUrl`
- un `*.png` por pantalla cuando Stitch devuelve `screenshot.downloadUrl`
- imágenes extraídas cuando Stitch las devuelve inline
- `screens.json` con el resultado de `list_screens`

## Notas operativas

- `GEMINI_3_FLASH` queda como default por velocidad.
- el runner sincroniza el design system derivado antes de generar, salvo `--skip-design-sync`;
- después de generar, el runner intenta aplicar ese design system primero sobre las pantallas devueltas por la propia corrida y solo cae a `get_project` si necesita resolver instancias desde el proyecto;
- si Stitch no expone `selectedScreenInstances`, el runner registra `apply-design-system.skipped.json` y no trata ese caso como falsa falla de cobertura visual;
- si una generación hace timeout, conviene revisar primero si la corrida dejó `*.result.json`, `*.html` o `*.png`; `list-screens` puede no devolver datos útiles en todos los proyectos.
- Este setup acelera la capa visual del prototipo; no reemplaza el canon UX ni la futura `UX-VALIDATION`.

## Ola núcleo `Stitch-first`

Además del slice `ONB-001`, este setup ya deja preparada la ola núcleo:

- `core-wave.prompts.json`

Slices incluidos:

- `reg-001`
- `reg-002`

Listar slices disponibles:

```powershell
npm run list-slices:core
```

Generar un slice núcleo:

```powershell
npm run generate:reg001
```

Comandos disponibles:

- `npm run sync-design:reg001`
- `npm run sync-design:reg002`
- `npm run generate:reg001`
- `npm run generate:reg002`

## Ola paciente restante `Stitch-first`

Además del slice `ONB-001`, este setup ya deja preparada la próxima ola paciente en:

- `patient-wave-a.prompts.json`
- `remaining-wave-b.prompts.json`

Slices incluidos:

- `vin-002`
- `vin-004`
- `vin-003`
- `con-002`
- `vis-001`
- `exp-001`
- `vin-001`
- `vis-002`
- `tg-001`
- `tg-002`

Listar slices disponibles:

```powershell
npm run list-slices:patientwave
npm run list-slices:remainingwave
```

Generar un slice de la ola:

```powershell
npm run generate:vin002
```

Comandos disponibles:

- `npm run generate:vin002`
- `npm run generate:vin004`
- `npm run generate:vin003`
- `npm run generate:con002`
- `npm run generate:vis001`
- `npm run generate:exp001`
- `npm run generate:vin001`
- `npm run generate:vis002`
- `npm run generate:tg001`
- `npm run generate:tg002`

Regla operativa recomendada:

- regenerar el pack antes de una nueva tanda importante;
- generar `core` primero;
- usar un slice por vez para reducir timeouts;
- si un estado puntual falla, reintentar con `node stitch-mode-a.mjs generate-prototype --config <file> --slice <id> --preset <screen-id>`;
- promover el resultado a `PROTOTYPE-*` solo cuando el output sea legible, trazable, registrado con design pack y suficiente para validación posterior.
