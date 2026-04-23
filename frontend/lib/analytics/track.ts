/**
 * Analytics — tracking client.
 *
 * Firma estable para instrumentar eventos del front. Llama a
 * POST /api/backend/analytics/events (proxy al backend /api/v1/analytics/events)
 * con fire-and-forget semantic: el error se swallow para no romper UX.
 *
 * PII policy: los `props` NO deben contener PII (email, nombre, contenido
 * clínico, trace ids externos). El backend valida longitud max (2048 chars)
 * pero NO inspecciona contenido — responsabilidad del caller.
 *
 * Eventos definidos (2026-04-23 login flow redesign):
 * - time_to_cta_ready: ms desde dashboard ready hasta openDialog.
 * - ctr_rail_vs_checkin: which CTA del actionRail se clickea.
 * - logout_accidental_rate: logout con <3 min de uso post shell mount.
 * - decline_consent_rate: decline vs accept en consent.
 */

export type AnalyticsEvent =
  | 'time_to_cta_ready'
  | 'ctr_rail_vs_checkin'
  | 'logout_accidental_rate'
  | 'decline_consent_rate';

export type AnalyticsPropValue = string | number | boolean | null | undefined;

export type AnalyticsProps = Record<string, AnalyticsPropValue>;

const ENDPOINT = '/api/backend/analytics/events';

export function track(event: AnalyticsEvent, props?: AnalyticsProps): void {
  if (typeof window === 'undefined') return;

  if (process.env.NODE_ENV !== 'production') {
    try {
      console.info('[analytics]', event, props ?? {});
    } catch {
      // swallow
    }
  }

  void sendEvent(event, props);
}

async function sendEvent(event: AnalyticsEvent, props?: AnalyticsProps): Promise<void> {
  try {
    const body = JSON.stringify({ event, props: props ?? {} });
    await fetch(ENDPOINT, {
      method: 'POST',
      credentials: 'same-origin',
      headers: { 'Content-Type': 'application/json' },
      body,
      keepalive: true,
    });
  } catch {
    // Fire-and-forget: analytics nunca debe romper UX.
  }
}
