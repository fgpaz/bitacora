# Task T2: impeccable-clarify (microcopy + ortografía + voseo)

## Shared Context
**Goal:** Cerrar bloqueantes de regla 9.1 (tildes) + canon 13 (voseo rioplatense) + aria-labels descriptivos, sin tocar copy congelado.
**Stack:** Next.js 16 + React 19 + CSS Modules.
**Architecture:** Edits puntuales en strings visibles y aria-labels en componentes paciente + profesional.

## Locked Decisions
- **NO modificar** los siguientes strings literales (copy congelado post dashboard-first):
  - `"Ingresar"`, `"Tu espacio personal de registro"`, `"Empezá con tu primer registro"`, `"Registrar humor"`, `"Nuevo registro"`, `"+ Nuevo registro"`, `"Check-in diario"`, `"Recibí recordatorios por Telegram"`, `"Conectar"`, `"Ahora no"`, `"Registro guardado."`, `"Check-in guardado."`.
- Voseo rioplatense obligatorio (canon 13): `vos / podés / tenés / ingresá / invitá / completá / descargá`.
- `persona` vs `paciente`: usar `paciente` en superficie profesional (contexto clínico-relacional), `persona` o segunda persona sin etiqueta en UX general.
- Reemplazos de copy vago en errores siguen ejemplos del canon 13 §"Reglas para errores".
- `aria-label` enriquecido debe preservar el texto visible + agregar contexto (fecha, duración, acción).

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-next-vercel
files:
  - modify: frontend/app/layout.tsx:15
  - modify: frontend/app/error.tsx:14-17
  - modify: frontend/components/patient/onboarding/OnboardingEntryHero.tsx:13
  - modify: frontend/components/ui/ProfessionalShell.tsx:44
  - modify: frontend/components/patient/consent/ConsentGatePanel.tsx:47-55
  - modify: frontend/components/patient/mood/MoodEntryForm.tsx:57
  - modify: frontend/components/patient/checkin/DailyCheckinForm.tsx:96
  - modify: frontend/components/patient/dashboard/Dashboard.tsx:287-290
  - modify: frontend/components/patient/dashboard/TelegramReminderBanner.tsx:66
  - modify: frontend/components/patient/vinculos/BindingCodeForm.tsx:90
  - modify: frontend/app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx:12
  - modify: frontend/components/professional/InviteForm.tsx:35-111
  - modify: frontend/components/professional/PatientList.tsx:61-145
  - modify: frontend/components/professional/PatientDetail.tsx:65
  - modify: frontend/components/professional/PatientSummaryCard.tsx:32
  - modify: frontend/components/professional/ExportGate.tsx:271-289
  - read: .docs/raw/reports/2026-04-22-impeccable-audit-baseline.md
  - read: .docs/raw/reports/2026-04-22-impeccable-critique.md
complexity: medium
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0 AND grep -rn 'Pagina\\|invitacion\\|electronico\\|Version\\|Ultimo\\|traves\\|tenes\\|todavia\\|seccion\\|vinculo' frontend/components frontend/app | grep -v '.module.css' | wc -l == 0"
```

## Reference
- Baseline §1.1-§1.5 lista exacta de reemplazos.
- Canon 13 §"Terminología" y §"Reglas para errores".
- Regla 9.1 del CLAUDE.md (ortografía mandatoria).

## Prompt
Sos un ejecutor write. Aplicás reemplazos puntuales listados en el baseline. No refactorizás. No inventás copy nuevo. No tocás el copy congelado (lista arriba).

Reemplazos obligatorios por archivo:

### Ortografía (regla 9.1)
- `frontend/app/layout.tsx:15` — metadata title `"Bitacora"` → `"Bitácora"`.
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:13` — wordmark `Bitacora` → `Bitácora` (NO confundir con el copy congelado "Tu espacio personal de registro" ni "Ingresar").
- `frontend/components/ui/ProfessionalShell.tsx:44` — `Bitacora Pro` → `Bitácora Pro`.
- `frontend/components/patient/consent/ConsentGatePanel.tsx:49` — `"Recorda que viniste a traves de una invitacion de tu profesional."` → `"Recordá que viniste a través de una invitación de tu profesional."`.
- `frontend/components/patient/consent/ConsentGatePanel.tsx:55` — `Version` → `Versión`.
- `frontend/app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx:12` — `"Ingresa el correo electronico de tu paciente. Le enviaremos un enlace para que se registre y quede vinculado a tu cuenta."` → `"Ingresá el correo electrónico de tu paciente. Le enviaremos un enlace para que se registre y quede vinculado a tu cuenta."`.
- `frontend/components/professional/InviteForm.tsx` — aplicar todos los reemplazos listados en baseline §1.1 fila InviteForm (tildes en `Invitación`, `envió`, `aparecerá`, `vínculo`, `está`, `electrónico`). NO agregar comas Oxford ni cambiar estructura de frases.
- `frontend/components/professional/PatientList.tsx:61,63,73,80,144,145` — reemplazos baseline §1.1 fila PatientList (`Paginación`, `Página`, `Página anterior/siguiente`, `No tenés pacientes vinculados todavía`, `Invitá a alguien desde la sección de invitaciones`).
- `frontend/components/professional/PatientDetail.tsx:65` y `PatientSummaryCard.tsx:32` — `Ultimo registro` → `Último registro`.
- `frontend/components/professional/ExportGate.tsx:271` — `exportacion` → `exportación`.
- `frontend/components/professional/ExportGate.tsx:287` — `No tenes permisos` → `No tenés permisos`.

