# CT-AUTH-ZITADEL: Contrato IdP Zitadel Teslita

> Root: `09_contratos_tecnicos.md` — seccion Autenticacion.
> Sibling: `CT-AUTH.md` (Supabase Auth, runtime actual Bitacora Wave A).
> Status: Wave A completada en modo PARTIAL 2026-04-19. Wave B integra Bitacora (ver plan Wave B).

## 1. Instancia

| Campo | Valor |
|-------|-------|
| Producto | Zitadel self-hosted |
| Version | v4.9.0 (pinneada) |
| Licencia | Apache 2.0 |
| Host | `id.nuestrascuentitas.com` |
| VPS | turismo (`54.37.157.93`), detras de Traefik + Let's Encrypt |
| Postgres | dedicada (`shared_zitadel_v4`, pg17), no comparte con Bitacora |
| Ecosistema | Teslita (nuestrascuentitas + bitacora + multi-tedi + gastos) |
| Dokploy project | `teslita-shared-idp` (`AKHsAJScexTwhJBzfFRlk`) |
| Docker app | `zFdEECmPr1hhxwL0DKu4B` (`app-program-optical-matrix-xmjswe`) |

## 2. Endpoints OIDC (validados smoke T4.1)

| Endpoint | URL |
|----------|-----|
| issuer | `https://id.nuestrascuentitas.com` |
| authorization | `/oauth/v2/authorize` |
| token | `/oauth/v2/token` |
| userinfo | `/oidc/v1/userinfo` |
| jwks_uri | `/oauth/v2/keys` |
| introspection | `/oauth/v2/introspect` |
| revocation | `/oauth/v2/revoke` |
| end_session | `/oidc/v1/end_session` |
| device_authorization | `/oauth/v2/device_authorization` |

Signing algs: `RS256` (default), `RS384`, `RS512`, `ES256/384/512`, `EdDSA`.

## 3. JWT claims

| Claim | Descripcion |
|-------|-------------|
| sub | stable userId (Zitadel) |
| aud | projectId del client emisor |
| iss | `https://id.nuestrascuentitas.com` |
| exp / iat | timestamps UTC |
| email / email_verified | identidad |
| preferred_username / name | perfil |
| urn:zitadel:iam:org:id | orgId del user |
| urn:zitadel:iam:org:project:roles | `{role: {orgId: orgName}}` |

## 4. Organizations

| Name | orgId | Productos |
|------|-------|-----------|
| `nuestrascuentitas` | `369202905795854607` | landing + marketing |
| `bitacora` | `369203115896865039` | clinical mood tracker |
| `multi-tedi` | `369203117457146127` | futuro (IA conversacional) |
| `gastos` | `369203118983872783` | futuro (gastos personales) |

## 5. OAuth Clients (per org)

Cada org tiene 1 project + 2 apps (Web PKCE public + API client_credentials).

| Org | projectId | web clientId | api clientId |
|-----|-----------|--------------|--------------|
| nuestrascuentitas | `369203157235925263` | `369203159148593423` | `369203161010864399` |
| bitacora | `369203162839515407` | `369203164785737999` | `369203166698340623` |
| multi-tedi | `369203168677986575` | `369203170624209167` | `369203172603855119` |
| gastos | `369203174449348879` | `369203176278130959` | `369203178190668047` |

Secrets de API clients + redirect URIs placeholder viven en Infisical `ZITADEL_CLIENT_<ORG>_*`. Wave B y Wave C' etc actualizan redirect URIs reales al momento de integrar cada frontend.

**Note (2026-04-19):** el grant client_credentials inicial devolvio `invalid_client` en el smoke. Probable causa: project grant authorization pendiente en Zitadel v4 (requiere flag adicional en el API app). Tech debt para Wave B.

## 6. Users admin

| Username | Type | Rol | PAT en Infisical |
|----------|------|-----|------------------|
| `paz.fgabriel@gmail.com` | HUMAN | IAM Owner default | no (humanos no tienen PAT) |
| `zitadel-admin-sa` | MACHINE | IAM Owner (via bootstrap) | `ZITADEL_ADMIN_PAT` |
| `login-client` | MACHINE | Login companion | `ZITADEL_LOGIN_CLIENT_PAT` |

Human admin password persiste en Infisical `ZITADEL_ADMIN_INITIAL_PASSWORD`. Debe rotarse post-MFA enrollment.

## 7. Policies baseline Teslita (post T2.5)

| Policy | Valor actual | Target |
|--------|-------------|--------|
| Password min length | 10 | 10 |
| Password complexity | upper + lower + number | idem |
| MFA required admins | pending (login UI needed) | yes (post Wave A2) |
| Session access lifetime | default | 1h target |
| Session refresh lifetime | default | 30d target |
| Audit retention | default Zitadel (~90d) | 2 anos |

## 8. SMTP

| Campo | Valor |
|-------|-------|
| Provider | Resend |
| Host:Port | smtp.resend.com:465 |
| TLS | true |
| Sender | `noreply@nuestrascuentitas.com` |
| SMTP config id | `369203306586702095` (active) |
| Test mail | enviado a `paz.fgabriel@gmail.com` (DKIM/SPF verify pending) |

## 9. Transicion desde Supabase (Wave B)

Bitacora Wave 1 valida JWT Supabase via clave simetrica (`SUPABASE_JWT_SECRET`). Wave B introduce dual-IdP feature flag + validacion JWKS RS256 de Zitadel. Mapeo:

| Supabase claim | Zitadel claim | Handling |
|----------------|---------------|----------|
| sub | sub | directo (stable id) |
| email | email | directo |
| user_metadata.role | urn:zitadel:iam:org:project:roles | parsing custom |
| aud | aud | formato distinto — aceptar ambos durante transicion |

Plan: `.docs/raw/plans/2026-04-17-migracion-bitacora-zitadel-wave-b.md`.

## 10. Backup & disaster recovery

Estrategia: docker volume snapshot + offsite (decidida 2026-04-18).

- Script: `infra/backups/zitadel/snapshot.sh` (git-tracked)
- Cron target: 03:00 UTC diario on VPS turismo (**install pending**)
- Offsite provider: por elegir (B2 / R2 / Hetzner / rsync desktop)
- Retention: 30d local / 90d offsite
- Runbook: `infra/runbooks/zitadel-backup.md`
- Recovery: `infra/runbooks/zitadel-recovery.md`

## 11. Gaps conocidos (post Wave A, 2026-04-19)

- [ ] **Login companion app** (`ghcr.io/zitadel/zitadel-login:v4.9.0`) no deployado. Bloqueante para login end-user en Wave B.
- [ ] Backup cron no instalado en VPS.
- [ ] MFA admin no enrollada (requiere login UI).
- [ ] Client credentials grant no funciona (project grant pending).
- [ ] Legacy Postgres `postgres-bypass-wireless-bus-tupzoj` sigue corriendo sin uso.
- [ ] Offsite backup remote no configurado (user picks provider).

## 12. Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (indice)
- `07_baseline_tecnica.md` (auth runtime dual)
- `02_arquitectura.md` (diagrama Auth externa)
- Wave B plan (mapeo claims + transicion)
- Memoria `project_idp_zitadel_multi_ecosistema.md` (endpoints reales)
- Runbooks `infra/runbooks/zitadel-*.md`
