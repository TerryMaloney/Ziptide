#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.5 — the VFX rails. These prove no recipe can ever ask the frame for more than the
    /// budget allows: the caps hold, inverted ranges self-repair, and the starter vocabulary is
    /// complete and unique. This is the "budgets the effects never had" contract, in CI.
    /// </summary>
    public class VfxRecipeTests
    {
        private static readonly string[] ExpectedIds =
        {
            "impact_metal", "impact_stone", "muzzle_taser",
            "steam_vent", "motes_amber", "motes_spore", "sparks_short",
        };

        [Test]
        public void Library_HasAllSevenStarters_WithUniqueIds()
        {
            var ids = VfxLibrary.All().Select(r => r.id).ToList();
            Assert.AreEqual(ExpectedIds.Length, ids.Count, "starter count");
            CollectionAssert.AreEquivalent(ExpectedIds, ids, "the canon starter ids");
            Assert.AreEqual(ids.Count, ids.Distinct().Count(), "ids must be unique");
        }

        [Test]
        public void Get_KnownId_Resolves_UnknownReturnsNull()
        {
            Assert.IsNotNull(VfxLibrary.Get("impact_metal"));
            Assert.IsNull(VfxLibrary.Get("does_not_exist"));
            Assert.IsNull(VfxLibrary.Get(null));
            Assert.IsNull(VfxLibrary.Get(""));
        }

        [Test]
        public void EveryRecipe_RespectsTheHardCaps()
        {
            foreach (var r in VfxLibrary.All())
            {
                Assert.LessOrEqual(r.PeakParticles(), VfxRecipeDefinition.MaxParticles,
                    r.id + " peaks above the particle cap");
                Assert.LessOrEqual(r.burstCount, VfxRecipeDefinition.MaxParticles, r.id + " burst over cap");
                Assert.GreaterOrEqual(r.PeakParticles(), 1, r.id + " emits nothing — pointless recipe");
            }
        }

        [Test]
        public void EveryRecipe_HasSaneOrderedRanges()
        {
            foreach (var r in VfxLibrary.All())
            {
                Assert.LessOrEqual(r.sizeMin, r.sizeMax, r.id + " size range inverted");
                Assert.LessOrEqual(r.speedMin, r.speedMax, r.id + " speed range inverted");
                Assert.LessOrEqual(r.lifetimeMin, r.lifetimeMax, r.id + " lifetime range inverted");
                Assert.Greater(r.sizeMin, 0f, r.id + " zero size");
                Assert.Greater(r.lifetimeMin, 0f, r.id + " zero lifetime");
                Assert.That(r.gravity, Is.InRange(-1f, 1f), r.id + " gravity out of rail");
            }
        }

        [Test]
        public void Validate_ClampsAnOversizedBurst_ToTheCap()
        {
            var r = new VfxRecipeDefinition { burstCount = 500 }.Validate();
            Assert.AreEqual(VfxRecipeDefinition.MaxParticles, r.burstCount);
        }

        [Test]
        public void Validate_RepairsInvertedRanges_AndOutOfRailGravity()
        {
            var r = new VfxRecipeDefinition
            {
                sizeMin = 0.5f, sizeMax = 0.1f,
                speedMin = 5f, speedMax = 1f,
                lifetimeMin = 3f, lifetimeMax = 0.5f,
                gravity = 9f,
            }.Validate();

            Assert.LessOrEqual(r.sizeMin, r.sizeMax);
            Assert.LessOrEqual(r.speedMin, r.speedMax);
            Assert.LessOrEqual(r.lifetimeMin, r.lifetimeMax);
            Assert.AreEqual(1f, r.gravity, "gravity clamps into [-1,1]");
        }

        [Test]
        public void EveryKindExceptDripsIsExercised_ByTheStarterSet()
        {
            var kinds = VfxLibrary.All().Select(r => r.kind).Distinct().ToList();
            foreach (var k in new[] { VfxKind.Impact, VfxKind.Muzzle, VfxKind.SteamVent, VfxKind.Motes, VfxKind.Sparks })
                CollectionAssert.Contains(kinds, k, k + " has no starter recipe");
        }

        [Test]
        public void ContinuousRecipes_LoopAndOneShotsBurst()
        {
            // A oneShot recipe bursts (has a burstCount); a looping recipe emits over time (has a rate).
            foreach (var r in VfxLibrary.All())
            {
                if (r.oneShot) Assert.Greater(r.burstCount, 0, r.id + " one-shot with no burst");
                else Assert.Greater(r.rateOverTime, 0f, r.id + " looping with no emission rate");
            }
        }
    }
}
#endif
