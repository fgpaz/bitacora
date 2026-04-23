import type { Metadata } from 'next';
import { ConsentRevocationPanel } from '../../../../components/patient/consent/ConsentRevocationPanel';
import { PatientPageShell } from '../../../../components/ui/PatientPageShell';

export const metadata: Metadata = {
  title: 'Consentimiento | Bitácora',
  description: 'Revisar y revocar el consentimiento informado activo.',
};

export default function ConsentPage() {
  return (
    <PatientPageShell>
      <ConsentRevocationPanel />
    </PatientPageShell>
  );
}
