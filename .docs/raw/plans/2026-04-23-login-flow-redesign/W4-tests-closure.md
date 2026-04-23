# Wave 4 — Tests + Trazabilidad + Closure (Redesign 2026-04-23)

**Objetivo:** Cerrar la rama con tests verdes, trazabilidad documental completa, y un reporte de closure que replique el shape del 2026-04-22-impeccable-hardening-closure.md. La rama queda lista para PR humano, NO se mergea a main.

**Scope**: ajuste de specs e2e existentes, posibles nuevos specs, typecheck+lint+e2e obligatorio 8/8 verde, `ps-trazabilidad`, `ps-auditar-trazabilidad`, closure report final.

---

## T4A — Ajuste e2e existentes + nuevos specs

**Skill líder:** `ps-worker`.

**Archivos:**
- `frontend/e2e/landing.spec.ts` — revisar si requiere ajustes post-Server-Component.
- `frontend/e2e/dashboard-modal.spec.ts` — revisar selectors post-rediseño (rail superior, DashboardSummary compact, modal cierre auto).
- `frontend/e2e/telegram-banner.spec.ts` — revisar si cambios en el dashboard rompen algo.
- `frontend/e2e/global-error.spec.ts` — **archivo nuevo opcional** si se decide validar global-error vía Playwright (forzando un throw). Puede omitirse si agregar un spec fuerza infra adicional.
- `frontend/e2e/logout-menu.spec.ts` — **archivo nuevo opcional** para verificar el ShellMenu (abrir, cerrar con Esc, ítem logout presente). Recomendado.

**Cambios esperados en specs existentes:**

1. `landing.spec.ts` — cambios **ninguno o mínimos**. El test corre sin cookie, por lo que el Server Component sigue renderizando `variant="standard"` con headline congelado `"Tu espacio personal de registro"`. Verificar que el spec no hace assert sobre ausencia total de `variant="returning"`.

2. `dashboard-modal.spec.ts`:
   - Selectors `getByRole('button', { name: /Nuevo registro/ })` siguen matching (text-based, tolera el cambio de posición del rail).
   - **Nuevo test** o extensión del existente: tras guardar un mood, el modal debe cerrarse automáticamente y debe aparecer toast `"Registro sumado a tu historial."`.
   - `expect(page.locator('dialog[open]')).toHaveCount(0)` post-save con timeout 2000ms.
   - `expect(page.getByRole('status')).toContainText(/Registro sumado/)`.

3. `telegram-banner.spec.ts` — revisar que el banner sigue visible en ready con DashboardSummary compact.

**Spec nuevo recomendado `logout-menu.spec.ts`** (simple):
```ts
import { test, expect } from '@playwright/test';
import { stubAuthenticatedSession } from './helpers/session';

test.describe('ShellMenu: logout protegido por overflow', () => {
  test('El trigger ⋯ abre el menu y contiene Cerrar sesión', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await page.goto('/dashboard');
    const trigger = page.getByRole('button', { name: /Mi cuenta/ });
    await expect(trigger).toBeVisible();
    await expect(trigger).toHaveAttribute('aria-expanded', 'false');
    await trigger.click();
    await expect(trigger).toHaveAttribute('aria-expanded', 'true');
    await expect(page.getByRole('menuitem', { name: /Cerrar sesión/ })).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(trigger).toHaveAttribute('aria-expanded', 'false');
  });
});
```

**Checkpoint T4A:**
- [ ] Los 3 specs existentes siguen verdes tras cambios.
- [ ] `dashboard-modal.spec.ts` tiene aserción de cierre auto del modal + toast.
- [ ] `logout-menu.spec.ts` agregado y verde (si se decidió crearlo).
- [ ] Total: 8/8 verde o más (dependiendo si se agregan 1-2 specs nuevos).

**Commit propuesto:**
```
test(w4): ajuste e2e para rediseno login flow + spec nuevo logout-menu

dashboard-modal.spec.ts: agrega assertion de cierre auto del modal post-save
y toast "Registro sumado a tu historial." (R-P0-4).

logout-menu.spec.ts (nuevo): verifica ShellMenu con aria-expanded, apertura,
item Cerrar sesion visible, Esc cierra (R-P0-2).

Selectors existentes text-based toleran el cambio de posicion del rail de
accion (R-P0-1) sin ajustes.

- Gabriel Paz -
```

---

## T4B — typecheck + lint + 8/8 e2e verde obligatorio

**Skill líder:** `ps-worker`.

**Comandos:**
```bash
cd frontend
npm run typecheck   # exit 0 obligatorio
npm run lint        # exit 0 obligatorio
npm run test:e2e    # 8/8 o más verdes
```

Si cualquiera falla: diagnóstico + fix iterativo. No avanzar con rojo.

**Done when:**
- typecheck + lint exit 0.
- e2e: todos los specs verdes (minimum 8; 9 o 10 si se agregaron nuevos en T4A).

