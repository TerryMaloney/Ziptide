using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Scene-level owner of JobRuntime. References WorldPackDefinition, updates ObjectiveBoard, creates spawn markers, and wires DispatchKiosk and delivery/target callbacks.
    /// </summary>
    public class JobDirector : MonoBehaviour
    {
        [Tooltip("World pack for this scene (jobs + spawn markers).")]
        [SerializeField] private WorldPackDefinition worldPack;

        [Tooltip("Optional: assign or will be found by name.")]
        [SerializeField] private ObjectiveBoard objectiveBoard;

        [Tooltip("Optional: assign or will be found by name.")]
        [SerializeField] private DispatchKiosk dispatchKiosk;

        private JobRuntime _runtime = new JobRuntime();
        private readonly List<Transform> _markerTransforms = new List<Transform>();
        private Transform _playerTransform;
        private float _goToCheckTimer;

        public JobRuntime Runtime => _runtime;
        public WorldPackDefinition WorldPack => worldPack;

        private void Start()
        {
            if (worldPack == null) return;

            // Non-blocking story-gate diagnostic: if the player reached this world without its required
            // flags, log it (don't yank them mid-scene — enforcement belongs at the travel/offer UI, which
            // touches the locked travel contract). Helps catch out-of-order travel during testing.
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string missing = WorldGating.FirstMissingRequirement(worldPack, profile);
            if (missing != null)
                Debug.Log("ZIPTIDE: WORLD_LOCKED world=" + worldPack.packId + " missingFlag=" + missing);

            _playerTransform = GetPlayerTransform();
            CreateSpawnMarkers();
            EnsureBoardAndKiosk();
            _runtime.StepChanged += OnStepChanged;
            _runtime.JobCompleted += OnJobCompleted;
            DroneRuntime.OnDroneDisabled += OnDroneDisabled;
            OnStepChanged();
        }

        private void OnDestroy()
        {
            _runtime.StepChanged -= OnStepChanged;
            _runtime.JobCompleted -= OnJobCompleted;
            DroneRuntime.OnDroneDisabled -= OnDroneDisabled;
        }

        private void Update()
        {
            if (worldPack == null || _playerTransform == null) return;
            _goToCheckTimer -= Time.deltaTime;
            if (_goToCheckTimer <= 0f)
            {
                _goToCheckTimer = 0.3f;
                CheckGoToMarker();
            }
        }

        /// <summary>
        /// Called by DispatchKiosk when player accepts a job. Starts the given job by index.
        /// </summary>
        public void StartJobByIndex(int index)
        {
            if (worldPack == null || worldPack.jobs == null || index < 0 || index >= worldPack.jobs.Count) return;
            _runtime.StartJob(worldPack.jobs[index]);
        }

        /// <summary>
        /// Called by DeliveryCradleSocketInteractor when an item is placed.
        /// </summary>
        public void ReportDeliver(string socketId, string itemId)
        {
            _runtime.ReportDeliver(socketId, itemId);
        }

        /// <summary>
        /// Called by JobTarget when a target is hit.
        /// </summary>
        public void ReportTargetHit()
        {
            _runtime.ReportTargetHit();
        }

        /// <summary>
        /// Called when player collects an item (e.g. Collectible with ItemRuntime grabbed). For CollectItemIdCountStep.
        /// </summary>
        public void ReportCollect(string itemId)
        {
            _runtime.ReportCollect(itemId);
        }

        private void OnDroneDisabled(DroneRuntime drone)
        {
            _runtime.ReportDroneDisabled();
        }

        private void CreateSpawnMarkers()
        {
            if (worldPack.spawnMarkers == null) return;
            var root = new GameObject("SpawnMarkers");
            root.transform.SetParent(transform);
            foreach (var m in worldPack.spawnMarkers)
            {
                var go = new GameObject("Marker_" + m.markerId);
                go.transform.SetParent(root.transform);
                go.transform.localPosition = m.localPosition;
                go.transform.localEulerAngles = m.localEulerAngles;
                _markerTransforms.Add(go.transform);
            }
        }

        private void CheckGoToMarker()
        {
            var step = _runtime.GetCurrentStep() as GoToMarkerStepDefinition;
            if (step == null) return;

            Transform marker = null;
            foreach (var t in _markerTransforms)
            {
                if (t.name == "Marker_" + step.markerId) { marker = t; break; }
            }
            if (marker == null) return;

            float dist = Vector3.Distance(_playerTransform.position, marker.position);
            if (dist <= step.arriveDistance)
                _runtime.ReportGoToArrived(step.markerId);
        }

        private void EnsureBoardAndKiosk()
        {
            if (objectiveBoard == null)
                objectiveBoard = FindObjectOfType<ObjectiveBoard>();
            if (objectiveBoard != null)
                objectiveBoard.Bind(this);

            if (dispatchKiosk == null)
                dispatchKiosk = FindObjectOfType<DispatchKiosk>();
            if (dispatchKiosk != null)
                dispatchKiosk.Bind(this);
        }

        private void OnStepChanged()
        {
            if (objectiveBoard != null)
                objectiveBoard.RefreshText();
        }

        private void OnJobCompleted()
        {
            if (objectiveBoard != null)
                objectiveBoard.RefreshText();

            // Pay the job's reward + set its completion flag into the live profile. Uses Architect's
            // JobRewards.Grant + the self-bootstrapping SaveSystem (so no _Boot edit needed). This is the
            // one runtime call that makes the Toxic City bounty actually pay.
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && _runtime.Definition != null)
            {
                JobRewards.Grant(_runtime.Definition, profile);
                Debug.Log("ZIPTIDE: JOB_REWARD_GRANTED job=" + _runtime.Definition.jobId);

                // World-level story flags (RILL beat + Signal threshold + W###_COMPLETE) that a single
                // job completionFlag can't all carry. Granted when the world's contract finishes.
                int worldFlags = WorldGating.GrantWorldFlags(worldPack, profile);
                if (worldFlags > 0)
                    Debug.Log("ZIPTIDE: WORLD_FLAGS_GRANTED world=" +
                              (worldPack != null ? worldPack.packId : "?") + " count=" + worldFlags);
            }
        }

        private Transform GetPlayerTransform()
        {
            var xr = GameObject.Find("XR Origin");
            if (xr != null) return xr.transform;
            var cam = Camera.main;
            if (cam != null) return cam.transform;
            return transform;
        }
    }
}
