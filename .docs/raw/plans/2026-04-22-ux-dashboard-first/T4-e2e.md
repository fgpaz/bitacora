# Task T4: Playwright E2E — landing, dashboard-modal, telegram-banner

## Shared Context
**Goal:** Cobertura E2E del rediseño: 3 specs (landing pública, modal de registro, banner Telegram) + reemplazar el obsoleto `login.spec.ts`.
**Stack:** Playwright 1.59, Chromium headless, baseURL `http://localhost:3000` (env override con `BASE_URL`).
**Architecture:** Specs usan `page.route()` para mockear endpoints del backend y `/api/auth/session` para simular sesión autenticada.

## Locked Decisions
- Borrar `frontend/e2e/login.spec.ts` (sus asserts refieren a copy ya eliminado).
- Nombres: `landing.spec.ts`, `dashboard-modal.spec.ts`, `telegram-banner.spec.ts`.
- Los specs autenticados mockean `/api/auth/session`, `/api/backend/visualizacion/timeline`, `/api/backend/visualizacion/summary`, `/api/backend/telegram/session` y `/api/backend/mood-entries`.
- Spec de landing NO mockea nada: asserts directos sobre HTML público.
- Dev server: el orquestador arranca `npm run dev` en foreground y lo deja correr; el runner hace `npm run test:e2e`. NO arrancarlo desde `playwright.config.ts.webServer` en este plan.

## Task Metadata
```yaml
id: T4
depends_on: [T1, T2, T3]
agent_type: ps-next-vercel
files:
  - delete: frontend/e2e/login.spec.ts
  - create: frontend/e2e/landing.spec.ts
  - create: frontend/e2e/dashboard-modal.spec.ts
  - create: frontend/e2e/telegram-banner.spec.ts
  - read: frontend/playwright.config.ts
complexity: medium
done_when: "cd frontend && npm run dev & (wait 15s) && npm run test:e2e exits 0; luego matar dev server"
```

## Reference
- `frontend/playwright.config.ts` — baseURL, executablePath Chromium.
- `frontend/lib/api/client.ts:206-215` — shape de `TelegramSessionResponse`.
- `frontend/lib/api/client.ts:43` — shape de `ConsentGrantResponse`.

## Prompt
Sos `ps-next-vercel`. Escribís tres specs de Playwright y corrés el runner contra un dev server local.

Reglas duras:
- NO modificar `playwright.config.ts` en este task.
- NO asumir cookie de sesión firmada: mockeá `/api/auth/session` con `page.route()`.
- Los asserts deben ser estables (por role/label, no por xpath frágil).
- Dev server debe arrancar antes del runner; tras `test:e2e`, detener el dev server.
- Si el mock no alcanza al `POST /api/backend/mood-entries`, el test debe fallar explícitamente — no silenciar.

## Execution Procedure
1. Ejecutar `rm frontend/e2e/login.spec.ts`. Verificar con `ls frontend/e2e/`.
2. Crear los tres specs con los Skeletons A, B, C.
3. Abrir un segundo shell (Bash) y ejecutar:
   ```bash
   cd frontend && npm run dev
   ```
   Esperar 15s hasta ver "Ready in" y `http://localhost:3000` responde 200.
4. En el shell original: `cd frontend && npm run test:e2e`. Esperar exit 0.
5. Si algún spec falla por timing: aumentar el `await page.waitForLoadState('networkidle')` en el setup. No bajar assertions.
6. Detener el dev server del shell auxiliar (Ctrl-C).
7. Commit.

## Skeleton

Skeleton A — `frontend/e2e/landing.spec.ts`:
```ts
import { test, expect } from '@playwright/test';

test.describe('Landing pública', () => {
  test('muestra un único CTA "Ingresar" sin campo de email', async ({ page }) => {
    await page.goto('/');
    const cta = page.getByRole('link', { name: 'Ingresar' });
    await expect(cta).toBeVisible();
    await expect(cta).toHaveAttribute('href', '/ingresar');
    await expect(page.locator('input[type="email"]')).toHaveCount(0);
    await expect(page.locator('button[type="submit"]')).toHaveCount(0);
    await expect(page.getByText('Revisá tu correo')).toHaveCount(0);
  });

  test('tiene headline, privacidad y soporte visibles', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'Tu espacio personal de registro' })).toBeVisible();
    await expect(page.getByText(/La privacidad de tus datos es fundamental/)).toBeVisible();
    await expect(page.locator('a[href^="mailto:"]')).toHaveAttribute('href', 'mailto:soporte@nuestrascuentitas.com');
  });

  test('/ingresar responde con redirect a Zitadel', async ({ request }) => {
    const response = await request.get('/ingresar', { maxRedirects: 0 });
    expect([302, 303, 307]).toContain(response.status());
    const location = response.headers()['location'];
    expect(location).toMatch(/oauth\/v2\/authorize/);
  });
});
```

