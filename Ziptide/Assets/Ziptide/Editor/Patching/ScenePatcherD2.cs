using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// D2 patcher: ensures LocomotionDirector + profile, PlayerRigPersistence,
    /// SpawnMarkerRuntime, and both turn providers exist in every scene.
    /// Runs during batch build after D1 patcher.
    /// </summary>
    public static class ScenePatcherD2
    {
        private const string DefaultProfilePath = "Assets/Ziptide/Content/Locomotion/DefaultLocomotionProfile.asset";
        private const string SpawnMarkerName = "__SPAWN_PLAYER";
        private const string DefaultAudioProfilePath = "Assets/Ziptide/Content/Audio/DefaultCityAudioProfile.asset";
        private const string AudioClipPath = "Assets/Ziptide/Content/Audio/Clips/Zerogravity_Bloom_Favorite_1.wav";

        [MenuItem("Ziptide/Apply D2 Controls + Persistence To Current Scene")]
        public static void PatchFromMenu()
        {
            PatchActiveScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Ziptide] D2 patcher applied to current scene.");
        }

        public static void PatchActiveScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return;

            string sceneName = scene.name;

            // World scenes: content only. No XR rig — it lives only in _Boot (DontDestroyOnLoad).
            if (sceneName != "_Boot")
            {
                StripRigFromWorldScene();
                EnsureSpawnMarker(sceneName);
                return;
            }

            EnsureLocomotionRig();
            var profile = EnsureLocomotionProfileAsset();
            EnsureLocomotionDirector(profile);
            EnsurePlayerRigPersistence();
            EnsureSpawnMarker(sceneName);
            EnsureAudioDirector();
            EnsureAudioProfileAsset();
            EnsureSingletonValidator();
            EnsureTravelCoordinator();
        }

        /// <summary>Removes any XR Origin, XRInteractionManager, and DontDestroyOnLoad singletons from the active scene so world scenes are content-only. Singletons (TravelCoordinator, AudioDirector) must live only in _Boot.</summary>
        private static void StripRigFromWorldScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root == null) continue;
                bool destroy = false;
                if (root.name == "XR Origin" || root.name.Contains("XR Origin"))
                    destroy = true;
                var xrOriginType = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
                if (xrOriginType != null && root.GetComponent(xrOriginType) != null)
                    destroy = true;
                if (root.GetComponent<XRInteractionManager>() != null)
                    destroy = true;
                // Singletons must live only in _Boot; strip from world scenes so audit passes.
                if (root.name == "__TravelCoordinator" || root.GetComponent<TravelCoordinator>() != null)
                    destroy = true;
                if (root.name == "__AudioDirector" || root.GetComponent<AudioDirector>() != null)
                    destroy = true;
                if (destroy)
                {
                    Object.DestroyImmediate(root);
                    Debug.Log("[Ziptide] D2 stripped rig from world scene: removed " + root.name);
                }
            }
        }

        private static void EnsureLocomotionRig()
        {
            Ziptide.Editor.Setup.EnsureLocomotionRig.Run();
        }

        private static LocomotionProfile EnsureLocomotionProfileAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<LocomotionProfile>(DefaultProfilePath);
            if (existing != null)
            {
                var so = new SerializedObject(existing);
                PatcherUtil.SetFloat(so, "smoothTurnSpeed", 120f);
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                return existing;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/Locomotion"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
                AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Locomotion");
            }

            var profile = ScriptableObject.CreateInstance<LocomotionProfile>();
            AssetDatabase.CreateAsset(profile, DefaultProfilePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created DefaultLocomotionProfile at " + DefaultProfilePath);
            return profile;
        }

        private static void EnsureLocomotionDirector(LocomotionProfile profile)
        {
            GameObject xrOriginGo = FindXROrigin();
            if (xrOriginGo == null) return;

            var director = xrOriginGo.GetComponentInChildren<LocomotionDirector>(true);
            if (director == null)
            {
                var locoSystem = xrOriginGo.transform.Find("Locomotion System");
                GameObject host = locoSystem != null ? locoSystem.gameObject : xrOriginGo;
                director = host.AddComponent<LocomotionDirector>();
            }

            var so = new SerializedObject(director);
            PatcherUtil.SetObjectRef(so, "profile", profile);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsurePlayerRigPersistence()
        {
            GameObject xrOriginGo = FindXROrigin();
            if (xrOriginGo == null) return;
            PatcherUtil.EnsureComponent<PlayerRigPersistence>(xrOriginGo);
            // Anti-stuck failsafe: hold both grips 1s to emergency respawn.
            PatcherUtil.EnsureComponent<EmergencyRespawn>(xrOriginGo);
        }

        public static void EnsureSpawnMarker(string sceneName)
        {
            Vector3 spawnPos = GetSceneSpawnPosition(sceneName);
            var go = PatcherUtil.EnsureRootObject(SpawnMarkerName, spawnPos);
            PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);

            var so = new SerializedObject(go.GetComponent<SpawnMarkerRuntime>());
            PatcherUtil.SetString(so, "markerId", "player");
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Vector3 GetSceneSpawnPosition(string sceneName)
        {
            if (sceneName == "D0_City")
                // Aligned with CourtyardA_Spawn center in D1 city (Z=-16, Y=walkwayHeight=2.5+0.1 for player capsule).
                return new Vector3(0f, 2.6f, -16f);
            return new Vector3(0f, 0.1f, 0f);
        }

        private static void EnsureSingletonValidator()
        {
            if (Object.FindObjectOfType<SingletonValidator>() != null) return;
            var go = PatcherUtil.EnsureRootObject("__SingletonValidator", Vector3.zero);
            PatcherUtil.EnsureComponent<SingletonValidator>(go);
        }

        private static void EnsureTravelCoordinator()
        {
            if (Object.FindObjectOfType<Ziptide.Gameplay.TravelCoordinator>() != null) return;
            var go = PatcherUtil.EnsureRootObject("__TravelCoordinator", Vector3.zero);
            PatcherUtil.EnsureComponent<Ziptide.Gameplay.TravelCoordinator>(go);
        }

        private static void EnsureAudioDirector()
        {
            if (Object.FindObjectOfType<AudioDirector>() != null) return;
            var go = PatcherUtil.EnsureRootObject("__AudioDirector", Vector3.zero);
            PatcherUtil.EnsureComponent<AudioDirector>(go);
        }

        public static AudioProfile EnsureAudioProfileAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<AudioProfile>(DefaultAudioProfilePath);
            if (existing != null) return existing;

            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/Audio"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
                AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Audio");
            }

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioClipPath);

            var profile = ScriptableObject.CreateInstance<AudioProfile>();
            profile.clip = clip;
            profile.volume = 0.35f;
            profile.loop = true;
            profile.crossfadeSeconds = 2f;
            profile.enabled = true;
            AssetDatabase.CreateAsset(profile, DefaultAudioProfilePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created DefaultCityAudioProfile at " + DefaultAudioProfilePath);
            return profile;
        }

        private static GameObject FindXROrigin()
        {
            var xrOriginType = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (xrOriginType != null)
            {
                var xr = Object.FindObjectOfType(xrOriginType) as Component;
                if (xr != null) return xr.gameObject;
            }
            var byName = GameObject.Find("XR Origin");
            if (byName != null) return byName;
            return null;
        }
    }
}
