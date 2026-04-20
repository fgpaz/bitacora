/**
 * API client for Bitacora backend.
 * All calls use Authorization: Bearer <access_token> and receive typed error envelopes.
 */

const API_PROXY_BASE = '/api/backend';
const DEFAULT_REMINDER_TIMEZONE = 'America/Argentina/Buenos_Aires';

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
  const url = `${API_PROXY_BASE}${path}`;
  const res = await fetch(url, {
    ...init,
    credentials: init.credentials ?? 'same-origin',
    headers: {
      'Content-Type': 'application/json',
      ...(init.headers ?? {}),
    },
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

  const text = await res.text();
  if (!text) {
    return undefined as T;
  }

  return JSON.parse(text) as T;
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

/* ─── Telegram ───────────────────────────────────────────────────────────── */

export interface TelegramPairingResponse {
  code: string;         // BIT-XXXXX
  expires_at: string;   // ISO UTC timestamp
}

interface RawTelegramPairingResponse {
  code?: string;
  Code?: string;
  expiresAt?: string;
  expiresAtUtc?: string;
  ExpiresAt?: string;
  expires_at?: string;
}

export interface TelegramSessionResponse {
  linked: boolean;
  linked_at?: string;   // ISO UTC timestamp (present when linked=true)
  chat_id?: string;     // obfuscated, present when linked=true
  session_id?: string;
}

interface RawTelegramSessionResponse {
  linked?: boolean;
  isLinked?: boolean;
  linked_at?: string | null;
  linkedAtUtc?: string | null;
  chat_id?: string | null;
  chatId?: string | null;
  session_id?: string | null;
  sessionId?: string | null;
}

export async function generatePairingCode(): Promise<TelegramPairingResponse> {
  const raw = await bitacoraFetch<RawTelegramPairingResponse>('/telegram/pairing', {
    method: 'POST',
  });

  return {
    code: raw.code ?? raw.Code ?? '',
    expires_at: raw.expires_at ?? raw.expiresAt ?? raw.expiresAtUtc ?? raw.ExpiresAt ?? '',
  };
}

export async function getTelegramSession(): Promise<TelegramSessionResponse> {
  const raw = await bitacoraFetch<RawTelegramSessionResponse>('/telegram/session');

  return {
    linked: raw.linked ?? raw.isLinked ?? false,
    linked_at: raw.linked_at ?? raw.linkedAtUtc ?? undefined,
    chat_id: raw.chat_id ?? raw.chatId ?? undefined,
    session_id: raw.session_id ?? raw.sessionId ?? undefined,
  };
}

/* ─── Care Links (Vínculos profesional-paciente) ───────────────────────────── */

export interface CareLink {
  id: string;
  professional_name: string;
  professional_email?: string;
  status: 'active' | 'pending' | 'revoked';
  can_view_data: boolean;
  created_at: string;
}

export interface GetCareLinksResponse {
  links: CareLink[];
}

export async function getCareLinksByPatient(): Promise<GetCareLinksResponse> {
  return bitacoraFetch<GetCareLinksResponse>('/vinculos');
}

export async function getActiveCareLinks(): Promise<GetCareLinksResponse> {
  return bitacoraFetch<GetCareLinksResponse>('/vinculos/active');
}

export async function acceptBindingCode(bindingCode: string): Promise<{ id: string; status: string }> {
  return bitacoraFetch('/vinculos/accept', {
    method: 'POST',
    body: JSON.stringify({ bindingCode }),
  });
}

export async function revokeCareLink(id: string): Promise<void> {
  await bitacoraFetch(`/vinculos/${id}`, { method: 'DELETE' });
}

/* ─── Telegram: desvincular y configurar recordatorio ──────────────────────── */

interface RawUnlinkTelegramResponse {
  unlinked?: boolean;
  unlinkedAtUtc?: string;
  unlinked_at_utc?: string;
  patientId?: string;
}

export async function unlinkTelegram(): Promise<{ unlinked: boolean; unlinked_at_utc?: string }> {
  const raw = await bitacoraFetch<RawUnlinkTelegramResponse | undefined>('/telegram/session', { method: 'DELETE' });

  return {
    unlinked: raw?.unlinked ?? true,
    unlinked_at_utc: raw?.unlinked_at_utc ?? raw?.unlinkedAtUtc,
  };
}

export interface ReminderScheduleResponse {
  hour: number;
  minute: number;
  timezone: string;
  next_fire_at_utc: string;
}

interface RawReminderScheduleResponse {
  hour?: number;
  hourUtc?: number;
  minute?: number;
  minuteUtc?: number;
  timezone?: string;
  reminderTimezone?: string;
  next_fire_at_utc?: string;
  nextFireAtUtc?: string;
}

interface TimeZoneDateParts {
  year: number;
  month: number;
  day: number;
  hour: number;
  minute: number;
}

function validateReminderTime(hour: number, minute: number) {
  if (!Number.isInteger(hour) || hour < 0 || hour > 23) {
    throw new RangeError('Reminder hour must be an integer between 0 and 23.');
  }

  if (!Number.isInteger(minute) || (minute !== 0 && minute !== 30)) {
    throw new RangeError('Reminder minute must be 0 or 30.');
  }
}

function getTimeZoneDateParts(date: Date, timezone: string): TimeZoneDateParts {
  const formatter = new Intl.DateTimeFormat('en-US-u-ca-iso8601', {
    timeZone: timezone,
    hourCycle: 'h23',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  });

  const values = Object.fromEntries(
    formatter.formatToParts(date)
      .filter((part) => part.type !== 'literal')
      .map((part) => [part.type, Number(part.value)])
  );

  return {
    year: values.year,
    month: values.month,
    day: values.day,
    hour: values.hour,
    minute: values.minute,
  };
}

export function toUtcReminderSchedule(
  localHour: number,
  localMinute: number,
  timezone = DEFAULT_REMINDER_TIMEZONE,
  referenceDate = new Date()
): { hourUtc: number; minuteUtc: number } {
  validateReminderTime(localHour, localMinute);

  const timezoneDate = getTimeZoneDateParts(referenceDate, timezone);
  const desiredLocalMs = Date.UTC(
    timezoneDate.year,
    timezoneDate.month - 1,
    timezoneDate.day,
    localHour,
    localMinute
  );
  let utcMs = desiredLocalMs;

  for (let i = 0; i < 3; i++) {
    const rendered = getTimeZoneDateParts(new Date(utcMs), timezone);
    const renderedLocalMs = Date.UTC(
      rendered.year,
      rendered.month - 1,
      rendered.day,
      rendered.hour,
      rendered.minute
    );

    const deltaMs = desiredLocalMs - renderedLocalMs;
    if (deltaMs === 0) break;
    utcMs += deltaMs;
  }

  const utcDate = new Date(utcMs);

  return {
    hourUtc: utcDate.getUTCHours(),
    minuteUtc: utcDate.getUTCMinutes(),
  };
}

export async function setReminderSchedule(
  hour: number,
  minute: number,
  timezone: string
): Promise<ReminderScheduleResponse> {
  const { hourUtc, minuteUtc } = toUtcReminderSchedule(hour, minute, timezone);

  const raw = await bitacoraFetch<RawReminderScheduleResponse>('/telegram/reminder-schedule', {
    method: 'PUT',
    body: JSON.stringify({ hourUtc, minuteUtc, timezone }),
  });

  return {
    hour,
    minute,
    timezone: raw.timezone ?? raw.reminderTimezone ?? timezone,
    next_fire_at_utc: raw.next_fire_at_utc ?? raw.nextFireAtUtc ?? '',
  };
}

/* ─── Patient Dashboard ──────────────────────────────────────────────────────── */

export interface PatientTimelineEntry {
  date: string; // YYYY-MM-DD
  mood_score: number | null; // -3 a +3
  factors?: Record<string, unknown>;
}

export interface PatientTimelineResponse {
  entries: PatientTimelineEntry[];
}

export interface PatientSummaryResponse {
  total_entries: number;
  avg_mood_score: number | null;
  last_entry_at: string | null;
}

interface RawPatientTimelineDay {
  date: string;
  moodEntry?: {
    moodEntryId?: string;
    moodScore?: number | null;
    createdAtUtc?: string;
  } | null;
  dailyCheckin?: {
    dailyCheckinId?: string;
    checkinDate?: string;
    sleepHours?: number | null;
    physicalActivity?: boolean | null;
    socialActivity?: boolean | null;
    anxiety?: boolean | null;
    irritability?: boolean | null;
    medicationTaken?: boolean | null;
  } | null;
}

interface RawPatientTimelineResponse {
  entries?: PatientTimelineEntry[];
  days?: RawPatientTimelineDay[];
}

interface RawPatientSummaryResponse {
  total_entries?: number;
  totalEntries?: number;
  avg_mood_score?: number | null;
  avgMoodScore?: number | null;
  last_entry_at?: string | null;
  lastEntryAt?: string | null;
  daysWithMoodEntry?: number;
  daysWithCheckin?: number;
  averageMoodScore?: number | null;
}

export async function getPatientTimeline(from: Date, to: Date): Promise<PatientTimelineResponse> {
  const fromStr = from.toISOString().split('T')[0];
  const toStr = to.toISOString().split('T')[0];
  const raw = await bitacoraFetch<RawPatientTimelineResponse>(
    `/visualizacion/timeline?from=${fromStr}&to=${toStr}`
  );

  if (raw.entries) {
    return raw as PatientTimelineResponse;
  }

  return {
    entries: (raw.days ?? [])
      .filter((day) => day.moodEntry || day.dailyCheckin)
      .map((day) => ({
        date: day.date,
        mood_score: day.moodEntry?.moodScore ?? null,
        factors: day.dailyCheckin
          ? {
              sleep_hours: day.dailyCheckin.sleepHours,
              physical_activity: day.dailyCheckin.physicalActivity,
              social_activity: day.dailyCheckin.socialActivity,
              anxiety: day.dailyCheckin.anxiety,
              irritability: day.dailyCheckin.irritability,
              medication_taken: day.dailyCheckin.medicationTaken,
            }
          : undefined,
      })),
  };
}

export async function getPatientSummary(from: Date, to: Date): Promise<PatientSummaryResponse> {
  const fromStr = from.toISOString().split('T')[0];
  const toStr = to.toISOString().split('T')[0];
  const raw = await bitacoraFetch<RawPatientSummaryResponse>(
    `/visualizacion/summary?from=${fromStr}&to=${toStr}`
  );

  const totalEntries =
    raw.total_entries ??
    raw.totalEntries ??
    (raw.daysWithMoodEntry ?? 0) + (raw.daysWithCheckin ?? 0);

  return {
    total_entries: totalEntries,
    avg_mood_score: raw.avg_mood_score ?? raw.avgMoodScore ?? raw.averageMoodScore ?? null,
    last_entry_at: raw.last_entry_at ?? raw.lastEntryAt ?? null,
  };
}
