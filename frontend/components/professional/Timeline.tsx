'use client';

/**
 * Timeline — chronological feed of patient entries.
 * Supports period filtering with longitudinal chart visualization.
 * Follows UI-RFC-VIS-001 state taxonomy.
 */
import { useCallback, useEffect, useState } from 'react';
import { getPatientTimelineByPeriod } from '@/lib/api/professional';
import type { TimelineEntry } from '@/lib/api/professional';
import styles from './Timeline.module.css';

interface Props {
  patientId: string;
}

/* ─── Period helpers ─────────────────────────────────────────────────────────── */

type Preset = 7 | 30 | 90;

function startOfDay(d: Date): Date {
  const copy = new Date(d);
  copy.setHours(0, 0, 0, 0);
  return copy;
}

function presetRange(preset: Preset): { from: Date; to: Date } {
  const to = startOfDay(new Date());
  const from = new Date(to);
  from.setDate(from.getDate() - preset + 1);
  return { from, to };
}

/* ─── Chart ─────────────────────────────────────────────────────────────────── */

interface MoodPoint {
  date: Date;
  score: number;
}

function minScore(pts: MoodPoint[]): number {
  return Math.min(...pts.map((p) => p.score));
}

function maxScore(pts: MoodPoint[]): number {
  return Math.max(...pts.map((p) => p.score));
}

function scaleX(i: number, total: number, width: number, pad: number): number {
  if (total <= 1) return pad + (width - 2 * pad) / 2;
  return pad + (i / (total - 1)) * (width - 2 * pad);
}

function scaleY(score: number, min: number, max: number, height: number, pad: number): number {
  const range = max - min || 1;
  return pad + ((max - score) / range) * (height - 2 * pad);
}

const CHART_W = 600;
const CHART_H = 160;
const PAD = 24;

function TimelineChart({ points }: { points: MoodPoint[] }) {
  if (points.length === 0) return null;

  const min = minScore(points);
  const max = maxScore(points);
  const w = CHART_W;
  const h = CHART_H;
  const total = points.length;

  const dots = points.map((pt, i) => {
    const cx = scaleX(i, total, w, PAD);
    const cy = scaleY(pt.score, min, max, h, PAD);
    const label = new Date(pt.date).toLocaleDateString('es-AR', { dateStyle: 'short' });
    return (
      <g key={i}>
        <title>{`${label}: ${pt.score > 0 ? '+' : ''}${pt.score}`}</title>
        <circle cx={cx} cy={cy} r={4} fill="var(--brand-primary)" />
      </g>
    );
  });

  // polyline points
  const polyPoints = points
    .map((pt, i) => {
      const cx = scaleX(i, total, w, PAD);
      const cy = scaleY(pt.score, min, max, h, PAD);
      return `${cx},${cy}`;
    })
    .join(' ');

  // horizontal reference lines at min/max
  const minY = scaleY(min, min, max, h, PAD);
  const maxY = scaleY(max, min, max, h, PAD);

  return (
    <div className={styles.chartWrapper}>
      <svg
        viewBox={`0 0 ${w} ${h}`}
        className={styles.chart}
        aria-label="Grafico de humor en el tiempo"
        role="img"
      >
        <style>{`
          .chart-line {
            fill: none;
            stroke: var(--brand-primary);
            stroke-width: 2;
            stroke-linecap: round;
            stroke-linejoin: round;
          }
          @media (prefers-reduced-motion: reduce) {
            .chart-line { animation: none; }
          }
        `}</style>
        {/* reference lines */}
        <line x1={PAD} y1={minY} x2={w - PAD} y2={minY} stroke="var(--surface-muted)" strokeWidth={1} strokeDasharray="4,4" />
        <line x1={PAD} y1={maxY} x2={w - PAD} y2={maxY} stroke="var(--surface-muted)" strokeWidth={1} strokeDasharray="4,4" />
        {/* line */}
        <polyline points={polyPoints} className="chart-line" />
        {/* dots on top */}
        {dots}
      </svg>
    </div>
  );
}

