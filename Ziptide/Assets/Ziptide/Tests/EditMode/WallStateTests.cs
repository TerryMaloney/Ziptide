using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class WallStateTests
    {
        [Test]
        public void Brick_Breaks_Only_After_BrickHits_Swings()
        {
            var w = new WallState(cols: 4, rows: 3, brickHits: 2, regenSeconds: 10.0);
            Assert.IsFalse(w.IsBroken(1, 1));

            bool broke = w.HitBrick(1, 1, 0.0);
            Assert.IsFalse(broke, "one swing must not break a 2-hit brick");
            Assert.IsFalse(w.IsBroken(1, 1));
            Assert.AreEqual(1, w.HitsOn(1, 1));

            broke = w.HitBrick(1, 1, 0.5);
            Assert.IsTrue(broke, "second swing breaks it");
            Assert.IsTrue(w.IsBroken(1, 1));
            Assert.AreEqual(1, w.BrokenCount);
        }

        [Test]
        public void Damage_Is_Localized_To_The_Struck_Brick()
        {
            var w = new WallState(cols: 4, rows: 3, brickHits: 1, regenSeconds: 10.0);
            w.HitBrick(2, 0, 0.0);
            Assert.IsTrue(w.IsBroken(2, 0));
            // Neighbors untouched.
            Assert.IsFalse(w.IsBroken(1, 0));
            Assert.IsFalse(w.IsBroken(3, 0));
            Assert.IsFalse(w.IsBroken(2, 1));
            Assert.AreEqual(1, w.BrokenCount);
        }

        [Test]
        public void OutOfBounds_And_AlreadyBroken_Hits_Are_Safe()
        {
            var w = new WallState(cols: 3, rows: 3, brickHits: 1, regenSeconds: 10.0);
            Assert.IsFalse(w.HitBrick(-1, 0, 0.0), "out of bounds ignored");
            Assert.IsFalse(w.HitBrick(3, 0, 0.0), "out of bounds ignored");
            Assert.AreEqual(0, w.BrokenCount);

            Assert.IsTrue(w.HitBrick(0, 0, 0.0));   // breaks
            Assert.IsFalse(w.HitBrick(0, 0, 0.1));  // already broken => false, no extra break
            Assert.AreEqual(1, w.BrokenCount);
        }

        [Test]
        public void Whole_Wall_Regenerates_After_Window()
        {
            var w = new WallState(cols: 3, rows: 3, brickHits: 1, regenSeconds: 10.0);
            w.HitBrick(0, 0, 0.0);
            w.HitBrick(2, 2, 0.0);
            Assert.AreEqual(2, w.BrokenCount);

            w.Tick(9.0);
            Assert.AreEqual(2, w.BrokenCount, "not yet healed before the window");

            w.Tick(10.0);
            Assert.AreEqual(0, w.BrokenCount, "all bricks restored after the window");
            Assert.IsFalse(w.AnyDamaged);
        }

        [Test]
        public void Regen_Clock_Resets_On_Each_Hit()
        {
            var w = new WallState(cols: 3, rows: 3, brickHits: 1, regenSeconds: 10.0);
            w.HitBrick(0, 0, 0.0);   // last hit t=0
            w.Tick(8.0);             // not yet
            w.HitBrick(1, 1, 8.0);   // last hit t=8
            w.Tick(17.0);            // 9s since last hit — not yet
            Assert.AreEqual(2, w.BrokenCount);
            w.Tick(18.0);            // 10s since last hit — heals all
            Assert.AreEqual(0, w.BrokenCount);
        }
    }
}
