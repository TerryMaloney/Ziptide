using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.InputSystem;

namespace Ziptide.Runtime
{
    /// <summary>
    /// Development-only on-screen HUD showing XR status, input system, and controller count.
    /// Only active when Debug.isDebugBuild is true. Toggle via DebugHUD.Enabled.
    /// </summary>
    public static class DebugHUD
    {
        public static bool Enabled { get; set; } = true;

        private static GameObject s_Root;
        private static Text s_Text;
        private static Coroutine s_UpdateCoroutine;
        private static bool s_LoggedXrQueryError;
        private static bool s_LoggedInputQueryError;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            if (!Debug.isDebugBuild || !Enabled) return;

            s_Root = new GameObject("Ziptide_DebugHUD");
            Object.DontDestroyOnLoad(s_Root);

            var canvas = s_Root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;
            s_Root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s_Root.AddComponent<GraphicRaycaster>();

            var go = new GameObject("Text");
            go.transform.SetParent(s_Root.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(20, 20);
            rect.sizeDelta = new Vector2(400, 120);

            s_Text = go.AddComponent<Text>();
            s_Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            s_Text.fontSize = 18;
            s_Text.color = new Color(1f, 1f, 1f, 0.9f);
            s_Text.text = "Ziptide Debug HUD";

            var host = s_Root.AddComponent<DebugHUDUpdater>();
            s_UpdateCoroutine = host.StartCoroutine(UpdateLoop());
        }

        private static IEnumerator UpdateLoop()
        {
            var wait = new WaitForSeconds(1f);
            while (s_Root != null && s_Text != null)
            {
                if (Enabled)
                {
                    s_Text.enabled = true;
                    s_Text.text = BuildStatusText();
                }
                else
                {
                    s_Text.enabled = false;
                }
                yield return wait;
            }
        }

        private static string BuildStatusText()
        {
            bool xrRunning = false;
            try
            {
                var displays = new List<XRDisplaySubsystem>();
                SubsystemManager.GetInstances(displays);
                xrRunning = displays.Count > 0 && displays[0].running;
            }
            catch (System.Exception e)
            {
                if (!s_LoggedXrQueryError)
                {
                    s_LoggedXrQueryError = true;
                    Debug.LogWarning("[Ziptide] DebugHUD: XR subsystem query failed - " + e.Message);
                }
            }

            string inputName = "Unknown";
#if ENABLE_INPUT_SYSTEM
            inputName = "NewInputSystem";
#elif ENABLE_LEGACY_INPUT_MANAGER
            inputName = "Legacy";
#endif

            int controllerCount = 0;
            try
            {
                var devices = InputSystem.devices;
                for (int i = 0; i < devices.Count; i++)
                {
                    var d = devices[i];
                    if (d == null) continue;
                    string name = d.name ?? "";
                    if (name.Contains("XR") || name.Contains("Controller") || name.Contains("Hand"))
                        controllerCount++;
                }
            }
            catch (System.Exception e)
            {
                if (!s_LoggedInputQueryError)
                {
                    s_LoggedInputQueryError = true;
                    Debug.LogWarning("[Ziptide] DebugHUD: input device query failed - " + e.Message);
                }
            }

            return $"XR: {(xrRunning ? "YES" : "NO")}\nInput: {inputName}\nControllers: {controllerCount}";
        }

        private class DebugHUDUpdater : MonoBehaviour { }
    }
}
