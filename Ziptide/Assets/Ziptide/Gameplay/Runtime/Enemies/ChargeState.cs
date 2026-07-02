using UnityEngine;

namespace Ziptide.Gameplay
{
    public enum ChargePhase { Idle, Windup, Charging, Recover }

    /// <summary>
    /// Pure, headless charge FSM for the Bruiser archetype (no Unity scene → CI-testable).
    /// Idle → (player in range) → Windup (the readable telegraph — fair in VR) → Charging (a fixed-time
    /// straight rush; the BEHAVIOR clamps it at walls) → Recover (vulnerable pause) → Idle.
    /// <see cref="BruiserBehavior"/> wraps it with motion + visuals.
    /// </summary>
    public class ChargeState
    {
        public float TriggerRange = 7f;
        public float WindupSeconds = 1.1f;
        public float ChargeSeconds = 0.9f;
        public float RecoverSeconds = 1.6f;

        public ChargePhase Phase { get; private set; } = ChargePhase.Idle;
        /// <summary>0..1 windup progress (drives the telegraph).</summary>
        public float WindupProgress { get; private set; }
        /// <summary>True for exactly the tick the charge launches (behavior locks its direction then).</summary>
        public bool ChargeStarted { get; private set; }

        private float _timer;

        public void Tick(float dt, float distToPlayer, bool active)
        {
            ChargeStarted = false;

            if (!active)
            {
                // A stun interrupts everything — including mid-charge (the taser counter).
                Phase = ChargePhase.Idle;
                WindupProgress = 0f;
                return;
            }

            switch (Phase)
            {
                case ChargePhase.Idle:
                    if (distToPlayer <= TriggerRange)
                    {
                        Phase = ChargePhase.Windup;
                        _timer = WindupSeconds;
                        WindupProgress = 0f;
                    }
                    break;

                case ChargePhase.Windup:
                    _timer -= dt;
                    WindupProgress = WindupSeconds > 0f ? Mathf.Clamp01(1f - _timer / WindupSeconds) : 1f;
                    if (_timer <= 0f)
                    {
                        Phase = ChargePhase.Charging;
                        _timer = ChargeSeconds;
                        ChargeStarted = true;
                        WindupProgress = 0f;
                    }
                    break;

                case ChargePhase.Charging:
                    _timer -= dt;
                    if (_timer <= 0f)
                    {
                        Phase = ChargePhase.Recover;
                        _timer = RecoverSeconds;
                    }
                    break;

                case ChargePhase.Recover:
                    _timer -= dt;
                    if (_timer <= 0f) Phase = ChargePhase.Idle;
                    break;
            }
        }

        /// <summary>The behavior calls this when the charge slams a wall — skips straight to Recover.</summary>
        public void HitWall()
        {
            if (Phase != ChargePhase.Charging) return;
            Phase = ChargePhase.Recover;
            _timer = RecoverSeconds;
        }
    }
}
