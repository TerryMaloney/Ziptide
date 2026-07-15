using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryUiSpatialRecord
    {
        public string type;
        public string hierarchyPath;
        public string scene;
        public string text;
        public string rendererPath;
        public string interactionTargetPath;
        public bool active;
        public bool visibleInFrustum;
        public bool hasViewportBounds;
        public float facingDot;
        public float viewportMinX;
        public float viewportMinY;
        public float viewportMaxX;
        public float viewportMaxY;
        public float viewportArea;
        public float viewportVisibleFraction;
        public float worldWidth;
        public float worldHeight;
    }

    [Serializable]
    public sealed class RecoveryUiSpatialFinding
    {
        public string severity;
        public string code;
        public string hierarchyPath;
        public string relatedPath;
        public string message;
    }

    [Serializable]
    public sealed class RecoveryUiSpatialReport
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string cameraPath;
        public string activeScene;
        public List<RecoveryUiSpatialRecord> records = new List<RecoveryUiSpatialRecord>();
        public List<RecoveryUiSpatialFinding> findings = new List<RecoveryUiSpatialFinding>();
    }

    public sealed class RecoveryUiSpatialArtifactPaths
    {
        public string JsonPath { get; }
        public string MarkdownPath { get; }

        public RecoveryUiSpatialArtifactPaths(string jsonPath, string markdownPath)
        {
            JsonPath = jsonPath;
            MarkdownPath = markdownPath;
        }
    }

    /// <summary>
    /// Test-owned camera-space audit for actual runtime world text. It records hierarchy-grounded
    /// facing, clipping, target ownership and screen overlap evidence. It intentionally does not
    /// judge prose or aesthetics; it catches composition failures visible to a real player camera.
    /// TMP is detected by walking the concrete component's base types to TMPro.TMP_Text, avoiding a
    /// compile-time dependency from the test assembly while still covering TextMeshPro subclasses.
    /// </summary>
    public static class RecoveryUiSpatialAudit
    {
        private const float FacingBlockerThreshold = 0.10f;
        private const float MinimumVisibleFraction = 0.75f;
        private const float OverlapBlockerRatio = 0.25f;
        private const float DuplicateOverlapRatio = 0.10f;

        public static RecoveryUiSpatialReport Capture(Camera camera, string label)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("UI spatial label is required.", nameof(label));

            var report = new RecoveryUiSpatialReport
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                cameraPath = RecoveryRuntimeCensus.HierarchyPath(camera.transform),
                activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            };

            CaptureTextMeshes(camera, report);
            CaptureTmpText(camera, report);

            report.records.Sort((a, b) => string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath));
            AddPerRecordFindings(report);
            AddOverlapFindings(report);
            report.findings.Sort((a, b) =>
            {
                int code = string.CompareOrdinal(a.code, b.code);
                if (code != 0) return code;
                int path = string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath);
                return path != 0 ? path : string.CompareOrdinal(a.relatedPath, b.relatedPath);
            });
            return report;
        }

        public static RecoveryUiSpatialArtifactPaths WriteArtifacts(
            RecoveryUiSpatialReport report,
            string stem)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                "recovery-ui-spatial"));
            Directory.CreateDirectory(directory);
            string safeStem = SanitizeStem(stem);
            string jsonPath = Path.Combine(directory, safeStem + ".json");
            string markdownPath = Path.Combine(directory, safeStem + ".md");
            File.WriteAllText(
                jsonPath,
                JsonUtility.ToJson(report, true) + Environment.NewLine,
                Encoding.UTF8);
            File.WriteAllText(markdownPath, BuildMarkdown(report), Encoding.UTF8);
            return new RecoveryUiSpatialArtifactPaths(jsonPath, markdownPath);
        }

        private static void CaptureTextMeshes(Camera camera, RecoveryUiSpatialReport report)
        {
            TextMesh[] textMeshes = UnityEngine.Object.FindObjectsOfType<TextMesh>();
            for (int i = 0; i < textMeshes.Length; i++)
            {
                TextMesh text = textMeshes[i];
                if (text == null || !text.gameObject.activeInHierarchy) continue;
                Renderer renderer = text.GetComponent<Renderer>();
                if (renderer == null || !renderer.enabled) continue;
                report.records.Add(BuildRecord(
                    camera,
                    text.transform,
                    renderer,
                    "UnityEngine.TextMesh",
                    text.text,
                    readsFromNegativeZ: true));
            }
        }

        private static void CaptureTmpText(Camera camera, RecoveryUiSpatialReport report)
        {
            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null ||
                    !behaviour.gameObject.activeInHierarchy ||
                    !behaviour.enabled ||
                    !IsTmpTextType(behaviour.GetType()))
                    continue;

                report.records.Add(BuildRecord(
                    camera,
                    behaviour.transform,
                    behaviour.GetComponent<Renderer>(),
                    behaviour.GetType().FullName ?? "TMPro.TMP_Text",
                    ReadStringProperty(behaviour, "text"),
                    readsFromNegativeZ: false));
            }
        }

        internal static bool IsTmpTextType(Type type)
        {
            Type current = type;
            while (current != null)
            {
                if (string.Equals(current.FullName, "TMPro.TMP_Text", StringComparison.Ordinal))
                    return true;
                current = current.BaseType;
            }
            return false;
        }

        private static RecoveryUiSpatialRecord BuildRecord(
            Camera camera,
            Transform transform,
            Renderer renderer,
            string type,
            string text,
            bool readsFromNegativeZ)
        {
            Vector3 towardViewer = camera.transform.position - transform.position;
            float facingDot = towardViewer.sqrMagnitude > 0.000001f
                ? Vector3.Dot(
                    readsFromNegativeZ ? -transform.forward : transform.forward,
                    towardViewer.normalized)
                : 0f;

            var record = new RecoveryUiSpatialRecord
            {
                type = type,
                hierarchyPath = RecoveryRuntimeCensus.HierarchyPath(transform),
                scene = transform.gameObject.scene.name,
                text = NormalizeText(text),
                rendererPath = renderer != null
                    ? RecoveryRuntimeCensus.HierarchyPath(renderer.transform)
                    : string.Empty,
                interactionTargetPath = FindInteractionTargetPath(transform),
                active = transform.gameObject.activeInHierarchy,
                facingDot = facingDot
            };

            if (renderer == null || !renderer.enabled) return record;
            Bounds bounds = renderer.bounds;
            record.worldWidth = bounds.size.x;
            record.worldHeight = bounds.size.y;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            record.visibleInFrustum = GeometryUtility.TestPlanesAABB(planes, bounds);

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3[] corners =
            {
                new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z),
                new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z),
                new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z)
            };

            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;
            bool anyInFront = false;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 viewport = camera.WorldToViewportPoint(corners[i]);
                if (viewport.z <= 0f) continue;
                anyInFront = true;
                minX = Mathf.Min(minX, viewport.x);
                minY = Mathf.Min(minY, viewport.y);
                maxX = Mathf.Max(maxX, viewport.x);
                maxY = Mathf.Max(maxY, viewport.y);
            }

            if (!anyInFront || maxX <= minX || maxY <= minY) return record;
            record.hasViewportBounds = true;
            record.viewportMinX = minX;
            record.viewportMinY = minY;
            record.viewportMaxX = maxX;
            record.viewportMaxY = maxY;
            record.viewportArea = (maxX - minX) * (maxY - minY);
            float clippedMinX = Mathf.Clamp01(minX);
            float clippedMinY = Mathf.Clamp01(minY);
            float clippedMaxX = Mathf.Clamp01(maxX);
            float clippedMaxY = Mathf.Clamp01(maxY);
            float visibleArea = Mathf.Max(0f, clippedMaxX - clippedMinX) *
                                Mathf.Max(0f, clippedMaxY - clippedMinY);
            record.viewportVisibleFraction = record.viewportArea > 0f
                ? visibleArea / record.viewportArea
                : 0f;
            return record;
        }

        private static void AddPerRecordFindings(RecoveryUiSpatialReport report)
        {
            for (int i = 0; i < report.records.Count; i++)
            {
                RecoveryUiSpatialRecord record = report.records[i];
                if (string.IsNullOrEmpty(record.rendererPath))
                {
                    AddFinding(report, "WARNING", "TEXT_RENDERER_MISSING", record.hierarchyPath, "",
                        "Runtime text has no enabled Renderer and cannot be evaluated visually.");
                    continue;
                }
                if (!record.visibleInFrustum || !record.hasViewportBounds) continue;
                if (record.facingDot < FacingBlockerThreshold)
                {
                    AddFinding(report, "BLOCKER", "TEXT_FACING_AWAY", record.hierarchyPath, "",
                        "Visible text facingDot=" + record.facingDot.ToString("F3") +
                        " is below " + FacingBlockerThreshold.ToString("F2") + ".");
                }
                if (record.viewportVisibleFraction < MinimumVisibleFraction)
                {
                    AddFinding(report, "BLOCKER", "TEXT_CLIPPED_VIEWPORT", record.hierarchyPath, "",
                        "Only " + (record.viewportVisibleFraction * 100f).ToString("F1") +
                        "% of the projected text bounds are inside the player viewport.");
                }
            }
        }

        private static void AddOverlapFindings(RecoveryUiSpatialReport report)
        {
            for (int i = 0; i < report.records.Count; i++)
            {
                RecoveryUiSpatialRecord a = report.records[i];
                if (!EligibleForOverlap(a)) continue;
                for (int j = i + 1; j < report.records.Count; j++)
                {
                    RecoveryUiSpatialRecord b = report.records[j];
                    if (!EligibleForOverlap(b)) continue;
                    float intersection = IntersectionArea(a, b);
                    if (intersection <= 0f) continue;
                    float smaller = Mathf.Min(a.viewportArea, b.viewportArea);
                    if (smaller <= 0f) continue;
                    float ratio = intersection / smaller;
                    bool duplicate = !string.IsNullOrEmpty(a.text) && a.text == b.text;
                    float threshold = duplicate ? DuplicateOverlapRatio : OverlapBlockerRatio;
                    if (ratio < threshold) continue;

                    AddFinding(
                        report,
                        "BLOCKER",
                        duplicate ? "DUPLICATE_VISIBLE_TEXT" : "TEXT_SCREEN_OVERLAP",
                        a.hierarchyPath,
                        b.hierarchyPath,
                        "Projected text overlap is " + (ratio * 100f).ToString("F1") +
                        "% of the smaller bounds. a='" + a.text + "' b='" + b.text + "'.");
                }
            }
        }

        private static bool EligibleForOverlap(RecoveryUiSpatialRecord record)
        {
            return record.visibleInFrustum &&
                   record.hasViewportBounds &&
                   record.viewportVisibleFraction > 0f &&
                   record.facingDot >= FacingBlockerThreshold;
        }

        private static float IntersectionArea(
            RecoveryUiSpatialRecord a,
            RecoveryUiSpatialRecord b)
        {
            float minX = Mathf.Max(a.viewportMinX, b.viewportMinX);
            float minY = Mathf.Max(a.viewportMinY, b.viewportMinY);
            float maxX = Mathf.Min(a.viewportMaxX, b.viewportMaxX);
            float maxY = Mathf.Min(a.viewportMaxY, b.viewportMaxY);
            return Mathf.Max(0f, maxX - minX) * Mathf.Max(0f, maxY - minY);
        }

        private static string FindInteractionTargetPath(Transform value)
        {
            Transform current = value;
            while (current != null)
            {
                MonoBehaviour[] behaviours = current.GetComponents<MonoBehaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    MonoBehaviour behaviour = behaviours[i];
                    if (behaviour == null) continue;
                    string name = behaviour.GetType().FullName ?? string.Empty;
                    if (name.IndexOf("Interactable", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Button", StringComparison.OrdinalIgnoreCase) >= 0)
                        return RecoveryRuntimeCensus.HierarchyPath(current);
                }
                current = current.parent;
            }
            return string.Empty;
        }

        private static string ReadStringProperty(object value, string propertyName)
        {
            try
            {
                var property = value.GetType().GetProperty(propertyName);
                object result = property != null ? property.GetValue(value, null) : null;
                return result != null ? result.ToString() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var builder = new StringBuilder(value.Length);
            bool previousWhitespace = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsWhiteSpace(c))
                {
                    if (!previousWhitespace) builder.Append(' ');
                    previousWhitespace = true;
                }
                else
                {
                    builder.Append(char.ToUpperInvariant(c));
                    previousWhitespace = false;
                }
            }
            return builder.ToString().Trim();
        }

        private static void AddFinding(
            RecoveryUiSpatialReport report,
            string severity,
            string code,
            string hierarchyPath,
            string relatedPath,
            string message)
        {
            report.findings.Add(new RecoveryUiSpatialFinding
            {
                severity = severity,
                code = code,
                hierarchyPath = hierarchyPath,
                relatedPath = relatedPath,
                message = message
            });
        }

        private static string BuildMarkdown(RecoveryUiSpatialReport report)
        {
            var text = new StringBuilder(4096);
            text.AppendLine("# Recovery UI Spatial Audit — " + report.label);
            text.AppendLine();
            text.AppendLine("- Captured: `" + report.capturedAtUtc + "`");
            text.AppendLine("- Active scene: `" + report.activeScene + "`");
            text.AppendLine("- Camera: `" + report.cameraPath + "`");
            text.AppendLine("- Text records: " + report.records.Count);
            text.AppendLine("- Findings: " + report.findings.Count);
            text.AppendLine();
            text.AppendLine("## Findings");
            text.AppendLine();
            if (report.findings.Count == 0)
            {
                text.AppendLine("None.");
            }
            else
            {
                text.AppendLine("| Severity | Code | Path | Related | Message |");
                text.AppendLine("|---|---|---|---|---|");
                for (int i = 0; i < report.findings.Count; i++)
                {
                    RecoveryUiSpatialFinding finding = report.findings[i];
                    text.Append("| ").Append(Escape(finding.severity))
                        .Append(" | ").Append(Escape(finding.code))
                        .Append(" | ").Append(Escape(finding.hierarchyPath))
                        .Append(" | ").Append(Escape(finding.relatedPath))
                        .Append(" | ").Append(Escape(finding.message)).AppendLine(" |");
                }
            }
            return text.ToString();
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("|", "\\|")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "recovery-ui-spatial";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }
    }
}
