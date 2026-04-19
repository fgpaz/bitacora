# CT-AUTH-ZITADEL: Contrato IdP Zitadel Teslita

> Root: `09_contratos_tecnicos.md` — seccion Autenticacion.
> Sibling: `CT-AUTH.md` (Supabase Auth, runtime actual Bitacora Wave A).
> Status: Wave A gap-closure ejecutada 2026-04-19. G1/G2/G4/G6 GREEN; G3/G5/G7 requieren evidencia/aprobacion del owner.

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
| Login companion | `0qRNmuYflmhQZz9vFeC8f` (`teslita-shared-zitadel-login`), image `ghcr.io/zitadel/zitadel-login:v4.9.0` |

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
| `nuestrascuentitas` | `369304228570661222` | landing + marketing |
| `bitacora` | `369305924310925670` | clinical mood tracker |
| `multi-tedi` | `369305928773665126` | futuro (IA conversacional) |
| `gastos` | `369305933253181798` | futuro (gastos personales) |

## 5. OAuth Clients (per org)

Cada org tiene 1 project + 1 Web PKCE public app + 1 machine service account para `client_credentials`.

| Org | projectId | web clientId | M2M clientId | M2M userId |
|-----|-----------|--------------|--------------|------------|
| nuestrascuentitas | `369306314246979942` | `369306318709784934` | `nuestrascuentitas-api-client` | `369306707119047014` |
| bitacora | `369306332534145382` | `369306336963330406` | `bitacora-api-client` | `369306719433523558` |
| multi-tedi | `369306350636761446` | `369306355065946470` | `multi-tedi-api-client` | `369306729382412646` |
| gastos | `369306369645347174` | `369306374074597734` | `gastos-api-client` | `369306738643435878` |

Secrets M2M + redirect URIs placeholder viven en Infisical `ZITADEL_CLIENT_<ORG>_*`. Wave B y Wave C' etc actualizan redirect URIs reales al momento de integrar cada frontend.

**Note (2026-04-19):** el grant `client_credentials` queda GREEN usando machine users con user secret. Los API apps placeholder originales no se usan como secreto M2M hasta definir un caso real de API app en cada producto.

## 6. Users admin

| Username | Type | Rol | PAT en Infisical |
|----------|------|-----|------------------|
| `paz.fgabriel@gmail.com` | HUMAN | IAM Owner default | no (humanos no tienen PAT) |
| `zitadel-admin-sa` | MACHINE | IAM Owner (via bootstrap) | `ZITADEL_ADMIN_PAT` |
| `login-client` | MACHINE | Login companion | `ZITADEL_LOGIN_CLIENT_PAT` |

Human admin password persiste en Infisical `ZITADEL_ADMIN_INITIAL_PASSWORD` y backup cifrado `infra/secrets.enc.env`. Fue rotada el 2026-04-19 durante el cierre de gaps; no se registra en docs, issues, logs ni chat.

## 7. Policies baseline Teslita (post T2.5)

| Policy | Valor actual | Target |
|--------|-------------|--------|
| Password min length | 10 | 10 |
| Password complexity | upper + lower + number | idem |
| MFA required admins | pending owner enrollment on owner device | yes |
| Session access lifetime | default | 1h target |
| Session refresh lifetime | default | 30d target |
| Audit retention | default Zitadel (~90d) | 2 anos |

## 8. SMTP

| Campo | Valor |
|-------|-------|
| Provider | Resend |
| Host:Port | smtp.resend.com:587 |
| TLS | true |
| Sender | `noreply@nuestrascuentitas.com` |
| SMTP config id | `369306109413949798` (active) |
| Test mail | enviado a `paz.fgabriel@gmail.com` via `/admin/v1/smtp/{id}/_test` |
| DKIM/SPF | DKIM TXT presente; SPF root TXT no resuelve; headers Gmail pendientes |

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

Estrategia: docker volume snapshot + offsite (decidida 2026-04-18, instalada 2026-04-19).

- Script: `infra/backups/zitadel/snapshot.sh` (git-tracked)
- Cron target: 03:00 UTC diario on VPS turismo (`/etc/cron.d/zitadel-backup`)
- Offsite provider: rclone SFTP via Tailscale to `teslita-zitadel:/home/fgpaz/backups/zitadel`
- Retention: 30d local / 90d offsite
- Latest verified snapshot: `zitadel-pg-20260419-173731.tar.gz` (`10,716,399` bytes), OIDC healthcheck `200`
- Runbook: `infra/runbooks/zitadel-backup.md`
- Recovery: `infra/runbooks/zitadel-recovery.md`

## 11. Gaps conocidos (post Wave A, 2026-04-19)

- [x] **Login companion app** deployado: `ghcr.io/zitadel/zitadel-login:v4.9.0`, route `/ui/v2/login`, authorize real devuelve `Welcome back!`.
- [x] Backup cron instalado en VPS y validado con snapshot manual.
- [ ] MFA admin no enrollada: login UI ya funciona, password humano rotado y verificado; `auth_factors/_search` devuelve `{}`. Falta enrollment del owner en su propio dispositivo. No crear passkey personal desde Playwright/Codex porque quedaria ligada al perfil de automatizacion; si se acepta un TOTP compartido, documentarlo como break-glass y guardar el secreto en Infisical.
- [x] Client credentials grant funciona con JWT Bearer RS256 y `kid` para las 4 orgs.
- [ ] Legacy Postgres `postgres-bypass-wireless-bus-tupzoj` sigue corriendo; no es safe removal porque contiene `801` eventos.
- [x] Offsite backup remote configurado y archivo listable via rclone.
- [ ] DKIM/SPF: test mail enviado; DKIM TXT presente, SPF root ausente, headers Gmail pendientes.

## 12. Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (indice)
- `07_baseline_tecnica.md` (auth runtime dual)
- `02_arquitectura.md` (diagrama Auth externa)
- Wave B plan (mapeo claims + transicion)
- Memoria `project_idp_zitadel_multi_ecosistema.md` (endpoints reales)
- Runbooks `infra/runbooks/zitadel-*.md`
