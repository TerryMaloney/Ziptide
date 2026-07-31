using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// THE ONE LIST OF WHAT A WEAPON IS.
    ///
    /// Before this, "weapon" was decided in three places that disagreed:
    ///   • `ScenePatcherToxicCity` placed two of them on the ground and no others;
    ///   • `HolsterSocketInteractor` hardcoded a five-id allowlist of what may go on the belt;
    ///   • nothing at all placed prism_beam, sonic_thumper, static_net or tide_pike, which are
    ///     authored, registered, forge-recipe'd, tested — and unobtainable by playing.
    /// So half the arsenal could not be found, and would not have been beltable if it had been.
    /// `ItemDefinition` carries no weapon flag and its `damage` field is unserialized on every
    /// weapon asset, so there was no predicate to ask either.
    ///
    /// This is that predicate. It is the same defect class as MISS_LEDGER #21 — a rule everyone
    /// followed by convention and no mechanism enforced — so it gets a mechanism.
    /// </summary>
    public static class WeaponCatalog
    {
        /// <summary>Every authored weapon. Adding one here is what makes it real: rackable,
        /// beltable, and counted by the ship's departure gate.</summary>
        public static readonly IReadOnlyList<string> WeaponIds = new[]
        {
            "pistol",
            "taser_dart_gun",
            "gravity_gun",
            "breaker_blade",
            "prism_beam",
            "sonic_thumper",
            "static_net",
            "tide_pike",
        };

        /// <summary>Non-weapon items that still belong on the belt (tools you carry, not fight with).</summary>
        public static readonly IReadOnlyList<string> HolsterableToolIds = new[]
        {
            "handheld_camera",
        };

        private static readonly HashSet<string> Weapons = new HashSet<string>(WeaponIds);
        private static readonly HashSet<string> Tools = new HashSet<string>(HolsterableToolIds);

        /// <summary>True for a weapon — the thing the ship's ramp counts.</summary>
        public static bool IsWeapon(string itemId) =>
            !string.IsNullOrEmpty(itemId) && Weapons.Contains(itemId);

        /// <summary>True for anything that may ride the belt. Every weapon is holsterable; a camera
        /// is holsterable without arming you.</summary>
        public static bool IsHolsterable(string itemId) =>
            IsWeapon(itemId) || (!string.IsNullOrEmpty(itemId) && Tools.Contains(itemId));
    }
}
