using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Core
{
    /// <summary>
    /// Ensures all XRI input action assets referenced by ActionBasedController and other XRI components
    /// are enabled at startup so grab/select work on Quest. Run after scene load.
    ///
    /// Also owns the bounded post-travel repair for the Input System cold-start defect proven during
    /// recovery: waiting cannot heal a stale InputActionState, but one disable/enable re-resolution can.
    /// The repair runs only after TravelCoordinator ends and only while locomotion readers are suspended.
    /// </summary>
    public static class RuntimeInputEnabler
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.RuntimeInputEnabler)) return;

            var assetsEnabled = new HashSet<InputActionAsset>();
            int controllersProcessed = 0;
            int otherProcessed = 0;

            var controllers = Object.FindObjectsOfType<ActionBasedController>(true);
            foreach (var c in controllers)
            {
                if (c == null) continue;
                InputActionAsset asset = GetAssetFromController(c);
                if (asset != null && assetsEnabled.Add(asset))
                {
                    asset.Enable();
                    controllersProcessed++;
                }
            }

            var allMono = Object.FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in allMono)
            {
                if (mb == null || mb is ActionBasedController) continue;
                InputActionAsset asset = GetAssetFromInputActionReferences(mb);
                if (asset != null && assetsEnabled.Add(asset))
                {
                    asset.Enable();
                    otherProcessed++;
                }
            }

            EnsureRepairDriver();

            int totalAssets = assetsEnabled.Count;
            Debug.Log($"[Ziptide] RuntimeInputEnabler: enabled {totalAssets} InputActionAsset(s). Controllers={controllersProcessed}, Other={otherProcessed}.");
        }

        private static void EnsureRepairDriver()
        {
            if (Object.FindObjectOfType<InputMutationRepairDriver>(true) != null) return;

            var rig = Object.FindObjectOfType<Ziptide.Gameplay.PlayerRigPersistence>(true);
            if (rig == null)
            {
                Debug.LogWarning("ZIPTIDE: INPUT_MUTATION_REPAIR_DRIVER no_persistent_rig");
                return;
            }

            rig.gameObject.AddComponent<InputMutationRepairDriver>();
            Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR_DRIVER ready owner=PlayerRigPersistence");
        }

        private static InputActionAsset GetAssetFromController(ActionBasedController c)
        {
            if (c == null) return null;
            var prop = c.selectAction;
            if (prop.reference != null && prop.reference.action != null)
            {
                var map = prop.reference.action.actionMap;
                if (map != null) return map.asset;
            }
            return null;
        }

        private static InputActionAsset GetAssetFromInputActionReferences(MonoBehaviour mb)
        {
            if (mb == null) return null;
            var type = mb.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                if (f.FieldType != typeof(InputActionReference)) continue;
                var refVal = f.GetValue(mb) as InputActionReference;
                if (refVal != null && refVal.action != null && refVal.action.actionMap != null)
                    return refVal.action.actionMap.asset;
            }
            return null;
        }
    }

    /// <summary>
    /// Existing RuntimeInputEnabler owner, split into a driver attached to the canonical persistent rig.
    /// This avoids introducing another persistent root while still observing every travel transition.
    /// </summary>
    internal sealed class InputMutationRepairDriver : MonoBehaviour
    {
        private const int MaxRepairAttempts = 2;
        private const int MaxSuspensionWaitFrames = 120;

        private bool _sawTravel;
        private Coroutine _repairRoutine;

        private void Awake()
        {
            _sawTravel = Ziptide.Gameplay.TravelCoordinator.IsTravelling;
        }

        private void Update()
        {
            bool travelling = Ziptide.Gameplay.TravelCoordinator.IsTravelling;
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
                if (Ziptide.Gameplay.TravelCoordinator.IsTravelling)
                {
                    _repairRoutine = null;
                    yield break;
                }

                if (LocomotionReadersAreSuspended()) break;
                waitFrames++;
                yield return null;
            }

            if (!LocomotionReadersAreSuspended())
            {
                Debug.LogError("ZIPTIDE: INPUT_MUTATION_REPAIR_ABORT cause=readers_not_suspended frames=" + waitFrames);
                _repairRoutine = null;
                yield break;
            }

            for (int attempt = 1; attempt <= MaxRepairAttempts; attempt++)
            {
                if (Ziptide.Gameplay.TravelCoordinator.IsTravelling)
                {
                    _repairRoutine = null;
                    yield break;
                }

                RepairCounts counts = RepairLocomotionBindings();
                Debug.Log("ZIPTIDE: INPUT_MUTATION_REPAIR owner=RuntimeInputEnabler attempt=" + attempt
                    + " maps=" + counts.Maps + " direct=" + counts.DirectActions
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
