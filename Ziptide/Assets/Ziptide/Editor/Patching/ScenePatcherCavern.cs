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
    /// HARDWIRING 1.4e — THE CAVERN TEST LAB: the first walkable cave, generated end-to-end from the
    /// tested pure spine. `CaveNetworkPlanner.Plan(seed)` decides the network (connected by
    /// construction); `CavernKitLibrary` modules give it a body (registry-driven — the Forge kit
    /// supersedes the look later without touching this file); the traversal runtimes make it
    /// crossable: bridges on walkable tunnels, CLIMBABLE shaft walls on steep ones (+ a lift beside
    /// the deep shafts), and one zipline from the highest chamber back to the lowest. Dim crystal-lit
    /// mood, catch-floor far below (fall safety), spawn + world pack + return door so Dev Warp can
    /// jump straight in. Idempotent per the sandbox contract. Menu: Ziptide → Dev → Build Cavern
    /// Test Lab (adds itself to Build Settings).
    /// </summary>
    public static class ScenePatcherCavern
    {
        private const string SceneName = "Cavern_TestLab";
        private const string ScenePath = "Assets/Scenes/" + SceneName + ".unity";
        private const string WorldPackPath = "Assets/Ziptide/Content/Worlds/Packs/Cavern_WorldPack.asset";
        private const string DefaultWorldProfilePath = "Assets/Ziptide/Content/World/DefaultWorldProfile.asset";
        private const int Seed = 20260709;   // fixed: the lab is ONE cave everyone can compare notes on

        [MenuItem("Ziptide/Dev/Build Cavern Test Lab")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActiveCavern();
            EnsureInBuildSettings();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Cavern Test Lab",
                "Built/updated " + SceneName + " (seed " + Seed + ").\nWarp in via Ziptide > Dev > Warp Window.", "OK");
        }

        /// <summary>Populate the currently-open cavern scene. Idempotent (find-or-create by name).</summary>
        public static void PopulateActiveCavern()
        {
            CavernKitLibrary.EnsureRegistered();
            var plan = CaveNetworkPlanner.Plan(Seed, extentX: 26f, extentZ: 26f, depth: 14f,
                                               chamberTarget: 8, minSpacing: 11f, loopChance: 0.3f);

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
                    float a = (ch.Id * 2.4f) + s * 2.1f; // deterministic scatter angle
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

            EnsureCaveZipline(plan);
            EnsureSpawnAndDoor(plan);
            EnsureWorldPackAsset(plan);
        }

        // ── Links ────────────────────────────────────────────────────────────

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

            // The climbable rock face rises from the lower pad's edge toward the upper chamber.
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
                wall.AddComponent<ClimbableSurface>(); // paints its own stud handholds at runtime
            }

            // Deep shafts also get the lift — climbing is the sport, the lift is the commute.
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

        // ── Shell (sandbox idiom, cave mood) ─────────────────────────────────

        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 6f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 0.35f;                       // caves are DIM — the crystal tips carry it
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

        /// <summary>A wide catch floor below the deepest chamber — falling off a bridge lands you
        /// somewhere walkable (the fall-safety spirit), never into the void.</summary>
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
            floor.transform.localScale = new Vector3(9f, 1f, 9f); // 90x90 — catches everything
            ItemFactory.ApplyURPColor(floor, new Color(0.12f, 0.115f, 0.11f));
        }

        private static void EnsureSpawnAndDoor(CavePlan plan)
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
            trigger.SetDestination("MilestoneA_GrabCube");
        }

        private static void EnsureWorldPackAsset(CavePlan plan)
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(WorldPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorldPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, WorldPackPath);
            }
            pack.packId = "cavern_lab";
            pack.displayName = "Cavern Test Lab";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            var first = plan.Chambers[0];
            pack.spawnMarkers.Add(new SpawnMarkerDefinition
            {
                markerId = "player",
                localPosition = new Vector3(first.X, first.Y + 0.25f, first.Z)
            });
            EditorUtility.SetDirty(pack);
        }

        private static Scene OpenOrCreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return EditorSceneManager.GetActiveScene();
            if (File.Exists(ScenePath))
                return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }

        private static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath)) return;
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
