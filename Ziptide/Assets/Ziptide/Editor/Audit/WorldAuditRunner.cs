using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Runs a minimum-set of structural checks on every enabled build scene.
    /// Writes docs/AUDIT_REPORT.md and docs/AUDIT_REPORT.json.
    /// Logs ZIPTIDE: AUDIT_OK or ZIPTIDE: AUDIT_FAIL as a single line so build logs
    /// and quest_smoke.ps1 can detect failures.
    ///
    /// Called automatically by BuildAndroid.PatchScenesThenAPK() before APK generation.
    /// Can also be run from the Unity menu: Ziptide > Audit > Run Audit (All Scenes).
    /// </summary>
    public static class WorldAuditRunner
    {
        private const string CityKitAssetPath = "Assets/Ziptide/Content/City/DefaultCityKit.asset";
        private const string ToxicCityLayoutPath = "Assets/Ziptide/Content/City/ToxicCityLayout.asset";

        // Scene names that must have at least one return travel door.
        private static readonly HashSet<string> TravelSceneNames = new HashSet<string>
        {
            "D0_City",
            "ToxicCity"
        };

        // Singletons that must NOT appear directly in world scenes.
        // They should live in _Boot and cross via DontDestroyOnLoad.
        private static readonly string[] SingletonTypeNames = new[]
        {
            "PlayerRigPersistence",
            "TravelCoordinator",
            "AudioDirector",
            "NarrativeSaveSystem"
        };

        [MenuItem("Ziptide/Audit/Run Audit (All Scenes)")]
        public static void RunAllFromMenu()
        {
            int total = RunAll();
            if (total > 0)
                EditorUtility.DisplayDialog("World Audit", "Audit found " + total + " blocker(s). See docs/AUDIT_REPORT.md.", "OK");
            else
                EditorUtility.DisplayDialog("World Audit", "Audit passed. See docs/AUDIT_REPORT.md for full report.", "OK");
        }

        /// <summary>
        /// Runs all checks on all enabled build scenes. Returns total blocker count.
        /// </summary>
        public static int RunAll()
        {
            var report = new WorldAuditReport();
            var scenes = EditorBuildSettings.scenes;

            // Remember what scene was open so we can restore it.
            string previousScenePath = EditorSceneManager.GetActiveScene().path;

            // BOOT_SCENE_MISSING: first enabled scene must be _Boot.
            RunBootSceneCheck(report, scenes);

            foreach (var sceneBuildEntry in scenes)
            {
                if (!sceneBuildEntry.enabled) continue;
                string path = sceneBuildEntry.path;
                if (string.IsNullOrEmpty(path)) continue;

                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                var sceneReport = new SceneAuditReport
                {
                    sceneName = Path.GetFileNameWithoutExtension(path)
                };

                // Skip _Boot scene for world-level checks.
                bool isBootScene = sceneReport.sceneName == "_Boot";
                if (!isBootScene)
                {
                    RunWorldSceneRigCheck(sceneReport);
                    RunSpawnChecks(sceneReport);
                    RunTravelChecks(sceneReport);
                    RunCityGeometryChecks(sceneReport);
                    RunSingletonChecks(sceneReport);
                }

                report.scenes.Add(sceneReport);
            }

            // Restore previous scene if reasonable.
            if (!string.IsNullOrEmpty(previousScenePath) && File.Exists(previousScenePath))
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);

            WriteReports(report);
            LogSummary(report);
            return report.totalBlockers;
        }

        // ── Global checks ────────────────────────────────────────────────────

        private static void RunBootSceneCheck(WorldAuditReport report, UnityEditor.EditorBuildSettingsScene[] scenes)
        {
            // Find first enabled scene.
            string firstName = null;
            foreach (var s in scenes)
            {
                if (!s.enabled) continue;
                firstName = Path.GetFileNameWithoutExtension(s.path);
                break;
            }

            if (firstName != "_Boot")
            {
                // Use a synthetic scene report for global findings.
                var globalReport = new SceneAuditReport { sceneName = "__GLOBAL__" };
                globalReport.Warning("BOOT_SCENE_MISSING",
                    "First enabled build scene is '" + (firstName ?? "none") +
                    "' — should be '_Boot'. Singletons may spawn in wrong scenes.");
                report.scenes.Add(globalReport);
            }
        }

        // ── World scene rig check (blocker) ───────────────────────────────────

        private static void RunWorldSceneRigCheck(SceneAuditReport report)
        {
            // World scenes must NOT contain XR Origin or XRInteractionManager; they live only in _Boot.
            var managers = Object.FindObjectsOfType<XRInteractionManager>();
            if (managers != null && managers.Length > 0)
            {
                foreach (var m in managers)
                    if (m != null)
                        report.Blocker("WORLD_SCENE_HAS_XRI_MANAGER",
                            "World scene must not contain XRInteractionManager. It must live only in _Boot (DontDestroyOnLoad).",
                            GetPath(m.gameObject));
            }

            var xrOriginType = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (xrOriginType != null)
            {
                var origins = Object.FindObjectsOfType(xrOriginType);
                if (origins != null && origins.Length > 0)
                {
                    foreach (var o in origins)
                    {
                        var comp = o as Component;
                        if (comp != null)
                            report.Blocker("WORLD_SCENE_HAS_XR_ORIGIN",
                                "World scene must not contain XR Origin. It must live only in _Boot (DontDestroyOnLoad).",
                                GetPath(comp.gameObject));
                    }
                }
            }

            foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root != null && (root.name == "XR Origin" || root.name.Contains("XR Origin")))
                    report.Blocker("WORLD_SCENE_HAS_XR_ORIGIN",
                        "World scene must not contain XR Origin GameObject. It must live only in _Boot (DontDestroyOnLoad).",
                        GetPath(root));
            }
        }

        // ── Singleton checks ─────────────────────────────────────────────────

        private static void RunSingletonChecks(SceneAuditReport report)
        {
            // SINGLETON_IN_WORLD_SCENE: blocker — DontDestroyOnLoad singletons must live only in _Boot.
            foreach (var typeName in SingletonTypeNames)
            {
                foreach (var go in Object.FindObjectsOfType<GameObject>())
                {
                    if (go.GetComponent(typeName) != null)
                    {
                        report.Blocker("SINGLETON_IN_WORLD_SCENE",
                            "Component '" + typeName + "' found on '" + go.name +
                            "' in world scene. Move to _Boot.unity. It should arrive via DontDestroyOnLoad.",
                            GetPath(go));
                        break; // one blocker per type per scene
                    }
                }
            }
        }

        // ── Spawn checks ────────────────────────────────────────────────────

        private static void RunSpawnChecks(SceneAuditReport report)
        {
            // Find spawn marker.
            var marker = Object.FindObjectOfType<SpawnMarkerRuntime>();
            GameObject markerGo = marker != null ? marker.gameObject : GameObject.Find("__SPAWN_PLAYER");

            if (markerGo == null)
            {
                report.Blocker("SPAWN_MISSING", "No SpawnMarkerRuntime or __SPAWN_PLAYER found in scene.");
                return;
            }

            Vector3 spawnPos = markerGo.transform.position;
            string markerPath = GetPath(markerGo);

            // SPAWN_NO_FLOOR: raycast down from slightly above spawn.
            Vector3 origin = spawnPos + Vector3.up * 0.2f;
            RaycastHit floorHit;
            bool hasFloor = Physics.Raycast(origin, Vector3.down, out floorHit, 6f);
            if (!hasFloor)
            {
                if (report.sceneName == "SampleScene")
                    report.Warning("SPAWN_NO_FLOOR", "No collider hit below spawn within 6m (SampleScene may be unused).", markerPath);
                else
                    report.Blocker("SPAWN_NO_FLOOR", "No collider hit below spawn within 6m. Spawn may be in the void.", markerPath);
            }

            // SPAWN_OVERLAP_SOLID: flag only obstructions (walls, boxes), not walkable floor or the player rig.
            // Exclude: the raycast floor hit, walkable surfaces, and the XR Origin (player) which is expected at spawn.
            Collider floorCollider = hasFloor ? floorHit.collider : null;
            float spawnFeetY = spawnPos.y + 0.2f;
            Collider[] overlapping = Physics.OverlapSphere(spawnPos, 0.35f);
            foreach (var col in overlapping)
            {
                if (col.isTrigger) continue;
                if (col == floorCollider) continue;
                if (col.bounds.max.y <= spawnFeetY) continue; // Walkable surface below feet — not an obstruction.
                if (IsPartOfPlayerRig(col.transform)) continue; // XR Origin / player rig is placed at spawn in editor.
                report.Blocker("SPAWN_OVERLAP_SOLID",
                    "Solid collider '" + col.name + "' overlaps spawn position. Player would spawn inside geometry.",
                    GetPath(col.gameObject));
                break;
            }

            // SPAWN_BELOW_WALKWAY: for city scenes, check spawn Y vs walkway height.
            if (IsCityScene(report.sceneName))
            {
                float minY = GetCityMinSpawnY(report.sceneName);
                if (spawnPos.y < minY)
                {
                    report.Blocker("SPAWN_BELOW_WALKWAY",
                        "Spawn Y=" + spawnPos.y.ToString("F2") + " is below minimum walkway Y="
                        + minY.ToString("F2") + ". Player will spawn in toxic zone.",
                        markerPath);
                }
            }

            // SPAWN_TRAPPED: cast 8 rays at foot height; require at least 2 unblocked.
            Vector3 footOrigin = spawnPos + Vector3.up * 0.5f;
            int unblocked = 0;
            float trapCheckDist = 1.5f;
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                if (!Physics.Raycast(footOrigin, dir, trapCheckDist))
                    unblocked++;
            }
            if (unblocked < 2)
            {
                report.Blocker("SPAWN_TRAPPED",
                    "Spawn appears to be enclosed (only " + unblocked + "/8 radial directions unblocked within "
                    + trapCheckDist + "m). Player may be hard-stuck.",
                    markerPath);
            }
        }

        // ── Door / travel checks ─────────────────────────────────────────────

        private static void RunTravelChecks(SceneAuditReport report)
        {
            // TRAVEL_NO_COORDINATOR
            if (Object.FindObjectOfType<TravelCoordinator>() == null)
                report.Warning("TRAVEL_NO_COORDINATOR",
                    "No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.");

            // TRAVEL_NO_DOOR (Blocker for travel scenes)
            if (TravelSceneNames.Contains(report.sceneName))
            {
                bool hasStation = Object.FindObjectOfType<WorldTravelStation>() != null;
                bool hasTrigger = Object.FindObjectOfType<ProximityTravelTrigger>() != null;
                if (!hasStation && !hasTrigger)
                {
                    report.Blocker("TRAVEL_NO_DOOR",
                        "Travel scene '" + report.sceneName + "' has neither a WorldTravelStation nor a ProximityTravelTrigger. Player cannot return.");
                }
            }

            // TRAVEL_DEST_NOT_IN_BUILD
            CheckTravelDestinations(report);
        }

        private static void CheckTravelDestinations(SceneAuditReport report)
        {
            var validSceneNames = GetBuildSceneNames();

            // Check WorldTravelStation destination packs.
            var stations = Object.FindObjectsOfType<WorldTravelStation>();
            foreach (var station in stations)
            {
                var so = new SerializedObject(station);
                var packsArray = so.FindProperty("destinationPacks");
                if (packsArray == null) continue;
                for (int i = 0; i < packsArray.arraySize; i++)
                {
                    var packRef = packsArray.GetArrayElementAtIndex(i).objectReferenceValue as WorldPackDefinition;
                    if (packRef == null) continue;
                    if (!string.IsNullOrEmpty(packRef.sceneName) && !validSceneNames.Contains(packRef.sceneName))
                    {
                        report.Blocker("TRAVEL_DEST_NOT_IN_BUILD",
                            "WorldTravelStation references pack '" + packRef.packId + "' with sceneName='"
                            + packRef.sceneName + "' which is not in enabled build scenes.",
                            GetPath(station.gameObject));
                    }
                }
            }

            // Check ProximityTravelTrigger destination scene names.
            var triggers = Object.FindObjectsOfType<ProximityTravelTrigger>();
            foreach (var trigger in triggers)
            {
                var so = new SerializedObject(trigger);
                var destProp = so.FindProperty("destinationSceneName");
                if (destProp == null) continue;
                string destScene = destProp.stringValue;
                if (!string.IsNullOrEmpty(destScene) && !validSceneNames.Contains(destScene))
                {
                    report.Blocker("TRAVEL_DEST_NOT_IN_BUILD",
                        "ProximityTravelTrigger references destinationSceneName='" + destScene
                        + "' which is not in enabled build scenes.",
                        GetPath(trigger.gameObject));
                }
            }
        }

        // ── City geometry checks ─────────────────────────────────────────────

        private static void RunCityGeometryChecks(SceneAuditReport report)
        {
            if (!IsCityScene(report.sceneName)) return;

            // CITY_NO_ROOT — accept any patcher's city root (legacy __D1_CITY_ROOT or the generalized
            // "__<CITYID>_ROOT" produced by ScenePatcherToxicCity / future city patchers).
            if (!HasCityRoot())
            {
                report.Blocker("CITY_NO_ROOT",
                    "No city root GameObject (expected '__D1_CITY_ROOT' or '__<CITYID>_ROOT'). City patcher may not have run.");
            }

            // CITY_NO_RAMP
            bool hasRamp = false;
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (go.name.IndexOf("Ramp", System.StringComparison.OrdinalIgnoreCase) >= 0)
                { hasRamp = true; break; }
            }
            if (!hasRamp)
                report.Warning("CITY_NO_RAMP", "No ramp found in city scene. Players may not be able to reach all levels.");

            // CITY_NO_COURTYARD
            bool hasCourt = false;
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (go.name.IndexOf("Courtyard", System.StringComparison.OrdinalIgnoreCase) >= 0)
                { hasCourt = true; break; }
            }
            if (!hasCourt)
                report.Warning("CITY_NO_COURTYARD", "No courtyard found in city scene. City may lack safe platform areas.");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static bool IsCityScene(string sceneName)
        {
            return sceneName == "D0_City" || sceneName.Contains("City");
        }

        private static bool IsPartOfPlayerRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin") return true;
                t = t.parent;
            }
            return false;
        }

        private static bool HasCityRoot()
        {
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (go.transform.parent != null) continue; // roots only
                string n = go.name;
                if (n.StartsWith("__") && n.EndsWith("_ROOT")) return true;
            }
            return false;
        }

        private static float GetCityMinSpawnY(string sceneName)
        {
            // ToxicCity (and future CityLayout-driven worlds) read their own layout's walkway height.
            if (sceneName == "ToxicCity")
            {
                var layout = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(ToxicCityLayoutPath);
                if (layout != null)
                    return layout.walkwayHeight - 0.25f;
            }
            var kit = AssetDatabase.LoadAssetAtPath<CityKitDefinition>(CityKitAssetPath);
            if (kit != null)
                return kit.walkwayHeight - 0.25f;
            return 1.0f; // fallback: at least 1m above ground
        }

        private static HashSet<string> GetBuildSceneNames()
        {
            var names = new HashSet<string>();
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (!s.enabled) continue;
                string sn = Path.GetFileNameWithoutExtension(s.path);
                if (!string.IsNullOrEmpty(sn)) names.Add(sn);
            }
            return names;
        }

        private static string GetPath(GameObject go)
        {
            if (go == null) return "";
            var parts = new System.Collections.Generic.List<string>();
            Transform t = go.transform;
            while (t != null) { parts.Insert(0, t.name); t = t.parent; }
            return string.Join("/", parts);
        }

        // ── Output ────────────────────────────────────────────────────────────

        private static void WriteReports(WorldAuditReport report)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string repoRoot = Path.GetFullPath(Path.Combine(projectRoot, ".."));
            string docsDir = Path.Combine(repoRoot, "docs");

            // Fall back to project root if docs not in parent.
            if (!Directory.Exists(docsDir))
            {
                docsDir = Path.Combine(projectRoot, "docs");
                if (!Directory.Exists(docsDir))
                    Directory.CreateDirectory(docsDir);
            }

            File.WriteAllText(Path.Combine(docsDir, "AUDIT_REPORT.md"), report.ToMarkdown());
            File.WriteAllText(Path.Combine(docsDir, "AUDIT_REPORT.json"), report.ToJson());

            Debug.Log("[Ziptide] Audit reports written to " + docsDir);
        }

        private static void LogSummary(WorldAuditReport report)
        {
            if (report.totalBlockers > 0)
                Debug.LogError("ZIPTIDE: AUDIT_FAIL blockers=" + report.totalBlockers + " warnings=" + report.totalWarnings);
            else
                Debug.Log("ZIPTIDE: AUDIT_OK blockers=0 warnings=" + report.totalWarnings);
        }
    }
}
