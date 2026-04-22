'use client';

/**
 * Dashboard — patient self-view of mood history and daily statistics.
 * States: loading | ready | error | empty
 * Fetches timeline and summary in parallel (async-parallel).
 * Handles 403 Consent Required by redirecting to /consent.
 * Opens MoodEntryDialog inline so the user never loses the history context.
 */

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import type { CSSProperties } from 'react';
import { useEffect, useMemo, useRef, useState } from 'react';
import { getPatientTimeline, getPatientSummary } from '@/lib/api/client';
import { DashboardSummary } from './DashboardSummary';
import { MoodEntryDialog } from './MoodEntryDialog';
import { TelegramReminderBanner } from './TelegramReminderBanner';
import styles from './Dashboard.module.css';

type ViewState = 'loading' | 'ready' | 'error' | 'empty';

interface EntryData {
  date: string;
  moodScore: number | null;
}

const SUMMARY_SKELETON_KEYS = ['summary-total', 'summary-average', 'summary-last'];
const ENTRY_SKELETON_KEYS = ['entry-1', 'entry-2', 'entry-3', 'entry-4'];
const MAX_ABS_SCORE = 3;

function formatEntryDate(date: string, options: Intl.DateTimeFormatOptions): string {
  return new Date(`${date}T00:00:00`).toLocaleDateString('es-AR', options);
}

function formatMoodScore(score: number | null): string {
  if (score === null) return 'sin puntaje';
  if (score > 0) return `+${score}`;
  return score.toString();
}

function getTrendBarClass(score: number | null): string {
  if (score === null) return `${styles.trendBar} ${styles.trendBarMissing}`;
  if (score > 0) return `${styles.trendBar} ${styles.trendBarPositive}`;
  if (score < 0) return `${styles.trendBar} ${styles.trendBarNegative}`;
  return `${styles.trendBar} ${styles.trendBarNeutral}`;
}

function getTrendBarStyle(score: number | null): CSSProperties {
  const magnitude = score === null ? 0 : Math.abs(score);
  const barSize = score === null ? 6 : Math.max(6, Math.round((magnitude / MAX_ABS_SCORE) * 58));

  return {
    '--bar-size': `${barSize}px`,
  } as CSSProperties;
}

