import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';
import {
  OIDC_NONCE_COOKIE,
  OIDC_STATE_COOKIE,
  OIDC_VERIFIER_COOKIE,
} from '@/lib/auth/constants';
import {
  clearAuthCookies,
  clearTransientAuthCookies,
  createSessionFromAccessToken,
  getAuthRuntimeConfig,
  setSessionCookie,
} from '@/lib/auth/server';

export const runtime = 'nodejs';

export async function GET(request: NextRequest) {
  const origin = request.nextUrl.origin;
  const config = getAuthRuntimeConfig(origin);
  const publicOrigin = resolvePublicOrigin(config.redirectUri, origin);
  const code = request.nextUrl.searchParams.get('code');
  const state = request.nextUrl.searchParams.get('state');
  const cookieStore = await cookies();
  const expectedState = cookieStore.get(OIDC_STATE_COOKIE)?.value;
  const verifier = cookieStore.get(OIDC_VERIFIER_COOKIE)?.value;
  const expectedNonce = cookieStore.get(OIDC_NONCE_COOKIE)?.value;

  if (!code || !state || !expectedState || state !== expectedState || !verifier || !expectedNonce) {
    return failedRedirect(publicOrigin);
  }

  try {
    const tokenResponse = await fetch(new URL('/oauth/v2/token', config.issuer), {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'authorization_code',
        code,
        redirect_uri: config.redirectUri,
        client_id: config.clientId,
        code_verifier: verifier,
      }),
      cache: 'no-store',
    });

    if (!tokenResponse.ok) {
      return failedRedirect(publicOrigin);
    }

    const tokenSet = await tokenResponse.json() as { access_token?: string; id_token?: string };
    if (!tokenSet.access_token) {
      return failedRedirect(publicOrigin);
    }

    const session = createSessionFromAccessToken(tokenSet.access_token, config, tokenSet.id_token, expectedNonce);
    const response = NextResponse.redirect(new URL('/onboarding', publicOrigin));
    clearTransientAuthCookies(response);
    setSessionCookie(response, session);
    return response;
  } catch {
    return failedRedirect(publicOrigin);
  }
}

function resolvePublicOrigin(redirectUri: string, fallbackOrigin: string): string {
  try {
    return new URL(redirectUri).origin;
  } catch {
    return fallbackOrigin;
  }
}

function failedRedirect(origin: string): NextResponse {
  const response = NextResponse.redirect(new URL('/?auth_error=callback', origin));
  clearAuthCookies(response);
  return response;
}
