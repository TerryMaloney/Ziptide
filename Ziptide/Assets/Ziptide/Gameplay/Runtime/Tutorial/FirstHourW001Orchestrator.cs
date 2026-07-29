using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FH-S08's other half — the two first-hour beats that no existing owner can raise on its own.
    ///
    /// 1. SIGNATURE_CREATURE_OBSERVED. The contract says the player gets a SAFE READ WINDOW before the
    ///    encounter turns into pressure: they must actually look at the creature, from outside its
    ///    personal space, for long enough to notice its tell. No owner measures "you looked at that
    ///    thing and understood it", so this does — read-only, from the real tracked head pose.
    ///
    /// 2. FIRST_HOUR_PAYOFF_OBSERVED. The tutorial's last beat is the changed ship. It completes ONLY
    ///    when the visible decal is actually on the hull — not when the flag is set. If the flag is
    ///    there and the plate is not, saved progress is preserved and the beat stays open, loudly.
    ///    A payoff you cannot see is not a payoff, and auto-completing it would hide a real bug.
    ///
    /// It owns no creature state, no ship state and no progression state; it observes and reports.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstHourW001Orchestrator : MonoBehaviour
    {
        /// <summary>The species the first hour stages its encounter around.</summary>
        public const string SignatureCreatureId = "husk_molter";

        /// <summary>The decal W001's completed contract earns (ShipLoadoutCore milestone table).</summary>
        public const string FirstContractDecal = "decal_first_contract";

        // Personal space: reading a creature means watching it from OUTSIDE its reach. Closing to
        // touch is not observation, and rewarding it would teach exactly the wrong instinct.
        public const float MinimumSafeDistance = 3f;
        private const float MaximumUsefulDistance = 28f;
        private const float RequiredViewDot = 0.82f;      // roughly the centre third of the view
        private const float RequiredDwellSeconds = 2.5f;
        private const float ScanIntervalSeconds = 0.25f;

        public static FirstHourW001Orchestrator Instance { get; private set; }

        private CreatureRuntime _signature;
        private float _dwell;
        private float _nextScanAt;
        private bool _observedPublished;
        private bool _payoffPublished;
        private bool _payoffGapLogged;
        private bool _speciesFallbackLogged;

        /// <summary>The instance the first hour is staged around; null until one is found.</summary>
        public CreatureRuntime Signature => _signature;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null || FindObjectOfType<FirstHourW001Orchestrator>() != null) return;
            var go = new GameObject("__FirstHourW001Orchestrator");
            DontDestroyOnLoad(go);
            go.AddComponent<FirstHourW001Orchestrator>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Creature instances belong to their world; a new world means a new search.
            _signature = null;
            _dwell = 0f;
            _observedPublished = false;
            _payoffGapLogged = false;
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextScanAt) return;
            _nextScanAt = Time.unscaledTime + ScanIntervalSeconds;

            string scene = SceneManager.GetActiveScene().name;
            if (scene == ZiptideConstants.SceneToxicCity) TickObservation();
            else if (scene == ZiptideConstants.SceneW000) TickPayoff();
        }

        // ── 1. The safe read window ──────────────────────────────────────────

        private void TickObservation()
        {
            if (_observedPublished) return;
            if (_signature == null || _signature.IsDisabled) _signature = FindSignature();
            if (_signature == null) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 toCreature = _signature.transform.position - cam.transform.position;
            float distance = toCreature.magnitude;
            if (distance < MinimumSafeDistance || distance > MaximumUsefulDistance)
            {
                _dwell = 0f;
                return;
            }

            float facing = Vector3.Dot(cam.transform.forward, toCreature / Mathf.Max(distance, 0.0001f));
            if (facing < RequiredViewDot)
            {
                _dwell = 0f;
                return;
            }

            _dwell += ScanIntervalSeconds;
            if (_dwell < RequiredDwellSeconds) return;

            _observedPublished = true;
            Debug.Log("ZIPTIDE: FIRST_HOUR_CREATURE id=" + _signature.creatureId
                + " state=observed distance=" + distance.ToString("F1"));
            FirstHourDirector.Instance?.Accept("SIGNATURE_CREATURE_OBSERVED");
        }

        private CreatureRuntime FindSignature()
        {
            CreatureRuntime[] all = FindObjectsOfType<CreatureRuntime>();
            CreatureRuntime fallback = null;
            for (int i = 0; i < all.Length; i++)
            {
                CreatureRuntime candidate = all[i];
                if (candidate == null || candidate.IsDisabled) continue;
                if (candidate.creatureId == SignatureCreatureId) return candidate;
                if (fallback == null) fallback = candidate;
            }

            // A world that has not yet had its signature species authored still deserves a working
            // encounter beat rather than a dead tutorial. Say so, once, and use what is there.
            if (fallback != null && !_speciesFallbackLogged)
            {
                _speciesFallbackLogged = true;
                Debug.Log("ZIPTIDE: FIRST_HOUR_CREATURE_FALLBACK wanted=" + SignatureCreatureId
                    + " using=" + fallback.creatureId);
            }
            return fallback;
        }

        // ── 2. The changed ship ──────────────────────────────────────────────

        private void TickPayoff()
        {
            if (_payoffPublished) return;

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || !profile.HasFlag(ZiptideFlags.W001_COMPLETE)) return;

            bool visible = FindDecalPlate() != null;
            if (!visible)
            {
                if (_payoffGapLogged) return;
                _payoffGapLogged = true;
                Debug.LogWarning("ZIPTIDE: FIRST_HOUR_PAYOFF decal=" + FirstContractDecal
                    + " present=False — progress preserved, beat left OPEN."
                    + " The contract is complete but the ship does not show it.");
                return;
            }

            _payoffPublished = true;
            Debug.Log("ZIPTIDE: FIRST_HOUR_PAYOFF decal=" + FirstContractDecal + " present=True");
            FirstHourDirector.Instance?.Accept("FIRST_HOUR_PAYOFF_OBSERVED");
        }

        /// <summary>
        /// The decal is a real quad named after the milestone, parented under the hull's
        /// "JourneyDecals" rail by ShipRefit. Looking for the object — not the flag that earned it —
        /// is the whole point of this check.
        /// </summary>
        private static Transform FindDecalPlate()
        {
            ShipBoardingStation[] ships = FindObjectsOfType<ShipBoardingStation>();
            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i] == null) continue;
                Transform rail = ships[i].transform.Find("JourneyDecals");
                Transform plate = rail != null ? rail.Find(FirstContractDecal) : null;
                if (plate != null) return plate;
            }
            return null;
        }
    }
}
