using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
    /// The post-update audit independently proves the real locomotion actions resolve controls on
    /// both left- and right-hand devices; a one-sided synthetic success is treated as an error.
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
            _bindingAuditUpdates = 0;
            _bindingAuditComplete = false;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            InputSystem.onAfterUpdate += AuditBilateralBindings;
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_LAYOUT controls=Primary2DAxis,GripButton");
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
                            foreach (InputControl control in action.controls)
                            {
                                if (control == null || control.device == null) continue;
                                if (HasUsage(control.device, CommonUsages.LeftHand)) leftControls++;
                                if (HasUsage(control.device, CommonUsages.RightHand)) rightControls++;
                            }
                        }
                    }
                }
            }

            if (locomotionActions < RequiredLocomotionActionCount) return;
            _bindingAuditUpdates++;

            if (leftControls > 0 && rightControls > 0)
            {
                _bindingAuditComplete = true;
                InputSystem.onAfterUpdate -= AuditBilateralBindings;
                Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_OK actions=" + locomotionActions
                    + " leftControls=" + leftControls
                    + " rightControls=" + rightControls
                    + " updates=" + _bindingAuditUpdates);
                return;
            }

            if (_bindingAuditUpdates < MaxBindingAuditUpdates) return;
            _bindingAuditComplete = true;
            InputSystem.onAfterUpdate -= AuditBilateralBindings;
            Debug.LogError("ZIPTIDE: RECOVERY_VIRTUAL_XR_BILATERAL_FAIL actions=" + locomotionActions
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
