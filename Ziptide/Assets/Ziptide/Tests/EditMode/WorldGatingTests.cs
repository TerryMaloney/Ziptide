using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Story-gating contract tests for WorldPackDefinition flagsRequired/flagsGranted: a world is
    /// available only when all its required flags are set, and completing it grants its world-level
    /// flags. Pure, headless — no scene, no device.
    /// </summary>
    public class WorldGatingTests
    {
        private static WorldPackDefinition Pack(string[] required = null, string[] granted = null)
        {
            var p = ScriptableObject.CreateInstance<WorldPackDefinition>();
            p.packId = "w004_broadcast_tomb";
            if (required != null) p.flagsRequired.AddRange(required);
            if (granted != null) p.flagsGranted.AddRange(granted);
            return p;
        }

        // ── MeetsRequirements ────────────────────────────────────────────────
        [Test]
        public void MeetsRequirements_EmptyList_AlwaysMet()
        {
            Assert.IsTrue(WorldGating.MeetsRequirements(Pack(), new PlayerProfile()));
        }

        [Test]
        public void MeetsRequirements_AllFlagsSet_True()
        {
            var profile = new PlayerProfile();
            profile.SetFlag("W001_COMPLETE");
            var pack = Pack(required: new[] { "W001_COMPLETE" });
            Assert.IsTrue(WorldGating.MeetsRequirements(pack, profile));
        }

        [Test]
        public void MeetsRequirements_MissingAnyFlag_False()
        {
            var profile = new PlayerProfile();
            profile.SetFlag("W001_COMPLETE");
            var pack = Pack(required: new[] { "W001_COMPLETE", "W002_COMPLETE" });
            Assert.IsFalse(WorldGating.MeetsRequirements(pack, profile));
        }

        [Test]
        public void MeetsRequirements_BlankEntriesDontGate()
        {
            var profile = new PlayerProfile();
            var pack = Pack(required: new[] { "", null });
            Assert.IsTrue(WorldGating.MeetsRequirements(pack, profile));
        }

        [Test]
        public void MeetsRequirements_NullProfile_FailsClosedOnRealRequirement()
        {
            Assert.IsFalse(WorldGating.MeetsRequirements(Pack(required: new[] { "W001_COMPLETE" }), null));
            Assert.IsTrue(WorldGating.MeetsRequirements(Pack(), null), "no requirement => still met");
        }

        // ── FirstMissingRequirement ──────────────────────────────────────────
        [Test]
        public void FirstMissingRequirement_ReturnsFirstUnset_OrNull()
        {
            var profile = new PlayerProfile();
            profile.SetFlag("W001_COMPLETE");
            var pack = Pack(required: new[] { "W001_COMPLETE", "W002_COMPLETE" });
            Assert.AreEqual("W002_COMPLETE", WorldGating.FirstMissingRequirement(pack, profile));

            profile.SetFlag("W002_COMPLETE");
            Assert.IsNull(WorldGating.FirstMissingRequirement(pack, profile));
        }

        // ── GrantWorldFlags ──────────────────────────────────────────────────
        [Test]
        public void GrantWorldFlags_SetsAll_ReturnsNewlySetCount()
        {
            var profile = new PlayerProfile();
            var pack = Pack(granted: new[] { "C1_W001_RILL_BOOT", "SIGNAL_THRESHOLD_1", "W001_COMPLETE" });

            int n = WorldGating.GrantWorldFlags(pack, profile);

            Assert.AreEqual(3, n);
            Assert.IsTrue(profile.HasFlag("C1_W001_RILL_BOOT"));
            Assert.IsTrue(profile.HasFlag("SIGNAL_THRESHOLD_1"));
            Assert.IsTrue(profile.HasFlag("W001_COMPLETE"));
        }

        [Test]
        public void GrantWorldFlags_Idempotent_CountsOnlyNew()
        {
            var profile = new PlayerProfile();
            profile.SetFlag("W001_COMPLETE");
            var pack = Pack(granted: new[] { "W001_COMPLETE", "SIGNAL_THRESHOLD_1" });

            Assert.AreEqual(1, WorldGating.GrantWorldFlags(pack, profile), "already-set flag not recounted");
            Assert.AreEqual(0, WorldGating.GrantWorldFlags(pack, profile), "second grant is a no-op");
            Assert.AreEqual(2, profile.flags.Count);
        }

        [Test]
        public void GrantWorldFlags_SkipsBlanks_AndIsNullSafe()
        {
            var profile = new PlayerProfile();
            Assert.AreEqual(0, WorldGating.GrantWorldFlags(Pack(granted: new[] { "", null }), profile));
            Assert.AreEqual(0, profile.flags.Count);
            Assert.DoesNotThrow(() => WorldGating.GrantWorldFlags(null, profile));
            Assert.DoesNotThrow(() => WorldGating.GrantWorldFlags(Pack(granted: new[] { "x" }), null));
        }
    }
}
