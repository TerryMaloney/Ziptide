#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Validation
{
    /// <summary>
    /// THE BOTH-SIDES WIRING GATE (Phase 2 of the 2026-07-06 wiring audit; see docs/WIRING_MAP.md Part 4
    /// + docs/WIRING_AUDIT_FINDINGS.md). Three models building in parallel kept shipping "wired on one
    /// side but not the other" seams; this proves the deterministic ones statically and FAILS CI via
    /// WiringValidatorTests, so a one-sided seam can't merge. Asset-based + reflection only — no device:
    ///
    ///   1. BUILD-HOOK completeness — every asset author (a type with a public-static
    ///      EnsureAllAuthored/EnsureAuthored/AssignAll/BakeAll) is called in
    ///      BuildAndroid.PatchScenesThenAPK, or its assets ship stale/missing ("nothing I fixed worked").
    ///   2. ITEM→RECIPE — every ItemDefinition.forgeRecipeId resolves to a shipped Resources/Forge recipe.
    ///   3. GENOME→CREATURE — every ForgeCreatureBody has a matching Resources/Enemies CreatureDefinition
    ///      (id convention), so no authored body is spawned by nothing.
    ///
    /// Run: menu <c>Ziptide → Validate wiring</c>, or automatically every CI push via the EditMode test.
    /// EXTENDING IT: add a check method + call it in <see cref="Validate"/>; keep checks deterministic
    /// (asset/reflection, no scene load) so they stay CI-safe. The queued flag grant↔consume check goes here.
    /// </summary>
    public static class WiringValidator
    {
        private static readonly string[] ProducerMethods = { "EnsureAllAuthored", "EnsureAuthored", "AssignAll", "BakeAll" };
        private const string BuildAndroidRelPath = "Ziptide/Editor/Build/BuildAndroid.cs";
        private const string ForgeDir = "Assets/Ziptide/Resources/Forge/";
        private const string EnemiesDir = "Assets/Ziptide/Resources/Enemies/";

        [MenuItem("Ziptide/Validate wiring")]
        public static void ValidateFromMenu()
        {
            bool ok = Validate(out var issues);
            EditorUtility.DisplayDialog(
                ok ? "Wiring Validator" : "Wiring Validator — ONE-SIDED SEAM(S)",
                ok ? "All checked seams are wired both sides. ✅" : string.Join("\n\n", issues),
                "OK");
        }

        /// <summary>True when every checked seam is wired both sides. <paramref name="issues"/> = the gaps.</summary>
        public static bool Validate(out List<string> issues)
        {
            issues = new List<string>();
            CheckBuildHooks(issues);
            CheckItemRecipes(issues);
            CheckCreatureGenomes(issues);
            return issues.Count == 0;
        }

        // 1. Every asset producer is called in BuildAndroid (else it ships stale/missing).
        private static void CheckBuildHooks(List<string> issues)
        {
            string src = Path.Combine(Application.dataPath, BuildAndroidRelPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(src)) { issues.Add("BUILD_HOOK: BuildAndroid.cs not found at " + src); return; }
            string buildSrc = File.ReadAllText(src);

            Type[] types;
            try { types = typeof(WiringValidator).Assembly.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

            foreach (var t in types)
            {
                if (t == null || t == typeof(WiringValidator)) continue;
                if (t.Name.EndsWith("Tests") || t.Name.EndsWith("Validator")) continue;
                bool isProducer = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Any(m => ProducerMethods.Contains(m.Name));
                if (!isProducer) continue;
                // The build must reference the type by name (e.g. "ForgeBodyLibrary.EnsureAllAuthored").
                if (!buildSrc.Contains(t.Name + "."))
                    issues.Add("BUILD_HOOK_MISSING: " + t.Name + " defines an asset-author method but is NOT "
                        + "called in BuildAndroid.PatchScenesThenAPK — its generated assets will ship stale or "
                        + "missing. Add the call (or explain why it's build-exempt).");
            }
        }

        // 2. Every ItemDefinition.forgeRecipeId resolves to a shipped recipe (a typo'd id is a dangling seam).
        private static void CheckItemRecipes(List<string> issues)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
            {
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (def == null || string.IsNullOrEmpty(def.forgeRecipeId)) continue; // unset = primitive look, fine
                var recipe = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(ForgeDir + def.forgeRecipeId + ".asset");
                if (recipe == null)
                    issues.Add("RECIPE_MISSING: item '" + def.name + "' references forgeRecipeId '" + def.forgeRecipeId
                        + "' but Resources/Forge/" + def.forgeRecipeId + ".asset does not exist.");
            }
        }

        // 3. Every ForgeCreatureBody genome has a matching CreatureDefinition (nothing authors a body no creature uses).
        private static void CheckCreatureGenomes(List<string> issues)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:ForgeCreatureBody"))
            {
                var body = AssetDatabase.LoadAssetAtPath<ForgeCreatureBody>(AssetDatabase.GUIDToAssetPath(guid));
                if (body == null || string.IsNullOrEmpty(body.bodyId)) continue;
                var def = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(EnemiesDir + body.bodyId + ".asset");
                if (def == null)
                    issues.Add("GENOME_ORPHAN: ForgeCreatureBody '" + body.bodyId + "' has no matching "
                        + "CreatureDefinition at Resources/Enemies/" + body.bodyId + ".asset — nothing spawns this body.");
            }
        }
    }
}
#endif
