#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// A4.5 — create-only author for the six launch augments (spec ABILITIES_AND_ARSENAL §2, exact
    /// numbers). Seeds AugmentDefinition assets into Resources/Items (the ItemFactory registry path);
    /// existing assets are the editable truth for tuning. ArenaWeaponAuthor pattern; menu + build-safe.
    /// </summary>
    public static class AugmentAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Items";

        [MenuItem("Ziptide/Items/Author Augments (create-only)")]
        public static void EnsureAllAuthored()
        {
            int made = 0;
            // Actives (the spec's numbers: dash 8s cd · bubble 2s/20s · overclock 4s/30s).
            made += Ensure("augment_surge_dash", AugmentKind.Active, "surge_dash",
                cooldown: 8f, duration: 0f, magnitude: 4.5f,          // 4.5m burst
                gem: new Color(0.95f, 0.75f, 0.25f));
            made += Ensure("augment_bubble_guard", AugmentKind.Active, "bubble_guard",
                cooldown: 20f, duration: 2f, magnitude: 1f,
                gem: new Color(0.5f, 0.8f, 1f));
            made += Ensure("augment_overclock", AugmentKind.Active, "overclock",
                cooldown: 30f, duration: 4f, magnitude: 0.5f,          // cooldowns halved
                gem: new Color(0.95f, 0.4f, 0.3f));
            // Passives (magnet 3m · sure step 50% · sixth sense cd halved).
            made += Ensure("augment_magnet_palm", AugmentKind.Passive, "magnet_palm",
                cooldown: 0f, duration: 0f, magnitude: 3f,
                gem: new Color(0.7f, 0.5f, 0.95f));
            made += Ensure("augment_sure_step", AugmentKind.Passive, "sure_step",
                cooldown: 0f, duration: 0f, magnitude: 0.5f,
                gem: new Color(0.45f, 0.85f, 0.5f));
            made += Ensure("augment_sixth_sense", AugmentKind.Passive, "sixth_sense",
                cooldown: 0f, duration: 0f, magnitude: 0.5f,
                gem: new Color(0.3f, 0.8f, 0.95f));
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            Debug.Log("[Ziptide] AugmentAuthor: " + made + " new asset(s); existing left untouched.");
        }

        private static int Ensure(string itemId, AugmentKind kind, string effectId,
            float cooldown, float duration, float magnitude, Color gem)
        {
            // augment_surge_dash -> AugmentSurgeDash.asset (registry casing convention).
            var parts = itemId.Split('_');
            var sb = new System.Text.StringBuilder();
            foreach (var p in parts)
                if (p.Length > 0) sb.Append(char.ToUpperInvariant(p[0])).Append(p.Substring(1));
            string path = Folder + "/" + sb + ".asset";

            if (AssetDatabase.LoadAssetAtPath<AugmentDefinition>(path) != null) return 0;
            Directory.CreateDirectory(Folder);
            var def = ScriptableObject.CreateInstance<AugmentDefinition>();
            def.itemId = itemId;
            def.augmentKind = kind;
            def.effectId = effectId;
            def.cooldownSeconds = cooldown;
            def.durationSeconds = duration;
            def.magnitude = magnitude;
            def.gemColor = gem;
            AssetDatabase.CreateAsset(def, path);
            return 1;
        }
    }
}
#endif
