using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// ONCE YOU HAVE IT, YOU HAVE IT (⚖ Terry). A weapon you have earned is yours permanently — it
    /// survives death, travel, quitting and reloading, and it reappears on the ship's rack whether or
    /// not you were carrying it when you left.
    ///
    /// Ownership rides the EXISTING persisted flag set on <see cref="PlayerProfile"/> rather than a new
    /// save field. That is deliberate: `SaveSystem` is a protected owner, and a schema change to store
    /// something the flag set already expresses would be new migration risk bought for nothing. One
    /// flag per weapon, `WEAPON_OWNED_&lt;itemId&gt;`.
    ///
    /// Pure, so the whole rule is provable without a headset: what you own, in what order, and that
    /// earning a weapon can never disturb the ones you already had.
    /// </summary>
    public static class WeaponOwnership
    {
        public const string FlagPrefix = "WEAPON_OWNED_";

        /// <summary>The persisted flag for a weapon id.</summary>
        public static string OwnedFlag(string itemId) =>
            string.IsNullOrEmpty(itemId) ? "" : FlagPrefix + itemId;

        /// <summary>True when this weapon has been earned. Non-weapons are never "owned".</summary>
        public static bool Owns(PlayerProfile profile, string itemId)
        {
            if (profile == null || !WeaponCatalog.IsWeapon(itemId)) return false;
            return profile.HasFlag(OwnedFlag(itemId));
        }

        /// <summary>
        /// Earn a weapon. Returns true only the FIRST time, so a caller can fire the "new weapon"
        /// moment without tracking that itself — picking the same gun up twice is not a discovery.
        /// </summary>
        public static bool Grant(PlayerProfile profile, string itemId)
        {
            if (profile == null || !WeaponCatalog.IsWeapon(itemId)) return false;
            string flag = OwnedFlag(itemId);
            if (profile.HasFlag(flag)) return false;
            profile.SetFlag(flag);
            return true;
        }

        /// <summary>
        /// Owned weapons in CATALOG order — never in the order they were earned.
        ///
        /// This is the load-bearing decision for the rack: a weapon must live at the same detent
        /// forever. If the carousel reordered itself every time you earned something, the muscle
        /// memory of "my pistol is two clicks left" would break exactly when the player has most
        /// invested in it.
        /// </summary>
        public static List<string> OwnedIds(PlayerProfile profile)
        {
            var owned = new List<string>();
            if (profile == null) return owned;
            for (int i = 0; i < WeaponCatalog.WeaponIds.Count; i++)
            {
                string id = WeaponCatalog.WeaponIds[i];
                if (profile.HasFlag(OwnedFlag(id))) owned.Add(id);
            }
            return owned;
        }

        /// <summary>
        /// The permanent detent index for a weapon: its position in the catalog, owned or not.
        /// Unearned positions still exist on the rack — they show the blanking plate — which is what
        /// keeps every other weapon's position fixed for the whole campaign.
        /// </summary>
        public static int SlotIndexOf(string itemId)
        {
            for (int i = 0; i < WeaponCatalog.WeaponIds.Count; i++)
                if (WeaponCatalog.WeaponIds[i] == itemId) return i;
            return -1;
        }
    }
}
