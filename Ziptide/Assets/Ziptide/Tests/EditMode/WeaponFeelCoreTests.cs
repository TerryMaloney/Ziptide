using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class WeaponFeelCoreTests
    {
        [TestCase(WeaponFeelKind.Ballistic)]
        [TestCase(WeaponFeelKind.Electric)]
        public void Resolve_StaysInsideComfortEnvelope(WeaponFeelKind kind)
        {
            WeaponFeelEnvelope value = WeaponFeelCore.Resolve(kind, 10f, 10f, 10f, 10f);
            Assert.That(value.Primary.Amplitude, Is.InRange(0f, 1f));
            Assert.That(value.Primary.Duration, Is.InRange(0.015f, 0.12f));
            Assert.That(value.Tail.Amplitude, Is.LessThan(value.Primary.Amplitude));
            Assert.That(value.Tail.Duration, Is.InRange(0.012f, 0.08f));
            Assert.That(value.Tail.Delay, Is.GreaterThan(value.Primary.Duration));
            Assert.That(value.HitConfirm.Amplitude, Is.LessThan(value.Primary.Amplitude));
            Assert.That(value.VisualKick, Is.InRange(0f, 0.04f));
            Assert.That(value.VisualReturnSeconds, Is.InRange(0.06f, 0.18f));
        }

        [Test]
        public void Resolve_UsesExistingDefinitionValuesAndIsDeterministic()
        {
            WeaponFeelEnvelope first = WeaponFeelCore.Resolve(
                WeaponFeelKind.Ballistic, 0.5f, 0.05f, 0.02f, 0.2f);
            WeaponFeelEnvelope second = WeaponFeelCore.Resolve(
                WeaponFeelKind.Ballistic, 0.5f, 0.05f, 0.02f, 0.2f);
            Assert.That(second.Primary.Amplitude, Is.EqualTo(first.Primary.Amplitude));
            Assert.That(second.Tail.Amplitude, Is.EqualTo(first.Tail.Amplitude));
            Assert.That(second.VisualKick, Is.EqualTo(first.VisualKick));
            Assert.That(second.VisualReturnSeconds, Is.EqualTo(first.VisualReturnSeconds));
        }

        [Test]
        public void ElectricTail_IsLongerThanBallisticTailAtSameDefinitionValues()
        {
            WeaponFeelEnvelope ballistic = WeaponFeelCore.Resolve(
                WeaponFeelKind.Ballistic, 0.6f, 0.08f, 0.02f, 0.4f);
            WeaponFeelEnvelope electric = WeaponFeelCore.Resolve(
                WeaponFeelKind.Electric, 0.6f, 0.08f, 0.02f, 0.4f);
            Assert.That(electric.Tail.Amplitude, Is.GreaterThan(ballistic.Tail.Amplitude));
            Assert.That(electric.Tail.Duration, Is.GreaterThan(ballistic.Tail.Duration));
        }
    }
}
