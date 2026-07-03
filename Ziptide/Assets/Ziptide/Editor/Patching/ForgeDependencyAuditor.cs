#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>Extension point (reconciliation R3, clarification #4): dependency facts can come
    /// from typed SO fields today, and from prefab/scene/manifest scanners later — new sources
    /// plug in without touching the pure bucketing core (`ForgeStaleness`).</summary>
    public interface IForgeDependencySource
    {
        string Name { get; }
        void Collect(Dictionary<string, ForgeAssetInfo> assets);
    }

    /// <summary>
    /// R3: collects every Forge asset's dependency facts and writes the DETERMINISTIC reports
    /// (sorted, no timestamps) to <c>Ziptide/Builds/Reports/</c> — gitignored build ARTIFACTS,
    /// never committed (clarification #3); CI uploads them with the audit-report artifact.
    /// FORGE_MANIFEST.json answers "what exists / what's playable / what's placeholder / what
    /// needs upgrade"; FORGE_DEPENDENCY_REPORT.md answers "the story changed — what's affected?".
    /// </summary>
    public static class ForgeDependencyAuditor
    {
        public static readonly List<IForgeDependencySource> Sources = new List<IForgeDependencySource>
        {
            new TypedFieldSource(),
            // Future: BakedPrefabSource (AssetDatabase.GetDependencies, post-E1.4), SceneSeamSource.
        };

        [MenuItem("Ziptide/Art/Write Forge Reports (manifest + dependencies)")]
        public static void WriteReportsFromMenu()
        {
            string dir = WriteReports();
            EditorUtility.DisplayDialog("Forge Reports", "Written to " + dir, "OK");
        }

        /// <summary>Collect from all sources and write both reports. Returns the output dir.</summary>
        public static string WriteReports()
        {
            var assets = Collect();
            string dir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Builds", "Reports");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "FORGE_MANIFEST.json"), BuildManifestJson(assets));
            File.WriteAllText(Path.Combine(dir, "FORGE_DEPENDENCY_REPORT.md"), BuildDependencyMd(assets));
            Debug.Log("[Ziptide] Forge reports written to " + dir + " (" + assets.Count + " assets)");
            return dir;
        }

        public static Dictionary<string, ForgeAssetInfo> Collect()
        {
            var assets = new Dictionary<string, ForgeAssetInfo>();
            foreach (var src in Sources)
            {
                try { src.Collect(assets); }
                catch (System.Exception ex) { Debug.LogWarning("[Ziptide] Dependency source '" + src.Name + "': " + ex.Message); }
            }
            return assets;
        }

        // ── v1 source: typed ScriptableObject fields (reliable, no serialization spelunking) ──────

        private class TypedFieldSource : IForgeDependencySource
        {
            public string Name => "typed-fields";

            public void Collect(Dictionary<string, ForgeAssetInfo> assets)
            {
                // Recipes: refs straight from the structured arrays + family + tags.
                foreach (var guid in AssetDatabase.FindAssets("t:ForgeRecipeDefinition"))
                {
                    var r = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                    if (r == null || string.IsNullOrEmpty(r.recipeId)) continue;
                    var refs = new List<string>();
                    void Add(IEnumerable<string> xs) { if (xs != null) refs.AddRange(xs.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLowerInvariant())); }
                    Add(r.storyRefs); Add(r.worldRuleRefs); Add(r.tokenRefs); Add(r.storyTags);
                    if (!string.IsNullOrEmpty(r.surfaceFamily)) refs.Add(r.surfaceFamily.ToLowerInvariant());
                    assets[r.recipeId] = new ForgeAssetInfo
                    {
                        id = r.recipeId,
                        refs = refs.Distinct().OrderBy(x => x).ToArray(),
                        state = r.qualityState,
                        hero = r.storyTags != null && System.Array.IndexOf(r.storyTags, "hero") >= 0,
                        hasConsumers = false, // consumer pass below
                    };
                }

                // Consumers: item definitions that point at recipe ids.
                foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
                {
                    var def = AssetDatabase.LoadAssetAtPath<Ziptide.Content.ItemDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                    if (def == null || string.IsNullOrEmpty(def.forgeRecipeId)) continue;
                    if (assets.TryGetValue(def.forgeRecipeId, out var info)) info.hasConsumers = true;
                }

                // Sky vistas: id + scene mapping from the library (themes consume them per scene).
                foreach (var spec in SkyVistaLibrary.Specs())
                {
                    var v = spec.Value();
                    if (v == null || string.IsNullOrEmpty(v.vistaId)) continue;
                    assets["vista:" + v.vistaId] = new ForgeAssetInfo
                    {
                        id = "vista:" + v.vistaId,
                        refs = new[] { spec.Key.ToLowerInvariant() },
                        state = ForgeQualityState.ProductionCandidate, // shipped + device-gated
                        hero = false,
                        hasConsumers = true, // themes assign them every build
                    };
                }
            }
        }

        // ── Deterministic report writers (sorted, no timestamps) ──────────────────────────────────

        public static string BuildManifestJson(Dictionary<string, ForgeAssetInfo> assets)
        {
            var sb = new StringBuilder(4096);
            sb.Append("{\n  \"assets\": [\n");
            bool first = true;
            foreach (var a in assets.Values.OrderBy(a => a.id, System.StringComparer.Ordinal))
            {
                if (!first) sb.Append(",\n");
                first = false;
                sb.Append("    {\"id\": \"").Append(a.id)
                  .Append("\", \"state\": \"").Append(a.state)
                  .Append("\", \"hero\": ").Append(a.hero ? "true" : "false")
                  .Append(", \"consumed\": ").Append(a.hasConsumers ? "true" : "false")
                  .Append(", \"refs\": [")
                  .Append(string.Join(", ", (a.refs ?? new string[0]).Select(r => "\"" + r + "\"")))
                  .Append("]}");
            }
            sb.Append("\n  ]\n}\n");
            return sb.ToString();
        }

        public static string BuildDependencyMd(Dictionary<string, ForgeAssetInfo> assets)
        {
            var sb = new StringBuilder(4096);
            sb.Append("# FORGE DEPENDENCY REPORT (generated build artifact — never committed)\n\n");
            sb.Append("Query a change: `ForgeStaleness.Affected(assets, changedRef)` or the tests.\n\n");
            sb.Append("| asset | state | hero | consumed | refs |\n|---|---|---|---|---|\n");
            foreach (var a in assets.Values.OrderBy(a => a.id, System.StringComparer.Ordinal))
                sb.Append("| ").Append(a.id).Append(" | ").Append(a.state)
                  .Append(" | ").Append(a.hero ? "yes" : "")
                  .Append(" | ").Append(a.hasConsumers ? "yes" : "NO — deprecation candidate")
                  .Append(" | ").Append(string.Join(" ", a.refs ?? new string[0])).Append(" |\n");
            return sb.ToString();
        }
    }
}
#endif
