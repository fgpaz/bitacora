#!/usr/bin/env node

import crypto from "node:crypto";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { readFile, writeFile } from "node:fs/promises";

const SCRIPT_DIR = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(SCRIPT_DIR, "..", "..");
const WIKI_ROOT = path.join(REPO_ROOT, ".docs", "wiki");

const SOURCE_DOCS = [
  "11_identidad_visual.md",
  "12_lineamientos_interfaz_visual.md",
  "13_voz_tono.md",
  "16_patrones_ui.md",
  "07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md"
];

export const DESIGN_PACK_PATHS = {
  base: path.join(SCRIPT_DIR, "DESIGN.md"),
  patient: path.join(SCRIPT_DIR, "DESIGN.patient.md"),
  professional: path.join(SCRIPT_DIR, "DESIGN.professional.md"),
  telegram: path.join(SCRIPT_DIR, "DESIGN.telegram.md"),
  manifest: path.join(SCRIPT_DIR, "DESIGN.manifest.json")
};

export const PROFILE_THEME_SPECS = {
  patient: {
    displayName: "Bitacora Patient Design System",
    theme: {
      colorMode: "LIGHT",
      headlineFont: "DM_SANS",
      bodyFont: "INTER",
      labelFont: "INTER",
      roundness: "ROUND_TWELVE",
      customColor: "#5E766E",
      colorVariant: "TONAL_SPOT",
      overridePrimaryColor: "#5E766E",
      overrideSecondaryColor: "#C8B7AE",
      overrideTertiaryColor: "#B67864",
      overrideNeutralColor: "#F6F1EA"
    }
  },
  professional: {
    displayName: "Bitacora Professional Design System",
    theme: {
      colorMode: "LIGHT",
      headlineFont: "DM_SANS",
      bodyFont: "INTER",
      labelFont: "INTER",
      roundness: "ROUND_EIGHT",
      customColor: "#5E766E",
      colorVariant: "NEUTRAL",
      overridePrimaryColor: "#5E766E",
      overrideSecondaryColor: "#C8B7AE",
      overrideTertiaryColor: "#B67864",
      overrideNeutralColor: "#F6F1EA"
    }
  },
  telegram: {
    displayName: "Bitacora Telegram Design System",
    theme: {
      colorMode: "LIGHT",
      headlineFont: "DM_SANS",
      bodyFont: "INTER",
      labelFont: "INTER",
      roundness: "ROUND_EIGHT",
      customColor: "#5E766E",
      colorVariant: "TONAL_SPOT",
      overridePrimaryColor: "#5E766E",
      overrideSecondaryColor: "#C8B7AE",
      overrideTertiaryColor: "#B67864",
      overrideNeutralColor: "#F6F1EA"
    }
  }
};

const PROFILE_PACK_KEYS = {
  patient: "patient",
  professional: "professional",
  telegram: "telegram"
};

function sha256(text) {
  return crypto.createHash("sha256").update(text).digest("hex");
}

