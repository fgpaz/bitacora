# Wave 3 — Dashboard Distill + Harden (Redesign 2026-04-23)

**Objetivo:** Reducir el peso tablero-vigilancia del dashboard en ready (DashboardSummary), completar la visibilidad de configuración vía ShellMenu extendido, normalizar focus ring en CTAs de entry points, adaptar trendChart a mobile 360px, y blindar la capa de errores user-facing con `UserFacingError` y `formatUserFacingError()`.

**Scope**: R-P1-5, R-P1-6, R-P1-7, R-P1-9, R-P1-10.

---

## T3A — R-P1-5: DashboardSummary en ready — colapsar o texto corrido

**Skill líder:** `impeccable-distill`.

**Archivos:**
- `frontend/components/patient/dashboard/Dashboard.tsx` — cambiar el modo de uso de DashboardSummary en `viewState === 'ready'`.
- `frontend/components/patient/dashboard/DashboardSummary.tsx` — soportar variante `compact` (texto corrido) vs `cards`.
- `frontend/components/patient/dashboard/DashboardSummary.module.css` — estilo texto corrido.

**Evidencia base (Explorer 1):**
- `Dashboard.tsx:218-222`: DashboardSummary renderizado sin condición en ready.
- `DashboardSummary.tsx:33-49`: 3 stat cards con peso tablero.

**Cambio concreto (opción b del audit §4.2):**
Transformar DashboardSummary a texto corrido sobrio:
```tsx
// DashboardSummary.tsx con prop variant
interface Props {
  totalEntries: number;
  avgMoodScore: number | null;
  lastEntryAt: string | null;
  variant?: 'cards' | 'compact';
}

export function DashboardSummary({ totalEntries, avgMoodScore, lastEntryAt, variant = 'cards' }: Props) {
  if (variant === 'compact') {
    const daysAgo = lastEntryAt ? computeDaysAgo(lastEntryAt) : null;
    return (
      <p className={styles.compactSummary}>
        Llevás {totalEntries} {totalEntries === 1 ? 'registro' : 'registros'}
        {daysAgo !== null && `. El último, ${formatDaysAgo(daysAgo)}.`}
      </p>
    );
  }
  // cards variant existente (se conserva para compatibilidad; profesional vista puede usarlo)
  return (/* ... 3 cards originales ... */);
}
```

En `Dashboard.tsx`:
```tsx
<DashboardSummary
  totalEntries={totalEntries}
  avgMoodScore={avgMoodScore}
  lastEntryAt={lastEntryAt}
  variant="compact"
/>
```

**Canon referenciado:**
- Lineamiento 12 §"aire generoso" + §densidad ("nunca debe acercar una pantalla al lenguaje de dashboard").
- Manifesto 10 §12.2 "evitar dashboards del paciente que parezcan tableros de vigilancia".

**Checkpoint T3A:**
- [ ] DashboardSummary en ready renderiza texto corrido, no 3 cards.
- [ ] Texto: `"Llevás N registros. El último, hace X días."` (o formato canon 13).
- [ ] Variant `cards` preservada para llamadas futuras / profesional.
- [ ] Empty state sigue ocultando DashboardSummary (closure 2026-04-22 preservado).

**Commit propuesto:**
```
refactor(w3-distill): R-P1-5 DashboardSummary texto corrido en dashboard ready

DashboardSummary ahora acepta prop variant=cards|compact. En Dashboard ready,
el componente se renderiza con variant=compact y produce un parrafo sobrio:
"Llevas N registros. El ultimo, hace X dias." en lugar de 3 stat cards con
peso tablero.

El variant=cards se preserva para compatibilidad (vista profesional u otras
llamadas futuras). El empty state sigue ocultando DashboardSummary por
completo (closure 2026-04-22 preservado).

Responde audit 2026-04-23 finding E3-F3 P1 + E3-F4 P2. Lineamiento 12
§densidad + Manifesto 10 §12.2 evitar dashboards tablero-vigilancia.

- Gabriel Paz -
```

---

