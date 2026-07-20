using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        private const string BootHoldFieldName = "_bootHold";
        private const string BootSuspendedFieldName = "_bootSuspended";

        private static FieldInfo _suspendedReadersField;
        private static FieldInfo _restoreCoroutineField;
        private static FieldInfo _bootHoldField;
        private static FieldInfo _bootSuspendedField;
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
                // A second trip can begin before the previous settle tail closes. That is fresh input
                // churn, not corruption. Cancel the obsolete verifier, retain its exact reader set,
                // and let the next arrival create a fresh bounded repair window.
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
            StopCanonicalRestoreCoroutine();
            Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_RESET reason=new_travel readers="
                + (readers != null ? readers.Count : 0));
        }

        private IEnumerator RepairAfterTravel()
        {
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

            // From this point the driver owns repair, verification and restoration. Stop the older
            // wait-only verifier so it cannot re-enable an intentionally empty direct action and race
            // the repair. The same canonical reader list is retained and restored below.
            StopCanonicalRestoreCoroutine();

            string lastFailure = string.Empty;
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
                    + " direct=" + counts.DirectActions + " inertDirect=" + counts.InertDirectActions
                    + " preservedDisabled=" + counts.PreservedDisabledActions);

                yield return null;
                yield return null;

                if (ReaderActionsReadSafely(readers, out lastFailure))
                {
                    RestoreCanonicalReaders(readers);
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_OK attempt=" + attempt);
                    _repairRoutine = null;
                    yield break;
                }

                Debug.LogWarning("ZIPTIDE: INPUT_MUTATION_REPAIR_PROBE_FAIL attempt=" + attempt
                    + " detail=" + lastFailure);

                if (attempt < MaxRepairAttempts)
                    yield return new WaitForSecondsRealtime(0.5f);
            }

            Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_FAIL attempts=" + MaxRepairAttempts
                + " readers=" + readers.Count + " detail=" + lastFailure);
            _repairRoutine = null;
        }

        private void StopCanonicalRestoreCoroutine()
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig == null) return;

            Coroutine restore = GetCanonicalRestoreCoroutine();
            if (restore != null) _rig.StopCoroutine(restore);
            SetCanonicalRestoreCoroutine(null);
        }

        private void RestoreCanonicalReaders(IList<Behaviour> readers)
        {
            bool handedToBootHold = false;
            IList<Behaviour> bootSuspended = null;
            ResolveCanonicalFields();

            try
            {
                BootHoldState bootHold = _bootHoldField?.GetValue(_rig) as BootHoldState;
                handedToBootHold = bootHold != null && bootHold.Held;
                bootSuspended = _bootSuspendedField?.GetValue(_rig) as IList<Behaviour>;
            }
            catch
            {
                handedToBootHold = SceneManager.GetActiveScene().name == ZiptideConstants.SceneBoot;
            }

            int restored = 0;
            int handed = 0;
            for (int i = 0; i < readers.Count; i++)
            {
                Behaviour reader = readers[i];
                if (reader == null) continue;

                if (handedToBootHold)
                {
                    if (bootSuspended != null && !bootSuspended.Contains(reader))
                        bootSuspended.Add(reader);
                    handed++;
                }
                else
                {
                    reader.enabled = true;
                    restored++;
                }
            }

            readers.Clear();
            SetCanonicalRestoreCoroutine(null);
            Debug.Log("ZIPTIDE: INPUT_MUTATION_READERS restored=" + restored
                + " handedToBootHold=" + handed);
        }

        private IList<Behaviour> GetCanonicalSuspendedReaders()
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig == null) return null;

            ResolveCanonicalFields();
            if (_suspendedReadersField == null) return null;

            try { return _suspendedReadersField.GetValue(_rig) as IList<Behaviour>; }
            catch { return null; }
        }

        private Coroutine GetCanonicalRestoreCoroutine()
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig == null) return null;

            ResolveCanonicalFields();
            if (_restoreCoroutineField == null) return null;

            try { return _restoreCoroutineField.GetValue(_rig) as Coroutine; }
            catch { return null; }
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
            _bootHoldField = typeof(PlayerRigPersistence).GetField(BootHoldFieldName, flags);
            _bootSuspendedField = typeof(PlayerRigPersistence).GetField(BootSuspendedFieldName, flags);

            if (_suspendedReadersField == null || _restoreCoroutineField == null ||
                _bootHoldField == null || _bootSuspendedField == null)
            {
                Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_SEAM_MISSING readersField="
                    + (_suspendedReadersField != null) + " restoreField="
                    + (_restoreCoroutineField != null) + " bootHoldField="
                    + (_bootHoldField != null) + " bootSuspendedField="
                    + (_bootSuspendedField != null));
            }
        }

        private static RepairCounts RepairLocomotionBindings(IList<Behaviour> readers)
        {
            var maps = new HashSet<InputActionMap>();
            var directActions = new HashSet<InputAction>();
            AddReaderActions(readers, maps, directActions);

            int repairedMaps = 0;
            int repairedDirect = 0;
            int inertDirect = 0;
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

                // _Boot deliberately leaves left-hand snap/smooth turn as empty embedded actions because
                // ZIPTIDE's control law is left-stick move, right-stick turn. An empty direct action is an
                // inert placeholder, not a binding that can be repaired. Keep it disabled so XRI never
                // calls ReadValue on InputActionState with zero bindings/controls.
                if (IsInertDirectAction(action))
                {
                    if (action.enabled) action.Disable();
                    inertDirect++;
                    continue;
                }

                if (action.enabled) action.Disable();
                action.Enable();
                repairedDirect++;
            }

            return new RepairCounts(repairedMaps, repairedDirect, inertDirect, preservedDisabled);
        }

        private static bool IsInertDirectAction(InputAction action)
        {
            if (action == null || action.actionMap != null) return false;
            try { return action.bindings.Count == 0; }
            catch { return false; }
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

        private static bool ReaderActionsReadSafely(
            IList<Behaviour> readers,
            out string failure)
        {
            failure = string.Empty;
            if (readers == null)
            {
                failure = "readers=<null>";
                return false;
            }

            for (int i = 0; i < readers.Count; i++)
            {
                Behaviour reader = readers[i];
                if (reader == null) continue;
                string owner = "reader[" + i + "]=" + reader.GetType().Name
                    + " path=" + HierarchyPath(reader.transform);

                switch (reader)
                {
                    case ActionBasedContinuousMoveProvider move:
                        if (!ActionReadsSafely(move.leftHandMoveAction,
                                owner + ".leftHandMoveAction", out failure) ||
                            !ActionReadsSafely(move.rightHandMoveAction,
                                owner + ".rightHandMoveAction", out failure)) return false;
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        if (!ActionReadsSafely(turn.leftHandTurnAction,
                                owner + ".leftHandTurnAction", out failure) ||
                            !ActionReadsSafely(turn.rightHandTurnAction,
                                owner + ".rightHandTurnAction", out failure)) return false;
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        if (!ActionReadsSafely(snap.leftHandSnapTurnAction,
                                owner + ".leftHandSnapTurnAction", out failure) ||
                            !ActionReadsSafely(snap.rightHandSnapTurnAction,
                                owner + ".rightHandSnapTurnAction", out failure)) return false;
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
            return ActionReadsSafely(property, "unscoped", out _);
        }

        private static bool ActionReadsSafely(
            InputActionProperty property,
            string owner,
            out string failure)
        {
            failure = string.Empty;
            InputAction action = property.action;
            if (action == null) return true;

            if (IsInertDirectAction(action))
            {
                if (!action.enabled) return true;
                failure = owner + " action=" + ActionPath(action)
                    + " phase=inert_direct_enabled";
                return false;
            }

            if (!action.enabled)
            {
                if (property.reference != null) return true;
                failure = owner + " action=" + ActionPath(action)
                    + " phase=disabled_direct";
                return false;
            }

            try
            {
                action.ReadValue<Vector2>();
                return true;
            }
            catch (System.Exception ex)
            {
                int controls = 0;
                int bindings = 0;
                try { controls = action.controls.Count; } catch { controls = -1; }
                try { bindings = action.bindings.Count; } catch { bindings = -1; }
                string activeControl;
                try
                {
                    activeControl = action.activeControl != null
                        ? action.activeControl.GetType().FullName
                        : "<none>";
                }
                catch { activeControl = "<unavailable>"; }

                failure = owner + " action=" + ActionPath(action)
                    + " expected=" + (action.expectedControlType ?? string.Empty)
                    + " enabled=" + action.enabled
                    + " reference=" + (property.reference != null)
                    + " bindings=" + bindings
                    + " controls=" + controls
                    + " activeControl=" + activeControl
                    + " exception=" + ex.GetType().Name + ":" + ex.Message;
                return false;
            }
        }

        private static string ActionPath(InputAction action)
        {
            if (action == null) return "<null>";
            string map = action.actionMap != null ? action.actionMap.name : "<direct>";
            return map + "/" + action.name;
        }

        private static string HierarchyPath(Transform value)
        {
            if (value == null) return "<none>";
            string path = value.name;
            Transform parent = value.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        private readonly struct RepairCounts
        {
            public readonly int Maps;
            public readonly int DirectActions;
            public readonly int InertDirectActions;
            public readonly int PreservedDisabledActions;

            public RepairCounts(
                int maps,
                int directActions,
                int inertDirectActions,
                int preservedDisabledActions)
            {
                Maps = maps;
                DirectActions = directActions;
                InertDirectActions = inertDirectActions;
                PreservedDisabledActions = preservedDisabledActions;
            }
        }
    }
}
