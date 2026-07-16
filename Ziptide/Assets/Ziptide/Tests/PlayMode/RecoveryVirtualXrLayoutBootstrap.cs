using System;
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
    /// Every newly-added virtual controller receives an explicit neutral stick state event. The
    /// post-update audit independently proves the real locomotion actions resolve state-backed controls
    /// on both hands and that every Vector2 locomotion action can actually be read; named controls alone
    /// are not accepted as proof because that exact gap reached the continuous/snap providers.
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

        private static int _bindingAuditUpdates;
        private static bool _bindingAuditComplete;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            InputSystem.RegisterLayoutOverride(OverrideJson);
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onDeviceChange += OnDeviceChange;
            ResetBindingAudit();
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_LAYOUT controls=Primary2DAxis,GripButton state=explicit-neutral");
        }

        private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (!(device is InputSystemXRController)) return;

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
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

            ResetBindingAudit();
        }

        private static void ResetBindingAudit()
        {
            _bindingAuditUpdates = 0;
            _bindingAuditComplete = false;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            InputSystem.onAfterUpdate += AuditBilateralBindings;
        }

        private static void AuditBilateralBindings()
        {
            if (_bindingAuditComplete) return;

            bool hasLeftDevice = false;
            bool hasRightDevice = false;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (!(device is InputSystemXRController)) continue;
                hasLeftDevice |= HasUsage(device, CommonUsages.LeftHand);
                hasRightDevice |= HasUsage(device, CommonUsages.RightHand);
            }
            if (!hasLeftDevice || !hasRightDevice) return;

            int locomotionActions = 0;
            int leftControls = 0;
            int rightControls = 0;
            int readableVector2Actions = 0;
            InputActionManager[] managers = UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);
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
                            if (action.name.IndexOf("Turn", StringComparison.OrdinalIgnoreCase) < 0 &&
                                action.name.IndexOf("Move", StringComparison.OrdinalIgnoreCase) < 0)
                                continue;

                            locomotionActions++;
                            bool hasVector2Control = false;
                            foreach (InputControl control in action.controls)
                            {
                                if (control == null || control.device == null) continue;
                                hasVector2Control |= control is Vector2Control;
                                if (HasUsage(control.device, CommonUsages.LeftHand)) leftControls++;
                                if (HasUsage(control.device, CommonUsages.RightHand)) rightControls++;
                            }

                            if (!hasVector2Control) continue;
                            try
                            {
                                action.ReadValue<Vector2>();
                                readableVector2Actions++;
                            }
                            catch (Exception ex)
                            {
                                _bindingAuditComplete = true;
                                InputSystem.onAfterUpdate -= AuditBilateralBindings;
                                Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_READ_FAIL action="
                                    + map.name + "/" + action.name + " controls=" + action.controls.Count
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
                readableVector2Actions >= RequiredLocomotionActionCount)
            {
                _bindingAuditComplete = true;
                InputSystem.onAfterUpdate -= AuditBilateralBindings;
                Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_OK actions=" + locomotionActions
                    + " readable=" + readableVector2Actions
                    + " leftControls=" + leftControls
                    + " rightControls=" + rightControls
                    + " updates=" + _bindingAuditUpdates);
                return;
            }

            if (_bindingAuditUpdates < MaxBindingAuditUpdates) return;
            _bindingAuditComplete = true;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_FAIL actions=" + locomotionActions
                + " readable=" + readableVector2Actions
                + " leftControls=" + leftControls
                + " rightControls=" + rightControls
                + " updates=" + _bindingAuditUpdates);
        }

        private static bool HasUsage(InputDevice device, UnityEngine.InputSystem.Utilities.InternedString usage)
        {
            if (device == null) return false;
            for (int i = 0; i < device.usages.Count; i++)
                if (device.usages[i] == usage) return true;
            return false;
        }
    }
}
