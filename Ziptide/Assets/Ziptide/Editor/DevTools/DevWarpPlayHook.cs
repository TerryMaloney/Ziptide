#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Gameplay.DevTools;

namespace Ziptide.Editor.DevTools
{
    /// <summary>
    /// Bridges the editor <see cref="DevWarpWindow"/> "Play here →" action into the running game:
    /// when play mode is entered with a pending warp stashed in SessionState, it asks the runtime
    /// <see cref="DevWarp"/> service to warp once the boot sequence settles, then clears the request.
    /// Editor-only.
    /// </summary>
    [InitializeOnLoad]
    public static class DevWarpPlayHook
    {
        static DevWarpPlayHook()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredPlayMode) return;

            string scene = SessionState.GetString(DevWarpWindow.PendingSceneKey, string.Empty);
            string marker = SessionState.GetString(DevWarpWindow.PendingMarkerKey, string.Empty);

            // Always clear so a normal Play doesn't re-trigger a stale warp.
            SessionState.EraseString(DevWarpWindow.PendingSceneKey);
            SessionState.EraseString(DevWarpWindow.PendingMarkerKey);

            if (string.IsNullOrEmpty(scene)) return; // plain "Play from _Boot" — let boot do its thing.

            // DevWarp's runner waits out the boot scene's initial travel before warping.
            DevWarp.WarpToScene(scene, string.IsNullOrEmpty(marker) ? null : marker);
        }
    }
}
#endif
