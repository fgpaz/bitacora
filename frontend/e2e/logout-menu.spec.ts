import { test, expect, type Page } from '@playwright/test';
import { injectPatientSession } from './helpers/session';

async function stubAuthenticatedSession(page: Page) {
  await page.route('**/api/auth/session', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({
        user: { id: 'test-user', email: 'test@bitacora.local', role: 'patient' },
        expiresAt: Date.now() + 3600_000,
      }),
    });
  });
}

async function stubEmptyDashboard(page: Page) {
  await page.route('**/api/backend/visualizacion/timeline**', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ entries: [] }),
    });
  });
  await page.route('**/api/backend/visualizacion/summary**', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({
        total_entries: 0,
        avg_mood_score: null,
        last_entry_at: null,
      }),
    });
  });
  await page.route('**/api/backend/telegram/session', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ linked: true }),
    });
  });
}

test.describe('ShellMenu: logout + configuracion protegidos por overflow', () => {
  test.beforeEach(async ({ context }) => {
    await context.clearCookies();
    await injectPatientSession(context);
  });

  test('el trigger "Mi cuenta" abre el menu y contiene los items esperados', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyDashboard(page);

    await page.goto('/dashboard');

    const trigger = page.getByRole('button', { name: 'Mi cuenta' });
    await expect(trigger).toBeVisible();
    await expect(trigger).toHaveAttribute('aria-expanded', 'false');

    // R-P0-2: logout NO debe ser el primer elemento interactivo del shell.
    // Abrimos el menu explicitamente.
    await trigger.click();
    await expect(trigger).toHaveAttribute('aria-expanded', 'true');

    // R-P0-2 + R-P1-6: los 3 items visibles dentro del menu.
    await expect(page.getByRole('menuitem', { name: 'Recordatorios' })).toBeVisible();
    await expect(page.getByRole('menuitem', { name: 'Vínculos' })).toBeVisible();
    await expect(page.getByRole('menuitem', { name: 'Cerrar sesión' })).toBeVisible();
  });

  test('Escape cierra el menu y retorna foco al trigger', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyDashboard(page);

    await page.goto('/dashboard');

    const trigger = page.getByRole('button', { name: 'Mi cuenta' });
    await trigger.click();
    await expect(trigger).toHaveAttribute('aria-expanded', 'true');

    await page.keyboard.press('Escape');
    await expect(trigger).toHaveAttribute('aria-expanded', 'false');
  });
});
