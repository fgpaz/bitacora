'use client';

/**
 * Dashboard — patient self-view of mood history and daily statistics.
 * States: loading | ready | error | empty
 * Fetches timeline and summary in parallel (async-parallel).
 * Handles 403 Consent Required by redirecting to /consent.
 */

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { getPatientTimeline, getPatientSummary } from '@/lib/api/client';
import type { PatientTimelineResponse, PatientSummaryResponse } from '@/lib/api/client';
import { DashboardSummary } from './DashboardSummary';

type ViewState = 'loading' | 'ready' | 'error' | 'empty';

interface EntryData {
  date: string;
  moodScore: number | null;
}

export function Dashboard() {
  const router = useRouter();
  const [viewState, setViewState] = useState<ViewState>('loading');
  const [entries, setEntries] = useState<EntryData[]>([]);
  const [totalEntries, setTotalEntries] = useState(0);
  const [avgMoodScore, setAvgMoodScore] = useState<number | null>(null);
  const [lastEntryAt, setLastEntryAt] = useState<string | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  useEffect(() => {
    async function loadData() {
      try {
        // Calculate date range: 30 days ago to today
        const to = new Date();
        const from = new Date();
        from.setDate(from.getDate() - 30);

        // Fetch timeline and summary in parallel
        const [timelineRes, summaryRes] = await Promise.all([
          getPatientTimeline(from, to),
          getPatientSummary(from, to),
        ]);

        // Populate summary stats
        setTotalEntries(summaryRes.total_entries);
        setAvgMoodScore(summaryRes.avg_mood_score);
        setLastEntryAt(summaryRes.last_entry_at);

        // Process timeline entries (take last 10, reverse for newest first)
        const sortedEntries = timelineRes.entries
          .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
          .slice(0, 10)
          .map((e) => ({
            date: e.date,
            moodScore: e.mood_score,
          }));

        setEntries(sortedEntries);
        setViewState(summaryRes.total_entries === 0 ? 'empty' : 'ready');
      } catch (err: unknown) {
        const code = (err as { code?: string }).code;
        // 403 Forbidden or consent-specific error: redirect to consent flow
        if (code === 'CONSENT_REQUIRED' || code === 'FORBIDDEN' || code === '403') {
          router.push('/consent');
          return;
        }
        // Other errors: show error state
        const message = (err as { message?: string }).message ?? 'Error desconocido';
        setErrorMsg(message);
        setViewState('error');
      }
    }

    loadData();
  }, [router]);

  if (viewState === 'loading') {
    return (
      <div className="space-y-6">
        {/* Skeleton: summary cards */}
        <div className="grid grid-cols-3 gap-4">
          {Array.from({ length: 3 }).map((_, i) => (
            <div
              key={i}
              className="bg-surface-muted rounded-lg p-6 h-24 animate-pulse"
              aria-hidden="true"
            />
          ))}
        </div>

        {/* Skeleton: entry list */}
        <div className="space-y-3">
          {Array.from({ length: 4 }).map((_, i) => (
            <div
              key={i}
              className="bg-surface-muted rounded-lg p-4 h-16 animate-pulse"
              aria-hidden="true"
            />
          ))}
        </div>
      </div>
    );
  }

  if (viewState === 'error') {
    return (
      <div
        className="bg-surface-default border border-status-error rounded-lg p-6 text-center"
        role="alert"
      >
        <p className="text-foreground-default font-medium mb-2">Error al cargar el historial</p>
        <small className="text-foreground-muted block mb-4">{errorMsg}</small>
        <button
          onClick={() => window.location.reload()}
          className="px-4 py-2 bg-brand-primary text-white rounded-md hover:opacity-90"
        >
          Reintentar
        </button>
      </div>
    );
  }

  if (viewState === 'empty') {
    return (
      <div className="space-y-6">
        <DashboardSummary
          totalEntries={totalEntries}
          avgMoodScore={avgMoodScore}
          lastEntryAt={lastEntryAt}
        />
        <div
          className="bg-surface-default border border-surface-muted rounded-lg p-12 text-center"
          role="status"
        >
          <svg
            width={48}
            height={48}
            viewBox="0 0 24 24"
            fill="none"
            className="mx-auto mb-4 opacity-50"
            aria-hidden="true"
          >
            <path
              d="M3 12h18M3 6h18M3 18h18"
              stroke="currentColor"
              strokeWidth={1.5}
              strokeLinecap="round"
            />
          </svg>
          <p className="text-foreground-default font-medium mb-2">Aún no tienes registros</p>
          <p className="text-foreground-muted text-sm mb-6">
            Comienza a registrar tu humor para ver tu historial aquí.
          </p>
          <Link
            href="/registro/mood-entry"
            className="inline-block px-6 py-2 bg-brand-primary text-white rounded-md hover:opacity-90"
          >
            Registrar ahora
          </Link>
        </div>
      </div>
    );
  }

  // viewState === 'ready'
  return (
    <div className="space-y-6">
      {/* Summary statistics */}
      <DashboardSummary
        totalEntries={totalEntries}
        avgMoodScore={avgMoodScore}
        lastEntryAt={lastEntryAt}
      />

      {/* Recent entries section */}
      <section aria-labelledby="recent-entries-heading">
        <h2
          id="recent-entries-heading"
          className="text-lg font-semibold text-foreground-default mb-4"
        >
          Registros recientes
        </h2>

        <div className="space-y-2">
          {entries.map((entry) => {
            const emoji = entry.moodScore === null
              ? '📝'
              : entry.moodScore <= -2
              ? '😢'
              : entry.moodScore === -1
              ? '😕'
              : entry.moodScore === 0
              ? '😐'
              : entry.moodScore === 1
              ? '🙂'
              : entry.moodScore === 2
              ? '😊'
              : '😁';

            const dateObj = new Date(entry.date);
            const dateStr = dateObj.toLocaleDateString('es-AR', {
              year: 'numeric',
              month: 'short',
              day: 'numeric',
            });

            return (
              <div
                key={entry.date}
                className="bg-surface-default border border-surface-muted rounded-lg p-4 flex items-center justify-between hover:bg-surface-muted transition-colors"
              >
                <div className="flex-1">
                  <p className="text-foreground-default font-medium">{dateStr}</p>
                </div>
                <div className="text-2xl">{emoji}</div>
              </div>
            );
          })}
        </div>
      </section>

      {/* Quick action buttons */}
      <section className="grid grid-cols-2 gap-3 pt-4 border-t border-surface-muted">
        <Link
          href="/registro/mood-entry"
          className="block px-4 py-3 bg-brand-primary text-white text-center rounded-md hover:opacity-90 font-medium text-sm"
        >
          Registrar humor
        </Link>
        <Link
          href="/registro/daily-checkin"
          className="block px-4 py-3 bg-brand-secondary text-white text-center rounded-md hover:opacity-90 font-medium text-sm"
        >
          Check-in diario
        </Link>
      </section>
    </div>
  );
}
