using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Ship pillar 2.2: equipped chassis/modules/name as pure profile-flag state
    /// (the CosmeticLocker idiom). Headless.</summary>
    public class ShipLockerTests
    {
        [Test]
        public void Equip_OneEntryPerSlot_SwapReplaces()
        {
            var p = new PlayerProfile();
            ShipLocker.Equip(p, "chassis", "interceptor");
            Assert.AreEqual("interceptor", ShipLocker.GetEquipped(p, "chassis"));
            ShipLocker.Equip(p, "chassis", "racer");
            Assert.AreEqual("racer", ShipLocker.GetEquipped(p, "chassis"), "swap replaces");
            int entries = 0;
            foreach (var f in p.flags) if (f != null && f.StartsWith("SHIP_EQUIP:chassis=")) entries++;
            Assert.AreEqual(1, entries, "never two entries for one slot");
        }

        [Test]
        public void SlotsAreIndependent()
        {
            var p = new PlayerProfile();
            ShipLocker.Equip(p, "chassis", "hauler");
            ShipLocker.Equip(p, "engine", "engine_tide");
            ShipLocker.Equip(p, "name", "LOW TIDE");
            Assert.AreEqual("hauler", ShipLocker.GetEquipped(p, "chassis"));
            Assert.AreEqual("engine_tide", ShipLocker.GetEquipped(p, "engine"));
            Assert.AreEqual("LOW TIDE", ShipLocker.GetEquipped(p, "name"));
        }

        [Test]
        public void EquippedModules_SkipsEmptySlots()
        {
            var p = new PlayerProfile();
            ShipLocker.Equip(p, "engine", "engine_ion");
            var mods = ShipLocker.EquippedModules(p, new[] { "engine", "wings", "hardpoint" });
            Assert.AreEqual(1, mods.Count);
            Assert.AreEqual("engine_ion", mods[0]);
        }

        [Test]
        public void NullProfile_IsSafeEverywhere()
        {
            Assert.IsNull(ShipLocker.GetEquipped(null, "chassis"));
            ShipLocker.Equip(null, "chassis", "racer"); // no throw
            Assert.AreEqual(0, ShipLocker.EquippedModules(null, null).Count);
        }
    }
}
