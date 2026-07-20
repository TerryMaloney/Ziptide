using System.Collections;
using System.Collections.Generic;
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
        private const int MaxSuspensionWaitFrames = 120;

        private bool _sawTravel;
        private Coroutine _repairRoutine;

        private void Awake()
        {
            _sawTravel = TravelCoordinator.IsTravelling;
        }

        private void Update()
        {
            bool travelling = TravelCoordinator.IsTravelling;
            if (travelling)
            {
                _sawTravel = true;
                return;
            }

            if (!_sawTravel) return;
            _sawTravel = false;

            if (_repairRoutine != null)
                StopCoroutine(_repairRoutine);
            _repairRoutine = StartCoroutine(RepairAfterTravel());
        }

        private IEnumerator RepairAfterTravel()
        {
            int waitFrames = 0;
            while (waitFrames < MaxSuspensionWaitFrames)
            {
                if (TravelCoordinator.IsTravelling)
                {
                    _repairRoutine = null;
                    yield break;
                }

                // Healthy travel can finish the original settle window before this observer runs.
                // In that case there is nothing to repair and active readers are expected, not an error.
                if (LocomotionActionsReadSafely())
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_SKIPPED cause=already_safe frames="
                        + waitFrames);
                    _repairRoutine = null;
                    yield break;
                }

                if (LocomotionReadersAreSuspended()) break;
                waitFrames++;
                yield return null;
            }

            if (!LocomotionReadersAreSuspended())
            {
                // Recheck at the boundary so a just-completed healthy settle does not become a false
                // failure. A genuinely unsafe active reader remains an error and stays fail-closed.
                if (LocomotionActionsReadSafely())
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_SKIPPED cause=settled_during_wait frames="
                        + waitFrames);
                    _repairRoutine = null;
                    yield break;
                }

                Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_ABORT cause=unsafe_readers_active frames="
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

                RepairCounts counts = RepairLocomotionBindings();
                Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR owner=PlayerInputSessionGuard attempt="
                    + attempt + " maps=" + counts.Maps + " direct=" + counts.DirectActions
                    + " preservedDisabled=" + counts.PreservedDisabledActions);

                yield return null;
                yield return null;

                if (LocomotionActionsReadSafely())
                {
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_OK attempt=" + attempt);
                    _repairRoutine = null;
                    yield break;
                }

                if (attempt < MaxRepairAttempts)
                    yield return new WaitForSecondsRealtime(0.5f);
            }

            Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_FAIL attempts=" + MaxRepairAttempts);
            _repairRoutine = null;
        }

        private static bool LocomotionReadersAreSuspended()
        {
            foreach (var move in Object.FindObjectsOfType<ActionBasedContinuousMoveProvider>(true))
                if (move != null && move.isActiveAndEnabled) return false;
            foreach (var turn in Object.FindObjectsOfType<ActionBasedContinuousTurnProvider>(true))
                if (turn != null && turn.isActiveAndEnabled) return false;
            foreach (var snap in Object.FindObjectsOfType<ActionBasedSnapTurnProvider>(true))
                if (snap != null && snap.isActiveAndEnabled) return false;
            return true;
        }

        private static RepairCounts RepairLocomotionBindings()
        {
            var maps = new HashSet<InputActionMap>();
            var directActions = new HashSet<InputAction>();

            foreach (var move in Object.FindObjectsOfType<ActionBasedContinuousMoveProvider>(true))
            {
                if (move == null) continue;
                AddAction(move.leftHandMoveAction, maps, directActions);
                AddAction(move.rightHandMoveAction, maps, directActions);
            }
            foreach (var turn in Object.FindObjectsOfType<ActionBasedContinuousTurnProvider>(true))
            {
                if (turn == null) continue;
                AddAction(turn.leftHandTurnAction, maps, directActions);
                AddAction(turn.rightHandTurnAction, maps, directActions);
            }
            foreach (var snap in Object.FindObjectsOfType<ActionBasedSnapTurnProvider>(true))
            {
                if (snap == null) continue;
                AddAction(snap.leftHandSnapTurnAction, maps, directActions);
                AddAction(snap.rightHandSnapTurnAction, maps, directActions);
            }

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
                if (action == null || !action.enabled) continue;
                action.Disable();
                action.Enable();
                repairedDirect++;
            }

            return new RepairCounts(repairedMaps, repairedDirect, preservedDisabled);
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

        private static bool LocomotionActionsReadSafely()
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
