import { NextRequest, NextResponse } from 'next/server';
import {
  OIDC_NONCE_COOKIE,
  OIDC_STATE_COOKIE,
  OIDC_VERIFIER_COOKIE,
} from '@/lib/auth/constants';
import { getAuthRuntimeConfig, setTransientCookie } from '@/lib/auth/server';

export const runtime = 'nodejs';

export async function GET(request: NextRequest) {
  const origin = request.nextUrl.origin;
  const config = getAuthRuntimeConfig(origin);
  const state = randomBase64Url(32);
  const nonce = randomBase64Url(32);
  const verifier = randomBase64Url(64);
  const challenge = await sha256Base64Url(verifier);

  const authorizeUrl = new URL('/oauth/v2/authorize', config.issuer);
  authorizeUrl.searchParams.set('response_type', 'code');
  authorizeUrl.searchParams.set('client_id', config.clientId);
  authorizeUrl.searchParams.set('redirect_uri', config.redirectUri);
  authorizeUrl.searchParams.set('scope', 'openid profile email');
  authorizeUrl.searchParams.set('state', state);
  authorizeUrl.searchParams.set('nonce', nonce);
  authorizeUrl.searchParams.set('code_challenge', challenge);
  authorizeUrl.searchParams.set('code_challenge_method', 'S256');

  const loginHint = request.nextUrl.searchParams.get('login_hint')
    ?? request.nextUrl.searchParams.get('email');
  if (loginHint) {
    authorizeUrl.searchParams.set('login_hint', loginHint);
  }

  const response = NextResponse.redirect(authorizeUrl);
  setTransientCookie(response, OIDC_STATE_COOKIE, state);
  setTransientCookie(response, OIDC_NONCE_COOKIE, nonce);
  setTransientCookie(response, OIDC_VERIFIER_COOKIE, verifier);
  return response;
}

function randomBase64Url(bytes: number): string {
  return Buffer.from(crypto.getRandomValues(new Uint8Array(bytes))).toString('base64url');
}

async function sha256Base64Url(value: string): Promise<string> {
  const digest = await crypto.subtle.digest('SHA-256', new TextEncoder().encode(value));
  return Buffer.from(digest).toString('base64url');
}
