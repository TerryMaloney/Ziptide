using System;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Hardwiring 1.5 catalog laws: the verb catalog meets the board's ≥12 bar, EVERY verb is worth
    /// engagement minutes (a future verb can't silently be worth nothing), and the original seven
    /// keep their exact enum values — authored layout assets serialize PoiType BY VALUE, so a shift
    /// would retype every POI in every world.
    /// </summary>
    public class PoiCatalogTests
    {
        [Test]
        public void Catalog_HasAtLeastTwelveVerbs()
        {
            Assert.GreaterOrEqual(Enum.GetValues(typeof(PoiType)).Length, 12,
                "The board's 1.5 bar: ≥12 POI types stamped.");
        }

        [Test]
        public void EveryVerb_IsWorthEngagementMinutes()
        {
            foreach (PoiType type in Enum.GetValues(typeof(PoiType)))
            {
                var poi = new PoiDef { id = "t", type = type };
                Assert.Greater(PoiQuality.PoiMinutes(poi), 0f,
                    type + " must be worth some engagement minutes — no verb is free.");
            }
        }

        [Test]
        public void OriginalSevenVerbs_KeepTheirSerializedValues()
        {
            Assert.AreEqual(0, (int)PoiType.CombatCamp);
            Assert.AreEqual(1, (int)PoiType.HarvestGrove);
            Assert.AreEqual(2, (int)PoiType.MachineSite);
            Assert.AreEqual(3, (int)PoiType.RuinCache);
            Assert.AreEqual(4, (int)PoiType.CaveSecret);
            Assert.AreEqual(5, (int)PoiType.StoryAnchor);
            Assert.AreEqual(6, (int)PoiType.TravelBerth);
        }

        [Test]
        public void DistinctVerbCount_SeesTheNewVerbs()
        {
            var pois = new System.Collections.Generic.List<PoiDef>
            {
                new PoiDef { type = PoiType.Market },
                new PoiDef { type = PoiType.Shrine },
                new PoiDef { type = PoiType.RepairBay },
                new PoiDef { type = PoiType.Transit },
                new PoiDef { type = PoiType.Lookout },
                new PoiDef { type = PoiType.Market }, // duplicate — counted once
            };
            Assert.AreEqual(5, PoiQuality.DistinctVerbCount(pois));
        }
    }
}
