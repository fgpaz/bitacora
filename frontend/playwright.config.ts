import { defineConfig, devices } from '@playwright/test';

// Flakiness mitigation (2026-04-23 follow-up #5): el dev server de Next se satura
// con varios workers disparando requests simultaneos (dashboard-modal + logout-menu
// timeouts intermitentes). Mantenemos fullyParallel=true para velocidad pero con
// workers=1 por default y retries=1 local. CI ya era workers=1 + retries=2.
// Cuando el equipo hostee un build de produccion para tests, re-habilitar paralelismo.
export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 1,
  workers: process.env.CI ? 1 : 1,
  reporter: 'list',
  use: {
    baseURL: process.env.BASE_URL || 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    launchOptions: process.platform === 'linux'
      ? {
          executablePath: '/usr/bin/chromium',
          args: ['--no-sandbox', '--disable-setuid-sandbox'],
        }
      : {},
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});