function cleanInlineMarkdown(value) {
  return String(value || "")
    .replace(/\*\*/g, "")
    .replace(/`/g, "")
    .trim();
}

function escapeRegex(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function extractSection(markdown, heading) {
  const regex = new RegExp(`^##\\s+${escapeRegex(heading)}\\s*$([\\s\\S]*?)(?=^##\\s+|\\Z)`, "m");
  const match = markdown.match(regex);
  return match ? match[1].trim() : "";
}

function parseKeyValueBullets(section) {
  const rows = {};
  for (const line of section.split("\n")) {
    const match = line.match(/^- [`"]?([^:`"\n]+)[`"]?:\s*(.+)$/);
    if (!match) continue;
    rows[cleanInlineMarkdown(match[1])] = cleanInlineMarkdown(match[2]);
  }
  return rows;
}

function parseMarkdownTable(section) {
  const lines = section
    .split("\n")
    .map((line) => line.trim())
    .filter((line) => line.startsWith("|"));

  if (lines.length < 2) return [];

  const headers = lines[0]
    .split("|")
    .map((cell) => cell.trim())
    .filter(Boolean);

  const rows = [];
  for (const line of lines.slice(2)) {
    const values = line
      .split("|")
      .map((cell) => cell.trim())
      .filter(Boolean);

    if (values.length !== headers.length) continue;

    const row = {};
    for (let index = 0; index < headers.length; index += 1) {
      row[headers[index]] = cleanInlineMarkdown(values[index]);
    }
    rows.push(row);
  }

  return rows;
}

function renderTable(headers, rows) {
  const head = `| ${headers.join(" | ")} |`;
  const divider = `| ${headers.map(() => "---").join(" | ")} |`;
  const body = rows.map((row) => `| ${headers.map((header) => row[header] || "").join(" | ")} |`);
  return [head, divider, ...body].join("\n");
}

function renderList(items) {
  return items.map((item) => `- ${item}`).join("\n");
}

function renderSources() {
  return SOURCE_DOCS.map((doc) => `- \`.docs/wiki/${doc}\``).join("\n");
}

function buildBaseOverview(snapshot) {
  const decisions = snapshot.lineamientos;
  return [
    `Bitácora debe sentirse como **${decisions["primera impresión"]}** con tono **${decisions.tono}** y desempate constante **${decisions.desempate}**.`,
    `La historia que lidera es **${decisions["historia líder"]}**. La jerarquía visible debe seguir **${decisions.jerarquía}**, con **${decisions.interacción}** y señales de confianza **${decisions["confianza jurídica"]}**.`,
    `La interfaz nace **${decisions.responsive}**, mantiene **${decisions.densidad}**, trabaja estados **${decisions.estados}** y trata accesibilidad como **${decisions.accesibilidad}**.`,
    `La traducción a Stitch debe ser **${decisions["traducción Stitch"]}**. No se permiten reinterpretaciones que rompan la familia visual, la voz ni la lógica de control del producto.`
  ];
}

function buildColorRows(snapshot) {
  return snapshot.palette.map((row) => ({
    Token: row.Token,
    Dirección: row.Dirección,
    Uso: row.Uso
  }));
}

function buildTypographyRows(snapshot) {
  return snapshot.typography.map((row) => ({
    Rol: row.Rol,
    "Familia canónica": row.Familia,
    Uso: row.Uso
  }));
}

function buildComponentRows(snapshot) {
  return snapshot.componentGrammar.map((row) => ({
    Familia: row.Familia,
    Propósito: row.Propósito,
    "Estados mínimos": row["Estados mínimos"]
  }));
}

function buildDos(snapshot) {
  return [
    `Mantener una acción dominante y una secundaria silenciosa en la misma pantalla.`,
    `Usar aire generoso, profundidad baja y contenedores con suavidad contenida.`,
    `Hacer visibles los límites de acceso solo cuando el contexto lo requiere.`,
    `Conservar confirmaciones y vacíos en tono ${snapshot.voz["Vacíos y confirmaciones"] || "sereno y humano"}.`,
    `Mantener contenido visible de ejemplo ${snapshot.voz["Contenido visible de ejemplo en prototipos y contratos"] || "cotidiano y concreto"}.`,
    `Usar la gramática de componentes ya definida en el sistema frontend y los patrones seed del canon, sin inventar familias paralelas.`
  ];
}

function buildDonts() {
  return [
    "No parecer dashboard SaaS, consola de monitoreo, app de wellness ni interfaz hospitalaria fría.",
    "No usar celebraciones, elogios, confeti, rebotes, pulsos persistentes ni CTAs que griten.",
    "No dejar paneles permanentes de confianza o privacidad compitiendo con el contenido principal.",
    "No introducir más de dos zonas funcionales visibles en desktop profesional.",
    "No usar el pack derivado como nueva autoridad editorial: siempre manda el wiki."
  ];
}

function buildBaseMarkdown(snapshot) {
  const headers = ["Token", "Dirección", "Uso"];
  const typeHeaders = ["Rol", "Familia canónica", "Uso"];
  const componentHeaders = ["Familia", "Propósito", "Estados mínimos"];

  return [
    "# DESIGN.md",
    "",
    "> Archivo derivado y regenerable desde el wiki. No editar manualmente.",
    "> Autoridad canónica: `.docs/wiki/11`, `.docs/wiki/12`, `.docs/wiki/13`, `.docs/wiki/16`, `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`.",
    "",
    "## Overview",
    renderList(buildBaseOverview(snapshot)),
    "",
    "## Colors",
    renderTable(headers, buildColorRows(snapshot)),
    "",
    "## Typography",
    renderTable(typeHeaders, buildTypographyRows(snapshot)),
    "",
    "## Elevation",
    renderList([
      "Profundidad baja y silenciosa.",
      "Sombras mínimas y difusas; bordes sutiles antes que recortes duros.",
      "Superficies cálidas con textura editorial suave y brillo mínimo.",
      "El color acompaña la tarea; no debe convertirse en ruido dominante."
    ]),
    "",
    "## Components",
    renderTable(componentHeaders, buildComponentRows(snapshot)),
    "",
    "## Do's and Don'ts",
    "### Do",
    renderList(buildDos(snapshot)),
    "",
    "### Don't",
    renderList(buildDonts()),
    "",
    "## Source of Truth",
    renderSources(),
    ""
  ].join("\n");
}

function buildProfileMarkdown(baseMarkdown, profile, overrideLines) {
  const displayName = PROFILE_THEME_SPECS[profile].displayName;
  return [
    `# DESIGN.${profile}.md`,
    "",
    "> Archivo derivado y regenerable desde el wiki. No editar manualmente.",
    `> Perfil Stitch: \`${profile}\` -> ${displayName}.`,
    "",
    baseMarkdown.replace(/^# DESIGN\.md\s*/m, "").trim(),
    "",
    "## Profile Override",
    renderList(overrideLines),
    ""
  ].join("\n");
}

function buildProfileOverrides(snapshot) {
  return {
    patient: [
      "Una sola columna serena como default.",
      "El contenido personal lidera antes que la mecánica del sistema.",
      "Sin paneles persistentes de confianza, sin dashboard y sin scoring visual."
    ],
    professional: [
      `Mantener la misma familia visual con ${snapshot.lineamientos["superficie profesional"]}.`,
      "Desktop puede usar hasta dos zonas funcionales visibles: principal y apoyo secundario.",
      "La lectura clínica debe ser sobria y comparativa, pero nunca dura ni caótica."
    ],
    telegram: [
      "Un paso dominante por pantalla y mensajes muy breves.",
      "Códigos, expiración y siguiente acción deben verse sin ruido técnico.",
      "Mantener la misma familia visual base, comprimida para puente externo y continuidad conversacional."
    ]
  };
}

async function loadSnapshot() {
  const sources = {};
  for (const relativePath of SOURCE_DOCS) {
    const absolutePath = path.join(WIKI_ROOT, ...relativePath.split("/"));
    sources[relativePath] = await readFile(absolutePath, "utf8");
  }

  const lineamientos = parseKeyValueBullets(
    extractSection(sources["12_lineamientos_interfaz_visual.md"], "Decisiones globales cerradas")
  );
  const voz = parseKeyValueBullets(
    extractSection(sources["13_voz_tono.md"], "Decisiones cerradas de la capa global de experiencia")
  );

  return {
    sources,
    lineamientos,
    voz,
    palette: parseMarkdownTable(extractSection(sources["11_identidad_visual.md"], "Paleta base")),
    typography: parseMarkdownTable(extractSection(sources["11_identidad_visual.md"], "Tipografía")),
    componentGrammar: parseMarkdownTable(extractSection(sources["07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md"], "Gramática de componentes"))
  };
}

export function getDesignPackPath(profile) {
  const key = PROFILE_PACK_KEYS[profile] || PROFILE_PACK_KEYS.patient;
  return DESIGN_PACK_PATHS[key];
}

export function getDesignSystemSpec(profile) {
  return PROFILE_THEME_SPECS[profile] || PROFILE_THEME_SPECS.patient;
}

export async function buildDesignPack({ write = true } = {}) {
  const snapshot = await loadSnapshot();
  const baseMarkdown = buildBaseMarkdown(snapshot);
  const profileOverrides = buildProfileOverrides(snapshot);
  const outputs = {
    [DESIGN_PACK_PATHS.base]: baseMarkdown,
    [DESIGN_PACK_PATHS.patient]: buildProfileMarkdown(baseMarkdown, "patient", profileOverrides.patient),
    [DESIGN_PACK_PATHS.professional]: buildProfileMarkdown(baseMarkdown, "professional", profileOverrides.professional),
    [DESIGN_PACK_PATHS.telegram]: buildProfileMarkdown(baseMarkdown, "telegram", profileOverrides.telegram)
  };

  const sourceHashes = Object.fromEntries(
    Object.entries(snapshot.sources).map(([relativePath, content]) => [relativePath, sha256(content)])
  );

  const outputHashes = Object.fromEntries(
    Object.entries(outputs).map(([absolutePath, content]) => [path.basename(absolutePath), sha256(content)])
  );

  const manifest = {
    authority: "wiki-derived",
    generator: ".docs/stitch/design-pack.mjs",
    sourceFingerprint: sha256(JSON.stringify(sourceHashes)),
    sources: sourceHashes,
    outputs: outputHashes,
    profiles: Object.fromEntries(
      Object.entries(PROFILE_THEME_SPECS).map(([profile, spec]) => [
        profile,
        {
          displayName: spec.displayName,
          designPack: path.basename(getDesignPackPath(profile)),
          theme: spec.theme
        }
      ])
    )
  };

  if (write) {
    for (const [absolutePath, content] of Object.entries(outputs)) {
      await writeFile(absolutePath, `${content.trim()}\n`, "utf8");
    }
    await writeFile(DESIGN_PACK_PATHS.manifest, `${JSON.stringify(manifest, null, 2)}\n`, "utf8");
  }

  return {
    manifest,
    outputs,
    paths: DESIGN_PACK_PATHS
  };
}

if (import.meta.url === `file://${process.argv[1]}`) {
  buildDesignPack({ write: true })
    .then(({ manifest, paths }) => {
      console.log(JSON.stringify({
        sourceFingerprint: manifest.sourceFingerprint,
        outputs: Object.values(paths).map((filePath) => path.basename(filePath))
      }, null, 2));
    })
    .catch((error) => {
      console.error(error.message || error);
      process.exit(1);
    });
}
