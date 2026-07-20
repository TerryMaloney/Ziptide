#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.WorldImprovement
{
    /// <summary>
    /// Deterministic compiler for docs/worldimprovements/*.improvement.json. Exact-scene manifests beat
    /// generated-world defaults; within each class the newest round/recipe wins while older recipes remain
    /// auditable history. Every compile replaces one owned root, runs versioned modules, stamps the recipe
    /// hash, scores required-aspect evidence and records the next weakest dimensions. No per-world C# and
    /// no hand-edited scene YAML.
    /// </summary>
    public static class WorldImprovementCompiler
    {
        public const int CompilerVersion = 2;
        public const string RootName = "__WORLD_IMPROVEMENT_ROUND";

        [Serializable]
        private sealed class CompileReport
        {
            public int schemaVersion = 2;
            public int compilerVersion = CompilerVersion;
            public List<CompileRecord> worlds = new List<CompileRecord>();
        }

        [Serializable]
        private sealed class CompileRecord
        {
            public string sceneName;
            public string manifestId;
            public int round;
            public int recipeVersion;
            public string recipeHash;
            public string manifestPath;
            public string[] modules;
            public int[] objectCounts;
            public string[] aspectNames;
            public int[] aspectScores;
            public string[] weakestAspects;
        }

        private sealed class ManifestSource
        {
            public WorldImprovementManifest Manifest;
            public string Json;
            public string Path;
        }

        private static readonly List<CompileRecord> SessionRecords = new List<CompileRecord>();

        public static string ManifestFolder =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/worldimprovements"));

        public static void BeginCompileSession() => SessionRecords.Clear();

        public static bool CompileActiveSceneIfDeclared(string scenePath)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || string.IsNullOrEmpty(scene.name)) return false;
            if (!TryResolveManifest(scene.name, scenePath, out WorldImprovementManifest manifest,
                    out string rawJson, out string manifestPath))
                return false;
            CompileActiveScene(manifest, rawJson, manifestPath, scenePath);
            return true;
        }

        public static void CompileActiveScene(WorldImprovementManifest manifest, string rawJson,
            string manifestPath, string scenePath)
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));
            List<string> issues = manifest.Validate();
            if (issues.Count > 0)
                throw new InvalidOperationException("World improvement manifest rejected: "
                    + string.Join(" | ", issues));

            Scene scene = SceneManager.GetActiveScene();
            string sceneName = scene.name;
            string hash = WorldImprovementHashCore.Compute(rawJson, CompilerVersion);
            GameObject existing = FindOwnedRoot(scene);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            var rootObject = new GameObject(RootName);
            var context = BuildContext(sceneName, scenePath, manifest, rootObject.transform);
            var moduleIds = new List<string>();
            var objectCounts = new List<int>();
            var aspectEvidence = new List<WorldAspectEvidence>();

            foreach (WorldImprovementModuleSpec spec in manifest.modules)
            {
                if (spec == null || !spec.enabled) continue;
                IWorldImprovementModule module = WorldImprovementModuleRegistry.Resolve(spec.moduleId);
                if (module == null)
                    throw new InvalidOperationException("Unknown world improvement module '" + spec.moduleId
                        + "'. Known=[" + string.Join(",", WorldImprovementModuleRegistry.KnownIds()) + "]");
                if (spec.version != module.CurrentVersion)
                    throw new InvalidOperationException("Module version mismatch id=" + spec.moduleId
                        + " manifest=" + spec.version + " compiler=" + module.CurrentVersion);

                string safeName = spec.moduleId.Replace('-', '_').Replace(' ', '_').ToUpperInvariant();
                var moduleRoot = new GameObject("__WIM_" + safeName).transform;
                moduleRoot.SetParent(rootObject.transform, false);
                WorldImprovementModuleResult result = module.Apply(context, spec, moduleRoot)
                    ?? new WorldImprovementModuleResult();
                if (result.ObjectCount > spec.budget)
                    throw new InvalidOperationException("Module budget exceeded id=" + spec.moduleId
                        + " count=" + result.ObjectCount + " budget=" + spec.budget);
                moduleRoot.gameObject.AddComponent<WorldImprovementModuleMarker>()
                    .Configure(spec, result.ObjectCount);
                moduleIds.Add(spec.moduleId);
                objectCounts.Add(result.ObjectCount);

                if (spec.aspects != null)
                {
                    for (int i = 0; i < spec.aspects.Length; i++)
                    {
                        string aspect = (spec.aspects[i] ?? string.Empty).Trim();
                        if (aspect.Length > 0)
                            aspectEvidence.Add(new WorldAspectEvidence(aspect, 1, result.ObjectCount));
                    }
                }

                Debug.Log("ZIPTIDE: WORLD_IMPROVEMENT_MODULE scene=" + sceneName
                    + " id=" + spec.moduleId + " version=" + spec.version
                    + " objects=" + result.ObjectCount + " summary=" + (result.Summary ?? string.Empty));
            }

            rootObject.AddComponent<WorldImprovementStamp>()
                .Configure(manifest, sceneName, CompilerVersion, hash);

            WorldAspectAssessment[] assessment = WorldImprovementAssessmentCore.Assess(
                manifest.requiredAspects, aspectEvidence);
            var aspectNames = new string[assessment.Length];
            var aspectScores = new int[assessment.Length];
            for (int i = 0; i < assessment.Length; i++)
            {
                aspectNames[i] = assessment[i].Aspect;
                aspectScores[i] = assessment[i].Score;
            }
            string[] weakest = WorldImprovementAssessmentCore.Weakest(assessment, 3);

            SessionRecords.Add(new CompileRecord
            {
                sceneName = sceneName,
                manifestId = manifest.manifestId,
                round = manifest.round,
                recipeVersion = manifest.recipeVersion,
                recipeHash = hash,
                manifestPath = MakeRepoRelative(manifestPath),
                modules = moduleIds.ToArray(),
                objectCounts = objectCounts.ToArray(),
                aspectNames = aspectNames,
                aspectScores = aspectScores,
                weakestAspects = weakest,
            });
            Debug.Log("ZIPTIDE: WORLD_IMPROVEMENT_COMPILED scene=" + sceneName
                + " manifest=" + manifest.manifestId + " round=" + manifest.round
                + " hash=" + hash.Substring(0, 12) + " modules=" + moduleIds.Count
                + " weakest=" + string.Join(",", weakest));
        }

        public static bool TryResolveManifest(string sceneName, string scenePath,
            out WorldImprovementManifest manifest, out string rawJson, out string manifestPath)
        {
            manifest = null;
            rawJson = null;
            manifestPath = null;
            if (!Directory.Exists(ManifestFolder)) return false;

            ManifestSource exact = null;
            ManifestSource generatedDefault = null;
            string[] files = Directory.GetFiles(ManifestFolder, "*.improvement.json");
            Array.Sort(files, StringComparer.Ordinal);
            for (int i = 0; i < files.Length; i++)
            {
                string json = File.ReadAllText(files[i]);
                WorldImprovementManifest candidate = WorldImprovementManifest.FromJson(json);
                if (!candidate.AppliesTo(sceneName, scenePath)) continue;
                var source = new ManifestSource { Manifest = candidate, Json = json, Path = files[i] };
                if (!string.IsNullOrEmpty(candidate.sceneName) && candidate.sceneName == sceneName)
                    exact = SelectNewest(exact, source, "exact", sceneName);
                else
                    generatedDefault = SelectNewest(generatedDefault, source, "generated-default", sceneName);
            }

            ManifestSource resolved = exact ?? generatedDefault;
            if (resolved == null) return false;
            manifest = resolved.Manifest;
            rawJson = resolved.Json;
            manifestPath = resolved.Path;
            return true;
        }

        private static ManifestSource SelectNewest(ManifestSource current, ManifestSource candidate,
            string kind, string sceneName)
        {
            if (current == null) return candidate;
            int round = candidate.Manifest.round.CompareTo(current.Manifest.round);
            if (round > 0) return candidate;
            if (round < 0) return current;

            int recipe = candidate.Manifest.recipeVersion.CompareTo(current.Manifest.recipeVersion);
            if (recipe > 0) return candidate;
            if (recipe < 0) return current;

            throw new InvalidOperationException("Duplicate " + kind + " world improvement manifests at round="
                + candidate.Manifest.round + " recipeVersion=" + candidate.Manifest.recipeVersion
                + " for " + sceneName + ": " + current.Path + " and " + candidate.Path);
        }

        public static void WriteReport()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string reportFolder = Path.Combine(projectRoot, "Builds", "Reports");
            Directory.CreateDirectory(reportFolder);
            var report = new CompileReport { worlds = new List<CompileRecord>(SessionRecords) };
            string path = Path.Combine(reportFolder, "world_improvement_compile.json");
            File.WriteAllText(path, JsonUtility.ToJson(report, true) + Environment.NewLine);
            Debug.Log("ZIPTIDE: WORLD_IMPROVEMENT_REPORT path=" + path
                + " worlds=" + SessionRecords.Count);
        }

        private static WorldImprovementContext BuildContext(string sceneName, string scenePath,
            WorldImprovementManifest manifest, Transform root)
        {
            GameObject spawnObject = GameObject.Find("__SPAWN_PLAYER");
            Color fog = RenderSettings.fog ? RenderSettings.fogColor : new Color(0.20f, 0.23f, 0.27f, 1f);
            Color primaryFallback = Color.Lerp(fog, new Color(0.30f, 0.34f, 0.38f, 1f), 0.55f);
            Color accentFallback = Color.Lerp(primaryFallback, new Color(0.65f, 0.48f, 0.22f, 1f), 0.55f);
            Color glowFallback = Color.Lerp(accentFallback, Color.white, 0.35f);
            return new WorldImprovementContext
            {
                SceneName = sceneName,
                ScenePath = scenePath,
                Root = root,
                Spawn = spawnObject != null ? spawnObject.transform : null,
                Manifest = manifest,
                Primary = manifest.ResolvePrimary(primaryFallback),
                Accent = manifest.ResolveAccent(accentFallback),
                Glow = manifest.ResolveGlow(glowFallback),
            };
        }

        private static GameObject FindOwnedRoot(Scene scene)
        {
            if (!scene.IsValid()) return null;
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                if (roots[i] != null && roots[i].name == RootName) return roots[i];
            return null;
        }

        private static string MakeRepoRelative(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
            string full = Path.GetFullPath(path);
            if (full.StartsWith(repoRoot, StringComparison.OrdinalIgnoreCase))
                return full.Substring(repoRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Replace('\\', '/');
            return full.Replace('\\', '/');
        }
    }
}
#endif
