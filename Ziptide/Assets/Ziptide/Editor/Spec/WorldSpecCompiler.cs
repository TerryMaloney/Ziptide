#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Spec
{
    /// <summary>
    /// THE WORLDSPEC COMPILER (ARCHITECTURE V2 Q1). Reads `docs/worldspecs/*.spec.json`, validates
    /// against the REAL registries, and fans each spec out into the existing factory assets:
    /// the world's CityLayoutDefinition (Content/City/Generated/&lt;scene&gt;_Layout.asset) and its
    /// WorldPackDefinition (Content/Worlds/Packs/&lt;scene&gt;_WorldPack.asset). Generates no geometry —
    /// the proven build pipeline (WorldStubGenerator → CityBuilder → experience/POI/dressing builders)
    /// consumes what this writes. Also EXPORTS any world back to a spec, so every existing world gets
    /// a starting document and the round-trip proves fidelity (SPEC_DRIFT warns when they diverge).
    /// Build-hooked after WorldLayoutLibrary seeding: library seeds defaults once, specs are the
    /// editable truth on top.
    /// </summary>
    public static class WorldSpecCompiler
    {
        private const string LayoutFolder = "Assets/Ziptide/Content/City/Generated";
        private const string PackFolder = "Assets/Ziptide/Content/Worlds/Packs";

        /// <summary>Repo-relative spec folder (the Unity project lives in Ziptide/).</summary>
        public static string SpecFolder => Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/worldspecs"));

        // ── Entry points ─────────────────────────────────────────────────────────────────────────
        [MenuItem("Ziptide/Worlds/Compile World Specs (docs-worldspecs)")]
        public static void CompileAllFromMenu()
        {
            int ok = CompileAll();
            EditorUtility.DisplayDialog("WorldSpec", ok + " spec(s) compiled. Red console lines = rejected specs.", "OK");
        }

        /// <summary>Compile every docs/worldspecs/*.spec.json. Returns how many applied cleanly.</summary>
        public static int CompileAll()
        {
            if (!Directory.Exists(SpecFolder)) return 0;
            int ok = 0;
            foreach (var file in Directory.GetFiles(SpecFolder, "*.spec.json"))
                if (CompileFile(file)) ok++;
            if (ok > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return ok;
        }

        public static bool CompileFile(string path)
        {
            WorldSpec spec;
            try { spec = WorldSpec.FromJson(File.ReadAllText(path)); }
            catch (System.Exception ex)
            {
                Debug.LogError("[Ziptide] SPEC_PARSE_FAILED " + Path.GetFileName(path) + ": " + ex.Message);
                return false;
            }

            var issues = WorldSpecValidator.Validate(spec, BuildRegistry());
            if (issues.Count > 0)
            {
                Debug.LogError("[Ziptide] SPEC_REJECTED " + Path.GetFileName(path) + " ("
                    + issues.Count + " issue(s)):\n  " + string.Join("\n  ", issues));
                return false;
            }

            ApplyToLayout(spec);
            ApplyToPack(spec);
            Debug.Log("[Ziptide] SPEC_APPLIED " + spec.sceneName);

            // SPEC_DRIFT: the round-trip must reproduce the input — a difference means an asset field
            // the spec doesn't cover was hand-edited (or the spec file is stale). WARN, never block.
            var reExported = ExportSpec(spec.sceneName);
            if (reExported != null && Normalize(reExported.ToJson()) != Normalize(File.ReadAllText(path)))
                Debug.LogWarning("ZIPTIDE: SPEC_DRIFT scene=" + spec.sceneName
                    + " (assets diverge from " + Path.GetFileName(path) + " — re-export via Ziptide menu to reconcile)");
            return true;
        }

        [MenuItem("Ziptide/Worlds/Export All World Specs (JSON)")]
        public static void ExportAllFromMenu()
        {
            Directory.CreateDirectory(SpecFolder);
            int n = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:CityLayoutDefinition"))
            {
                var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (kit == null || string.IsNullOrEmpty(kit.sceneName)) continue;
                var spec = ExportSpec(kit.sceneName);
                if (spec == null) continue;
                File.WriteAllText(Path.Combine(SpecFolder, kit.sceneName + ".spec.json"), spec.ToJson());
                n++;
            }
            EditorUtility.DisplayDialog("WorldSpec", n + " spec(s) exported to docs/worldspecs.", "OK");
        }

        // ── Spec → assets ────────────────────────────────────────────────────────────────────────
        private static void ApplyToLayout(WorldSpec s)
        {
            string path = LayoutFolder + "/" + s.sceneName + "_Layout.asset";
            var kit = FindLayoutBySceneName(s.sceneName);
            if (kit == null)
            {
                Directory.CreateDirectory(LayoutFolder);
                kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
                AssetDatabase.CreateAsset(kit, path);
            }

            kit.cityId = s.cityId;
            kit.seed = s.seed;
            kit.sceneName = s.sceneName;
            kit.displayName = s.displayName;
            kit.spawnDistrictId = s.spawnDistrictId;
            kit.spawnStarterWeapons = s.spawnStarterWeapons;
            kit.walkwayHeight = s.walkwayHeight;
            kit.palette = s.palette;
            kit.fogEnabled = s.fogEnabled;
            kit.fogColor = s.fogColor;
            kit.fogDensity = s.fogDensity;
            kit.skyHorizonColor = s.skyHorizonColor;
            kit.skyTopColor = s.skyTopColor;
            kit.themeGroundTint = s.themeGroundTint;
            kit.planetVisible = s.planetVisible;
            kit.planetBaseColor = s.planetBaseColor;
            kit.planetAccentColor = s.planetAccentColor;
            kit.planetAngularSize = s.planetAngularSize;
            kit.skylineRingRadius = s.skylineRingRadius;
            kit.skylineCount = s.skylineCount;
            kit.skylineMinHeight = s.skylineMinHeight;
            kit.skylineMaxHeight = s.skylineMaxHeight;
            kit.experience = s.experience;
            kit.pois = s.pois;
            kit.districts = s.districts;
            kit.connections = s.connections;
            kit.canals = s.canals;
            kit.droneZones = s.droneZones;
            kit.hazards = s.hazards;
            kit.creatureZones = s.creatureZones;
            kit.shipyard = s.shipyard;
            EditorUtility.SetDirty(kit);
        }

        private static void ApplyToPack(WorldSpec s)
        {
            string path = PackFolder + "/" + s.sceneName + "_WorldPack.asset";
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (pack == null)
            {
                Directory.CreateDirectory(PackFolder);
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                pack.packId = s.cityId;
                pack.sceneName = s.sceneName;
                AssetDatabase.CreateAsset(pack, path);
                // WorldStubGenerator enriches (spawn markers, jobs, exit pack) at build — load-or-create there.
            }
            pack.displayName = s.displayName;
            pack.collectibles = s.collectibles;
            pack.machines = s.machines;
            pack.mines = s.mines;
            pack.gardens = s.gardens;
            pack.sockets = s.sockets;
            pack.flagsRequired = s.flagsRequired;
            pack.flagsGranted = s.flagsGranted;
            EditorUtility.SetDirty(pack);
        }

        // ── Assets → spec ────────────────────────────────────────────────────────────────────────
        public static WorldSpec ExportSpec(string sceneName)
        {
            var kit = FindLayoutBySceneName(sceneName);
            if (kit == null) return null;
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(PackFolder + "/" + sceneName + "_WorldPack.asset");

            var s = new WorldSpec
            {
                sceneName = kit.sceneName,
                cityId = kit.cityId,
                displayName = kit.displayName,
                seed = kit.seed,
                spawnDistrictId = kit.spawnDistrictId,
                spawnStarterWeapons = kit.spawnStarterWeapons,
                walkwayHeight = kit.walkwayHeight,
                palette = kit.palette,
                fogEnabled = kit.fogEnabled,
                fogColor = kit.fogColor,
                fogDensity = kit.fogDensity,
                skyHorizonColor = kit.skyHorizonColor,
                skyTopColor = kit.skyTopColor,
                themeGroundTint = kit.themeGroundTint,
                planetVisible = kit.planetVisible,
                planetBaseColor = kit.planetBaseColor,
                planetAccentColor = kit.planetAccentColor,
                planetAngularSize = kit.planetAngularSize,
                skylineRingRadius = kit.skylineRingRadius,
                skylineCount = kit.skylineCount,
                skylineMinHeight = kit.skylineMinHeight,
                skylineMaxHeight = kit.skylineMaxHeight,
                experience = kit.experience,
                pois = kit.pois,
                districts = kit.districts,
                connections = kit.connections,
                canals = kit.canals,
                droneZones = kit.droneZones,
                hazards = kit.hazards,
                creatureZones = kit.creatureZones,
                shipyard = kit.shipyard,
            };
            if (pack != null)
            {
                s.collectibles = pack.collectibles;
                s.machines = pack.machines;
                s.mines = pack.mines;
                s.gardens = pack.gardens;
                s.sockets = pack.sockets;
                s.flagsRequired = pack.flagsRequired;
                s.flagsGranted = pack.flagsGranted;
            }
            return s;
        }

        // ── Registry (real ids for the validator) ────────────────────────────────────────────────
        public static WorldSpecRegistry BuildRegistry()
        {
            var reg = new WorldSpecRegistry
            {
                CreatureIds = AssetNames("t:CreatureDefinition"),
                PlantIds = AssetNames("t:PlantDefinition"),
                ItemIds = new HashSet<string>(),
                // Sky ids stay null (permissive) until sky binding integrates with the spec — the
                // library maps by scene name today (documented v1 limitation).
            };
            foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
            {
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (def != null && !string.IsNullOrEmpty(def.itemId)) reg.ItemIds.Add(def.itemId);
            }
            return reg;
        }

        private static HashSet<string> AssetNames(string filter)
        {
            var set = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets(filter))
                set.Add(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)));
            return set;
        }

        private static CityLayoutDefinition FindLayoutBySceneName(string sceneName)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:CityLayoutDefinition"))
            {
                var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (kit != null && kit.sceneName == sceneName) return kit;
            }
            return null;
        }

        private static string Normalize(string json) =>
            json.Replace("\r\n", "\n").Trim();
    }
}
#endif
