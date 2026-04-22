import type { BrowserContext } from '@playwright/test';

export interface FakeSession {
  role: 'patient' | 'professional';
  subject: string;
  email: string;
  accessToken: string;
  expiresInSeconds?: number;
}

/**
 * Inyecta una cookie `bitacora_session` válida para que `proxy.ts` permita
 * acceder a rutas protegidas (/dashboard, /registro, /configuracion, /consent).
 * Formato espejo de `setSessionCookie` en `lib/auth/server.ts`: base64url(JSON).
 */
export async function injectPatientSession(
  context: BrowserContext,
  overrides: Partial<FakeSession> = {},
): Promise<void> {
  const expiresInSeconds = overrides.expiresInSeconds ?? 3600;
  const session = {
    accessToken: overrides.accessToken ?? 'test-access-token',
    subject: overrides.subject ?? 'test-user',
    email: overrides.email ?? 'test@bitacora.local',
    role: overrides.role ?? 'patient',
    expiresAt: Math.floor(Date.now() / 1000) + expiresInSeconds,
  };
  const value = Buffer.from(JSON.stringify(session), 'utf8').toString('base64url');
  await context.addCookies([
    {
      name: 'bitacora_session',
      value,
      domain: 'localhost',
      path: '/',
      httpOnly: true,
      secure: false,
      sameSite: 'Lax',
      expires: session.expiresAt,
    },
  ]);
}
