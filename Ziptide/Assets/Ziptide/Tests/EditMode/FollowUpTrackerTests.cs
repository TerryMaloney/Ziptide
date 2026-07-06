using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    public class FollowUpTrackerTests
    {
        private static RillLine Line(string id, int delay) =>
            new RillLine { id = id, trigger = RillTrigger.FollowUp, key = "SOME_FLAG", text = "...", crossingsDelay = delay };

        [Test]
        public void FiresExactlyOnDelayedTick_NotBefore()
        {
            var t = new FollowUpTracker();
            t.Register(Line("a", 3));

            var ready = new List<RillLine>();
            t.TickGateCrossing(ready); // 1
            t.TickGateCrossing(ready); // 2
            Assert.AreEqual(0, ready.Count, "must not fire before its delay elapses");

            t.TickGateCrossing(ready); // 3 — due
            Assert.AreEqual(1, ready.Count);
            Assert.AreEqual("a", ready[0].id);
        }

        [Test]
        public void FiresOnlyOnce_ThenStopsTracking()
        {
            var t = new FollowUpTracker();
            t.Register(Line("a", 1));

            var first = new List<RillLine>();
            t.TickGateCrossing(first);
            Assert.AreEqual(1, first.Count);

            var second = new List<RillLine>();
            t.TickGateCrossing(second);
            Assert.AreEqual(0, second.Count, "already-fired line must not fire again");
            Assert.AreEqual(0, t.PendingCount);
        }

        [Test]
        public void RegisteringSameIdTwice_IsIdempotent()
        {
            var t = new FollowUpTracker();
            t.Register(Line("a", 2));
            t.Register(Line("a", 2)); // e.g. a repeated poll hit on the same flag
            Assert.AreEqual(1, t.PendingCount);
        }

        [Test]
        public void MultiplePending_EachFiresOnItsOwnSchedule()
        {
            var t = new FollowUpTracker();
            t.Register(Line("short", 1));
            t.Register(Line("long", 3));

            var ready = new List<RillLine>();
            t.TickGateCrossing(ready); // 1: short due, long has 2 left
            Assert.AreEqual(1, ready.Count);
            Assert.AreEqual("short", ready[0].id);

            ready.Clear();
            t.TickGateCrossing(ready); // 2: long has 1 left
            Assert.AreEqual(0, ready.Count);

            ready.Clear();
            t.TickGateCrossing(ready); // 3: long due
            Assert.AreEqual(1, ready.Count);
            Assert.AreEqual("long", ready[0].id);
        }

        [Test]
        public void ZeroOrNegativeDelay_ClampsToAtLeastOneTick()
        {
            var t = new FollowUpTracker();
            t.Register(Line("a", 0));

            var ready = new List<RillLine>();
            t.TickGateCrossing(ready);
            Assert.AreEqual(1, ready.Count, "a non-positive delay should still require one crossing, not fire instantly");
        }
    }
}
