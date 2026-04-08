# Estandares Clinicos — Escalas de Humor

## NIMH Life Chart Method (LCM-p)

El estandar clinico validado para charting longitudinal de trastornos afectivos.

- Escala original: **-4 a +4** (donde 0 = eutimia/estado normal)
- 4 niveles de severidad por polo: leve, moderado-bajo, moderado-alto, severo
- La severidad se basa en el **grado de deterioro funcional** asociado al humor
- Validado contra IDS-C (depresion) y YMRS (mania) — correlaciones altas
- Soporta estados mixtos (mania + depresion simultanea)
- Registro diario: humor, sueno, medicacion, eventos vitales

**Fuentes:**
- [NIMH-LCM-p Validation - PubMed](https://pubmed.ncbi.nlm.nih.gov/11097079/)
- [NIMH-LCM Self-Rating Validation - BMC Psychiatry](https://bmcpsychiatry.biomedcentral.com/articles/10.1186/1471-244X-14-130)

## Escala -3 a +3 (Simplificacion adoptada)

La escala -3 a +3 del proyecto es una simplificacion practica valida del NIMH-LCM:

- Elimina el nivel "severo" (-4/+4) que requiere evaluacion clinica
- 3 niveles de cada polo cubren leve/moderado/intenso — suficiente para auto-reporte
- Es mas accesible para pacientes sin entrenamiento clinico
- Compatible con el espiritu del NIMH-LCM-p

**Mapeo de niveles:**

| Valor | Polo | Intensidad |
|-------|------|-----------|
| +3 | Animo elevado | Intenso |
| +2 | Animo elevado | Moderado |
| +1 | Animo elevado | Leve |
| 0 | Normal / Eutimia | — |
| -1 | Animo deprimido | Leve |
| -2 | Animo deprimido | Moderado |
| -3 | Animo deprimido | Intenso |

## Otras escalas relevantes

- **PHQ-9** (Patient Health Questionnaire-9): cuestionario estandarizado de 9 items para depresion. Util como formulario configurable adicional (no MVP).
- **GAD-7** (Generalized Anxiety Disorder-7): cuestionario estandarizado de 7 items para ansiedad. Util como formulario configurable adicional (no MVP).
- **Escala 0-100 del NIMH** (50=equilibrio): alternativa lineal simple, menos granular.
- **Young Mania Rating Scale (YMRS)**: escala clinica administrada por profesional, no auto-reporte.

## Implicaciones para el diseno

- La escala -3..+3 esta clinicamente fundamentada y es la adoptada para el MVP.
- PHQ-9 y GAD-7 se reservan para la fase de formularios configurables (Fase 2).
- El NIMH-LCM valida el enfoque de charting longitudinal diario como metodo superior al registro en papel.
