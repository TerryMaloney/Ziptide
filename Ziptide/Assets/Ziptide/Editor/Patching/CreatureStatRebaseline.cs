#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// One-way migration of the committed CreatureDefinition assets onto the unified combat scale
    /// (COMBAT_HEALTH_PLAN A3). Idempotent + version-guarded: only assets whose statScaleVersion is
    /// behind <see cref="CreatureBaselines.StatScaleVersion"/> are touched, so a creature you hand-tune
    /// later (already at the current version) is NEVER clobbered — the "existing assets are the editable
    /// truth" law holds. Runs automatically from <see cref="CreatureVariantAuthor.EnsureAllAuthored"/>
    /// (already build-hooked), so the device never ships the old ~5×-tanky creature scale, and there is
    /// a menu for a manual pass.
    /// </summary>
    public static class CreatureStatRebaseline
    {
        private const string EnemiesFolder = "Assets/Ziptide/Resources/Enemies";

        [MenuItem("Ziptide/Worlds/Rebaseline Creature Stats (unified scale)")]
        public static void RebaselineFromMenu()
        {
            int n = RebaselineAll();
            EditorUtility.DisplayDialog("Creature Rebaseline",
                n + " creature asset(s) migrated to the unified combat scale (v" +
                CreatureBaselines.StatScaleVersion + "). Assets already at that version were left alone.",
                "OK");
        }

        /// <summary>Migrate every out-of-date creature asset to the baseline health for its id.
        /// Returns how many changed.</summary>
        public static int RebaselineAll()
        {
            int changed = 0;
            string[] guids = AssetDatabase.FindAssets("t:CreatureDefinition", new[] { EnemiesFolder });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var def = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path);
                if (def == null || def.statScaleVersion >= CreatureBaselines.StatScaleVersion) continue;

                def.maxHealth = CreatureBaselines.HealthFor(def.id);
                def.statScaleVersion = CreatureBaselines.StatScaleVersion;
                EditorUtility.SetDirty(def);
                changed++;
                Debug.Log("[Ziptide] Rebaselined creature " + def.id + " -> hp " + def.maxHealth +
                          " (scale v" + CreatureBaselines.StatScaleVersion + ")");
            }
            if (changed > 0) AssetDatabase.SaveAssets();
            return changed;
        }
    }
}
#endif
