import { test, expect, type Page } from '@playwright/test';

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

test.describe('Dashboard: modal de nuevo registro', () => {
  test('CTA "Registrar humor" abre modal y permite cerrar', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyDashboard(page);

    await page.goto('/dashboard');

    await expect(page.getByText('Empezá con tu primer registro')).toBeVisible();

    await page.getByRole('button', { name: 'Registrar humor' }).click();

    const dialogTitle = page.getByRole('heading', { name: 'Nuevo registro' });
    await expect(dialogTitle).toBeVisible();

    await page.getByRole('button', { name: 'Cerrar' }).click();
    await expect(dialogTitle).not.toBeVisible();
  });

  test('guardar un registro refresca el historial sin salir del dashboard', async ({ page }) => {
    await stubAuthenticatedSession(page);

    let createCalls = 0;
    let summaryCalls = 0;
    await page.route('**/api/backend/visualizacion/timeline**', async (route) => {
      const entries = createCalls === 0
        ? []
        : [{ date: '2026-04-22', mood_score: 2 }];
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ entries }),
      });
    });
    await page.route('**/api/backend/visualizacion/summary**', async (route) => {
      summaryCalls += 1;
      const body = createCalls === 0
        ? { total_entries: 0, avg_mood_score: null, last_entry_at: null }
        : { total_entries: 1, avg_mood_score: 2, last_entry_at: '2026-04-22T10:00:00Z' };
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(body),
      });
    });
    await page.route('**/api/backend/telegram/session', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ linked: true }),
      });
    });
    await page.route('**/api/backend/mood-entries', async (route) => {
      if (route.request().method() === 'POST') {
        createCalls += 1;
        await route.fulfill({
          status: 201,
          contentType: 'application/json',
          body: JSON.stringify({ id: 'entry-1' }),
        });
      } else {
        await route.fallback();
      }
    });

    await page.goto('/dashboard');
    await page.getByRole('button', { name: 'Registrar humor' }).click();

    await page.getByRole('radio', { name: '+2' }).click();
    await page.getByRole('button', { name: 'Guardar' }).click();

    await expect(page.getByText('Registro guardado.')).toBeVisible();
    expect(summaryCalls).toBeGreaterThan(1);

    await expect(page).toHaveURL(/\/dashboard$/);
  });
});