---

## T4C — `ps-trazabilidad` final

**Skill líder:** `main`.

**Pasos:**
1. Invocar `Skill("ps-trazabilidad")` con scope del feature branch completo.
2. Validar que:
   - Los cambios no tocaron RF sin actualizar canon: los cambios UI-only (dashboard, consent, modal) NO modifican RF-ONB-003/004/005 (no cambian contratos ni flows).
   - FL-CON-01 puede requerir nota sobre "CTA secundario de rechazo en consent" — evaluar si el cambio de UX amerita entrada en FL-CON-01.
   - Canon 23_uxui puede quedar con drift conocido (ver §Follow-ups no bloqueantes del closure anterior).
3. Reportar: gaps críticos = 0; drifts documentados en follow-ups.

**Done when:**
- `ps-trazabilidad` devuelve 0 gaps críticos.
- Drifts documentados explícitamente en closure.

---

## T4D — `ps-auditar-trazabilidad` full

**Skill líder:** `main`.

**Pasos:**
1. Invocar `Skill("ps-auditar-trazabilidad")` en modo full.
2. Cross-check: baseline (audit 2026-04-23) + plan (este documento) + cadena de commits + canon (wiki).
3. Reportar verdict: `0 critical gaps` o listado explícito.

**Done when:**
- Audit cross-check sin gaps críticos.
- Drifts conocidos documentados.

---

## T4E — Closure report

**Skill líder:** `main` + opcional `ps-docs`.

**Archivo:**
- `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` — **archivo nuevo**.

**Shape de referencia:** `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

**Secciones obligatorias:**
1. **Metadata** — Fecha, rama, base commit, HEAD, classification, compliance (Ley 25.326/26.529/26.657).
2. **Cadena de commits** — tabla con # / commit hash / wave / alcance.
3. **Checkpoints intermedios** — CKP1/CKP2/CKP3 con resultado PASS/FAIL + notas.
4. **Verdict final** — tabla de verificaciones de cierre (governance in_sync, zonas congeladas, backend/schema sin tocar, tests verdes, grep deprecados, typecheck/lint/e2e).
5. **Sync canon wiki** — si se tocó canon.
6. **Resumen por dimensión impeccable** — distill, harden, clarify, normalize, onboard, adapt aplicados.
7. **Follow-ups no bloqueantes**:
   - ⚠ legal-review R-P1-3 antes de deploy (wording "Ahora no" + mensaje decline).
   - Sync 23_uxui por slice (UJ-ONB-*, UXS-ONB-*) si se decide abrirlos.
   - Analytics / telemetría (seguimiento time-to-CTA, CTR rail).
   - 2 modules CSS con focus-ring gap fuera de scope paciente (ProfessionalShell, SaveRail).
   - Test nuevo global-error.spec.ts (decidir si se agrega).
8. **Deuda explícitamente aceptada**:
   - DashboardSummary `variant=cards` se preserva para profesional (no se refactoriza ahora).
   - Middleware vs Server Component: se optó por Server Component; si el equipo auth habilita tocar proxy.ts en el futuro, se puede migrar sin cambios de UX.
9. **Tests Playwright** — 8+ specs, 8/8 passed.
10. **Estado del repo al cierre** — branch ahead de main por N commits + HEAD + git status + métricas.
11. **Recomendación para merge** — PR con este report como descripción; legal-review para R-P1-3 antes del deploy.

**Done when:**
- Archivo persistido en `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.
- Shape espejo del closure 2026-04-22.
- Follow-ups y deuda explícitos.

---

## Entregable final al humano

```
✓ Feature branch: feature/login-flow-redesign-2026-04-23
✓ Commits totales: N (W0 + W1 + W2 + W3 + W4)
✓ HEAD: <hash>
✓ typecheck + lint: exit 0
✓ e2e: 8/8 (o más) verdes
✓ Zonas congeladas: 0 cruces
✓ Closure report: .docs/raw/reports/2026-04-23-login-flow-redesign-closure.md
✓ Verdict: needs-redesign → resuelto para dashboard como hub; needs-refinement → resuelto para consent, onboarding, modal, shell, edge.
⚠ R-P1-3 requiere legal-review antes de deploy.

Próxima acción del humano:
1. Review del closure + diff.
2. Coordinar legal-review del wording del CTA "Ahora no".
3. PR a main (no-ff, preservando historia de waves).
4. Deploy a Dokploy post-OK legal.
```

---

## Zonas congeladas — verificación final

Grep recursivo del diff completo del feature branch:
```bash
git diff main...HEAD --name-only | grep -E "^(frontend/lib/auth/|frontend/app/api/|frontend/app/auth/|frontend/proxy\.ts|frontend/src/)"
# Debe devolver 0 matches
```

Si devuelve matches: ABORT + re-auditar + revertir.
