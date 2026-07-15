using UnityEngine;
using UnityEngine.InputSystem;

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
    /// </summary>
    internal static class RecoveryVirtualXrLayoutBootstrap
    {
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            InputSystem.RegisterLayoutOverride(OverrideJson);
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_LAYOUT controls=Primary2DAxis,GripButton");
        }
    }
}
