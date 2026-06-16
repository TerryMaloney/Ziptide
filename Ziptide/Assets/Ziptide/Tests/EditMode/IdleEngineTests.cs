using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public class IdleEngineTests
    {
        [Test]
        public void Accrue_Basic_NoCap()
        {
            var r = IdleEngine.Accrue(ratePerSecond: 2.0, secondsElapsed: 100, currentStored: 0, storageCap: 0);
            Assert.AreEqual(200.0, r.produced, 1e-9);
            Assert.AreEqual(200.0, r.stored, 1e-9);
            Assert.AreEqual(200.0, r.added, 1e-9);
            Assert.AreEqual(0.0, r.overflow, 1e-9);
            Assert.AreEqual(100L, r.secondsApplied);
        }

        [Test]
        public void Accrue_RespectsStorageCap_Overflow()
        {
            var r = IdleEngine.Accrue(1.0, 100, currentStored: 0, storageCap: 50);
            Assert.AreEqual(100.0, r.produced, 1e-9);
            Assert.AreEqual(50.0, r.stored, 1e-9);
            Assert.AreEqual(50.0, r.added, 1e-9);
            Assert.AreEqual(50.0, r.overflow, 1e-9);
        }

        [Test]
        public void Accrue_ClampsWindowToMaxSeconds()
        {
            var r = IdleEngine.Accrue(1.0, secondsElapsed: 100000, currentStored: 0, storageCap: 0, maxSeconds: 60);
            Assert.AreEqual(60.0, r.produced, 1e-9);
            Assert.AreEqual(60L, r.secondsApplied);
        }

        [Test]
        public void Accrue_AlreadyFull_AddsNothing()
        {
            var r = IdleEngine.Accrue(1.0, 100, currentStored: 50, storageCap: 50);
            Assert.AreEqual(0.0, r.added, 1e-9);
            Assert.AreEqual(50.0, r.stored, 1e-9);
            Assert.AreEqual(100.0, r.overflow, 1e-9);
        }

        [Test]
        public void Accrue_NegativeElapsed_IsZero()
        {
            var r = IdleEngine.Accrue(5.0, -100, currentStored: 10, storageCap: 0);
            Assert.AreEqual(0.0, r.produced, 1e-9);
            Assert.AreEqual(10.0, r.stored, 1e-9);
            Assert.AreEqual(0L, r.secondsApplied);
        }

        [Test]
        public void SecondsBetween_NeverNegative()
        {
            Assert.AreEqual(150L, IdleEngine.SecondsBetween(1000, 1150));
            Assert.AreEqual(0L, IdleEngine.SecondsBetween(1150, 1000));
        }
    }
}
