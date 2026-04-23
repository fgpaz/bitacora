/**
 * UserFacingError — contrato de mensajes de error que se muestran al paciente.
 *
 * Nunca expone err.message crudo al usuario. Mapea codigos tecnicos conocidos
 * a copy canon 13 y cae a un fallback concreto (no generico).
 *
 * Canon 13 §Errores: "decir que paso + que puede hacer la persona si corresponde".
 * Evita formulaciones prohibidas como "Ups. Algo salio mal." o "Ocurrio algo
 * inesperado.".
 */

export interface UserFacingError {
  title: string;
  description: string;
  retry?: () => void;
}

const DEFAULT_ERROR: UserFacingError = {
  title: 'No pudimos completar la acción.',
  description: 'Probá en unos minutos o volvé al inicio.',
};

interface FormatOptions {
  fallback?: UserFacingError;
  retry?: () => void;
}

/**
 * Mapea un error tecnico a un mensaje canon 13 usable por UI.
 * Nunca devuelve err.message crudo.
 */
export function formatUserFacingError(
  err: unknown,
  options: FormatOptions = {},
): UserFacingError {
  const fallback = options.fallback ?? DEFAULT_ERROR;
  const retry = options.retry;

  const code =
    err && typeof err === 'object' && 'code' in err
      ? (err as { code?: string }).code
      : undefined;

  switch (code) {
    case 'NETWORK':
    case 'FETCH_FAILED':
      return {
        title: 'Sin conexión estable.',
        description: 'Revisá tu conexión y probá de nuevo.',
        retry,
      };
    case 'UNAUTHORIZED':
    case 'ONB_001_JWT_INVALID':
    case 'ONB_001_JWT_EXPIRED':
    case 'SESSION_EXPIRED':
      return {
        title: 'Tu sesión caducó.',
        description: 'Ingresá de nuevo para continuar.',
      };
    case 'CONSENT_REQUIRED':
      return {
        title: 'Necesitás aceptar el consentimiento.',
        description: 'Te llevamos al consentimiento para que puedas continuar.',
      };
    case 'CONSENT_VERSION_MISMATCH':
      return {
        title: 'La versión del consentimiento cambió.',
        description: 'Revisá el nuevo texto antes de aceptarlo.',
        retry,
      };
    case 'NO_CONSENT_CONFIG':
    case 'AUDIT_WRITE_FAILED':
      return {
        title: 'El servicio no está disponible en este momento.',
        description: 'Probá en unos minutos.',
        retry,
      };
    case 'INVALID_SCORE':
      return {
        title: 'El valor elegido no es válido.',
        description: 'Elegí otro y probá de nuevo.',
        retry,
      };
    default:
      return retry ? { ...fallback, retry } : fallback;
  }
}
