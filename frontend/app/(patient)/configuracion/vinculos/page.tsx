import type { Metadata } from 'next';
import { VinculosManager } from '../../../../components/patient/vinculos/VinculosManager';
import { PatientPageShell } from '../../../../components/ui/PatientPageShell';

export const metadata: Metadata = {
  title: 'Mis vínculos | Bitácora',
};

export default function VinculosPage() {
  return (
    <PatientPageShell>
      <VinculosManager />
    </PatientPageShell>
  );
}
