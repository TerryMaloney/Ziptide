#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors the creature DATA layer (create-only — existing assets are the editable truth):
    /// 1. Drone difficulty variants → `Resources/Enemies/<variantId>` DroneCombatProfile assets. A
    ///    world's layout sets `DroneZoneDef.variantId` to pick one; `CityBuilder.MakeDrone` loads it.
    ///    Tuning drone difficulty per world = edit ONE profile asset, no code.
    /// 2. The story-creature catalog → CreatureDefinition assets (stats/loot/biome per the WORLD_DATA
    ///    records). NOT consumed at runtime yet (Phase E builds CreatureRuntime against them) — authored
    ///    now so Phase E starts data-complete.
    /// Called from BuildAndroid so the assets always exist; menu for manual runs.
    /// </summary>
    public static class CreatureVariantAuthor
    {
        private const string EnemiesResourceFolder = "Assets/Ziptide/Resources/Enemies";

        [MenuItem("Ziptide/Worlds/Author Creature Data (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Creature Data",
                made + " asset(s) created (drone variants + creature catalog). Existing ones untouched.", "OK");
        }

        public static int EnsureAllAuthored()
        {
            int made = 0;

            // ── Drone difficulty bands (chapter-scaled; values riff on DroneCombatProfile defaults) ──
            made += Variant("drone_easy", p =>       // Ch.1 — forgiving: slow bolts, long telegraph
            {
                p.detectRange = 8f; p.standoffDistance = 5.5f; p.orbitSpeed = 32f;
                p.telegraphSeconds = 1.2f; p.boltCooldown = 3.2f; p.boltSpeed = 5f;
                p.stunSeconds = 1.0f; p.slowFactor = 0.55f;
            });
            made += Variant("drone_standard", p =>   // Ch.2–3 — the serialized defaults, explicit
            {
                p.detectRange = 10f; p.standoffDistance = 5f; p.orbitSpeed = 40f;
                p.telegraphSeconds = 0.9f; p.boltCooldown = 2.5f; p.boltSpeed = 6f;
                p.stunSeconds = 1.2f; p.slowFactor = 0.45f;
            });
            made += Variant("drone_veteran", p =>    // Ch.4+ — pushy: closer orbit, faster cycle
            {
                p.detectRange = 12f; p.standoffDistance = 4f; p.orbitSpeed = 52f;
                p.telegraphSeconds = 0.7f; p.boltCooldown = 1.9f; p.boltSpeed = 7.5f;
                p.stunSeconds = 1.4f; p.slowFactor = 0.4f;
            });

            // ── Story-creature catalog (ids match the WORLD_DATA `creatures:` lines; the behavior is
            //    picked by CityBuilder.MakeCreature — special ids override the archetype default) ──
            made += Creature("swarm_bug", CreatureArchetype.Swarmer, hp: 8f, speed: 4.5f, dmg: 2f,
                biome: "dry_cistern", loot: ("carapace", 1));
            made += Creature("tendril", CreatureArchetype.WallCrawler, hp: 20f, speed: 2f, dmg: 4f,
                biome: "glass_shelf", loot: ("spore", 1));
            // M3 novel behaviors + the Warden:
            made += Creature("light_grazer", CreatureArchetype.Swarmer, hp: 14f, speed: 1.6f, dmg: 2f,
                biome: "dry_cistern", loot: ("spore", 2));
            made += Creature("witness_mite", CreatureArchetype.Swarmer, hp: 10f, speed: 3.5f, dmg: 2f,
                biome: "mirror_flats", loot: ("prism", 1));
            made += Creature("tether_swarm", CreatureArchetype.Swarmer, hp: 16f, speed: 2f, dmg: 3f,
                biome: "chitinwall", loot: ("carapace", 3));
            made += Creature("husk_molter", CreatureArchetype.WallCrawler, hp: 18f, speed: 2.4f, dmg: 3f,
                biome: "chitinwall", loot: ("carapace", 2));
            made += Creature("warden", CreatureArchetype.Bruiser, hp: 60f, speed: 3.2f, dmg: 0f,
                biome: "", loot: ("data_chip", 1)); // lawful — damage 0; the arrest is a stun

            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static int Variant(string id, System.Action<DroneCombatProfile> tune)
        {
            string path = EnemiesResourceFolder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<DroneCombatProfile>(path) != null) return 0;
            Directory.CreateDirectory(EnemiesResourceFolder);
            var p = ScriptableObject.CreateInstance<DroneCombatProfile>();
            tune(p);
            AssetDatabase.CreateAsset(p, path);
            Debug.Log("[Ziptide] Authored drone variant " + path);
            return 1;
        }

        private static int Creature(string id, CreatureArchetype archetype, float hp, float speed, float dmg,
            string biome, (string resourceId, double amount) loot)
        {
            // Creatures MUST live under Resources/Enemies: CreatureRuntime resolves its definition at
            // runtime via Resources.Load("Enemies/<id>") and CityBuilder picks the behavior from the
            // same path at build time. (Was Content/Creatures/Generated — unreachable at runtime.)
            string path = EnemiesResourceFolder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path) != null) return 0;
            Directory.CreateDirectory(EnemiesResourceFolder);
            var c = ScriptableObject.CreateInstance<CreatureDefinition>();
            c.id = id; // Definition-registry id (matches WORLD_DATA `creatures:` lines)
            c.archetype = archetype;
            c.maxHealth = hp; c.moveSpeed = speed; c.damage = dmg;
            c.biomeId = biome; c.shockable = true;
            c.loot.Add(new ResourceCost { resourceId = loot.resourceId, amount = loot.amount });
            AssetDatabase.CreateAsset(c, path);
            Debug.Log("[Ziptide] Authored creature " + path);
            return 1;
        }
    }
}
#endif