## T3B — R-P1-6: Acceso visible a configuración vía ShellMenu

**Skill líder:** `impeccable-onboard` + `impeccable-clarify`.

**Archivos:**
- `frontend/components/ui/PatientPageShell.tsx` — agregar ítems nuevos al `ShellMenu` ya creado en W1.
- `frontend/components/ui/ShellMenu.module.css` — estilo de los ítems adicionales.

**Evidencia base (Explorer 1):**
- `/configuracion/vinculos` tiene 0 matches en scope del dashboard (invisible).
- ShellMenu ya existe desde T1B W1.

**Cambio concreto:**
Extender los ítems del menu:
```tsx
<ShellMenu>
  <ShellMenuItem onClick={() => router.push('/configuracion/telegram')}>
    Recordatorios
  </ShellMenuItem>
  <ShellMenuItem onClick={() => router.push('/configuracion/vinculos')}>
    Vínculos
  </ShellMenuItem>
  <ShellMenuItem onClick={handleLogout}>
    Cerrar sesión
  </ShellMenuItem>
</ShellMenu>
```

Estilo visual: "Cerrar sesión" con variante más silenciosa o separada (canon 12 §"acciones destructivas separadas"). Puede ser un `<li className={styles.separator}></li>` antes del logout item.

**Canon referenciado:**
- Patrón 16 #12 "Puente de siguiente acción".
- Manifesto 10 §8.1 "controles visibles para activar, desactivar o revocar".

**Checkpoint T3B:**
- [ ] ShellMenu tiene 3 ítems: Recordatorios, Vínculos, Cerrar sesión.
- [ ] Navegación via `router.push` funciona.
- [ ] Logout separado visualmente (separator o color).
- [ ] Keyboard nav (Tab entre ítems, Esc cierra) funciona con los 3 ítems.

**Commit propuesto:**
```
feat(w3-onboard+clarify): R-P1-6 acceso visible a Recordatorios y Vinculos en ShellMenu

ShellMenu ahora contiene tres items: Recordatorios (-> /configuracion/telegram),
Vinculos (-> /configuracion/vinculos) y Cerrar sesion. El logout queda
separado visualmente del resto via separator CSS (canon 12 §"acciones
destructivas separadas").

Responde audit 2026-04-23 findings E3-F5 P1 + E3-F12 P1. Patron 16 #12
"Puente de siguiente accion" + Manifesto 10 §8.1 "controles visibles".

R-P1-4 cerrada en W2 (revocabilidad) ahora tiene correspondencia navegable:
el paciente que lee "Podes revocarlo cuando quieras desde Mi cuenta." tiene
el link visible en ShellMenu -> Vinculos.

- Gabriel Paz -
```

---

## T3C — R-P1-7: Focus ring normalize en CTAs entry points

**Skill líder:** `impeccable-normalize`.

**Archivos (según Explorer 4):**
- `frontend/components/patient/onboarding/OnboardingEntryHero.module.css` — `.primaryCta` falta `:focus-visible`.
- `frontend/components/patient/onboarding/OnboardingEntryHero.module.css` — `.footerLink` falta `:focus-visible`.
- Posibles otros módulos del flujo login con gaps (revisar Explorer 4 output).

**Evidencia base (Explorer 4):**
- `:focus-visible` correcto en: AppState, Dashboard, MoodEntryDialog, PatientPageShell, MoodEntryForm, DailyCheckinForm, TelegramReminderBanner.
- Gaps:
  - `OnboardingEntryHero.module.css:62-85` `.primaryCta` sin `:focus-visible`.
  - `OnboardingEntryHero.module.css:109-115` `.footerLink` sin `:focus-visible` (hallazgo E1-F9 P3).
  - `ProfessionalShell.module.css:50-76` usa outline sin `var(--focus-ring)` (fuera de scope paciente inmediato; NO tocar).
  - `SaveRail.module.css:32-33` usa outline sin `var(--focus-ring)` (revisar si está en flujo login).

