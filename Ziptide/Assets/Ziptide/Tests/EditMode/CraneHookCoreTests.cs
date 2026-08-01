using UnityEngine;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// A moving object in a static yard is worth more than five props — and a moving object at the
    /// wrong speed is worth less than none, because it reads as a bug rather than as machinery. The
    /// eye is extremely good at judging whether something heavy is moving believably, and it gets it
    /// wrong in exactly one direction: too fast.
    /// </summary>
    public class CraneHookCoreTests
    {
        private const float Min = CraneHookCore.DefaultMinDrop;
        private const float Max = CraneHookCore.DefaultMaxDrop;
        private const float Period = CraneHookCore.DefaultPeriod;

        [Test]
        public void TheShippedLoopIsSlowEnoughToReadAsWeight()
        {
            Assert.IsTrue(CraneHookCore.IsCalm(Min, Max, Period),
                "peak speed " + CraneHookCore.PeakSpeed(Min, Max, Period).ToString("F2")
                + " m/s is over the believable limit — a loaded hook creeps.");
        }

        [Test]
        public void AFasterLoopIsRejected_SoNobodyTunesItByFeelOnAMonitor()
        {
            // Same travel, a third of the time. This is exactly the tweak that looks fine on a desk
            // and looks like a broken animation in a headset.
            Assert.IsFalse(CraneHookCore.IsCalm(Min, Max, Period / 3f));
        }

        [Test]
        public void TheHookStaysInsideItsTravel()
        {
            for (int i = 0; i <= 120; i++)
            {
                float d = CraneHookCore.DropAt(i * 0.5f, Min, Max, Period);
                Assert.GreaterOrEqual(d, Min - 0.001f);
                Assert.LessOrEqual(d, Max + 0.001f);
            }
        }

        [Test]
        public void TheReversalIsEased_NotSnapped()
        {
            // At the bottom of the travel a triangle wave changes direction instantly. Sampling either
            // side of the turn, an eased loop has moved almost nothing; a snapped one has moved a lot.
            float half = Period * 0.5f;
            float dt = Period * 0.02f;
            float before = CraneHookCore.DropAt(half - dt, Min, Max, Period);
            float at = CraneHookCore.DropAt(half, Min, Max, Period);
            float after = CraneHookCore.DropAt(half + dt, Min, Max, Period);

            Assert.AreEqual(before, after, 0.001f, "the loop is symmetric about its turn");
            Assert.Less(Mathf.Abs(at - before), (Max - Min) * 0.02f,
                "the hook lurches at the reversal — that is the tell that nothing here has mass");
        }

        [Test]
        public void ItLoopsSeamlessly_SoThereIsNoVisibleRestart()
        {
            Assert.AreEqual(CraneHookCore.DropAt(0f, Min, Max, Period),
                CraneHookCore.DropAt(Period, Min, Max, Period), 0.001f);
            Assert.AreEqual(CraneHookCore.DropAt(3f, Min, Max, Period),
                CraneHookCore.DropAt(Period * 4f + 3f, Min, Max, Period), 0.001f);
        }

        [Test]
        public void ItActuallyMoves_BecauseAStillHookIsJustAProp()
        {
            float lo = CraneHookCore.DropAt(0f, Min, Max, Period);
            float hi = CraneHookCore.DropAt(Period * 0.5f, Min, Max, Period);
            Assert.Greater(Mathf.Abs(hi - lo), 2f, "under two metres of travel is not visible at 16 m up");
        }

        [Test]
        public void TheCableAlwaysSpansJibToHook()
        {
            // The failure: a fixed-length cable detaches from one end. A floating cable end is more
            // distracting than no motion at all.
            for (int i = 0; i <= 60; i++)
            {
                float d = CraneHookCore.DropAt(i, Min, Max, Period);
                CraneHookCore.CableSpan(d, out float centre, out float length);
                Assert.AreEqual(d, length, 0.001f, "the cable is exactly as long as the hook has dropped");
                Assert.AreEqual(0f, centre + length * 0.5f, 0.001f, "its top stays on the jib");
                Assert.AreEqual(-d, centre - length * 0.5f, 0.001f, "its bottom stays on the hook");
            }
        }

        [Test]
        public void ADegeneratePeriodParksTheHookInsteadOfDividingByZero()
        {
            Assert.AreEqual(Min, CraneHookCore.DropAt(7f, Min, Max, 0f), 0.001f);
            Assert.AreEqual(0f, CraneHookCore.PeakSpeed(Min, Max, 0f), 0.001f);
        }
    }
}
