import { NextRequest, NextResponse } from 'next/server';
import { getBackendBaseUrl, readServerSession } from '@/lib/auth/server';

export const runtime = 'nodejs';

const BODY_METHODS = new Set(['POST', 'PUT', 'PATCH', 'DELETE']);

export async function GET(request: NextRequest) {
  return proxyToBackend(request);
}

export async function POST(request: NextRequest) {
  return proxyToBackend(request);
}

export async function PUT(request: NextRequest) {
  return proxyToBackend(request);
}

export async function PATCH(request: NextRequest) {
  return proxyToBackend(request);
}

export async function DELETE(request: NextRequest) {
  return proxyToBackend(request);
}

async function proxyToBackend(request: NextRequest): Promise<NextResponse> {
  const session = await readServerSession();
  if (!session) {
    return NextResponse.json(
      { error: { code: 'UNAUTHENTICATED', message: 'Sesión requerida.', trace_id: '' } },
      { status: 401 },
    );
  }

  const upstreamUrl = buildUpstreamUrl(request);
  const headers = new Headers();
  const contentType = request.headers.get('content-type');
  const accept = request.headers.get('accept');
  const correlationId = request.headers.get('x-correlation-id');

  if (contentType) headers.set('Content-Type', contentType);
  if (accept) headers.set('Accept', accept);
  if (correlationId) headers.set('X-Correlation-ID', correlationId);
  headers.set('Authorization', `Bearer ${session.accessToken}`);

  const upstreamResponse = await fetch(upstreamUrl, {
    method: request.method,
    headers,
    body: BODY_METHODS.has(request.method) ? await request.arrayBuffer() : undefined,
    cache: 'no-store',
  });

  return new NextResponse(upstreamResponse.body, {
    status: upstreamResponse.status,
    statusText: upstreamResponse.statusText,
    headers: filterResponseHeaders(upstreamResponse.headers),
  });
}

function buildUpstreamUrl(request: NextRequest): URL {
  const localUrl = new URL(request.url);
  const backendPath = localUrl.pathname.replace(/^\/api\/backend/, '');
  const upstream = new URL(`/api/v1${backendPath}${localUrl.search}`, getBackendBaseUrl());
  return upstream;
}

function filterResponseHeaders(headers: Headers): Headers {
  const forwarded = new Headers();
  for (const [key, value] of headers) {
    const normalized = key.toLowerCase();
    if (normalized === 'content-type' || normalized === 'content-disposition' || normalized === 'x-trace-id') {
      forwarded.set(key, value);
    }
  }

  forwarded.set('Cache-Control', 'no-store');
  return forwarded;
}
