'use client';

/**
 * VinculosManager — orchestrates care link lifecycle on the client.
 *
 * Responsibilities:
 *  - Load initial care links via getCareLinksByPatient()
 *  - Handle revoke action with error handling and refetch
 *  - Trigger refetch after successful binding code acceptance
 *  - Render sections: current links + binding code form
 *
 * State machine:
 *  - loading: initial fetch
 *  - ready: display list + form
 *  - revoking: DELETE in flight for a specific link
 *  - error: API error (shown inline, retry available)
 */
import { useEffect, useState } from 'react';
import {
  getCareLinksByPatient,
  revokeCareLink,
  type CareLink,
} from '../../../lib/api/client';
import { BindingCodeForm } from './BindingCodeForm';
import { VinculosList } from './VinculosList';
import styles from './VinculosManager.module.css';

type LoadState = 'loading' | 'ready' | 'error';

export function VinculosManager() {
  const [links, setLinks] = useState<CareLink[]>([]);
  const [loadState, setLoadState] = useState<LoadState>('loading');
  const [loadError, setLoadError] = useState<string | null>(null);
  const [revoking, setRevoking] = useState<string | null>(null);
  const [revokeError, setRevokeError] = useState<string | null>(null);

  // Load care links on mount
  useEffect(() => {
    loadLinks();
  }, []);

  const loadLinks = async () => {
    setLoadState('loading');
    setLoadError(null);
    try {
      const response = await getCareLinksByPatient();
      setLinks(response.links);
      setLoadState('ready');
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al cargar los vínculos';
      setLoadError(msg);
      setLoadState('error');
    }
  };

  const handleRevoke = async (id: string) => {
    setRevoking(id);
    setRevokeError(null);
    try {
      await revokeCareLink(id);
      setLinks((prev) => prev.filter((link) => link.id !== id));
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error al desvincular';
      setRevokeError(msg);
    } finally {
      setRevoking(null);
    }
  };

  const handleBindingCodeSuccess = async () => {
    // Refetch after successful code acceptance
    await loadLinks();
  };

  if (loadState === 'loading') {
    return (
      <div className={styles.container}>
        <div className={styles.skeleton} aria-busy="true" aria-label="Cargando vínculos" />
      </div>
    );
  }

  return (
    <div className={styles.container}>
      {/* Section 1: Current care links */}
      <section className={styles.section}>
        <h2 className={styles.sectionTitle}>Mis profesionales</h2>

        {loadState === 'error' && (
          <div className={styles.errorBox} role="alert">
            <p className={styles.errorText}>{loadError}</p>
            <button className={styles.retryButton} onClick={loadLinks}>
              Reintentar
            </button>
          </div>
        )}

        {loadState === 'ready' && (
          <>
            <VinculosList
              links={links}
              onRevoke={handleRevoke}
              revoking={revoking}
            />

            {revokeError && (
              <p className={styles.inlineError} role="alert">
                {revokeError}
              </p>
            )}
          </>
        )}
      </section>

      {/* Divider */}
      <hr className={styles.divider} />

      {/* Section 2: Bind new professional */}
      <section className={styles.section}>
        <h2 className={styles.sectionTitle}>Vincularme con un profesional</h2>
        <p className={styles.sectionHint}>
          Pedí a tu profesional que te envíe un código de invitación.
        </p>
        <BindingCodeForm onSuccess={handleBindingCodeSuccess} />
      </section>
    </div>
  );
}