### Tuteo → voseo (canon 13)
- `frontend/app/(profesional)/(profesionalAuth)/profesional/invitaciones/page.tsx:12` — `Ingresa` → `Ingresá` (ya contemplado arriba).
- `frontend/components/professional/PatientList.tsx:145` — `Invita` → `Invitá`.

### Errores genéricos → errores concretos (canon 13 §"Errores")
- `frontend/components/patient/mood/MoodEntryForm.tsx:57` — `"Ocurrió un error. Intentá de nuevo."` → `"No pudimos guardar el registro. Probá de nuevo."`.
- `frontend/components/patient/checkin/DailyCheckinForm.tsx:96` — `"Ocurrió un error. Intentá de nuevo."` → `"No pudimos guardar el check-in. Probá de nuevo."`.
- `frontend/app/error.tsx:14-17` — reemplazar bloque `{h2}Algo salió mal{/h2} {p}Ocurrió un error inesperado. Intentá de nuevo.{/p}` por `{h2}No pudimos cargar esta pantalla{/h2} {p}Ocurrió algo inesperado. Probá recargar la página o volver en unos minutos.{/p}`. **Preservar** el botón `"Reintentar"` (es copy corto aceptable).

### Scoring language → lenguaje humano (canon 13 §"Términos preferidos")
- `frontend/components/patient/dashboard/Dashboard.tsx:287` — `aria-label="Puntaje de humor"` → `aria-label="Estado de ánimo"`.
- `frontend/components/patient/dashboard/Dashboard.tsx:288` — `"Sin puntaje"` → `"Sin registro"`.
- **NO cambiar** `Dashboard.tsx:231` (`"Variabilidad diaria"`) — canon acepta lenguaje clínico sobrio; mover esa decisión a T9/T8 si critique lo marca.

### Celebración decorativa (canon 13 §"Reglas para éxito")
- `frontend/components/patient/vinculos/BindingCodeForm.tsx:90` — `"Invitación aceptada ✓"` → `"Invitación aceptada"`. Eliminar el caracter `✓` sin agregar otro ícono.

### aria-labels descriptivos
- `frontend/components/patient/dashboard/TelegramReminderBanner.tsx:66` — al botón `"Ahora no"` **agregar** prop `aria-label="Descartar recordatorio por 30 días"`. El texto visible sigue igual (`"Ahora no"` es copy congelado).
- `frontend/components/patient/dashboard/Dashboard.tsx:287-290` — reemplazar `aria-label="Puntaje de humor"` por `aria-label={\`Estado de ánimo: ${formatMoodScore(entry.moodScore)}, ${dateStr}\`}` donde `dateStr` es el texto de fecha ya calculado en el mismo scope. Ver estructura actual:
  ```tsx
  <div className={styles.scoreBadge} aria-label="Puntaje de humor">
    {formatMoodScore(entry.moodScore)}
  </div>
  ```
  Resultado:
  ```tsx
  <div className={styles.scoreBadge} aria-label={`Estado de ánimo: ${formatMoodScore(entry.moodScore)}, ${dateStr}`}>
    {formatMoodScore(entry.moodScore)}
  </div>
  ```

### Rozaduras menores (canon 13)
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx:30-32` — `"Un lugar tranquilo para llevar tu registro de humor y bienestar, con la tranquilidad de que tus datos son privados."` → `"Un lugar sobrio para llevar tu registro de humor y bienestar, con la seguridad de que tus datos son privados."` (elimina la duplicación semántica de "tranquilidad"; mantiene longitud y estructura).
- `frontend/components/patient/dashboard/TelegramReminderBanner.tsx:60` — `"Tarda un minuto y te ayuda a no olvidar tu registro."` → `"Tarda un minuto. El recordatorio te llega a Telegram."` (neutraliza el push paternalista; mantiene longitud similar).

## Execution Procedure
1. Leé el baseline §1 completo.
2. Para cada archivo listado en `files.modify`, abrilo con Read, aplicá los reemplazos exactos con Edit. Usá `replace_all: false` para evitar cambios no deseados; repetí Edit cuando haya múltiples ocurrencias.
3. Tras cada archivo, `cd frontend && npm run typecheck` — si rompe, pausar.
4. Cuando todos los archivos estén editados, correr:
   ```bash
   cd frontend && npm run lint
   cd frontend && npm run test:e2e
   ```
5. Si algún spec rompe por cambio de texto visible (ej. assertion sobre "Invita" o "Pagina"), **pausar** y reportar al humano con `AskUserQuestion` — NO modificar specs unilateralmente.
6. Validar grep objetivo:
   ```bash
   grep -rn 'Pagina \|Pagina$\|invitacion\|electronico\|Version \|Ultimo registro\|a traves\|No tenes\|todavia$\|seccion de\|vinculo a' frontend/components frontend/app | grep -v '.module.css' | grep -v 'Pagina-web'
   ```
   Debe devolver vacío (ignorar resultados dentro de CSS Modules y wordmarks no-UI).
7. Si el grep arroja matches, investigar: o bien el patrón omitió uno del baseline (ampliar Edit), o bien es una falsa coincidencia (documentar en commit message).

## Skeleton
```tsx
// Patrón de Edit con contexto suficiente para unicidad:
// ConsentGatePanel.tsx:49 — antes:
//   <p className={styles.inviteHint}>
//     Recorda que viniste a traves de una invitacion de tu profesional.
//   </p>
// ConsentGatePanel.tsx:49 — después:
//   <p className={styles.inviteHint}>
//     Recordá que viniste a través de una invitación de tu profesional.
//   </p>
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
```
Exit 0 + grep objetivo vacío.

## Commit
`style(impeccable-clarify): tildes obligatorias, voseo profesional y aria-labels descriptivos`
