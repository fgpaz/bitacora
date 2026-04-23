import { test, expect, type Page } from '@playwright/test';
import { injectPatientSession } from './helpers/session';

async function stubConsentCurrent(page: Page, status: 'granted' | 'revoked' | 'pending' = 'granted') {
  await page.route('**/api/backend/consent/current', async (route) => {
    if (route.request().method() === 'GET') {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          version: '2026-04-09.v1',
          text: 'Necesitamos tu consentimiento informado...',
          sections: [
            { id: 'data-use', title: 'Uso de tus datos', content: 'test' },
          ],
          patientStatus: status,
        }),
      });
    } else if (route.request().method() === 'DELETE') {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          status: 'revoked',
          revokedAtUtc: new Date().toISOString(),
        }),
      });
    } else {
      await route.fallback();
    }
  });
}

test.describe('CON-002: revocación de consentimiento desde Mi cuenta', () => {
  test.beforeEach(async ({ context }) => {
    await context.clearCookies();
    await injectPatientSession(context);
  });

  test('la ruta /configuracion/consent muestra el panel de revocación con impacto visible', async ({ page }) => {
    await stubConsentCurrent(page, 'granted');

    await page.goto('/configuracion/consent');

    await expect(page.getByRole('heading', { name: 'Revocar consentimiento', level: 1 })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Qué pasa si seguís', level: 2 })).toBeVisible();

    // Impact list should mention suspension, professional access, and revocabilidad.
    await expect(page.getByText(/Se suspende el registro/)).toBeVisible();
    await expect(page.getByText(/profesionales vinculados pierden acceso/)).toBeVisible();
    await expect(page.getByText(/volver a otorgar el consentimiento/)).toBeVisible();

    // Both CTAs present with canon 13 copy.
    await expect(page.getByRole('button', { name: 'Conservar consentimiento' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Revocar consentimiento' })).toBeVisible();
  });

  test('clickear Revocar dispara DELETE /consent/current y muestra confirmación serena', async ({ page }) => {
    await stubConsentCurrent(page, 'granted');

    await page.goto('/configuracion/consent');
    await page.getByRole('button', { name: 'Revocar consentimiento' }).click();

    // Mensaje de confirmación sereno sin celebración.
    await expect(page.getByText(/Tu consentimiento quedó revocado/)).toBeVisible({ timeout: 5000 });
    await expect(page.getByRole('button', { name: 'Volver al dashboard' })).toBeVisible();
  });

  test('el ShellMenu contiene el item Consentimiento que navega a /configuracion/consent', async ({ page }) => {
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
        body: JSON.stringify({ total_entries: 0, avg_mood_score: null, last_entry_at: null }),
      });
    });
    await page.route('**/api/backend/telegram/session', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ linked: true }),
      });
    });

    await page.goto('/dashboard');

    const trigger = page.getByRole('button', { name: 'Mi cuenta' });
    await trigger.click();
    await expect(page.getByRole('menuitem', { name: 'Consentimiento' })).toBeVisible();
  });
});
