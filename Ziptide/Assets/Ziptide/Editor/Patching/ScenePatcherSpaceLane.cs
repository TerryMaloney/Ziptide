#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// P4b — THE FLIGHT TRIAL scene (SPACEFLIGHT_PHYSICS "what v1 should do NOW"): a bounded space
    /// lane with a dock pad, an open cockpit frame, a helm running <see cref="ShipFlightRuntime"/>,
    /// and a ring course inside a LaneContent root (the thing that moves — the rig never does).
    /// Ships as a world pack in the story Packs folder, so every berthed ship's helm and the dev
    /// warp list "Flight Trial" with zero CityBuilder edits. Idempotent; standard patcher shell.
    /// 🔧 Terry: run Ziptide → Worlds → Build Space Lane (Flight Trial) once, commit scene + assets.
    /// </summary>
    public static class ScenePatcherSpaceLane
    {
        public const string SceneName = "SpaceLane_Trial";
        private const string ScenePath = "Assets/Ziptide/Scenes/" + SceneName + ".unity";
        private const string PackFolder = "Assets/Ziptide/Content/Worlds/Packs";

        private static readonly Vector3 HelmPos = new Vector3(0f, 0.05f, 0f);
        private static readonly Vector3 PlayerSpawn = new Vector3(0f, 0.1f, -2.5f);

        // Lane-space ring course: gentle climb + two turns, all reachable inside the pitch clamp
        // and snap-yaw comfort rails. Pass radius on the runtime is forgiving on purpose.
        private static readonly Vector3[] Rings =
        {
            new Vector3(0f, 3f, 60f),
            new Vector3(14f, 7f, 130f),
            new Vector3(-8f, 12f, 210f),
            new Vector3(-28f, 6f, 295f),
            new Vector3(0f, 10f, 380f),
        };
        private const float RingVisualRadius = 6f;
        private const float RingPassRadius = 7f;

        /// <summary>
        /// Ensure the space lane exists and ships. Called by BuildAndroid, exactly as ToxicCity is.
        ///
        /// ⚠ THIS IS WHY THE SPACE LEG DID NOT EXIST. The patcher below was complete and correct for
        /// months, but it was reachable ONLY from a menu item, and its header asked Terry to run it
        /// once by hand. Nobody did, so SpaceLane_Trial.unity was never created, never entered Build
        /// Settings, and the first level's whole middle -- flight, salvage, approach, reentry -- had
        /// no scene to happen in. A world that only a human can generate is a world that does not
        /// exist. Every other shipped scene self-generates in the build; now this one does too.
        /// </summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var created = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(created, ScenePath);
                Debug.Log("[Ziptide] Created empty space lane at " + normalized
                    + " (populated by the per-scene pass below).");
            }
            EnsureSceneEnabled();
        }

        /// <summary>No-op unless the active scene is the space lane. Called in the per-scene loop.</summary>
        public static void PatchActiveScene()
        {
            if (EditorSceneManager.GetActiveScene().name != SceneName) return;
            Populate();
        }

        [MenuItem("Ziptide/Worlds/Build Space Lane (Flight Trial)")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = File.Exists(ScenePath)
                ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Populate();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureSceneEnabled();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Space lane built: " + ScenePath + " (" + Rings.Length + " rings)");
        }

        private static void Populate()
        {
            var old = GameObject.Find("__SPACELANE_ROOT");
            if (old != null) Object.DestroyImmediate(old);
            var root = new GameObject("__SPACELANE_ROOT").transform;

            BuildDock(root);
            var lane = BuildLaneContent(root);

            EnsureLighting();
            EnsureEventSystem();

            // Deep-space dressing through the standard pipeline: near-black sky, a big cold planet.
            var theme = ThemeAuthor.EnsureThemeAsset(SceneName,
                new Color(0.02f, 0.03f, 0.06f), new Color(0.01f, 0.01f, 0.03f),
                new Color(0.05f, 0.06f, 0.08f),
                planetVisible: true,
                new Color(0.35f, 0.45f, 0.60f), new Color(0.55f, 0.35f, 0.45f), 14f);
            var profile = ThemeAuthor.EnsureWorldProfileAsset(SceneName, 0f, theme);
            EnsureWorldRuntime(profile);

            EnsureSpawn("player", PlayerSpawn);
            EnsureFlightRuntime(lane);
            EnsureWorldPack();
        }

        // ── Dock: a small pad + open cockpit frame around the helm (interior stays static) ─────────
        private static void BuildDock(Transform root)
        {
            Cube(root, "DockPad", new Vector3(0f, -0.25f, 0f), new Vector3(8f, 0.5f, 10f),
                new Color(0.16f, 0.18f, 0.22f));

            // Open cockpit silhouette: floor rails + two side struts + a canopy bar. No walls —
            // the view of the moving lane IS the game; geometry just anchors the "inside" feel.
            var frame = new GameObject("CockpitFrame").transform;
            frame.SetParent(root, false);
            Cube(frame, "Strut_L", new Vector3(-1.4f, 1.1f, 1.2f), new Vector3(0.15f, 2.2f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));
            Cube(frame, "Strut_R", new Vector3(1.4f, 1.1f, 1.2f), new Vector3(0.15f, 2.2f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));
            Cube(frame, "CanopyBar", new Vector3(0f, 2.2f, 1.2f), new Vector3(3f, 0.15f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));
        }

        // ── The lane: everything that moves past the pilot lives under this one root ───────────────
        // Drone targets flank the course between rings 2 and 4 — off the racing line, so pacifists
        // still finish the rings; hunters detour, disable, and fly in for the salvage.
        private static readonly Vector3[] Drones =
        {
            new Vector3(26f, 9f, 170f),
            new Vector3(-22f, 14f, 250f),
            new Vector3(-38f, 8f, 330f),
        };

        private static Transform BuildLaneContent(Transform root)
        {
            var lane = new GameObject("LaneContent").transform;
            lane.SetParent(root, false);

            for (int i = 0; i < Rings.Length; i++)
                BuildRing(lane, i, Rings[i]);

            for (int i = 0; i < Drones.Length; i++)
                BuildDroneTarget(lane, i, Drones[i]);

            // Sparse drift rocks flanking the course — parallax so speed reads.
            var rng = new System.Random(777);
            var rocks = new GameObject("DriftRocks").transform;
            rocks.SetParent(lane, false);
            for (int i = 0; i < 28; i++)
            {
                float t = (float)rng.NextDouble();
                Vector3 along = Vector3.Lerp(new Vector3(0f, 4f, 20f), new Vector3(-5f, 8f, 420f), t);
                Vector3 off = new Vector3(((float)rng.NextDouble() - 0.5f) * 90f,
                    ((float)rng.NextDouble() - 0.5f) * 40f, ((float)rng.NextDouble() - 0.5f) * 30f);
                if (Mathf.Abs(off.x) < 14f) off.x = Mathf.Sign(off.x == 0f ? 1f : off.x) * 14f; // keep the course clear
                var rock = GameObject.CreatePrimitive(rng.Next(2) == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere);
                rock.name = "Rock_" + i;
                rock.transform.SetParent(rocks, false);
                rock.transform.localPosition = along + off;
                rock.transform.localRotation = Quaternion.Euler(rng.Next(360), rng.Next(360), rng.Next(360));
                rock.transform.localScale = Vector3.one * (1.5f + (float)rng.NextDouble() * 4f);
                Object.DestroyImmediate(rock.GetComponent<Collider>()); // scenery only — nothing to hit in v1
                Paint(rock, new Color(0.20f, 0.22f, 0.27f));
            }
            return lane;
        }

        private static void BuildDroneTarget(Transform lane, int index, Vector3 center)
        {
            // Direct child of LaneContent — SpaceTargetRuntime's lane math assumes this frame.
            var drone = new GameObject("Drone_" + index);
            drone.transform.SetParent(lane, false);
            drone.transform.localPosition = center;

            var hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hull.name = "Hull";
            hull.transform.SetParent(drone.transform, false);
            hull.transform.localScale = new Vector3(1.6f, 0.9f, 2.2f);
            Object.DestroyImmediate(hull.GetComponent<Collider>()); // hits resolve in the aim cone, not physics
            Paint(hull, new Color(0.9f, 0.6f, 0.2f));

            for (int s = -1; s <= 1; s += 2)
            {
                var wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wing.name = s < 0 ? "Wing_L" : "Wing_R";
                wing.transform.SetParent(drone.transform, false);
                wing.transform.localPosition = new Vector3(s * 1.5f, 0f, -0.3f);
                wing.transform.localScale = new Vector3(1.4f, 0.12f, 1.1f);
                Object.DestroyImmediate(wing.GetComponent<Collider>());
                Paint(wing, new Color(0.55f, 0.35f, 0.15f));
            }

            var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "Eye";
            eye.transform.SetParent(drone.transform, false);
            eye.transform.localPosition = new Vector3(0f, 0.2f, 1.2f);
            eye.transform.localScale = Vector3.one * 0.5f;
            Object.DestroyImmediate(eye.GetComponent<Collider>());
            Paint(eye, new Color(1f, 0.3f, 0.2f));

            drone.AddComponent<SpaceTargetRuntime>(); // serialized defaults: 6 armor, "scrap" ×6
        }

        private static void BuildRing(Transform lane, int index, Vector3 center)
        {
            var ring = new GameObject("Ring_" + index).transform;
            ring.SetParent(lane, false);
            ring.localPosition = center;
            const int segments = 10;
            for (int s = 0; s < segments; s++)
            {
                float a = (s / (float)segments) * Mathf.PI * 2f;
                var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                seg.name = "Seg_" + s;
                seg.transform.SetParent(ring, false);
                seg.transform.localPosition = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * RingVisualRadius;
                seg.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);
                seg.transform.localScale = new Vector3(1.2f, 0.35f, 0.35f);
                Object.DestroyImmediate(seg.GetComponent<Collider>()); // fly THROUGH, never into
                Paint(seg, new Color(0.9f, 0.55f, 0.2f));
            }
        }

        private static void EnsureFlightRuntime(Transform lane)
        {
            var go = PatcherUtil.EnsureRootObject("ShipFlightHelm", HelmPos);
            var flight = PatcherUtil.EnsureComponent<ShipFlightRuntime>(go);
            var so = new SerializedObject(flight);
            PatcherUtil.SetObjectRef(so, "laneContent", lane);
            PatcherUtil.SetString(so, "returnScene", "W000_DriftIn");
            PatcherUtil.SetFloat(so, "ringRadius", RingPassRadius);
            var list = so.FindProperty("ringPositions");
            list.arraySize = Rings.Length;
            for (int i = 0; i < Rings.Length; i++)
                list.GetArrayElementAtIndex(i).vector3Value = Rings[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ── Standard shell (same as every patcher) ──────────────────────────────────────────────────
        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 10f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 0.7f; // dim — deep space
            go.transform.rotation = Quaternion.Euler(35f, -30f, 0f);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var go = PatcherUtil.EnsureRootObject("EventSystem", Vector3.zero);
            PatcherUtil.EnsureComponent<UnityEngine.EventSystems.EventSystem>(go);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(go);
        }

        private static void EnsureWorldRuntime(WorldProfile profile)
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            if (profile != null)
            {
                var so = new SerializedObject(wr);
                PatcherUtil.SetObjectRef(so, "worldProfile", profile);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureSpawn(string markerId, Vector3 pos)
        {
            string objName = markerId == "player" ? ZiptideConstants.GoSpawnPlayer : "__SPAWN_" + markerId;
            var go = PatcherUtil.EnsureRootObject(objName, pos);
            var marker = PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", markerId);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureWorldPack()
        {
            string path = PackFolder + "/" + SceneName + "_WorldPack.asset";
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (pack == null)
            {
                Directory.CreateDirectory(PackFolder);
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, path);
            }
            pack.packId = "spacelane_trial";
            pack.displayName = "Flight Trial";
            pack.sceneName = SceneName;
            var player = pack.spawnMarkers.Find(m => m != null && m.markerId == "player");
            if (player == null)
                pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = PlayerSpawn });
            else
                player.localPosition = PlayerSpawn;
            EditorUtility.SetDirty(pack);
        }

        private static void EnsureSceneEnabled()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == ScenePath);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(ScenePath, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPos, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            Paint(go, color);
            return go;
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
