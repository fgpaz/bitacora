import { cookies } from 'next/headers';
import type { NextResponse } from 'next/server';
import {
  OIDC_NONCE_COOKIE,
  OIDC_STATE_COOKIE,
  OIDC_VERIFIER_COOKIE,
  SESSION_COOKIE,
  ZITADEL_ROLES_CLAIM,
  type BitacoraRole,
  type PublicSession,
} from './constants';

const DEFAULT_ISSUER = 'https://id.nuestrascuentitas.com';
const DEFAULT_CLIENT_ID = '369306336963330406';
const DEFAULT_AUDIENCE = '369306332534145382';

export interface AuthRuntimeConfig {
  issuer: string;
  clientId: string;
  audience: string;
  redirectUri: string;
  postLogoutRedirectUri: string;
}

export interface BitacoraSession {
  accessToken: string;
  subject: string;
  email: string;
  role: BitacoraRole;
  expiresAt: number;
}

export function getAuthRuntimeConfig(origin: string): AuthRuntimeConfig {
  const issuer = normalizeIssuer(
    process.env.ZITADEL_ISSUER
      ?? process.env.ZITADEL_AUTHORITY
      ?? process.env.NEXT_PUBLIC_ZITADEL_ISSUER
      ?? process.env.ZITADEL_EXTERNALDOMAIN
      ?? DEFAULT_ISSUER,
  );

  return {
    issuer,
    clientId: process.env.ZITADEL_WEB_CLIENT_ID
      ?? process.env.ZITADEL_CLIENT_BITACORA_WEB_ID
      ?? process.env.NEXT_PUBLIC_ZITADEL_CLIENT_ID
      ?? DEFAULT_CLIENT_ID,
    audience: process.env.ZITADEL_AUDIENCE
      ?? process.env.ZITADEL_PROJECT_BITACORA_ID
      ?? DEFAULT_AUDIENCE,
    redirectUri: process.env.ZITADEL_WEB_REDIRECT_URI
      ?? process.env.NEXT_PUBLIC_ZITADEL_REDIRECT_URI
      ?? `${origin}/auth/callback`,
    postLogoutRedirectUri: process.env.ZITADEL_WEB_POST_LOGOUT_REDIRECT_URI
      ?? process.env.NEXT_PUBLIC_ZITADEL_POST_LOGOUT_REDIRECT_URI
      ?? origin,
  };
}

export function getBackendBaseUrl(): string {
  return (process.env.API_BASE_URL
    ?? process.env.NEXT_PUBLIC_API_BASE_URL
    ?? 'http://localhost:5000').replace(/\/$/, '');
}

export function setTransientCookie(response: NextResponse, name: string, value: string): void {
  response.cookies.set(name, value, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    path: '/',
    maxAge: 10 * 60,
  });
}

export function setSessionCookie(response: NextResponse, session: BitacoraSession): void {
  const maxAge = Math.max(0, session.expiresAt - Math.floor(Date.now() / 1000));
  response.cookies.set(SESSION_COOKIE, encodeSession(session), {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    path: '/',
    maxAge,
  });
}

export function clearAuthCookies(response: NextResponse): void {
  response.cookies.delete(SESSION_COOKIE);
  response.cookies.delete(OIDC_STATE_COOKIE);
  response.cookies.delete(OIDC_VERIFIER_COOKIE);
  response.cookies.delete(OIDC_NONCE_COOKIE);
}

export async function readServerSession(): Promise<BitacoraSession | null> {
  const cookieStore = await cookies();
  const raw = cookieStore.get(SESSION_COOKIE)?.value;
  if (!raw) return null;

  const session = decodeSession(raw);
  if (!session || session.expiresAt <= Math.floor(Date.now() / 1000) + 30) {
    return null;
  }

  return session;
}

export function toPublicSession(session: BitacoraSession | null): PublicSession {
  if (!session) {
    return { user: null, expiresAt: null };
  }

  return {
    user: {
      id: session.subject,
      email: session.email,
      role: session.role,
    },
    expiresAt: session.expiresAt,
  };
}

