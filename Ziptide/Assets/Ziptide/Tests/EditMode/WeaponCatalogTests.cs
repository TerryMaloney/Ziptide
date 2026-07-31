using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Half the arsenal was unreachable: prism_beam, sonic_thumper, static_net and tide_pike are
    /// authored, registered, forge-recipe'd and tested — and were placed nowhere, and could not have
    /// gone on the belt if they had been, because the holster hardcoded a five-id allowlist.
    /// This pins the single list that now answers both questions.
    /// </summary>
    public class WeaponCatalogTests
    {
        [Test]
        public void EveryAuthoredWeapon_IsInTheCatalog()
        {
            // The four that were stranded, named explicitly — this is the regression that mattered.
            foreach (string id in new[] { "prism_beam", "sonic_thumper", "static_net", "tide_pike" })
                Assert.IsTrue(WeaponCatalog.IsWeapon(id), id + " was unobtainable before; keep it real");

            foreach (string id in new[] { "pistol", "taser_dart_gun", "gravity_gun", "breaker_blade" })
                Assert.IsTrue(WeaponCatalog.IsWeapon(id));

            Assert.AreEqual(8, WeaponCatalog.WeaponIds.Count,
                "the rack has 8 slots because there are 8 weapons — keep them in step");
        }

        [Test]
        public void EveryWeapon_CanGoOnTheBelt()
        {
            // The ship's ramp requires a holstered weapon. A weapon the holster rejects would be an
            // unsatisfiable gate — the exact soft-lock ShipArmouryCore's no-trap law exists to avoid.
            foreach (string id in WeaponCatalog.WeaponIds)
                Assert.IsTrue(WeaponCatalog.IsHolsterable(id),
                    id + " is a weapon the departure gate counts but the belt would refuse");
        }

        [Test]
        public void ATool_IsHolsterable_ButDoesNotArmYou()
        {
            // Carrying the camera must not open the ramp. "Holsterable" and "weapon" are different
            // questions and the catalog has to keep them apart.
            Assert.IsTrue(WeaponCatalog.IsHolsterable("handheld_camera"));
            Assert.IsFalse(WeaponCatalog.IsWeapon("handheld_camera"));
        }

        [Test]
        public void UnknownAndEmptyIds_AreNeitherWeaponNorHolsterable()
        {
            // Quest items are absorbed into the inventory by CollectibleRuntime, not belted, so the
            // catalog must not quietly promote them to gear.
            foreach (string id in new[] { null, "", "relay_cell", "artifact_half_a" })
            {
                Assert.IsFalse(WeaponCatalog.IsWeapon(id));
                Assert.IsFalse(WeaponCatalog.IsHolsterable(id));
            }
        }
    }
}
