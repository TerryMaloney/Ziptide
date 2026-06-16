using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public class PlayerProfileTests
    {
        [Test]
        public void AddResource_AccumulatesAndClampsAtZero()
        {
            var p = new PlayerProfile();
            Assert.AreEqual(10.0, p.AddResource("ore", 10), 1e-9);
            Assert.AreEqual(15.0, p.AddResource("ore", 5), 1e-9);
            Assert.AreEqual(0.0,  p.AddResource("ore", -100), 1e-9); // clamps, never negative
            Assert.AreEqual(0.0,  p.GetResource("unknown"), 1e-9);
        }

        [Test]
        public void Flags_SetHasClear_IsIdempotent()
        {
            var p = new PlayerProfile();
            Assert.IsFalse(p.HasFlag("x"));
            p.SetFlag("x");
            p.SetFlag("x");                  // idempotent — no duplicate
            Assert.IsTrue(p.HasFlag("x"));
            Assert.AreEqual(1, p.flags.Count);
            p.ClearFlag("x");
            Assert.IsFalse(p.HasFlag("x"));
        }

        [Test]
        public void GetWorld_CreateIfMissing_ReturnsSameInstance()
        {
            var p = new PlayerProfile();
            Assert.IsNull(p.GetWorld("nope"));
            var w = p.GetWorld("d0_city", createIfMissing: true);
            Assert.IsNotNull(w);
            Assert.AreSame(w, p.GetWorld("d0_city"));   // resolves to the same stored instance
        }
    }
}
