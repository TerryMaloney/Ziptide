using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class StunStateTests
    {
        [Test]
        public void Apply_SetsSlow_AndNotClear()
        {
            var s = new StunState();
            Assert.IsTrue(s.IsClear);
            Assert.AreEqual(1f, s.SlowFactor, 1e-5);
            s.Apply(1.2f, 0.45f);
            Assert.IsFalse(s.IsClear);
            Assert.AreEqual(0.45f, s.SlowFactor, 1e-5);
        }

        [Test]
        public void Tick_SelfClears_ToFullSpeed()
        {
            var s = new StunState();
            s.Apply(1.0f, 0.5f);
            s.Tick(0.5f);
            Assert.IsFalse(s.IsClear);
            Assert.AreEqual(0.5f, s.SlowFactor, 1e-5);
            s.Tick(0.6f); // past the end
            Assert.IsTrue(s.IsClear);
            Assert.AreEqual(1f, s.SlowFactor, 1e-5);
        }

        [Test]
        public void ReStun_RefreshesMaxTime_NotStack()
        {
            var s = new StunState();
            s.Apply(1.0f, 0.5f);
            s.Tick(0.8f);            // 0.2 remaining
            s.Apply(1.0f, 0.5f);     // refresh to 1.0, not 1.2
            s.Tick(0.9f);            // 0.1 remaining
            Assert.IsFalse(s.IsClear);
            s.Tick(0.2f);
            Assert.IsTrue(s.IsClear);
        }

        [Test]
        public void ReStun_TakesStrongerSlow_WhileActive()
        {
            var s = new StunState();
            s.Apply(2.0f, 0.6f);
            s.Apply(2.0f, 0.3f); // stronger slow wins while active
            Assert.AreEqual(0.3f, s.SlowFactor, 1e-5);
        }

        [Test]
        public void ZeroSeconds_IsNoOp()
        {
            var s = new StunState();
            s.Apply(0f, 0.2f);
            Assert.IsTrue(s.IsClear);
            Assert.AreEqual(1f, s.SlowFactor, 1e-5);
        }
    }
}
