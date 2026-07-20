using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class InputSessionConsolidationCoreTests
    {
        [Test]
        public void StableEnabledMatchingSession_DoesNotMutate()
        {
            Assert.That(InputSessionConsolidationCore.NeedsPrimaryMutation(
                primaryEnabled: true, assetListsMatch: true, fullyDisabledAssetCount: 0), Is.False);
        }

        [TestCase(false, true, 0)]
        [TestCase(true, false, 0)]
        [TestCase(true, true, 1)]
        public void ChangedOrDisabledSession_MustMutate(bool enabled, bool listsMatch, int disabledAssets)
        {
            Assert.That(InputSessionConsolidationCore.NeedsPrimaryMutation(
                enabled, listsMatch, disabledAssets), Is.True);
        }
    }
}
