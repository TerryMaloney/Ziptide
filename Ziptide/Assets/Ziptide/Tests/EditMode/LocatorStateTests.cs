using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class LocatorStateTests
    {
        [Test]
        public void NotHeld_NeverFires_NoProgress()
        {
            var l = new LocatorState(1.0, 5.0);
            Assert.IsFalse(l.Tick(0.0, 0.5, held: false));
            Assert.AreEqual(0f, l.HoldProgress, 1e-5);
        }

        [Test]
        public void HoldingLongEnough_FiresOnce_ThenCooldown()
        {
            var l = new LocatorState(1.0, 5.0);
            Assert.IsFalse(l.Tick(0.0, 0.5, true));   // 0.5 held
            Assert.IsTrue(l.Tick(0.6, 0.6, true));    // crosses 1.0 -> ping
            Assert.IsTrue(l.OnCooldown);
            Assert.IsFalse(l.Tick(1.0, 0.6, true));   // still cooling, no re-fire
        }

        [Test]
        public void Cooldown_Clears_AfterWindow_ThenCanFireAgain()
        {
            var l = new LocatorState(1.0, 5.0);
            l.Tick(0.0, 1.1, true);   // fire at t=0 -> cooldown until 5.0
            Assert.IsTrue(l.OnCooldown);
            l.Tick(5.0, 0.0, false);  // window elapsed -> cooldown clears
            Assert.IsFalse(l.OnCooldown);
            // hold again to fire
            Assert.IsFalse(l.Tick(5.1, 0.5, true));
            Assert.IsTrue(l.Tick(5.7, 0.6, true));
        }

        [Test]
        public void ReleasingMidHold_ResetsProgress()
        {
            var l = new LocatorState(1.0, 5.0);
            l.Tick(0.0, 0.7, true);
            Assert.Greater(l.HoldProgress, 0f);
            l.Tick(0.7, 0.1, false);  // released
            Assert.AreEqual(0f, l.HoldProgress, 1e-5);
        }
    }
}
