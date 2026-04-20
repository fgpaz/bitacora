/**
 * Bitacora middleware - runtime hardening layer.
 *
 * It only performs UX routing checks from the product session cookie. Bitacora.Api
 * remains the authority for token signature, issuer, audience and data access.
 */

import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { SESSION_COOKIE, type BitacoraRole } from './lib/auth/constants';

interface MiddlewareSession {
  role: BitacoraRole;
  expiresAt: number;
}

const PATIENT_ROUTES = ['/registro', '/consent', '/configuracion', '/dashboard'];
const PROFESIONAL_ROUTES = ['/profesional'];
const PUBLIC_ROUTES = ['/', '/onboarding', '/ingresar', '/auth/callback', '/auth/logout'];

function isPublic(pathname: string): boolean {
  return PUBLIC_ROUTES.some((path) => pathname === path || pathname.startsWith(path + '/'));
}

function requiresAuth(pathname: string): 'patient' | 'professional' | 'closed' {
  if (PATIENT_ROUTES.some((path) => pathname.startsWith(path))) return 'patient';
  if (PROFESIONAL_ROUTES.some((path) => pathname.startsWith(path))) return 'professional';
  return 'closed';
}

function readSession(request: NextRequest): MiddlewareSession | null {
  const raw = request.cookies.get(SESSION_COOKIE)?.value;
  if (!raw) return null;

  try {
    const padded = raw.padEnd(raw.length + ((4 - (raw.length % 4)) % 4), '=');
    const decoded = atob(padded.replace(/-/g, '+').replace(/_/g, '/'));
    const parsed = JSON.parse(decoded) as Partial<MiddlewareSession>;

    if (typeof parsed.expiresAt !== 'number') return null;
    return {
      role: parsed.role === 'professional' ? 'professional' : 'patient',
      expiresAt: parsed.expiresAt,
    };
  } catch {
    return null;
  }
}

function isExpired(expiresAt: number): boolean {
  return Date.now() / 1000 > expiresAt - 30;
}

function redirectToRoot(request: NextRequest): NextResponse {
  const response = NextResponse.redirect(new URL('/', request.url));
  response.cookies.delete(SESSION_COOKIE);
  return response;
}

function withSecurityHeaders(response: NextResponse): NextResponse {
  response.headers.set('X-Frame-Options', 'DENY');
  response.headers.set('X-Content-Type-Options', 'nosniff');
  response.headers.set('Referrer-Policy', 'strict-origin-when-cross-origin');
  response.headers.set('Permissions-Policy', 'camera=(), microphone=(), geolocation=()');
  response.headers.set(
    'Content-Security-Policy',
    [
      "default-src 'self'",
      "script-src 'self' 'unsafe-inline'",
      "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com",
      "font-src 'self' https://fonts.gstatic.com",
      "img-src 'self' data: https://*",
      "connect-src 'self' https://*",
      "frame-ancestors 'none'",
    ].join('; '),
  );
  return response;
}

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const response = withSecurityHeaders(NextResponse.next());

  if (isPublic(pathname)) {
    return response;
  }

  const requiredGroup = requiresAuth(pathname);
  if (requiredGroup === 'closed') {
    return redirectToRoot(request);
  }

  const session = readSession(request);
  if (!session || isExpired(session.expiresAt)) {
    return redirectToRoot(request);
  }

  if (requiredGroup === 'professional' && session.role !== 'professional') {
    return redirectToRoot(request);
  }

  return response;
}

export const config = {
  matcher: [
    '/((?!_next/static|_next/image|api/|favicon.ico|robots.txt|manifest.json).*)',
  ],
};
