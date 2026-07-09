using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// Ship pillar 2.2 — equipped chassis/modules/name as PURE profile-flag state (the CosmeticLocker
    /// idiom exactly: string entries "SHIP_EQUIP:&lt;slot&gt;=&lt;id&gt;" in PlayerProfile.flags, so the
    /// loadout persists through the existing save pipeline with zero schema work and syncs over any
    /// future handshake as strings). Slots: "chassis", module slot ids ("engine"/"wings"/…), "name".
    /// </summary>
    public static class ShipLocker
    {
        private const string Prefix = "SHIP_EQUIP:";

        public static void Equip(PlayerProfile profile, string slot, string id)
        {
            if (profile == null || string.IsNullOrEmpty(slot)) return;
            string key = Prefix + slot + "=";
            // One entry per slot: clear any existing before writing.
            profile.flags.RemoveAll(f => f != null && f.StartsWith(key));
            if (!string.IsNullOrEmpty(id)) profile.SetFlag(key + id);
        }

        public static string GetEquipped(PlayerProfile profile, string slot)
        {
            if (profile == null || profile.flags == null || string.IsNullOrEmpty(slot)) return null;
            string key = Prefix + slot + "=";
            foreach (var f in profile.flags)
                if (f != null && f.StartsWith(key))
                    return f.Substring(key.Length);
            return null;
        }

        /// <summary>All equipped module ids for the given slot ids (skips empties) — feed for
        /// ShipLoadoutCore.Resolve.</summary>
        public static List<string> EquippedModules(PlayerProfile profile, IEnumerable<string> slotIds)
        {
            var list = new List<string>();
            if (slotIds == null) return list;
            foreach (var slot in slotIds)
            {
                var id = GetEquipped(profile, slot);
                if (!string.IsNullOrEmpty(id)) list.Add(id);
            }
            return list;
        }
    }
}
