#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Keeps editor physics aligned with scenes opened synchronously by the batch world audit.
    /// EditorSceneManager.OpenScene returns before the next normal editor update tick; without an
    /// explicit sync, immediate raycasts can miss valid colliders already serialized in the scene.
    /// </summary>
    [InitializeOnLoad]
    public static class AuditPhysicsSync
    {
        static AuditPhysicsSync()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;

            Physics.autoSyncTransforms = true;
            Physics.SyncTransforms();

            if (scene.name == "W011_Undercroft")
                Debug.Log("[Ziptide] audit physics synchronized scene=W011_Undercroft");
        }
    }
}
#endif
