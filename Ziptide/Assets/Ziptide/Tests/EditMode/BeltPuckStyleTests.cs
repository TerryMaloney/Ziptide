using NUnit.Framework;
using Ziptide.Content.Automation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1j (LAW 6) — pins the puck-style contracts: styling is DETERMINISTIC (same
    /// resource = same look on every device and session — string.GetHashCode is neither, FNV-1a
    /// is), colors stay inside the readable band, and the live economy's common resources don't all
    /// collapse onto one shape (the catalog-breadth spirit of the law).
    /// </summary>
    public class BeltPuckStyleTests
    {
        [Test]
        public void Style_IsDeterministic_AndNullSafe()
        {
            Assert.AreEqual(BeltPuckStyle.Hash("scrap"), BeltPuckStyle.Hash("scrap"));
            Assert.AreEqual(BeltPuckStyle.ShapeIndex("mineral"), BeltPuckStyle.ShapeIndex("mineral"));
            Assert.DoesNotThrow(() => BeltPuckStyle.ShapeIndex(null));
            BeltPuckStyle.ColorOf("scrap", out float h1, out float s1, out float v1);
            BeltPuckStyle.ColorOf("scrap", out float h2, out float s2, out float v2);
            Assert.AreEqual(h1, h2); Assert.AreEqual(s1, s2); Assert.AreEqual(v1, v2);
        }

        [Test]
        public void Shapes_AreInRange_AndNotAllTheSame()
        {
            string[] common = { "scrap", "mineral", "credits", "ore", "spore", "resin", "alloy", "gem" };
            var seen = new System.Collections.Generic.HashSet<int>();
            foreach (var id in common)
            {
                int s = BeltPuckStyle.ShapeIndex(id);
                Assert.GreaterOrEqual(s, 0);
                Assert.Less(s, BeltPuckStyle.ShapeCount);
                seen.Add(s);
            }
            Assert.Greater(seen.Count, 1, "the common resources must not all ride as one shape");
        }

        [Test]
        public void Colors_StayInsideTheReadableBand()
        {
            string[] ids = { "scrap", "mineral", "ore", "x", "", "a_very_long_resource_identifier" };
            foreach (var id in ids)
            {
                BeltPuckStyle.ColorOf(id, out float h, out float s, out float v);
                Assert.That(h, Is.InRange(0f, 1f), id);
                Assert.That(s, Is.InRange(0.45f, 0.80f), id + " saturation band");
                Assert.That(v, Is.InRange(0.70f, 0.95f), id + " value band — always readable on dark tiles");
            }
        }
    }
}
