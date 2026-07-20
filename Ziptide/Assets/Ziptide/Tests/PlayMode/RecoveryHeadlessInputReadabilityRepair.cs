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
    /// Headless-only cold-start repair for the abstract XRController layout used by Recovery PlayMode.
    ///
    /// The suite installs its left/right generic controllers before _Boot and the actual-rig simulator
    /// verifies that all eight Move/Turn actions have bilateral controls. On the first no-domain-reload
    /// route, however, Input System can still report those actions as enabled and bound while one action's
    /// internal state throws during ReadValue. Later tests inherit the naturally repaired state, hiding the
    /// cold-start ordering defect.
    ///
    /// This test-only owner reacts to the simulator-ready diagnostic, quiesces locomotion readers, probes
    /// every bound Move/Turn action using its declared value type, and resets only an action that is actually
    /// unreadable. Enabled state is preserved exactly. It runs outside the production APK and never touches
    /// anchor actions, gameplay input policy, scene assets, or the production fail-closed settle guard.
    /// </summary>
    internal static class RecoveryHeadlessInputReadabilityRepair
    {
        private const int RequiredLocomotionActions = 8;
        private const string SimulatorReadyPrefix = "ZIPTIDE: RECOVERY_TRACKED_RIG_SIM";
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
            if (_repairing || string.IsNullOrEmpty(condition)
                || !condition.StartsWith(SimulatorReadyPrefix, StringComparison.Ordinal))
                return;

            _repairing = true;
            try
            {
                RepairFreshHeadlessState();
            }
            finally
            {
                _repairing = false;
            }
        }

        private static void RepairFreshHeadlessState()
        {
            if (!HasVirtualHand(CommonUsages.LeftHand) || !HasVirtualHand(CommonUsages.RightHand))
            {
                Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL reason=missing_virtual_hands");
                return;
            }

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
                    Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL reason=missing_actions count="
                        + actions.Count + " required=" + RequiredLocomotionActions);
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
                        Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_ACTION_REPAIR action="
                            + ActionPath(action) + " expected=" + ExpectedValueType(action)
                            + " wasEnabled=" + wasEnabled + " controls=" + action.controls.Count);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL action="
                            + ActionPath(action) + " phase=reset reason="
                            + ex.GetType().Name + ":" + ex.Message);
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

                    Debug.LogError("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_FAIL action="
                        + ActionPath(action) + " phase=verify expected=" + ExpectedValueType(action)
                        + " enabled=" + action.enabled + " controls=" + action.controls.Count
                        + " reason=" + reason);
                    return;
                }

                Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_READABILITY_OK actions=" + actions.Count
                    + " readable=" + readable + " virtualControls=" + controls
                    + " repaired=" + repaired + " providersQuiesced=" + providerStates.Count);
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
