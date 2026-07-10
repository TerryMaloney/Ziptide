using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE II P4 gait-motor contract. The decisive one is MirroredLegs_AreAntiPhase: it builds
    /// the real skeleton, applies a motor pose the way ForgeCreatureAnimator does, and proves the
    /// left and right feet displace in OPPOSITE directions — the "both legs kick together" failure
    /// class caught in EditMode, before any scene or device is involved.
    /// </summary>
    public class ForgeGaitMotorTests
    {
        // Same genome as ForgeSkinnedBuilderTests: torso + mirrored 2-seg leg + 2-seg tail = 7 bones.
        private static ForgeCreatureBody SampleBody()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "test_walker";
            b.palette = new[] { new Color(0.1f, 0.1f, 0.12f), new Color(0.4f, 0.3f, 0.2f),
                new Color(0.3f, 0.8f, 0.95f), new Color(1f, 0.3f, 0.2f) };
            b.coreParts = new[]
            {
                new ForgePart { name = "Torso", op = ForgeOp.SphereSection, bevel = 1f, segments = 10,
                    smooth = true, size = new Vector3(0.5f, 0.3f, 0.6f), position = new Vector3(0f, 0.4f, 0f), paletteSlot = 1 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "LegFront", attachLocal = new Vector3(0.2f, 0.35f, 0.15f),
                    chainDirection = new Vector3(0.5f, -1f, 0.1f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.06f, 0.2f, 0.07f), paletteSlot = 0 },
                        new ForgeLimbSegment { size = new Vector3(0.05f, 0.22f, 0.06f), paletteSlot = 0 },
                    }
                },
                new ForgeLimb
                {
                    name = "Tail", attachLocal = new Vector3(0f, 0.4f, -0.3f),
                    chainDirection = new Vector3(0f, 0.1f, -1f), role = GaitRole.Tail,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.05f, 0.18f, 0.05f), paletteSlot = 1 },
                        new ForgeLimbSegment { size = new Vector3(0.035f, 0.16f, 0.035f), paletteSlot = 1 },
                    }
                },
            };
            return b;
        }

        private static Quaternion[] Eval(ForgeCreatureBody body, float t, float sp)
        {
            var q = new Quaternion[body.BoneCount()];
            ForgeGaitMotor.Evaluate(body, t, sp, q);
            return q;
        }

        [Test]
        public void Evaluate_IsDeterministic_AndNormalized()
        {
            var body = SampleBody();
            var a = Eval(body, 1.234f, 0.7f);
            var b = Eval(body, 1.234f, 0.7f);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i], "bone " + i + " diverged between identical calls");
                float dot = Quaternion.Dot(a[i], a[i]);
                Assert.AreEqual(1f, dot, 1e-3f, "bone " + i + " quaternion not normalized");
            }
        }

        [Test]
        public void Root_StaysIdentity_BodyBobIsNotTheSkeletonsJob()
        {
            var q = Eval(SampleBody(), 2.7f, 1f);
            Assert.Less(Quaternion.Angle(q[0], Quaternion.identity), 1e-3f);
        }

        [Test]
        public void MirroredLegs_AreAntiPhase()
        {
            var body = SampleBody();
            var r = ForgeSkinnedBuilder.Build(body);
            try
            {
                // Capture build pose, then pose at peak swing (sin(phase)=1 for the unmirrored leg
                // at speed 1: phase = 2π·hz·t = π/2 → t = 0.25/hz, hz = 1.2 + 1.6).
                var baseLocal = new Quaternion[r.bones.Length];
                for (int i = 0; i < r.bones.Length; i++) baseLocal[i] = r.bones[i].localRotation;
                Vector3 kneeL0 = r.bones[2].position; // bone order: 1,2 = leg; 3,4 = mirrored leg
                Vector3 kneeR0 = r.bones[4].position;

                float t = 0.25f / (1.2f + 1.6f);
                var q = Eval(body, t, 1f);
                for (int i = 1; i < r.bones.Length; i++)
                    r.bones[i].localRotation = baseLocal[i] * q[i];

                float dzL = r.bones[2].position.z - kneeL0.z;
                float dzR = r.bones[4].position.z - kneeR0.z;
                Assert.Greater(Mathf.Abs(dzL), 0.005f, "left leg did not swing at full speed");
                Assert.Greater(Mathf.Abs(dzR), 0.005f, "right leg did not swing at full speed");
                Assert.Less(dzL * dzR, 0f, "mirrored legs must swing in OPPOSITE directions "
                    + "(dzL=" + dzL + " dzR=" + dzR + ")");
            }
            finally { Object.DestroyImmediate(r.skeletonRoot); }
        }

        [Test]
        public void SpeedZero_LegsNearIdle_TailStillAlive()
        {
            var body = SampleBody();
            // Legs (bones 1–4) must be nearly still at speed 0 — idle sway only.
            float maxLeg = 0f;
            for (float t = 0f; t < 3f; t += 0.37f)
            {
                var q = Eval(body, t, 0f);
                for (int i = 1; i <= 4; i++)
                    maxLeg = Mathf.Max(maxLeg, Quaternion.Angle(q[i], Quaternion.identity));
            }
            Assert.Less(maxLeg, 4f, "legs must not walk while standing still");

            // The tail (bones 5–6) never sleeps — the creature reads as alive at idle.
            float maxTail = 0f;
            for (float t = 0f; t < 3f; t += 0.37f)
            {
                var q = Eval(body, t, 0f);
                for (int i = 5; i <= 6; i++)
                    maxTail = Mathf.Max(maxTail, Quaternion.Angle(q[i], Quaternion.identity));
            }
            Assert.Greater(maxTail, 0.5f, "the tail must idle-sway even when standing");
        }

        [Test]
        public void Motion_IsContinuous_NoSnapBetweenFrames()
        {
            var body = SampleBody();
            for (float t = 0f; t < 2f; t += 0.11f)
            {
                var a = Eval(body, t, 1f);
                var b = Eval(body, t + 0.01f, 1f);
                for (int i = 0; i < a.Length; i++)
                    Assert.Less(Quaternion.Angle(a[i], b[i]), 8f,
                        "bone " + i + " snapped " + Quaternion.Angle(a[i], b[i]) + "° in 10 ms at t=" + t);
            }
        }

        [Test]
        public void Breath_OscillatesAtIdle_BoundedAndDeterministic()
        {
            // The chest must MOVE at idle ("moving and breathing"), stay within its tiny
            // amplitude, hold volume roughly (XZ swell against slight counter-Y), and be
            // reproducible for identical inputs.
            float min = float.MaxValue, max = float.MinValue;
            for (float t = 0f; t < 5f; t += 0.23f)
            {
                Vector3 s = ForgeGaitMotor.BreathScale(7, t, 0f);
                Assert.AreEqual(s.x, s.z, 1e-5f, "breath swells the chest evenly in XZ");
                Assert.AreEqual(1f - 0.35f * (s.x - 1f), s.y, 1e-5f, "counter-Y holds volume");
                min = Mathf.Min(min, s.x); max = Mathf.Max(max, s.x);
                Assert.LessOrEqual(Mathf.Abs(s.x - 1f), 0.0121f, "breath amplitude bounded");
            }
            Assert.Greater(max - min, 0.005f, "the chest must actually move at idle");
            Assert.AreEqual(ForgeGaitMotor.BreathScale(7, 1.7f, 0f),
                ForgeGaitMotor.BreathScale(7, 1.7f, 0f), "breath must be deterministic");
        }

        [Test]
        public void Breath_FadesWithSpeed_AndSeedsDesync()
        {
            // A sprinting body reads through its gait, not its chest — amplitude at full speed
            // is well under idle. And two pack members must not breathe in lockstep.
            float idleAmp = 0f, runAmp = 0f;
            for (float t = 0f; t < 5f; t += 0.19f)
            {
                idleAmp = Mathf.Max(idleAmp, Mathf.Abs(ForgeGaitMotor.BreathScale(3, t, 0f).x - 1f));
                runAmp = Mathf.Max(runAmp, Mathf.Abs(ForgeGaitMotor.BreathScale(3, t, 1f).x - 1f));
            }
            Assert.Less(runAmp, idleAmp * 0.6f, "breath must fade as the gait takes over");

            bool diverged = false;
            for (float t = 0f; t < 3f && !diverged; t += 0.31f)
                diverged = Mathf.Abs(ForgeGaitMotor.BreathScale(11, t, 0f).x
                    - ForgeGaitMotor.BreathScale(500, t, 0f).x) > 1e-4f;
            Assert.IsTrue(diverged, "different seeds must desync the pack's breathing");
        }

        [Test]
        public void BoneOrder_MatchesTheBuilder_EveryLimbBoneDriven()
        {
            var body = SampleBody();
            // Over a full cycle at speed 1 every limb bone must carry SOME rotation — a
            // never-moving limb bone means the motor's enumeration drifted from the builder's
            // bone order. (Sampled over time: the rectified knee is legitimately zero for half
            // its cycle, so a single instant would false-alarm.)
            var max = new float[body.BoneCount()];
            for (float t = 0f; t < 1.2f; t += 0.07f)
            {
                var q = Eval(body, t, 1f);
                Assert.AreEqual(body.BoneCount(), q.Length);
                for (int i = 1; i < q.Length; i++)
                    max[i] = Mathf.Max(max[i], Quaternion.Angle(q[i], Quaternion.identity));
            }
            for (int i = 1; i < max.Length; i++)
                Assert.Greater(max[i], 0.5f,
                    "bone " + i + " never moved — enumeration mismatch with ForgeSkinnedBuilder?");
        }
    }
}
