using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class InputSessionSceneLatchCoreTests
    {
        [Test]
        public void SameSceneActivation_IsAcceptedOnlyOnce()
        {
            var latch = new InputSessionSceneLatchCore();
            Assert.That(latch.TryEnter(101), Is.True);
            Assert.That(latch.TryEnter(101), Is.False);
            Assert.That(latch.HasScene, Is.True);
            Assert.That(latch.SceneHandle, Is.EqualTo(101));
        }

        [Test]
        public void NewSceneActivation_AdvancesTheLatch()
        {
            var latch = new InputSessionSceneLatchCore();
            Assert.That(latch.TryEnter(101), Is.True);
            Assert.That(latch.TryEnter(102), Is.True);
            Assert.That(latch.TryEnter(102), Is.False);
            Assert.That(latch.SceneHandle, Is.EqualTo(102));
        }

        [Test]
        public void Reset_AllowsARepeatedHandleInANewRuntimeSession()
        {
            var latch = new InputSessionSceneLatchCore();
            Assert.That(latch.TryEnter(101), Is.True);
            latch.Reset();
            Assert.That(latch.HasScene, Is.False);
            Assert.That(latch.TryEnter(101), Is.True);
        }
    }
}
