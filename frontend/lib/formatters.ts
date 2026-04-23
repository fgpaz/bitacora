/**
 * Shared number and date formatters for patient-facing views.
 *
 * formatMoodScore is canonical: accepts `null | undefined` with configurable
 * fallback (default `'—'`) and optional decimal places. Replaces two
 * divergent local implementations in Dashboard/DashboardSummary.
 */

export interface FormatMoodScoreOptions {
  decimals?: number;
  fallback?: string;
}

export function formatMoodScore(
  score: number | null | undefined,
  options: FormatMoodScoreOptions = {},
): string {
  const { decimals = 0, fallback = '—' } = options;
  if (score === null || score === undefined) return fallback;
  const formatted = decimals > 0 ? score.toFixed(decimals) : score.toString();
  return score > 0 ? `+${formatted}` : formatted;
}
