using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class ToxicExposureCoreTests
    {
        [Test]
        public void Entry_IsReportedOnce()
        {
            var core = new ToxicExposureCore(1f, 4f);

            ToxicExposureStep first = core.Tick(true, 0.1f);
            ToxicExposureStep second = core.Tick(true, 0.1f);

            Assert.That(first.Entered, Is.True);
            Assert.That(second.Entered, Is.False);
            Assert.That(core.IsInside, Is.True);
        }

        [Test]
        public void DamagePulses_UseBoundedCadence()
        {
            var core = new ToxicExposureCore(0.75f, 4.5f);

            Assert.That(core.Tick(true, 0.70f).DamagePulse, Is.False);
            Assert.That(core.Tick(true, 0.06f).DamagePulse, Is.True);
            Assert.That(core.Tick(true, 0.20f).DamagePulse, Is.False);
            Assert.That(core.Tick(true, 0.55f).DamagePulse, Is.True);
        }

        [Test]
        public void LethalEvent_FiresOncePerContinuousExposure()
        {
            var core = new ToxicExposureCore(0.5f, 2f);

            Assert.That(core.Tick(true, 1.9f).Lethal, Is.False);
            Assert.That(core.Tick(true, 0.2f).Lethal, Is.True);
            Assert.That(core.Tick(true, 2f).Lethal, Is.False);
        }

        [Test]
        public void Exit_ResetsExposureAndAllowsFreshLethalWindow()
        {
            var core = new ToxicExposureCore(0.5f, 1f);
            Assert.That(core.Tick(true, 1.1f).Lethal, Is.True);

            ToxicExposureStep exit = core.Tick(false, 0.1f);
            ToxicExposureStep reenter = core.Tick(true, 0.2f);

            Assert.That(exit.Exited, Is.True);
            Assert.That(core.ExposureSeconds, Is.EqualTo(0.2f).Within(0.001f));
            Assert.That(reenter.Entered, Is.True);
            Assert.That(reenter.Lethal, Is.False);
        }

        [Test]
        public void OutsideWithoutPriorEntry_IsAStableNoOp()
        {
            var core = new ToxicExposureCore();
            ToxicExposureStep step = core.Tick(false, 10f);

            Assert.That(step.Entered, Is.False);
            Assert.That(step.Exited, Is.False);
            Assert.That(step.DamagePulse, Is.False);
            Assert.That(step.Lethal, Is.False);
            Assert.That(core.ExposureSeconds, Is.Zero);
        }
    }
}
