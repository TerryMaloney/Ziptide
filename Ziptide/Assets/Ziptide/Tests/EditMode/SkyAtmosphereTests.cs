using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The SKYSCAPE_DESIGN contract, pinned: every hazard tag in the chapter catalog maps to an
    /// atmosphere default (§4.1), specs respect the design's own laws (radiation is THIN, void is
    /// near-empty, swarm brings no separate motes, layers sit at real stereo depths), and the drift
    /// math is a pure bounded function of (seed, index, time) — no simulation state, CI-provable.
    /// </summary>
    public class SkyAtmosphereTests
    {
        // §4.1's full vocabulary — the table must answer for every tag the chapter docs use.
        private static readonly string[] AllHazardTags =
        {
            "Bloom", "spore", "fire", "acid", "radiation", "flood", "vibration", "cave-in",
            "swarm", "static", "pattern", "reflection", "wind", "void", "pressure", "all",
        };

        [Test]
        public void EveryHazardTag_HasADefault()
        {
            foreach (var tag in AllHazardTags)
            {
                var s = SkyAtmosphere.ForHazard(tag, 1f);
                Assert.IsTrue(s.HazeEnabled || s.MoteCount > 0 || tag == "none",
                    tag + " maps to a completely empty atmosphere — the table lost a row");
            }
            var off = SkyAtmosphere.ForHazard("none", 1f);
            Assert.IsFalse(off.HazeEnabled); Assert.AreEqual(0, off.MoteCount);
            var unknown = SkyAtmosphere.ForHazard("brand_new_hazard", 1f);
            Assert.IsFalse(unknown.HazeEnabled, "unknown tags must defer, never invent");
        }

        [Test]
        public void DesignLaws_HoldInTheTable()
        {
            var bloom = SkyAtmosphere.ForHazard("Bloom", 1f);
            var rad = SkyAtmosphere.ForHazard("radiation", 1f);
            var voidS = SkyAtmosphere.ForHazard("void", 1f);
            var swarm = SkyAtmosphere.ForHazard("swarm", 1f);
            var wind = SkyAtmosphere.ForHazard("wind", 1f);

            Assert.Greater(bloom.MoteCount, 100, "Bloom is the game's Prospect reference — dense spore air");
            Assert.Less(rad.MoteCount, 25, "radiation: LESS particulate reads as MORE dangerous");
            Assert.Less(rad.HazeTint.a, 0.15f, "radiation haze is thin");
            Assert.AreEqual(0, voidS.MoteCount, "raw space has nothing drifting");
            Assert.AreEqual(0, swarm.MoteCount, "the swarm ITSELF is the particulate — no separate layer");
            Assert.AreEqual(DriftStyle.Wind, wind.Drift);
            Assert.AreEqual(DriftStyle.Glitch, SkyAtmosphere.ForHazard("pattern", 1f).Drift);
        }

        [Test]
        public void Layers_SitAtRealStereoDepths()
        {
            // The whole VR argument: haze FAR (tens of metres), motes NEAR (single-digit-to-teens),
            // never on the infinite dome, never overlapping each other's band.
            var s = SkyAtmosphere.ForHazard("Bloom", 1f);
            Assert.GreaterOrEqual(s.HazeDistance, 25f);
            Assert.LessOrEqual(s.HazeDistance, 80f);
            Assert.GreaterOrEqual(s.ShellNear, 4f);
            Assert.LessOrEqual(s.ShellFar, 20f);
            Assert.Less(s.ShellFar, s.HazeDistance, "motes must sit NEARER than the haze card");
        }

        [Test]
        public void Intensity_ScalesAndZeroDisables()
        {
            var full = SkyAtmosphere.ForHazard("Bloom", 1f);
            var half = SkyAtmosphere.ForHazard("Bloom", 0.5f);
            var off = SkyAtmosphere.ForHazard("Bloom", 0f);
            Assert.Less(half.MoteCount, full.MoteCount);
            Assert.Less(half.HazeTint.a, full.HazeTint.a);
            Assert.IsFalse(off.HazeEnabled);
            Assert.AreEqual(0, off.MoteCount);
        }

        [Test]
        public void Drift_IsDeterministic()
        {
            var s = SkyAtmosphere.ForHazard("Bloom", 1f);
            for (int i = 0; i < 10; i++)
            {
                var a = SkyAtmosphere.MotePosition(42, i, 13.7f, in s);
                var b = SkyAtmosphere.MotePosition(42, i, 13.7f, in s);
                Assert.AreEqual(a, b, "same (seed,index,time) must be the same position");
            }
            var other = SkyAtmosphere.MotePosition(43, 0, 13.7f, in s);
            Assert.AreNotEqual(SkyAtmosphere.MotePosition(42, 0, 13.7f, in s), other,
                "different seeds must scatter differently");
        }

        [Test]
        public void Drift_StaysInsideTheShell_ForEveryStyle()
        {
            foreach (var tag in new[] { "Bloom", "wind", "static", "pattern", "radiation" })
            {
                var s = SkyAtmosphere.ForHazard(tag, 1f);
                if (s.MoteCount == 0) continue;
                for (int i = 0; i < 40; i++)
                    for (float t = 0f; t < 30f; t += 1.7f)
                    {
                        var p = SkyAtmosphere.MotePosition(7, i, t, in s);
                        float horiz = new Vector2(p.x, p.z).magnitude;
                        Assert.LessOrEqual(horiz, s.ShellFar + 1.5f, tag + " mote escaped the shell");
                        Assert.GreaterOrEqual(p.y, -4f, tag + " mote sank away");
                        Assert.LessOrEqual(p.y, 9f, tag + " mote floated away");
                    }
            }
        }

        [Test]
        public void W005_IsTheSignatureProof()
        {
            // SKYSCAPE_DESIGN §6 step 1: the prototype world carries the FULL stack — Bloom
            // atmosphere, body glow, and the mandatory light coupling (pillar 4). Pinned so a
            // library retune can't silently drop the game's first Prospect-bar sky.
            SkyVistaDefinition w005 = null;
            foreach (var spec in Ziptide.Editor.Patching.SkyVistaLibrary.Specs())
                if (spec.Key == "W005_OxidizedCanopy") w005 = spec.Value();
            Assert.IsNotNull(w005);
            Assert.IsTrue(w005.atmosphere.enabled, "W005 is the signature prototype");
            Assert.AreEqual("Bloom", w005.atmosphere.hazardTag);
            Assert.IsTrue(w005.atmosphere.bodyGlow);
            Assert.Greater(w005.directionalLightIntensity, 0f, "pillar 4: the sky's color reaches the ground");
            Assert.IsTrue(w005.overrideAmbient, "pillar 4: ambient carries the amber too");
            var derived = SkyAtmosphere.ForHazard(w005.atmosphere.hazardTag, w005.atmosphere.intensity);
            Assert.IsTrue(derived.HazeEnabled);
            Assert.Greater(derived.MoteCount, 50, "a Bloom signature world has dense spore air");
        }

        [Test]
        public void Glitch_Steps_WhileSteadyFlows()
        {
            var pattern = SkyAtmosphere.ForHazard("pattern", 1f);
            var bloom = SkyAtmosphere.ForHazard("Bloom", 1f);
            // Over a tiny dt a glitch mote holds EXACTLY still almost always (it only ever moves by
            // stepping between poses); a steady mote ALWAYS glides a little. Statistical on purpose —
            // a single sample could land exactly on a step boundary.
            const float dt = 0.005f;
            int glitchHolds = 0, samples = 0;
            for (int i = 0; i < 10; i++)
                for (float t = 3f; t < 13f; t += 2.1f)
                {
                    samples++;
                    var g1 = SkyAtmosphere.MotePosition(5, i, t, in pattern);
                    var g2 = SkyAtmosphere.MotePosition(5, i, t + dt, in pattern);
                    if ((g2 - g1).magnitude < 1e-4f) glitchHolds++;
                }
            Assert.Greater(glitchHolds, samples * 3 / 5,
                "glitch motes must mostly hold between steps (" + glitchHolds + "/" + samples + ")");

            var s1 = SkyAtmosphere.MotePosition(5, 3, 10f, in bloom);
            var s2 = SkyAtmosphere.MotePosition(5, 3, 10f + 0.05f, in bloom);
            float steadyMove = (s2 - s1).magnitude;
            Assert.Greater(steadyMove, 0f, "steady drift never freezes");
            Assert.Less(steadyMove, 0.2f, "steady drift is continuous and slow");
        }
    }
}