**Cambio concreto:**
Aplicar patrón canónico:
```css
.primaryCta:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  box-shadow: var(--focus-ring);
}

.footerLink:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  box-shadow: var(--focus-ring);
}
```

Revisar el snippet devuelto por Explorer 4 y aplicar mismo patrón a otros módulos del flujo login si los hay.

**Canon referenciado:**
- Lineamiento 12 §foco ("verse con claridad; sentirse integrado a la marca").
- WCAG 2.2 §2.4.7 Focus Visible.

**Checkpoint T3C:**
- [ ] `.primaryCta` tiene `:focus-visible` + `var(--focus-ring)`.
- [ ] `.footerLink` tiene `:focus-visible` + `var(--focus-ring)`.
- [ ] Tab a través del landing muestra foco visible en cada CTA.
- [ ] No se rompen estilos existentes (solo se agregan reglas).

**Commit propuesto:**
```
style(w3-normalize): R-P1-7 focus ring canonizado en primaryCta y footerLink del hero

OnboardingEntryHero.module.css agrega :focus-visible con outline 2px + offset +
box-shadow var(--focus-ring) en .primaryCta y .footerLink. Patron canonico de
focus ring (canon 11 + lineamiento 12 §foco) ya aplicado en el resto del flujo
paciente.

Responde audit 2026-04-23 findings E1-F3 P1 + E1-F9 P3. WCAG 2.2 §2.4.7 Focus
Visible.

Se deja fuera de scope ProfessionalShell (rama profesional) y SaveRail (si no
esta en flujo login) para otra wave de normalize global.

- Gabriel Paz -
```

---

## T3D — R-P1-9: trendChart colapsable en 360px

**Skill líder:** `impeccable-adapt`.

**Archivos:**
- `frontend/components/patient/dashboard/Dashboard.tsx` — prop/limite para trendChart en mobile.
- `frontend/components/patient/dashboard/Dashboard.module.css` — media query ≤400px.

**Evidencia base (Explorer 1):**
- `Dashboard.module.css:193-200` — `.trendChart` con `grid-template-columns: repeat(var(--trend-count), minmax(0, 1fr))` y `gap: var(--space-xs)` (4px).
- `.module.css:345` — breakpoint `min-width: 480px` sube gap a `--space-sm`.

**Cambio concreto:**
Dos opciones:
1. **Opción A — limitar entradas en mobile**: `Dashboard.tsx` pasa `--trend-count` máximo de 5 cuando `window.innerWidth < 400` (vía hook `useViewport` o CSS-only con `:is()` y `nth-child(n+6) { display: none }`).
2. **Opción B — CSS-only reflow**: en `@media (max-width: 399px)`, subir `gap: var(--space-sm)` y ocultar las columnas extra.

Recomendado Opción B (CSS-only, sin JS ni hooks nuevos):
```css
@media (max-width: 399px) {
  .trendChart {
    grid-template-columns: repeat(5, minmax(0, 1fr));
    gap: var(--space-sm);
    /* opcional: agregar overflow-x: hidden */
  }
  .trendDay:nth-child(n+6) {
    display: none;
  }
}
```

**Canon referenciado:**
- Lineamiento 12 §mobile-first.
- WCAG 1.4.10 Reflow.

**Checkpoint T3D:**
- [ ] En 360px el trendChart muestra solo 5 entradas con gap legible.
- [ ] En ≥400px el comportamiento actual se preserva.
- [ ] Screenshot manual confirma etiquetas de fecha legibles.

**Commit propuesto:**
```
style(w3-adapt): R-P1-9 trendChart limitado a 5 entradas en 360px con gap legible

Dashboard.module.css agrega @media (max-width: 399px) que limita .trendChart
a 5 columnas con gap var(--space-sm) y oculta columnas n+6 via :nth-child.
Las etiquetas de fecha 11px mono ahora tienen ~60px de ancho efectivo en
lugar de ~28px, recuperando legibilidad en mobile cansado.

Responde audit 2026-04-23 finding E3-F9 P1. Lineamiento 12 §mobile-first +
WCAG 1.4.10 Reflow.

Solucion CSS-only sin hooks ni deps nuevas. Behavior en >=400px preservado
intacto.

- Gabriel Paz -
```

