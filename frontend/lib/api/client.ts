/**
 * API client for Bitacora backend.
 * All calls use Authorization: Bearer <access_token> and receive typed error envelopes.
 */

const API_BASE = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5000';
const API_VERSION = 'v1';

/* ─── Types ─────────────────────────────────────────────────────────────────── */

export interface ApiErrorEnvelope {
  error: {
    code: string;
    message: string;
    trace_id: string;
  };
}

export interface BootstrapResponse {
  userId: string;
  status: 'registered' | 'consent_granted' | 'active';
  needsConsent: boolean;
  resumePendingInvite: boolean;
}

export interface ConsentSection {
  id: string;
  title: string;
  content: string;
}

export interface ConsentCurrentResponse {
  version: string;
  text: string;
  sections: ConsentSection[];
  patientStatus: 'none' | 'pending' | 'granted' | 'revoked';
}

export interface ConsentGrantResponse {
  consentGrantId: string;
  status: 'consent_granted';
  grantedAtUtc: string;
  needsFirstEntry: boolean;
  resumePendingInvite: boolean;
}

export interface MoodEntryRequest {
  score: number; // -3 .. +3
}

export interface MoodEntryResponse {
  mood_entry_id: string;
  safe_projection: {
    mood_score: number;
    channel: 'api';
    created_at: string;
  };
}

export interface DailyCheckinRequest {
  sleep_hours: number;
  physical_activity: boolean;
  social_activity: boolean;
  anxiety: boolean;
  irritability: boolean;
  medication_taken: boolean;
  medication_time?: string; // HH:MM — required when medication_taken = true
}

export interface DailyCheckinResponse {
  daily_checkin_id: string;
  safe_projection: {
    checkin_date: string;
    channel: 'api';
  };
  checkin_date: string;
}

/* ─── Core fetch ─────────────────────────────────────────────────────────────── */

export async function bitacoraFetch<T>(
  path: string,
  init: RequestInit = {},
): Promise<T> {
  const url = `${API_BASE}/api/${API_VERSION}${path}`;
  const res = await fetch(url, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(init.headers ?? {}),
    },
    credentials: 'include',
  });

  if (!res.ok) {
    let envelope: ApiErrorEnvelope | null = null;
    try {
      envelope = await res.json();
    } catch {
      // not JSON
    }
    const code = envelope?.error?.code ?? 'UNKNOWN';
    const message = envelope?.error?.message ?? res.statusText;
    const traceId = envelope?.error?.trace_id;
    const err = new Error(message);
    (err as unknown as { code: string; trace_id?: string }).code = code;
    if (traceId) (err as unknown as { trace_id: string }).trace_id = traceId;
    throw err;
  }

  return res.json() as Promise<T>;
}

/* ─── Auth / Bootstrap ──────────────────────────────────────────────────────── */

export async function bootstrapPatient(inviteToken?: string): Promise<BootstrapResponse> {
  const qs = inviteToken ? `?invite_token=${encodeURIComponent(inviteToken)}` : '';
  return bitacoraFetch<BootstrapResponse>(`/auth/bootstrap${qs}`, {
    method: 'POST',
  });
}

/* ─── Consent ────────────────────────────────────────────────────────────────── */

export async function getCurrentConsent(): Promise<ConsentCurrentResponse> {
  return bitacoraFetch<ConsentCurrentResponse>('/consent/current');
}

export async function grantConsent(version: string): Promise<ConsentGrantResponse> {
  return bitacoraFetch<ConsentGrantResponse>('/consent', {
    method: 'POST',
    body: JSON.stringify({ version, accepted: true }),
  });
}

/* ─── Mood Entries ─────────────────────────────────────────────────────────── */

export async function createMoodEntry(score: number): Promise<MoodEntryResponse> {
  return bitacoraFetch<MoodEntryResponse>('/mood-entries', {
    method: 'POST',
    body: JSON.stringify({ score }),
  });
}

/* ─── Daily Check-ins ─────────────────────────────────────────────────────── */

export async function upsertDailyCheckin(
  data: DailyCheckinRequest,
): Promise<DailyCheckinResponse> {
  return bitacoraFetch<DailyCheckinResponse>('/daily-checkins', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
