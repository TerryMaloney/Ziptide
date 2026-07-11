using System;
using System.Globalization;
using UnityEngine;

namespace Ziptide.Gameplay.Tutorial
{
    /// <summary>
    /// Read-only Unity adapter for FH-S01. A future TutorialDirector activates one recognized beat
    /// at a time and receives a neutral semantic completion signal. This component never moves or
    /// locks the rig, changes locomotion, writes profiles, loads scenes, or presents dialogue.
    /// </summary>
    public sealed class FirstHourObservationAdapter : MonoBehaviour
    {
        public const string LookBeatId = "FH_LOOK_AT_RILL";
        public const string MoveBeatId = "FH_MOVE_IN_QUARTERS";
        public const string ArrivalBeatId = "FH_W001_ARRIVAL";

        public const string LookSignalId = "PLAYER_LOOKED_AT_RILL";
        public const string MoveSignalId = "PLAYER_MOVED_SAFE_DISTANCE";
        public const string ArrivalSignalId = "W001_ARRIVAL_ORIENTATION_COMPLETE";

        private const string RillOrbName = "__RillOrb";
        private const float WaitingLogIntervalSeconds = 1f;

        private readonly FirstHourObservationCore _core = new FirstHourObservationCore();

        private string _activeBeatId;
        private string _activeSignalId;
        private bool _emitted;
        private bool _missingCameraLogged;
        private bool _missingTargetLogged;
        private float _nextWaitingLogAt;
        private Transform _rillTarget;

        public static FirstHourObservationAdapter Instance { get; private set; }

        public event Action<string> SignalCompleted;

        public string ActiveBeatId => _activeBeatId;
        public string ActiveSignalId => _activeSignalId;
        public bool IsTracking => !_emitted && !string.IsNullOrEmpty(_activeBeatId);
        public bool HasEmitted => _emitted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null || FindObjectOfType<FirstHourObservationAdapter>() != null) return;
            var go = new GameObject("__FirstHourObservationAdapter");
            DontDestroyOnLoad(go);
            go.AddComponent<FirstHourObservationAdapter>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Activates and resets one supported observation beat. Unknown beats are rejected and leave
        /// the current tracker unchanged.
        /// </summary>
        public bool BeginBeat(string beatId)
        {
            string signalId;
            if (!TryGetSignalId(beatId, out signalId)) return false;

            _activeBeatId = beatId;
            _activeSignalId = signalId;
            _emitted = false;
            _missingCameraLogged = false;
            _missingTargetLogged = false;
            _nextWaitingLogAt = Time.unscaledTime + WaitingLogIntervalSeconds;
            _rillTarget = null;

            if (beatId == LookBeatId) _core.ResetLook();
            else if (beatId == MoveBeatId) _core.ResetMovement();
            else _core.ResetArrival();

            LogWaiting(0d, "begin");
            return true;
        }

        public void CancelBeat()
        {
            _activeBeatId = null;
            _activeSignalId = null;
            _emitted = false;
            _rillTarget = null;
            _missingCameraLogged = false;
            _missingTargetLogged = false;
        }

        private void Update()
        {
            if (!IsTracking) return;

            Camera camera = Camera.main;
            if (camera == null)
            {
                LogMissingOnce(ref _missingCameraLogged, "missing_camera");
                return;
            }

            bool complete;
            double value;

            if (_activeBeatId == LookBeatId)
            {
                complete = UpdateLook(camera.transform, out value);
            }
            else if (_activeBeatId == MoveBeatId)
            {
                Vector3 position = camera.transform.position;
                complete = _core.SampleMovement(ToObservationVector(position));
                value = _core.MovementProgress01;
            }
            else if (_activeBeatId == ArrivalBeatId)
            {
                complete = _core.SampleArrival(
                    ToObservationVector(camera.transform.forward),
                    Time.unscaledDeltaTime);
                value = _core.ArrivalProgress01;
            }
            else
            {
                CancelBeat();
                return;
            }

            if (complete)
            {
                EmitComplete(value);
                return;
            }

            if (Time.unscaledTime >= _nextWaitingLogAt)
            {
                _nextWaitingLogAt = Time.unscaledTime + WaitingLogIntervalSeconds;
                LogWaiting(value, null);
            }
        }

        private bool UpdateLook(Transform cameraTransform, out double value)
        {
            if (_rillTarget == null)
            {
                GameObject target = GameObject.Find(RillOrbName);
                if (target != null)
                {
                    _rillTarget = target.transform;
                    _missingTargetLogged = false;
                }
            }

            if (_rillTarget == null)
            {
                LogMissingOnce(ref _missingTargetLogged, "missing_target");
                value = _core.LookProgress01;
                return false;
            }

            Vector3 direction = _rillTarget.position - cameraTransform.position;
            bool complete = _core.SampleLook(
                ToObservationVector(cameraTransform.forward),
                ToObservationVector(direction),
                Time.unscaledDeltaTime);
            value = _core.LookProgress01;
            return complete;
        }

        private void EmitComplete(double value)
        {
            if (_emitted) return;
            _emitted = true;
            Debug.Log(
                "ZIPTIDE: FIRST_HOUR_OBSERVE beat=" + _activeBeatId +
                " result=complete value=" + Format(value) +
                " signal=" + _activeSignalId);
            SignalCompleted?.Invoke(_activeSignalId);
        }

        private void LogWaiting(double value, string detail)
        {
            string suffix = string.IsNullOrEmpty(detail) ? string.Empty : " detail=" + detail;
            Debug.Log(
                "ZIPTIDE: FIRST_HOUR_OBSERVE beat=" + _activeBeatId +
                " result=waiting value=" + Format(value) + suffix);
        }

        private void LogMissingOnce(ref bool latch, string detail)
        {
            if (latch) return;
            latch = true;
            LogWaiting(-1d, detail);
        }

        public static bool TryGetSignalId(string beatId, out string signalId)
        {
            if (string.Equals(beatId, LookBeatId, StringComparison.Ordinal))
            {
                signalId = LookSignalId;
                return true;
            }
            if (string.Equals(beatId, MoveBeatId, StringComparison.Ordinal))
            {
                signalId = MoveSignalId;
                return true;
            }
            if (string.Equals(beatId, ArrivalBeatId, StringComparison.Ordinal))
            {
                signalId = ArrivalSignalId;
                return true;
            }

            signalId = null;
            return false;
        }

        private static FirstHourObservationVector ToObservationVector(Vector3 value)
        {
            return new FirstHourObservationVector(value.x, value.y, value.z);
        }

        private static string Format(double value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
