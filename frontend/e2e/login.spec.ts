import { test, expect } from '@playwright/test';

test('la pagina de inicio muestra el CTA correcto', async ({ page }) => {
  await page.goto('http://localhost:3000');
  
  // Verificar que el boton principal tiene el texto clarificado
  const ctaButton = page.locator('button[type="submit"]');
  await expect(ctaButton).toHaveText('Iniciar sesión con Zitadel');
  
  // Verificar que el label del campo email es claro
  const emailLabel = page.locator('label[for="onboarding-email"]');
  await expect(emailLabel).toHaveText('Correo (opcional, pre-completa el login)');
  
  // Verificar que el footer tiene el link de soporte
  const footerLink = page.locator('footer a');
  await expect(footerLink).toHaveAttribute('href', 'mailto:soporte@nuestrascuentitas.com');
});

test('sin autenticacion redirige al inicio desde dashboard', async ({ page }) => {
  // Verificar que sin sesion, el dashboard redirige al inicio
  await page.goto('http://localhost:3000/dashboard');
  
  // Verificar que fuimos redirigidos a la pagina de inicio
  await expect(page).toHaveURL('http://localhost:3000/');
  
  // Verificar que estamos en la pagina de login
  const ctaButton = page.locator('button[type="submit"]');
  await expect(ctaButton).toHaveText('Iniciar sesión con Zitadel');
});