# Bitacora API - Production Dokploy Spec

## Truthful scope for T01

- Runtime: `Bitacora.Api`
- Build source: `Dockerfile` en repo root
- Service recipe mirror: `src/Bitacora.Api/Dockerfile`
- Public host: `api.bitacora.nuestrascuentitas.com`
- Public root host moved to the dedicated frontend runtime.
- Internal port: `8080`
- Liveness: `GET /health`
- Readiness: `GET /health/ready`
- Event bus: disabled (`EventBusSettings__HostAddress` stays blank)

## Creation flow

1. Ensure `infra/.env` exists and `dkp.ps1 doctor` passes from this repo.
2. Create a project if `BITACORA_PROJECT_ID` is still blank:

```powershell
& $DKP POST project.create '{"name":"bitacora","description":"Bitacora dedicated production lane","environmentId":"<DOKPLOY_ENVIRONMENT_ID>"}'
```

3. Create the application if `BITACORA_API_APP_ID` is blank:

```powershell
& $DKP POST application.create '{"name":"bitacora-api","projectId":"<BITACORA_PROJECT_ID>","description":"Bitacora.Api production"}'
```

4. Pin the build type to Dockerfile:

```powershell
& $DKP POST application.saveBuildType '{"applicationId":"<BITACORA_API_APP_ID>","buildType":"dockerfile"}'
```

5. Attach the GitHub App source:

```powershell
& $DKP POST application.saveGithubProvider '{"applicationId":"<BITACORA_API_APP_ID>","repository":"bitacora","branch":"main","owner":"fgpaz","githubId":"<DOKPLOY_GITHUB_PROVIDER_ID>"}'
```

6. In Dokploy UI or API settings, keep:
   - Dockerfile path: `Dockerfile`
   - Port: `8080`
   - Start command: Dockerfile default

7. Save the runtime environment with:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__BitacoraDb`
   - `ZITADEL_AUTHORITY=https://id.nuestrascuentitas.com`
   - `ZITADEL_AUDIENCE=369306332534145382`
   - `ZITADEL_PROJECT_BITACORA_ID=369306332534145382`
   - `ZITADEL_CLIENT_BITACORA_WEB_ID=369306336963330406`
   - `BITACORA_ENCRYPTION_KEY`
   - `BITACORA_PSEUDONYM_SALT`
   - `SUPABASE_JWT_SECRET` only during the rollback window; the active runtime does not read it
   - optional telemetry vars

8. Create the public domain:

```powershell
& $DKP POST domain.create '{"applicationId":"<BITACORA_API_APP_ID>","host":"api.bitacora.nuestrascuentitas.com","port":8080,"https":true,"certificateType":"letsencrypt"}'
```

9. Deploy:

```powershell
& $DKP POST application.deploy '{"applicationId":"<BITACORA_API_APP_ID>"}'
```

10. Validate with:
   - `& $DKP status <BITACORA_API_APP_ID>`
   - `sshr exec --host turismo --cmd "docker ps --format '{{.Names}} {{.Status}}' | grep -i bitacora"`
   - `curl -f https://api.bitacora.nuestrascuentitas.com/health`
   - `curl -f https://api.bitacora.nuestrascuentitas.com/health/ready`
   - `curl -I https://api.bitacora.nuestrascuentitas.com/health`

## Blocking conditions

- `infra/.env` missing or `DOKPLOY_API_KEY` unresolved
- `BITACORA_PROJECT_ID` or `DOKPLOY_GITHUB_PROVIDER_ID` unknown and not discoverable via control-plane API
- DNS for `api.bitacora.nuestrascuentitas.com` not pointed to `54.37.157.93`
- the bootstrap commit with `Dockerfile` and readiness/smoke assets is not pushed to `main`
- readiness remains red after migrations and env wiring
