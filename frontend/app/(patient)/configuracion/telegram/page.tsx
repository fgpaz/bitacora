import type { Metadata } from 'next';
import { TelegramPairingCard } from '../../../../components/patient/telegram/TelegramPairingCard';
import { PatientPageShell } from '../../../../components/ui/PatientPageShell';

export const metadata: Metadata = {
  title: 'Vincular Telegram — Bitácora',
};

export default function TelegramConfigPage() {
  return (
    <PatientPageShell>
      <TelegramPairingCard />
    </PatientPageShell>
  );
}