export function createSessionFromAccessToken(
  accessToken: string,
  config: Pick<AuthRuntimeConfig, 'issuer' | 'clientId' | 'audience'>,
  idToken?: string,
  expectedNonce?: string,
): BitacoraSession {
  const payload = parseJwtPayload(accessToken);
  const identityPayload = idToken ? parseJwtPayload(idToken) : payload;
  const subject = readOptionalStringClaim(payload, 'sub') ?? readStringClaim(identityPayload, 'sub');
  const email = readOptionalStringClaim(payload, 'email') ?? readStringClaim(identityPayload, 'email');
  const issuer = readStringClaim(payload, 'iss');
  const expiresAt = readNumberClaim(payload, 'exp');

  if (issuer !== config.issuer) {
    throw new Error('Invalid Zitadel issuer.');
  }

  if (expiresAt <= Math.floor(Date.now() / 1000)) {
    throw new Error('Zitadel access token is expired.');
  }

  const audiences = readAudience(payload);
  if (!audiences.includes(config.audience) && !audiences.includes(config.clientId)) {
    throw new Error('Invalid Zitadel audience.');
  }

  if (expectedNonce && idToken && readOptionalStringClaim(identityPayload, 'nonce') !== expectedNonce) {
    throw new Error('Invalid Zitadel nonce.');
  }

  return {
    accessToken,
    subject,
    email,
    role: readRole(payload),
    expiresAt,
  };
}

export function parseJwtPayload(token: string): Record<string, unknown> {
  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new Error('Invalid JWT format.');
  }

  const payload = Buffer.from(parts[1]!, 'base64url').toString('utf8');
  return JSON.parse(payload) as Record<string, unknown>;
}

function encodeSession(session: BitacoraSession): string {
  return Buffer.from(JSON.stringify(session), 'utf8').toString('base64url');
}

function decodeSession(raw: string): BitacoraSession | null {
  try {
    const parsed = JSON.parse(Buffer.from(raw, 'base64url').toString('utf8')) as Partial<BitacoraSession>;
    if (
      typeof parsed.accessToken !== 'string' ||
      typeof parsed.subject !== 'string' ||
      typeof parsed.email !== 'string' ||
      typeof parsed.expiresAt !== 'number'
    ) {
      return null;
    }

    return {
      accessToken: parsed.accessToken,
      subject: parsed.subject,
      email: parsed.email,
      role: parsed.role === 'professional' ? 'professional' : 'patient',
      expiresAt: parsed.expiresAt,
    };
  } catch {
    return null;
  }
}

function readStringClaim(payload: Record<string, unknown>, name: string): string {
  const value = readOptionalStringClaim(payload, name);
  if (!value) {
    throw new Error(`Missing ${name} claim.`);
  }

  return value;
}

function readOptionalStringClaim(payload: Record<string, unknown>, name: string): string | null {
  const value = payload[name];
  if (typeof value !== 'string' || value.trim().length === 0) {
    return null;
  }

  return value;
}

function readNumberClaim(payload: Record<string, unknown>, name: string): number {
  const value = payload[name];
  if (typeof value !== 'number') {
    throw new Error(`Missing ${name} claim.`);
  }

  return value;
}

function readAudience(payload: Record<string, unknown>): string[] {
  const aud = payload.aud;
  if (typeof aud === 'string') return [aud];
  if (Array.isArray(aud)) return aud.filter((value): value is string => typeof value === 'string');
  return [];
}

function readRole(payload: Record<string, unknown>): BitacoraRole {
  const roles = payload[ZITADEL_ROLES_CLAIM];
  if (roles && typeof roles === 'object' && !Array.isArray(roles) && 'professional' in roles) {
    return 'professional';
  }

  if (Array.isArray(roles) && roles.includes('professional')) {
    return 'professional';
  }

  if (roles === 'professional') {
    return 'professional';
  }

  return 'patient';
}

function normalizeIssuer(value: string): string {
  const trimmed = value.trim().replace(/\/$/, '');
  if (trimmed.startsWith('https://') || trimmed.startsWith('http://')) {
    return trimmed;
  }

  return `https://${trimmed}`;
}
