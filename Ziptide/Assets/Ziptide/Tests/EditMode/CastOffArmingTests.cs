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
    }
}
