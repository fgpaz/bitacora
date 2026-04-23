/**
 * Analytics — tracking stub.
 *
 * Firma estable para instrumentar eventos del front sin dependencia
 * de vendor externo ni endpoint backend.
 *
 * Por ahora: console.info('[analytics]', event, props).
 * TODO: reemplazar con fetch('/api/analytics', { method: 'POST', body: ... })
 * o navigator.sendBeacon cuando el endpoint backend este disponible.
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

export function track(event: AnalyticsEvent, props?: AnalyticsProps): void {
  if (typeof window === 'undefined') return;
  try {
    console.info('[analytics]', event, props ?? {});
    // TODO: cuando el endpoint backend este disponible:
    // navigator.sendBeacon('/api/analytics', JSON.stringify({ event, props, ts: Date.now() }));
  } catch {
    // Swallow - analytics nunca debe romper UX.
  }
}
