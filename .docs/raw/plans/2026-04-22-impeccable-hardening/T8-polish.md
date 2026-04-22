# Task T8: impeccable-polish (consistencia fina + delay teatral)

## Shared Context
**Goal:** Cerrar pendientes menores: delay teatral en BindingCodeForm, rozaduras de copy ya aplicadas en T2, consistencia de spacing final.
**Stack:** React + CSS Modules.
**Architecture:** Edits puntuales de polish.

## Locked Decisions
- `BindingCodeForm.tsx:46-49` timer de 1500ms → reducir a 400ms. Canon 12 §"Motion no permitido" prohíbe delays teatrales.
- Verificar que los copy ajustes de T2 están consistentes (sin variantes silenciosas en strings duplicados).
- Sin nuevos componentes. Sin refactors grandes.

## Task Metadata
```yaml
id: T8
depends_on: [T7]
agent_type: ps-next-vercel
files:
  - modify: frontend/components/patient/vinculos/BindingCodeForm.tsx:46-49
  - read: frontend/components/patient/**/*.tsx
complexity: low
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0"
```

## Reference
Baseline §7.4 (delay teatral), §1.5 (rozaduras).

## Prompt
### 8.1 — Reducir delay teatral
Abrí `BindingCodeForm.tsx:46-49`. Localizá:
```tsx
setTimeout(() => {
  setState('idle');
  onSuccess();
}, 1500);
```
Cambiá `1500` a `400`.

### 8.2 — Consistencia de copy (grep pass)
Correr greps para verificar que no quedaron variantes silenciosas:
```bash
grep -rn 'Invitacion\|invitacion\|electronico\|Version \|Ultimo\|traves de\|No tenes\|todavia\|seccion de\|vinculo activo' frontend/components frontend/app | grep -v '.module.css'
```
Si devuelve matches, aplicar los mismos reemplazos que T2 (tildes).

### 8.3 — Consistencia de Casing / puntuación menor
- Verificar que todas las confirmaciones paciente terminan con punto: `"Registro guardado."`, `"Check-in guardado."` — sin agregar puntos a labels de botón.
- Verificar que los aria-labels no tienen puntuación final (convención: sí para oraciones, no para etiquetas cortas).

No tocar copy congelado.

## Execution Procedure
1. Edit BindingCodeForm paso 8.1.
2. Grep pass paso 8.2 — si match, fix.
3. Revisión casing paso 8.3.
4. Verify.

## Skeleton
N/A (edits triviales).

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
```

## Commit
`style(impeccable-polish): delay BindingCodeForm 400ms y consistencia fina de copy`