/* ─── PeriodFilter ───────────────────────────────────────────────────────────── */

type FilterMode = 'preset' | 'custom';

interface PeriodFilterProps {
  activePreset: Preset | null;
  customFrom: string;
  customTo: string;
  onPreset: (p: Preset) => void;
  onCustomChange: (from: string, to: string) => void;
  onApply: () => void;
}

function PeriodFilter({
  activePreset,
  customFrom,
  customTo,
  onPreset,
  onCustomChange,
  onApply,
}: PeriodFilterProps) {
  return (
    <div className={styles.periodFilter} role="group" aria-label="Filtrar por periodo">
      <span className={styles.filterLabel}>Periodo:</span>
      {([7, 30, 90] as Preset[]).map((p) => (
        <button
          key={p}
          className={`${styles.presetBtn} ${activePreset === p ? styles.presetBtnActive : ''}`}
          onClick={() => onPreset(p)}
          aria-pressed={activePreset === p}
        >
          {p}d
        </button>
      ))}
      <span className={styles.filterDivider} aria-hidden="true" />
      <label className={styles.customLabel}>
        Desde
        <input
          type="date"
          className={styles.dateInput}
          value={customFrom}
          onChange={(e) => onCustomChange(e.target.value, customTo)}
        />
      </label>
      <label className={styles.customLabel}>
        Hasta
        <input
          type="date"
          className={styles.dateInput}
          value={customTo}
          onChange={(e) => onCustomChange(customFrom, e.target.value)}
        />
      </label>
      <button className={styles.applyBtn} onClick={onApply}>
        Aplicar
      </button>
    </div>
  );
}

/* ─── EntryRow ──────────────────────────────────────────────────────────────── */

function EntryRow({ entry }: { entry: TimelineEntry }) {
  const date = new Date(entry.created_at).toLocaleDateString('es-AR', {
    dateStyle: 'medium',
    timeStyle: 'short',
  });

  if (entry.entry_type === 'mood_entry') {
    const score = (entry.data as { score?: number }).score;
    return (
      <div className={styles.entry}>
        <span className={styles.entryType}>Registro de humor</span>
        <span className={styles.entryDate}>{date}</span>
        {score != null && (
          <span className={`${styles.score} ${score >= 0 ? styles.positive : styles.negative}`}>
            {score > 0 ? `+${score}` : score}
          </span>
        )}
      </div>
    );
  }

  if (entry.entry_type === 'daily_checkin') {
    const d = entry.data as Record<string, boolean | string | number>;
    return (
      <div className={styles.entry}>
        <span className={styles.entryType}>Check-in diario</span>
        <span className={styles.entryDate}>{date}</span>
        {d.anxiety && <span className={styles.flag}>Ansiedad</span>}
        {d.irritability && <span className={styles.flag}>Irritabilidad</span>}
      </div>
    );
  }

  return (
    <div className={styles.entry}>
      <span className={styles.entryType}>{entry.entry_type}</span>
      <span className={styles.entryDate}>{date}</span>
    </div>
  );
}

/* ─── MoodPoint helper ──────────────────────────────────────────────────────── */

function extractMoodPoints(entries: TimelineEntry[]): MoodPoint[] {
  return entries
    .filter((e) => e.entry_type === 'mood_entry')
    .map((e) => ({
      date: new Date(e.created_at),
      score: (e.data as { score?: number }).score ?? 0,
    }))
    .sort((a, b) => a.date.getTime() - b.date.getTime());
}

/* ─── Main component ────────────────────────────────────────────────────────── */

type ViewState = 'loading' | 'ready' | 'empty' | 'error';

