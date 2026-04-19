import { NextRequest, NextResponse } from 'next/server';
import { clearAuthCookies, getAuthRuntimeConfig } from '@/lib/auth/server';

export const runtime = 'nodejs';

export async function GET(request: NextRequest) {
  const origin = request.nextUrl.origin;
  const config = getAuthRuntimeConfig(origin);
  const endSessionUrl = new URL('/oidc/v1/end_session', config.issuer);
  endSessionUrl.searchParams.set('post_logout_redirect_uri', config.postLogoutRedirectUri);

  const response = NextResponse.redirect(endSessionUrl);
  clearAuthCookies(response);
  return response;
}
