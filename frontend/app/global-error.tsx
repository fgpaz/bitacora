'use client';

/**
 * global-error.tsx — refugio de marca ante error catastrofico del layout root.
 * Contrato Next.js App Router: 'use client' + wrapper <html><body>.
 *
 * Estilos inline porque CSS Modules NO estan disponibles cuando el layout root falla.
 * Los valores son espejo exacto de frontend/styles/tokens.css para preservar identidad:
 *   surface: #F6F1EA, foreground: #2E2A28, brand-primary: #5E766E,
 *   foreground-muted: #4A4440, radius-md: 8.
 */

import { useEffect } from 'react';

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // No exponer error.message al usuario (canon 13).
    // El digest queda disponible en logs del servidor via Next.js.
  }, [error]);

  return (
    <html lang="es">
      <body
        style={{
          minHeight: '100vh',
          margin: 0,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '24px',
          fontFamily: "'Source Sans 3', system-ui, sans-serif",
          backgroundColor: '#F6F1EA',
          color: '#2E2A28',
        }}
      >
        <main style={{ maxWidth: 480, width: '100%', textAlign: 'center' }}>
          <p
            style={{
              fontFamily: "'Newsreader', Georgia, serif",
              fontSize: '1.75rem',
              fontWeight: 600,
              margin: 0,
            }}
          >
            Bitácora
          </p>
          <h1
            style={{
              fontFamily: "'Newsreader', Georgia, serif",
              fontSize: '1.5rem',
              marginTop: '32px',
              marginBottom: '16px',
            }}
          >
            No pudimos cargar el sitio.
          </h1>
          <p
            style={{
              color: '#4A4440',
              marginTop: 0,
              marginBottom: '32px',
              lineHeight: 1.5,
            }}
          >
            Probá recargar la página. Si el problema continúa, escribinos desde tu correo.
          </p>
          <button
            type="button"
            onClick={() => reset()}
            style={{
              minHeight: 44,
              minWidth: 120,
              padding: '12px 24px',
              borderRadius: 8,
              border: '1px solid #5E766E',
              backgroundColor: '#5E766E',
              color: '#FFFFFF',
              fontFamily: "'Source Sans 3', system-ui, sans-serif",
              fontSize: '0.9375rem',
              fontWeight: 700,
              cursor: 'pointer',
            }}
          >
            Recargar
          </button>
        </main>
      </body>
    </html>
  );
}
