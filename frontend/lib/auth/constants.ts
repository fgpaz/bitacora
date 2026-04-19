export const SESSION_COOKIE = 'bitacora_session';
export const OIDC_STATE_COOKIE = 'bitacora_oidc_state';
export const OIDC_VERIFIER_COOKIE = 'bitacora_pkce_verifier';
export const OIDC_NONCE_COOKIE = 'bitacora_oidc_nonce';
export const LEGACY_SUPABASE_COOKIES = ['sb-access-token', 'sb-supabase-session'] as const;
export const ZITADEL_ROLES_CLAIM = 'urn:zitadel:iam:org:project:roles';

export type BitacoraRole = 'patient' | 'professional';

export interface PublicSession {
  user: {
    id: string;
    email: string;
    role: BitacoraRole;
  } | null;
  expiresAt: number | null;
}
