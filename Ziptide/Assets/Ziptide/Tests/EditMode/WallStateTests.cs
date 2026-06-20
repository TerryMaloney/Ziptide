using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class WallStateTests
    {
        [Test]
        public void Hit_Escalates_Intact_Small_Large_AndCaps()
        {
            var w = new WallState(10.0);
            Assert.AreEqual(WallStage.Intact, w.Stage);
            Assert.IsFalse(w.ShootThrough);

            w.Hit(0);
            Assert.AreEqual(WallStage.SmallHole, w.Stage);
            Assert.IsTrue(w.ShootThrough);
            Assert.IsFalse(w.PassThrough);

            w.Hit(0.5);
            Assert.AreEqual(WallStage.LargeHole, w.Stage);
            Assert.IsTrue(w.PassThrough);

            w.Hit(1.0); // capped
            Assert.AreEqual(WallStage.LargeHole, w.Stage);
        }

        [Test]
        public void Regenerates_ToIntact_AfterWindow()
        {
            var w = new WallState(10.0);
            w.Hit(0);
            w.Tick(9.0);
            Assert.AreEqual(WallStage.SmallHole, w.Stage);
            w.Tick(10.0);
            Assert.AreEqual(WallStage.Intact, w.Stage);
            Assert.IsFalse(w.ShootThrough);
        }

        [Test]
        public void Regen_ClockResets_OnEachHit()
        {
            var w = new WallState(10.0);
            w.Hit(0);     // SmallHole, last hit t=0
            w.Tick(8.0);  // not yet
            w.Hit(8.0);   // LargeHole, last hit t=8
            w.Tick(17.0); // 9s since last hit — not yet
            Assert.AreEqual(WallStage.LargeHole, w.Stage);
            w.Tick(18.0); // 10s since last hit — heals
            Assert.AreEqual(WallStage.Intact, w.Stage);
        }
    }
}
