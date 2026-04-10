# CT-AUTH: Integracion con Supabase Auth

> Root: `09_contratos_tecnicos.md` seccion autenticacion.

## Provider

| Campo | Valor |
|-------|-------|
| Servicio | Supabase Auth (GoTrue) |
| Instancia | auth.tedi.nuestrascuentitas.com |
| Compartida con | multi-tedi (misma instancia) |
| Metodos habilitados | Magic Link, Google OAuth |
| Password auth | Solo para QA/testing |

## Flujo de autenticacion

```text
1. Frontend (Next.js) inicia auth via @supabase/supabase-js
2. Supabase Auth emite JWT con sub = supabase_user_id
3. Frontend envia JWT como Bearer header en cada request
4. Bitacora.Api valida JWT por clave simetrica (`Supabase:JwtSecret` o sus aliases por env)
5. Resuelve User.supabase_user_id → User.user_id + role
6. Inyecta contexto: {user_id, role, patient_id_or_professional_id}
```

## Validacion JWT

- **Metodo:** Clave simetrica (`Supabase:JwtSecret` o env `Supabase__JwtSecret` / `SUPABASE_JWT_SECRET`), no OIDC discovery.
- **Razon:** GoTrue no sirve `.well-known/openid-configuration`. La red interna Dokploy causaria fallos DNS para discovery URLs.
- **Claims usados:** `sub` (supabase_user_id), `email`, `exp`.
- **Validaciones adicionales implementadas:** si existe `User.sessions_revoked_at` y `jwt.iat` es anterior, el token se rechaza.
- **Clock skew:** 30 segundos tolerados.

## Variables de entorno

```env
SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com
SUPABASE_JWT_SECRET=<secret>
SUPABASE_ANON_KEY=<anon_key>
```

El runtime local tambien acepta `Supabase:JwtSecret` desde `appsettings*.json` para destrabar tooling y smoke local sin exponer secretos reales.

## Session revocation

| Campo en User | Funcion |
|---------------|---------|
| sessions_revoked_at | Si JWT.iat < sessions_revoked_at → rechazar token |

Permite revocacion global de sesiones sin depender de Supabase.

## Roles

| Role | Descripcion | Resolucion |
|------|-------------|-----------|
| patient | Paciente registrado | User.role = patient |
| professional | Profesional (psicologo/psiquiatra) | User.role = professional |

> No hay role `admin` ni `operator` en MVP. Operaciones administrativas son directas en DB.

## Estado de alcance

- Implementado hoy: JWT de paciente, bootstrap local de `User`, revocacion por `sessions_revoked_at`.
- Diferido: autenticacion Telegram, flujos profesionales y cualquier rol administrativo.

## Telegram auth

El bot de Telegram no usa JWT. La autenticacion es por TelegramSession:
- chat_id → TelegramSession → patient_id
- No hay header Bearer en webhooks de Telegram
- La autenticacion del webhook se valida por Telegram signature (secret token)

## Sync gates

Cambios en auth fuerzan revision de:
- RF-ONB-001, RF-ONB-002 (bootstrap de User desde JWT)
- 07_baseline_tecnica.md si cambia provider o metodo de validacion
- Frontend si cambian claims o metodos de auth
