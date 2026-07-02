#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE FIRST SUPPLY DROP (QUARTERS.md): authors CosmeticDefinition assets into Resources/Cosmetics
    /// so the Quarters bays stop being stubs. CREATE-ONLY (the asset is the live, tunable truth —
    /// tweak colors there; add new drops here). Every cosmetic is EARNED by story progress via
    /// ownedFlag — the flags below are already granted by the shipped contracts, so playing the arc
    /// stocks the locker with zero extra wiring. Everything is a LOOK, never a stat.
    /// Wired into BuildAndroid next to the other data authors; also runnable from the menu.
    /// </summary>
    public static class CosmeticAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Cosmetics";

        [MenuItem("Ziptide/Story/Author Cosmetics Drop (missing only)")]
        public static void EnsureAuthored()
        {
            int made = 0;

            // Free starter skin — everyone learns the equip flow with something in the bay.
            made += Skin("taser_rustline", "Rustline", CosmeticKind.WeaponSkin, "taser_dart_gun",
                body: new Color(0.62f, 0.36f, 0.18f), ownedFlag: "");

            // Ch.1 rewards: finishing early worlds stocks the locker.
            made += Skin("taser_tidebreak", "Tidebreak", CosmeticKind.WeaponSkin, "taser_dart_gun",
                body: new Color(0.16f, 0.62f, 0.66f), ownedFlag: ZiptideFlags.W002_COMPLETE);
            made += Skin("grav_ember", "Ember Coil", CosmeticKind.WeaponSkin, "gravity_gun",
                body: new Color(0.85f, 0.42f, 0.16f), ownedFlag: ZiptideFlags.W004_COMPLETE);

            // The Ch.2 capstone earns the prestige skin — you SAW the cage; your gun remembers.
            made += Skin("grav_voidglass", "Voidglass", CosmeticKind.WeaponSkin, "gravity_gun",
                body: new Color(0.16f, 0.10f, 0.28f), ownedFlag: ZiptideFlags.C2_CONTAINMENT_REVEALED);

            // First bounty unlocks the Guild livery + emblem (toxiccity_complete = the W001 contract).
            made += Skin("livery_wakeguild", "Wake Guild Livery", CosmeticKind.ShipLivery, "",
                body: new Color(0.72f, 0.58f, 0.22f), ownedFlag: "toxiccity_complete");
            made += Skin("emblem_firstcontract", "First Contract", CosmeticKind.Emblem, "",
                body: new Color(0.85f, 0.70f, 0.30f), ownedFlag: "toxiccity_complete");

            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            Debug.Log("[Ziptide] Cosmetics drop: " + made + " new asset(s) under " + Folder);
        }

        private static int Skin(string id, string displayName, CosmeticKind kind, string targetItemId,
                                Color body, string ownedFlag)
        {
            string path = Folder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<CosmeticDefinition>(path) != null) return 0;
            Directory.CreateDirectory(Folder);
            var c = ScriptableObject.CreateInstance<CosmeticDefinition>();
            c.cosmeticId = id;
            c.displayName = displayName;
            c.kind = kind;
            c.targetItemId = targetItemId;
            c.bodyColor = new Color(body.r, body.g, body.b, 1f);
            c.ownedFlag = ownedFlag;
            AssetDatabase.CreateAsset(c, path);
            return 1;
        }
    }
}
#endif
