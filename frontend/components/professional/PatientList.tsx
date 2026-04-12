'use client';

/**
 * PatientList — fetches and displays professional patient list.
 * States: loading | ready | error | empty | pagination
 */
import { useEffect, useState } from 'react';
import Link from 'next/link';
import { getProfessionalPatientsPaginated } from '@/lib/api/professional';
import type { ProfessionalPatient } from '@/lib/api/professional';
import styles from './PatientList.module.css';

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 10;

function PatientCard({ patient }: { patient: ProfessionalPatient }) {
  return (
    <Link
      href={`/profesional/pacientes/${patient.id}`}
      className={styles.card}
    >
      <div className={styles.cardHeader}>
        <span className={styles.cardName}>{patient.display_name}</span>
        <span className={styles.cardHeaderRight}>
          {patient.hasRecentAlert && (
            <span className={styles.alertBadge} title="Alerta reciente">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                <path d="M12 2L2 20h20L12 2zm0 3.5L19.5 19h-15L12 5.5zM11 10v4h2v-4h-2zm0 6v2h2v-2h-2z"/>
              </svg>
            </span>
          )}
          <span className={`${styles.cardStatus} ${styles[patient.status]}`}>
            {patient.status === 'active' ? 'Activo' : patient.status === 'pending' ? 'Pendiente' : 'Inactivo'}
          </span>
        </span>
      </div>
      <span className={styles.cardEmail}>{patient.email}</span>
    </Link>
  );
}

function PaginationControls({
  page,
  totalPages,
  hasMore,
  total,
  onPrev,
  onNext,
}: {
  page: number;
  totalPages: number;
  hasMore: boolean;
  total?: number;
  onPrev: () => void;
  onNext: () => void;
}) {
  const prevDisabled = page <= 1;
  const nextDisabled = !hasMore;

  return (
    <div className={styles.pagination} aria-label="Paginacion">
      <span className={styles.pageInfo}>
        Pagina {page} de {totalPages}
        {total !== undefined && (
          <span className={styles.totalCount}> &middot; {total} pacientes</span>
        )}
      </span>
      <div className={styles.pageButtons}>
        <button
          className={styles.pageBtn}
          onClick={onPrev}
          disabled={prevDisabled}
          aria-label="Pagina anterior"
        >
          Anterior
        </button>
        <button
          className={styles.pageBtn}
          onClick={onNext}
          disabled={nextDisabled}
          aria-label="Pagina siguiente"
        >
          Siguiente
        </button>
      </div>
    </div>
  );
}

function SkeletonCard() {
  return (
    <div className={`${styles.card} ${styles.skeleton}`} aria-hidden="true">
      <div className={styles.skeletonName} />
      <div className={styles.skeletonEmail} />
    </div>
  );
}

export function PatientList() {
  const [patients, setPatients] = useState<ProfessionalPatient[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(DEFAULT_PAGE);
  const [pageSize] = useState(DEFAULT_PAGE_SIZE);
  const [total, setTotal] = useState<number | undefined>(undefined);
  const [hasMore, setHasMore] = useState(false);

  const totalPages = total !== undefined ? Math.ceil(total / pageSize) : 1;

  useEffect(() => {
    setPatients(null);
    getProfessionalPatientsPaginated(page, pageSize)
      .then((res) => {
        setPatients(res.patients ?? []);
        setTotal(res.total);
        setHasMore(res.hasMore ?? false);
      })
      .catch((err: Error) => setError(err.message));
  }, [page, pageSize]);

  const handlePrev = () => setPage((p) => Math.max(1, p - 1));
  const handleNext = () => setPage((p) => p + 1);

  if (error) {
    return (
      <div className={styles.errorState} role="alert">
        <p>No se pudo cargar la lista de pacientes.</p>
        <small>{error}</small>
      </div>
    );
  }

  if (patients === null) {
    return (
      <div className={styles.list}>
        {Array.from({ length: 3 }).map((_, i) => (
          <SkeletonCard key={i} />
        ))}
      </div>
    );
  }

  if (patients.length === 0) {
    return (
      <div className={styles.emptyState}>
        <p>No tenes pacientes vinculados todavia.</p>
        <p>Invita a alguien desde la seccion de invitaciones.</p>
      </div>
    );
  }

  return (
    <div>
      <div className={styles.list}>
        {patients.map((p) => (
          <PatientCard key={p.id} patient={p} />
        ))}
      </div>
      <PaginationControls
        page={page}
        totalPages={totalPages}
        hasMore={hasMore}
        total={total}
        onPrev={handlePrev}
        onNext={handleNext}
      />
    </div>
  );
}
