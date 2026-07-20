using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Ziptide.Content
{
    [Serializable]
    public sealed class WorldImprovementModuleSpec
    {
        public string moduleId;
        public int version = 1;
        public bool enabled = true;
        [Range(0f, 2f)] public float intensity = 1f;
        public int budget = 24;
        public int seedOffset;
        public string[] aspects = Array.Empty<string>();

        public IEnumerable<string> Validate()
        {
            if (string.IsNullOrWhiteSpace(moduleId)) yield return "moduleId is empty";
            if (version < 1) yield return moduleId + ": version must be >= 1";
            if (budget < 0) yield return moduleId + ": budget must be >= 0";
            if (intensity < 0f || intensity > 2f) yield return moduleId + ": intensity must be 0..2";
        }
    }

    /// <summary>
    /// Portable recipe for one repeatable world-improvement round. The compiler and audit consume this
    /// shape without knowing Ziptide story, biome or scene conventions; those remain data in JSON.
    /// </summary>
    [Serializable]
    public sealed class WorldImprovementManifest
    {
        public int schemaVersion = 1;
        public string gameId = "game";
        public string manifestId;
        public string sceneName;
        public string displayName;
        public int round = 1;
        public int recipeVersion = 1;
        public int seed = 1;
        public bool appliesToGeneratedWorlds;
        public string[] excludedScenes = Array.Empty<string>();
        public string[] requiredAspects = Array.Empty<string>();
        public string[] requiredEvidence = Array.Empty<string>();
        public Color primaryColor = Color.clear;
        public Color accentColor = Color.clear;
        public Color glowColor = Color.clear;
        public List<WorldImprovementModuleSpec> modules = new List<WorldImprovementModuleSpec>();

        public static WorldImprovementManifest FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Manifest JSON is empty.");
            var manifest = JsonUtility.FromJson<WorldImprovementManifest>(json);
            if (manifest == null) throw new ArgumentException("Manifest JSON did not produce an object.");
            manifest.modules ??= new List<WorldImprovementModuleSpec>();
            manifest.excludedScenes ??= Array.Empty<string>();
            manifest.requiredAspects ??= Array.Empty<string>();
            manifest.requiredEvidence ??= Array.Empty<string>();
            return manifest;
        }

        public string ToJson(bool pretty = true) => JsonUtility.ToJson(this, pretty);

        public bool AppliesTo(string candidateSceneName, string scenePath)
        {
            if (string.IsNullOrEmpty(candidateSceneName)) return false;
            if (!string.IsNullOrEmpty(sceneName) && sceneName == candidateSceneName) return true;
            if (!appliesToGeneratedWorlds) return false;
            if (string.IsNullOrEmpty(scenePath)
                || scenePath.Replace('\\', '/').IndexOf("/Scenes/Generated/", StringComparison.OrdinalIgnoreCase) < 0)
                return false;
            for (int i = 0; i < excludedScenes.Length; i++)
                if (string.Equals(excludedScenes[i], candidateSceneName, StringComparison.Ordinal)) return false;
            return true;
        }

        public List<string> Validate()
        {
            var issues = new List<string>();
            if (schemaVersion != 1) issues.Add("unsupported schemaVersion " + schemaVersion);
            if (string.IsNullOrWhiteSpace(gameId)) issues.Add("gameId is empty");
            if (string.IsNullOrWhiteSpace(manifestId)) issues.Add("manifestId is empty");
            if (string.IsNullOrWhiteSpace(sceneName) && !appliesToGeneratedWorlds)
                issues.Add("sceneName is empty and appliesToGeneratedWorlds is false");
            if (round < 1) issues.Add("round must be >= 1");
            if (recipeVersion < 1) issues.Add("recipeVersion must be >= 1");
            if (modules.Count == 0) issues.Add("no modules declared");

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var covered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var module in modules)
            {
                if (module == null) { issues.Add("null module entry"); continue; }
                foreach (string issue in module.Validate()) issues.Add(issue);
                if (!string.IsNullOrEmpty(module.moduleId) && !ids.Add(module.moduleId))
                    issues.Add("duplicate moduleId " + module.moduleId);
                if (module.aspects == null) continue;
                foreach (string aspect in module.aspects)
                    if (!string.IsNullOrWhiteSpace(aspect)) covered.Add(aspect.Trim());
            }

            foreach (string aspect in requiredAspects)
                if (!string.IsNullOrWhiteSpace(aspect) && !covered.Contains(aspect.Trim()))
                    issues.Add("required aspect has no enabled module coverage: " + aspect);
            return issues;
        }

        public Color ResolvePrimary(Color fallback) => primaryColor.a > 0.001f ? primaryColor : fallback;
        public Color ResolveAccent(Color fallback) => accentColor.a > 0.001f ? accentColor : fallback;
        public Color ResolveGlow(Color fallback) => glowColor.a > 0.001f ? glowColor : fallback;
    }

    /// <summary>Stable hash law for manifest JSON + compiler version. Timestamps are deliberately excluded.</summary>
    public static class WorldImprovementHashCore
    {
        public static string Compute(string manifestJson, int compilerVersion)
        {
            string normalized = (manifestJson ?? string.Empty).Replace("\r\n", "\n").Trim();
            byte[] bytes = Encoding.UTF8.GetBytes("compiler=" + compilerVersion + "\n" + normalized);
            using var sha = SHA256.Create();
            byte[] digest = sha.ComputeHash(bytes);
            var builder = new StringBuilder(digest.Length * 2);
            for (int i = 0; i < digest.Length; i++) builder.Append(digest[i].ToString("x2"));
            return builder.ToString();
        }
    }
}
