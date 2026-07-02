#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Create-only author for the A4 arsenal definitions (BotProfileAuthor pattern): seeds the three
    /// ArenaWeaponDefinition assets into Resources/Items (the ItemFactory registry path) if missing,
    /// never overwrites — the assets are the editable truth for feel tuning. Build-hooked in
    /// BuildAndroid so CI ships them; menu for editor preview.
    /// </summary>
    public static class ArenaWeaponAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Items";

        [MenuItem("Ziptide/Items/Author Arena Weapons (create-only)")]
        public static void EnsureAllAuthored()
        {
            int made = 0;
            made += Ensure("static_net", ArenaWeaponKind.StaticNet, cooldown: 1.4f);
            made += Ensure("sonic_thumper", ArenaWeaponKind.SonicThumper, cooldown: 1.2f);
            made += Ensure("prism_beam", ArenaWeaponKind.PrismBeam, cooldown: 0.0f); // prism paces itself (charge+cd)
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            Debug.Log("[Ziptide] ArenaWeaponAuthor: " + made + " new asset(s); existing left untouched.");
        }

        private static int Ensure(string itemId, ArenaWeaponKind kind, float cooldown)
        {
            string path = Folder + "/" + ToAssetName(itemId) + ".asset";
            if (AssetDatabase.LoadAssetAtPath<ArenaWeaponDefinition>(path) != null) return 0;
            Directory.CreateDirectory(Folder);
            var def = ScriptableObject.CreateInstance<ArenaWeaponDefinition>();
            def.itemId = itemId;
            def.kind = kind;
            if (cooldown > 0f) def.fireCooldown = cooldown;
            AssetDatabase.CreateAsset(def, path);
            return 1;
        }

        private static string ToAssetName(string itemId)
        {
            // static_net -> StaticNet.asset (matches DefaultPistol/DefaultTaserDartGun casing style)
            var parts = itemId.Split('_');
            var sb = new System.Text.StringBuilder();
            foreach (var p in parts)
                if (p.Length > 0) sb.Append(char.ToUpperInvariant(p[0])).Append(p.Substring(1));
            return sb.ToString();
        }
    }
}
#endif
