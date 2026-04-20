# Bitacora

Mood tracker clinico para pacientes de psicologia/psiquiatria. Permite registrar el estado animico diario (escala -3 a +3), horas de sueno, actividad fisica/social, ansiedad, irritabilidad y adherencia a la medicacion. Genera graficos longitudinales para compartir de forma segura con el profesional tratante.

**Slug:** bitacora.nuestrascuentitas.com
**Stack:** .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL
**Auth:** Zitadel OIDC + PKCE (`id.nuestrascuentitas.com`)
**Deploy:** Dokploy VPS

## Canales

- **Web:** registro completo, graficos, gestion de vinculo profesional-paciente
- **Telegram Bot:** registro rapido de humor y factores diarios, recordatorios

## Integracion futura

Preparado para integrarse con multi-tedi via capability service protocol como parte de un paquete de salud digital.
