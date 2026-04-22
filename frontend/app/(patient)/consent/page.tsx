/**
 * ConsentPage — dedicated route for standalone consent reading and granting.
 * Delegates to OnboardingFlow's consent sub-states for consistency.
 */
import { Suspense } from 'react';
import { OnboardingFlow } from '@/components/patient/onboarding/OnboardingFlow';

function LoadingFallback() {
  return <div style={{ minHeight: '100vh', background: 'var(--surface)' }} />;
}

export default function ConsentPage() {
  return (
    <Suspense fallback={<LoadingFallback />}>
      <OnboardingFlow />
    </Suspense>
  );
}
