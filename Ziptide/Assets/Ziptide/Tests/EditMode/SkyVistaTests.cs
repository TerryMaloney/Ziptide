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

        // ── Library: the canon progression (SkyVistaLibrary specs, in-memory) ─

        private static Dictionary<string, SkyVistaDefinition> BuildLibrary()
        {
            var built = new Dictionary<string, SkyVistaDefinition>();
            foreach (var spec in Ziptide.Editor.Patching.SkyVistaLibrary.Specs())
                built[spec.Key] = spec.Value();
            return built;
        }

        [Test]
        public void Library_CoversAllStoryWorldsAndArenas()
        {
            var lib = BuildLibrary();
            string[] required =
            {
                "ToxicCity",
                "W002_DryCistern", "W003_GlassShelf", "W004_BroadcastTomb", "W005_OxidizedCanopy",
                "W006_MirrorFlats", "W007_SableStation", "W008_SealedArchive", "W009_Chitinwall",
                "W010_TidalArray", "W011_TheHum", "W012_MarasLastJump",
                "Arena_Cistern", "Arena_Chitinwall", "Arena_MirrorFlats", "Arena_Tidal", "Arena_Void"
            };
            foreach (var scene in required)
                Assert.IsTrue(lib.ContainsKey(scene), "no vista spec for " + scene);
        }

        [Test]
        public void Library_AllVistasValidateClean_WithUniqueIds()
        {
            var lib = BuildLibrary();
            var ids = new HashSet<string>();
            foreach (var kv in lib)
            {
                var issues = kv.Value.Validate();
                Assert.IsEmpty(issues, kv.Key + ": " + string.Join(" | ", issues));
                Assert.IsTrue(ids.Add(kv.Value.vistaId), "duplicate vistaId " + kv.Value.vistaId);
            }
        }

        [Test]
        public void Canon_ShellGrid_InvisibleThroughW006_ThenMonotonic_ToFullWallAtW012()
        {
            var lib = BuildLibrary();
            string[] storyOrder =
            {
                "ToxicCity",
                "W002_DryCistern", "W003_GlassShelf", "W004_BroadcastTomb", "W005_OxidizedCanopy",
                "W006_MirrorFlats", "W007_SableStation", "W008_SealedArchive", "W009_Chitinwall",
                "W010_TidalArray", "W011_TheHum", "W012_MarasLastJump"
            };
            float prev = 0f;
            foreach (var scene in storyOrder)
            {
                float g = lib[scene].shellGridIntensity;
                Assert.GreaterOrEqual(g, prev, "Shell grid regressed at " + scene);
                prev = g;
            }
            Assert.AreEqual(0f, lib["W006_MirrorFlats"].shellGridIntensity, "grid must be invisible through W006");
            Assert.Greater(lib["W007_SableStation"].shellGridIntensity, 0f, "W007 is the first faint glimpse");
            Assert.AreEqual(0.5f, lib["W009_Chitinwall"].shellGridIntensity, 0.01f, "W009 is the banding beat");
            Assert.AreEqual(1f, lib["W012_MarasLastJump"].shellGridIntensity, "W012 is the full wall");
        }

        [Test]
        public void Canon_TheBandedGiant_GrowsAcrossTheEarlyWorlds()
        {
            var lib = BuildLibrary();
            float SizeOfGiant(string scene)
            {
                foreach (var b in lib[scene].bodies)
                    if (b.type == SkyVistaDefinition.BodyType.BandedPlanet) return b.angularSizeDeg;
                Assert.Fail(scene + " has no banded giant");
                return 0f;
            }
            // The canon growth beats: dim over W001 → closer at W005 → HUGE at W007 → the wall-body at W012.
            float w001 = SizeOfGiant("ToxicCity");
            float w005 = SizeOfGiant("W005_OxidizedCanopy");
            float w007 = SizeOfGiant("W007_SableStation");
            float w012 = SizeOfGiant("W012_MarasLastJump");
            Assert.Greater(w005, w001);
            Assert.Greater(w007, w005);
            Assert.Greater(w012, w007);
        }

        [Test]
        public void Canon_W003_HasTwoMoons_AndTheFirstPatternShimmer()
        {
            var lib = BuildLibrary();
            var w003 = lib["W003_GlassShelf"];
            int moons = 0;
            foreach (var b in w003.bodies)
                if (b.type == SkyVistaDefinition.BodyType.Moon) moons++;
            Assert.AreEqual(2, moons, "W003 canon: two moons");
            Assert.IsTrue(w003.zenithShimmer.enabled, "W003 canon: the zenith shimmer is the first Pattern seed");
            // And no earlier world shimmers.
            Assert.IsFalse(lib["ToxicCity"].zenithShimmer.enabled);
            Assert.IsFalse(lib["W002_DryCistern"].zenithShimmer.enabled);
        }

        [Test]
        public void Canon_RillCyan_IsSeededInAnEarlyWorldSky()
        {
            var lib = BuildLibrary();
            Color rill = Ziptide.Editor.Patching.SkyVistaLibrary.RillCyan;
            var w005 = lib["W005_OxidizedCanopy"];
            Assert.IsTrue(w005.nebula.enabled);
            Assert.Less(ColorDistance(w005.nebula.colorB, rill), 0.01f,
                "W005's nebula must carry RILL's exact chased color");
        }

        [Test]
        public void Arenas_HaveDistinctSkies()
        {
            var lib = BuildLibrary();
            string[] arenas = { "Arena_Cistern", "Arena_Chitinwall", "Arena_MirrorFlats", "Arena_Tidal", "Arena_Void" };
            for (int i = 0; i < arenas.Length; i++)
                for (int j = i + 1; j < arenas.Length; j++)
                {
                    var a = lib[arenas[i]];
                    var b = lib[arenas[j]];
                    bool differ = ColorDistance(a.skyGradient.Evaluate(0f), b.skyGradient.Evaluate(0f)) > 0.02f
                        || ColorDistance(a.skyGradient.Evaluate(1f), b.skyGradient.Evaluate(1f)) > 0.02f
                        || a.bodies.Count != b.bodies.Count
                        || Mathf.Abs(a.shellGridIntensity - b.shellGridIntensity) > 0.02f
                        || a.nebula.enabled != b.nebula.enabled
                        || (a.bodies.Count > 0 && b.bodies.Count > 0 && a.bodies[0].type != b.bodies[0].type);
                    Assert.IsTrue(differ, arenas[i] + " and " + arenas[j] + " read as the same sky");
                }
        }

        [Test]
        public void Library_EveryVistaBakesWithoutError()
        {
            var px = new Color32[W * H];
            var bodyPx = new Color32[W * H];
            foreach (var kv in BuildLibrary())
            {
                SkyVistaTexture.BakeDome(kv.Value, W, H, px);
                foreach (var b in kv.Value.bodies)
                    SkyVistaTexture.BakeBody(b, W, H, bodyPx);
            }
        }

        private static float ColorDistance(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
        }
    }
}
