using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    [Serializable]
    public sealed class WorldImprovementSceneExemption
    {
        public string sceneName;
        public string reason;
    }

    /// <summary>
    /// Portable classification of which shipped scenes are game worlds that require an improvement
    /// manifest. Scenes outside these selectors are not silently called worlds; explicit exemptions carry
    /// a reason so test/bootstrap/arena modes remain visible policy decisions.
    /// </summary>
    [Serializable]
    public sealed class WorldImprovementCoveragePolicy
    {
        public int schemaVersion = 1;
        public string gameId = "game";
        public string[] requiredScenes = Array.Empty<string>();
        public string[] requiredPathContains = Array.Empty<string>();
        public List<WorldImprovementSceneExemption> exemptions = new List<WorldImprovementSceneExemption>();

        public static WorldImprovementCoveragePolicy FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Coverage policy JSON is empty.");
            var policy = JsonUtility.FromJson<WorldImprovementCoveragePolicy>(json);
            if (policy == null) throw new ArgumentException("Coverage policy JSON did not produce an object.");
            policy.requiredScenes ??= Array.Empty<string>();
            policy.requiredPathContains ??= Array.Empty<string>();
            policy.exemptions ??= new List<WorldImprovementSceneExemption>();
            return policy;
        }

        public List<string> Validate()
        {
            var issues = new List<string>();
            if (schemaVersion != 1) issues.Add("unsupported schemaVersion " + schemaVersion);
            if (string.IsNullOrWhiteSpace(gameId)) issues.Add("gameId is empty");
            var required = new HashSet<string>(StringComparer.Ordinal);
            foreach (string scene in requiredScenes)
            {
                if (string.IsNullOrWhiteSpace(scene)) { issues.Add("empty required scene"); continue; }
                if (!required.Add(scene)) issues.Add("duplicate required scene " + scene);
            }
            var exempt = new HashSet<string>(StringComparer.Ordinal);
            foreach (WorldImprovementSceneExemption entry in exemptions)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.sceneName))
                {
                    issues.Add("exemption has empty sceneName");
                    continue;
                }
                if (!exempt.Add(entry.sceneName)) issues.Add("duplicate exemption " + entry.sceneName);
                if (string.IsNullOrWhiteSpace(entry.reason)) issues.Add("exemption lacks reason " + entry.sceneName);
                if (required.Contains(entry.sceneName)) issues.Add("scene is both required and exempt " + entry.sceneName);
            }
            return issues;
        }

        public bool IsExempt(string sceneName)
        {
            foreach (WorldImprovementSceneExemption entry in exemptions)
                if (entry != null && string.Equals(entry.sceneName, sceneName, StringComparison.Ordinal)) return true;
            return false;
        }

        public bool RequiresManifest(string sceneName, string scenePath)
        {
            if (string.IsNullOrWhiteSpace(sceneName) || IsExempt(sceneName)) return false;
            foreach (string required in requiredScenes)
                if (string.Equals(required, sceneName, StringComparison.Ordinal)) return true;
            string normalized = (scenePath ?? string.Empty).Replace('\\', '/');
            foreach (string token in requiredPathContains)
                if (!string.IsNullOrWhiteSpace(token)
                    && normalized.IndexOf(token.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }
    }
}
