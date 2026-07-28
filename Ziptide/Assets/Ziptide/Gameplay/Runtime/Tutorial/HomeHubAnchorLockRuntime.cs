using System.Collections;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Device correction for the cold-start menu. The panel follows only while tracking settles, then
    /// becomes world-anchored. While the Home Hub owns boot, belt sockets are interaction-suppressed so
    /// their hip spheres cannot intercept New Game/Continue/Settings rays. They are restored before
    /// New Game or Continue commits travel.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HomeHubAnchorLockRuntime : MonoBehaviour
    {
        private HomeHubRuntime _hub;
        private bool _beltSuppressed;
        private float _nextBeltScan;

        private void Awake()
        {
            _hub = GetComponent<HomeHubRuntime>();
        }

        private void OnEnable()
        {
            HomeHubRuntime.ChoiceSelected += OnChoiceSelected;
            SuppressBeltSockets();
        }

        private void OnDisable()
        {
            HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;
        }

        private void OnDestroy()
        {
            HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;
            RestoreBeltSockets();
        }

        private void Update()
        {
            if (!_beltSuppressed && Time.unscaledTime >= _nextBeltScan)
            {
                _nextBeltScan = Time.unscaledTime + 0.05f;
                SuppressBeltSockets();
            }
        }

        private IEnumerator Start()
        {
            // One extra frame covers BeltRig.Start creating its sockets after this component's Awake.
            yield return null;
            SuppressBeltSockets();

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

            if (_hub != null)
            {
                _hub.enabled = false;
                Debug.Log("ZIPTIDE: HOME_HUB_ANCHOR locked_world=true pos=" + transform.position.ToString("F2"));
            }
        }

        private void OnChoiceSelected(HomeHubChoice choice)
        {
            if (choice == HomeHubChoice.Settings) return;
            RestoreBeltSockets();
        }

        private void SuppressBeltSockets()
        {
            HolsterSocketInteractor[] sockets = Object.FindObjectsOfType<HolsterSocketInteractor>(true);
            if (sockets == null || sockets.Length == 0)
            {
                _beltSuppressed = false;
                return;
            }

            int changed = 0;
            for (int i = 0; i < sockets.Length; i++)
            {
                HolsterSocketInteractor socket = sockets[i];
                if (socket == null) continue;
                if (socket.enabled)
                {
                    socket.enabled = false;
                    changed++;
                }
                Collider[] colliders = socket.GetComponents<Collider>();
                for (int j = 0; j < colliders.Length; j++)
                    if (colliders[j] != null) colliders[j].enabled = false;
            }

            _beltSuppressed = true;
            if (changed > 0)
                Debug.Log("ZIPTIDE: HOME_HUB_BELT_SUPPRESS sockets=" + sockets.Length);
        }

        private void RestoreBeltSockets()
        {
            if (!_beltSuppressed) return;
            HolsterSocketInteractor[] sockets = Object.FindObjectsOfType<HolsterSocketInteractor>(true);
            for (int i = 0; i < sockets.Length; i++)
            {
                HolsterSocketInteractor socket = sockets[i];
                if (socket == null) continue;
                Collider[] colliders = socket.GetComponents<Collider>();
                for (int j = 0; j < colliders.Length; j++)
                    if (colliders[j] != null) colliders[j].enabled = true;
                socket.enabled = true;
            }
            _beltSuppressed = false;
            Debug.Log("ZIPTIDE: HOME_HUB_BELT_RESTORE sockets=" + sockets.Length);
        }
    }
}
