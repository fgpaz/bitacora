/**
 * Client-side auth commands.
 *
 * Tokens stay in httpOnly cookies managed by Next route handlers. Browser code
 * only starts login, starts logout, or checks whether a product session exists.
 */

export async function signInWithZitadel(email?: string): Promise<{ error: string | null }> {
  const loginUrl = new URL('/ingresar', window.location.origin);
  if (email?.trim()) {
    loginUrl.searchParams.set('login_hint', email.trim());
  }
  window.location.assign(loginUrl.toString());
  return { error: null };
}

export async function signOut(): Promise<void> {
  window.location.assign('/auth/logout');
}

export async function getAccessToken(): Promise<string | null> {
  const response = await fetch('/api/auth/session', { cache: 'no-store' });
  if (!response.ok) return null;

  const session = await response.json() as { user?: unknown };
  return session.user ? 'server-session' : null;
}
