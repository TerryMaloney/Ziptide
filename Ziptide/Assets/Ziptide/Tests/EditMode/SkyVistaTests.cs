using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// SkyVista contract, part 1 (pure core): definition validation + deterministic texel math.
    /// The dome bake must be seed-deterministic, dithered (no banding rows), and the Shell grid
    /// must be an exact no-op at intensity 0 (story canon: W001 shows no grid at all).
    /// Library/progression coverage lands with SkyVistaLibrary.
    /// </summary>
    public class SkyVistaTests
    {
        private const int W = 64;
        private const int H = 64;

        private static SkyVistaDefinition NewVista(string id = "test_vista")
        {
            var def = ScriptableObject.CreateInstance<SkyVistaDefinition>();
            def.vistaId = id;
            var g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(new Color(0.30f, 0.28f, 0.26f), 0f), new GradientColorKey(new Color(0.10f, 0.12f, 0.20f), 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            def.skyGradient = g;
            return def;
        }

        // ── Validation ──────────────────────────────────────────────────────

        [Test]
        public void WellFormedVista_ValidatesClean()
        {
            var def = NewVista();
            def.bodies.Add(new SkyVistaDefinition.CelestialBodyDef());
            Assert.IsEmpty(def.Validate());
        }

        [Test]
        public void EmptyId_IsInvalid()
        {
            var def = NewVista("");
            CollectionAssert.Contains(def.Validate(), "empty vistaId");
        }

        [Test]
        public void MoreThanThreeBodies_IsInvalid()
        {
            var def = NewVista();
            for (int i = 0; i < 4; i++) def.bodies.Add(new SkyVistaDefinition.CelestialBodyDef());
            Assert.IsNotEmpty(def.Validate());
        }

        [Test]
        public void BodyAngularSizeOutOfRange_IsInvalid()
        {
            var def = NewVista();
            def.bodies.Add(new SkyVistaDefinition.CelestialBodyDef { angularSizeDeg = 90f });
            Assert.IsNotEmpty(def.Validate());
        }

        // ── Dome bake ───────────────────────────────────────────────────────

        [Test]
        public void BakeDome_FillsExactBuffer()
        {
            var def = NewVista();
            var px = new Color32[W * H];
            SkyVistaTexture.BakeDome(def, W, H, px);
            // Horizon row must match the gradient's bottom color (within dither ± 1 LSB).
            Color expected = def.skyGradient.Evaluate(0f);
            Assert.LessOrEqual(Mathf.Abs(px[0].r - expected.r * 255f), 2f);
        }

        [Test]
        public void BakeDome_IsDeterministicPerSeed()
        {
            var def = NewVista();
            def.stars.density = 0.4f; def.stars.seed = 11;
            def.nebula.enabled = true; def.nebula.seed = 5;
            var a = new Color32[W * H];
            var b = new Color32[W * H];
            SkyVistaTexture.BakeDome(def, W, H, a);
            SkyVistaTexture.BakeDome(def, W, H, b);
            CollectionAssert.AreEqual(a, b);
        }

        [Test]
        public void DifferentStarSeeds_ProduceDifferentSkies()
        {
            var defA = NewVista(); defA.stars.density = 0.5f; defA.stars.seed = 1;
            var defB = NewVista(); defB.stars.density = 0.5f; defB.stars.seed = 2;
            var a = new Color32[W * H];
            var b = new Color32[W * H];
            SkyVistaTexture.BakeDome(defA, W, H, a);
            SkyVistaTexture.BakeDome(defB, W, H, b);
            CollectionAssert.AreNotEqual(a, b);
        }

        [Test]
        public void Dither_BreaksUpBandingRows()
        {
            // A subtle gradient quantized without dithering produces long runs of identical rows;
            // the Bayer dither must keep identical-consecutive-row runs short.
            var def = NewVista();
            var px = new Color32[W * 256];
            SkyVistaTexture.BakeDome(def, W, 256, px);

            int longestRun = 1, run = 1;
            for (int y = 1; y < 256; y++)
            {
                bool same = true;
                for (int x = 0; x < W; x++)
                    if (!px[y * W + x].Equals(px[(y - 1) * W + x])) { same = false; break; }
                run = same ? run + 1 : 1;
                if (run > longestRun) longestRun = run;
            }
            Assert.Less(longestRun, 8, "dome gradient shows banding (identical row run of " + longestRun + ")");
        }

        [Test]
        public void ShellGrid_AtZeroIntensity_IsExactNoOp()
        {
            var defOff = NewVista(); defOff.shellGridIntensity = 0f;
            var defOn = NewVista(); defOn.shellGridIntensity = 1f;
            var baseline = new Color32[W * H];
            var off = new Color32[W * H];
            var on = new Color32[W * H];
            SkyVistaTexture.BakeDome(NewVista(), W, H, baseline);
            SkyVistaTexture.BakeDome(defOff, W, H, off);
            SkyVistaTexture.BakeDome(defOn, W, H, on);
            CollectionAssert.AreEqual(baseline, off, "intensity 0 must not alter the sky");
            CollectionAssert.AreNotEqual(baseline, on, "intensity 1 must draw the grid");
        }

        [Test]
        public void StarCount_ScalesWithDensity()
        {
            int Bright(float density)
            {
                var def = NewVista();
                def.skyGradient.SetKeys(
                    new[] { new GradientColorKey(Color.black, 0f), new GradientColorKey(Color.black, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
                def.stars.density = density; def.stars.seed = 9;
                var px = new Color32[W * H];
                SkyVistaTexture.BakeDome(def, W, H, px);
                int n = 0;
                foreach (var p in px) if (p.r > 60) n++;
                return n;
            }
            int sparse = Bright(0.08f);
            int dense = Bright(0.6f);
            Assert.Greater(dense, sparse);
            Assert.Greater(sparse, 0);
        }

        [Test]
        public void ZenithShimmer_OnlyTouchesTheUpperSky()
        {
            var def = NewVista();
            def.zenithShimmer.enabled = true;
            def.zenithShimmer.intensity = 1f;
            var plain = new Color32[W * H];
            var shimmered = new Color32[W * H];
            SkyVistaTexture.BakeDome(NewVista(), W, H, plain);
            SkyVistaTexture.BakeDome(def, W, H, shimmered);
            // Bottom 60% of the dome must be untouched (the Pattern hangs at the zenith).
            for (int y = 0; y < (int)(H * 0.6f); y++)
                for (int x = 0; x < W; x++)
                    Assert.AreEqual(plain[y * W + x], shimmered[y * W + x],
                        "shimmer leaked below the zenith band at row " + y);
            CollectionAssert.AreNotEqual(plain, shimmered);
        }

        // ── Body bakes ──────────────────────────────────────────────────────

        [Test]
        public void BandedPlanet_UsesBothPaletteColors()
        {
            var body = new SkyVistaDefinition.CelestialBodyDef
            {
                type = SkyVistaDefinition.BodyType.BandedPlanet,
                baseColor = new Color(0.8f, 0.4f, 0.2f),
                accentColor = new Color(0.2f, 0.3f, 0.5f),
                bandCount = 6,
                phase = 0f
            };
            var px = new Color32[W * H];
            SkyVistaTexture.BakeBody(body, W, H, px);
            bool sawWarm = false, sawCool = false;
            foreach (var p in px)
            {
                if (p.r > p.b + 40) sawWarm = true;
                if (p.b > p.r + 20) sawCool = true;
            }
            Assert.IsTrue(sawWarm && sawCool, "banded planet must show both base and accent bands");
        }

        [Test]
        public void BlackHole_IsDarkWithABrightRing()
        {
            var body = new SkyVistaDefinition.CelestialBodyDef
            {
                type = SkyVistaDefinition.BodyType.BlackHole,
                baseColor = Color.white,
                accentColor = new Color(1f, 0.8f, 0.45f)
            };
            var px = new Color32[W * H];
            SkyVistaTexture.BakeBody(body, W, H, px);
            // Poles (v≈0 and v≈1) near-black; equator band blazing.
            Assert.Less((int)px[(H - 1) * W + W / 2].r, 30);
            int equator = (H / 2) * W + W / 2;
            Assert.Greater((int)px[equator].r, 150);
        }

        [Test]
        public void BodyBake_IsDeterministic()
        {
            var body = new SkyVistaDefinition.CelestialBodyDef { type = SkyVistaDefinition.BodyType.Moon, seed = 4 };
            var a = new Color32[W * H];
            var b = new Color32[W * H];
            SkyVistaTexture.BakeBody(body, W, H, a);
            SkyVistaTexture.BakeBody(body, W, H, b);
            CollectionAssert.AreEqual(a, b);
        }
    }
}
