using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE II P4 — THE GAIT MOTOR (pure math). Given a <see cref="ForgeCreatureBody"/>, a time
    /// and a normalized speed, emits one LOCAL-space rotation delta per bone (aligned with
    /// <see cref="ForgeSkinnedBuilder"/>'s bone order: [0]=root, then limb chains in declaration
    /// order, mirrored chain after its source). The applier composes them onto the captured
    /// build-pose local rotations: <c>bone.localRotation = baseLocal[i] * delta[i]</c>.
    ///
    /// THE CONJUGATION LAW (why this works for ANY chain direction): gait angles are authored
    /// about ROOT-space axes (X=right → fore-aft swing, Y=up → lateral wag, Z=forward → wing
    /// flap) and conjugated into bone space with the SAME chainRot the builder used
    /// (<c>delta = inv(chainRot) * rootDelta * chainRot</c>). FromToRotation(up, down) picks an
    /// arbitrary twist for straight-down legs — but builder and motor compute the identical
    /// quaternion, so the conjugation cancels the arbitrariness. Never author swings about raw
    /// bone-local axes; that trap is exactly what this file exists to avoid.
    ///
    /// Gait vocabulary per <see cref="GaitRole"/>:
    ///  Leg      — fore-aft swing about root X; mirrored side is anti-phase, successive leg
    ///             limbs alternate phase (diagonal gait); segments 1+ add rectified knee flexion
    ///             that lags the hip. Near-still at speed 0 (idle sway only).
    ///  Tail     — lateral traveling wave about root Y + a half-amplitude vertical bob;
    ///             amplitude GROWS toward the tip (whip), never sleeps (idle sway).
    ///  Tentacle — two-axis traveling wave (X and Z, quarter-turn offset) with per-segment lag.
    ///  Wing     — fast flap about root Z, mirrored wings beat in opposition.
    ///  Antenna  — small incommensurate two-axis sway (reads as air, not metronome).
    ///  None     — identity.
    /// Deterministic: same (body, time, speed) → identical quaternions. Root stays identity
    /// (body bob/lean is the behavior mover's job, not the skeleton's).
    /// </summary>
    public static class ForgeGaitMotor
    {
        /// <summary>Fill <paramref name="into"/> (length = body.BoneCount()) with per-bone local
        /// rotation deltas at <paramref name="time"/> seconds, <paramref name="speed01"/> ∈ [0,1].</summary>
        public static void Evaluate(ForgeCreatureBody body, float time, float speed01, Quaternion[] into)
        {
            if (into == null) return;
            for (int i = 0; i < into.Length; i++) into[i] = Quaternion.identity;
            if (body == null || body.limbs == null) return;
            speed01 = Mathf.Clamp01(speed01);

            int bone = 1;
            int legOrdinal = 0;
            foreach (var limb in body.limbs)
            {
                if (limb == null || limb.segments == null || limb.segments.Length == 0) continue;
                int myLegOrdinal = limb.role == GaitRole.Leg ? legOrdinal++ : 0;
                bone = EvaluateChain(limb, false, myLegOrdinal, time, speed01, bone, into);
                if (limb.mirrorX)
                    bone = EvaluateChain(limb, true, myLegOrdinal, time, speed01, bone, into);
            }
        }

        private static int EvaluateChain(ForgeLimb limb, bool mirrored, int legOrdinal,
            float time, float speed01, int bone, Quaternion[] into)
        {
            Vector3 dir = limb.chainDirection.sqrMagnitude > 1e-6f ? limb.chainDirection.normalized : Vector3.down;
            if (mirrored) dir.x = -dir.x;
            // MUST match ForgeSkinnedBuilder.BuildLimbChain exactly — see the conjugation law above.
            Quaternion chainRot = Quaternion.FromToRotation(Vector3.up, dir);
            Quaternion invChain = Quaternion.Inverse(chainRot);

            int count = limb.segments.Length;
            for (int s = 0; s < count && bone < into.Length; s++, bone++)
            {
                Quaternion rootDelta = RootDelta(limb.role, mirrored, legOrdinal, s, count, time, speed01);
                into[bone] = invChain * rootDelta * chainRot;
            }
            return bone;
        }

        private static Quaternion RootDelta(GaitRole role, bool mirrored, int legOrdinal,
            int s, int count, float t, float sp)
        {
            const float TwoPi = Mathf.PI * 2f;
            switch (role)
            {
                case GaitRole.Leg:
                {
                    float hz = 1.2f + 1.6f * sp;
                    float phase = TwoPi * hz * t + (mirrored ? Mathf.PI : 0f) + legOrdinal * Mathf.PI;
                    if (s == 0)
                    {
                        float swing = (1.5f + 26f * sp) * Mathf.Sin(phase);
                        return Quaternion.AngleAxis(swing, Vector3.right);
                    }
                    // The knee: flexes during the swing half of the cycle, lagging the hip —
                    // rectified so the leg never hyper-extends backwards.
                    float knee = (1.5f + 34f * sp) * Mathf.Max(0f, Mathf.Sin(phase + 0.7f));
                    return Quaternion.AngleAxis(knee, Vector3.right);
                }
                case GaitRole.Tail:
                {
                    float hz = 0.7f + 0.9f * sp;
                    float lag = 0.9f * s;
                    float amp = (5f + 13f * sp) * (s + 1f) / count; // whip: grows toward the tip
                    float wag = amp * Mathf.Sin(TwoPi * hz * t - lag);
                    float bob = amp * 0.3f * Mathf.Sin(TwoPi * hz * 2f * t - lag);
                    return Quaternion.AngleAxis(wag, Vector3.up) * Quaternion.AngleAxis(bob, Vector3.right);
                }
                case GaitRole.Tentacle:
                {
                    float hz = 0.5f + 0.8f * sp;
                    float lag = 1.1f * s;
                    float amp = 9f + 11f * sp;
                    float ax = amp * Mathf.Sin(TwoPi * hz * t - lag);
                    float az = amp * Mathf.Cos(TwoPi * hz * 0.7f * t - lag);
                    if (mirrored) az = -az;
                    return Quaternion.AngleAxis(ax, Vector3.right) * Quaternion.AngleAxis(az, Vector3.forward);
                }
                case GaitRole.Wing:
                {
                    float hz = 2.2f + 2.6f * sp;
                    float amp = (16f + 24f * sp) * Mathf.Max(0.3f, 1f - 0.25f * s); // tip follows softer
                    float flap = amp * Mathf.Sin(TwoPi * hz * t - 0.5f * s) * (mirrored ? -1f : 1f);
                    return Quaternion.AngleAxis(flap, Vector3.forward);
                }
                case GaitRole.Antenna:
                {
                    // Incommensurate frequencies so the sway never settles into a metronome.
                    float ax = 3f * Mathf.Sin(1.3f * t + s * 0.8f);
                    float az = 3f * Mathf.Sin(1.7f * t + s * 1.3f + 0.5f);
                    return Quaternion.AngleAxis(ax, Vector3.right) * Quaternion.AngleAxis(az, Vector3.forward);
                }
                default:
                    return Quaternion.identity;
            }
        }
    }
}
