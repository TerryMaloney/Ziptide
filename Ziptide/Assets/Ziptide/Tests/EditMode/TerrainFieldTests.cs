using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// H3 TerrainField contracts (ARCHITECTURE V2 Q3): determinism, bounds, walkability (fraction at
    /// mesh resolution; worst-case for smooth biomes), warp actually warps, biomes actually differ,
    /// climate behaves. These pin the math the TERRAIN_SLOPE_UNWALKABLE gate trusts. Pure, headless.
    /// </summary>
    public class TerrainFieldTests
    {
        private static readonly BiomePreset[] AllBiomes =
        {
            BiomePreset.Dunes, BiomePreset.Mesas, BiomePreset.Canyon,
            BiomePreset.CavernFloor, BiomePreset.TideFlats,
        };

        private const float Amp = 22f;    // the largest amplitude any authored world asks for
        private const int Seed = 1337;

        // ── Determinism ───────────────────────────────────────────────────────────────────────────

        [Test]
        public void SameInputs_BitIdenticalHeights()
        {
            foreach (var biome in AllBiomes)
                for (float x = -120f; x <= 120f; x += 37.3f)
                    for (float z = -120f; z <= 120f; z += 41.7f)
                        Assert.AreEqual(
                            TerrainField.Height(biome, x, z, Amp, Seed),
                            TerrainField.Height(biome, x, z, Amp, Seed),
                            0f, biome + " not deterministic at " + x + "," + z);
        }

        [Test]
        public void DifferentSeeds_ProduceDifferentFields()
        {
            float divergence = 0f;
            for (float x = 0f; x < 200f; x += 13f)
                divergence += Mathf.Abs(TerrainField.Height(BiomePreset.Dunes, x, x, Amp, 1)
                                      - TerrainField.Height(BiomePreset.Dunes, x, x, Amp, 2));
            Assert.Greater(divergence, 1f, "two seeds produced (nearly) the same terrain");
        }

        [Test]
        public void EstimateMaxSlope_IsDeterministic()
        {
            Assert.AreEqual(
                TerrainField.EstimateMaxSlope(BiomePreset.Canyon, Amp, Seed, 80f),
                TerrainField.EstimateMaxSlope(BiomePreset.Canyon, Amp, Seed, 80f), 0f);
        }

        // ── Bounds ────────────────────────────────────────────────────────────────────────────────

        [Test]
        public void Height_StaysWithinAmplitudeEnvelope()
        {
            foreach (var biome in AllBiomes)
                for (float x = -250f; x <= 250f; x += 17.7f)
                    for (float z = -250f; z <= 250f; z += 19.3f)
                    {
                        float h = TerrainField.Height(biome, x, z, Amp, Seed);
                        Assert.LessOrEqual(Mathf.Abs(h), Amp * 1.5f,
                            biome + " exceeded the 1.5x amplitude envelope at " + x + "," + z);
                    }
        }

        [Test]
        public void Fbm_StaysInMinusOneOne()
        {
            for (float x = -300f; x <= 300f; x += 23f)
            {
                float n = TerrainField.Fbm(x, -x * 0.7f, Seed, 5, 60f);
                Assert.GreaterOrEqual(n, -1f);
                Assert.LessOrEqual(n, 1f);
            }
        }

        [Test]
        public void DegenerateInputs_AreSafe()
        {
            Assert.AreEqual(0f, TerrainField.Height(BiomePreset.None, 10f, 10f, Amp, Seed));
            Assert.AreEqual(0f, TerrainField.Height(BiomePreset.Dunes, 10f, 10f, 0f, Seed));
            Assert.AreEqual(0f, TerrainField.Height(BiomePreset.Dunes, 10f, 10f, -5f, Seed));
        }

        // ── Walkability (the gate's foundation) ───────────────────────────────────────────────────

        [Test]
        public void EveryBiome_MeetsTheWalkableFractionBar_EvenAtAbsurdAmplitude()
        {
            foreach (var biome in AllBiomes)
            {
                float frac = TerrainField.WalkableFraction(biome, 500f, Seed, 150f); // clamp must save us
                Assert.GreaterOrEqual(frac, TerrainField.MinWalkableFraction,
                    biome + " walkable fraction " + frac + " under the bar — internal clamp failed");
            }
        }

        [Test]
        public void SmoothBiomes_AlsoPinTheWorstCase()
        {
            foreach (var biome in new[] { BiomePreset.Dunes, BiomePreset.CavernFloor, BiomePreset.TideFlats })
            {
                float worst = TerrainField.EstimateMaxSlope(biome, Amp, Seed, 150f);
                Assert.LessOrEqual(worst, TerrainField.MaxWalkableSlope,
                    biome + " worst slope " + worst + " — smooth biomes must have no cliffs at all");
            }
        }

        [Test]
        public void WalkableFraction_HoldsAcrossSeeds()
        {
            for (int seed = 1; seed <= 12; seed++)
            {
                float frac = TerrainField.WalkableFraction(BiomePreset.Canyon, Amp, seed, 100f);
                Assert.GreaterOrEqual(frac, TerrainField.MinWalkableFraction,
                    "Canyon seed " + seed + " fraction " + frac);
            }
        }

        // ── Character: biomes differ, warp warps ──────────────────────────────────────────────────

        [Test]
        public void Biomes_ProducePairwiseDistinctFields()
        {
            for (int i = 0; i < AllBiomes.Length; i++)
            {
                for (int j = i + 1; j < AllBiomes.Length; j++)
                {
                    float diff = 0f;
                    for (float x = 0f; x < 300f; x += 11f)
                        diff += Mathf.Abs(TerrainField.Height(AllBiomes[i], x, x * 0.6f, Amp, Seed)
                                        - TerrainField.Height(AllBiomes[j], x, x * 0.6f, Amp, Seed));
                    Assert.Greater(diff, 5f, AllBiomes[i] + " vs " + AllBiomes[j] + " are near-identical");
                }
            }
        }

        [Test]
        public void TideFlats_ReliefIsMostlyBelowGrade()
        {
            int below = 0, total = 0;
            for (float x = -200f; x <= 200f; x += 9f)
            {
                float h = TerrainField.Height(BiomePreset.TideFlats, x, x * 1.3f, Amp, Seed);
                if (Mathf.Abs(h) > 0.3f) { total++; if (h < 0f) below++; }
            }
            Assert.Greater(total, 20, "flats produced almost no relief to measure");
            Assert.Greater(below / (float)total, 0.6f, "tide flats should carve pools, not raise hills");
        }

        [Test]
        public void DomainWarp_ActuallyChangesTheField()
        {
            // A pure translation of inputs can't emulate warp: compare the warped field with the
            // raw fBM the same parameters would give — they must diverge substantially.
            var p = TerrainField.ParamsFor(BiomePreset.Dunes);
            float diff = 0f;
            for (float x = 0f; x < 260f; x += 13f)
            {
                float warped = TerrainField.Height(BiomePreset.Dunes, x, x * 0.8f, Amp, Seed);
                float raw = TerrainField.Fbm(x, x * 0.8f, Seed, p.octaves, p.wavelength)
                            * Mathf.Min(Amp, p.wavelength * 0.22f);
                diff += Mathf.Abs(warped - raw);
            }
            Assert.Greater(diff, 3f, "warped field matches raw fBM — domain warp is a no-op");
        }

        // ── Climate ───────────────────────────────────────────────────────────────────────────────

        [Test]
        public void Climate_IsDeterministicAndInRange()
        {
            for (float x = -300f; x <= 300f; x += 47f)
            {
                Vector2 c1 = TerrainField.Climate(x, -x, Seed);
                Vector2 c2 = TerrainField.Climate(x, -x, Seed);
                Assert.AreEqual(c1, c2);
                Assert.That(c1.x, Is.InRange(0f, 1f));
                Assert.That(c1.y, Is.InRange(0f, 1f));
            }
        }

        [Test]
        public void Climate_VariesAcrossTheWorld()
        {
            float minT = 1f, maxT = 0f, minM = 1f, maxM = 0f;
            for (float x = -400f; x <= 400f; x += 21f)
            {
                Vector2 c = TerrainField.Climate(x, x * 0.5f, Seed);
                minT = Mathf.Min(minT, c.x); maxT = Mathf.Max(maxT, c.x);
                minM = Mathf.Min(minM, c.y); maxM = Mathf.Max(maxM, c.y);
            }
            Assert.Greater(maxT - minT, 0.25f, "temperature field is flat");
            Assert.Greater(maxM - minM, 0.25f, "moisture field is flat");
        }

        [Test]
        public void Climate_TemperatureAndMoisture_AreIndependentFields()
        {
            float diff = 0f;
            for (float x = 0f; x < 400f; x += 19f)
            {
                Vector2 c = TerrainField.Climate(x, x * 0.4f, Seed);
                diff += Mathf.Abs(c.x - c.y);
            }
            Assert.Greater(diff, 2f, "temp and moisture are the same field");
        }
    }
}
