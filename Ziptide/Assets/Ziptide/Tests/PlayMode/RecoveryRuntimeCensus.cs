using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryRuntimeCensusSnapshot
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string scopePath;
        public string activeProfile;
        public string[] activeFeatureIds;
        public List<RecoveryCensusSceneRecord> scenes = new List<RecoveryCensusSceneRecord>();
        public List<RecoveryCensusObjectRecord> roots = new List<RecoveryCensusObjectRecord>();
        public List<RecoveryCensusComponentRecord> cameras = new List<RecoveryCensusComponentRecord>();
        public List<RecoveryCensusComponentRecord> managers = new List<RecoveryCensusComponentRecord>();
        public List<RecoveryCensusComponentRecord> ui = new List<RecoveryCensusComponentRecord>();
        public List<RecoveryCensusComponentRecord> audioSources = new List<RecoveryCensusComponentRecord>();
        public List<RecoveryCensusMaterialRecord> materials = new List<RecoveryCensusMaterialRecord>();
        public List<RecoveryCensusOwnerRecord> automaticOwners = new List<RecoveryCensusOwnerRecord>();
        public List<RecoveryCensusFinding> findings = new List<RecoveryCensusFinding>();
    }

    [Serializable]
    public sealed class RecoveryCensusSceneRecord
    {
        public string name;
        public string path;
        public bool loaded;
        public int buildIndex;
        public int rootCount;
    }

    [Serializable]
    public sealed class RecoveryCensusObjectRecord
    {
        public string name;
        public string hierarchyPath;
        public string scene;
        public bool activeSelf;
        public bool activeInHierarchy;
        public bool dontDestroyOnLoad;
        public string[] componentTypes;
    }

    [Serializable]
    public sealed class RecoveryCensusComponentRecord
    {
        public string category;
        public string type;
        public string hierarchyPath;
        public string scene;
        public string role;
        public bool active;
        public bool enabled;
    }

    [Serializable]
    public sealed class RecoveryCensusMaterialRecord
    {
        public int instanceId;
        public string name;
        public string shader;
        public string rendererPath;
        public string scene;
        public string hideFlags;
    }

    [Serializable]
    public sealed class RecoveryCensusOwnerRecord
    {
        public string ownerId;
        public string symbol;
        public string featureId;
        public string classification;
        public bool allowedByProfile;
        public int componentCount;
        public int activeCount;
        public string[] objectPaths;
    }

    [Serializable]
    public sealed class RecoveryCensusFinding
    {
        public string code;
        public string severity;
        public string owner;
        public string hierarchyPath;
        public string message;
    }

    public sealed class RecoveryCensusArtifactPaths
    {
        public string JsonPath { get; }
        public string MarkdownPath { get; }

        public RecoveryCensusArtifactPaths(string jsonPath, string markdownPath)
        {
            JsonPath = jsonPath;
            MarkdownPath = markdownPath;
        }
    }

    /// <summary>
    /// Test-owned R1.4 runtime census. It observes the running Unity object graph without adding a
    /// production bootstrap or mutating scene content. Records use stable type names, hierarchy
    /// paths and explicit recovery IDs; assertions do not depend on incidental child counts.
    /// </summary>
    public static class RecoveryRuntimeCensus
    {
        public const string ArtifactDirectoryName = "recovery-census";

        public static RecoveryRuntimeCensusSnapshot Capture(string label, Transform scopeRoot = null)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Census label is required.", nameof(label));

            var snapshot = new RecoveryRuntimeCensusSnapshot
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                scopePath = scopeRoot != null ? HierarchyPath(scopeRoot) : "GLOBAL",
                activeProfile = RecoveryRuntimeGate.ActiveProfileName,
                activeFeatureIds = FeatureIds()
            };

            CaptureScenes(snapshot);
            GameObject[] gameObjects = SceneGameObjects(scopeRoot);
            CaptureRoots(snapshot, gameObjects, scopeRoot);
            CaptureComponents(snapshot, gameObjects);
            CaptureMaterials(snapshot, gameObjects);
            CaptureAutomaticOwners(snapshot, gameObjects);
            BuildFindings(snapshot, gameObjects);
            Sort(snapshot);
            return snapshot;
        }

        public static RecoveryCensusArtifactPaths WriteArtifacts(
            RecoveryRuntimeCensusSnapshot snapshot,
            string stem)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            string safeStem = SanitizeStem(stem);
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                ArtifactDirectoryName));
            Directory.CreateDirectory(directory);

            string jsonPath = Path.Combine(directory, safeStem + ".json");
            string markdownPath = Path.Combine(directory, safeStem + ".md");
            File.WriteAllText(jsonPath, JsonUtility.ToJson(snapshot, true) + Environment.NewLine,
                Encoding.UTF8);
            File.WriteAllText(markdownPath, BuildMarkdown(snapshot), Encoding.UTF8);
            return new RecoveryCensusArtifactPaths(jsonPath, markdownPath);
        }

        private static void CaptureScenes(RecoveryRuntimeCensusSnapshot snapshot)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                snapshot.scenes.Add(new RecoveryCensusSceneRecord
                {
                    name = scene.name,
                    path = scene.path,
                    loaded = scene.isLoaded,
                    buildIndex = scene.buildIndex,
                    rootCount = scene.isLoaded ? scene.rootCount : 0
                });
            }
        }

        private static GameObject[] SceneGameObjects(Transform scopeRoot)
        {
            var list = new List<GameObject>();
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go == null || !go.scene.IsValid()) continue;
                if (!InScope(go.transform, scopeRoot)) continue;
                list.Add(go);
            }
            list.Sort((a, b) => string.CompareOrdinal(HierarchyPath(a.transform), HierarchyPath(b.transform)));
            return list.ToArray();
        }

        private static void CaptureRoots(
            RecoveryRuntimeCensusSnapshot snapshot,
            GameObject[] gameObjects,
            Transform scopeRoot)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject go = gameObjects[i];
                bool rootInScope = go.transform.parent == null ||
                                   (scopeRoot != null && go.transform == scopeRoot);
                if (!rootInScope) continue;

                Component[] components = go.GetComponents<Component>();
                var componentNames = new List<string>();
                for (int c = 0; c < components.Length; c++)
                {
                    Component component = components[c];
                    componentNames.Add(component != null
                        ? component.GetType().FullName
                        : "MISSING_SCRIPT");
                }
                componentNames.Sort(StringComparer.Ordinal);

                snapshot.roots.Add(new RecoveryCensusObjectRecord
                {
                    name = go.name,
                    hierarchyPath = HierarchyPath(go.transform),
                    scene = SceneName(go),
                    activeSelf = go.activeSelf,
                    activeInHierarchy = go.activeInHierarchy,
                    dontDestroyOnLoad = IsDontDestroyOnLoad(go),
                    componentTypes = componentNames.ToArray()
                });
            }
        }

        private static void CaptureComponents(
            RecoveryRuntimeCensusSnapshot snapshot,
            GameObject[] gameObjects)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject go = gameObjects[i];
                Camera camera = go.GetComponent<Camera>();
                if (camera != null)
                    snapshot.cameras.Add(ComponentRecord("Camera", camera, CameraRole(camera)));

                XRInteractionManager xri = go.GetComponent<XRInteractionManager>();
                if (xri != null)
                    snapshot.managers.Add(ComponentRecord("XRInteractionManager", xri, "XRI_SESSION"));

                InputActionManager input = go.GetComponent<InputActionManager>();
                if (input != null)
                    snapshot.managers.Add(ComponentRecord("InputActionManager", input, "INPUT_SESSION"));

                Canvas canvas = go.GetComponent<Canvas>();
                if (canvas != null)
                    snapshot.ui.Add(ComponentRecord("Canvas", canvas, canvas.renderMode.ToString()));

                TextMesh textMesh = go.GetComponent<TextMesh>();
                if (textMesh != null)
                    snapshot.ui.Add(ComponentRecord("TextMesh", textMesh, "WORLD_TEXT"));

                AudioSource audioSource = go.GetComponent<AudioSource>();
                if (audioSource != null)
                    snapshot.audioSources.Add(ComponentRecord("AudioSource", audioSource, "AUDIO_SOURCE"));

                MonoBehaviour[] behaviours = go.GetComponents<MonoBehaviour>();
                for (int b = 0; b < behaviours.Length; b++)
                {
                    MonoBehaviour behaviour = behaviours[b];
                    if (behaviour == null) continue;
                    string fullName = behaviour.GetType().FullName;
                    string category = UiCategory(fullName);
                    if (category == null) continue;
                    snapshot.ui.Add(ComponentRecord(category, behaviour, fullName));
                }
            }
        }

        private static void CaptureMaterials(
            RecoveryRuntimeCensusSnapshot snapshot,
            GameObject[] gameObjects)
        {
            var seen = new HashSet<int>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                Renderer renderer = gameObjects[i].GetComponent<Renderer>();
                if (renderer == null) continue;
                Material[] materials = renderer.sharedMaterials;
                for (int m = 0; m < materials.Length; m++)
                {
                    Material material = materials[m];
                    if (material == null) continue;
                    int id = material.GetInstanceID();
                    if (!seen.Add(id)) continue;
                    snapshot.materials.Add(new RecoveryCensusMaterialRecord
                    {
                        instanceId = id,
                        name = material.name,
                        shader = material.shader != null ? material.shader.name : "NULL_SHADER",
                        rendererPath = HierarchyPath(renderer.transform),
                        scene = SceneName(renderer.gameObject),
                        hideFlags = material.hideFlags.ToString()
                    });
                }
            }
        }

        private static void CaptureAutomaticOwners(
            RecoveryRuntimeCensusSnapshot snapshot,
            GameObject[] gameObjects)
        {
            IReadOnlyList<RecoveryAutomaticOwnerRegistration> catalog = RecoveryAutomaticOwnerCatalog.All;
            for (int r = 0; r < catalog.Count; r++)
            {
                RecoveryAutomaticOwnerRegistration registration = catalog[r];
                var paths = new List<string>();
                int active = 0;
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    MonoBehaviour[] behaviours = gameObjects[i].GetComponents<MonoBehaviour>();
                    for (int b = 0; b < behaviours.Length; b++)
                    {
                        MonoBehaviour behaviour = behaviours[b];
                        if (behaviour == null || behaviour.GetType().FullName != registration.Symbol) continue;
                        paths.Add(HierarchyPath(behaviour.transform));
                        if (IsActive(behaviour)) active++;
                    }
                }
                paths.Sort(StringComparer.Ordinal);
                snapshot.automaticOwners.Add(new RecoveryCensusOwnerRecord
                {
                    ownerId = registration.OwnerId,
                    symbol = registration.Symbol,
                    featureId = registration.FeatureId.ToString(),
                    classification = registration.Classification.ToString(),
                    allowedByProfile = RecoveryRuntimeGate.Allows(registration.FeatureId),
                    componentCount = paths.Count,
                    activeCount = active,
                    objectPaths = paths.ToArray()
                });
            }
        }

        private static void BuildFindings(
            RecoveryRuntimeCensusSnapshot snapshot,
            GameObject[] gameObjects)
        {
            int activeXri = CountActive(snapshot.managers, "XRInteractionManager");
            int activeInput = CountActive(snapshot.managers, "InputActionManager");
            if (activeXri > 1)
                AddFinding(snapshot, "DUPLICATE_XRI_MANAGER", "BLOCKER", "XRInteractionManager", "",
                    "Active XRInteractionManager count is " + activeXri + "; expected at most one.");
            if (activeInput > 1)
                AddFinding(snapshot, "DUPLICATE_INPUT_ACTION_MANAGER", "BLOCKER", "InputActionManager", "",
                    "Active InputActionManager count is " + activeInput + "; expected at most one.");

            int activeCanonicalCameras = 0;
            for (int i = 0; i < snapshot.cameras.Count; i++)
            {
                RecoveryCensusComponentRecord camera = snapshot.cameras[i];
                if (!camera.active) continue;
                if (camera.role == "CANONICAL_PLAYER_VIEW") activeCanonicalCameras++;
                else if (camera.role == "UNCLASSIFIED")
                    AddFinding(snapshot, "ACTIVE_CAMERA_ROLE_UNCLASSIFIED", "BLOCKER", camera.type,
                        camera.hierarchyPath,
                        "Active camera has no explicit player, field, spectator or diagnostic role evidence.");
            }
            if (activeCanonicalCameras > 1)
                AddFinding(snapshot, "DUPLICATE_PLAYER_VIEW_CAMERA", "BLOCKER", "Camera", "",
                    "Active canonical player-view camera count is " + activeCanonicalCameras + ".");

            for (int i = 0; i < snapshot.automaticOwners.Count; i++)
            {
                RecoveryCensusOwnerRecord owner = snapshot.automaticOwners[i];
                if (owner.activeCount > 0 && !owner.allowedByProfile)
                    AddFinding(snapshot, "FORBIDDEN_AUTOMATIC_OWNER_ACTIVE", "BLOCKER", owner.ownerId,
                        First(owner.objectPaths),
                        owner.symbol + " is active while profile " + snapshot.activeProfile + " forbids " + owner.featureId + ".");
                if (owner.activeCount > 1 && owner.allowedByProfile)
                    AddFinding(snapshot, "DUPLICATE_CANONICAL_OWNER", "BLOCKER", owner.ownerId,
                        First(owner.objectPaths),
                        owner.symbol + " has " + owner.activeCount + " active instances.");
            }

            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject go = gameObjects[i];
                CreditsHud credits = go.GetComponent<CreditsHud>();
                if (credits != null && IsActive(credits) &&
                    !RecoveryPlayerSurfacePolicy.Allows(RecoveryPlayerSurfaceId.CreditsHud))
                {
                    AddFinding(snapshot, "FORBIDDEN_PLAYER_SURFACE_ACTIVE", "BLOCKER", "CreditsHud",
                        HierarchyPath(go.transform),
                        "Legacy CreditsHud is active in profile " + snapshot.activeProfile + ".");
                }
                if (go.name == "CreditsHudText" && go.activeInHierarchy &&
                    !RecoveryPlayerSurfacePolicy.Allows(RecoveryPlayerSurfaceId.CreditsHud))
                {
                    AddFinding(snapshot, "FORBIDDEN_PLAYER_SURFACE_VISIBLE", "BLOCKER", "CreditsHudText",
                        HierarchyPath(go.transform),
                        "Legacy yellow credits text exists in the recovery player view.");
                }
            }
        }

        private static RecoveryCensusComponentRecord ComponentRecord(
            string category,
            Component component,
            string role)
        {
            Behaviour behaviour = component as Behaviour;
            return new RecoveryCensusComponentRecord
            {
                category = category,
                type = component.GetType().FullName,
                hierarchyPath = HierarchyPath(component.transform),
                scene = SceneName(component.gameObject),
                role = role,
                active = component.gameObject.activeInHierarchy,
                enabled = behaviour == null || behaviour.enabled
            };
        }

        private static string CameraRole(Camera camera)
        {
            if (camera.GetComponentInParent<PlayerRigPersistence>(true) != null)
                return "CANONICAL_PLAYER_VIEW";
            if (camera.CompareTag("MainCamera"))
                return "TAGGED_MAIN_CAMERA";

            string name = camera.gameObject.name.ToLowerInvariant();
            if (name.Contains("field") || name.Contains("photo")) return "FIELD_CAMERA_NAME_EVIDENCE";
            if (name.Contains("spectator")) return "SPECTATOR_CAMERA_NAME_EVIDENCE";
            if (name.Contains("diagnostic") || name.Contains("snapshot")) return "DIAGNOSTIC_CAMERA_NAME_EVIDENCE";
            return "UNCLASSIFIED";
        }

        private static string UiCategory(string fullName)
        {
            if (fullName == "UnityEngine.EventSystems.EventSystem") return "EventSystem";
            if (fullName == "UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule") return "XRUIInputModule";
            if (fullName != null && fullName.StartsWith("TMPro.", StringComparison.Ordinal)) return "TMP";
            if (fullName == "UnityEngine.Rendering.Volume") return "Volume";
            return null;
        }

        private static bool IsActive(Component component)
        {
            if (component == null || !component.gameObject.activeInHierarchy) return false;
            Behaviour behaviour = component as Behaviour;
            return behaviour == null || behaviour.enabled;
        }

        private static bool InScope(Transform value, Transform scopeRoot)
        {
            return scopeRoot == null || value == scopeRoot || value.IsChildOf(scopeRoot);
        }

        private static bool IsDontDestroyOnLoad(GameObject go)
        {
            return string.Equals(go.scene.name, "DontDestroyOnLoad", StringComparison.Ordinal);
        }

        private static string SceneName(GameObject go)
        {
            return go.scene.IsValid() ? go.scene.name : "INVALID_SCENE";
        }

        public static string HierarchyPath(Transform value)
        {
            if (value == null) return "none";
            string path = value.name;
            Transform parent = value.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        private static string[] FeatureIds()
        {
            IReadOnlyList<RecoveryFeatureId> features = RecoveryRuntimeGate.ActiveFeatureIds;
            var result = new string[features.Count];
            for (int i = 0; i < features.Count; i++) result[i] = features[i].ToString();
            Array.Sort(result, StringComparer.Ordinal);
            return result;
        }

        private static int CountActive(List<RecoveryCensusComponentRecord> records, string category)
        {
            int count = 0;
            for (int i = 0; i < records.Count; i++)
                if (records[i].category == category && records[i].active && records[i].enabled) count++;
            return count;
        }

        private static void AddFinding(
            RecoveryRuntimeCensusSnapshot snapshot,
            string code,
            string severity,
            string owner,
            string hierarchyPath,
            string message)
        {
            snapshot.findings.Add(new RecoveryCensusFinding
            {
                code = code,
                severity = severity,
                owner = owner,
                hierarchyPath = hierarchyPath,
                message = message
            });
        }

        private static string First(string[] values)
        {
            return values != null && values.Length > 0 ? values[0] : "";
        }

        private static void Sort(RecoveryRuntimeCensusSnapshot snapshot)
        {
            snapshot.scenes.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            snapshot.roots.Sort((a, b) => string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath));
            snapshot.cameras.Sort(ComponentCompare);
            snapshot.managers.Sort(ComponentCompare);
            snapshot.ui.Sort(ComponentCompare);
            snapshot.audioSources.Sort(ComponentCompare);
            snapshot.materials.Sort((a, b) => string.CompareOrdinal(a.rendererPath, b.rendererPath));
            snapshot.automaticOwners.Sort((a, b) => string.CompareOrdinal(a.ownerId, b.ownerId));
            snapshot.findings.Sort((a, b) =>
            {
                int code = string.CompareOrdinal(a.code, b.code);
                return code != 0 ? code : string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath);
            });
        }

        private static int ComponentCompare(
            RecoveryCensusComponentRecord a,
            RecoveryCensusComponentRecord b)
        {
            int category = string.CompareOrdinal(a.category, b.category);
            return category != 0 ? category : string.CompareOrdinal(a.hierarchyPath, b.hierarchyPath);
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "runtime-census";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }

        private static string BuildMarkdown(RecoveryRuntimeCensusSnapshot snapshot)
        {
            var text = new StringBuilder(2048);
            text.AppendLine("# Recovery Runtime Census — " + snapshot.label);
            text.AppendLine();
            text.AppendLine("- Captured: `" + snapshot.capturedAtUtc + "`");
            text.AppendLine("- Profile: `" + snapshot.activeProfile + "`");
            text.AppendLine("- Scope: `" + snapshot.scopePath + "`");
            text.AppendLine("- Scenes: " + snapshot.scenes.Count);
            text.AppendLine("- Roots: " + snapshot.roots.Count);
            text.AppendLine("- Cameras: " + snapshot.cameras.Count);
            text.AppendLine("- Managers: " + snapshot.managers.Count);
            text.AppendLine("- UI records: " + snapshot.ui.Count);
            text.AppendLine("- Materials in scope: " + snapshot.materials.Count);
            text.AppendLine("- Audio sources: " + snapshot.audioSources.Count);
            text.AppendLine("- Findings: " + snapshot.findings.Count);
            text.AppendLine();
            text.AppendLine("## Findings");
            text.AppendLine();
            if (snapshot.findings.Count == 0)
            {
                text.AppendLine("None.");
            }
            else
            {
                text.AppendLine("| Severity | Code | Owner | Path | Message |");
                text.AppendLine("|---|---|---|---|---|");
                for (int i = 0; i < snapshot.findings.Count; i++)
                {
                    RecoveryCensusFinding finding = snapshot.findings[i];
                    text.Append("| ").Append(Escape(finding.severity))
                        .Append(" | ").Append(Escape(finding.code))
                        .Append(" | ").Append(Escape(finding.owner))
                        .Append(" | ").Append(Escape(finding.hierarchyPath))
                        .Append(" | ").Append(Escape(finding.message)).AppendLine(" |");
                }
            }
            text.AppendLine();
            text.AppendLine("## Automatic owners");
            text.AppendLine();
            text.AppendLine("| Owner | Feature | Allowed | Components | Active |");
            text.AppendLine("|---|---|---:|---:|---:|");
            for (int i = 0; i < snapshot.automaticOwners.Count; i++)
            {
                RecoveryCensusOwnerRecord owner = snapshot.automaticOwners[i];
                text.Append("| ").Append(Escape(owner.ownerId))
                    .Append(" | ").Append(Escape(owner.featureId))
                    .Append(" | ").Append(owner.allowedByProfile ? "yes" : "no")
                    .Append(" | ").Append(owner.componentCount)
                    .Append(" | ").Append(owner.activeCount).AppendLine(" |");
            }
            return text.ToString();
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
