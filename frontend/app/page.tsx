import { cookies } from 'next/headers';
import { OnboardingEntryHero } from '@/components/patient/onboarding/OnboardingEntryHero';

// Nombre literal de la cookie de sesion. Espejo de lib/auth/constants.ts SESSION_COOKIE.
// Se hardcode para evitar import desde lib/auth/* (zona congelada).
const SESSION_COOKIE_NAME = 'bitacora_session';

interface HomePageProps {
  searchParams: Promise<{ declined?: string }>;
}

export default async function HomePage({ searchParams }: HomePageProps) {
  const params = await searchParams;
  const declined = params.declined === '1';

  const cookieStore = await cookies();
  const hasSession = cookieStore.has(SESSION_COOKIE_NAME);

  // Si el paciente acaba de rechazar el consent, no redirigimos al dashboard
  // aunque la cookie siga presente: mostramos el hero estandar con mensaje sereno.
  const variant = declined ? 'standard' : hasSession ? 'returning' : 'standard';
  const message = declined
    ? 'Podés aceptar cuando quieras. Tu sesión sigue activa.'
    : undefined;

  return <OnboardingEntryHero variant={variant} message={message} />;
}
