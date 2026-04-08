# Comparticion Profesional-Paciente — Patrones Seguros

## Patrones encontrados en la industria

| Patron | Ejemplo | Pros | Contras |
|--------|---------|------|---------|
| **PDF mensual por email** | eMoods | Simple, offline | Sin interactividad, no tiempo real |
| **Codigos temporales con vencimiento** | Smart contracts | Expiracion automatica, control del paciente | Complejidad de implementacion |
| **Links con TTL** | Google Docs sharing | Acceso temporal por URL, facil de usar | Riesgo de forward de links |
| **Acceso delegado con consentimiento** | HIPAA-compliant platforms | Mas seguro, auditable | Requiere gestion de cuentas |

**Fuentes:**
- [Privacy in Mental Health Apps - PMC](https://pmc.ncbi.nlm.nih.gov/articles/PMC9643945/)
- [Secure Health Data Sharing - Nature npj Digital Medicine](https://www.nature.com/articles/s41746-025-01945-z)

## Diseno recomendado (3 niveles)

### Nivel 1: Codigo temporal (publico)
- El paciente genera un codigo desde la web/Telegram
- El profesional accede con codigo + DNI del paciente
- Vence en X horas/dias (configurable)
- Vista de solo lectura, sin interaccion
- **Decision MVP:** pendiente — podria quedar fuera del MVP si el vinculo persistente es suficiente

### Nivel 2: Cuenta vinculada (principal en MVP)
- El profesional invita al paciente O el paciente se registra y vincula con codigo del profesional
- El paciente acepta y otorga consentimiento digital
- Acceso permanente revocable por el paciente en cualquier momento
- Auditoria completa de cada acceso
- **Decision MVP:** este es el mecanismo principal confirmado

### Nivel 3: Export PDF/CSV (fallback)
- Siempre disponible como opcion offline
- El paciente genera y descarga/envia manualmente
- **Decision MVP:** pendiente — podria ser MVP o Roadmap

## Implicaciones para el diseno

- El vinculo persistente (Nivel 2) es el nucleo del modelo de sharing del MVP.
- El consentimiento revocable es obligatorio por Ley 25.326 y 26.657.
- Cada acceso profesional debe quedar registrado en el log de auditoria.
- El modelo hibrido de onboarding (profesional crea O paciente se auto-registra) soporta ambos flujos.
