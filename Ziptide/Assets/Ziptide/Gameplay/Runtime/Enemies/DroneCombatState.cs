using UnityEngine;

namespace Ziptide.Gameplay
{
    public enum DroneCombatPhase { Patrol, Engage, Telegraph, Cooldown }

    /// <summary>
    /// Pure, headless combat FSM for a single drone (no Unity scene needed → CI-testable).
    /// Drives Patrol → Engage → Telegraph → Fire → Cooldown from distance/LoS/alive inputs.
    /// <see cref="DroneCombatBehavior"/> wraps it with movement + VFX + projectile spawning.
    /// </summary>
    public class DroneCombatState
    {
        public float DetectRange = 10f;
        public float LoseRange = 14f;
        public float TelegraphSeconds = 0.9f;
        public float BoltCooldown = 2.5f;

        public DroneCombatPhase Phase { get; private set; } = DroneCombatPhase.Patrol;
        /// <summary>True for exactly the one tick a bolt should be fired.</summary>
        public bool FireRequested { get; private set; }
        /// <summary>0..1 telegraph progress (for wind-up VFX).</summary>
        public float TelegraphProgress { get; private set; }

        private float _telegraphTimer;
        private float _cooldownTimer;

        public void Tick(float dt, float distance, bool hasLoS, bool isActive)
        {
            FireRequested = false;

            if (!isActive)
            {
                Phase = DroneCombatPhase.Patrol;
                _telegraphTimer = 0f;
                TelegraphProgress = 0f;
                return;
            }

            switch (Phase)
            {
                case DroneCombatPhase.Patrol:
                    if (distance <= DetectRange && hasLoS)
                        Phase = DroneCombatPhase.Engage;
                    break;

                case DroneCombatPhase.Engage:
                    if (distance > LoseRange || !hasLoS) { Phase = DroneCombatPhase.Patrol; break; }
                    Phase = DroneCombatPhase.Telegraph;
                    _telegraphTimer = TelegraphSeconds;
                    TelegraphProgress = 0f;
                    break;

                case DroneCombatPhase.Telegraph:
                    // Lose the shot if the target leaves range OR breaks line-of-sight — no firing
                    // through walls (you ducking behind cover now actually cancels the bolt).
                    if (distance > LoseRange || !hasLoS) { Phase = DroneCombatPhase.Patrol; TelegraphProgress = 0f; break; }
                    _telegraphTimer -= dt;
                    TelegraphProgress = TelegraphSeconds > 0f
                        ? Mathf.Clamp01(1f - _telegraphTimer / TelegraphSeconds) : 1f;
                    if (_telegraphTimer <= 0f)
                    {
                        FireRequested = true;
                        Phase = DroneCombatPhase.Cooldown;
                        _cooldownTimer = BoltCooldown;
                        TelegraphProgress = 0f;
                    }
                    break;

                case DroneCombatPhase.Cooldown:
                    _cooldownTimer -= dt;
                    if (_cooldownTimer <= 0f)
                    {
                        if (distance <= LoseRange && hasLoS)
                        {
                            Phase = DroneCombatPhase.Telegraph;
                            _telegraphTimer = TelegraphSeconds;
                            TelegraphProgress = 0f;
                        }
                        else Phase = DroneCombatPhase.Patrol;
                    }
                    break;
            }
        }
    }
}
