import { NextResponse } from 'next/server';
import { readServerSession, toPublicSession } from '@/lib/auth/server';

export const runtime = 'nodejs';

export async function GET() {
  const session = await readServerSession();
  return NextResponse.json(toPublicSession(session), {
    headers: { 'Cache-Control': 'no-store' },
  });
}
