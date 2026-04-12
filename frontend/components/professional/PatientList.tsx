'use client';

/**
 * PatientList — fetches and displays professional patient list.
 * States: loading | ready | error | empty
 */
import { useEffect, useState } from 'react';
import Link from 'next/link';
import { getProfessionalPatients } from '@/lib/api/professional';
import type { ProfessionalPatient } from '@/lib/api/professional';
import styles from './PatientList.module.css';

function PatientCard({ patient }: { patient: ProfessionalPatient }) {
  return (
    <Link
      href={`/profesional/pacientes/${patient.id}`}
      className={styles.card}
    >
      <div className={styles.cardHeader}>
        <span className={styles.cardName}>{patient.display_name}</span>
        <span className={`${styles.cardStatus} ${styles[patient.status]}`}>
          {patient.status === 'active' ? 'Activo' : patient.status === 'pending' ? 'Pendiente' : 'Inactivo'}
        </span>
      </div>
      <span className={styles.cardEmail}>{patient.email}</span>
    </Link>
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

  useEffect(() => {
    getProfessionalPatients()
      .then((res) => setPatients(res.patients ?? []))
      .catch((err: Error) => setError(err.message));
  }, []);

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
    <div className={styles.list}>
      {patients.map((p) => (
        <PatientCard key={p.id} patient={p} />
      ))}
    </div>
  );
}
