using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Runtime consequence owner for an authored toxic river basin. The editor builder owns geometry;
    /// this component owns exposure only: enter/exit evidence, bounded damage pulses, and lethal recovery
    /// through WorldRuntime after continuous immersion. It never moves the player while merely wading.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ToxicRiverRuntime : MonoBehaviour
    {
        [SerializeField] private string riverId = "toxic_river";
        [SerializeField] private Vector3 localBoundsCenter = new Vector3(0f, -1f, 0f);
        [SerializeField] private Vector3 boundsSize = new Vector3(8f, 2f, 12f);
        [SerializeField] private float pulseIntervalSeconds = ToxicExposureCore.DefaultPulseInterval;
        [SerializeField] private float lethalAfterSeconds = ToxicExposureCore.DefaultLethalSeconds;

        private ToxicExposureCore _exposure;
        private PlayerRigPersistence _rig;
        private PlayerStunReceiver _stun;
        private WorldRuntime _world;
        private ToxicRiverSurfaceRuntime _surface;
        private Bounds _worldBounds;
        private int _pulseCount;

        public string RiverId => riverId;
        public Vector3 BoundsSize => boundsSize;
        public float LethalAfterSeconds => lethalAfterSeconds;

        public void Configure(string id, Vector3 center, Vector3 size,
            float pulseInterval = ToxicExposureCore.DefaultPulseInterval,
            float lethalSeconds = ToxicExposureCore.DefaultLethalSeconds)
        {
            riverId = string.IsNullOrEmpty(id) ? "toxic_river" : id;
            localBoundsCenter = center;
            boundsSize = new Vector3(
                Mathf.Max(0.5f, Mathf.Abs(size.x)),
                Mathf.Max(0.5f, Mathf.Abs(size.y)),
                Mathf.Max(0.5f, Mathf.Abs(size.z)));
            pulseIntervalSeconds = Mathf.Max(0.15f, pulseInterval);
            lethalAfterSeconds = Mathf.Max(pulseIntervalSeconds, lethalSeconds);
            RebuildState();
        }

        private void Awake()
        {
            RebuildState();
            _surface = GetComponentInChildren<ToxicRiverSurfaceRuntime>(true);
            Debug.Log("ZIPTIDE: TOXIC_RIVER_READY id=" + riverId
                + " size=" + boundsSize.ToString("F1")
                + " lethal=" + lethalAfterSeconds.ToString("F1"));
        }

        private void RebuildState()
        {
            _exposure = new ToxicExposureCore(pulseIntervalSeconds, lethalAfterSeconds);
            _worldBounds = new Bounds(transform.TransformPoint(localBoundsCenter), boundsSize);
            _pulseCount = 0;
        }

        private void Update()
        {
            if (_exposure == null) RebuildState();
            ResolveOwners();
            if (_rig == null) return;

            Vector3 sample = _rig.transform.position + Vector3.up * 0.25f;
            bool inside = _worldBounds.Contains(sample);
            ToxicExposureStep step = _exposure.Tick(inside, Time.deltaTime);

            if (step.Entered)
                Debug.Log("ZIPTIDE: TOXIC_RIVER_ENTER id=" + riverId);
            if (step.Exited)
                Debug.Log("ZIPTIDE: TOXIC_RIVER_EXIT id=" + riverId);

            if (step.DamagePulse)
            {
                _pulseCount++;
                _stun?.ApplyStun(0.32f, 0.58f);
                _surface?.PulseDamage();
                Debug.Log("ZIPTIDE: TOXIC_RIVER_DAMAGE id=" + riverId
                    + " pulse=" + _pulseCount
                    + " exposure=" + step.ExposureSeconds.ToString("F2"));
            }

            if (!step.Lethal) return;

            Debug.Log("ZIPTIDE: TOXIC_RIVER_LETHAL id=" + riverId
                + " exposure=" + step.ExposureSeconds.ToString("F2"));
            if (_world != null)
                _world.RespawnPlayer(_rig.transform);
            _exposure.Reset();
        }

        private void ResolveOwners()
        {
            if (_rig == null) _rig = FindObjectOfType<PlayerRigPersistence>();
            if (_stun == null) _stun = FindObjectOfType<PlayerStunReceiver>();
            if (_world == null) _world = FindObjectOfType<WorldRuntime>();
        }

        private void OnDisable()
        {
            _exposure?.Reset();
        }
    }
}
