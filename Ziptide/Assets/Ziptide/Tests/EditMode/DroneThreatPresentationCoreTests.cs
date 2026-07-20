using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class DroneThreatPresentationCoreTests
    {
        [Test]
        public void Resolve_HidesOutsideTelegraphAndFlash()
        {
            DroneThreatPresentation value = DroneThreatPresentationCore.Resolve(0.8f, false, false);
            Assert.That(value.Visible, Is.False);
            Assert.That(value.ShowAimLine, Is.False);
            Assert.That(value.OrbScale, Is.EqualTo(0f));
        }

        [Test]
        public void Resolve_RevealsAimLineOnlyInReadableWindupWindow()
        {
            DroneThreatPresentation early = DroneThreatPresentationCore.Resolve(0.1f, true, false);
            DroneThreatPresentation late = DroneThreatPresentationCore.Resolve(0.8f, true, false);
            Assert.That(early.Visible, Is.True);
            Assert.That(early.ShowAimLine, Is.False);
            Assert.That(late.ShowAimLine, Is.True);
            Assert.That(late.OrbScale, Is.GreaterThan(early.OrbScale));
            Assert.That(late.LineWidth, Is.GreaterThan(early.LineWidth));
            Assert.That(late.Intensity, Is.GreaterThan(early.Intensity));
        }

        [Test]
        public void Resolve_ClampsPresentationEnvelope()
        {
            for (int i = -10; i <= 20; i++)
            {
                DroneThreatPresentation value = DroneThreatPresentationCore.Resolve(i / 10f, true, false);
                Assert.That(value.OrbScale, Is.InRange(0.07f, 0.35f));
                Assert.That(value.LineWidth, Is.InRange(0.003f, 0.012f));
                Assert.That(value.Intensity, Is.InRange(0.55f, 2f));
            }
        }

        [Test]
        public void ShotFlash_IsShortDistinctAndNeverShowsAimLine()
        {
            DroneThreatPresentation value = DroneThreatPresentationCore.Resolve(0f, false, true);
            Assert.That(value.Visible, Is.True);
            Assert.That(value.ShowAimLine, Is.False);
            Assert.That(value.OrbScale, Is.EqualTo(0.42f).Within(0.001f));
            Assert.That(value.Intensity, Is.EqualTo(2.4f).Within(0.001f));
        }
    }
}
