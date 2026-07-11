#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.XR;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// Button-free development gesture: hold both tracked controllers close together above the
    /// headset for a continuous hold. All sampled positions come from the same XR tracking space.
    /// The detector latches after firing and must be released before it can fire again.
    /// </summary>
    public sealed class DevMenuGesture
    {
        public const float HoldSeconds = 2f;
        public const float MinimumAboveHead = 0.05f;
        public const float MaximumHeadDistance = 0.55f;
        public const float MaximumControllerSeparation = 0.35f;

        private float _heldSeconds;
        private bool _latched;

        public float Progress01 => Mathf.Clamp01(_heldSeconds / HoldSeconds);

        public bool Tick(float deltaTime)
        {
            bool active = TryReadTrackedPose(out Vector3 head, out Vector3 left, out Vector3 right)
                && IsSummonPose(head, left, right);
            return TickPose(active, deltaTime);
        }

        /// <summary>Pure state seam for EditMode tests.</summary>
        public bool TickPose(bool active, float deltaTime)
        {
            if (!active)
            {
                _heldSeconds = 0f;
                _latched = false;
                return false;
            }

            if (_latched) return false;

            _heldSeconds += Mathf.Max(0f, deltaTime);
            if (_heldSeconds < HoldSeconds) return false;

            _heldSeconds = HoldSeconds;
            _latched = true;
            return true;
        }

        public static bool IsSummonPose(Vector3 head, Vector3 left, Vector3 right)
        {
            float minimumY = head.y + MinimumAboveHead;
            if (left.y < minimumY || right.y < minimumY) return false;
            if (Vector3.Distance(head, left) > MaximumHeadDistance) return false;
            if (Vector3.Distance(head, right) > MaximumHeadDistance) return false;
            return Vector3.Distance(left, right) <= MaximumControllerSeparation;
        }

        private static bool TryReadTrackedPose(out Vector3 head, out Vector3 left, out Vector3 right)
        {
            head = default;
            left = default;
            right = default;

            InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            return headDevice.isValid
                && leftDevice.isValid
                && rightDevice.isValid
                && headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out head)
                && leftDevice.TryGetFeatureValue(CommonUsages.devicePosition, out left)
                && rightDevice.TryGetFeatureValue(CommonUsages.devicePosition, out right);
        }
    }
}
#endif