---

## T3E — R-P1-10: `UserFacingError` + `formatUserFacingError()`

**Skill líder:** `impeccable-harden` + `impeccable-clarify`.

**Archivos:**
- `frontend/lib/errors/user-facing.ts` — **archivo nuevo** con tipo + función.
- `frontend/app/error.tsx` — reemplazar sub "Ocurrió algo inesperado".
- `frontend/components/ui/PatientPageShell.tsx` — tipar prop `error` como `UserFacingError | null`.
- `frontend/components/patient/onboarding/OnboardingFlow.tsx` — usar `formatUserFacingError` en catch (líneas 62, 85, 106).
- `frontend/components/patient/vinculos/VinculosManager.tsx` — usar en línea 50, 63.
- `frontend/components/patient/telegram/BindingCodeForm.tsx` — usar en línea 52.
- `frontend/components/patient/dashboard/Dashboard.tsx` — revisar línea 99 del errorMsg (fórmula actual `"No se pudo cargar el historial."` es canon-correcta pero la label en :157 `"Error al cargar el historial"` es canon-prohibida).

**Evidencia base (Explorer 3):**
- Callsites con `err.message` crudo: `OnboardingFlow.tsx:62,85,106`; `VinculosManager.tsx:50,63`; `BindingCodeForm.tsx:52`.
- `app/error.tsx:16`: `"Ocurrió algo inesperado. Probá recargar la página o volver en unos minutos."` sub canon-prohibido.
- `PatientPageShell.tsx:14`: `error?: string | null` sin tipado semántico.

**Cambio concreto:**
1. Crear `frontend/lib/errors/user-facing.ts`:
   ```ts
   export interface UserFacingError {
     title: string;
     description: string;
     retry?: () => void;
   }

   /**
    * Mapea errores tecnicos a mensajes canon 13 usables por UI.
    * NUNCA expone err.message crudo.
    */
   export function formatUserFacingError(
     err: unknown,
     fallback: UserFacingError = {
       title: 'No pudimos completar la acción.',
       description: 'Probá en unos minutos o volvé al inicio.',
     },
   ): UserFacingError {
     // Mapeo de codigos conocidos segun canon 13
     if (err && typeof err === 'object' && 'code' in err) {
       const code = (err as { code?: string }).code;
       switch (code) {
         case 'NETWORK':
           return {
             title: 'Sin conexión estable.',
             description: 'Revisá tu conexión y probá de nuevo.',
           };
         case 'SESSION_EXPIRED':
           return {
             title: 'Tu sesión caducó.',
             description: 'Ingresá de nuevo para continuar.',
           };
         // ...
       }
     }
     return fallback;
   }
   ```

2. `app/error.tsx:16` → reemplazar sub:
   ```tsx
   <p>No pudimos completar la acción. Probá en unos minutos o volvé al inicio.</p>
   ```

3. `PatientPageShell.tsx`:
   ```tsx
   interface Props {
     children?: ReactNode;
     loading?: boolean;
     error?: UserFacingError | null; // <-- cambio
   }
   ```

   El render del error usa `error.title` + `error.description` + CTA opcional con `error.retry`:
   ```tsx
   {error && (
     <div className={styles.errorState} role="alert">
       <p className={styles.errorTitle}>{error.title}</p>
       <p>{error.description}</p>
       {error.retry && (
         <button type="button" onClick={error.retry}>Reintentar</button>
       )}
     </div>
   )}
   ```

4. Callsites (`OnboardingFlow`, `VinculosManager`, `BindingCodeForm`) — reemplazar:
   ```ts
   // ANTES
   setError((err as Error).message ?? 'Error al cargar el consentimiento.');

   // DESPUES
   setError(formatUserFacingError(err, {
     title: 'No pudimos cargar el consentimiento.',
     description: 'Probá de nuevo en unos minutos.',
     retry: () => loadConsent(),
   }));
   ```

