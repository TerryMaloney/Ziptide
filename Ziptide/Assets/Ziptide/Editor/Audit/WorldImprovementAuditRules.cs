#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Editor.WorldImprovement;

namespace Ziptide.Editor.Audit
{
    /// <summary>Per-world excellence contract for deterministic improvement rounds.</summary>
    public static class WorldImprovementAuditRules
    {
        public const string StampMissing = "WORLD_IMPROVEMENT_STAMP_MISSING";
        public const string StaleRecipe = "WORLD_IMPROVEMENT_STALE_VS_RECIPE";
        public const string ModuleMissing = "WORLD_IMPROVEMENT_MODULE_MISSING";
        public const string ModuleDuplicate = "WORLD_IMPROVEMENT_MODULE_DUPLICATE";
        public const string ModuleVersion = "WORLD_IMPROVEMENT_MODULE_VERSION";
        public const string ModuleEmpty = "WORLD_IMPROVEMENT_MODULE_EMPTY";
        public const string ModuleBudget = "WORLD_IMPROVEMENT_MODULE_OVER_BUDGET";
        public const string AspectMissing = "WORLD_IMPROVEMENT_ASPECT_UNCOVERED";
        public const string EvidenceMissing = "WORLD_IMPROVEMENT_EVIDENCE_UNDECLARED";

        public static void Run(SceneAuditReport report)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid()) return;
            string scenePath = scene.path;
            if (!WorldImprovementCompiler.TryResolveManifest(scene.name, scenePath,
                    out WorldImprovementManifest manifest, out string rawJson, out _))
                return;
            string expectedHash = WorldImprovementHashCore.Compute(rawJson,
                WorldImprovementCompiler.CompilerVersion);
            AuditAgainstManifest(report, manifest, expectedHash, scene.name);
        }

        public static void AuditAgainstManifest(SceneAuditReport report,
            WorldImprovementManifest manifest, string expectedHash, string actualSceneName)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));

            WorldImprovementStamp stamp = FindStamp(actualSceneName);
            if (stamp == null)
            {
                report.Blocker(StampMissing,
                    "Scene has improvement manifest '" + manifest.manifestId
                    + "' but no compiled " + WorldImprovementCompiler.RootName + " stamp.");
                return;
            }

            string stampPath = GetPath(stamp.transform);
            if (!string.Equals(stamp.ManifestId, manifest.manifestId, StringComparison.Ordinal)
                || !string.Equals(stamp.SceneName, actualSceneName, StringComparison.Ordinal)
                || stamp.Round != manifest.round
                || stamp.RecipeVersion != manifest.recipeVersion
                || stamp.CompilerVersion != WorldImprovementCompiler.CompilerVersion
                || !string.Equals(stamp.RecipeHash, expectedHash, StringComparison.Ordinal))
            {
                report.Blocker(StaleRecipe,
                    "Compiled stamp does not match current manifest/compiler. expectedManifest="
                    + manifest.manifestId + " expectedRound=" + manifest.round
                    + " expectedHash=" + Short(expectedHash) + " actualHash=" + Short(stamp.RecipeHash),
                    stampPath);
            }

            var markers = new Dictionary<string, WorldImprovementModuleMarker>(StringComparer.Ordinal);
            foreach (WorldImprovementModuleMarker marker in stamp.GetComponentsInChildren<WorldImprovementModuleMarker>(true))
            {
                if (marker == null || string.IsNullOrEmpty(marker.ModuleId)) continue;
                if (markers.ContainsKey(marker.ModuleId))
                    report.Blocker(ModuleDuplicate, "Duplicate module marker '" + marker.ModuleId + "'.",
                        GetPath(marker.transform));
                else
                    markers.Add(marker.ModuleId, marker);
            }

            var covered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (WorldImprovementModuleSpec spec in manifest.modules)
            {
                if (spec == null || !spec.enabled) continue;
                if (!markers.TryGetValue(spec.moduleId, out WorldImprovementModuleMarker marker))
                {
                    report.Blocker(ModuleMissing, "Required module '" + spec.moduleId + "' is absent.", stampPath);
                    continue;
                }
                string markerPath = GetPath(marker.transform);
                if (marker.ModuleVersion != spec.version)
                    report.Blocker(ModuleVersion, "Module '" + spec.moduleId + "' version="
                        + marker.ModuleVersion + " expected=" + spec.version + ".", markerPath);
                if (marker.ObjectCount <= 0)
                    report.Blocker(ModuleEmpty, "Module '" + spec.moduleId + "' produced no owned objects.", markerPath);
                if (marker.ObjectCount > spec.budget)
                    report.Blocker(ModuleBudget, "Module '" + spec.moduleId + "' objects="
                        + marker.ObjectCount + " budget=" + spec.budget + ".", markerPath);
                if (marker.Aspects != null)
                    foreach (string aspect in marker.Aspects)
                        if (!string.IsNullOrWhiteSpace(aspect)) covered.Add(aspect.Trim());
            }

            foreach (string aspect in manifest.requiredAspects ?? Array.Empty<string>())
                if (!string.IsNullOrWhiteSpace(aspect) && !covered.Contains(aspect.Trim()))
                    report.Blocker(AspectMissing, "Required quality aspect '" + aspect
                        + "' has no compiled module coverage.", stampPath);

            var evidence = new HashSet<string>(stamp.RequiredEvidence ?? Array.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
            foreach (string required in manifest.requiredEvidence ?? Array.Empty<string>())
                if (!string.IsNullOrWhiteSpace(required) && !evidence.Contains(required.Trim()))
                    report.Blocker(EvidenceMissing, "Required evidence kind '" + required
                        + "' is absent from the compiled stamp.", stampPath);
        }

        private static WorldImprovementStamp FindStamp(string sceneName)
        {
            foreach (WorldImprovementStamp stamp in UnityEngine.Object.FindObjectsOfType<WorldImprovementStamp>(true))
                if (stamp != null && stamp.gameObject.scene.name == sceneName) return stamp;
            return null;
        }

        private static string Short(string value)
            => string.IsNullOrEmpty(value) ? "<empty>" : value.Substring(0, Mathf.Min(12, value.Length));

        private static string GetPath(Transform value)
        {
            if (value == null) return string.Empty;
            string path = value.name;
            while (value.parent != null)
            {
                value = value.parent;
                path = value.name + "/" + path;
            }
            return path;
        }
    }
}
#endif
