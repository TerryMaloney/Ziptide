using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Transmission clarity-tier derivation contract (WORLD_DATA.md §3): fragments in → tier out,
    /// derived clarity flags stay consistent and only ever rise. Pure, headless.
    /// </summary>
    public class TransmissionProgressTests
    {
        private static PlayerProfile With(params string[] flags)
        {
            var p = new PlayerProfile();
            foreach (var f in flags) p.SetFlag(f);
            return p;
        }

        [Test]
        public void ComputeTier_FollowsTheFragmentSchedule()
        {
            Assert.AreEqual(0, TransmissionProgress.ComputeTier(new PlayerProfile()));
            Assert.AreEqual(1, TransmissionProgress.ComputeTier(With(ZiptideFlags.FRAGMENT_T1_FOUND)));
            Assert.AreEqual(2, TransmissionProgress.ComputeTier(With(
                ZiptideFlags.FRAGMENT_T1_FOUND, ZiptideFlags.FRAGMENT_T2_FOUND)));
            Assert.AreEqual(3, TransmissionProgress.ComputeTier(With(ZiptideFlags.FRAGMENT_T3_FOUND)),
                "T3 alone reaches tier 3 — clarity comes from the strongest fragment recovered");
            Assert.AreEqual(3, TransmissionProgress.ComputeTier(With(ZiptideFlags.FRAGMENT_T4_FOUND)));
        }

        [Test]
        public void ComputeTier_MaxNeedsAllFivePlusRillConfession()
        {
            var allButRill = With(
                ZiptideFlags.FRAGMENT_T1_FOUND, ZiptideFlags.FRAGMENT_T2_FOUND,
                ZiptideFlags.FRAGMENT_T3_FOUND, ZiptideFlags.FRAGMENT_T4_FOUND,
                ZiptideFlags.FRAGMENT_T5_FOUND);
            Assert.AreEqual(3, TransmissionProgress.ComputeTier(allButRill), "missing the RILL line");

            allButRill.SetFlag(ZiptideFlags.FRAGMENT_RILL_CONFESS);
            Assert.AreEqual(TransmissionProgress.TierMax, TransmissionProgress.ComputeTier(allButRill));
        }

        [Test]
        public void SyncClarityFlags_SetsCumulativeTiers_AndIsIdempotent()
        {
            var p = With(ZiptideFlags.FRAGMENT_T3_FOUND);

            Assert.AreEqual(3, TransmissionProgress.SyncClarityFlags(p), "tier 3 sets clarity 1+2+3");
            Assert.IsTrue(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_1));
            Assert.IsTrue(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_2));
            Assert.IsTrue(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_3));
            Assert.IsFalse(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_MAX));

            Assert.AreEqual(0, TransmissionProgress.SyncClarityFlags(p), "second sync is a no-op");
        }

        [Test]
        public void SyncClarityFlags_NeverClearsATier()
        {
            var p = With(ZiptideFlags.FRAGMENT_T2_FOUND);
            TransmissionProgress.SyncClarityFlags(p);
            Assert.IsTrue(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_2));

            // Even if fragment flags were somehow removed, clarity stays (memory doesn't re-garble).
            p.ClearFlag(ZiptideFlags.FRAGMENT_T2_FOUND);
            TransmissionProgress.SyncClarityFlags(p);
            Assert.IsTrue(p.HasFlag(ZiptideFlags.TRANSMISSION_CLARITY_2));
        }

        [Test]
        public void NullProfile_IsSafe()
        {
            Assert.AreEqual(0, TransmissionProgress.ComputeTier(null));
            Assert.AreEqual(0, TransmissionProgress.SyncClarityFlags(null));
        }
    }
}
