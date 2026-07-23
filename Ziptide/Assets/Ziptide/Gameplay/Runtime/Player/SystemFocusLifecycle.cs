using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>The six lifecycle states of docs/runtime_lifecycle/QUEST_SYSTEM_FOCUS_CONTRACT.md.</summary>
    public enum LifecycleState
    {
        Active,
        SystemOverlay,
        HeadsetRemoved,
        TrackingLost,
        Backgrounded,
        Resuming,
    }

    /// <summary>
    /// PURE state derivation for the system-focus lifecycle (contract §3; slice S1). Deterministic:
    /// (previous state, focus, pause, trackingValid) → next state. Pause dominates focus; tracking
    /// loss only registers while otherwise Active; every recovery routes through Resuming — the
    /// checklist state later slices extend (S5) — never straight back to Active.
    /// Pinned by SystemFocusLifecycleTests.
    /// </summary>
    public static class SystemFocusStateMachine
    {
        public static LifecycleState Next(LifecycleState previous, bool focused, bool paused, bool trackingValid)
        {
            if (paused)
            {
                // OS pause (doff / app switch). Signal-identical on Quest; keep an already-derived
                // Backgrounded sticky, otherwise classify as HeadsetRemoved (contract §3).
                return previous == LifecycleState.Backgrounded
                    ? LifecycleState.Backgrounded
                    : LifecycleState.HeadsetRemoved;
            }

            if (!focused)
            {
                // Running but not focused: Universal Menu / Guardian overlay.
                return LifecycleState.SystemOverlay;
            }

            if (!trackingValid)
            {
                return LifecycleState.TrackingLost;
            }

            // Focused, unpaused, tracked: any state other than Active must pass through Resuming.
            switch (previous)
            {
                case LifecycleState.Active:
                    return LifecycleState.Active;
                case LifecycleState.Resuming:
                    return LifecycleState.Resuming; // completion is the OWNER's explicit step, not derivation
                default:
                    return LifecycleState.Resuming;
            }
        }
    }

    /// <summary>
    /// THE CANONICAL LIFECYCLE OWNER (contract §2; slice S1 of S1–S6). One persistent subscriber to
    /// the OS focus/pause signals derives one <see cref="LifecycleState"/> and broadcasts it —
    /// consumers subscribe to <see cref="StateChanged"/> instead of reading Unity callbacks
    /// themselves. S1 scope ONLY: derivation + logging + the overlay autosave. It suppresses
    /// nothing, ducks nothing, gates nothing yet (S2–S5); tracking stays reported-valid until S4
    /// wires real detection. SaveSystem's own pause-save is intentionally KEPT beneath this
    /// (contract risk R1: layered defense, both atomic/idempotent).
    /// Always-required safety class (like SaveSystem) — deliberately NOT RecoveryRuntimeGate-gated;
    /// flagged for the unified-readiness review in HANDOFF. Logs ZIPTIDE: LIFECYCLE state=a->b.
    /// </summary>
    public class SystemFocusLifecycle : MonoBehaviour
    {
        public static SystemFocusLifecycle Instance { get; private set; }

        /// <summary>(previous, next) — fired on every derived change and on Resuming→Active.</summary>
        public event System.Action<LifecycleState, LifecycleState> StateChanged;

        public LifecycleState State { get; private set; } = LifecycleState.Active;

        private bool _focused = true;
        private bool _paused;
        private const bool TrackingValid = true; // S4 replaces this constant with real detection.

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (Object.FindObjectOfType<SystemFocusLifecycle>() != null) return;
            var go = new GameObject("SystemFocusLifecycle");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<SystemFocusLifecycle>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log("ZIPTIDE: DUP_SINGLETON SystemFocusLifecycle");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnApplicationFocus(bool focused)
        {
            _focused = focused;
            Derive("focusChanged(" + focused + ")");
        }

        private void OnApplicationPause(bool paused)
        {
            _paused = paused;
            Derive("applicationPause(" + paused + ")");
        }

        private void Update()
        {
            // S1's minimal Resuming completion: one full frame after signals settle, become Active.
            // S5 replaces this with the ordered checklist (guard pass → visuals → audio → input).
            if (State == LifecycleState.Resuming && _focused && !_paused)
                Transition(LifecycleState.Active, "resume_checklist_s1");
        }

        private void Derive(string reason)
        {
            LifecycleState next = SystemFocusStateMachine.Next(State, _focused, _paused, TrackingValid);
            if (next != State) Transition(next, reason);
        }

        private void Transition(LifecycleState next, string reason)
        {
            LifecycleState previous = State;
            State = next;
            Debug.Log("ZIPTIDE: LIFECYCLE state=" + previous + "->" + next + " reason=" + reason);

            // Contract §4: the one S1 behavior — durable progress the instant an overlay opens.
            // (The OS-pause path is already covered by SaveSystem's own hook — layered defense.)
            if (next == LifecycleState.SystemOverlay)
                SaveSystem.AutosaveNow("system_overlay");

            StateChanged?.Invoke(previous, next);
        }
    }
}