Skeleton B — `frontend/e2e/dashboard-modal.spec.ts`:
```ts
import { test, expect, type Page } from '@playwright/test';

async function stubAuthenticatedSession(page: Page) {
  await page.route('**/api/auth/session', async (route) => {
    await route.fulfill({ status: 200, contentType: 'application/json',
      body: JSON.stringify({ user: { id: 'u1', email: 't@b.local', role: 'patient' }, expiresAt: Date.now() + 3_600_000 }) });
  });
}

async function stubEmptyDashboard(page: Page) {
  await page.route('**/api/backend/visualizacion/timeline**', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ entries: [] }) }));
  await page.route('**/api/backend/visualizacion/summary**', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ total_entries: 0, avg_mood_score: null, last_entry_at: null }) }));
  await page.route('**/api/backend/telegram/session', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ linked: true }) }));
}

test.describe('Dashboard: modal de nuevo registro', () => {
  test('CTA "Registrar humor" abre y cierra el modal', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyDashboard(page);
    await page.goto('/dashboard');
    await expect(page.getByText('Empezá con tu primer registro')).toBeVisible();
    await page.getByRole('button', { name: 'Registrar humor' }).click();
    await expect(page.getByRole('heading', { name: 'Nuevo registro' })).toBeVisible();
    await page.getByRole('button', { name: 'Cerrar' }).click();
    await expect(page.getByRole('heading', { name: 'Nuevo registro' })).not.toBeVisible();
  });

  test('guardar un registro refresca el historial sin salir', async ({ page }) => {
    await stubAuthenticatedSession(page);
    let saved = 0;
    await page.route('**/api/backend/visualizacion/timeline**', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json',
        body: JSON.stringify({ entries: saved === 0 ? [] : [{ date: '2026-04-22', mood_score: 2 }] }) }));
    await page.route('**/api/backend/visualizacion/summary**', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json',
        body: JSON.stringify(saved === 0
          ? { total_entries: 0, avg_mood_score: null, last_entry_at: null }
          : { total_entries: 1, avg_mood_score: 2, last_entry_at: '2026-04-22T10:00:00Z' }) }));
    await page.route('**/api/backend/telegram/session', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ linked: true }) }));
    await page.route('**/api/backend/mood-entries', async (r) => {
      if (r.request().method() === 'POST') { saved += 1; return r.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify({ id: 'e1' }) }); }
      return r.fallback();
    });
    await page.goto('/dashboard');
    await page.getByRole('button', { name: 'Registrar humor' }).click();
    await page.getByRole('radio', { name: '+2' }).click();
    await page.getByRole('button', { name: 'Guardar' }).click();
    await expect(page.getByText('Registro guardado.')).toBeVisible();
    await expect(page).toHaveURL(/\/dashboard$/);
  });
});
```

Skeleton C — `frontend/e2e/telegram-banner.spec.ts`:
```ts
import { test, expect, type Page } from '@playwright/test';

async function stubAuth(page: Page) {
  await page.route('**/api/auth/session', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json',
      body: JSON.stringify({ user: { id: 'u1', email: 't@b.local', role: 'patient' }, expiresAt: Date.now() + 3_600_000 }) }));
}

async function stubEmptyTimeline(page: Page) {
  await page.route('**/api/backend/visualizacion/timeline**', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ entries: [] }) }));
  await page.route('**/api/backend/visualizacion/summary**', async (r) =>
    r.fulfill({ status: 200, contentType: 'application/json',
      body: JSON.stringify({ total_entries: 0, avg_mood_score: null, last_entry_at: null }) }));
}

test.describe('TelegramReminderBanner', () => {
  test.beforeEach(async ({ context }) => { await context.clearCookies(); });

  test('visible cuando no está vinculado', async ({ page }) => {
    await stubAuth(page); await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ linked: false }) }));
    await page.goto('/dashboard');
    await expect(page.getByText('Recibí recordatorios por Telegram')).toBeVisible();
    await expect(page.getByRole('link', { name: 'Conectar' })).toHaveAttribute('href', '/configuracion/telegram');
  });

  test('oculto cuando está vinculado', async ({ page }) => {
    await stubAuth(page); await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ linked: true }) }));
    await page.goto('/dashboard');
    await expect(page.getByText('Empezá con tu primer registro')).toBeVisible();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);
  });

  test('dismiss persiste 30 días', async ({ page }) => {
    await stubAuth(page); await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (r) =>
      r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ linked: false }) }));
    await page.goto('/dashboard');
    await page.getByRole('button', { name: 'Ahora no' }).click();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);
    const ts = await page.evaluate(() => window.localStorage.getItem('bitacora.telegram.banner.dismissedAt'));
    expect(ts).not.toBeNull();
    await page.reload();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);
  });
});
```

## Verify
```bash
# shell auxiliar:
cd frontend && npm run dev   # dejar corriendo hasta ver "Ready"
# shell principal:
cd frontend && npm run test:e2e   # expected: 3 specs, todos PASS, exit 0
# luego detener dev server
```

## Commit
```
test(e2e): specs landing + dashboard modal + telegram banner
```
