using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 0.4 — the one comfort math. Pins the contract every rider (walk / flight / vehicle)
    /// relies on: stillness and strength-0 mean fully open, motion closes toward a floor (never a
    /// keyhole), turning outweighs speed, and the iris closes fast but reopens slow.
    /// </summary>
    public class ComfortCoreTests
    {
        [Test]
        public void Still_IsFullyOpen()
        {
            Assert.AreEqual(ComfortCore.OpenAperture, ComfortCore.TargetAperture(0f, 0f, 1f), 1e-6f);
        }

        [Test]
        public void StrengthZero_DisablesTheTunnel_EvenAtFullMotion()
        {
            Assert.AreEqual(ComfortCore.OpenAperture, ComfortCore.TargetAperture(1f, 1f, 0f), 1e-6f);
        }

        [Test]
        public void FullMotion_FullStrength_HitsTheFloor_NeverBelow()
        {
            float a = ComfortCore.TargetAperture(1f, 1f, 1f);
            Assert.AreEqual(ComfortCore.MinAperture, a, 1e-6f);
            // Absurd over-range inputs still respect the floor.
            Assert.GreaterOrEqual(ComfortCore.TargetAperture(10f, 10f, 10f), ComfortCore.MinAperture);
        }

        [Test]
        public void Turning_ClosesHarderThanSpeed()
        {
            float speedOnly = ComfortCore.TargetAperture(1f, 0f, 1f);
            float turnOnly = ComfortCore.TargetAperture(0f, 1f, 1f);
            Assert.Less(turnOnly, speedOnly, "turn is the stronger sickness driver — must tunnel deeper");
        }

        [Test]
        public void MoreStrength_MeansDeeperTunnel()
        {
            float weak = ComfortCore.TargetAperture(1f, 0.5f, 0.3f);
            float strong = ComfortCore.TargetAperture(1f, 0.5f, 1f);
            Assert.Less(strong, weak);
        }

        [Test]
        public void Step_ClosesFast_OpensSlow()
        {
            const float dt = 0.1f;
            float closed = ComfortCore.Step(ComfortCore.OpenAperture, ComfortCore.MinAperture, dt);
            float opened = ComfortCore.Step(ComfortCore.MinAperture, ComfortCore.OpenAperture, dt);
            float closeDelta = ComfortCore.OpenAperture - closed;
            float openDelta = opened - ComfortCore.MinAperture;
            Assert.Greater(closeDelta, openDelta, "protection must arrive faster than it fades");
        }

        [Test]
        public void Step_ConvergesAndClamps()
        {
            float a = ComfortCore.OpenAperture;
            for (int i = 0; i < 200; i++) a = ComfortCore.Step(a, ComfortCore.MinAperture, 0.02f);
            Assert.AreEqual(ComfortCore.MinAperture, a, 1e-4f);
            for (int i = 0; i < 200; i++) a = ComfortCore.Step(a, ComfortCore.OpenAperture, 0.02f);
            Assert.AreEqual(ComfortCore.OpenAperture, a, 1e-4f);
            Assert.AreEqual(a, ComfortCore.Step(a, a, 0f), 1e-6f, "zero dt is a no-op");
        }
    }
}
