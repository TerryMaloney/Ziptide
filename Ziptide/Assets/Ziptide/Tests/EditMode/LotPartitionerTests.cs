using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>ARCHITECTURE V2 Q2a — the lot partitioner's contracts: determinism, containment,
    /// no overlap, min area, the frontage guarantee, and the aspect law.</summary>
    public class LotPartitionerTests
    {
        private static readonly Rect District = new Rect(-30f, -24f, 60f, 48f);
        private const float Street = 4f;
        private const float MinArea = 60f;
        private const float MaxAspect = 3f;

        private static List<Lot> Run(int seed = 77) =>
            LotPartitioner.Partition(District, Street, MinArea, MaxAspect, seed);

        [Test]
        public void SameSeed_IdenticalLots()
        {
            var a = Run(); var b = Run();
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i].Bounds, b[i].Bounds, "lot " + i);
                Assert.AreEqual(a[i].FrontS, b[i].FrontS);
                Assert.AreEqual(a[i].FrontE, b[i].FrontE);
            }
        }

        [Test]
        public void DifferentSeeds_DifferentLayouts()
        {
            var a = Run(77); var b = Run(78);
            bool differs = a.Count != b.Count;
            for (int i = 0; !differs && i < a.Count; i++) differs = a[i].Bounds != b[i].Bounds;
            Assert.IsTrue(differs, "two seeds should not produce the same city block");
        }

        [Test]
        public void ProducesMultipleLots_InsideTheBlock()
        {
            var lots = Run();
            Assert.GreaterOrEqual(lots.Count, 4, "a 60x48 district should yield a real block");
            var block = new Rect(District.x + Street, District.y + Street,
                District.width - 2 * Street, District.height - 2 * Street);
            foreach (var l in lots)
            {
                Assert.GreaterOrEqual(l.Bounds.xMin, block.xMin - 0.001f);
                Assert.GreaterOrEqual(l.Bounds.yMin, block.yMin - 0.001f);
                Assert.LessOrEqual(l.Bounds.xMax, block.xMax + 0.001f);
                Assert.LessOrEqual(l.Bounds.yMax, block.yMax + 0.001f);
            }
        }

        [Test]
        public void NoLotOverlaps()
        {
            var lots = Run();
            for (int i = 0; i < lots.Count; i++)
                for (int j = i + 1; j < lots.Count; j++)
                {
                    var a = lots[i].Bounds; var b = lots[j].Bounds;
                    bool overlap = a.xMin < b.xMax - 0.001f && b.xMin < a.xMax - 0.001f
                                && a.yMin < b.yMax - 0.001f && b.yMin < a.yMax - 0.001f;
                    Assert.IsFalse(overlap, "lots " + i + " and " + j + " overlap");
                }
        }

        [Test]
        public void EveryLot_MeetsMinArea_AndMinSide()
        {
            foreach (var l in Run())
            {
                Assert.GreaterOrEqual(l.Area, MinArea - 0.001f, "lot " + l.Bounds);
                Assert.GreaterOrEqual(l.Bounds.width, LotPartitioner.MinSide - 0.001f);
                Assert.GreaterOrEqual(l.Bounds.height, LotPartitioner.MinSide - 0.001f);
            }
        }

        [Test]
        public void FrontageGuarantee_EveryLotFrontsAStreet()
        {
            foreach (var l in Run())
                Assert.IsTrue(l.HasFrontage, "landlocked lot at " + l.Bounds + " — the street-cut rule failed");
        }

        [Test]
        public void AspectLaw_ViolationsOnlyWhenUnsplittable()
        {
            // A lot may only be too narrow when no legal cut existed: too small to split, or the
            // shortest legal child (max of MinSide and MinArea/shortSide) doesn't fit half the
            // street-cut usable length.
            foreach (var l in Run())
            {
                if (l.Aspect <= MaxAspect) continue;
                float shortS = Mathf.Min(l.Bounds.width, l.Bounds.height);
                float longS = Mathf.Max(l.Bounds.width, l.Bounds.height);
                float req = Mathf.Max(LotPartitioner.MinSide, MinArea / shortS);
                bool couldCut = l.Area >= 2f * MinArea && req <= 0.5f * (longS - Street);
                Assert.IsFalse(couldCut,
                    "lot " + l.Bounds + " (aspect " + l.Aspect.ToString("F1") + ") had a legal cut but wasn't split");
            }
        }

        [Test]
        public void FrontageGuarantee_HoldsAcrossManySeeds()
        {
            for (int seed = 1; seed <= 25; seed++)
                foreach (var l in Run(seed))
                    Assert.IsTrue(l.HasFrontage, "seed " + seed + " produced a landlocked lot");
        }

        [Test]
        public void TinyDistrict_YieldsNothing_NotGarbage()
        {
            var lots = LotPartitioner.Partition(new Rect(0, 0, 8f, 8f), Street, MinArea, MaxAspect, 5);
            Assert.IsEmpty(lots);
        }

        [Test]
        public void ZeroStreetWidth_StillPartitions()
        {
            var lots = LotPartitioner.Partition(District, 0f, MinArea, MaxAspect, 9);
            Assert.GreaterOrEqual(lots.Count, 2);
            foreach (var l in lots) Assert.IsTrue(l.HasFrontage);
        }

        [Test]
        public void AreaConservation_LotsPlusStreetsFitTheBlock()
        {
            var lots = Run();
            float lotArea = 0f;
            foreach (var l in lots) lotArea += l.Area;
            var block = (District.width - 2 * Street) * (District.height - 2 * Street);
            Assert.LessOrEqual(lotArea, block + 0.01f, "lots exceed the block — geometry leak");
            Assert.Greater(lotArea, block * 0.5f, "more than half the block vanished into streets");
        }
    }
}