export function Timeline({ patientId }: Props) {
  const [preset, setPreset] = useState<Preset | null>(30);
  const [customFrom, setCustomFrom] = useState('');
  const [customTo, setCustomTo] = useState('');
  const [entries, setEntries] = useState<TimelineEntry[] | null>(null);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [viewState, setViewState] = useState<ViewState>('loading');
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const load = useCallback(
    (from: Date, to: Date) => {
      setViewState('loading');
      getPatientTimelineByPeriod(patientId, from, to)
        .then((res) => {
          setEntries(res.entries);
          setTotal(res.total);
          setViewState(res.entries.length === 0 ? 'empty' : 'ready');
        })
        .catch((err: Error) => {
          setErrorMsg(err.message);
          setViewState('error');
        });
    },
    [patientId],
  );

  // Initial load with default preset
  useEffect(() => {
    const { from, to } = presetRange(30);
    let cancelled = false;

    getPatientTimelineByPeriod(patientId, from, to)
      .then((res) => {
        if (cancelled) return;
        setEntries(res.entries);
        setTotal(res.total);
        setViewState(res.entries.length === 0 ? 'empty' : 'ready');
      })
      .catch((err: Error) => {
        if (cancelled) return;
        setErrorMsg(err.message);
        setViewState('error');
      });

    return () => {
      cancelled = true;
    };
  }, [patientId]);

  const handlePreset = (p: Preset) => {
    setPreset(p);
    setCustomFrom('');
    setCustomTo('');
    const { from, to } = presetRange(p);
    load(from, to);
  };

  const handleCustomChange = (from: string, to: string) => {
    setCustomFrom(from);
    setCustomTo(to);
    setPreset(null);
  };

  const handleApply = () => {
    if (!customFrom || !customTo) return;
    const from = new Date(customFrom + 'T00:00:00');
    const to = new Date(customTo + 'T23:59:59');
    setPreset(null);
    load(from, to);
  };

  const moodPoints = entries ? extractMoodPoints(entries) : [];

  if (viewState === 'error') {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo cargar el historial.</p>
        <small>{errorMsg}</small>
        <button className={styles.retryBtn} onClick={() => handlePreset(preset ?? 30)}>
          Reintentar
        </button>
      </div>
    );
  }

  if (viewState === 'loading' && entries === null) {
    return (
      <section aria-labelledby="timeline-heading">
        <h2 id="timeline-heading" className={styles.heading}>Historial</h2>
        <div className={styles.skeleton}>
          <div className={styles.skeletonChart} aria-hidden="true" />
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className={styles.skeletonRow} />
          ))}
        </div>
      </section>
    );
  }

  if (viewState === 'empty') {
    return (
      <section aria-labelledby="timeline-heading">
        <h2 id="timeline-heading" className={styles.heading}>Historial</h2>
        <PeriodFilter
          activePreset={preset}
          customFrom={customFrom}
          customTo={customTo}
          onPreset={handlePreset}
          onCustomChange={handleCustomChange}
          onApply={handleApply}
        />
        <div className={styles.emptyState} role="status">
          <svg width={40} height={40} viewBox="0 0 24 24" fill="none" aria-hidden="true">
            <path d="M3 12h18M3 6h18M3 18h18" stroke="var(--foreground-muted)" strokeWidth={1.5} strokeLinecap="round" />
          </svg>
          <p>No hay registros en este periodo.</p>
        </div>
      </section>
    );
  }

  return (
    <section aria-labelledby="timeline-heading">
      <h2 id="timeline-heading" className={styles.heading}>Historial</h2>

      {/* S02-PERIOD: chart + filter */}
      <TimelineChart points={moodPoints} />

      <PeriodFilter
        activePreset={preset}
        customFrom={customFrom}
        customTo={customTo}
        onPreset={handlePreset}
        onCustomChange={handleCustomChange}
        onApply={handleApply}
      />

      {/* List below chart */}
      <div className={styles.list}>
        {(entries ?? []).map((e) => (
          <EntryRow key={e.id} entry={e} />
        ))}
      </div>

      {total > page * 20 && (
        <button
          className={styles.loadMore}
          onClick={() => setPage((p) => p + 1)}
        >
          Cargar mas
        </button>
      )}
    </section>
  );
}
