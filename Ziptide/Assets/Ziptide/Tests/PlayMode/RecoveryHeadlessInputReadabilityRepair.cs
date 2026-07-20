#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using InputSystemXRController = UnityEngine.InputSystem.XR.XRController;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Headless-only action-state repair for the abstract XRController layout used by Recovery PlayMode.
    ///
    /// The suite installs its left/right generic controllers before _Boot and the actual-rig simulator
    /// verifies that all eight Move/Turn actions have bilateral controls. Linux batchmode can nevertheless
    /// leave one of those already-bound actions unreadable after PlayerRigPersistence mutates/re-resolves
    /// the canonical assets at a scene boundary. Real Quest controller layouts settle through the runtime
    /// Input System; the abstract headless layout sometimes does not.
    ///
    /// This test-only owner runs at the two proven boundaries: when the tracked-rig simulator comes online
    /// and immediately after each `XRI_WIRING` mutation completes. It quiesces locomotion readers, probes
    /// every bound Move/Turn action using its declared value type, resets only an action that actually throws,
    /// and preserves enabled state exactly. The production settle coroutine remains fail-closed and is still
    /// solely responsible for restoring providers. This owner is absent from the headset APK.
    /// </summary>
    internal static class RecoveryHeadlessInputReadabilityRepair
    {
        private const int RequiredLocomotionActions = 8;
        private const string SimulatorReadyPrefix = "ZIPTIDE: RECOVERY_TRACKED_RIG_SIM";
        private const string InputMutationCompletePrefix = "ZIPTIDE: XRI_WIRING";
        private static bool _repairing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Application.logMessageReceived -= OnLogMessage;
            Application.logMessageReceived += OnLogMessage;
            _repairing = false;
            Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_ARMED");
        }

        private static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (_repairing || string.IsNullOrEmpty(condition)) return;

            bool simulatorReady = condition.StartsWith(SimulatorReadyPrefix, StringComparison.Ordinal);
            bool mutationComplete = condition.StartsWith(InputMutationCompletePrefix, StringComparison.Ordinal);
            if (!simulatorReady && !mutationComplete) return;

            // XRI_WIRING is emitted by production code in many PlayMode contexts. Only intervene when
            // this Recovery suite's abstract left/right XR pair is actually installed.
            if (!HasVirtualHand(CommonUsages.LeftHand) || !HasVirtualHand(CommonUsages.RightHand))
            {
                if (simulatorReady)
                    Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL reason=missing_virtual_hands");
                return;
            }

            _repairing = true;
            try
            {
                RepairFreshHeadlessState(simulatorReady ? "simulator_ready" : "xri_wiring");
            }
            finally
            {
                _repairing = false;
            }
        }

        private static void RepairFreshHeadlessState(string boundary)
        {
            var providerStates = new List<ProviderState>();
            foreach (LocomotionProvider provider in UnityEngine.Object.FindObjectsOfType<LocomotionProvider>(true))
            {
                if (provider == null) continue;
                providerStates.Add(new ProviderState(provider));
                provider.enabled = false;
            }

            try
            {
                var actions = CollectLocomotionActions();
                if (actions.Count < RequiredLocomotionActions)
                {
                    // During an early _Boot wiring pass the canonical action manager may not yet own the
                    // complete asset. The simulator-ready boundary is mandatory; an earlier mutation pass
                    // is only an opportunity to settle state.
                    if (boundary == "simulator_ready")
                    {
                        Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL boundary="
                            + boundary + " reason=missing_actions count=" + actions.Count
                            + " required=" + RequiredLocomotionActions);
                    }
                    else
                    {
                        Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_DEFER boundary="
                            + boundary + " actions=" + actions.Count
                            + " required=" + RequiredLocomotionActions);
                    }
                    return;
                }

                int repaired = 0;
                for (int i = 0; i < actions.Count; i++)
                {
                    InputAction action = actions[i];
                    if (ReadsExpectedValue(action, out _)) continue;

                    bool wasEnabled = action.enabled;
                    try
                    {
                        if (wasEnabled) action.Disable();
                        action.Enable();
                        if (!wasEnabled) action.Disable();
                        repaired++;
                        Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_ACTION_REPAIR boundary="
                            + boundary + " action=" + ActionPath(action)
                            + " expected=" + ExpectedValueType(action)
                            + " wasEnabled=" + wasEnabled
                            + " controls=" + action.controls.Count);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL boundary="
                            + boundary + " action=" + ActionPath(action)
                            + " phase=reset reason=" + ex.GetType().Name + ":" + ex.Message);
                        return;
                    }
                }

                InputSystem.Update();

                int readable = 0;
                int controls = 0;
                for (int i = 0; i < actions.Count; i++)
                {
                    InputAction action = actions[i];
                    controls += CountVirtualControls(action);
                    if (ReadsExpectedValue(action, out string reason))
                    {
                        readable++;
                        continue;
                    }

                    Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL boundary="
                        + boundary + " action=" + ActionPath(action)
                        + " phase=verify expected=" + ExpectedValueType(action)
                        + " enabled=" + action.enabled + " controls=" + action.controls.Count
                        + " reason=" + reason);
                    return;
                }

                Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_OK boundary=" + boundary
                    + " actions=" + actions.Count + " readable=" + readable
                    + " virtualControls=" + controls + " repaired=" + repaired
                    + " providersQuiesced=" + providerStates.Count);
            }
            finally
            {
                for (int i = providerStates.Count - 1; i >= 0; i--)
                {
                    ProviderState state = providerStates[i];
                    if (state.Provider != null) state.Provider.enabled = state.Enabled;
                }
            }
        }

        private static List<InputAction> CollectLocomotionActions()
        {
            var actions = new List<InputAction>();
            var seen = new HashSet<InputAction>();
            foreach (InputActionManager manager in UnityEngine.Object.FindObjectsOfType<InputActionManager>(true))
            {
                if (manager == null || manager.actionAssets == null) continue;
                foreach (InputActionAsset asset in manager.actionAssets)
                {
                    if (asset == null) continue;
                    foreach (InputActionMap map in asset.actionMaps)
                    {
                        foreach (InputAction action in map.actions)
                        {
                            if (action == null || !seen.Add(action) || !IsLocomotionAction(action)) continue;
                            if (CountVirtualControls(action) <= 0) continue;
                            actions.Add(action);
                        }
                    }
                }
            }
            actions.Sort((a, b) => string.Compare(ActionPath(a), ActionPath(b), StringComparison.Ordinal));
            return actions;
        }

        private static bool IsLocomotionAction(InputAction action)
        {
            string name = action.name ?? string.Empty;
            return name.IndexOf("Move", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Turn", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool ReadsExpectedValue(InputAction action, out string reason)
        {
            try
            {
                if (string.Equals(ExpectedValueType(action), "Button", StringComparison.OrdinalIgnoreCase))
                    action.ReadValue<float>();
                else
                    action.ReadValue<Vector2>();
                reason = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                reason = ex.GetType().Name + ":" + ex.Message;
                return false;
            }
        }

        private static string ExpectedValueType(InputAction action)
            => string.IsNullOrEmpty(action.expectedControlType) ? "Vector2" : action.expectedControlType;

        private static int CountVirtualControls(InputAction action)
        {
            int count = 0;
            foreach (InputControl control in action.controls)
            {
                if (control == null || !(control.device is InputSystemXRController)) continue;
                count++;
            }
            return count;
        }

        private static bool HasVirtualHand(UnityEngine.InputSystem.Utilities.InternedString usage)
        {
            foreach (InputDevice device in InputSystem.devices)
            {
                if (!(device is InputSystemXRController)) continue;
                for (int i = 0; i < device.usages.Count; i++)
                    if (device.usages[i] == usage) return true;
            }
            return false;
        }

        private static string ActionPath(InputAction action)
        {
            string map = action.actionMap != null ? action.actionMap.name : "<direct>";
            return map + "/" + action.name;
        }

        private readonly struct ProviderState
        {
            public readonly LocomotionProvider Provider;
            public readonly bool Enabled;

            public ProviderState(LocomotionProvider provider)
            {
                Provider = provider;
                Enabled = provider != null && provider.enabled;
            }
        }
    }
}
#endif
