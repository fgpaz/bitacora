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

async function stubEmptyTimeline(page: Page) {
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
}

test.describe('TelegramReminderBanner', () => {
  test.beforeEach(async ({ context }) => {
    await context.clearCookies();
  });

  test('aparece cuando Telegram no está vinculado', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ linked: false }),
      });
    });

    await page.goto('/dashboard');

    await expect(page.getByText('Recibí recordatorios por Telegram')).toBeVisible();
    await expect(page.getByRole('link', { name: 'Conectar' })).toHaveAttribute(
      'href',
      '/configuracion/telegram',
    );
  });

  test('queda oculto cuando Telegram ya está vinculado', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ linked: true, linked_at: '2026-04-10T00:00:00Z' }),
      });
    });

    await page.goto('/dashboard');

    await expect(page.getByText('Empezá con tu primer registro')).toBeVisible();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);
  });

  test('"Ahora no" oculta el banner y persiste el dismiss', async ({ page }) => {
    await stubAuthenticatedSession(page);
    await stubEmptyTimeline(page);
    await page.route('**/api/backend/telegram/session', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ linked: false }),
      });
    });

    await page.goto('/dashboard');
    await page.getByRole('button', { name: 'Ahora no' }).click();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);

    const dismissedAt = await page.evaluate(() =>
      window.localStorage.getItem('bitacora.telegram.banner.dismissedAt'),
    );
    expect(dismissedAt).not.toBeNull();

    await page.reload();
    await expect(page.getByText('Recibí recordatorios por Telegram')).toHaveCount(0);
  });
});
