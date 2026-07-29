using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FH-S08 — the first hour's conductor.
    ///
    /// It owns NOTHING. Every verb in the first hour already has an owner: the Home Hub owns the boot
    /// choice, the comfort console owns comfort, RepairableMachine owns repair stages, JobRuntime owns
    /// jobs, TravelCoordinator owns travel, CreatureRuntime owns the disable. This class listens to
    /// those owners, feeds their neutral signals into the pure <see cref="FirstHourProgressCore"/>,
    /// writes back the flags the contract says a completed beat grants, and — only when the player has
    /// visibly stalled — asks RILL for that beat's teaching line by id.
    ///
    /// THE TEACHING LAW: a hint fires only after the beat's authored hesitation interval. A player who
    /// simply does the thing hears nothing at all. The game teaches by waiting, not by narrating.
    ///
    /// Failure is always quiet and non-blocking. A missing contract asset, a missing line library or a
    /// missing owner disables orchestration and logs why; it never blocks base gameplay, never locks
    /// input, and never moves the rig.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstHourDirector : MonoBehaviour
    {
        /// <summary>Profile flag prefix marking one completed beat, so a reload resumes mid-hour.</summary>
        public const string BeatFlagPrefix = "FH_BEAT_";

        /// <summary>The contract's own name for "the tutorial is finished".</summary>
        public const string ContractDoneFlag = "TUTORIAL_DONE";

        private const float RebindDelaySeconds = 0.5f;

        public static FirstHourDirector Instance { get; private set; }

        private FirstHourContractDefinition _contract;
        private FirstHourProgressCore _core;
        private RillCompanion _rill;

        private readonly List<RepairableMachine> _machines = new List<RepairableMachine>();
        private readonly List<ZiplineRuntime> _ziplines = new List<ZiplineRuntime>();
        private readonly List<TargetRuntime> _targets = new List<TargetRuntime>();
        private JobRuntime _job;
        private bool _jobWasLive;

        private string _watchedBeatId;
        private float _beatStartedAt;
        private float _rebindAt;
        private bool _disabled;
        private bool _outboundTravelDone;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null || FindObjectOfType<FirstHourDirector>() != null) return;
            var go = new GameObject("__FirstHourDirector");
            DontDestroyOnLoad(go);
            go.AddComponent<FirstHourDirector>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _contract = Resources.Load<FirstHourContractDefinition>(
                FirstHourContractDefinition.ResourcesPath);
            if (_contract == null || _contract.beats == null || _contract.beats.Count == 0)
            {
                Disable("contract_missing");
                return;
            }

            _core = new FirstHourProgressCore(MapBeats(_contract), CompletedBeatIdsFromProfile());
            if (!_core.IsEnabled)
            {
                Disable(_core.DisabledReasonCode);
                return;
            }

            Debug.Log("ZIPTIDE: FIRST_HOUR_DIRECTOR ready beats=" + _contract.beats.Count
                + " resumedAt=" + (_core.CurrentBeatId ?? "complete"));
            Subscribe();
            OnBeatChanged();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Unsubscribe();
        }

        // ── Contract → pure core ─────────────────────────────────────────────

        private static IEnumerable<FirstHourProgressBeat> MapBeats(FirstHourContractDefinition contract)
        {
            var mapped = new List<FirstHourProgressBeat>();
            for (int i = 0; i < contract.beats.Count; i++)
            {
                FirstHourBeatDefinition beat = contract.beats[i];
                if (beat == null) continue;
                mapped.Add(new FirstHourProgressBeat(
                    beat.id,
                    beat.required,
                    beat.prerequisites,
                    beat.completionSignal != null ? beat.completionSignal.id : null,
                    beat.setsFlags,
                    beat.hesitationSeconds,
                    beat.rillLine != null ? beat.rillLine.id : null));
            }
            return mapped;
        }

        /// <summary>
        /// A returning save must resume where it stopped, not replay the tutorial. Completed beats are
        /// stored one flag each so the record survives any save-schema change that adds fields.
        /// </summary>
        private static IEnumerable<string> CompletedBeatIdsFromProfile()
        {
            var done = new List<string>();
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || profile.flags == null) return done;

            for (int i = 0; i < profile.flags.Count; i++)
            {
                string flag = profile.flags[i];
                if (!string.IsNullOrEmpty(flag) && flag.StartsWith(BeatFlagPrefix, StringComparison.Ordinal))
                    done.Add(flag.Substring(BeatFlagPrefix.Length));
            }
            return done;
        }

        private void Disable(string reason)
        {
            _disabled = true;
            Debug.Log("ZIPTIDE: FIRST_HOUR_DIRECTOR_DISABLED reason=" + reason);
        }

        // ── Owner subscriptions ──────────────────────────────────────────────

        private void Subscribe()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            HomeHubRuntime.BootPresentationReady += OnBootReady;
            HomeHubRuntime.NewGameProfileCreated += OnNewGameProfile;
            ComfortConsoleRuntime.PresetConfirmed += OnComfortConfirmed;
            FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed += OnBunkObjectGrabbed;
            FirstDestinationHelmRuntime.FirstDestinationSelected += OnFirstDestinationSelected;
            HolsterSocketInteractor.ItemHolstered += OnItemHolstered;
            TravelCoordinator.TravelCompleted += OnTravelCompleted;
            CreatureRuntime.CreatureDisabled += OnCreatureDisabled;
            WristScanner.ScanResultPublished += OnScanResult;

            if (FirstHourObservationAdapter.Instance != null)
                FirstHourObservationAdapter.Instance.SignalCompleted += Accept;
        }

        private void Unsubscribe()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            HomeHubRuntime.BootPresentationReady -= OnBootReady;
            HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfile;
            ComfortConsoleRuntime.PresetConfirmed -= OnComfortConfirmed;
            FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed -= OnBunkObjectGrabbed;
            FirstDestinationHelmRuntime.FirstDestinationSelected -= OnFirstDestinationSelected;
            HolsterSocketInteractor.ItemHolstered -= OnItemHolstered;
            TravelCoordinator.TravelCompleted -= OnTravelCompleted;
            CreatureRuntime.CreatureDisabled -= OnCreatureDisabled;
            WristScanner.ScanResultPublished -= OnScanResult;

            if (FirstHourObservationAdapter.Instance != null)
                FirstHourObservationAdapter.Instance.SignalCompleted -= Accept;

            UnbindSceneOwners();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Runtime-created machines, ziplines and targets do not exist yet on the frame a scene
            // finishes loading — JobDirector materialises them in its own Start. Rebind a moment later.
            _rebindAt = Time.unscaledTime + RebindDelaySeconds;
        }

        /// <summary>
        /// Per-instance owners cannot be reached through a static event, so they are re-found whenever
        /// a world loads. Every subscription is matched by an unbind first, so a re-entered world can
        /// never double-count a repair stage or a zipline arrival.
        /// </summary>
        private void BindSceneOwners()
        {
            UnbindSceneOwners();

            _machines.AddRange(FindObjectsOfType<RepairableMachine>(true));
            for (int i = 0; i < _machines.Count; i++)
                if (_machines[i] != null) _machines[i].StageChanged += OnRepairStageChanged;

            _ziplines.AddRange(FindObjectsOfType<ZiplineRuntime>(true));
            for (int i = 0; i < _ziplines.Count; i++)
                if (_ziplines[i] != null) _ziplines[i].RideEnded += OnZiplineRideEnded;

            _targets.AddRange(FindObjectsOfType<TargetRuntime>(true));
            for (int i = 0; i < _targets.Count; i++)
                if (_targets[i] != null && _targets[i].OnHit != null)
                    _targets[i].OnHit.AddListener(OnPracticeTargetHit);

            var director = FindObjectOfType<JobDirector>();
            _job = director != null ? director.Runtime : null;
            _jobWasLive = _job != null && _job.Definition != null;
            if (_job != null)
            {
                _job.StepChanged += OnJobStepChanged;
                _job.JobCompleted += OnJobCompleted;
            }

            Debug.Log("ZIPTIDE: FIRST_HOUR_BIND scene=" + SceneManager.GetActiveScene().name
                + " machines=" + _machines.Count + " ziplines=" + _ziplines.Count
                + " targets=" + _targets.Count + " job=" + (_job != null));
        }

        private void UnbindSceneOwners()
        {
            for (int i = 0; i < _machines.Count; i++)
                if (_machines[i] != null) _machines[i].StageChanged -= OnRepairStageChanged;
            _machines.Clear();

            for (int i = 0; i < _ziplines.Count; i++)
                if (_ziplines[i] != null) _ziplines[i].RideEnded -= OnZiplineRideEnded;
            _ziplines.Clear();

            for (int i = 0; i < _targets.Count; i++)
                if (_targets[i] != null && _targets[i].OnHit != null)
                    _targets[i].OnHit.RemoveListener(OnPracticeTargetHit);
            _targets.Clear();

            if (_job != null)
            {
                _job.StepChanged -= OnJobStepChanged;
                _job.JobCompleted -= OnJobCompleted;
                _job = null;
            }
            _jobWasLive = false;
        }

        // ── Signal translation ───────────────────────────────────────────────

        private void OnBootReady(bool canContinue) => Accept("BOOT_PRESENTATION_READY");
        private void OnNewGameProfile(PlayerProfile profile) => Accept("NEW_GAME_PROFILE_CREATED");
        private void OnComfortConfirmed(ComfortPreset preset) => Accept("COMFORT_PRESET_CONFIRMED");
        private void OnBunkObjectGrabbed(string itemId) => Accept("NAMED_BUNK_OBJECT_GRABBED");
        private void OnFirstDestinationSelected(string destination) => Accept("HELM_FIRST_DESTINATION_SELECTED");
        private void OnItemHolstered(string itemId) => Accept("ITEM_HOLSTERED");
        private void OnPracticeTargetHit() => Accept("FIRST_PRACTICE_TARGET_HIT");

        private void OnTravelCompleted(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            if (sceneName == ZiptideConstants.SceneToxicCity)
            {
                _outboundTravelDone = true;
                Accept("TRAVEL_W000_TO_W001_COMPLETE");
            }
            else if (sceneName == ZiptideConstants.SceneW000 && _outboundTravelDone)
            {
                Accept("TRAVEL_W001_TO_W000_COMPLETE");
            }
        }

        private void OnRepairStageChanged(RepairStage stage)
        {
            switch (stage)
            {
                case RepairStage.Part: Accept("FIRST_MACHINE_ACCESS_OPENED"); break;   // panel pulled
                case RepairStage.Power: Accept("FIRST_REPAIR_PART_SEATED"); break;     // part seated
                case RepairStage.Running: Accept("FIRST_MACHINE_POWERED"); break;      // power pressed
            }
        }

        private void OnZiplineRideEnded(string reason, float seconds)
        {
            // Only a completed ride counts. Letting go halfway is not learning the verb.
            if (reason == "arrived") Accept("FIRST_JOB_ZIPLINE_USED");
        }

        private void OnJobStepChanged()
        {
            bool live = _job != null && _job.Definition != null;
            if (live && !_jobWasLive) Accept("FIRST_JOB_ACCEPTED");
            _jobWasLive = live;
        }

        private void OnJobCompleted() => Accept("FIRST_JOB_REWARD_ROUTED");

        /// <summary>
        /// The scan beat completes on a REAL pulse that actually contained the faulty machine — not on
        /// waving the wrist at nothing. The scanner publishes empty pulses too, and those must not
        /// teach the player that scanning worked.
        /// </summary>
        private void OnScanResult(WristScanResult result)
        {
            if (result == null) return;
            for (int i = 0; i < result.Targets.Count; i++)
            {
                var machine = result.Targets[i].Source as RepairableMachine;
                if (machine != null && !machine.IsRepaired)
                {
                    Accept("FIRST_MACHINE_FAULT_SCANNED");
                    return;
                }
            }
        }

        /// <summary>
        /// The encounter beat belongs to THE signature creature. A swarm bug falling over on the far
        /// side of the canal is not the player resolving their first encounter, and letting it count
        /// would quietly skip the beat the whole scene was staged for.
        /// </summary>
        private void OnCreatureDisabled(CreatureRuntime creature)
        {
            if (creature == null) return;

            var orchestrator = FirstHourW001Orchestrator.Instance;
            if (orchestrator != null && orchestrator.Signature != null && orchestrator.Signature != creature)
                return;

            Accept("SIGNATURE_CREATURE_REDIRECTED_OR_DISABLED");
        }

        // ── The core loop ────────────────────────────────────────────────────

        /// <summary>
        /// Hand one owner signal to the pure core. Everything the core rejects — early, unknown,
        /// duplicate — is a deliberate no-op: an owner is always allowed to fire, and the contract
        /// decides whether it means anything yet.
        /// </summary>
        public void Accept(string signalId)
        {
            if (_disabled || _core == null) return;

            FirstHourSignalResult result = _core.AcceptSignal(signalId);
            if (!result.Advanced) return;

            PersistBeat(result.CompletedBeatId);
            GrantFlags(result.GrantedFlags);

            Debug.Log("ZIPTIDE: FIRST_HOUR_BEAT done=" + result.CompletedBeatId
                + " signal=" + signalId + " next=" + (result.CurrentBeatId ?? "complete"));

            if (result.IsComplete)
                Debug.Log("ZIPTIDE: FIRST_HOUR_COMPLETE beats=" + _core.CompletedBeatIds.Count);

            OnBeatChanged();
        }

        private void PersistBeat(string beatId)
        {
            if (string.IsNullOrEmpty(beatId)) return;
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.SetFlag(BeatFlagPrefix + beatId);
        }

        private void GrantFlags(IReadOnlyList<string> flags)
        {
            if (flags == null || flags.Count == 0) return;
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return;

            for (int i = 0; i < flags.Count; i++)
            {
                string flag = flags[i];
                if (string.IsNullOrEmpty(flag)) continue;
                profile.SetFlag(flag);

                // The contract speaks its own vocabulary; the rest of the game reads ZiptideFlags.
                // This is the ONE translation between them.
                if (flag == ContractDoneFlag) profile.SetFlag(ZiptideFlags.TUTORIAL_COMPLETE);
            }
        }

        private void OnBeatChanged()
        {
            _watchedBeatId = _core != null ? _core.CurrentBeatId : null;
            _beatStartedAt = Time.unscaledTime;

            // Hand the observation beats to their adapter — it owns look/move/arrival measurement.
            var adapter = FirstHourObservationAdapter.Instance;
            if (adapter == null || string.IsNullOrEmpty(_watchedBeatId)) return;
            if (!adapter.BeginBeat(_watchedBeatId)) adapter.CancelBeat();
        }

        private void Update()
        {
            if (_disabled || _core == null) return;

            if (_rebindAt > 0f && Time.unscaledTime >= _rebindAt)
            {
                _rebindAt = 0f;
                BindSceneOwners();
                OnBeatChanged();
            }

            // The beat can change without a signal we heard (a save restore, a blocked prerequisite
            // clearing), so keep the hesitation clock honest.
            string current = _core.CurrentBeatId;
            if (current != _watchedBeatId) OnBeatChanged();

            if (!_core.ShouldOfferHint(Time.unscaledTime - _beatStartedAt)) return;
            DeliverHint();
        }

        private void DeliverHint()
        {
            string lineId = _core.CurrentHintLineId;
            if (string.IsNullOrEmpty(lineId)) return;

            if (_rill == null) _rill = FindObjectOfType<RillCompanion>();
            if (_rill == null) return; // try again next frame; the rig may still be assembling

            if (_rill.SayById(lineId))
                Debug.Log("ZIPTIDE: FIRST_HOUR_HINT beat=" + _core.CurrentBeatId + " line=" + lineId);
            else
                Debug.Log("ZIPTIDE: FIRST_HOUR_HINT_SKIPPED beat=" + _core.CurrentBeatId
                    + " line=" + lineId + " reason=already_said_or_missing");

            // Latch either way: a line already said on this save must not re-arm every frame.
            _core.MarkHintDelivered();
        }
    }
}
