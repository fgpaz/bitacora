# Regulaciones y Privacidad — Argentina

## Ley 25.326 (Proteccion de Datos Personales / Habeas Data)

- Datos de salud son **datos sensibles** — requieren consentimiento expreso, libre e informado
- El consentimiento debe ser documentado por escrito o medio equivalente digital
- Nadie puede ser obligado a proporcionar datos sensibles
- Transferencia solo para fines directamente relacionados con el interes legitimo de ambas partes
- El titular puede ejercer derecho de acceso, rectificacion y supresion

**Fuente:** [Ley 25.326 - Argentina.gob.ar](https://www.argentina.gob.ar/justicia/derechofacil/leysimple/datos-personales)

## Ley 26.529 (Derechos del Paciente, Historia Clinica, Consentimiento Informado)

- Historia clinica unica por paciente (obligatorio)
- Consentimiento informado para todo acto medico
- Ley 27.706: programa federal de digitalizacion de historias clinicas

**Fuente:** [Ley 26.529 - Derechos del Paciente](https://www.argentina.gob.ar/normativa/nacional/ley-26529-160432)

## Ley 26.657 (Salud Mental)

- Proteccion especial para datos de salud mental
- Consentimiento informado como derecho fundamental
- Privacidad y confidencialidad reforzadas

**Fuente:** [Ley 26.657 - Salud Mental](https://www.argentina.gob.ar/normativa/nacional/ley-26657-175977)

## Implicaciones para el diseno del producto

1. **Consentimiento informado digital obligatorio** antes del primer registro de datos
2. **Datos cifrados** en reposo y en transito
3. **El paciente controla quien accede** y puede revocar en cualquier momento
4. **Registro de auditoria** de cada acceso profesional (quien, cuando, que vio)
5. **Politica de retencion y eliminacion** de datos clara y accesible
6. **No se pueden compartir datos sin consentimiento explicito** del paciente
7. **Derecho de supresion**: el paciente debe poder eliminar sus datos completamente

## Validacion legal pendiente

Antes de produccion se requiere validacion legal especifica sobre:
- Forma y contenido del consentimiento informado digital
- Periodo de retencion obligatorio vs derecho de supresion
- Requisitos de notificacion ante la AAIP (Agencia de Acceso a la Informacion Publica)
- Transferencia internacional de datos (si el hosting esta fuera de Argentina)
