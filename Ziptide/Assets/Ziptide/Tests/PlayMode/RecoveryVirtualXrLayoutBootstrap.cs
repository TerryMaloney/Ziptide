using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using InputSystemXRController = UnityEngine.InputSystem.XR.XRController;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Test-only Input System layout augmentation for headless PlayMode. The canonical XRI action
    /// asset binds locomotion through XRController controls aliased/used as Primary2DAxis and
    /// GripButton. Unity's abstract base XRController layout has neither control when instantiated
    /// directly, so the recovery simulator's otherwise-correct left/right devices cannot bind.
    ///
    /// A layout override is non-destructive: it augments only the test process' XRController layout,
    /// leaves production assets and bindings unchanged, and is installed before any test scene loads.
    /// Every newly-added virtual controller receives an explicit neutral stick state event. Before the
    /// simulator performs its required whole-asset disable/enable rebind, this bootstrap snapshots the
    /// canonical per-action enabled state. It then restores that exact state after the rebind and after
    /// device teardown. This matters because production intentionally leaves Rotate/Translate Anchor
    /// disabled; an asset-wide Enable must not silently turn those actions back on.
    ///
    /// The post-update audit independently proves all eight real Move/Turn actions resolve bilateral,
    /// state-backed controls and can be read using their declared value type. Named controls alone are
    /// not accepted as proof because that exact gap reached the continuous/snap providers.
    /// </summary>
    internal static class RecoveryVirtualXrLayoutBootstrap
    {
        private const int RequiredLocomotionActionCount = 8;
        private const int MaxBindingAuditUpdates = 8;

        private const string OverrideJson = @"
        {
            ""name"" : ""RecoveryXRControllerControls"",
            ""extend"" : ""XRController"",
            ""controls"" : [
                {
                    ""name"" : ""primary2DAxis"",
                    ""layout"" : ""Stick"",
                    ""aliases"" : [ ""Primary2DAxis"", ""Joystick"" ],
                    ""usages"" : [ ""Primary2DAxis"" ]
                },
                {
                    ""name"" : ""gripButton"",
                    ""layout"" : ""Button"",
                    ""aliases"" : [ ""GripButton"" ],
                    ""usages"" : [ ""GripButton"" ]
                }
            ]
        }";

        private readonly struct CanonicalActionState
        {
            public readonly InputAction Action;
            public readonly bool Enabled;

            public CanonicalActionState(InputAction action)
            {
                Action = action;
                Enabled = action != null && action.enabled;
            }
        }

        private static readonly List<CanonicalActionState> CanonicalActionStates =
            new List<CanonicalActionState>();
        private static readonly HashSet<int> VirtualDeviceIds = new HashSet<int>();

        private static int _capturedEnabledActions;
        private static int _bindingAuditUpdates;
        private static bool _bindingAuditComplete;
        private static bool _awaitingPostRemovalRestore;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            InputSystem.RegisterLayoutOverride(OverrideJson);
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            InputSystem.onAfterUpdate += AuditBilateralBindings;
            CanonicalActionStates.Clear();
            VirtualDeviceIds.Clear();
            _capturedEnabledActions = 0;
            _awaitingPostRemovalRestore = false;
            ResetBindingAudit();
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_LAYOUT controls=Primary2DAxis,GripButton state=explicit-neutral actionState=exact");
        }

        private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (!(device is InputSystemXRController)) return;

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                if (VirtualDeviceIds.Count == 0)
                    CaptureCanonicalActionStates();
                VirtualDeviceIds.Add(device.deviceId);
                _awaitingPostRemovalRestore = false;

                StickControl stick = device.TryGetChildControl<StickControl>("primary2DAxis");
                if (stick == null)
                {
                    Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_STATE_FAIL device=" + device.layout
                        + " reason=primary2DAxis_not_stick");
                }
                else
                {
                    // A real XR controller publishes state even while neutral. Queueing an explicit
                    // zero state prevents the Input System from presenting a named-but-uninitialized
                    // control to XRI's continuous and snap-turn providers in headless PlayMode.
                    InputSystem.QueueDeltaStateEvent(stick, Vector2.zero);
                    Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_STATE_QUEUED device=" + device.deviceId
                        + " layout=" + device.layout + " control=" + stick.path);
                }
            }
            else if (change == InputDeviceChange.Removed ||
                     change == InputDeviceChange.Disconnected)
            {
                VirtualDeviceIds.Remove(device.deviceId);
                if (VirtualDeviceIds.Count == 0 && CanonicalActionStates.Count > 0)
                    _awaitingPostRemovalRestore = true;
            }

            ResetBindingAudit();
        }

        private static void CaptureCanonicalActionStates()
        {
            CanonicalActionStates.Clear();
            _capturedEnabledActions = 0;
            var seen = new HashSet<InputAction>();
            InputActionManager[] managers =
                UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);
            for (int managerIndex = 0; managerIndex < managers.Length; managerIndex++)
            {
                InputActionManager manager = managers[managerIndex];
                if (manager == null || manager.actionAssets == null) continue;
                foreach (InputActionAsset asset in manager.actionAssets)
                {
                    if (asset == null) continue;
                    foreach (InputActionMap map in asset.actionMaps)
                    {
                        foreach (InputAction action in map.actions)
                        {
                            if (action == null || !seen.Add(action)) continue;
                            var state = new CanonicalActionState(action);
                            CanonicalActionStates.Add(state);
                            if (state.Enabled) _capturedEnabledActions++;
                        }
                    }
                }
            }

            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_ACTION_STATE_CAPTURED total="
                + CanonicalActionStates.Count
                + " enabled=" + _capturedEnabledActions
                + " disabled=" + (CanonicalActionStates.Count - _capturedEnabledActions));
        }

        private static int RestoreCanonicalActionStates()
        {
            int changed = 0;
            for (int i = 0; i < CanonicalActionStates.Count; i++)
            {
                CanonicalActionState state = CanonicalActionStates[i];
                if (state.Action == null || state.Action.enabled == state.Enabled) continue;
                if (state.Enabled) state.Action.Enable();
                else state.Action.Disable();
                changed++;
            }
            return changed;
        }

        private static bool AllCapturedActionsAreDisabled()
        {
            if (CanonicalActionStates.Count == 0 || _capturedEnabledActions == 0) return false;
            for (int i = 0; i < CanonicalActionStates.Count; i++)
            {
                InputAction action = CanonicalActionStates[i].Action;
                if (action != null && action.enabled) return false;
            }
            return true;
        }

        private static void ResetBindingAudit()
        {
            _bindingAuditUpdates = 0;
            _bindingAuditComplete = false;
        }

        private static void AuditBilateralBindings()
        {
            // The simulator deliberately disables the entire asset before rebinding or removing
            // devices. Do not fight that quiescent phase. The next asset-wide Enable is where the
            // exact per-action snapshot must be restored.
            if (CanonicalActionStates.Count > 0 && !AllCapturedActionsAreDisabled())
            {
                int restored = RestoreCanonicalActionStates();
                if (restored > 0)
                {
                    Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_ACTION_STATE_RESTORED changed="
                        + restored + " devices=" + VirtualDeviceIds.Count);
                    ResetBindingAudit();
                    return;
                }

                if (_awaitingPostRemovalRestore && VirtualDeviceIds.Count == 0)
                {
                    Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_ACTION_STATE_RELEASED total="
                        + CanonicalActionStates.Count);
                    CanonicalActionStates.Clear();
                    _capturedEnabledActions = 0;
                    _awaitingPostRemovalRestore = false;
                    return;
                }
            }

            if (_bindingAuditComplete) return;

            bool hasLeftDevice = false;
            bool hasRightDevice = false;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (!(device is InputSystemXRController) ||
                    !VirtualDeviceIds.Contains(device.deviceId)) continue;
                hasLeftDevice |= HasUsage(device, CommonUsages.LeftHand);
                hasRightDevice |= HasUsage(device, CommonUsages.RightHand);
            }
            if (!hasLeftDevice || !hasRightDevice) return;

            int locomotionActions = 0;
            int readableActions = 0;
            int leftControls = 0;
            int rightControls = 0;
            var seenActions = new HashSet<InputAction>();
            InputActionManager[] managers =
                UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);
            for (int managerIndex = 0; managerIndex < managers.Length; managerIndex++)
            {
                InputActionManager manager = managers[managerIndex];
                if (manager == null || manager.actionAssets == null) continue;
                foreach (InputActionAsset asset in manager.actionAssets)
                {
                    if (asset == null) continue;
                    foreach (InputActionMap map in asset.actionMaps)
                    {
                        foreach (InputAction action in map.actions)
                        {
                            if (action == null || !seenActions.Add(action)) continue;
                            if (action.name.IndexOf("Turn", StringComparison.OrdinalIgnoreCase) < 0 &&
                                action.name.IndexOf("Move", StringComparison.OrdinalIgnoreCase) < 0)
                                continue;

                            locomotionActions++;
                            bool hasExpectedControl = false;
                            foreach (InputControl control in action.controls)
                            {
                                if (control == null || control.device == null ||
                                    !VirtualDeviceIds.Contains(control.device.deviceId)) continue;
                                if (string.Equals(action.expectedControlType, "Button",
                                        StringComparison.OrdinalIgnoreCase))
                                    hasExpectedControl |= control is ButtonControl;
                                else
                                    hasExpectedControl |= control is Vector2Control;
                                if (HasUsage(control.device, CommonUsages.LeftHand)) leftControls++;
                                if (HasUsage(control.device, CommonUsages.RightHand)) rightControls++;
                            }

                            if (!hasExpectedControl) continue;
                            try
                            {
                                if (string.Equals(action.expectedControlType, "Button",
                                        StringComparison.OrdinalIgnoreCase))
                                    action.ReadValue<float>();
                                else
                                    action.ReadValue<Vector2>();
                                readableActions++;
                            }
                            catch (Exception ex)
                            {
                                _bindingAuditComplete = true;
                                Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_READ_FAIL action="
                                    + map.name + "/" + action.name
                                    + " expected=" + action.expectedControlType
                                    + " enabled=" + action.enabled
                                    + " controls=" + action.controls.Count
                                    + " reason=" + ex.GetType().Name + ":" + ex.Message);
                                return;
                            }
                        }
                    }
                }
            }

            if (locomotionActions < RequiredLocomotionActionCount) return;
            _bindingAuditUpdates++;

            if (leftControls > 0 && rightControls > 0 &&
                readableActions >= RequiredLocomotionActionCount)
            {
                _bindingAuditComplete = true;
                Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_OK actions=" + locomotionActions
                    + " readable=" + readableActions
                    + " leftControls=" + leftControls
                    + " rightControls=" + rightControls
                    + " updates=" + _bindingAuditUpdates);
                return;
            }

            if (_bindingAuditUpdates < MaxBindingAuditUpdates) return;
            _bindingAuditComplete = true;
            Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_FAIL actions=" + locomotionActions
                + " readable=" + readableActions
                + " leftControls=" + leftControls
                + " rightControls=" + rightControls
                + " updates=" + _bindingAuditUpdates);
        }

        private static bool HasUsage(
            InputDevice device,
            UnityEngine.InputSystem.Utilities.InternedString usage)
        {
            if (device == null) return false;
            for (int i = 0; i < device.usages.Count; i++)
                if (device.usages[i] == usage) return true;
            return false;
        }
    }
}
