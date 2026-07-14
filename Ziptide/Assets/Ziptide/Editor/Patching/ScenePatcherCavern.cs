#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Editor.Art;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// HARDWIRING 1.4e/g — THE CAVE FACTORY: walkable caves generated end-to-end from the tested pure
    /// spine. `CaveNetworkPlanner.Plan(seed)` decides the network (connected by construction);
    /// `CavernKitLibrary` modules give it a body (registry-driven — the Forge kit supersedes the look
    /// later without touching this file); the traversal runtimes make it crossable: bridges on
    /// walkable tunnels, CLIMBABLE shaft walls (+ a lift beside the deep ones), grapple anchors on
    /// the high chambers, one zipline highest→lowest. Parameterized (1.4g) so caves are WORLDS, not
    /// just a lab: each config = scene + pack + return door, Dev-Warp reachable. Two shipped configs:
    ///  · CAVERN TEST LAB — the fixed-seed dev cave everyone compares notes on.
    ///  · THE UNDERCROFT (W011's deep layer) — the first cave that is a PLACE: bigger, deeper, story
    ///    return door to W011_TheHum. RILL speaks on entry (RillLineAuthor, additive).
    /// Idempotent per the sandbox contract.
    /// </summary>
    public static class ScenePatcherCavern
    {
        private class CaveConfig
        {
            public string sceneName, packId, displayName, returnDestination;
            public int seed;
            public float extentX, extentZ, depth;
            public int chambers;
            public float minSpacing, loopChance;
        }

        private static readonly CaveConfig TestLab = new CaveConfig
        {
            sceneName = "Cavern_TestLab", packId = "cavern_lab", displayName = "Cavern Test Lab",
            returnDestination = "MilestoneA_GrabCube",
            seed = 20260709, extentX = 26f, extentZ = 26f, depth = 14f,
            chambers = 8, minSpacing = 11f, loopChance = 0.3f,
        };

        // The Undercroft: The Hum's deep layer (W011 — "that sound is in the rock"). Bigger, deeper,
        // loopier than the lab; its return door goes home to W011 itself.
        private static readonly CaveConfig Undercroft = new CaveConfig
        {
            sceneName = "W011_Undercroft", packId = "w011_undercroft", displayName = "The Undercroft",
            returnDestination = "W011_TheHum",
            seed = 1101, extentX = 34f, extentZ = 34f, depth = 22f,
            chambers = 11, minSpacing = 11f, loopChance = 0.45f,
        };

        private const string DefaultWorldProfilePath = "Assets/Ziptide/Content/World/DefaultWorldProfile.asset";

        [MenuItem("Ziptide/Dev/Build Cavern Test Lab")]
        public static void BuildTestLab() => Build(TestLab);

        [MenuItem("Ziptide/Worlds/Build W011 Undercroft (cave world)")]
        public static void BuildUndercroft() => Build(Undercroft);

        /// <summary>
        /// Batchmode-safe build hook. W011's surface cave mouth is generated automatically, so its
        /// destination must be generated and enabled before the world audit reads Build Settings.
        /// </summary>
        public static void EnsureUndercroftInBuildSettings()
        {
            EditorSceneManager.SaveOpenScenes();
            string scenePath = ScenePathOf(Undercroft);
            Scene scene;
            if (File.Exists(scenePath))
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }

            Populate(Undercroft);
            EnsureInBuildSettings(scenePath);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] ensured batch cave world " + Undercroft.sceneName + " in Build Settings");
        }

        private static void Build(CaveConfig cfg)
        {
            var scene = OpenOrCreateScene(ScenePathOf(cfg));
            Populate(cfg);
            EnsureInBuildSettings(ScenePathOf(cfg));
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePathOf(cfg));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(cfg.displayName,
                "Built/updated " + cfg.sceneName + " (seed " + cfg.seed + ").\nWarp in via Ziptide > Dev > Warp Window.", "OK");
        }

        private static string ScenePathOf(CaveConfig cfg) => "Assets/Scenes/" + cfg.sceneName + ".unity";
        private static string PackPathOf(CaveConfig cfg)
            => "Assets/Ziptide/Content/Worlds/Packs/" + cfg.sceneName + "_WorldPack.asset";

        /// <summary>Populate the currently-open cave scene from its config. Idempotent by name.</summary>
        private static void Populate(CaveConfig cfg)
        {
            CavernKitLibrary.EnsureRegistered();
            var plan = CaveNetworkPlanner.Plan(cfg.seed, cfg.extentX, cfg.extentZ, cfg.depth,
                                               cfg.chambers, cfg.minSpacing, cfg.loopChance);

            EnsureLighting();
            EnsureWorldRuntime();
            EnsureCatchFloor(plan);

            // Chambers: registry floor pads + stalactites overhead.
            foreach (var ch in plan.Chambers)
            {
                string padName = "CavePad_" + ch.Id;
                if (GameObject.Find(padName) == null &&
                    ArtModuleRegistry.TryBuild("cavernModule:rock/FloorPad", out var pad))
                {
                    pad.name = padName;
                    pad.transform.position = new Vector3(ch.X, ch.Y, ch.Z);
                    pad.transform.localScale = new Vector3(ch.Radius, 1f, ch.Radius);
                }
                for (int s = 0; s < 3; s++)
                {
                    string stName = "CaveStal_" + ch.Id + "_" + s;
                    if (GameObject.Find(stName) != null) continue;
                    if (!ArtModuleRegistry.TryBuild("cavernModule:rock/Stalactite", out var stal)) continue;
                    stal.name = stName;
                    float a = (ch.Id * 2.4f) + s * 2.1f;
                    stal.transform.position = new Vector3(
                        ch.X + Mathf.Cos(a) * ch.Radius * 0.5f,
                        ch.Y + 6.5f,
                        ch.Z + Mathf.Sin(a) * ch.Radius * 0.5f);
                }
            }

            // Tunnels: bridges for walkable runs; climbable shaft walls (+ a lift when deep) for steep.
            foreach (var t in plan.Tunnels)
            {
                var a = plan.Chambers[t.FromId];
                var b = plan.Chambers[t.ToId];
                string linkName = "CaveLink_" + t.FromId + "_" + t.ToId;
                if (GameObject.Find(linkName) != null) continue;
                var link = new GameObject(linkName);
                if (!t.IsShaft) BuildBridge(link.transform, a, b);
                else BuildShaft(link.transform, a, b);
            }

            EnsureGrappleAnchors(plan);
            EnsureCaveZipline(plan);
            EnsureSpawnAndDoor(plan, cfg);
            EnsureWorldPackAsset(plan, cfg);
        }

        private static void BuildBridge(Transform parent, CaveChamber a, CaveChamber b)
        {
            Vector3 pa = new Vector3(a.X, a.Y, a.Z);
            Vector3 pb = new Vector3(b.X, b.Y, b.Z);
            var span = GameObject.CreatePrimitive(PrimitiveType.Cube);
            span.name = "Bridge";
            span.transform.SetParent(parent, true);
            span.transform.position = (pa + pb) * 0.5f + Vector3.up * 0.02f;
            span.transform.rotation = Quaternion.LookRotation(pb - pa);
            span.transform.localScale = new Vector3(2.4f, 0.14f, Vector3.Distance(pa, pb));
            ItemFactory.ApplyURPColor(span, new Color(0.20f, 0.185f, 0.175f));
        }

        private static void BuildShaft(Transform parent, CaveChamber a, CaveChamber b)
        {
            var lower = a.Y <= b.Y ? a : b;
            var upper = a.Y <= b.Y ? b : a;
            Vector3 pl = new Vector3(lower.X, lower.Y, lower.Z);
            Vector3 pu = new Vector3(upper.X, upper.Y, upper.Z);
            float height = pu.y - pl.y;

            Vector3 flat = new Vector3(pu.x - pl.x, 0f, pu.z - pl.z);
            Vector3 dir = flat.sqrMagnitude > 0.01f ? flat.normalized : Vector3.forward;
            Vector3 basePos = pl + dir * (lower.Radius * 0.8f);
            if (ArtModuleRegistry.TryBuild("cavernModule:rock/ShaftWall", out var wall))
            {
                wall.name = "ShaftWall";
                wall.transform.SetParent(parent, true);
                wall.transform.position = basePos + Vector3.up * (height * 0.5f);
                wall.transform.rotation = Quaternion.LookRotation(dir);
                wall.transform.localScale = new Vector3(2.2f, height + 1f, 1f);
                wall.AddComponent<ClimbableSurface>();
            }
            if (height > 5f)
            {
                var lift = new GameObject("ShaftLift");
                lift.transform.SetParent(parent, true);
                lift.AddComponent<LiftRuntime>().Init(
                    new[] { basePos + dir * 1.6f + Vector3.up * 0.15f,
                            pu - dir * (upper.Radius * 0.5f) + Vector3.up * 0.15f },
                    speed: 1.6f, dwell: 4f);
            }
        }

        private static void EnsureGrappleAnchors(CavePlan plan)
        {
            var ys = new List<float>();
            foreach (var c in plan.Chambers) ys.Add(c.Y);
            ys.Sort();
            float median = ys[ys.Count / 2];
            foreach (var ch in plan.Chambers)
            {
                if (ch.Y <= median) continue;
                string name = "CaveGrapple_" + ch.Id;
                if (GameObject.Find(name) != null) continue;
                var anchor = new GameObject(name);
                anchor.transform.position = new Vector3(ch.X, ch.Y + 3.2f, ch.Z);
                anchor.AddComponent<GrappleAnchorRuntime>();
            }
        }

        private static void EnsureCaveZipline(CavePlan plan)
        {
            if (GameObject.Find("CaveZipline") != null || plan.Chambers.Count < 2) return;
            CaveChamber hi = plan.Chambers[0], lo = plan.Chambers[0];
            foreach (var c in plan.Chambers)
            {
                if (c.Y > hi.Y) hi = c;
                if (c.Y < lo.Y) lo = c;
            }
            if (hi == lo) return;
            var zip = new GameObject("CaveZipline");
            zip.AddComponent<ZiplineRuntime>().Init(
                new Vector3(hi.X, hi.Y + 2.6f, hi.Z),
                new Vector3(lo.X, lo.Y + 1.6f, lo.Z));
        }

        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 6f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 0.35f;
            light.color = new Color(0.7f, 0.8f, 0.9f);
            go.transform.rotation = Quaternion.Euler(75f, -20f, 0f);
        }

        private static void EnsureWorldRuntime()
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            var profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldProfilePath);
            if (profile != null)
            {
                var so = new SerializedObject(wr);
                PatcherUtil.SetObjectRef(so, "worldProfile", profile);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureCatchFloor(CavePlan plan)
        {
            float minY = 0f;
            foreach (var c in plan.Chambers) if (c.Y < minY) minY = c.Y;
            var floor = GameObject.Find("CaveCatchFloor");
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "CaveCatchFloor";
            }
            floor.transform.position = new Vector3(0f, minY - 3f, 0f);
            floor.transform.localScale = new Vector3(9f, 1f, 9f);
            ItemFactory.ApplyURPColor(floor, new Color(0.12f, 0.115f, 0.11f));
        }

        private static void EnsureSpawnAndDoor(CavePlan plan, CaveConfig cfg)
        {
            var first = plan.Chambers[0];
            Vector3 spawnPos = new Vector3(first.X, first.Y + 0.25f, first.Z);
            var go = PatcherUtil.EnsureRootObject("__SPAWN_PLAYER", spawnPos);
            var marker = PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", "player");
            so.ApplyModifiedPropertiesWithoutUndo();

            var door = PatcherUtil.EnsureRootObject("__ReturnDoor",
                spawnPos + new Vector3(0f, 1.0f, -Mathf.Max(2.5f, first.Radius * 0.7f)));
            var box = PatcherUtil.EnsureComponent<BoxCollider>(door);
            box.isTrigger = true;
            box.size = new Vector3(3f, 2.5f, 1f);
            var trigger = PatcherUtil.EnsureComponent<ProximityTravelTrigger>(door);
            trigger.SetDestination(cfg.returnDestination);
        }

        private static void EnsureWorldPackAsset(CavePlan plan, CaveConfig cfg)
        {
            string packPath = PackPathOf(cfg);
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(packPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(packPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, packPath);
            }
            pack.packId = cfg.packId;
            pack.displayName = cfg.displayName;
            pack.sceneName = cfg.sceneName;
            pack.spawnMarkers.Clear();
            var first = plan.Chambers[0];
            pack.spawnMarkers.Add(new SpawnMarkerDefinition
            {
                markerId = "player",
                localPosition = new Vector3(first.X, first.Y + 0.25f, first.Z)
            });
            EditorUtility.SetDirty(pack);
        }

        private static Scene OpenOrCreateScene(string scenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return EditorSceneManager.GetActiveScene();
            if (File.Exists(scenePath))
                return EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
            EditorSceneManager.SaveScene(scene, scenePath);
            return scene;
        }

        private static void EnsureInBuildSettings(string scenePath)
        {
            string normalized = scenePath.Replace('\\', '/');
            if (!File.Exists(scenePath)) return;
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0)
            {
                scenes.Add(new EditorBuildSettingsScene(normalized, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            else if (!scenes[idx].enabled)
            {
                scenes[idx] = new EditorBuildSettingsScene(normalized, true);
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }
    }
}
#endif
