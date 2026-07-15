using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryFallbackSurfaceReport
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string activeScene;
        public int activeRendererCount;
        public int materialSlotCount;
        public List<RecoveryFallbackSurfaceFinding> findings =
            new List<RecoveryFallbackSurfaceFinding>();
    }

    [Serializable]
    public sealed class RecoveryFallbackSurfaceFinding
    {
        public string code;
        public string severity = "BLOCKER";
        public string rendererPath;
        public string scene;
        public int materialIndex;
        public string materialName;
        public string shaderName;
        public string message;
    }

    public sealed class RecoveryFallbackSurfaceArtifactPaths
    {
        public string JsonPath { get; }
        public string MarkdownPath { get; }

        public RecoveryFallbackSurfaceArtifactPaths(string jsonPath, string markdownPath)
        {
            JsonPath = jsonPath;
            MarkdownPath = markdownPath;
        }
    }

    /// <summary>
    /// Test-owned R1.9 gate for known player-visible material failures. It examines only enabled
    /// renderers on active scene objects and blocks null material slots, null/error shaders, and the
    /// explicit RuntimeMaterialFixer_* output contract. It intentionally does not impose a broad
    /// shader whitelist or treat authored primitives as failures.
    /// </summary>
    public static class RecoveryFallbackSurfaceAudit
    {
        public const string ArtifactDirectoryName = "recovery-fallback-surfaces";

        public static RecoveryFallbackSurfaceReport Capture(string label, Transform scopeRoot = null)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Fallback-surface label is required.", nameof(label));

            var report = new RecoveryFallbackSurfaceReport
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                activeScene = SceneManager.GetActiveScene().name
            };

            Renderer[] renderers = scopeRoot != null
                ? scopeRoot.GetComponentsInChildren<Renderer>(true)
                : Resources.FindObjectsOfTypeAll<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.gameObject.scene.IsValid()) continue;
                if (!renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
                if (scopeRoot != null && renderer.transform != scopeRoot &&
                    !renderer.transform.IsChildOf(scopeRoot)) continue;

                report.activeRendererCount++;
                Material[] materials = renderer.sharedMaterials;
                if (materials == null) continue;
                report.materialSlotCount += materials.Length;

                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    Material material = materials[materialIndex];
                    if (material == null)
                    {
                        AddFinding(
                            report,
                            "VISIBLE_NULL_MATERIAL",
                            renderer,
                            materialIndex,
                            "NULL_MATERIAL",
                            "NULL_SHADER",
                            "Enabled renderer has a null material slot.");
                        continue;
                    }

                    string materialName = material.name ?? string.Empty;
                    string shaderName = material.shader != null
                        ? material.shader.name ?? string.Empty
                        : "NULL_SHADER";

                    if (materialName.StartsWith(
                            "RuntimeMaterialFixer_",
                            StringComparison.Ordinal))
                    {
                        AddFinding(
                            report,
                            "VISIBLE_RUNTIME_MATERIAL_FIXER",
                            renderer,
                            materialIndex,
                            materialName,
                            shaderName,
                            "RuntimeMaterialFixer replacement is visible in the Golden player view.");
                    }

                    if (material.shader == null)
                    {
                        AddFinding(
                            report,
                            "VISIBLE_NULL_SHADER",
                            renderer,
                            materialIndex,
                            materialName,
                            shaderName,
                            "Enabled renderer material has no shader.");
                    }
                    else if (shaderName.IndexOf(
                                 "InternalErrorShader",
                                 StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        AddFinding(
                            report,
                            "VISIBLE_ERROR_SHADER",
                            renderer,
                            materialIndex,
                            materialName,
                            shaderName,
                            "Enabled renderer is using Unity's internal error shader.");
                    }
                }
            }

            report.findings.Sort((a, b) =>
            {
                int path = string.CompareOrdinal(a.rendererPath, b.rendererPath);
                if (path != 0) return path;
                int slot = a.materialIndex.CompareTo(b.materialIndex);
                return slot != 0 ? slot : string.CompareOrdinal(a.code, b.code);
            });
            return report;
        }

        public static RecoveryFallbackSurfaceArtifactPaths AssertAndWrite(
            string label,
            string stem,
            Transform scopeRoot = null)
        {
            RecoveryFallbackSurfaceReport report = Capture(label, scopeRoot);
            RecoveryFallbackSurfaceArtifactPaths paths = WriteArtifacts(report, stem);
            if (report.findings.Count == 0)
            {
                Debug.Log("ZIPTIDE: RECOVERY_FALLBACK_SURFACE_OK label=" + label
                    + " renderers=" + report.activeRendererCount
                    + " slots=" + report.materialSlotCount);
                return paths;
            }

            var details = new StringBuilder();
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryFallbackSurfaceFinding finding = report.findings[i];
                details.AppendLine(finding.code + " path=" + finding.rendererPath
                    + " slot=" + finding.materialIndex
                    + " material=" + finding.materialName
                    + " shader=" + finding.shaderName);
            }

            Assert.Fail(label + " exposed known fallback/material blockers. JSON=" + paths.JsonPath
                + " Markdown=" + paths.MarkdownPath + "\n" + details);
            return paths;
        }

        public static RecoveryFallbackSurfaceArtifactPaths WriteArtifacts(
            RecoveryFallbackSurfaceReport report,
            string stem)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            string safeStem = SanitizeStem(stem);
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                ArtifactDirectoryName));
            Directory.CreateDirectory(directory);

            string jsonPath = Path.Combine(directory, safeStem + ".json");
            string markdownPath = Path.Combine(directory, safeStem + ".md");
            File.WriteAllText(
                jsonPath,
                JsonUtility.ToJson(report, true) + Environment.NewLine,
                Encoding.UTF8);
            File.WriteAllText(markdownPath, BuildMarkdown(report), Encoding.UTF8);
            return new RecoveryFallbackSurfaceArtifactPaths(jsonPath, markdownPath);
        }

        private static void AddFinding(
            RecoveryFallbackSurfaceReport report,
            string code,
            Renderer renderer,
            int materialIndex,
            string materialName,
            string shaderName,
            string message)
        {
            report.findings.Add(new RecoveryFallbackSurfaceFinding
            {
                code = code,
                rendererPath = RecoveryRuntimeCensus.HierarchyPath(renderer.transform),
                scene = renderer.gameObject.scene.name,
                materialIndex = materialIndex,
                materialName = materialName,
                shaderName = shaderName,
                message = message
            });
        }

        private static string BuildMarkdown(RecoveryFallbackSurfaceReport report)
        {
            var text = new StringBuilder(1024);
            text.AppendLine("# Recovery Fallback Surface Audit — " + report.label);
            text.AppendLine();
            text.AppendLine("- Captured: `" + report.capturedAtUtc + "`");
            text.AppendLine("- Active scene: `" + report.activeScene + "`");
            text.AppendLine("- Active renderers: " + report.activeRendererCount);
            text.AppendLine("- Material slots: " + report.materialSlotCount);
            text.AppendLine("- Blockers: " + report.findings.Count);
            text.AppendLine();
            text.AppendLine("| Code | Renderer | Scene | Slot | Material | Shader | Message |");
            text.AppendLine("|---|---|---|---:|---|---|---|");
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryFallbackSurfaceFinding finding = report.findings[i];
                text.Append("| ").Append(Escape(finding.code))
                    .Append(" | ").Append(Escape(finding.rendererPath))
                    .Append(" | ").Append(Escape(finding.scene))
                    .Append(" | ").Append(finding.materialIndex)
                    .Append(" | ").Append(Escape(finding.materialName))
                    .Append(" | ").Append(Escape(finding.shaderName))
                    .Append(" | ").Append(Escape(finding.message)).AppendLine(" |");
            }
            return text.ToString();
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "fallback-surfaces";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
