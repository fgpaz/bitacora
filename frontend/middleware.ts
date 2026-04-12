/**
 * Bitacora middleware — runtime hardening layer.
 *
 * Hardening applied:
 * 1. Fail-closed redirects for unauthenticated access to protected groups.
 * 2. Session / JWT expiry guard: expired token → redirect to /.
 * 3. Security headers on every response.
 *
 * Route groups:
 *   /                — public (landing + magic-link initiation)
 *   /onboarding      — public (post-magic-link flow)
 *   /(patient)/*     — requires authenticated patient session
 *   /(profesional)/* — requires authenticated professional session
 *   all other paths  — closed (404-like redirect to /)
 */

import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

// ── Helpers ───────────────────────────────────────────────────────────────────

/**
 * Decode a Supabase JWT (JWT.Part2) without verification.
 * We only check the `exp` claim to detect expiry.
 * The actual verification happens server-side in Bitacora.Api.
 */
function decodeJwtExpiry(token: string): number | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
    return typeof payload.exp === 'number' ? payload.exp : null;
  } catch {
    return null;
  }
}

function decodeJwtRole(token: string): string | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
    return typeof payload.user_metadata?.role === 'string' ? payload.user_metadata.role : null;
  } catch {
    return null;
  }
}

function isExpired(exp: number): boolean {
  // Add 30-second grace period — treat as expired slightly before actual TTL
  return Date.now() / 1000 > exp - 30;
}

function redirectToRoot(request: NextRequest): NextResponse {
  return NextResponse.redirect(new URL('/', request.url));
}

// ── Protected route patterns ───────────────────────────────────────────────────

const PATIENT_ROUTES = ['/registro', '/consent'];
const PROFESIONAL_ROUTES = ['/profesional'];
const PUBLIC_ROUTES = ['/', '/onboarding'];

function isPublic(pathname: string): boolean {
  return PUBLIC_ROUTES.some((p) => pathname === p || pathname.startsWith(p + '/'));
}

function requiresAuth(pathname: string): 'patient' | 'professional' | 'closed' {
  for (const p of PATIENT_ROUTES) {
    if (pathname.startsWith(p)) return 'patient';
  }
  for (const p of PROFESIONAL_ROUTES) {
    if (pathname.startsWith(p)) return 'professional';
  }
  // Unknown routes: fail-closed
  return 'closed';
}

// ── Middleware ─────────────────────────────────────────────────────────────────

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // ── 1. Security headers ────────────────────────────────────────────────────
  const response = NextResponse.next();
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
    ].join('; ')
  );

  // ── 2. Public routes: pass through ───────────────────────────────────────
  if (isPublic(pathname)) {
    return response;
  }

  // ── 3. Route group that requires auth ────────────────────────────────────
  const requiredGroup = requiresAuth(pathname);
  if (requiredGroup === 'closed') {
    // Fail-closed: redirect unknown routes to home
    return redirectToRoot(request);
  }

  // ── 4. Auth check ──────────────────────────────────────────────────────────
  const supabaseToken = request.cookies.get('sb-access-token')?.value
    ?? request.cookies.get('sb-supabase-session')?.value; // fallback for newer Supabase versions

  if (!supabaseToken) {
    return redirectToRoot(request);
  }

  const exp = decodeJwtExpiry(supabaseToken);
  if (exp === null || isExpired(exp)) {
    // Clear session cookies so the client-side SessionContext also resets
    const clearResp = redirectToRoot(request);
    clearResp.cookies.delete('sb-access-token');
    clearResp.cookies.delete('sb-supabase-session');
    return clearResp;
  }

  // ── 5. Group-specific guard ─────────────────────────────────────────────────
  if (requiredGroup === 'professional') {
    const role = decodeJwtRole(supabaseToken);
    if (role !== 'professional') {
      return redirectToRoot(request);
    }
  }

  return response;
}

export const config = {
  matcher: [
    /*
     * Match all request paths EXCEPT:
     * - _next/static (static files)
     * - _next/image (image optimization)
     * - api/ (API routes — handled by Bitacora.Api directly)
     * - favicon.ico, robots.txt, manifest.json
     */
    '/((?!_next/static|_next/image|api/|favicon.ico|robots.txt|manifest.json).*)',
  ],
};
