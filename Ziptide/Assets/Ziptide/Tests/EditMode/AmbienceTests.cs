using NUnit.Framework;
using System;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The ambience contract (EXCELLENCE_MAP "ambient audio" row, moving ⬜→🧱): every shipped
    /// scene maps to a biome bed and is never silent by accident; synthesis is deterministic,
    /// clip-free of clipping, and LOOP-EXACT (integer-cycle construction: the seam between
    /// sample[N-1] and sample[0] is no bigger than an ordinary adjacent-sample step).
    /// </summary>
    public class AmbienceTests
    {
        private const int Rate = 22050;

        private static readonly string[] ShippedScenes =
        {
            "ToxicCity", "W002_DryCistern", "W003_GlassShelf", "W004_BroadcastTomb",
            "W005_OxidizedCanopy", "W006_MirrorFlats", "W007_SableStation", "W008_SealedArchive",
            "W009_Chitinwall", "W010_TidalArray", "W011_TheHum", "W012_MarasLastJump",
            "W000_DriftIn", "SandboxTestLab", "Cavern_TestLab", "W011_Undercroft",
            "PvP_Arena01", "SpaceLane_Trial", "D0_City", "MilestoneA_GrabCube",
        };

        [Test]
        public void EveryShippedScene_HasAnAudibleBed()
        {
            foreach (var scene in ShippedScenes)
            {
                var spec = BiomeAmbience.ForScene(scene);
                Assert.IsFalse(spec.Silent, scene + " is dead silent — the standard says never by accident");
            }
            // …and an unknown scene still gets the gentle default, not silence.
            Assert.IsFalse(BiomeAmbience.ForScene("W099_SomeFutureWorld").Silent);
        }

        [Test]
        public void Biomes_AreSonicallyDistinct()
        {
            var forest = BiomeAmbience.ForBiome("forest");
            var cave = BiomeAmbience.ForBiome("cave");
            var coast = BiomeAmbience.ForBiome("coastal");
            var voidB = BiomeAmbience.ForBiome("void");
            Assert.Greater(forest.ChirpDensity, 5f, "the Bloom sings");
            Assert.Greater(cave.DripDensity, 5f, "caves drip");
            Assert.AreEqual(0f, cave.ChirpDensity, "nothing chirps in the dark");
            Assert.Greater(coast.RumbleLevel, cave.RumbleLevel, "surf out-rumbles rock");
            Assert.LessOrEqual(voidB.WindLevel, 0f, "no air in the void");
            Assert.Greater(voidB.HumLevel, 0f, "…but the nothing has a pitch");
        }

        [Test]
        public void Synthesis_IsDeterministic()
        {
            var a = new float[Rate]; var b = new float[Rate];
            AmbienceSynth.FillWindLoop(a, 42, 0.5f, 0.5f, Rate);
            AmbienceSynth.FillWindLoop(b, 42, 0.5f, 0.5f, Rate);
            Assert.AreEqual(a[1234], b[1234], 0f);
            AmbienceSynth.FillWindLoop(b, 43, 0.5f, 0.5f, Rate);
            bool anyDiff = false;
            for (int i = 0; i < a.Length; i += 500) if (Math.Abs(a[i] - b[i]) > 1e-6f) { anyDiff = true; break; }
            Assert.IsTrue(anyDiff, "different seeds must sound different");
        }

        [Test]
        public void Loops_AreSeamless_AndClippingFree()
        {
            int n = Rate * 6;
            var buf = new float[n];

            void CheckLoop(string name)
            {
                float peak = 0f, maxStep = 0f;
                for (int i = 0; i < n; i++)
                {
                    float a = Math.Abs(buf[i]);
                    if (a > peak) peak = a;
                    if (i > 0)
                    {
                        float d = Math.Abs(buf[i] - buf[i - 1]);
                        if (d > maxStep) maxStep = d;
                    }
                }
                Assert.LessOrEqual(peak, 0.99f, name + " clips");
                float seam = Math.Abs(buf[n - 1] - buf[0]);
                Assert.LessOrEqual(seam, Math.Max(maxStep * 2f, 1e-4f),
                    name + " loop seam pops (seam " + seam + " vs max step " + maxStep + ")");
            }

            AmbienceSynth.FillWindLoop(buf, 7, 0.6f, 0.7f, Rate);
            CheckLoop("wind");
            AmbienceSynth.FillHumLoop(buf, 92f, 0.2f, Rate);
            CheckLoop("hum");
            AmbienceSynth.FillRumbleLoop(buf, 7, 0.3f, Rate);
            CheckLoop("rumble");
        }

        [Test]
        public void ZeroLevel_MeansTrueSilence()
        {
            var buf = new float[Rate];
            AmbienceSynth.FillWindLoop(buf, 5, 0f, 0.5f, Rate);
            foreach (var s in buf) Assert.AreEqual(0f, s);
            AmbienceSynth.FillHumLoop(buf, 60f, 0f, Rate);
            foreach (var s in buf) Assert.AreEqual(0f, s);
        }

        [Test]
        public void OneShots_AreShortAndDecay()
        {
            var buf = new float[(int)(Rate * 0.6f)];
            AmbienceSynth.FillChirp(buf, 3, 1200f, Rate);
            float head = 0f, tail = 0f;
            for (int i = 0; i < buf.Length / 4; i++) head = Math.Max(head, Math.Abs(buf[i]));
            for (int i = buf.Length * 3 / 4; i < buf.Length; i++) tail = Math.Max(tail, Math.Abs(buf[i]));
            Assert.Greater(head, 0.1f, "chirp speaks up front");
            Assert.Less(tail, head * 0.25f, "chirp decays — no clicks at the clip end");
        }
    }
}
