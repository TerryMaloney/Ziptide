using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The POI quality-gate heuristics (Quality Bar P1c/P1f). These numbers ARE the bar the audit
    /// enforces — the tests pin them so a future tuning session changes them deliberately, not by
    /// accident. Pure, headless.
    /// </summary>
    public class PoiQualityTests
    {
        private static PoiDef Poi(string id, PoiType type, float x, float z, int tier = 0)
            => new PoiDef { id = id, type = type, position = new Vector3(x, 0f, z), tier = tier };

        /// <summary>The standard-ring shape every experience world seeds with (8 POIs / 7 verbs).</summary>
        private static List<PoiDef> StandardishRing()
        {
            return new List<PoiDef>
            {
                Poi("story", PoiType.StoryAnchor, 0f, 105f),
                Poi("camp_a", PoiType.CombatCamp, 114f, 53f),
                Poi("camp_b", PoiType.CombatCamp, -152f, -55f, 1),
                Poi("grove", PoiType.HarvestGrove, 56f, -97f),
                Poi("works", PoiType.MachineSite, -90f, 107f),
                Poi("ruin", PoiType.RuinCache, 140f, -65f),
                Poi("cave", PoiType.CaveSecret, -80f, -170f, 1),
                Poi("berth", PoiType.TravelBerth, 0f, 20f),
            };
        }

        [Test]
        public void StandardRing_PassesEveryGateByConstruction()
        {
            var ring = StandardishRing();
            Assert.GreaterOrEqual(ring.Count, 5, "POI_COUNT_LOW would fire");
            Assert.GreaterOrEqual(PoiQuality.DistinctVerbCount(ring), 3, "VERB_VARIETY_LOW would fire");
            Assert.IsTrue(PoiQuality.HasStoryAnchor(ring), "STORY_ANCHOR_MISSING would fire");
            Assert.GreaterOrEqual(PoiQuality.EstimatePlayMinutes(ring, Vector3.zero), 8f,
                "EST_PLAY_MINUTES_LOW would fire");
        }

        [Test]
        public void TinySameyWorld_FailsTheBar()
        {
            var bland = new List<PoiDef>
            {
                Poi("a", PoiType.RuinCache, 5f, 5f),
                Poi("b", PoiType.RuinCache, -5f, 5f),
            };
            Assert.Less(bland.Count, 5);
            Assert.Less(PoiQuality.DistinctVerbCount(bland), 3);
            Assert.IsFalse(PoiQuality.HasStoryAnchor(bland));
            Assert.Less(PoiQuality.EstimatePlayMinutes(bland, Vector3.zero), 8f);
        }

        [Test]
        public void EstimatePlayMinutes_GrowsWithTierAndDistance()
        {
            var near = new List<PoiDef> { Poi("a", PoiType.CombatCamp, 10f, 0f) };
            var far = new List<PoiDef> { Poi("a", PoiType.CombatCamp, 280f, 0f) };
            var buff = new List<PoiDef> { Poi("a", PoiType.CombatCamp, 10f, 0f, 2) };
            Assert.Greater(PoiQuality.EstimatePlayMinutes(far, Vector3.zero),
                           PoiQuality.EstimatePlayMinutes(near, Vector3.zero));
            Assert.Greater(PoiQuality.EstimatePlayMinutes(buff, Vector3.zero),
                           PoiQuality.EstimatePlayMinutes(near, Vector3.zero));
        }

        [Test]
        public void DegenerateInputs_AreSafe()
        {
            Assert.AreEqual(0, PoiQuality.DistinctVerbCount(null));
            Assert.AreEqual(0f, PoiQuality.EstimatePlayMinutes(null, Vector3.zero));
            Assert.IsFalse(PoiQuality.HasStoryAnchor(null));
            Assert.AreEqual(0f, PoiQuality.PoiMinutes(null));
            var withNull = new List<PoiDef> { null, Poi("a", PoiType.StoryAnchor, 0f, 0f) };
            Assert.AreEqual(1, PoiQuality.DistinctVerbCount(withNull));
            Assert.IsTrue(PoiQuality.HasStoryAnchor(withNull));
        }

        [Test]
        public void DuplicatePoiIds_FailLayoutValidation()
        {
            var kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.districts.Add(new DistrictDef { id = "A" });
            kit.pois.Add(Poi("dup", PoiType.RuinCache, 0f, 0f));
            kit.pois.Add(Poi("dup", PoiType.CombatCamp, 10f, 0f));
            CollectionAssert.IsNotEmpty(kit.Validate());
        }
    }
}
