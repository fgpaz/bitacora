/**
 * API client for Bitacora professional-facing endpoints.
 * All calls use Authorization: Bearer <access_token>.
 */
import { bitacoraFetch } from './client';
import { sha256Hex } from '../sha256';

export interface ProfessionalPatient {
  id: string;
  email: string;
  display_name: string;
  created_at: string;
  status: 'active' | 'pending' | 'inactive';
  hasRecentAlert?: boolean;
}

export interface ProfessionalPatientListResponse {
  patients: ProfessionalPatient[];
  total?: number;
  hasMore?: boolean;
}

export interface InviteRequest {
  email: string;
}

export interface InviteResponse {
  invite_id: string;
  email: string;
  status: 'pending' | 'sent';
  created_at: string;
}

export interface PatientSummary {
  patient_id: string;
  display_name: string;
  email: string;
  created_at: string;
  total_entries: number;
  avg_mood_score: number | null;
  last_entry_at: string | null;
}

export interface TimelineEntry {
  id: string;
  entry_type: 'mood_entry' | 'daily_checkin';
  created_at: string;
  data: Record<string, unknown>;
}

export interface TimelineResponse {
  entries: TimelineEntry[];
  total: number;
  page: number;
  page_size: number;
}

export interface Alert {
  id: string;
  rule_id: string;
  patient_id: string;
  type: string;
  severity: 'low' | 'medium' | 'high';
  message: string;
  acknowledged: boolean;
  created_at: string;
}

export interface AlertsResponse {
  alerts: Alert[];
  total: number;
}

export interface ExportConstraint {
  export_type: 'mood_entries' | 'daily_checkins' | 'full';
  allowed: boolean;
  reason?: string;
}

/* ─── Patients ─────────────────────────────────────────────────────────── */

export async function getProfessionalPatients(): Promise<ProfessionalPatientListResponse> {
  return bitacoraFetch<ProfessionalPatientListResponse>('/professional/patients');
}

export async function getProfessionalPatientsPaginated(
  page: number,
  pageSize: number,
): Promise<ProfessionalPatientListResponse> {
  return bitacoraFetch<ProfessionalPatientListResponse>(
    `/professional/patients?page=${page}&pageSize=${pageSize}`,
  );
}

/* ─── Invites ──────────────────────────────────────────────────────────── */

export async function createProfessionalInvite(
  payload: InviteRequest,
): Promise<InviteResponse> {
  const emailHash = await sha256Hex(payload.email);
  return bitacoraFetch<InviteResponse>('/professional/invites', {
    method: 'POST',
    body: JSON.stringify({ EmailHash: emailHash }),
  });
}

/* ─── Patient Detail ──────────────────────────────────────────────────── */

export async function getPatientSummary(patientId: string): Promise<PatientSummary> {
  return bitacoraFetch<PatientSummary>(`/professional/patients/${patientId}/summary`);
}

export async function getPatientTimeline(
  patientId: string,
  page = 1,
  pageSize = 20,
): Promise<TimelineResponse> {
  return bitacoraFetch<TimelineResponse>(
    `/professional/patients/${patientId}/timeline?page=${page}&page_size=${pageSize}`,
  );
}

function toDateOnly(d: Date): string {
  return d.toISOString().split('T')[0]!;
}

export async function getPatientTimelineByPeriod(
  patientId: string,
  from: Date,
  to: Date,
): Promise<TimelineResponse> {
  const fromStr = toDateOnly(from);
  const toStr = toDateOnly(to);
  return bitacoraFetch<TimelineResponse>(
    `/professional/patients/${patientId}/timeline?from=${fromStr}&to=${toStr}`,
  );
}

export async function getPatientAlerts(patientId: string): Promise<AlertsResponse> {
  return bitacoraFetch<AlertsResponse>(`/professional/patients/${patientId}/alerts`);
}

/* ─── Export ───────────────────────────────────────────────────────────── */

export async function getExportConstraints(patientId: string): Promise<ExportConstraint> {
  // Patient-owner-only endpoint; for professionals this will return allowed:false
  // with a reason explaining the limitation.
  return bitacoraFetch<ExportConstraint>(`/export/${patientId}/constraints`);
}

export type PeriodPreset = '7d' | '30d' | '90d' | 'custom';

export interface PeriodSelection {
  preset: PeriodPreset;
  from: string; // ISO date string yyyy-MM-dd
  to: string;   // ISO date string yyyy-MM-dd
}

export async function downloadExportCsv(
  _patientId: string,
  from: string,
  to: string,
): Promise<void> {
  const url = `/api/backend/export/patient-summary/csv?from=${from}&to=${to}`;

  const response = await fetch(url, {
    method: 'GET',
    headers: { 'Content-Type': 'application/json' },
  });

  if (!response.ok) {
    let message = `HTTP ${response.status}`;
    try {
      const json = await response.json();
      message = json?.error?.message ?? message;
    } catch { /* ignore */ }
    throw new Error(message);
  }

  const blob = await response.blob();
  const objectUrl = URL.createObjectURL(blob);
  const anchor = document.createElement('a');
  anchor.href = objectUrl;
  anchor.download = `bitacora-export-${from}-${to}.csv`;
  document.body.appendChild(anchor);
  anchor.click();
  document.body.removeChild(anchor);
  URL.revokeObjectURL(objectUrl);
}
