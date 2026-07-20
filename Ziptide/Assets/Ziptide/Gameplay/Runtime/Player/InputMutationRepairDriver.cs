using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Bounded production repair for the Input System cold-start defect proven by the recovery route.
    /// Waiting cannot heal a stale InputActionState; while PlayerRigPersistence has the locomotion
    /// readers suspended, one disable/enable re-resolution of only their owning maps can.
    ///
    /// This component has no bootstrap of its own. PlayerInputSessionGuard installs one instance on
    /// the canonical persistent player rig, preserving the existing automatic-owner contract.
    /// </summary>
    internal sealed class InputMutationRepairDriver : MonoBehaviour
    {
        private const int MaxRepairAttempts = 2;
        private const int MaxWindowWaitFrames = 120;
        private const string SuspendedReadersFieldName = "_mutationSuspendedReaders";
        private const string RestoreCoroutineFieldName = "_mutationReaderRestore";

        private static FieldInfo _suspendedReadersField;
        private static FieldInfo _restoreCoroutineField;
        private static bool _canonicalFieldsResolved;

        private PlayerRigPersistence _rig;
        private bool _sawTravel;
        private bool _wasTravelling;
        private Coroutine _repairRoutine;

        private void Awake()
        {
            _rig = GetComponent<PlayerRigPersistence>();
            _sawTravel = TravelCoordinator.IsTravelling;
            _wasTravelling = TravelCoordinator.IsTravelling;
            ResolveCanonicalFields();
        }

        private void Update()
        {
            bool travelling = TravelCoordinator.IsTravelling;
            if (travelling)
            {
                // A second trip can begin before the previous two-frame settle tail closes. That is
                // fresh input churn, not corruption. Cancel the obsolete verifier immediately, keep
                // its exact suspended-reader set, and let EnsureXRIWiring start a fresh verifier on
                // arrival. The gate lead gives this edge detector a frame before scene activation.
                if (!_wasTravelling)
                    ResetCanonicalSettleForChainedTravel();

                _wasTravelling = true;
                _sawTravel = true;
                return;
            }

            _wasTravelling = false;
            if (!_sawTravel) return;
            _sawTravel = false;

            if (_repairRoutine != null)
                StopCoroutine(_repairRoutine);
            _repairRoutine = StartCoroutine(RepairAfterTravel());
        }

        private void ResetCanonicalSettleForChainedTravel()
        {
            if (_repairRoutine != null)
            {
                StopCoroutine(_repairRoutine);
                _repairRoutine = null;
            }

            IList<Behaviour> readers = GetCanonicalSuspendedReaders();
            Coroutine restore = GetCanonicalRestoreCoroutine();
            if (_rig == null || restore == null) return;

            _rig.StopCoroutine(restore);
            SetCanonicalRestoreCoroutine(null);
            Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_RESET reason=new_travel readers="
                + (readers != null ? readers.Count : 0));
        }

        private IEnumerator RepairAfterTravel()
        {
            // The canonical window is PlayerRigPersistence's exact owned reader set. Provider enabled
            // state is not a valid proxy: the boot hold and the travel settle owner can hand readers
            // between themselves while components appear active or inactive for unrelated reasons.
            int waitFrames = 0;
            IList<Behaviour> readers = null;
            while (waitFrames < MaxWindowWaitFrames)
            {
                if (TravelCoordinator.IsTravelling)
                {
                    _repairRoutine = null;
                    yield break;
                }

                readers = GetCanonicalSuspendedReaders();
                if (readers == null)
                {
                    Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_ABORT cause=canonical_window_unavailable");
                    _repairRoutine = null;
                    yield break;
                }

                if (readers.Count > 0) break;

                // A healthy route may close the canonical settle window before this observer runs.
                // In that case there is nothing to repair.
                if (AllLocomotionActionsReadSafely())
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_SKIPPED cause=no_window_already_safe frames="
                        + waitFrames);
                    _repairRoutine = null;
                    yield break;
                }

                waitFrames++;
                yield return null;
            }

            if (readers == null || readers.Count == 0)
            {
                if (AllLocomotionActionsReadSafely())
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_SKIPPED cause=window_closed_safe frames="
                        + waitFrames);
                    _repairRoutine = null;
                    yield break;
                }

                Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_ABORT cause=no_canonical_window frames="
                    + waitFrames);
                _repairRoutine = null;
                yield break;
            }

            for (int attempt = 1; attempt <= MaxRepairAttempts; attempt++)
            {
                if (TravelCoordinator.IsTravelling)
                {
                    _repairRoutine = null;
                    yield break;
                }

                RepairCounts counts = RepairLocomotionBindings(readers);
                Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR owner=PlayerInputSessionGuard attempt="
                    + attempt + " readers=" + readers.Count + " maps=" + counts.Maps
                    + " direct=" + counts.DirectActions
                    + " preservedDisabled=" + counts.PreservedDisabledActions);

                yield return null;
                yield return null;

                // The original settle coroutine may clear the shared list after observing this repair.
                // An empty list therefore means the canonical owner verified and closed the window.
                if (readers.Count == 0 || ReaderActionsReadSafely(readers))
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_OK attempt=" + attempt);
                    _repairRoutine = null;
                    yield break;
                }

                if (attempt < MaxRepairAttempts)
                    yield return new WaitForSecondsRealtime(0.5f);
            }

            Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_FAIL attempts=" + MaxRepairAttempts
                + " readers=" + readers.Count);
            _repairRoutine = null;
        }

        private IList<Behaviour> GetCanonicalSuspendedReaders()
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig == null) return null;

            ResolveCanonicalFields();
            if (_suspendedReadersField == null) return null;

            try
            {
                return _suspendedReadersField.GetValue(_rig) as IList<Behaviour>;
            }
            catch
            {
                return null;
            }
        }

        private Coroutine GetCanonicalRestoreCoroutine()
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig == null) return null;

            ResolveCanonicalFields();
            if (_restoreCoroutineField == null) return null;

            try
            {
                return _restoreCoroutineField.GetValue(_rig) as Coroutine;
            }
            catch
            {
                return null;
            }
        }

        private void SetCanonicalRestoreCoroutine(Coroutine value)
        {
            if (_rig == null) return;
            ResolveCanonicalFields();
            if (_restoreCoroutineField == null) return;

            try { _restoreCoroutineField.SetValue(_rig, value); }
            catch { }
        }

        private static void ResolveCanonicalFields()
        {
            if (_canonicalFieldsResolved) return;
            _canonicalFieldsResolved = true;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            _suspendedReadersField = typeof(PlayerRigPersistence).GetField(
                SuspendedReadersFieldName, flags);
            _restoreCoroutineField = typeof(PlayerRigPersistence).GetField(
                RestoreCoroutineFieldName, flags);

            if (_suspendedReadersField == null || _restoreCoroutineField == null)
                Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_SEAM_MISSING readersField="
                    + (_suspendedReadersField != null) + " restoreField="
                    + (_restoreCoroutineField != null));
        }

        private static RepairCounts RepairLocomotionBindings(IList<Behaviour> readers)
        {
            var maps = new HashSet<InputActionMap>();
            var directActions = new HashSet<InputAction>();

            AddReaderActions(readers, maps, directActions);

            int repairedMaps = 0;
            int repairedDirect = 0;
            int preservedDisabled = 0;

            foreach (InputActionMap map in maps)
            {
                if (map == null || !map.enabled) continue;

                var disabledBefore = new List<InputAction>();
                foreach (InputAction action in map.actions)
                    if (action != null && !action.enabled)
                        disabledBefore.Add(action);

                map.Disable();
                map.Enable();
                repairedMaps++;

                foreach (InputAction action in disabledBefore)
                {
                    if (action != null && action.enabled)
                    {
                        action.Disable();
                        preservedDisabled++;
                    }
                }
            }

            foreach (InputAction action in directActions)
            {
                if (action == null) continue;

                // PlayerRigPersistence's canonical probe prepares a disabled direct action by enabling
                // it while the provider is suspended, then requires a later-frame read. Mirror that
                // contract here; skipping a disabled direct action makes the repair predicate impossible.
                if (action.enabled) action.Disable();
                action.Enable();
                repairedDirect++;
            }

            return new RepairCounts(repairedMaps, repairedDirect, preservedDisabled);
        }

        private static void AddReaderActions(
            IList<Behaviour> readers,
            HashSet<InputActionMap> maps,
            HashSet<InputAction> directActions)
        {
            if (readers == null) return;
            for (int i = 0; i < readers.Count; i++)
            {
                switch (readers[i])
                {
                    case ActionBasedContinuousMoveProvider move:
                        AddAction(move.leftHandMoveAction, maps, directActions);
                        AddAction(move.rightHandMoveAction, maps, directActions);
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        AddAction(turn.leftHandTurnAction, maps, directActions);
                        AddAction(turn.rightHandTurnAction, maps, directActions);
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        AddAction(snap.leftHandSnapTurnAction, maps, directActions);
                        AddAction(snap.rightHandSnapTurnAction, maps, directActions);
                        break;
                }
            }
        }

        private static void AddAction(
            InputActionProperty property,
            HashSet<InputActionMap> maps,
            HashSet<InputAction> directActions)
        {
            InputAction action = property.action;
            if (action == null) return;
            if (action.actionMap != null) maps.Add(action.actionMap);
            else directActions.Add(action);
        }

        private static bool ReaderActionsReadSafely(IList<Behaviour> readers)
        {
            if (readers == null) return false;
            for (int i = 0; i < readers.Count; i++)
            {
                switch (readers[i])
                {
                    case ActionBasedContinuousMoveProvider move:
                        if (!ActionReadsSafely(move.leftHandMoveAction) ||
                            !ActionReadsSafely(move.rightHandMoveAction)) return false;
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        if (!ActionReadsSafely(turn.leftHandTurnAction) ||
                            !ActionReadsSafely(turn.rightHandTurnAction)) return false;
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        if (!ActionReadsSafely(snap.leftHandSnapTurnAction) ||
                            !ActionReadsSafely(snap.rightHandSnapTurnAction)) return false;
                        break;
                }
            }
            return true;
        }

        private static bool AllLocomotionActionsReadSafely()
        {
            foreach (var move in Object.FindObjectsOfType<ActionBasedContinuousMoveProvider>(true))
            {
                if (move == null) continue;
                if (!ActionReadsSafely(move.leftHandMoveAction) ||
                    !ActionReadsSafely(move.rightHandMoveAction)) return false;
            }
            foreach (var turn in Object.FindObjectsOfType<ActionBasedContinuousTurnProvider>(true))
            {
                if (turn == null) continue;
                if (!ActionReadsSafely(turn.leftHandTurnAction) ||
                    !ActionReadsSafely(turn.rightHandTurnAction)) return false;
            }
            foreach (var snap in Object.FindObjectsOfType<ActionBasedSnapTurnProvider>(true))
            {
                if (snap == null) continue;
                if (!ActionReadsSafely(snap.leftHandSnapTurnAction) ||
                    !ActionReadsSafely(snap.rightHandSnapTurnAction)) return false;
            }
            return true;
        }

        private static bool ActionReadsSafely(InputActionProperty property)
        {
            InputAction action = property.action;
            if (action == null) return true;
            if (!action.enabled) return property.reference != null;

            try
            {
                action.ReadValue<Vector2>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private readonly struct RepairCounts
        {
            public readonly int Maps;
            public readonly int DirectActions;
            public readonly int PreservedDisabledActions;

            public RepairCounts(int maps, int directActions, int preservedDisabledActions)
            {
                Maps = maps;
                DirectActions = directActions;
                PreservedDisabledActions = preservedDisabledActions;
            }
        }
    }
}
