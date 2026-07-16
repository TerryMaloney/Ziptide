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
    /// This owner is deliberately observation-only after device creation: it registers the test layout,
    /// queues neutral state and audits that all eight real Move/Turn actions resolve bilateral readable
    /// controls. It never enables or disables a production action. Exact action-state preservation belongs
    /// to RecoveryActualRigControllerSimulation's explicit rebind/teardown windows; a global after-update
    /// restorer previously fought PlayerRigPersistence.DisableAnchorInputActions during travel and caused
    /// the R1.10 SnapTurn processor null-reference in run 29504515247.
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

        private static readonly HashSet<int> VirtualDeviceIds = new HashSet<int>();
        private static int _bindingAuditUpdates;
        private static bool _bindingAuditComplete;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            InputSystem.RegisterLayoutOverride(OverrideJson);
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            InputSystem.onAfterUpdate += AuditBilateralBindings;
            VirtualDeviceIds.Clear();
            ResetBindingAudit();
            Debug.Log(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_LAYOUT controls=Primary2DAxis,GripButton " +
                "state=explicit-neutral actionState=observer-only");
        }

        private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (!(device is InputSystemXRController)) return;

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                VirtualDeviceIds.Add(device.deviceId);
                StickControl stick = device.TryGetChildControl<StickControl>("primary2DAxis");
                if (stick == null)
                {
                    Debug.LogError(
                        "ZIPTIDE: RECOVERY_VIRTUAL_XR_STATE_FAIL device=" + device.layout +
                        " reason=primary2DAxis_not_stick");
                }
                else
                {
                    InputSystem.QueueDeltaStateEvent(stick, Vector2.zero);
                    Debug.Log(
                        "ZIPTIDE: RECOVERY_VIRTUAL_XR_STATE_QUEUED device=" + device.deviceId +
                        " layout=" + device.layout + " control=" + stick.path);
                }
            }
            else if (change == InputDeviceChange.Removed ||
                     change == InputDeviceChange.Disconnected)
            {
                VirtualDeviceIds.Remove(device.deviceId);
            }

            ResetBindingAudit();
        }

        private static void ResetBindingAudit()
        {
            _bindingAuditUpdates = 0;
            _bindingAuditComplete = false;
        }

        private static void AuditBilateralBindings()
        {
            if (_bindingAuditComplete) return;

            bool hasLeftDevice = false;
            bool hasRightDevice = false;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (!(device is InputSystemXRController) ||
                    !VirtualDeviceIds.Contains(device.deviceId))
                    continue;
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
                                    !VirtualDeviceIds.Contains(control.device.deviceId))
                                    continue;
                                if (string.Equals(
                                        action.expectedControlType,
                                        "Button",
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
                                if (string.Equals(
                                        action.expectedControlType,
                                        "Button",
                                        StringComparison.OrdinalIgnoreCase))
                                    action.ReadValue<float>();
                                else
                                    action.ReadValue<Vector2>();
                                readableActions++;
                            }
                            catch (Exception ex)
                            {
                                _bindingAuditComplete = true;
                                Debug.LogError(
                                    "ZIPTIDE: RECOVERY_VIRTUAL_XR_READ_FAIL action=" +
                                    map.name + "/" + action.name +
                                    " expected=" + action.expectedControlType +
                                    " enabled=" + action.enabled +
                                    " controls=" + action.controls.Count +
                                    " reason=" + ex.GetType().Name + ":" + ex.Message);
                                return;
                            }
                        }
                    }
                }
            }

            if (locomotionActions < RequiredLocomotionActionCount) return;
            _bindingAuditUpdates++;

            if (leftControls > 0 &&
                rightControls > 0 &&
                readableActions >= RequiredLocomotionActionCount)
            {
                _bindingAuditComplete = true;
                Debug.Log(
                    "ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_OK actions=" + locomotionActions +
                    " readable=" + readableActions +
                    " leftControls=" + leftControls +
                    " rightControls=" + rightControls +
                    " updates=" + _bindingAuditUpdates);
                return;
            }

            if (_bindingAuditUpdates < MaxBindingAuditUpdates) return;
            _bindingAuditComplete = true;
            Debug.LogError(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_FAIL actions=" + locomotionActions +
                " readable=" + readableActions +
                " leftControls=" + leftControls +
                " rightControls=" + rightControls +
                " updates=" + _bindingAuditUpdates);
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
