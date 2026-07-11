#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Warning-only v1 quality gate for world-space TextMesh labels and text-bearing diegetic
    /// interactables. It reports structural readability/reach risks without mutating authored content.
    /// Thresholds are locked in docs/design/UI_READABILITY.md and literal tests.
    /// </summary>
    public static class UiReadabilityAuditRules
    {
        public const float MinimumEffectiveTextScale = 1.2f;
        public const float MinimumTargetFaceMeters = 0.10f;
        public const float MaximumLabelSeparationMeters = 0.30f;

        public static void Run(SceneAuditReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            foreach (var label in UnityEngine.Object.FindObjectsOfType<TextMesh>(true))
            {
                if (label == null || string.IsNullOrWhiteSpace(label.text)) continue;

                float effective = EffectiveTextScale(label.characterSize, label.fontSize);
                if (effective < MinimumEffectiveTextScale)
                {
                    report.Warning(
                        "UI_TEXT_TOO_SMALL",
                        "TextMesh '" + label.name + "' has effective scale " +
                        effective.ToString("F2") + " (characterSize " +
                        Mathf.Abs(label.characterSize).ToString("F3") + " × fontSize " +
                        label.fontSize + "); minimum is " + MinimumEffectiveTextScale.ToString("F2") +
                        ". Increase the authored text size and verify at arm's length.",
                        Path(label.transform));
                }
            }

            foreach (var interactable in UnityEngine.Object.FindObjectsOfType<XRBaseInteractable>(true))
            {
                if (interactable == null) continue;

                TextMesh[] labels = interactable.GetComponentsInChildren<TextMesh>(true);
                if (labels == null || labels.Length == 0) continue; // physical prop, not a UI surface

                Collider[] colliders = interactable.GetComponentsInChildren<Collider>(true);
                if (colliders == null || colliders.Length == 0)
                {
                    Collider parentCollider = interactable.GetComponentInParent<Collider>();
                    if (parentCollider != null) colliders = new[] { parentCollider };
                }

                string where = Path(interactable.transform);
                if (colliders == null || colliders.Length == 0)
                {
                    report.Warning(
                        "UI_INTERACTABLE_NO_COLLIDER",
                        "Text-bearing interactable '" + interactable.name +
                        "' has no collider on itself, its children, or its parent. It may be visible but impossible to select.",
                        where);
                    continue;
                }

                Bounds bounds = CombinedBounds(colliders);
                if (!TargetFaceIsLargeEnough(bounds.size))
                {
                    Vector3 size = Abs(bounds.size);
                    report.Warning(
                        "UI_TARGET_TOO_SMALL",
                        "Text-bearing interactable '" + interactable.name + "' target bounds are " +
                        size.x.ToString("F2") + " × " + size.y.ToString("F2") + " × " +
                        size.z.ToString("F2") + "m. At least two dimensions must be ≥ " +
                        MinimumTargetFaceMeters.ToString("F2") + "m.",
                        where);
                }

                foreach (var label in labels)
                {
                    if (label == null || string.IsNullOrWhiteSpace(label.text)) continue;
                    float separation = Vector3.Distance(label.transform.position,
                        bounds.ClosestPoint(label.transform.position));
                    if (separation > MaximumLabelSeparationMeters)
                    {
                        report.Warning(
                            "UI_LABEL_DETACHED",
                            "Label '" + label.name + "' is " + separation.ToString("F2") +
                            "m from interactable '" + interactable.name + "' collider bounds; maximum is " +
                            MaximumLabelSeparationMeters.ToString("F2") +
                            "m. The visible instruction may not identify the selectable target.",
                            Path(label.transform));
                    }
                }
            }
        }

        public static float EffectiveTextScale(float characterSize, int fontSize)
        {
            return Mathf.Abs(characterSize) * Mathf.Max(1, fontSize);
        }

        public static bool TargetFaceIsLargeEnough(Vector3 worldSize)
        {
            float[] axes = { Mathf.Abs(worldSize.x), Mathf.Abs(worldSize.y), Mathf.Abs(worldSize.z) };
            Array.Sort(axes);
            return axes[1] >= MinimumTargetFaceMeters && axes[2] >= MinimumTargetFaceMeters;
        }

        public static Bounds CombinedBounds(IReadOnlyList<Collider> colliders)
        {
            if (colliders == null || colliders.Count == 0)
                throw new ArgumentException("At least one collider is required.", nameof(colliders));

            bool initialized = false;
            Bounds combined = default;
            for (int i = 0; i < colliders.Count; i++)
            {
                Collider collider = colliders[i];
                if (collider == null) continue;
                if (!initialized)
                {
                    combined = collider.bounds;
                    initialized = true;
                }
                else
                {
                    combined.Encapsulate(collider.bounds);
                }
            }

            if (!initialized)
                throw new ArgumentException("At least one non-null collider is required.", nameof(colliders));
            return combined;
        }

        private static Vector3 Abs(Vector3 value)
        {
            return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
        }

        private static string Path(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}
#endif
