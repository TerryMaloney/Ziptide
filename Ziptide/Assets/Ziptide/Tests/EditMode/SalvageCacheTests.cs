using NUnit.Framework;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 1.3c — the interior loot cache pays through the ONE economy path. Pins: the grant
    /// lands on the profile balance, writes a ledger entry with the Salvage source (the mode
    /// contract), and degenerate inputs are safe no-ops.
    /// </summary>
    public class SalvageCacheTests
    {
        [Test]
        public void Grant_CreditsBalance_AndWritesSalvageLedgerEntry()
        {
            var profile = ProfileSerializer.NewProfile();
            double granted = SalvageCacheRuntime.GrantTo(profile, "scrap", 7);

            Assert.AreEqual(7, granted, 1e-9);
            Assert.AreEqual(7, profile.GetResource("scrap"), 1e-9);
            Assert.Greater(profile.ledger.Count, 0, "every grant flows through the ledger");
            var last = profile.ledger[profile.ledger.Count - 1];
            Assert.AreEqual(LedgerSource.Salvage, last.source);
            Assert.AreEqual("scrap", last.resourceId);
            Assert.AreEqual(7, last.delta, 1e-9);
        }

        [Test]
        public void Grant_DegenerateInputs_AreSafeNoOps()
        {
            var profile = ProfileSerializer.NewProfile();
            Assert.AreEqual(0, SalvageCacheRuntime.GrantTo(null, "scrap", 5), 1e-9, "null profile");
            Assert.AreEqual(0, SalvageCacheRuntime.GrantTo(profile, "", 5), 1e-9, "empty resource");
            Assert.AreEqual(0, SalvageCacheRuntime.GrantTo(profile, "scrap", 0), 1e-9, "zero amount");
            Assert.AreEqual(0, profile.GetResource("scrap"), 1e-9, "nothing credited");
        }
    }
}
