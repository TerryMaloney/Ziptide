using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// GARDEN AAA 4.2c pour laws: nothing pours below the start angle, flow ramps monotonically to
    /// full at the full-tilt angle, a can never pours more than it holds, an empty can is a no-op,
    /// and refill snaps back to full. Pure, headless — the watering can's FEEL lives here.
    /// </summary>
    public class PourCoreTests
    {
        [Test]
        public void Rate_IsZeroUpright_FullWhenTipped_MonotonicBetween()
        {
            Assert.AreEqual(0f, PourCore.RateAt(0f));
            Assert.AreEqual(0f, PourCore.RateAt(PourCore.PourStartDeg), "at the threshold, still sealed");
            Assert.AreEqual(1f, PourCore.RateAt(PourCore.PourFullDeg));
            Assert.AreEqual(1f, PourCore.RateAt(180f), "past full tilt stays full, never overflows the law");

            float prev = 0f;
            for (float a = PourCore.PourStartDeg; a <= PourCore.PourFullDeg; a += 5f)
            {
                float r = PourCore.RateAt(a);
                Assert.GreaterOrEqual(r, prev, "flow must ramp, never stutter");
                prev = r;
            }
        }

        [Test]
        public void Tick_DrainsTheCan_AndCapsAtEmpty()
        {
            var can = CanState.Full;
            float total = 0f;
            for (int i = 0; i < 72 * 10; i++) // 10s fully tipped — far past the ~2s empty point
            {
                can = PourCore.Tick(can, 180f, 1f / 72f, out float poured);
                Assert.GreaterOrEqual(poured, 0f);
                total += poured;
            }
            Assert.AreEqual(0f, can.fill01, 1e-5f, "the can ends empty");
            Assert.AreEqual(1f, total, 1e-4f, "exactly one can's worth of water came out — no free water");

            can = PourCore.Tick(can, 180f, 1f, out float extra);
            Assert.AreEqual(0f, extra, "an empty can pours nothing");
        }

        [Test]
        public void UprightTick_PoursNothing_AndRefillSnapsFull()
        {
            var can = new CanState { fill01 = 0.4f };
            can = PourCore.Tick(can, 20f, 1f, out float poured);
            Assert.AreEqual(0f, poured);
            Assert.AreEqual(0.4f, can.fill01, 1e-6f);

            can = PourCore.Refill(can);
            Assert.AreEqual(1f, can.fill01);
        }
    }
}