5. `Dashboard.tsx:157` — revisar label `"Error al cargar el historial"`; reemplazar por `"No pudimos cargar el historial."` (canon 13).

**Canon referenciado:**
- Canon 13 §Errores ("decir qué pasó + qué puede hacer"; evitar `"Ups. Algo salió mal"` / `"Error al cargar X"`).
- WCAG 4.1.3 Status Messages.

**Checkpoint T3E:**
- [ ] `frontend/lib/errors/user-facing.ts` existe.
- [ ] `UserFacingError` tipo exportado.
- [ ] `formatUserFacingError()` cubre al menos fallback genérico + casos `NETWORK`, `SESSION_EXPIRED`.
- [ ] `app/error.tsx` sub reemplazado.
- [ ] `PatientPageShell` prop `error` tipada.
- [ ] Callsites migrados (6+ sites según Explorer 3).
- [ ] Dashboard.tsx:157 label reemplazada.
- [ ] Grep `err.message|err as Error` en frontend/components/patient/: 0 matches activos (o solo en área profesional fuera de scope).

**Commit propuesto:**
```
feat(w3-harden+clarify): R-P1-10 UserFacingError + formatUserFacingError + capa de copy canon 13

Crea frontend/lib/errors/user-facing.ts con tipo UserFacingError y funcion
formatUserFacingError(err, fallback) que mapea errores tecnicos a mensajes
canon 13 usables por UI. Nunca expone err.message crudo al usuario.

Migrados callsites:
- OnboardingFlow.tsx:62,85,106 (3 catches)
- VinculosManager.tsx:50,63 (2 catches)
- BindingCodeForm.tsx:52 (1 catch)
- Dashboard.tsx:157 label "Error al cargar" -> "No pudimos cargar"
- app/error.tsx:16 sub "Ocurrio algo inesperado" -> "No pudimos completar
  la accion. Proba en unos minutos o volve al inicio."

PatientPageShell prop error ahora es UserFacingError | null en lugar de string
plano. El render soporta title + description + retry callback opcional.

Responde audit 2026-04-23 findings E5-F1 P1 (regresion), E5-F7 P1, E5-F10 P1.
Canon 13 §Errores + WCAG 4.1.3.

- Gabriel Paz -
```

---

## T3F — Cierre W3: review + typecheck + lint + e2e + grep zonas congeladas

**Skill líder:** `ps-code-reviewer` + `ps-worker`.

**Pasos:**
1. Dispatch ps-code-reviewer con diff W3 (5 commits).
2. typecheck + lint + e2e.
3. Grep zonas congeladas.
4. Verificar que `dashboard-modal.spec.ts` sigue verde (los cambios de DashboardSummary + trendChart no deberían afectar los selectors text-based del modal).
5. Checkpoint humano.

**Done when:**
- ps-code-reviewer APROBADO.
- typecheck + lint + 8/8 e2e verde.
- Grep zonas congeladas = 0.

---

## Checkpoint humano W3

Reportar:
- Cadena de commits W3.
- Diff summary.
- Verdict ps-code-reviewer.
- Tests: 8/8 verde.
- Esperar OK humano.

---

## Zonas congeladas — verificación

Archivos W3 permitidos:
- `frontend/components/patient/dashboard/Dashboard.tsx` + `.module.css` ✓
- `frontend/components/patient/dashboard/DashboardSummary.tsx` + `.module.css` ✓
- `frontend/components/ui/PatientPageShell.tsx` ✓
- `frontend/components/ui/ShellMenu.tsx` + `.module.css` ✓
- `frontend/components/patient/onboarding/OnboardingEntryHero.module.css` ✓
- `frontend/components/patient/onboarding/OnboardingFlow.tsx` ✓
- `frontend/components/patient/vinculos/VinculosManager.tsx` ✓
- `frontend/components/patient/telegram/BindingCodeForm.tsx` ✓
- `frontend/lib/errors/user-facing.ts` (nuevo) ✓
- `frontend/app/error.tsx` ✓

Zero imports desde `lib/auth/*`.
