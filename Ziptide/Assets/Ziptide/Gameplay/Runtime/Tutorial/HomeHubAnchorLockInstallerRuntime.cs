using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>Finds the BootLoader-created HomeHubRuntime even when it appears after AfterSceneLoad.</summary>
    public sealed class HomeHubAnchorLockInstallerRuntime : MonoBehaviour
    {
        private static HomeHubAnchorLockInstallerRuntime _instance;
        private float _nextScan;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstaller()
        {
            if (_instance != null) return;
            GameObject go = new GameObject("__HomeHubAnchorLockInstaller");
            Object.DontDestroyOnLoad(go);
            _instance = go.AddComponent<HomeHubAnchorLockInstallerRuntime>();
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextScan) return;
            _nextScan = Time.unscaledTime + 0.2f;
            HomeHubRuntime[] hubs = Object.FindObjectsOfType<HomeHubRuntime>(true);
            for (int i = 0; i < hubs.Length; i++)
            {
                HomeHubRuntime hub = hubs[i];
                if (hub != null && hub.GetComponent<HomeHubAnchorLockRuntime>() == null)
                    hub.gameObject.AddComponent<HomeHubAnchorLockRuntime>();
            }
        }
    }
}
