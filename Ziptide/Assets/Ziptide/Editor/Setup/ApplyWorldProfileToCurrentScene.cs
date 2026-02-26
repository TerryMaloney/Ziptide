using UnityEngine;
using UnityEditor;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Setup
{
    public static class ApplyWorldProfileToCurrentScene
    {
        private const string MenuPath = "Ziptide/Apply World Profile To Current Scene";
        private const string DefaultWorldPath = "Assets/Ziptide/Content/Worlds/DefaultWorldProfile.asset";

        [MenuItem(MenuPath)]
        public static void ApplyWorldProfile()
        {
            WorldProfile profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldPath);
            if (profile == null)
            {
                CreateDefaultWorldProfile.CreateDefaultWorld();
                profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldPath);
            }
            if (profile == null)
            {
                Debug.LogWarning("[Ziptide] Default World Profile not found. Run Ziptide > Create Default World Profile first.");
                return;
            }

            WorldRuntime worldRuntime = Object.FindObjectOfType<WorldRuntime>();
            if (worldRuntime == null)
            {
                GameObject go = new GameObject("WorldRuntime");
                worldRuntime = go.AddComponent<WorldRuntime>();
                SerializedObject so = new SerializedObject(worldRuntime);
                so.FindProperty("worldProfile").objectReferenceValue = profile;
                so.ApplyModifiedPropertiesWithoutUndo();
                Undo.RegisterCreatedObjectUndo(go, "Apply World Profile");
            }
            else
            {
                SerializedObject so = new SerializedObject(worldRuntime);
                var prop = so.FindProperty("worldProfile");
                if (prop.objectReferenceValue == null)
                {
                    prop.objectReferenceValue = profile;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            ThemeSwitchStation station = Object.FindObjectOfType<ThemeSwitchStation>();
            if (station == null)
            {
                GameObject go = new GameObject("ThemeSwitchStation");
                go.transform.position = profile.spawnPosition + Vector3.forward * 1.5f;
                go.AddComponent<ThemeSwitchStation>();
                Undo.RegisterCreatedObjectUndo(go, "Theme Switch Station");
            }

            GameObject xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin != null)
            {
                FallRespawner respawner = xrOrigin.GetComponent<FallRespawner>();
                if (respawner == null)
                {
                    respawner = xrOrigin.AddComponent<FallRespawner>();
                    SerializedObject so = new SerializedObject(respawner);
                    so.FindProperty("worldRuntime").objectReferenceValue = worldRuntime;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                else
                {
                    SerializedObject so = new SerializedObject(respawner);
                    if (so.FindProperty("worldRuntime").objectReferenceValue == null)
                    {
                        so.FindProperty("worldRuntime").objectReferenceValue = worldRuntime;
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
            }

            EditorUtility.SetDirty(worldRuntime.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(worldRuntime.gameObject.scene);
            Debug.Log("[Ziptide] Applied World Profile to scene. WorldRuntime, ThemeSwitchStation, FallRespawner configured.");
        }
    }
}
