'use client';

/**
 * VinculosList — displays active care links with professional details.
 *
 * Props:
 *  - links: array of CareLink objects
 *  - onRevoke: callback to revoke a specific link
 *  - revoking: ID of the link currently being revoked (for UI state)
 *
 * Features:
 *  - Status badge (Activo: green, Pendiente: yellow, Revocado: gray)
 *  - Professional name and email (if available)
 *  - Inline confirmation before revoke
 *  - Disabled button while revoking
 */
import { useState } from 'react';
import type { CareLink } from '../../../lib/api/client';
import styles from './VinculosList.module.css';

interface VinculosListProps {
  links: CareLink[];
  onRevoke: (id: string) => void;
  revoking: string | null;
}

export function VinculosList({ links, onRevoke, revoking }: VinculosListProps) {
  const [confirmingId, setConfirmingId] = useState<string | null>(null);

  if (links.length === 0) {
    return (
      <div className={styles.empty}>
        <p className={styles.emptyText}>No tenés profesionales vinculados</p>
      </div>
    );
  }

  const getStatusBadge = (status: CareLink['status']) => {
    if (status === 'active') return { label: 'Activo', className: styles.badgeActive };
    if (status === 'pending') return { label: 'Pendiente', className: styles.badgePending };
    if (status === 'revoked') return { label: 'Revocado', className: styles.badgeRevoked };
    return { label: status, className: '' };
  };

  return (
    <div className={styles.list}>
      {links.map((link) => {
        const badge = getStatusBadge(link.status);
        const isRevoking = revoking === link.id;
        const isConfirming = confirmingId === link.id;

        return (
          <div key={link.id} className={styles.item}>
            <div className={styles.header}>
              <div className={styles.info}>
                <p className={styles.name}>{link.professional_name}</p>
                {link.professional_email && (
                  <p className={styles.email}>{link.professional_email}</p>
                )}
              </div>
              <span className={badge.className}>{badge.label}</span>
            </div>

            <div className={styles.details}>
              <p className={styles.permission}>
                {link.can_view_data ? 'Puede acceder a tus registros' : 'Sin acceso a registros'}
              </p>
              <p className={styles.timestamp}>
                Vinculado desde {new Date(link.created_at).toLocaleDateString('es-AR')}
              </p>
            </div>

            {link.status === 'active' && !isConfirming && (
              <button
                className={styles.revokeButton}
                onClick={() => setConfirmingId(link.id)}
                disabled={isRevoking}
                aria-busy={isRevoking}
              >
                {isRevoking ? 'Desvinculando...' : 'Desvincular'}
              </button>
            )}

            {isConfirming && (
              <div className={styles.confirmBlock}>
                <p className={styles.confirmText}>¿Desvincularte de este profesional?</p>
                <div className={styles.confirmButtons}>
                  <button
                    className={styles.confirmCancel}
                    onClick={() => setConfirmingId(null)}
                    disabled={isRevoking}
                  >
                    Cancelar
                  </button>
                  <button
                    className={styles.confirmRevoke}
                    onClick={() => {
                      setConfirmingId(null);
                      onRevoke(link.id);
                    }}
                    disabled={isRevoking}
                    aria-busy={isRevoking}
                  >
                    {isRevoking ? 'Desvinculando...' : 'Sí, desvincular'}
                  </button>
                </div>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
