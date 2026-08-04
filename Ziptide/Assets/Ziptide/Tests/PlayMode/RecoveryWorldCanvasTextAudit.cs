using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Adds world-space TextMeshProUGUI evidence to the existing R1.8 report without introducing a
    /// compile-time TMPro dependency. TMP UGUI renders through CanvasRenderer rather than Renderer;
    /// this augmentation projects its RectTransform world corners and evaluates facing, clipping and
    /// overlap against every existing TextMesh/TMP mesh record in the same report.
    /// </summary>
    public static class RecoveryWorldCanvasTextAudit
    {
        private const float FacingBlockerThreshold = 0.10f;
        private const float MinimumVisibleFraction = 0.75f;
        private const float OverlapBlockerRatio = 0.25f;
        private const float DuplicateOverlapRatio = 0.10f;

        public static void Append(Camera camera, RecoveryUiSpatialReport report)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            if (report == null) throw new ArgumentNullException(nameof(report));

            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (!IsSupportedWorldCanvasText(behaviour, out CanvasRenderer canvasRenderer,
                        out RectTransform rectTransform))
                    continue;

                string hierarchyPath = RecoveryRuntimeCensus.HierarchyPath(rectTransform);
                RemoveUnsupportedDuplicate(report, hierarchyPath);
                RecoveryUiSpatialRecord record = BuildRecord(
                    camera,
                    behaviour,
                    canvasRenderer,
                    rectTransform,
                    hierarchyPath);
                report.records.Add(record);
                AddRecordFindings(report, record);
                AddCrossOverlapFindings(report, record);
            }

            report.records.Sort((a, b) => string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath));
            report.findings.Sort((a, b) =>
            {
                int code = string.CompareOrdinal(a.code, b.code);
                if (code != 0) return code;
                int path = string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath);
                return path != 0 ? path : string.CompareOrdinal(a.relatedPath, b.relatedPath);
            });
        }

        private static bool IsSupportedWorldCanvasText(
            MonoBehaviour behaviour,
            out CanvasRenderer canvasRenderer,
            out RectTransform rectTransform)
        {
            canvasRenderer = null;
            rectTransform = null;
            if (behaviour == null ||
                !behaviour.enabled ||
                !behaviour.gameObject.activeInHierarchy ||
                !RecoveryUiSpatialAudit.IsTmpTextType(behaviour.GetType()) ||
                behaviour.GetComponent<Renderer>() != null)
                return false;

            rectTransform = behaviour.transform as RectTransform;
            canvasRenderer = behaviour.GetComponent<CanvasRenderer>();
            if (rectTransform == null || canvasRenderer == null) return false;
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            return canvas != null &&
                   canvas.enabled &&
                   canvas.gameObject.activeInHierarchy &&
                   canvas.renderMode == RenderMode.WorldSpace;
        }

        private static RecoveryUiSpatialRecord BuildRecord(
            Camera camera,
            MonoBehaviour behaviour,
            CanvasRenderer canvasRenderer,
            RectTransform rectTransform,
            string hierarchyPath)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 towardViewer = camera.transform.position - rectTransform.position;
            float facingDot = towardViewer.sqrMagnitude > 0.000001f
                ? Vector3.Dot(-rectTransform.forward, towardViewer.normalized)
                : 0f;

            Bounds bounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Length; i++) bounds.Encapsulate(corners[i]);
            bool visible = !canvasRenderer.cull &&
                           canvasRenderer.GetInheritedAlpha() > 0.001f &&
                           GeometryUtility.TestPlanesAABB(
                               GeometryUtility.CalculateFrustumPlanes(camera),
                               bounds);

            var record = new RecoveryUiSpatialRecord
            {
                type = behaviour.GetType().FullName ?? "TMPro.TextMeshProUGUI",
                hierarchyPath = hierarchyPath,
                scene = behaviour.gameObject.scene.name,
                text = NormalizeText(ReadText(behaviour)),
                rendererPath = hierarchyPath,
                interactionTargetPath = FindInteractionTargetPath(rectTransform),
                active = behaviour.gameObject.activeInHierarchy,
                visibleInFrustum = visible,
                facingDot = facingDot,
                worldWidth = Vector3.Distance(corners[0], corners[3]),
                worldHeight = Vector3.Distance(corners[0], corners[1])
            };
            ApplyViewportBounds(camera, record, corners);
            return record;
        }

        private static void ApplyViewportBounds(
            Camera camera,
            RecoveryUiSpatialRecord record,
            Vector3[] corners)
        {
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

            if (!anyInFront || maxX <= minX || maxY <= minY) return;
            record.hasViewportBounds = true;
            record.viewportMinX = minX;
            record.viewportMinY = minY;
            record.viewportMaxX = maxX;
            record.viewportMaxY = maxY;
            record.viewportArea = (maxX - minX) * (maxY - minY);
            float visibleArea = Mathf.Max(0f, Mathf.Clamp01(maxX) - Mathf.Clamp01(minX)) *
                                Mathf.Max(0f, Mathf.Clamp01(maxY) - Mathf.Clamp01(minY));
            record.viewportVisibleFraction = record.viewportArea > 0f
                ? visibleArea / record.viewportArea
                : 0f;
        }

        private static void RemoveUnsupportedDuplicate(
            RecoveryUiSpatialReport report,
            string hierarchyPath)
        {
            for (int i = report.records.Count - 1; i >= 0; i--)
            {
                RecoveryUiSpatialRecord record = report.records[i];
                if (record.hierarchyPath == hierarchyPath && string.IsNullOrEmpty(record.rendererPath))
                    report.records.RemoveAt(i);
            }
            for (int i = report.findings.Count - 1; i >= 0; i--)
            {
                RecoveryUiSpatialFinding finding = report.findings[i];
                if (finding.hierarchyPath == hierarchyPath && finding.code == "TEXT_RENDERER_MISSING")
                    report.findings.RemoveAt(i);
            }
        }

        private static void AddRecordFindings(
            RecoveryUiSpatialReport report,
            RecoveryUiSpatialRecord record)
        {
            if (!record.visibleInFrustum || !record.hasViewportBounds) return;
            if (record.facingDot < FacingBlockerThreshold)
            {
                AddFinding(report, "BLOCKER", "TEXT_FACING_AWAY", record.hierarchyPath, "",
                    "Visible world-canvas text facingDot=" + record.facingDot.ToString("F3") +
                    " is below " + FacingBlockerThreshold.ToString("F2") + ".");
            }
            if (record.viewportVisibleFraction < MinimumVisibleFraction)
            {
                AddFinding(report, "BLOCKER", "TEXT_CLIPPED_VIEWPORT", record.hierarchyPath, "",
                    "Only " + (record.viewportVisibleFraction * 100f).ToString("F1") +
                    "% of the projected world-canvas text bounds are inside the player viewport.");
            }
        }

        private static void AddCrossOverlapFindings(
            RecoveryUiSpatialReport report,
            RecoveryUiSpatialRecord canvasRecord)
        {
            if (!EligibleForOverlap(canvasRecord)) return;
            for (int i = 0; i < report.records.Count; i++)
            {
                RecoveryUiSpatialRecord other = report.records[i];
                if (other == canvasRecord || !EligibleForOverlap(other)) continue;
                float intersection = IntersectionArea(canvasRecord, other);
                if (intersection <= 0f) continue;
                float smaller = Mathf.Min(canvasRecord.viewportArea, other.viewportArea);
                if (smaller <= 0f) continue;
                float ratio = intersection / smaller;
                bool duplicate = !string.IsNullOrEmpty(canvasRecord.text) &&
                                 canvasRecord.text == other.text;
                float threshold = duplicate ? DuplicateOverlapRatio : OverlapBlockerRatio;
                if (ratio < threshold) continue;

                AddFinding(
                    report,
                    "BLOCKER",
                    duplicate ? "DUPLICATE_VISIBLE_TEXT" : "TEXT_SCREEN_OVERLAP",
                    other.hierarchyPath,
                    canvasRecord.hierarchyPath,
                    "Projected text overlap is " + (ratio * 100f).ToString("F1") +
                    "% of the smaller bounds. a='" + other.text + "' b='" + canvasRecord.text + "'.");
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

        private static string ReadText(object value)
        {
            try
            {
                var property = value.GetType().GetProperty("text");
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
            var builder = new System.Text.StringBuilder(value.Length);
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
    }
}
