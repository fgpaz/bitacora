# W5 — Tests + cierre

**Tipo:** mixed · **Subagente:** `ps-worker` (ops) + ejecución directa · **Duración estimada:** 20-30 min
**Salida:** closure update + traceability sync + no merge.

---

## T5A — Test suite

```bash
# Typecheck + lint (ya validados en cada wave, re-verificar al final)
npm --prefix frontend run typecheck
npm --prefix frontend run lint

# E2E: mínimo 10/10 verdes (8 existentes + 2 nuevos del 2026-04-23)
npm --prefix frontend run test:e2e
```

Si algún spec falla:
- Si es por selector que cambió por migración `UserFacingError` (error branch ahora tiene 2 `<p>`), actualizar selector del spec en el mismo commit de cierre.
- Si es por regresión funcional real, ROLLBACK de la wave afectada y re-abrir.

---

## T5B — Traceability sync

```bash
# ps-trazabilidad no existe como Skill/CLI directo en este repo. Se ejecuta via Skill("ps-trazabilidad").
# Si no esta disponible, sync manual validando que:
# - canon 23_uxui refleja commits del rediseño 2026-04-23 y del followups 2026-04-23.
# - 09_contratos_tecnicos.md no tiene endpoints nuevos (W4 no agrega endpoint, solo stub).
# - 05_modelo_datos.md sin cambios (sin schema).
```

Invocar `Skill("ps-trazabilidad")` tras W4 commit.

---

## T5C — Closure update

Crear `.docs/raw/reports/2026-04-23-login-flow-followups-closure.md` con shape similar al `2026-04-23-login-flow-redesign-closure.md`.

**Secciones obligatorias:**

1. Metadato (fecha, rama, base, HEAD, classification).
2. Cadena de commits (W1-W4).
3. Checkpoints intermedios (si los hubo).
4. Verdict por follow-up:
   - Follow-up #1 (Legal R-P1-3): `resuelto-sin-cambios-pending-formal-legal-opinion`.
   - Follow-up #2 (Canon 23_uxui): `resuelto` — deltas agregados.
   - Follow-up #3 (Analytics): `resuelto-stub` — endpoint futuro como TODO explícito.
   - Follow-up #4 (PatientPageShell strict): `resuelto`.
   - Follow-up #5 (Focus ring normalize): `resuelto`.
   - Follow-up #6 (global-error.spec): `postponed-P3` por decisión humana.
5. Verificaciones de cierre (governance, zonas congeladas, backend, deps, copy, tests, typecheck/lint, positivos previos).
6. Sync canon wiki (qué se tocó en `.docs/wiki/23_uxui/`).
7. Compliance health data (confirmar sin cambios en storage/access/consent-backend/audit).
8. Tests Playwright (status + conteo).
9. Estado del repo al cierre (rama, HEAD, commits, files touched).
10. Recomendación pre-merge + NO merge automático.
11. Follow-ups remanentes (global-error.spec P3 + legal formal opinion pending + endpoint analytics P2).

---

## T5D — Verificaciones finales

**Grep zonas congeladas sobre el diff completo:**
```bash
git diff --name-only main..HEAD | grep -E "^frontend/(lib/auth/|app/api/|app/auth/|proxy\.ts|src/)"
# esperado: 0
```

**Grep deps npm:**
```bash
git diff main..HEAD -- frontend/package.json frontend/package-lock.json
# esperado: vacio
```

**Grep copy congelado preservado:**
```bash
# Verificar que las 13 strings del closure 2026-04-23 §3 siguen intactas
for copy in "Ingresar" "Tu espacio personal de registro" "Solo vos ves lo que registrás. Tus datos son privados." "Registrar humor" "Empezá con tu primer registro" "+ Nuevo registro" "Check-in diario" "Registro guardado." "Tus últimos días" "Recibí recordatorios por Telegram" "Conectar" "Ahora no" "Nuevo registro"; do
  grep -r --include="*.tsx" --include="*.ts" -l "$copy" frontend/ || echo "MISSING: $copy"
done
```

---

## T5E — Commit closure + traceability

```bash
git add .docs/raw/reports/2026-04-23-login-flow-followups-closure.md

git commit -m "$(cat <<'EOF'
docs(closure): reporte de cierre followups login flow 2026-04-23

5/6 follow-ups cerrados (legal, canon, analytics, strict, focus ring).
6/6 si contamos global-error.spec como postponed-P3 aceptado.

- Legal R-P1-3: resuelto-sin-cambios-pending-formal-legal-opinion.
- Canon 23_uxui: deltas 2026-04-23 agregados a UXS/VOICE/UJ correspondientes.
- Analytics: stub console.info + 4 eventos instrumentados. 0 deps nuevas.
- PatientPageShell: prop error strict UserFacingError migrada + callsite unico actualizado.
- Focus ring: ProfessionalShell + SaveRail + outline none sueltos normalizados.
- global-error.spec: postponed P3 por decision humana (requiere infra playwright adicional).

10/10+ e2e verdes. typecheck + lint exit 0. 0 cruces zonas congeladas. 0 deps nuevas.

- Gabriel Paz -
EOF
)"
```

---

## T5F — Devolución al humano

Devolver:
- Path del closure.
- Conteo de commits (esperado: +5 docs + code).
- Verdict por follow-up.
- Próxima acción: PR humano + validación formal legal + decisión sobre endpoint /api/analytics + posible sesión P3 para global-error.spec.

**NO mergear a main sin OK humano.**
