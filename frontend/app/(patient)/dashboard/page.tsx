/**
 * Patient Dashboard Page
 * URL: /dashboard
 * Shows mood history, summary statistics, and quick access buttons.
 */

import type { Metadata } from 'next';
import { Dashboard } from '@/components/patient/dashboard/Dashboard';
import { PatientPageShell } from '@/components/ui/PatientPageShell';
import styles from './DashboardPage.module.css';

export const metadata: Metadata = {
  title: 'Mi historial | Bitácora',
  description: 'Visualizá tu historial de humor y estadísticas.',
};

export default function DashboardPage() {
  return (
    <PatientPageShell>
      <section aria-labelledby="dashboard-heading">
        <h1 id="dashboard-heading" className={styles.heading}>
          Mi historial
        </h1>
        <Dashboard />
      </section>
    </PatientPageShell>
  );
}
