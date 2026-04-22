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
    await expect(page.locator('a[href^="mailto:"]')).toHaveAttribute(
      'href',
      'mailto:soporte@nuestrascuentitas.com',
    );
  });

  test('/ingresar responde con redirect a Zitadel', async ({ request }) => {
    const response = await request.get('/ingresar', { maxRedirects: 0 });
    expect([302, 303, 307]).toContain(response.status());

    const location = response.headers()['location'];
    expect(location).toBeTruthy();
    expect(location).toMatch(/oauth\/v2\/authorize/);
  });
});
