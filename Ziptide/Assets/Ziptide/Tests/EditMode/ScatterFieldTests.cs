using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// H5 ScatterField contracts (ARCHITECTURE V2): determinism, blue-noise spacing, radius
    /// containment, mask exclusion, density thinning, per-kind spacing, degenerate safety, and
    /// coverage (Poisson must FILL the disc, not fizzle). Pure, headless.
    /// </summary>
    public class ScatterFieldTests
    {
        private const float Radius = 120f;
        private const float Spacing = 14f;
        private const int Kinds = 4;
        private const int Seed = 1337;

        private static List<ScatterPoint> Gen(int seed = Seed, List<ScatterMask> masks = null,
            System.Func<float, float, float> density = null, float[] kindSpacings = null)
            => ScatterField.Generate(Radius, Spacing, Kinds, seed, masks, density, kindSpacings);

        [Test]
        public void SameSeed_IdenticalScatter()
        {
            var a = Gen();
            var b = Gen();
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i].Position, b[i].Position);
                Assert.AreEqual(a[i].Kind, b[i].Kind);
            }
        }

        [Test]
        public void DifferentSeeds_DifferentScatter()
        {
            var a = Gen(1);
            var b = Gen(2);
            bool differ = a.Count != b.Count;
            for (int i = 0; i < Mathf.Min(a.Count, b.Count) && !differ; i++)
                differ = a[i].Position != b[i].Position;
            Assert.IsTrue(differ);
        }

        [Test]
        public void AllPoints_InsideTheRadius()
        {
            foreach (var p in Gen())
                Assert.LessOrEqual(p.Position.magnitude, Radius + 0.001f);
        }

        [Test]
        public void BlueNoise_MinSpacingHolds_AllPairs()
        {
            var pts = Gen();
            for (int i = 0; i < pts.Count; i++)
                for (int j = i + 1; j < pts.Count; j++)
                    Assert.GreaterOrEqual((pts[i].Position - pts[j].Position).magnitude, Spacing - 0.001f,
                        "points " + i + "/" + j + " closer than the Poisson radius");
        }

        [Test]
        public void Coverage_ThePoissonActuallyFillsTheDisc()
        {
            // Theoretical max ≈ area / (π r²/4); Bridson typically lands ~50-70% of it. Require 25%.
            float theoretical = (Mathf.PI * Radius * Radius) / (Mathf.PI * Spacing * Spacing * 0.25f);
            Assert.Greater(Gen().Count, theoretical * 0.25f, "scatter fizzled — disc mostly empty");
        }

        [Test]
        public void Masks_ExcludeEverythingInsideThem()
        {
            var masks = new List<ScatterMask>
            {
                ScatterMask.Disc(new Vector2(0f, 0f), 30f),
                ScatterMask.Capsule(new Vector2(-100f, -100f), new Vector2(100f, 100f), 12f),
            };
            foreach (var p in Gen(masks: masks))
                Assert.IsFalse(ScatterField.Excluded(p.Position, masks),
                    "point " + p.Position + " inside an exclusion mask");
        }

        [Test]
        public void DensityZero_RegionStaysEmpty()
        {
            // Left half-plane density 0 — nothing may land there.
            var pts = Gen(density: (x, z) => x < 0f ? 0f : 1f);
            foreach (var p in pts)
                Assert.GreaterOrEqual(p.Position.x, 0f, "point spawned in a zero-density region");
            Assert.Greater(pts.Count, 10, "the live half should still be populated");
        }

        [Test]
        public void DensityChannel_ModulatesCounts()
        {
            var sparse = Gen(density: (x, z) => 0.15f);
            var dense = Gen(density: (x, z) => 0.95f);
            Assert.Greater(dense.Count, sparse.Count * 2,
                "density 0.95 vs 0.15 should produce far more points (" + dense.Count + " vs " + sparse.Count + ")");
        }

        [Test]
        public void PerKindSpacing_HoldsForTheConstrainedKind()
        {
            float[] kindSpacings = { 60f, 0f, 0f, 0f }; // kind 0 is rare-and-lonely
            var pts = Gen(kindSpacings: kindSpacings);
            for (int i = 0; i < pts.Count; i++)
                for (int j = i + 1; j < pts.Count; j++)
                    if (pts[i].Kind == 0 && pts[j].Kind == 0)
                        Assert.GreaterOrEqual((pts[i].Position - pts[j].Position).magnitude, 60f - 0.001f);
        }

        [Test]
        public void Kinds_AreWithinRange_AndAllRepresented()
        {
            var seen = new HashSet<int>();
            foreach (var p in Gen())
            {
                Assert.That(p.Kind, Is.InRange(0, Kinds - 1));
                seen.Add(p.Kind);
            }
            Assert.AreEqual(Kinds, seen.Count, "some prop kind never appears at uniform density");
        }

        [Test]
        public void DegenerateInputs_AreSafe()
        {
            Assert.IsEmpty(ScatterField.Generate(0f, Spacing, Kinds, Seed, null, null));
            Assert.IsEmpty(ScatterField.Generate(-5f, Spacing, Kinds, Seed, null, null));
            Assert.IsEmpty(ScatterField.Generate(Radius, 0f, Kinds, Seed, null, null));
            Assert.IsFalse(ScatterField.Excluded(Vector2.zero, null));
        }
    }
}
