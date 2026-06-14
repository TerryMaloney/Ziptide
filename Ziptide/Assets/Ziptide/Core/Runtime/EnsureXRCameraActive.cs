using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Ensures only the XR Origin camera is active on XR builds (e.g. Quest).
    /// Disables any other cameras so the headset view is driven by XR, not a 2D camera.
    /// </summary>
    public static class EnsureXRCameraActive
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            Camera[] all = Object.FindObjectsOfType<Camera>(true);
            int disabled = 0;
            int kept = 0;
            foreach (Camera c in all)
            {
                if (c == null || !c.gameObject.activeInHierarchy)
                    continue;
                bool underXROrigin = IsUnderXROrigin(c.transform);
                if (underXROrigin)
                {
                    kept++;
                    continue;
                }
                c.gameObject.SetActive(false);
                disabled++;
            }
            Debug.Log($"[Ziptide] EnsureXRCameraActive: kept {kept} XR camera(s), disabled {disabled} non-XR camera(s).");
        }

        private static bool IsUnderXROrigin(Transform t)
        {
            while (t != null)
            {
                if (t.name.Contains("XR Origin") || t.name.Contains("Camera Offset"))
                    return true;
                t = t.parent;
            }
            return false;
        }
    }
}
