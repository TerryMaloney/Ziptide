#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// PG-2: validates travel against the scene universe an artifact actually packages, not the wider
    /// EditorBuildSettings list. Recovery Golden Android passes its locked three-scene profile here before
    /// BuildPipeline runs. The active-scene method is public so deliberately-broken EditMode fixtures can
    /// prove the blocker without mutating project build settings.
    /// </summary>
    public static class BuildProfileTravelAuditRules
    {
        public const string DestinationBlocker = "TRAVEL_DEST_UNREACHABLE_IN_PROFILE";

        public static int RunScenes(IReadOnlyList<string> shippedScenePaths)
        {
            if (shippedScenePaths == null || shippedScenePaths.Count == 0)
                throw new System.ArgumentException("Shipped scene profile must contain at least one scene.");

            HashSet<string> allowed = SceneNamesFromPaths(shippedScenePaths);
            string previous = EditorSceneManager.GetActiveScene().path;
            int blockers = 0;
            try
            {
                for (int i = 0; i < shippedScenePaths.Count; i++)
                {
                    string path = shippedScenePaths[i];
                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                        throw new FileNotFoundException("Build-profile scene is missing.", path);
                    Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    var report = new SceneAuditReport
                    {
                        sceneName = Path.GetFileNameWithoutExtension(scene.path)
                    };
                    ValidateActiveScene(report, allowed);
                    blockers += report.blockerCount;
                    foreach (AuditFinding finding in report.findings)
                        if (finding.severity == AuditSeverity.Blocker)
                            Debug.LogError("ZIPTIDE: PROFILE_AUDIT_BLOCKER scene=" + report.sceneName
                                + " " + finding);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(previous) && File.Exists(previous))
                    EditorSceneManager.OpenScene(previous, OpenSceneMode.Single);
            }

            Debug.Log("ZIPTIDE: BUILD_PROFILE_TRAVEL_AUDIT scenes=" + allowed.Count
                + " blockers=" + blockers + " allowed=" + string.Join(",", allowed));
            return blockers;
        }

        public static void ValidateActiveScene(SceneAuditReport report,
            IReadOnlyCollection<string> shippedSceneNames)
        {
            if (report == null) throw new System.ArgumentNullException(nameof(report));
            if (shippedSceneNames == null || shippedSceneNames.Count == 0)
                throw new System.ArgumentException("Shipped scene names are required.");

            HashSet<string> allowed = shippedSceneNames as HashSet<string>
                ?? new HashSet<string>(shippedSceneNames);

            foreach (WorldTravelStation station in Object.FindObjectsOfType<WorldTravelStation>(true))
                ValidatePackArray(report, station, "destinationPacks", allowed, "WorldTravelStation");

            foreach (ShipBoardingStation station in Object.FindObjectsOfType<ShipBoardingStation>(true))
                ValidatePackArray(report, station, "destinationPacks", allowed, "ShipBoardingStation");

            foreach (ProximityTravelTrigger trigger in Object.FindObjectsOfType<ProximityTravelTrigger>(true))
            {
                var serialized = new SerializedObject(trigger);
                SerializedProperty destination = serialized.FindProperty("destinationSceneName");
                if (destination == null) continue;
                ValidateDestination(report, destination.stringValue, allowed,
                    "ProximityTravelTrigger", GetPath(trigger.gameObject));
            }
        }

        public static HashSet<string> SceneNamesFromPaths(IReadOnlyList<string> scenePaths)
        {
            var names = new HashSet<string>();
            if (scenePaths == null) return names;
            for (int i = 0; i < scenePaths.Count; i++)
            {
                string name = Path.GetFileNameWithoutExtension(scenePaths[i]);
                if (!string.IsNullOrEmpty(name)) names.Add(name);
            }
            return names;
        }

        public static bool IsDestinationAllowed(string destination,
            IReadOnlyCollection<string> shippedSceneNames)
        {
            if (string.IsNullOrEmpty(destination)) return true;
            return shippedSceneNames != null && shippedSceneNames.Contains(destination);
        }

        private static void ValidatePackArray(SceneAuditReport report, Component owner,
            string propertyName, HashSet<string> allowed, string ownerKind)
        {
            var serialized = new SerializedObject(owner);
            SerializedProperty packs = serialized.FindProperty(propertyName);
            if (packs == null) return;
            for (int i = 0; i < packs.arraySize; i++)
            {
                WorldPackDefinition pack = packs.GetArrayElementAtIndex(i).objectReferenceValue
                    as WorldPackDefinition;
                if (pack == null) continue;
                ValidateDestination(report, pack.sceneName, allowed,
                    ownerKind + " pack='" + pack.packId + "'", GetPath(owner.gameObject));
            }
        }

        private static void ValidateDestination(SceneAuditReport report, string destination,
            HashSet<string> allowed, string ownerDescription, string objectPath)
        {
            if (IsDestinationAllowed(destination, allowed)) return;
            report.Blocker(DestinationBlocker,
                ownerDescription + " targets scene '" + destination
                + "', but the artifact profile packages only [" + string.Join(", ", allowed) + "].",
                objectPath);
        }

        private static string GetPath(GameObject value)
        {
            if (value == null) return string.Empty;
            var parts = new List<string>();
            Transform current = value.transform;
            while (current != null)
            {
                parts.Insert(0, current.name);
                current = current.parent;
            }
            return string.Join("/", parts);
        }
    }
}
#endif
