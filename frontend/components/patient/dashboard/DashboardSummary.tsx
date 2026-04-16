'use client';

/**
 * DashboardSummary — patient dashboard statistics cards.
 * Displays total entries, average mood score, and last entry timestamp.
 */

interface Props {
  totalEntries: number;
  avgMoodScore: number | null;
  lastEntryAt: string | null;
}

function formatDate(isoString: string | null): string {
  if (!isoString) return '—';
  try {
    const date = new Date(isoString);
    return date.toLocaleDateString('es-AR', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  } catch {
    return '—';
  }
}

function formatMoodScore(score: number | null): string {
  if (score === null || score === undefined) return '—';
  return score >= 0 ? `+${score.toFixed(1)}` : score.toFixed(1);
}

export function DashboardSummary({ totalEntries, avgMoodScore, lastEntryAt }: Props) {
  return (
    <div className="grid grid-cols-3 gap-4 mb-8">
      {/* Total entries card */}
      <div className="bg-surface-default border border-surface-muted rounded-lg p-6 text-center">
        <p className="text-sm font-medium text-foreground-muted mb-2">Registros totales</p>
        <p className="text-3xl font-bold text-foreground-default">{totalEntries}</p>
      </div>

      {/* Average mood score card */}
      <div className="bg-surface-default border border-surface-muted rounded-lg p-6 text-center">
        <p className="text-sm font-medium text-foreground-muted mb-2">Promedio de humor</p>
        <p className="text-3xl font-bold text-foreground-default">
          {formatMoodScore(avgMoodScore)}
        </p>
      </div>

      {/* Last entry date card */}
      <div className="bg-surface-default border border-surface-muted rounded-lg p-6 text-center">
        <p className="text-sm font-medium text-foreground-muted mb-2">Último registro</p>
        <p className="text-lg font-semibold text-foreground-default">{formatDate(lastEntryAt)}</p>
      </div>
    </div>
  );
}
