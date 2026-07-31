using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The drum that turns (⚖ Terry: *"have the gun rack rotate … they can cycle through them but it
    /// only takes up a small amount of space"*).
    ///
    /// Thin translator over <see cref="ArmouryCarouselCore"/>: it holds a target angle, eases toward
    /// it, and asks the core where everything goes. It decides nothing — which detent a weapon lives
    /// at, where the drum settles, and which way it turns are all pure and already tested.
    ///
    /// Easing rather than snapping instantly is a VR comfort choice, not decoration: a rack that
    /// teleports between positions half a metre from your face reads as a glitch. Easing along the
    /// SHORT arc (the core guarantees it) keeps the motion small, predictable, and always in the
    /// direction the player asked for.
    /// </summary>
    [DisallowMultipleComponent]
    public class ArmouryCarouselRuntime : MonoBehaviour
    {
        /// <summary>Degrees per second the drum eases at. Tuned on device; brisk but not snappy.</summary>
        private const float TurnSpeedDegPerSec = 320f;

        /// <summary>Below this the drum is considered settled and stops updating.</summary>
        private const float SettleEpsilon = 0.05f;

        private float _targetAngle;
        private float _currentAngle;
        private int _detent;

        /// <summary>Which detent is presented to the player right now.</summary>
        public int Detent => _detent;

        /// <summary>Advance one detent. +1 turns the next weapon toward the player, -1 the previous.</summary>
        public void Step(int direction)
        {
            if (direction == 0) return;
            _detent = _detent + (direction > 0 ? 1 : -1);
            _targetAngle = ArmouryCarouselCore.SnapToDetent(
                _targetAngle - Mathf.Sign(direction) * ArmouryCarouselCore.StepDegrees);

            Debug.Log("ZIPTIDE: ARMOURY_TURN detent=" + _detent
                + " face=" + ArmouryCarouselCore.FaceAtFront(_targetAngle)
                + " target=" + _targetAngle.ToString("F0"));
        }

        /// <summary>Turn a specific weapon slot to the front without stepping through the others.</summary>
        public void PresentSlot(int slotIndex)
        {
            _targetAngle = ArmouryCarouselCore.AngleForSlot(slotIndex);
            Debug.Log("ZIPTIDE: ARMOURY_PRESENT slot=" + slotIndex
                + " target=" + _targetAngle.ToString("F0"));
        }

        private void Update()
        {
            float delta = ArmouryCarouselCore.ShortestDelta(_currentAngle, _targetAngle);
            if (Mathf.Abs(delta) < SettleEpsilon) return;

            float step = TurnSpeedDegPerSec * Time.deltaTime;
            _currentAngle += Mathf.Clamp(delta, -step, step);
            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }
}
