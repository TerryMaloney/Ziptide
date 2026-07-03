using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE NORTH-STAR regression test (reconciliation R3): "we changed the story direction of this
    /// world" → affected assets land in the right buckets — safe-auto / review / breaking /
    /// deprecated — deterministically. Pure; no AssetDatabase.
    /// </summary>
    public class ForgeStalenessTests
    {
        private static List<ForgeAssetInfo> World()
        {
            return new List<ForgeAssetInfo>
            {
                new ForgeAssetInfo { id = "taser_gun_mk1", refs = new[] { "toxiccity", "glow_teal", "salvage" },
                    state = ForgeQualityState.ProxyPlus, hero = false, hasConsumers = true },
                new ForgeAssetInfo { id = "hero_gate_key", refs = new[] { "toxiccity", "glow_teal" },
                    state = ForgeQualityState.ProductionCandidate, hero = true, hasConsumers = true },
                new ForgeAssetInfo { id = "locked_relic", refs = new[] { "glow_teal" },
                    state = ForgeQualityState.Locked, hero = false, hasConsumers = true },
                new ForgeAssetInfo { id = "old_crate", refs = new[] { "toxiccity" },
                    state = ForgeQualityState.Proxy, hero = false, hasConsumers = false },
                new ForgeAssetInfo { id = "desert_prop", refs = new[] { "w006_mirrorflats" },
                    state = ForgeQualityState.Proxy, hero = false, hasConsumers = true },
            };
        }

        [Test]
        public void ChangedToken_BucketsExactlyPerTheContract()
        {
            var buckets = ForgeStaleness.Affected(World(), "glow_teal");
            CollectionAssert.AreEqual(new[] { "taser_gun_mk1" }, buckets[ForgeStaleness.Bucket.SafeAuto],
                "unlocked non-hero proxies are safe to regenerate");
            CollectionAssert.AreEqual(new[] { "hero_gate_key", "locked_relic" }, buckets[ForgeStaleness.Bucket.Review],
                "hero + locked assets need a human before anything regenerates");
            Assert.IsEmpty(buckets[ForgeStaleness.Bucket.Breaking]);
            Assert.IsEmpty(buckets[ForgeStaleness.Bucket.Deprecated]);
        }

        [Test]
        public void ChangedWorld_FindsItsAssets_AndFlagsTheOrphan()
        {
            var buckets = ForgeStaleness.Affected(World(), "ToxicCity"); // case-insensitive
            CollectionAssert.AreEqual(new[] { "taser_gun_mk1" }, buckets[ForgeStaleness.Bucket.SafeAuto]);
            CollectionAssert.AreEqual(new[] { "hero_gate_key" }, buckets[ForgeStaleness.Bucket.Review]);
            CollectionAssert.AreEqual(new[] { "old_crate" }, buckets[ForgeStaleness.Bucket.Deprecated],
                "nothing consumes it — deprecation candidate, never auto-deleted");
        }

        [Test]
        public void RenamingAConsumedAssetId_IsBreaking()
        {
            var buckets = ForgeStaleness.Affected(World(), "taser_gun_mk1");
            CollectionAssert.AreEqual(new[] { "taser_gun_mk1" }, buckets[ForgeStaleness.Bucket.Breaking],
                "consumed ids are seams — changing one is flagged, never auto-acted");
        }

        [Test]
        public void UnrelatedChange_AffectsNothing()
        {
            var buckets = ForgeStaleness.Affected(World(), "w012_void_rules");
            foreach (var kv in buckets) Assert.IsEmpty(kv.Value, kv.Key.ToString());
        }

        [Test]
        public void Output_IsDeterministicallyOrdered()
        {
            var a = ForgeStaleness.Affected(World(), "glow_teal");
            var reversed = World(); reversed.Reverse();
            var b = ForgeStaleness.Affected(reversed, "glow_teal");
            foreach (ForgeStaleness.Bucket bucket in System.Enum.GetValues(typeof(ForgeStaleness.Bucket)))
                CollectionAssert.AreEqual(a[bucket], b[bucket], "input order must not change output order");
        }
    }
}
