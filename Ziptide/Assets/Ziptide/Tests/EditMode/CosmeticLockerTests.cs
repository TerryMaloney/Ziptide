using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The locker contract (QUARTERS.md): one equipped cosmetic per target, replace-on-equip,
    /// unequip returns to default, targets are independent, state is plain profile flags (so it
    /// saves/travels/crosses modes for free). Pure, headless.
    /// </summary>
    public class CosmeticLockerTests
    {
        [Test]
        public void Equip_Get_RoundTrips()
        {
            var p = new PlayerProfile();
            Assert.IsNull(CosmeticLocker.GetEquipped(p, "taser_dart_gun"), "default look = null");

            CosmeticLocker.Equip(p, "taser_dart_gun", "taser_rustline");
            Assert.AreEqual("taser_rustline", CosmeticLocker.GetEquipped(p, "taser_dart_gun"));
        }

        [Test]
        public void Equip_Replaces_ThePreviousSkin()
        {
            var p = new PlayerProfile();
            CosmeticLocker.Equip(p, "taser_dart_gun", "taser_rustline");
            CosmeticLocker.Equip(p, "taser_dart_gun", "taser_tidebreak");

            Assert.AreEqual("taser_tidebreak", CosmeticLocker.GetEquipped(p, "taser_dart_gun"));
            int lockerFlags = 0;
            foreach (var f in p.flags) if (CosmeticLocker.IsLockerFlag(f)) lockerFlags++;
            Assert.AreEqual(1, lockerFlags, "exactly one equip flag per target — never stacks");
        }

        [Test]
        public void Unequip_ReturnsToDefault()
        {
            var p = new PlayerProfile();
            CosmeticLocker.Equip(p, "gravity_gun", "grav_ember");
            CosmeticLocker.Unequip(p, "gravity_gun");
            Assert.IsNull(CosmeticLocker.GetEquipped(p, "gravity_gun"));
        }

        [Test]
        public void Targets_AreIndependent()
        {
            var p = new PlayerProfile();
            CosmeticLocker.Equip(p, "taser_dart_gun", "taser_rustline");
            CosmeticLocker.Equip(p, "shiplivery", "hull_tidebreak");

            Assert.AreEqual("taser_rustline", CosmeticLocker.GetEquipped(p, "taser_dart_gun"));
            Assert.AreEqual("hull_tidebreak", CosmeticLocker.GetEquipped(p, "shiplivery"));
            CosmeticLocker.Unequip(p, "taser_dart_gun");
            Assert.AreEqual("hull_tidebreak", CosmeticLocker.GetEquipped(p, "shiplivery"), "unaffected");
        }

        [Test]
        public void NullAndEmpty_AreSafe()
        {
            CosmeticLocker.Equip(null, "x", "y");
            CosmeticLocker.Unequip(null, "x");
            Assert.IsNull(CosmeticLocker.GetEquipped(null, "x"));

            var p = new PlayerProfile();
            CosmeticLocker.Equip(p, "", "y");
            CosmeticLocker.Equip(p, "x", "");
            Assert.IsNull(CosmeticLocker.GetEquipped(p, "x"));
        }

        [Test]
        public void LockerFlags_AreRecognizable_SoScannersCanSkipThem()
        {
            var p = new PlayerProfile();
            CosmeticLocker.Equip(p, "taser_dart_gun", "taser_rustline");
            p.SetFlag("W002_COMPLETE");

            foreach (var f in p.flags)
            {
                if (f == "W002_COMPLETE") Assert.IsFalse(CosmeticLocker.IsLockerFlag(f));
                else Assert.IsTrue(CosmeticLocker.IsLockerFlag(f));
            }
        }
    }
}
