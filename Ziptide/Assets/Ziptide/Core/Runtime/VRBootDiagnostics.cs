using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Ziptide.Runtime
{
    /// <summary>
    /// Logs scene, cameras, XR, URP, and renderer state at startup for adb logcat inspection.
    /// </summary>
    public static class VRBootDiagnostics
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            Debug.Log("[Ziptide] VRBootDiagnostics: --- start ---");

            try
            {
                Scene sc = SceneManager.GetActiveScene();
                Debug.Log($"[Ziptide] VRBootDiagnostics: scene={sc.name}, rootCount={sc.rootCount}");

                LogCameras();
                LogXRDisplay();
                LogURPPipeline();
                LogRenderers();
            }
            catch (System.Exception e)
            {
                Debug.Log($"[Ziptide] VRBootDiagnostics: error - {e.Message}\n{e.StackTrace}");
            }

            Debug.Log("[Ziptide] VRBootDiagnostics: --- end ---");
        }

        private static void LogCameras()
        {
            Camera[] all = Object.FindObjectsOfType<Camera>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Camera c = all[i];
                if (c == null) { Debug.Log("[Ziptide] VRBootDiagnostics: camera[" + i + "] is null"); continue; }
                string parentChain = GetParentChain(c.transform);
                int clearFlags = (int)c.clearFlags;
                Debug.Log($"[Ziptide] VRBootDiagnostics: camera name={c.name} enabled={c.enabled} activeInHierarchy={c.gameObject.activeInHierarchy} clearFlags={clearFlags} tag={c.tag} parentChain={parentChain}");
            }
        }

        private static string GetParentChain(Transform t)
        {
            var names = new List<string>();
            while (t != null)
            {
                names.Add(t.name ?? "(null)");
                t = t.parent;
            }
            names.Reverse();
            return string.Join("/", names);
        }

        private static void LogXRDisplay()
        {
            try
            {
                var displays = new List<XRDisplaySubsystem>();
                SubsystemManager.GetInstances(displays);
                if (displays.Count == 0)
                    Debug.Log("[Ziptide] VRBootDiagnostics: XR display subsystems=0 (not running or no XR loader)");
                else
                {
                    for (int i = 0; i < displays.Count; i++)
                        Debug.Log($"[Ziptide] VRBootDiagnostics: XR display[{i}] running={displays[i].running}");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"[Ziptide] VRBootDiagnostics: XR display query failed - {e.Message}");
            }
        }

        private static void LogURPPipeline()
        {
            try
            {
                var rp = GraphicsSettings.currentRenderPipeline;
                if (rp == null)
                    Debug.Log("[Ziptide] VRBootDiagnostics: currentRenderPipeline=null");
                else
                    Debug.Log($"[Ziptide] VRBootDiagnostics: currentRenderPipeline={rp.name}");
            }
            catch (System.Exception e)
            {
                Debug.Log($"[Ziptide] VRBootDiagnostics: URP query failed - {e.Message}");
            }
        }

        private static void LogRenderers()
        {
            Renderer[] all = Object.FindObjectsOfType<Renderer>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Renderer r = all[i];
                if (r == null) { Debug.Log("[Ziptide] VRBootDiagnostics: renderer[" + i + "] is null"); continue; }
                string matName = "(null)";
                string shaderName = "(null)";
                try
                {
                    if (r.sharedMaterial != null)
                    {
                        matName = r.sharedMaterial.name ?? "(null)";
                        if (r.sharedMaterial.shader != null)
                            shaderName = r.sharedMaterial.shader.name ?? "(null)";
                    }
                }
                catch (System.Exception e)
                {
                    matName = "error:" + e.Message;
                }
                Bounds b = r.bounds;
                Debug.Log($"[Ziptide] VRBootDiagnostics: renderer name={r.gameObject.name} material={matName} shader={shaderName} bounds=({b.center.x:F2},{b.center.y:F2},{b.center.z:F2}) size=({b.size.x:F2},{b.size.y:F2},{b.size.z:F2})");
            }
        }
    }
}
