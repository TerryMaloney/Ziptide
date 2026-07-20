#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemXRController = UnityEngine.InputSystem.XR.XRController;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Shared headless environment for the complete Recovery PlayMode namespace. The real Quest always
    /// has tracked left/right controllers before _Boot resolves its InputAction assets; Linux CI did not.
    /// Previously only individual route tests installed virtual devices, and they did so after _Boot,
    /// allowing the production input-settle guard to fail closed before the test simulator existed.
    ///
    /// This fixture establishes the missing platform precondition once for the suite. Per-test controller
    /// simulators may still add their own devices for pose/ray control; these neutral devices only ensure
    /// that the canonical action assets can resolve safely during cold boot.
    /// </summary>
    [SetUpFixture]
    public sealed class RecoveryPlayModeInputEnvironment
    {
        private static InputSystemXRController _left;
        private static InputSystemXRController _right;

        [OneTimeSetUp]
        public void InstallHeadlessTrackedControllers()
        {
            EnsureDevices();
            Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_READY devices=2");
        }

        [SetUp]
        public void EnsureDevicesBeforeEveryRecoveryTest()
        {
            EnsureDevices();
        }

        [OneTimeTearDown]
        public void RemoveHeadlessTrackedControllers()
        {
            Remove(ref _right);
            Remove(ref _left);
            InputSystem.Update();
            Debug.Log("ZIPTIDE: RECOVERY_HEADLESS_XR_REMOVED");
        }

        private static void EnsureDevices()
        {
            if (_left == null || !_left.added)
            {
                _left = InputSystem.AddDevice<InputSystemXRController>();
                InputSystem.SetDeviceUsage(_left, CommonUsages.LeftHand);
            }

            if (_right == null || !_right.added)
            {
                _right = InputSystem.AddDevice<InputSystemXRController>();
                InputSystem.SetDeviceUsage(_right, CommonUsages.RightHand);
            }

            InputSystem.Update();
        }

        private static void Remove(ref InputSystemXRController device)
        {
            if (device != null && device.added)
                InputSystem.RemoveDevice(device);
            device = null;
        }
    }
}
#endif
