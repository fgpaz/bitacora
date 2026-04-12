'use client';

import { Suspense } from 'react';
import { OnboardingFlow } from '@/components/patient/onboarding/OnboardingFlow';

function LoadingFallback() {
  return <div style={{ minHeight: '100vh', background: 'var(--surface)' }} />;
}

export default function OnboardingPage() {
  return (
    <Suspense fallback={<LoadingFallback />}>
      <OnboardingFlow />
    </Suspense>
  );
}
