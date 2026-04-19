# B0 Preflight — Wave B Zitadel Cutover

Date: 2026-04-19
Workspace: `C:\repos\mios\humor`
Plan commit: `1097e18 docs(plan): add bitacora wave b zitadel plan`

## Result

Initial B0 was blocked before code execution by missing `teslita` access in the Bash preflight shell.
The `mi-key-cli` wrapper access was repaired on 2026-04-19; B1-B6 remain intentionally not started until the rest of B0 is rerun.

## Checks

| Check | Result | Evidence |
|---|---|---|
| Governance | Pass | `mi-lsp nav governance --workspace bitacora --format toon` reported `blocked: false`, `sync: in_sync`. |
| Git status | Pass with known untracked docs/prompts | No tracked code WIP present. Existing untracked docs/prompts/reports remain untouched. |
| Recent commit lineage | Pass | `git log -5 --oneline` includes `885d268`, `7e48087`, and `9ea904c`. |
| OIDC discovery | Pass | `https://id.nuestrascuentitas.com/.well-known/openid-configuration` returned `200`. |
| JWKS | Pass | `https://id.nuestrascuentitas.com/oauth/v2/keys` returned `200`. |
| Infisical `teslita` access | Pass after repair | `bash -lc '$HOME/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod'` wrote `infra/.env` with 83 variables from vault `teslita`. |
| Redirect/logout URI verification | Blocked | Requires Zitadel/Infisical credentials from vault `teslita`. |
| No-user/no-clinical-data gate | Blocked | Requires approved production DB secret access from vault `teslita`. |

## Secret Gate Detail

The initial shell exposed `INFISICAL_TOKEN_BUHO`, but no `INFISICAL_TOKEN_TESLITA` or equivalent universal auth variables were available to the Bash implementation. The approved command failed without printing secret values:

```text
AuthTokenMissing: no se pudo resolver token para vault 'teslita'.
Opciones:
1. Setear INFISICAL_TOKEN_TESLITA
2. Setear INFISICAL_UNIVERSAL_AUTH_CLIENT_ID_TESLITA + ..._SECRET_TESLITA
3. Correr: infisical login --domain=http://100.115.71.27:9881
```

The repair keeps secrets out of plaintext logs and does not introduce a new Teslita service token. The Bash wrapper now delegates Windows/WSL calls to the PowerShell implementation, which can read Windows User/Machine scoped Machine Identity variables and the Universal Auth cache. PowerShell 5.1 fallback compatibility was also fixed for `Join-Path` and UTF-8 no-BOM `.env` writes.

Verified safe evidence:

```text
[mkey pull] Descargando secretos: proyecto=bitacora env=prod vault=teslita
[mkey pull] Secretos escritos en: C:\repos\mios\humor\infra\.env
[mkey pull] Total: 83 variables
ENV_EXISTS=True
ENV_KEY_LINES=83
ENV_HAS_UTF8_BOM=False
git check-ignore -v infra/.env -> .gitignore:48:infra/.env
```

## Remaining Stop Condition

Per the accepted plan, code waves B1-B6 must not start until B0 also verifies:

- verify Bitacora Zitadel redirect/logout URIs;
- verify the production no-user/no-clinical-data assumption with read-only evidence.

## Next Operator Action

Rerun the full T0 sequence from:

`.docs/raw/plans/2026-04-19-bitacora-wave-b-zitadel-y-fixes/T0-b0-preflight-secrets-redirects.md`
