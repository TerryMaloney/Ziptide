using System.Collections;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Device correction for the cold-start menu. HomeHubRuntime may follow the camera only while the
    /// first tracked pose is settling; once the head is stable this component freezes the already-built
    /// world-space panel. Tile delegates remain live, but ordinary head turns no longer drag the menu.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HomeHubAnchorLockRuntime : MonoBehaviour
    {
        private HomeHubRuntime _hub;

        private void Awake()
        {
            _hub = GetComponent<HomeHubRuntime>();
        }

        private IEnumerator Start()
        {
            float deadline = Time.realtimeSinceStartup + 4f;
            float stableFor = 0f;
            Vector3 lastPosition = Vector3.positiveInfinity;
            Quaternion lastRotation = Quaternion.identity;

            while (Time.realtimeSinceStartup < deadline)
            {
                Camera cam = Camera.main;
                if (cam == null)
                {
                    stableFor = 0f;
                    yield return null;
                    continue;
                }

                float distance = lastPosition.x == float.PositiveInfinity
                    ? float.MaxValue : Vector3.Distance(lastPosition, cam.transform.position);
                float angle = Quaternion.Angle(lastRotation, cam.transform.rotation);
                if (distance <= 0.015f && angle <= 1.5f) stableFor += Time.unscaledDeltaTime;
                else stableFor = 0f;

                lastPosition = cam.transform.position;
                lastRotation = cam.transform.rotation;
                if (stableFor >= 0.45f) break;
                yield return null;
            }

            // HomeHubRuntime has already created the tiles and captured their callbacks. Disabling its
            // Update freezes presentation only; selecting New/Continue/Settings still invokes those callbacks.
            if (_hub != null)
            {
                _hub.enabled = false;
                Debug.Log("ZIPTIDE: HOME_HUB_ANCHOR locked_world=true pos=" + transform.position.ToString("F2"));
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            HomeHubRuntime[] hubs = Object.FindObjectsOfType<HomeHubRuntime>(true);
            for (int i = 0; i < hubs.Length; i++)
            {
                if (hubs[i] != null && hubs[i].GetComponent<HomeHubAnchorLockRuntime>() == null)
                    hubs[i].gameObject.AddComponent<HomeHubAnchorLockRuntime>();
            }
        }
    }
}
