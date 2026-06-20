using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure, headless stun timing (no Unity scene needed → CI-testable). Tracks a self-clearing
    /// movement-slow window. Re-stuns REFRESH (take max remaining + strongest slow), never stack.
    /// <see cref="PlayerStunReceiver"/> wraps it with the screen flash + move-speed scaling.
    /// </summary>
    public class StunState
    {
        private float _remaining;
        private float _slow = 1f;

        /// <summary>Current movement multiplier (1 = full speed, &lt;1 = slowed).</summary>
        public float SlowFactor => _remaining > 0f ? _slow : 1f;

        /// <summary>True when no stun is active.</summary>
        public bool IsClear => _remaining <= 0f;

        public void Apply(float seconds, float slowFactor)
        {
            if (seconds <= 0f) return;
            bool wasActive = _remaining > 0f;
            _remaining = Mathf.Max(_remaining, seconds);        // refresh, not stack
            _slow = wasActive ? Mathf.Min(_slow, slowFactor) : slowFactor; // strongest slow while active
        }

        public void Tick(float dt)
        {
            if (_remaining <= 0f) return;
            _remaining -= dt;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _slow = 1f;
            }
        }
    }
}
