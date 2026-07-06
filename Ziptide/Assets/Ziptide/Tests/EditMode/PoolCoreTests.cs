using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>ARCHITECTURE V2 Q4a — the pure pooling core's contract (GamePool's tested brain).</summary>
    public class PoolCoreTests
    {
        private sealed class Box { public int Id; }

        private static PoolCore<Box> MakePool(int maxRetained = 0)
        {
            int next = 0;
            return new PoolCore<Box>(() => new Box { Id = next++ }, maxRetained: maxRetained);
        }

        [Test]
        public void FirstAcquire_Builds_NotReuses()
        {
            var p = MakePool();
            var a = p.Acquire();
            Assert.IsNotNull(a);
            Assert.AreEqual(1, p.Created);
            Assert.AreEqual(0, p.Reused);
            Assert.AreEqual(1, p.Live);
            Assert.AreEqual(0, p.Free);
        }

        [Test]
        public void Release_Then_Acquire_Reuses_SameInstance()
        {
            var p = MakePool();
            var a = p.Acquire();
            bool retained = p.Release(a);
            Assert.IsTrue(retained);
            Assert.AreEqual(0, p.Live);
            Assert.AreEqual(1, p.Free);

            var b = p.Acquire();
            Assert.AreSame(a, b, "an available free item must be reused, not rebuilt");
            Assert.AreEqual(1, p.Created, "no new instance should be built");
            Assert.AreEqual(1, p.Reused);
            Assert.AreEqual(1, p.Live);
            Assert.AreEqual(0, p.Free);
        }

        [Test]
        public void ReleaseNull_IsNoOp_ReturnsFalse()
        {
            var p = MakePool();
            p.Acquire();
            Assert.IsFalse(p.Release(null));
            Assert.AreEqual(1, p.Live, "null release must not change Live");
            Assert.AreEqual(0, p.Free);
        }

        [Test]
        public void Live_NeverGoesNegative_OnOverRelease()
        {
            var p = MakePool();
            var a = p.Acquire();
            p.Release(a);
            p.Release(a); // second release of an already-freed item
            Assert.GreaterOrEqual(p.Live, 0);
        }

        [Test]
        public void MaxRetained_CapsTheFreeList_AndDropsOverflow()
        {
            var p = MakePool(maxRetained: 2);
            var a = p.Acquire();
            var b = p.Acquire();
            var c = p.Acquire();
            Assert.IsTrue(p.Release(a), "1st fits the cap");
            Assert.IsTrue(p.Release(b), "2nd fits the cap");
            Assert.IsFalse(p.Release(c), "3rd exceeds cap → dropped (caller destroys)");
            Assert.AreEqual(2, p.Free, "free list never exceeds the retained cap");
        }

        [Test]
        public void Hooks_Fire_OnGet_And_OnRelease()
        {
            int gets = 0, releases = 0;
            var p = new PoolCore<Box>(() => new Box(), onGet: _ => gets++, onRelease: _ => releases++);
            var a = p.Acquire();
            p.Release(a);
            var b = p.Acquire(); // reuse, onGet again
            Assert.AreEqual(2, gets);
            Assert.AreEqual(1, releases);
        }

        [Test]
        public void Prewarm_BuildsFreeItems_WithoutLoaningThem()
        {
            var p = MakePool();
            int made = p.Prewarm(5);
            Assert.AreEqual(5, made);
            Assert.AreEqual(5, p.Free);
            Assert.AreEqual(0, p.Live);
            Assert.AreEqual(5, p.Created);
            // Those prewarmed items are then reused, not rebuilt.
            p.Acquire();
            Assert.AreEqual(1, p.Reused);
            Assert.AreEqual(5, p.Created);
        }

        [Test]
        public void Prewarm_RespectsRetainedCap()
        {
            var p = MakePool(maxRetained: 3);
            Assert.AreEqual(3, p.Prewarm(10));
            Assert.AreEqual(3, p.Free);
        }

        [Test]
        public void DrainFree_EmptiesFreeList_ReturnsThem()
        {
            var p = MakePool();
            p.Prewarm(4);
            var drained = p.DrainFree();
            Assert.AreEqual(4, drained.Length);
            Assert.AreEqual(0, p.Free);
        }

        [Test]
        public void Deterministic_CounterSequence()
        {
            // A fixed op script must produce exact counters every run.
            var p = MakePool();
            var x = p.Acquire();   // C1 R0 L1 F0
            var y = p.Acquire();   // C2 R0 L2 F0
            p.Release(x);          // C2 R0 L1 F1
            var z = p.Acquire();   // reuse x → C2 R1 L2 F0
            p.Release(y);          // C2 R1 L1 F1
            p.Release(z);          // C2 R1 L0 F2
            Assert.AreEqual(2, p.Created);
            Assert.AreEqual(1, p.Reused);
            Assert.AreEqual(0, p.Live);
            Assert.AreEqual(2, p.Free);
        }
    }
}
