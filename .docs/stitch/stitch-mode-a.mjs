#!/usr/bin/env node

import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StreamableHTTPClientTransport } from "@modelcontextprotocol/sdk/client/streamableHttp.js";
import { execFileSync, execSync } from "node:child_process";
import { mkdir, readFile, readdir, writeFile } from "node:fs/promises";
import { existsSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { parseArgs } from "node:util";

import {
  buildDesignPack,
  getDesignPackPath,
  getDesignSystemSpec
} from "./design-pack.mjs";

const STITCH_URL = "https://stitch.googleapis.com/mcp";
const SCRIPT_DIR = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(SCRIPT_DIR, "..", "..");
const STITCH_ROOT = path.join(os.homedir(), ".stitch-mcp");
const GCLOUD_WINDOWS = path.join(STITCH_ROOT, "google-cloud-sdk", "bin", "gcloud.cmd");
const GCLOUD_POSIX = path.join(STITCH_ROOT, "google-cloud-sdk", "bin", "gcloud");
const CONFIG_DEFAULT = path.join(STITCH_ROOT, "config", "configurations", "config_default");
const DEFAULT_PROMPTS_PATH = path.join(SCRIPT_DIR, "onb-001.prompts.json");
const DEFAULT_OUTPUT_ROOT = path.join(REPO_ROOT, "artifacts", "stitch");
const DEFAULT_AUDIT_PATH = path.join(SCRIPT_DIR, "STITCH-ARTIFACTS-AUDIT.md");
const DEFAULT_SLEEP_MS = 3000;
const GCLOUD_TIMEOUT_MS = 60000;

function nowStamp() {
  return new Date().toISOString().replace(/[:.]/g, "-");
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function outputJson(value) {
  console.log(JSON.stringify(value, null, 2));
}

function unique(values) {
  return [...new Set(values.filter(Boolean))];
}

function resolveGcloudPath() {
  if (process.platform === "win32" && existsSync(GCLOUD_WINDOWS)) return GCLOUD_WINDOWS;
  if (existsSync(GCLOUD_POSIX)) return GCLOUD_POSIX;
  return process.platform === "win32" ? "gcloud.cmd" : "gcloud";
}

async function readConfiguredProjectId() {
  if (process.env.STITCH_GCP_PROJECT) return process.env.STITCH_GCP_PROJECT;
  if (!existsSync(CONFIG_DEFAULT)) return "";
  const raw = await readFile(CONFIG_DEFAULT, "utf8");
  const match = raw.match(/^\s*project\s*=\s*(.+)\s*$/m);
  return match ? match[1].trim() : "";
}

function printAccessToken() {
  if (process.env.STITCH_ACCESS_TOKEN) return process.env.STITCH_ACCESS_TOKEN.trim();
  const gcloud = resolveGcloudPath();
  if (process.platform === "win32") {
    return execSync(`"${gcloud}" auth application-default print-access-token`, {
      encoding: "utf8",
      timeout: GCLOUD_TIMEOUT_MS,
      stdio: ["pipe", "pipe", "pipe"]
    }).trim();
  }
  return execFileSync(gcloud, ["auth", "application-default", "print-access-token"], {
    encoding: "utf8",
    timeout: GCLOUD_TIMEOUT_MS,
    stdio: ["pipe", "pipe", "pipe"]
  }).trim();
}

function resolveLocalPath(candidatePath) {
  if (!candidatePath) return "";
  if (path.isAbsolute(candidatePath)) return candidatePath;
  return path.join(SCRIPT_DIR, candidatePath);
}

function resolveConfigPath(configPath) {
  return resolveLocalPath(configPath) || DEFAULT_PROMPTS_PATH;
}

function inferOutputSlug(projectTitle) {
  return String(projectTitle || "prototype")
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "") || "prototype";
}

async function readConfigFile(configPath) {
  const resolved = resolveConfigPath(configPath);
  const raw = await readFile(resolved, "utf8");
  return { resolved, parsed: JSON.parse(raw) };
}

function resolvePromptDesignConfig(candidate, defaults = {}) {
  const designProfile = candidate.designProfile || defaults.designProfile || "patient";
  const designSpec = getDesignSystemSpec(designProfile);
  return {
    designProfile,
    designSystemDisplayName:
      candidate.designSystemDisplayName || defaults.designSystemDisplayName || designSpec.displayName,
    designPackPath: candidate.designPackPath || defaults.designPackPath || ""
  };
}

async function loadPromptsConfig(configPath, sliceId) {
  const { resolved, parsed } = await readConfigFile(configPath);

  if (parsed.slices) {
    if (!sliceId) {
      throw new Error(`Config ${path.basename(resolved)} requires --slice <id>.`);
    }

    const slice = parsed.slices[sliceId];
    if (!slice) {
      const available = Object.keys(parsed.slices).sort().join(", ");
      throw new Error(`Slice "${sliceId}" not found in ${path.basename(resolved)}. Available: ${available}`);
    }

    const defaults = parsed.defaults || {};
    const designConfig = resolvePromptDesignConfig(slice, defaults);
    return {
      configPath: resolved,
      sliceId,
      projectTitle: slice.projectTitle || defaults.projectTitle || sliceId,
      outputSlug: slice.outputSlug || defaults.outputSlug || sliceId,
      deviceType: slice.deviceType || defaults.deviceType,
      modelId: slice.modelId || defaults.modelId,
      presets: slice.presets || {},
      screens: slice.screens || {},
      style: [defaults.style, slice.style, slice.styleAppend].filter(Boolean).join("\n\n"),
      ...designConfig
    };
  }

  const designConfig = resolvePromptDesignConfig(parsed);
  const syntheticSliceId = parsed.outputSlug || inferOutputSlug(parsed.projectTitle);

  return {
    ...parsed,
    configPath: resolved,
    sliceId: sliceId || syntheticSliceId,
    outputSlug: parsed.outputSlug || syntheticSliceId,
    ...designConfig
  };
}

function getTextBlocks(result) {
  return (result.content ?? []).filter((item) => item.type === "text").map((item) => item.text);
}

function getImageBlocks(result) {
  return (result.content ?? []).filter((item) => item.type === "image" && item.data);
}

function getGeneratedScreens(result) {
  return result?.structuredContent?.outputComponents?.flatMap((component) => component?.design?.screens ?? []) ?? [];
}

function getGeneratedScreenInstances(result) {
  return getGeneratedScreens(result)
    .map((screen) => ({
      id: normalizeScreenInstanceId(screen.id || screen.name),
      sourceScreen: normalizeSourceScreen(screen.name || screen.sourceScreen)
    }))
    .filter((screen) => screen.id && screen.sourceScreen);
}

function extractJsonCandidate(raw) {
  const trimmed = raw.trim();
  if (!trimmed) return "";
  if (trimmed.startsWith("{") || trimmed.startsWith("[")) return trimmed;

  const firstObject = trimmed.indexOf("{");
  const firstArray = trimmed.indexOf("[");
  const firstIndex = [firstObject, firstArray].filter((index) => index >= 0).sort((left, right) => left - right)[0];
  if (firstIndex === undefined) return "";

  const objectTail = trimmed.lastIndexOf("}");
  const arrayTail = trimmed.lastIndexOf("]");
  const lastIndex = Math.max(objectTail, arrayTail);
  if (lastIndex < firstIndex) return "";

  return trimmed.slice(firstIndex, lastIndex + 1);
}

function parseJsonFromTextBlocks(textBlocks) {
  const candidates = unique([textBlocks.join("\n"), ...textBlocks]);
  for (const candidate of candidates) {
    const jsonCandidate = extractJsonCandidate(candidate);
    if (!jsonCandidate) continue;
    try {
      return JSON.parse(jsonCandidate);
    } catch {}
  }
  return null;
}

async function downloadToFile(url, targetPath) {
  if (!url) return false;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Download failed (${response.status}) for ${url}`);
  }
  const bytes = Buffer.from(await response.arrayBuffer());
  await writeFile(targetPath, bytes);
  return true;
}

function parseProjects(textBlocks) {
  const parsed = parseJsonFromTextBlocks(textBlocks);
  if (Array.isArray(parsed)) return parsed;
  if (Array.isArray(parsed?.projects)) return parsed.projects;
  return [];
}

function parseAssets(textBlocks) {
  const parsed = parseJsonFromTextBlocks(textBlocks);
  if (Array.isArray(parsed)) return parsed;
  if (Array.isArray(parsed?.assets)) return parsed.assets;
  if (Array.isArray(parsed?.designSystems)) return parsed.designSystems;
  return [];
}

function parseProjectResource(textBlocks) {
  const parsed = parseJsonFromTextBlocks(textBlocks);
  if (!parsed) return null;
  return parsed.project || parsed;
}

function projectTitleOf(project) {
  return project.title || project.displayName || project.name || "";
}

function projectIdOf(project) {
  const raw = project.name || project.id || "";
  const match = String(raw).match(/projects\/(.+)$/);
  return match ? match[1] : String(raw);
}

function extractProjectId(textBlocks) {
  const combined = textBlocks.join("\n");
  const match = combined.match(/projects\/([A-Za-z0-9_-]+)/);
  return match ? match[1] : "";
}

function assetDisplayNameOf(asset) {
  return asset.displayName || asset.title || asset.name || "";
}

function assetIdOf(asset) {
  const raw = asset.name || asset.id || "";
  const match = String(raw).match(/assets\/(.+)$/);
  return match ? match[1] : String(raw);
}

function extractAssetId(textBlocks) {
  const combined = textBlocks.join("\n");
  const match = combined.match(/assets\/([A-Za-z0-9_-]+)/);
  return match ? match[1] : "";
}

function normalizeAssetResourceName(nameOrId) {
  const value = String(nameOrId || "").trim();
  if (!value) return "";
  if (value.startsWith("assets/")) return value;
  return `assets/${value}`;
}

function normalizeAssetId(nameOrId) {
  return String(nameOrId || "").replace(/^assets\//, "").trim();
}

function normalizeScreenInstanceId(value) {
  const raw = String(value || "").trim();
  if (!raw) return "";
  if (raw.includes("/screens/") && !raw.includes("/screenInstances/")) return "";
  if (raw.includes("/screenInstances/")) {
    const match = raw.match(/screenInstances\/([^/]+)$/);
    return match ? match[1] : "";
  }
  if (raw.startsWith("screenInstances/")) {
    return raw.replace(/^screenInstances\//, "");
  }
  if (/^[A-Za-z0-9_-]+$/.test(raw)) return raw;
  return "";
}

function normalizeSourceScreen(value) {
  if (!value) return "";
  if (typeof value === "string") {
    return value.includes("/screens/") ? value : "";
  }
  if (typeof value === "object") {
    return normalizeSourceScreen(value.name || value.sourceScreen || value.sourceScreenName);
  }
  return "";
}

function extractSelectedScreenInstances(projectResource) {
  const selected = [];
  const seen = new Set();

  function visit(node) {
    if (!node || typeof node !== "object") return;
    if (Array.isArray(node)) {
      node.forEach(visit);
      return;
    }

    const sourceScreen = normalizeSourceScreen(
      node.sourceScreen || node.sourceScreenName || node.source_screen || node.screen || node.screenRef
    );
    const instanceId = normalizeScreenInstanceId(node.id || node.instanceId || node.screenInstanceId || node.name);
    if (sourceScreen && instanceId) {
      const key = `${instanceId}:${sourceScreen}`;
      if (!seen.has(key)) {
        seen.add(key);
        selected.push({ id: instanceId, sourceScreen });
      }
    }

    for (const value of Object.values(node)) {
      visit(value);
    }
  }

  visit(projectResource);
  return selected;
}

function normalizeScreenIds(requested, promptsConfig) {
  if (requested.length > 0) return requested;
  return promptsConfig.presets.core || [];
}

function buildPrompt(promptsConfig, screenId) {
  const screen = promptsConfig.screens[screenId];
  if (!screen) {
    throw new Error(`Screen prompt not found: ${screenId}`);
  }

  const preface = [
    "Use the existing project design system as the visual authority.",
    `Active profile: ${promptsConfig.designProfile}.`,
    "All visible UI text must be in Spanish unless the screen prompt explicitly asks for another language.",
    "If the prompt quotes approved microcopy, preserve that copy verbatim.",
    "Do not add extra primary actions, privacy panels or trust blocks unless the screen prompt explicitly asks for them.",
    "Do not add inspirational quotes, slogans, testimonials, ornamental helper text, bottom navigation, menus or profile tabs unless the screen prompt explicitly asks for them.",
    "Keep helper copy compact and local to the task. Avoid mentioning professionals, encryption or privacy unless the current screen is the sensitive context that requires it.",
    "Do not invent a new color system, font family, component grammar or product mood."
  ].join(" ");

  return [
    preface,
    promptsConfig.style ? `Slice-specific supplement:\n${promptsConfig.style}` : "",
    `Screen goal: ${screen.goal}`,
    screen.prompt
  ].filter(Boolean).join("\n\n");
}

async function connectClient() {
  const projectId = await readConfiguredProjectId();
  if (!projectId) {
    throw new Error("No GCP project configured in ~/.stitch-mcp/config/configurations/config_default or STITCH_GCP_PROJECT.");
  }

  const token = printAccessToken();
  if (!token) {
    throw new Error("Could not resolve OAuth access token.");
  }

  const transport = new StreamableHTTPClientTransport(
    new URL(STITCH_URL),
    {
      requestInit: {
        headers: {
          Authorization: `Bearer ${token}`,
          "x-goog-user-project": projectId
        }
      }
    }
  );

  const client = new Client({ name: "bitacora-stitch-mode-a", version: "0.2.0" }, { capabilities: {} });
  await client.connect(transport);

  return { client, projectId, token, transport };
}

async function callTool(client, name, args) {
  return client.callTool({ name, arguments: args });
}

async function ensureProjectId(client, requestedProjectId, requestedTitle) {
  if (requestedProjectId) return requestedProjectId;

  const listResult = await callTool(client, "list_projects", {});
  const projects = parseProjects(getTextBlocks(listResult));
  const targetTitle = requestedTitle.trim().toLowerCase();
  const existing = projects.find((project) => projectTitleOf(project).toLowerCase().includes(targetTitle));

  if (existing) return projectIdOf(existing);

  const createResult = await callTool(client, "create_project", { title: requestedTitle });
  const createdId = extractProjectId(getTextBlocks(createResult));
  if (!createdId) {
    throw new Error(`Could not resolve project id from create_project response for "${requestedTitle}".`);
  }

  return createdId;
}

async function getProjectResource(client, projectId) {
  const result = await callTool(client, "get_project", {
    name: `projects/${projectId}`
  });
  const textBlocks = getTextBlocks(result);
  return {
    raw: result,
    textBlocks,
    resource: parseProjectResource(textBlocks)
  };
}

async function ensureDesignSystem(client, targetProjectId, promptsConfig) {
  await buildDesignPack({ write: true });

  const designProfile = promptsConfig.designProfile || "patient";
  const designSpec = getDesignSystemSpec(designProfile);
  const designPackPath = promptsConfig.designPackPath
    ? resolveLocalPath(promptsConfig.designPackPath)
    : getDesignPackPath(designProfile);
  const designMd = await readFile(designPackPath, "utf8");
  const displayName = promptsConfig.designSystemDisplayName || designSpec.displayName;

  const designSystemPayload = {
    displayName,
    theme: {
      ...designSpec.theme,
      designMd
    }
  };

  const listResult = await callTool(client, "list_design_systems", { projectId: targetProjectId });
  const assets = parseAssets(getTextBlocks(listResult));
  const existing = assets.find(
    (asset) => assetDisplayNameOf(asset).trim().toLowerCase() === displayName.trim().toLowerCase()
  );

  let operation = "created";
  let result;
  let assetId = "";
  if (existing) {
    operation = "updated";
    assetId = assetIdOf(existing);
    result = await callTool(client, "update_design_system", {
      name: normalizeAssetResourceName(existing.name || assetId),
      projectId: targetProjectId,
      designSystem: designSystemPayload
    });
  } else {
    result = await callTool(client, "create_design_system", {
      projectId: targetProjectId,
      designSystem: designSystemPayload
    });
    assetId = extractAssetId(getTextBlocks(result));
  }

  if (!assetId) {
    assetId = extractAssetId(getTextBlocks(result));
  }

  return {
    operation,
    assetId: normalizeAssetId(assetId),
    displayName,
    designProfile,
    designPackPath,
    theme: designSpec.theme
  };
}

async function applyDesignSystemToProject(client, targetProjectId, assetId, options = {}) {
  if (!assetId) {
    throw new Error("Cannot apply a design system without an asset id.");
  }

  const selectedScreenInstances = options.selectedScreenInstances?.length
    ? options.selectedScreenInstances
    : [];
  let projectSnapshot = null;

  if (selectedScreenInstances.length === 0) {
    projectSnapshot = await getProjectResource(client, targetProjectId);
    selectedScreenInstances.push(...extractSelectedScreenInstances(projectSnapshot.resource));
  }

  if (selectedScreenInstances.length === 0) {
    throw new Error("Could not resolve selectedScreenInstances for apply_design_system.");
  }

  const result = await callTool(client, "apply_design_system", {
    projectId: targetProjectId,
    selectedScreenInstances,
    assetId: normalizeAssetId(assetId)
  });

  return {
    assetId: normalizeAssetId(assetId),
    selectedScreenInstances,
    projectSnapshot,
    result
  };
}

async function commandListSlices(configPath) {
  const { resolved, parsed } = await readConfigFile(configPath);
  outputJson({
    configPath: resolved,
    slices: Object.keys(parsed.slices || {}).sort()
  });
}

async function commandCheckAuth() {
  const configuredProject = await readConfiguredProjectId();
  const gcloud = resolveGcloudPath();
  const token = printAccessToken();

  outputJson({
    stitchRoot: STITCH_ROOT,
    gcloud,
    configDefaultExists: existsSync(CONFIG_DEFAULT),
    configuredProject,
    accessTokenResolved: Boolean(token),
    accessTokenPreview: token.slice(0, 20)
  });
}

async function commandBuildDesignPack() {
  const { manifest } = await buildDesignPack({ write: true });
  outputJson({
    sourceFingerprint: manifest.sourceFingerprint,
    outputs: Object.keys(manifest.outputs)
  });
}

async function commandListProjects() {
  const { client, projectId } = await connectClient();
  try {
    const result = await callTool(client, "list_projects", {});
    outputJson({
      quotaProject: projectId,
      response: getTextBlocks(result)
    });
  } finally {
    await client.close().catch(() => {});
  }
}

async function commandListScreens(projectId) {
  if (!projectId) throw new Error("--project is required for list-screens.");
  const { client, projectId: quotaProject } = await connectClient();
  try {
    const result = await callTool(client, "list_screens", { projectId });
    outputJson({
      quotaProject,
      targetProject: projectId,
      response: getTextBlocks(result)
    });
  } finally {
    await client.close().catch(() => {});
  }
}

async function commandSyncDesignSystem(options) {
  const promptsConfig = await loadPromptsConfig(options.config || "", options.slice || "");
  const { client, projectId: quotaProject } = await connectClient();
  const projectTitle = options.title || promptsConfig.projectTitle;

  try {
    const targetProjectId = await ensureProjectId(client, options.project || "", projectTitle);
    const designSystem = await ensureDesignSystem(client, targetProjectId, promptsConfig);
    outputJson({
      quotaProject,
      targetProjectId,
      projectTitle,
      designSystem
    });
  } finally {
    await client.close().catch(() => {});
  }
}

async function commandApplyDesignSystem(options) {
  const { client, projectId: quotaProject } = await connectClient();
  let promptsConfig = null;
  let projectTitle = "";

  try {
    if (!options.asset) {
      promptsConfig = await loadPromptsConfig(options.config || "", options.slice || "");
      projectTitle = options.title || promptsConfig.projectTitle;
    }

    const targetProjectId = promptsConfig
      ? await ensureProjectId(client, options.project || "", projectTitle)
      : (options.project || "");

    if (!targetProjectId) {
      throw new Error("--project is required when --asset is used without --config.");
    }

    const designSystem = promptsConfig
      ? await ensureDesignSystem(client, targetProjectId, promptsConfig)
      : { assetId: normalizeAssetId(options.asset) };

    const applied = await applyDesignSystemToProject(client, targetProjectId, designSystem.assetId);
    outputJson({
      quotaProject,
      targetProjectId,
      assetId: designSystem.assetId,
      appliedTo: applied.selectedScreenInstances.length
    });
  } finally {
    await client.close().catch(() => {});
  }
}

async function commandGeneratePrototype(options, requestedScreens) {
  const promptsConfig = await loadPromptsConfig(options.config || "", options.slice || "");
  const { client, projectId: quotaProject } = await connectClient();
  const projectTitle = options.title || promptsConfig.projectTitle;
  const modelId = options.model || promptsConfig.modelId;
  const deviceType = options.device || promptsConfig.deviceType;
  const outputRoot = options.out
    ? path.resolve(options.out)
    : path.join(DEFAULT_OUTPUT_ROOT, promptsConfig.outputSlug || inferOutputSlug(projectTitle));

  try {
    const targetProjectId = await ensureProjectId(client, options.project || "", projectTitle);
    const screenIds = options.preset
      ? (promptsConfig.presets[options.preset] || [])
      : normalizeScreenIds(requestedScreens, promptsConfig);

    if (screenIds.length === 0) {
      throw new Error("No screens selected for generation.");
    }

    const runDir = path.join(outputRoot, nowStamp());
    await mkdir(runDir, { recursive: true });

    const designSystem = options.skipDesignSync
      ? null
      : await ensureDesignSystem(client, targetProjectId, promptsConfig);

    await writeFile(path.join(runDir, "run.json"), JSON.stringify({
      configPath: promptsConfig.configPath,
      sliceId: promptsConfig.sliceId,
      quotaProject,
      targetProjectId,
      projectTitle,
      modelId,
      deviceType,
      screenIds,
      designProfile: promptsConfig.designProfile,
      designSystemDisplayName: promptsConfig.designSystemDisplayName,
      designPackPath: designSystem?.designPackPath || getDesignPackPath(promptsConfig.designProfile),
      designSystemAssetId: designSystem?.assetId || ""
    }, null, 2));

    const failures = [];
    const generatedScreenInstances = [];
    for (let index = 0; index < screenIds.length; index += 1) {
      const screenId = screenIds[index];
      const prompt = buildPrompt(promptsConfig, screenId);
      try {
        const result = await callTool(client, "generate_screen_from_text", {
          projectId: targetProjectId,
          deviceType,
          modelId,
          prompt
        });

        await writeFile(path.join(runDir, `${screenId}.result.json`), JSON.stringify(result, null, 2));

        const textBlocks = getTextBlocks(result);
        if (result?.isError) {
          const message = textBlocks.join("\n").trim() || "Stitch returned isError=true.";
          failures.push({ screenId, error: message });
          await writeFile(path.join(runDir, `${screenId}.error.json`), JSON.stringify({
            screenId,
            error: message
          }, null, 2));
          continue;
        }

        const generatedScreen = getGeneratedScreens(result)[0];
        const images = getImageBlocks(result);
        const hasDownloadableOutput = Boolean(
          generatedScreen?.htmlCode?.downloadUrl ||
          generatedScreen?.screenshot?.downloadUrl ||
          images.length > 0
        );

        if (!generatedScreen || !hasDownloadableOutput) {
          const message = textBlocks.join("\n").trim() || "Stitch did not return a downloadable screen artifact.";
          failures.push({ screenId, error: message });
          await writeFile(path.join(runDir, `${screenId}.error.json`), JSON.stringify({
            screenId,
            error: message
          }, null, 2));
          continue;
        }

        generatedScreenInstances.push(...getGeneratedScreenInstances(result));
        if (generatedScreen?.htmlCode?.downloadUrl) {
          await downloadToFile(generatedScreen.htmlCode.downloadUrl, path.join(runDir, `${screenId}.html`));
        }
        if (generatedScreen?.screenshot?.downloadUrl) {
          await downloadToFile(generatedScreen.screenshot.downloadUrl, path.join(runDir, `${screenId}.png`));
        }

        for (let imageIndex = 0; imageIndex < images.length; imageIndex += 1) {
          const image = images[imageIndex];
          const extension = image.mimeType === "image/jpeg" ? "jpg" : "png";
          const buffer = Buffer.from(image.data, "base64");
          await writeFile(path.join(runDir, `${screenId}${imageIndex === 0 ? "" : `-${imageIndex}`}.${extension}`), buffer);
        }
      } catch (error) {
        failures.push({ screenId, error: error.message || String(error) });
        await writeFile(path.join(runDir, `${screenId}.error.json`), JSON.stringify({
          screenId,
          error: error.message || String(error)
        }, null, 2));
      }

      if (index < screenIds.length - 1) {
        await sleep(DEFAULT_SLEEP_MS);
      }
    }

    let applyDesignSystem = null;
    if (designSystem?.assetId) {
      try {
        const applied = await applyDesignSystemToProject(client, targetProjectId, designSystem.assetId, {
          selectedScreenInstances: unique(
            generatedScreenInstances.map((screen) => `${screen.id}|${screen.sourceScreen}`)
          ).map((key) => {
            const [id, sourceScreen] = key.split("|");
            return { id, sourceScreen };
          })
        });
        applyDesignSystem = {
          assetId: designSystem.assetId,
          appliedTo: applied.selectedScreenInstances.length,
          source: applied.projectSnapshot ? "project-snapshot" : "generated-results"
        };
        if (applied.projectSnapshot?.resource) {
          await writeFile(path.join(runDir, "project.json"), JSON.stringify(applied.projectSnapshot.resource, null, 2));
        }
        await writeFile(path.join(runDir, "apply-design-system.json"), JSON.stringify({
          assetId: designSystem.assetId,
          selectedScreenInstances: applied.selectedScreenInstances,
          source: applied.projectSnapshot ? "project-snapshot" : "generated-results"
        }, null, 2));
      } catch (error) {
        const message = error.message || String(error);
        if (generatedScreenInstances.length === 0 && message.includes("selectedScreenInstances")) {
          applyDesignSystem = {
            assetId: designSystem.assetId,
            skipped: true,
            reason: "No selectedScreenInstances available from generated screens or project snapshot."
          };
          await writeFile(path.join(runDir, "apply-design-system.skipped.json"), JSON.stringify(applyDesignSystem, null, 2));
        } else {
          applyDesignSystem = {
            assetId: designSystem.assetId,
            error: message
          };
          await writeFile(path.join(runDir, "apply-design-system.error.json"), JSON.stringify(applyDesignSystem, null, 2));
        }
      }
    }

    const screensResult = await callTool(client, "list_screens", { projectId: targetProjectId });
    await writeFile(path.join(runDir, "screens.json"), JSON.stringify(getTextBlocks(screensResult), null, 2));

    outputJson({
      quotaProject,
      targetProjectId,
      projectTitle,
      modelId,
      deviceType,
      runDir,
      screenIds,
      designSystem,
      applyDesignSystem,
      failures
    });
  } finally {
    await client.close().catch(() => {});
  }
}

async function collectPromptEntries() {
  const promptFiles = (await readdir(SCRIPT_DIR))
    .filter((fileName) => fileName.endsWith(".prompts.json"))
    .sort();

  const entries = [];
  for (const fileName of promptFiles) {
    const { parsed } = await readConfigFile(fileName);
    if (parsed.slices) {
      const defaults = parsed.defaults || {};
      for (const sliceId of Object.keys(parsed.slices).sort()) {
        const slice = parsed.slices[sliceId];
        const designConfig = resolvePromptDesignConfig(slice, defaults);
        entries.push({
          sliceId,
          configFile: fileName,
          outputSlug: slice.outputSlug || sliceId,
          projectTitle: slice.projectTitle || defaults.projectTitle || sliceId,
          presets: slice.presets || {},
          ...designConfig
        });
      }
      continue;
    }

    const designConfig = resolvePromptDesignConfig(parsed);
    entries.push({
      sliceId: parsed.outputSlug || inferOutputSlug(parsed.projectTitle || fileName.replace(/\.prompts\.json$/, "")),
      configFile: fileName,
      outputSlug: parsed.outputSlug || inferOutputSlug(parsed.projectTitle),
      projectTitle: parsed.projectTitle || fileName.replace(/\.prompts\.json$/, ""),
      presets: parsed.presets || {},
      ...designConfig
    });
  }

  return entries;
}

function classifyArtifactCoverage(entry, inspection) {
  if (!inspection.exists) {
    return {
      status: "missing",
      recommendation: "Generar primera corrida con design pack derivado.",
      gap: "No hay artifacts del slice."
    };
  }

  const coreExpected = unique(entry.presets.core || []);
  const fullExpected = unique((entry.presets.full || []).length > 0 ? entry.presets.full : coreExpected);
  const coreCovered = coreExpected.filter((screenId) => inspection.successfulScreens.includes(screenId));
  const fullCovered = fullExpected.filter((screenId) => inspection.successfulScreens.includes(screenId));
  const legacy = inspection.latestRunMetadata?.designPackPath ? "pack-synced" : "legacy-pre-design-pack";

  if (fullExpected.length > 0 && fullCovered.length === fullExpected.length) {
    return {
      status: "full-candidate",
      recommendation: legacy === "pack-synced"
        ? "Listo para auditoría visual manual."
        : "Reejecutar bajo design pack derivado antes de auditar.",
      gap: legacy === "pack-synced"
        ? "Sin gap de cobertura visible."
        : "La última corrida no registra design pack derivado."
    };
  }

  if (coreExpected.length > 0 && coreCovered.length === coreExpected.length) {
    return {
      status: "core-only",
      recommendation: "Completar estados faltantes antes de abrir UI-RFC.",
      gap: `Cobertura parcial: ${fullCovered.length}/${fullExpected.length} estados full.`
    };
  }

  return {
    status: "partial",
    recommendation: "Reejecutar el slice y completar estados faltantes.",
    gap: `Cobertura insuficiente: ${fullCovered.length}/${fullExpected.length} estados full.`
  };
}

async function inspectArtifactDirectory(outputSlug) {
  const directory = path.join(DEFAULT_OUTPUT_ROOT, outputSlug);
  if (!existsSync(directory)) {
    return {
      exists: false,
      runs: [],
      successfulScreens: [],
      errorScreens: [],
      latestRunMetadata: null
    };
  }

  const runs = (await readdir(directory, { withFileTypes: true }))
    .filter((entry) => entry.isDirectory())
    .map((entry) => entry.name)
    .sort();

  const successfulScreens = new Set();
  const errorScreens = new Set();
  const artifactScreens = new Set();
  let latestRunMetadata = null;

  for (const runName of runs) {
    const runDir = path.join(directory, runName);
    const files = await readdir(runDir);

    for (const fileName of files) {
      const successMatch = fileName.match(/^(.+)\.result\.json$/);
      if (successMatch) successfulScreens.add(successMatch[1]);

      const errorMatch = fileName.match(/^(.+)\.error\.json$/);
      if (errorMatch) errorScreens.add(errorMatch[1]);

      const artifactMatch = fileName.match(/^(.+?)(?:-\d+)?\.(html|png|jpg|jpeg)$/i);
      if (artifactMatch) artifactScreens.add(artifactMatch[1]);
    }

    if (runName === runs[runs.length - 1] && files.includes("run.json")) {
      try {
        latestRunMetadata = JSON.parse(await readFile(path.join(runDir, "run.json"), "utf8"));
      } catch {
        latestRunMetadata = null;
      }
    }
  }

  return {
    exists: true,
    runs,
    successfulScreens: [...successfulScreens]
      .filter((screenId) => artifactScreens.has(screenId) && !errorScreens.has(screenId))
      .sort(),
    errorScreens: [...errorScreens].sort(),
    latestRunMetadata
  };
}

function renderMarkdownTable(headers, rows) {
  const head = `| ${headers.join(" | ")} |`;
  const divider = `| ${headers.map(() => "---").join(" | ")} |`;
  const body = rows.map((row) => `| ${headers.map((header) => row[header] || "").join(" | ")} |`);
  return [head, divider, ...body].join("\n");
}

async function commandAuditArtifacts() {
  const entries = await collectPromptEntries();
  await buildDesignPack({ write: true });

  const rows = [];
  for (const entry of entries) {
    const inspection = await inspectArtifactDirectory(entry.outputSlug);
    const coverage = classifyArtifactCoverage(entry, inspection);
    const fullExpected = unique((entry.presets.full || []).length > 0 ? entry.presets.full : entry.presets.core || []);
    const fullCovered = fullExpected.filter((screenId) => inspection.successfulScreens.includes(screenId));
    const latestRunState = inspection.latestRunMetadata?.designPackPath
      ? path.basename(inspection.latestRunMetadata.designPackPath)
      : "legacy-pre-design-pack";

    rows.push({
      Slice: entry.sliceId,
      Config: entry.configFile,
      Perfil: entry.designProfile,
      "Design pack": latestRunState,
      Artifacts: inspection.exists ? `${inspection.runs.length} run(s)` : "0 run(s)",
      Cobertura: `${fullCovered.length}/${fullExpected.length}`,
      Estado: coverage.status,
      Gap: coverage.gap,
      Fallback: "acotado",
      Recomendación: coverage.recommendation
    });
  }

  const markdown = [
    "# STITCH-ARTIFACTS-AUDIT",
    "",
    "> Archivo derivado. Resume el estado estático de los artifacts Stitch y la preparación de cada slice frente al gate `strict Stitch only`.",
    "",
    renderMarkdownTable(
      ["Slice", "Config", "Perfil", "Design pack", "Artifacts", "Cobertura", "Estado", "Gap", "Fallback", "Recomendación"],
      rows
    ),
    ""
  ].join("\n");

  await writeFile(DEFAULT_AUDIT_PATH, markdown, "utf8");
  outputJson({
    auditPath: DEFAULT_AUDIT_PATH,
    slices: rows.length
  });
}

async function main() {
  const { positionals, values } = parseArgs({
    args: process.argv.slice(2),
    allowPositionals: true,
    options: {
      project: { type: "string" },
      title: { type: "string" },
      preset: { type: "string" },
      model: { type: "string" },
      device: { type: "string" },
      out: { type: "string" },
      config: { type: "string" },
      slice: { type: "string" },
      asset: { type: "string" },
      skipDesignSync: { type: "boolean", default: false }
    }
  });

  const [command = "help", ...rest] = positionals;

  if (command === "build-design-pack") {
    await commandBuildDesignPack();
    return;
  }

  if (command === "check-auth") {
    await commandCheckAuth();
    return;
  }

  if (command === "list-projects") {
    await commandListProjects();
    return;
  }

  if (command === "list-screens") {
    await commandListScreens(values.project || rest[0] || "");
    return;
  }

  if (command === "list-slices") {
    await commandListSlices(values.config || "");
    return;
  }

  if (command === "sync-design-system") {
    await commandSyncDesignSystem(values);
    return;
  }

  if (command === "apply-design-system") {
    await commandApplyDesignSystem(values);
    return;
  }

  if (command === "audit-artifacts") {
    await commandAuditArtifacts();
    return;
  }

  if (command === "generate-prototype") {
    await commandGeneratePrototype(values, rest);
    return;
  }

  if (command === "generate-onb-001") {
    await commandGeneratePrototype({
      ...values,
      config: values.config || "onb-001.prompts.json"
    }, rest);
    return;
  }

  console.log([
    "Usage:",
    "  node stitch-mode-a.mjs build-design-pack",
    "  node stitch-mode-a.mjs check-auth",
    "  node stitch-mode-a.mjs list-projects",
    "  node stitch-mode-a.mjs list-screens --project <id>",
    "  node stitch-mode-a.mjs list-slices --config <file>",
    "  node stitch-mode-a.mjs sync-design-system --config <file> [--slice <id>] [--project <id>]",
    "  node stitch-mode-a.mjs apply-design-system --config <file> [--slice <id>] [--project <id>]",
    "  node stitch-mode-a.mjs audit-artifacts",
    "  node stitch-mode-a.mjs generate-prototype --config <file> [--slice <id>] [--preset core|full] [--project <id>]",
    "  node stitch-mode-a.mjs generate-onb-001 [--preset core|full] [--project <id>] [--title <project title>]",
    "  node stitch-mode-a.mjs generate-onb-001 s01-entry s03-default s04-mood"
  ].join("\n"));
}

main().catch((error) => {
  console.error(error.message || error);
  process.exit(1);
});
