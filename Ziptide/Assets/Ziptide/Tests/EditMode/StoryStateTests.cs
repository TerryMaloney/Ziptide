using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Signal-tier and RILL-memory-state derivation contracts (GAME_PLAN M1): story flags in →
    /// one consistent number/state out. Pure, headless.
    /// </summary>
    public class StoryStateTests
    {
        private static PlayerProfile With(params string[] flags)
        {
            var p = new PlayerProfile();
            foreach (var f in flags) p.SetFlag(f);
            return p;
        }

        // ── SignalState ─────────────────────────────────────────────────────

        [Test]
        public void Signal_Tier_FollowsTheThresholdSchedule()
        {
            Assert.AreEqual(0, SignalState.Tier(new PlayerProfile()));
            Assert.AreEqual(0, SignalState.Tier(null));
            Assert.AreEqual(1, SignalState.Tier(With(ZiptideFlags.SIGNAL_THRESHOLD_1)));
            Assert.AreEqual(2, SignalState.Tier(With(ZiptideFlags.SIGNAL_THRESHOLD_1, ZiptideFlags.SIGNAL_THRESHOLD_2)));
            Assert.AreEqual(3, SignalState.Tier(With(ZiptideFlags.SIGNAL_THRESHOLD_3)));
            Assert.AreEqual(SignalState.TierMax, SignalState.Tier(With(ZiptideFlags.SIGNAL_MAX)));
        }

        [Test]
        public void Signal_HighestGrantedFlagWins_EvenWithGaps()
        {
            // A missing intermediate flag must never understate the tier.
            Assert.AreEqual(2, SignalState.Tier(With(ZiptideFlags.SIGNAL_THRESHOLD_2)));
            Assert.AreEqual(SignalState.TierMax, SignalState.Tier(With(ZiptideFlags.SIGNAL_MAX, ZiptideFlags.SIGNAL_THRESHOLD_1)));
        }

        // ── RillState ───────────────────────────────────────────────────────

        [Test]
        public void Rill_StartsDormant_AndStirsAfterW004()
        {
            Assert.AreEqual(RillMemoryState.Dormant, RillState.Compute(null));
            Assert.AreEqual(RillMemoryState.Dormant, RillState.Compute(new PlayerProfile()));
            Assert.AreEqual(RillMemoryState.Dormant, RillState.Compute(With(ZiptideFlags.W002_COMPLETE)));
            Assert.AreEqual(RillMemoryState.Stirring, RillState.Compute(With(ZiptideFlags.W004_COMPLETE)));
        }

        [Test]
        public void Rill_ArcAdvances_OnChapterCapstones()
        {
            Assert.AreEqual(RillMemoryState.Remembering,
                RillState.Compute(With(ZiptideFlags.W004_COMPLETE, ZiptideFlags.C2_CONTAINMENT_REVEALED)));
            Assert.AreEqual(RillMemoryState.Remembering,
                RillState.Compute(With(ZiptideFlags.C3_W013_MEMORY_SHARD)));
            Assert.AreEqual(RillMemoryState.Unsealing,
                RillState.Compute(With(ZiptideFlags.W028_COMPLETE)));
            Assert.AreEqual(RillMemoryState.Unsealing,
                RillState.Compute(With(ZiptideFlags.C4_W028_NO_JOB)));
            Assert.AreEqual(RillMemoryState.Integrated,
                RillState.Compute(With(ZiptideFlags.C6_W051_RILL_NAMED)));
        }

        [Test]
        public void Rill_HigherState_WinsOverLower()
        {
            // The arc never regresses: capstone flags accumulate; the furthest one decides.
            var p = With(ZiptideFlags.W004_COMPLETE, ZiptideFlags.C2_CONTAINMENT_REVEALED,
                         ZiptideFlags.W028_COMPLETE, ZiptideFlags.C6_W051_RILL_NAMED);
            Assert.AreEqual(RillMemoryState.Integrated, RillState.Compute(p));
        }

        [Test]
        public void Rill_EndingFlags_AreTerminal()
        {
            Assert.AreEqual(RillMemoryState.EndgameA,
                RillState.Compute(With(ZiptideFlags.C6_W051_RILL_NAMED, ZiptideFlags.C12_W063_ENDING_A)));
            Assert.AreEqual(RillMemoryState.EndgameB, RillState.Compute(With(ZiptideFlags.C12_W063_ENDING_B)));
            Assert.AreEqual(RillMemoryState.EndgameC, RillState.Compute(With(ZiptideFlags.C12_W063_ENDING_C)));
            Assert.AreEqual(RillMemoryState.EndgameD, RillState.Compute(With(ZiptideFlags.C12_W063_ENDING_D)));
        }
    }
}
