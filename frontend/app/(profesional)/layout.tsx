import type { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'Bitacora — Panel Profesional',
  description: 'Panel de acompanamiento profesional',
};

export default function ProfesionalLayout({ children }: { children: React.ReactNode }) {
  return children;
}
