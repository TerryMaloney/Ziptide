using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryRuntimeArtifactFinding
    {
        public string code;
        public string featureId;
        public string hierarchyPath;
        public string objectName;
        public string[] componentTypes;
        public string message;
    }

    [Serializable]
    public sealed class RecoveryRuntimeArtifactReport
    {
        public string schemaVersion = "2";
        public string label;
        public string capturedAtUtc;
        public string activeProfile;
        public RecoverySpawnClearanceReport spawnClearance;
        public List<RecoveryRuntimeArtifactFinding> findings =
            new List<RecoveryRuntimeArtifactFinding>();
    }

    /// <summary>
    /// Test-owned guard for runtime artifacts created by static or third-party owners whose source
    /// symbol is not itself a MonoBehaviour. The PlayMode process starts in editor FullDevelopment,
    /// so DebugHUD and Photon can exist before an actual-scene test switches to GoldenSlice. They
    /// are removed during fresh-Golden isolation, then this same guard fails if either artifact is
    /// recreated by the real Golden boot path. In content scenes it also embeds the standing
    /// spawn-clearance report, so every settled Golden arrival proves floor, head height and torso
    /// collision instead of relying on XZ alignment alone.
    /// </summary>
    public static class RecoveryRuntimeArtifactGuard
    {
        public static RecoveryRuntimeArtifactReport Capture(string label)
        {
            var report = new RecoveryRuntimeArtifactReport
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                activeProfile = RecoveryRuntimeGate.ActiveProfileName
            };

            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go == null || !go.scene.IsValid() || !go.activeInHierarchy) continue;

                string[] componentTypes = ComponentTypes(go);
                if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.DebugHud) &&
                    IsDebugHudArtifact(go, componentTypes))
                {
                    report.findings.Add(new RecoveryRuntimeArtifactFinding
                    {
                        code = "FORBIDDEN_DEBUG_HUD_ARTIFACT",
                        featureId = RecoveryFeatureId.DebugHud.ToString(),
                        hierarchyPath = RecoveryRuntimeCensus.HierarchyPath(go.transform),
                        objectName = go.name,
                        componentTypes = componentTypes,
                        message = "Development DebugHUD artifact is active while the profile forbids DebugHud."
                    });
                }

                if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.NetBootstrap) &&
                    IsPhotonArtifact(go, componentTypes))
                {
                    report.findings.Add(new RecoveryRuntimeArtifactFinding
                    {
                        code = "FORBIDDEN_PHOTON_RUNTIME_ARTIFACT",
                        featureId = RecoveryFeatureId.NetBootstrap.ToString(),
                        hierarchyPath = RecoveryRuntimeCensus.HierarchyPath(go.transform),
                        objectName = go.name,
                        componentTypes = componentTypes,
                        message = "Photon runtime loop is active while the Golden profile forbids NetBootstrap."
                    });
                }
            }

            CaptureSpawnClearanceWhenApplicable(report, label);
            report.findings.Sort((a, b) =>
            {
                int code = string.CompareOrdinal(a.code, b.code);
                return code != 0 ? code : string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath);
            });
            return report;
        }

        public static int DestroyForbiddenArtifactsImmediate()
        {
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            var destroy = new HashSet<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go == null || !go.scene.IsValid()) continue;
                string[] componentTypes = ComponentTypes(go);

                if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.DebugHud) &&
                    IsDebugHudArtifact(go, componentTypes))
                    destroy.Add(go);
                if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.NetBootstrap) &&
                    IsPhotonArtifact(go, componentTypes))
                    destroy.Add(go);
            }

            int count = 0;
            foreach (GameObject go in destroy)
            {
                if (go == null) continue;
                Debug.Log("ZIPTIDE: RECOVERY_STALE_ARTIFACT_REMOVED path="
                    + RecoveryRuntimeCensus.HierarchyPath(go.transform));
                UnityEngine.Object.DestroyImmediate(go);
                count++;
            }
            return count;
        }

        public static string WriteArtifact(RecoveryRuntimeArtifactReport report, string stem)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                RecoveryRuntimeCensus.ArtifactDirectoryName));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, SanitizeStem(stem) + ".runtime-artifacts.json");
            File.WriteAllText(path, JsonUtility.ToJson(report, true) + Environment.NewLine,
                Encoding.UTF8);
            return path;
        }

        private static void CaptureSpawnClearanceWhenApplicable(
            RecoveryRuntimeArtifactReport report,
            string label)
        {
            if (SceneManager.GetActiveScene().name == ZiptideConstants.SceneBoot) return;

            PlayerRigPersistence rig = UnityEngine.Object.FindObjectOfType<PlayerRigPersistence>();
            if (rig == null) return; // Controlled artifact fixtures do not model a content arrival.

            Camera camera = rig.GetComponentInChildren<Camera>(true);
            if (camera == null)
            {
                report.findings.Add(new RecoveryRuntimeArtifactFinding
                {
                    code = "SPAWN_HEAD_CAMERA_MISSING",
                    featureId = string.Empty,
                    hierarchyPath = RecoveryRuntimeCensus.HierarchyPath(rig.transform),
                    objectName = rig.name,
                    componentTypes = ComponentTypes(rig.gameObject),
                    message = "Persistent rig has no tracked-head camera for standing spawn proof."
                });
                return;
            }

            RecoverySpawnClearanceReport spawn = RecoverySpawnClearanceAudit.Capture(
                rig.transform,
                camera,
                label + "_SPAWN_CLEARANCE");
            report.spawnClearance = spawn;
            for (int i = 0; i < spawn.findings.Count; i++)
            {
                RecoverySpawnClearanceFinding finding = spawn.findings[i];
                report.findings.Add(new RecoveryRuntimeArtifactFinding
                {
                    code = finding.code,
                    featureId = string.Empty,
                    hierarchyPath = finding.hierarchyPath,
                    objectName = string.Empty,
                    componentTypes = Array.Empty<string>(),
                    message = finding.message
                });
            }
        }

        private static bool IsDebugHudArtifact(GameObject go, string[] componentTypes)
        {
            if (go.name == "Ziptide_DebugHUD") return true;
            return ContainsType(componentTypes, "Ziptide.Core.DebugHUD+DebugHUDUpdater");
        }

        private static bool IsPhotonArtifact(GameObject go, string[] componentTypes)
        {
            if (go.name == "PhotonMono") return true;
            return ContainsType(componentTypes, "Photon.Pun.PhotonHandler");
        }

        private static bool ContainsType(string[] values, string expected)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i] == expected) return true;
            return false;
        }

        private static string[] ComponentTypes(GameObject go)
        {
            Component[] components = go.GetComponents<Component>();
            var values = new List<string>(components.Length);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                values.Add(component != null ? component.GetType().FullName : "MISSING_SCRIPT");
            }
            values.Sort(StringComparer.Ordinal);
            return values.ToArray();
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "runtime-artifacts";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }
    }
}
