using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The PUNCH IT arming law (the boarded fuel-cell gate): the launch only ever blocks when a
    /// gate is configured AND its machine exists in the scene AND that machine is still broken.
    /// Every other combination stays armed — a missing coupler must never strand the tutorial.
    /// </summary>
    public class CastOffArmingTests
    {
        [Test]
        public void Blocks_OnlyWhen_ConfiguredMachinePresentAndBroken()
        {
            Assert.IsFalse(CastOffArming.IsArmed(gateConfigured: true, machineFound: true, machineRepaired: false),
                "Configured gate with a broken machine must block the launch.");
        }

        [Test]
        public void Arms_WhenMachineRepaired()
        {
            Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: true, machineFound: true, machineRepaired: true));
        }

        [Test]
        public void Arms_WhenMachineMissing_NeverStrandsTheLaunch()
        {
            Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: true, machineFound: false, machineRepaired: false),
                "A configured gate whose machine is absent (dev warp / non-tutorial berth) must stay armed.");
        }

        [Test]
        public void Arms_WhenNoGateConfigured()
        {
            Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: false, machineFound: false, machineRepaired: false));
            Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: false, machineFound: true, machineRepaired: false));
        }

        // ── The key gate (the berth the first hour ends at) ───────────────────────

        [Test]
        public void KeyGate_BlocksUntilTheKeyIsSeated()
        {
            Assert.IsFalse(CastOffArming.IsArmed(false, false, false, keyRequired: true, keySeated: false),
                "the ship has no route until the artifact gives it one — this is the whole beat");
            Assert.IsTrue(CastOffArming.IsArmed(false, false, false, keyRequired: true, keySeated: true));
        }

        [Test]
        public void KeyGate_BlocksOnAbsence_WhereTheMachineGateForgivesIt()
        {
            // The two gates are deliberately opposite and this is the easiest thing here to get
            // backwards: a MISSING coupler must never strand the launch, but a MISSING key must.
            Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: true, machineFound: false, machineRepaired: false));
            Assert.IsFalse(CastOffArming.IsArmed(false, false, false, keyRequired: true, keySeated: false));
        }

        [Test]
        public void KeyGate_IsInertWhenNotRequired()
        {
            Assert.IsTrue(CastOffArming.IsArmed(false, false, false, keyRequired: false, keySeated: false),
                "every other berth in the game must behave exactly as it did before");
            Assert.IsFalse(CastOffArming.IsArmed(true, true, false, keyRequired: false, keySeated: true),
                "and a seated key must not paper over a broken coupler");
        }

        [Test]
        public void KeyGate_AndMachineGate_MustBothPass()
        {
            Assert.IsFalse(CastOffArming.IsArmed(true, true, false, keyRequired: true, keySeated: true),
                "key seated but coupler broken still blocks");
            Assert.IsTrue(CastOffArming.IsArmed(true, true, true, keyRequired: true, keySeated: true));
        }
    }
}