export function Dashboard() {
  const router = useRouter();
  const [viewState, setViewState] = useState<ViewState>('loading');
  const [entries, setEntries] = useState<EntryData[]>([]);
  const [totalEntries, setTotalEntries] = useState(0);
  const [avgMoodScore, setAvgMoodScore] = useState<number | null>(null);
  const [lastEntryAt, setLastEntryAt] = useState<string | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [refreshNonce, setRefreshNonce] = useState(0);
  const openDialogRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    let cancelled = false;
    async function loadData() {
      try {
        const to = new Date();
        const from = new Date();
        from.setDate(from.getDate() - 30);

        const [timelineRes, summaryRes] = await Promise.all([
          getPatientTimeline(from, to),
          getPatientSummary(from, to),
        ]);
        if (cancelled) return;

        const sortedEntries = timelineRes.entries
          .toSorted((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
          .slice(0, 10)
          .map((e) => ({
            date: e.date,
            moodScore: e.mood_score,
          }));

        setTotalEntries(summaryRes.total_entries);
        setAvgMoodScore(summaryRes.avg_mood_score);
        setEntries(sortedEntries);
        setLastEntryAt(summaryRes.last_entry_at ?? sortedEntries[0]?.date ?? null);
        setViewState(summaryRes.total_entries === 0 ? 'empty' : 'ready');
      } catch (err: unknown) {
        if (cancelled) return;
        const code = (err as { code?: string }).code;
        if (code === 'CONSENT_REQUIRED' || code === 'FORBIDDEN' || code === '403') {
          router.push('/consent');
          return;
        }
        setErrorMsg('No se pudo cargar el historial.');
        setViewState('error');
      }
    }
    loadData();
    return () => {
      cancelled = true;
    };
  }, [router, refreshNonce]);

  function openDialog() {
    setDialogOpen(true);
  }

  function closeDialog() {
    setDialogOpen(false);
    requestAnimationFrame(() => openDialogRef.current?.focus());
  }

  function handleEntrySaved() {
    setRefreshNonce((n) => n + 1);
  }

  const trendEntries = useMemo(() => entries.toReversed(), [entries]);
  const trendCount = Math.max(1, trendEntries.length);

  if (viewState === 'loading') {
    return (
      <div className={styles.stack} aria-busy="true" aria-label="Cargando historial">
        <div className={styles.summarySkeletonGrid}>
          {SUMMARY_SKELETON_KEYS.map((key) => (
            <div
              key={key}
              className={styles.summarySkeleton}
              aria-hidden="true"
            />
          ))}
        </div>

        <div className={styles.entrySkeletonList}>
          {ENTRY_SKELETON_KEYS.map((key) => (
            <div
              key={key}
              className={styles.entrySkeleton}
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
        className={styles.errorState}
        role="alert"
      >
        <p className={styles.errorTitle}>Error al cargar el historial</p>
        <small className={styles.errorText}>{errorMsg}</small>
        <button
          onClick={() => window.location.reload()}
          className={styles.primaryButton}
        >
          Reintentar
        </button>
      </div>
    );
  }

  if (viewState === 'empty') {
    return (
      <>
        <div className={styles.stack}>
          <TelegramReminderBanner />
          {/* DashboardSummary removido del empty state — canon 10 anti-señal "tablero de vigilancia" aplicada a primer uso */}
          <div
            className={styles.emptyState}
            role="region"
            aria-label="Historial vacío"
          >
            <svg
              width={48}
              height={48}
              viewBox="0 0 24 24"
              fill="none"
              className={styles.emptyIcon}
              aria-hidden="true"
            >
              <path
                d="M3 12h18M3 6h18M3 18h18"
                stroke="currentColor"
                strokeWidth={1.5}
                strokeLinecap="round"
              />
            </svg>
            <p className={styles.emptyTitle}>Empezá con tu primer registro</p>
            <p className={styles.emptyText}>
              Cuando cargues tu primer registro, lo vas a ver acá.
            </p>
            <button
              ref={openDialogRef}
              type="button"
              onClick={openDialog}
              className={styles.primaryButton}
            >
              Registrar humor
            </button>
          </div>
        </div>
        <MoodEntryDialog open={dialogOpen} onClose={closeDialog} onSaved={handleEntrySaved} />
      </>
    );
  }

  return (
    <>
      <div className={styles.stack}>
        <TelegramReminderBanner />
        <DashboardSummary
          totalEntries={totalEntries}
          avgMoodScore={avgMoodScore}
          lastEntryAt={lastEntryAt}
        />

        <section className={styles.trendPanel} aria-labelledby="trend-heading">
          <div className={styles.trendHeader}>
            <h2 id="trend-heading" className={styles.sectionTitle}>
              Tus últimos días
            </h2>
            <span className={styles.trendCaption}>Resumen visual</span>
          </div>

          <div
            className={styles.trendChart}
            role="list"
            aria-label="Estado de ánimo de los últimos días"
            style={{ '--trend-count': trendCount } as CSSProperties}
          >
            <div className={styles.trendMidline} aria-hidden="true" />
            {trendEntries.map((entry) => {
              const dayLabel = formatEntryDate(entry.date, { day: '2-digit', month: 'short' });
              const ariaLabel = `${dayLabel}: ${formatMoodScore(entry.moodScore)}`;

              return (
                <div key={entry.date} className={styles.trendColumn} role="listitem">
                  <div className={styles.trendTrack} aria-label={ariaLabel} title={ariaLabel}>
                    <span
                      className={getTrendBarClass(entry.moodScore)}
                      style={getTrendBarStyle(entry.moodScore)}
                      aria-hidden="true"
                    />
                  </div>
                  <span className={styles.trendDay}>{dayLabel}</span>
                </div>
              );
            })}
          </div>
        </section>

        <section aria-labelledby="recent-entries-heading">
          <h2
            id="recent-entries-heading"
            className={styles.sectionTitle}
          >
            Registros recientes
          </h2>

          <div className={styles.entryList}>
            {entries.map((entry) => {
              const dateStr = formatEntryDate(entry.date, {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
              });

              return (
                <div
                  key={entry.date}
                  className={styles.entryItem}
                >
                  <div className={styles.entryInfo}>
                    <p className={styles.entryDate}>{dateStr}</p>
                  </div>
                  <div className={styles.scoreBadge} aria-label="Estado de ánimo">
                    {entry.moodScore === null ? 'Sin registro' : formatMoodScore(entry.moodScore)}
                  </div>
                </div>
              );
            })}
          </div>
        </section>

        <section className={styles.actions} aria-label="Acciones de registro">
          <button
            ref={openDialogRef}
            type="button"
            onClick={openDialog}
            className={styles.primaryButton}
          >
            + Nuevo registro
          </button>
          <Link
            href="/registro/daily-checkin"
            className={styles.secondaryLink}
          >
            Check-in diario
          </Link>
        </section>
      </div>
      <MoodEntryDialog open={dialogOpen} onClose={closeDialog} onSaved={handleEntrySaved} />
    </>
  );
}
