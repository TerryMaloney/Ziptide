using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Device correction for the cold-start menu. The panel follows only while tracking settles, then
    /// becomes world-anchored. While the Home Hub owns boot, belt sockets are interaction-suppressed so
    /// their hip spheres cannot intercept New Game/Continue/Settings rays. A committed departure is a
    /// one-way lifecycle transition: sockets are restored from cached state and can never be suppressed
    /// again by the dying Boot scene.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HomeHubAnchorLockRuntime : MonoBehaviour
    {
        private HomeHubRuntime _hub;
        private bool _beltSuppressed;
        private bool _departureCommitted;
        private float _nextBeltScan;
        private readonly Dictionary<HolsterSocketInteractor, bool> _socketStates =
            new Dictionary<HolsterSocketInteractor, bool>();
        private readonly Dictionary<Collider, bool> _colliderStates =
            new Dictionary<Collider, bool>();

        private void Awake()
        {
            _hub = GetComponent<HomeHubRuntime>();
        }

        private void OnEnable()
        {
            HomeHubRuntime.ChoiceSelected += OnChoiceSelected;
            if (!_departureCommitted) SuppressBeltSockets();
        }

        private void OnDisable()
        {
            HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;
            RestoreBeltSockets();
        }

        private void OnDestroy()
        {
            HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;
            RestoreBeltSockets();
        }

        private void Update()
        {
            if (_departureCommitted) return;
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
            if (!_departureCommitted) SuppressBeltSockets();

            float deadline = Time.realtimeSinceStartup + 4f;
            float stableFor = 0f;
            Vector3 lastPosition = Vector3.positiveInfinity;
            Quaternion lastRotation = Quaternion.identity;

            while (!_departureCommitted && Time.realtimeSinceStartup < deadline)
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

            if (!_departureCommitted && _hub != null)
            {
                _hub.enabled = false;
                Debug.Log("ZIPTIDE: HOME_HUB_ANCHOR locked_world=true pos=" + transform.position.ToString("F2"));
            }
        }

        private void OnChoiceSelected(HomeHubChoice choice)
        {
            if (choice == HomeHubChoice.Settings) return;

            // This fixes the device-proven race where New Game restored sockets, Update suppressed them
            // again 16 ms later, and the unloading Boot scene could no longer find the persistent belt.
            _departureCommitted = true;
            RestoreBeltSockets();
            Debug.Log("ZIPTIDE: HOME_HUB_DEPARTURE sockets_restored=true choice=" + choice);
            enabled = false;
        }

        private void SuppressBeltSockets()
        {
            if (_departureCommitted) return;

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

                if (!_socketStates.ContainsKey(socket)) _socketStates.Add(socket, socket.enabled);
                if (socket.enabled)
                {
                    socket.enabled = false;
                    changed++;
                }

                Collider[] colliders = socket.GetComponents<Collider>();
                for (int j = 0; j < colliders.Length; j++)
                {
                    Collider col = colliders[j];
                    if (col == null) continue;
                    if (!_colliderStates.ContainsKey(col)) _colliderStates.Add(col, col.enabled);
                    col.enabled = false;
                }
            }

            _beltSuppressed = true;
            if (changed > 0)
                Debug.Log("ZIPTIDE: HOME_HUB_BELT_SUPPRESS sockets=" + sockets.Length);
        }

        private void RestoreBeltSockets()
        {
            if (!_beltSuppressed && _socketStates.Count == 0 && _colliderStates.Count == 0) return;

            int restored = 0;
            foreach (KeyValuePair<Collider, bool> pair in _colliderStates)
                if (pair.Key != null) pair.Key.enabled = pair.Value;

            foreach (KeyValuePair<HolsterSocketInteractor, bool> pair in _socketStates)
            {
                if (pair.Key == null) continue;
                pair.Key.enabled = pair.Value;
                restored++;
            }

            _colliderStates.Clear();
            _socketStates.Clear();
            _beltSuppressed = false;
            Debug.Log("ZIPTIDE: HOME_HUB_BELT_RESTORE sockets=" + restored);
        }
    }
}
