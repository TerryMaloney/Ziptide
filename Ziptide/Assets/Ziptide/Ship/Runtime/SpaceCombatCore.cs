using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>Armor-only ship damage state (the locked combat model, in space): no health bar,
    /// armor recharges after a quiet delay, zero armor = DISABLED — and disabled is final until
    /// salvaged (non-lethal: ships power down, they never explode).</summary>
    public struct ShipArmorState
    {
        public float Armor;
        public float LastHitTime;
        public bool Disabled;
    }

    /// <summary>
    /// SPACE COMBAT 3.1 — the PURE core (SPACE_COMBAT.md: non-lethal disable + salvage). No scene
    /// types, no clocks of its own — callers pass `now`, tests drive time by hand. Laws pinned by
    /// SpaceCombatCoreTests: fire respects the cooldown · the aim cone is generous (VR comfort —
    /// precision comes from flying, not from pixel-hunting) · armor floors at 0 and DISABLES ·
    /// live armor recharges only after the quiet delay · a disabled ship NEVER recharges (salvage
    /// stays salvage) · salvage needs proximity (fly to your prize — the loop that feeds the economy).
    /// </summary>
    public static class SpaceCombatCore
    {
        public const float BoltRange = 220f;
        public const float AimConeDegrees = 6f;
        public const float FireCooldown = 0.55f;
        public const float BoltDamage = 2f;
        public const float RechargeDelay = 4f;
        public const float RechargePerSecond = 2f;
        public const float SalvageRange = 14f;

        public static bool CanFire(float lastFireTime, float now) => now - lastFireTime >= FireCooldown;

        /// <summary>Cone hit test from the ship's nose. Pure geometry: inside range AND within the
        /// half-angle of forward. The cone IS the auto-aim (comfort law: no pixel-hunting in VR).</summary>
        public static bool InAimCone(Vector3 shipPos, Vector3 forward, Vector3 targetPos,
            float coneDegrees = AimConeDegrees, float range = BoltRange)
        {
            Vector3 to = targetPos - shipPos;
            float dist = to.magnitude;
            if (dist > range || dist < 0.01f) return false;
            return Vector3.Angle(forward, to) <= coneDegrees;
        }

        /// <summary>Apply one bolt. Armor floors at 0; crossing 0 flips Disabled — permanently.</summary>
        public static ShipArmorState Hit(ShipArmorState s, float damage, float now)
        {
            if (s.Disabled) return s; // already down — bolts pass a powered-down hull
            s.Armor = Mathf.Max(0f, s.Armor - Mathf.Max(0f, damage));
            s.LastHitTime = now;
            if (s.Armor <= 0f) s.Disabled = true;
            return s;
        }

        /// <summary>Advance armor recharge. Disabled ships never recharge — the salvage loop needs
        /// the wreck to STAY a wreck. Live ships climb back to max after the quiet delay.</summary>
        public static ShipArmorState Recharge(ShipArmorState s, float maxArmor, float now, float dt)
        {
            if (s.Disabled || dt <= 0f) return s;
            if (now - s.LastHitTime < RechargeDelay) return s;
            s.Armor = Mathf.Min(maxArmor, s.Armor + RechargePerSecond * dt);
            return s;
        }

        public static bool InSalvageRange(Vector3 shipPos, Vector3 targetPos)
            => (targetPos - shipPos).sqrMagnitude <= SalvageRange * SalvageRange;

        /// <summary>How far out the wreck starts answering your approach, as a multiple of range.</summary>
        public const float ApproachBandMultiplier = 2.5f;

        /// <summary>
        /// 0..1 approach read for a downed wreck: 0 outside the band, rising as you close, 1 once
        /// you are actually in salvage range. Without it the salvage loop was invisible — a pilot
        /// either got the pickup or got nothing, with no signal that flying closer was the verb.
        /// Pure so the cue's shape is testable without a scene.
        /// </summary>
        public static float SalvageApproach01(Vector3 shipPos, Vector3 targetPos)
        {
            float d = (targetPos - shipPos).magnitude;
            float band = SalvageRange * ApproachBandMultiplier;
            if (d >= band) return 0f;
            if (d <= SalvageRange) return 1f;
            return 1f - (d - SalvageRange) / (band - SalvageRange);
        }
    }
}
