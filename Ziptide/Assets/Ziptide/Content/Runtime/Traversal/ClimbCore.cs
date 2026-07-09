using System;

namespace Ziptide.Content.Traversal
{
    public enum Hand { Left, Right }

    /// <summary>
    /// VR HAND-OVER-HAND climbing — pure math (Hardwiring Phase 1.4; the "headline VR feel" the design
    /// doc calls for). The rule every good VR climb uses: while a hand grips a hold and moves, the RIG
    /// translates by the negative of that hand's world displacement — you pull the world down to pull
    /// yourself up. This core owns only the math + the two-hand handoff (so swapping grip hands never
    /// teleports the rig) + a release-fling comfort clamp. No UnityEngine, no XRI → EditMode-testable;
    /// the scene component reads controller poses, calls <see cref="MoveGrip"/>, and applies the
    /// returned delta to the XR Origin (never parenting the rig to the surface — SHIPS.md law).
    ///
    /// Two-hand semantics: the MOST-RECENTLY-gripped hand drives the rig. Gripping the second hand
    /// doesn't move you; it just becomes the standby anchor. Releasing the driving hand hands off to the
    /// other still-gripping hand with ZERO rig jump (that hand's last pos becomes the new reference).
    /// </summary>
    public sealed class ClimbGrip
    {
        private bool _leftGripped, _rightGripped;
        private TVec3 _leftPos, _rightPos;
        private Hand _driver;
        private bool _hasDriver;

        public bool IsClimbing => _hasDriver && (_leftGripped || _rightGripped);
        public Hand Driver => _driver;

        /// <summary>Grab a hold at a world position. Becomes the new driving hand.</summary>
        public void Grip(Hand hand, TVec3 worldPos)
        {
            if (hand == Hand.Left) { _leftGripped = true; _leftPos = worldPos; }
            else { _rightGripped = true; _rightPos = worldPos; }
            _driver = hand;
            _hasDriver = true;
        }

        /// <summary>The driving hand moved to a new world position. Returns the rig displacement to
        /// apply (= −handDelta). A non-driving hand's move returns zero (it's just tracking).</summary>
        public TVec3 MoveGrip(Hand hand, TVec3 newWorldPos)
        {
            if (!_hasDriver || hand != _driver) { StoreOnly(hand, newWorldPos); return Zero; }
            TVec3 last = hand == Hand.Left ? _leftPos : _rightPos;
            TVec3 rigDelta = (last - newWorldPos); // opposite the hand: pull yourself along the hold
            StoreOnly(hand, newWorldPos);
            return rigDelta;
        }

        /// <summary>Release a hand. If it was driving and the other hand still grips, hand off to it
        /// with no jump. Returns a comfort-clamped fling velocity for the launch-off case (empty when
        /// still climbing).</summary>
        public TVec3 Release(Hand hand, TVec3 handVelocity, float maxFling)
        {
            if (hand == Hand.Left) _leftGripped = false; else _rightGripped = false;

            if (_hasDriver && hand == _driver)
            {
                if (hand != Hand.Left && _leftGripped) { _driver = Hand.Left; return Zero; }
                if (hand != Hand.Right && _rightGripped) { _driver = Hand.Right; return Zero; }
                // No hand left on the wall — you let go. Hand your momentum to the rig, comfort-capped.
                _hasDriver = false;
                return ClampFling(handVelocity, maxFling);
            }
            return Zero;
        }

        private void StoreOnly(Hand hand, TVec3 pos)
        {
            if (hand == Hand.Left) _leftPos = pos; else _rightPos = pos;
        }

        private static TVec3 Zero => new TVec3(0f, 0f, 0f);

        /// <summary>Cap the launch velocity so releasing at the top of a fast pull doesn't fling the
        /// player across the world (VR comfort). Direction preserved, magnitude clamped.</summary>
        public static TVec3 ClampFling(TVec3 v, float maxFling)
        {
            float len = v.Length;
            if (maxFling <= 0f || len <= maxFling || len <= 1e-5f) return v;
            return v * (maxFling / len);
        }
    }
}
