namespace Ziptide.Core
{
    /// <summary>
    /// Pure equip-state for cosmetics (GAME_PLAN M4 / QUARTERS.md) — the ONE source of truth for "what
    /// look is equipped on what", shared by every mode (story worlds, the ship, and the future PvP
    /// pre-round locker — architect's crossover consumes exactly this API). State is stored as profile
    /// flags with a reserved prefix (`COSM_EQUIP:<targetKey>=<cosmeticId>`), so it saves/loads/travels
    /// with the existing profile plumbing for free and never needs its own persistence. One equipped
    /// cosmetic per target key (a targetKey is an itemId for weapon skins, or a kind name like
    /// "ship_livery" for singletons). Headless + deterministic — EditMode-tested.
    /// </summary>
    public static class CosmeticLocker
    {
        private const string Prefix = "COSM_EQUIP:";

        /// <summary>Equip a cosmetic on a target (replaces any previous equip on that target).</summary>
        public static void Equip(PlayerProfile profile, string targetKey, string cosmeticId)
        {
            if (profile == null || string.IsNullOrEmpty(targetKey) || string.IsNullOrEmpty(cosmeticId)) return;
            Unequip(profile, targetKey);
            profile.SetFlag(Prefix + targetKey + "=" + cosmeticId);
        }

        /// <summary>Remove whatever is equipped on a target (back to the default look).</summary>
        public static void Unequip(PlayerProfile profile, string targetKey)
        {
            if (profile == null || profile.flags == null || string.IsNullOrEmpty(targetKey)) return;
            string keyPrefix = Prefix + targetKey + "=";
            for (int i = profile.flags.Count - 1; i >= 0; i--)
                if (profile.flags[i] != null && profile.flags[i].StartsWith(keyPrefix))
                    profile.flags.RemoveAt(i);
        }

        /// <summary>The equipped cosmetic id for a target, or null (default look).</summary>
        public static string GetEquipped(PlayerProfile profile, string targetKey)
        {
            if (profile == null || profile.flags == null || string.IsNullOrEmpty(targetKey)) return null;
            string keyPrefix = Prefix + targetKey + "=";
            for (int i = 0; i < profile.flags.Count; i++)
                if (profile.flags[i] != null && profile.flags[i].StartsWith(keyPrefix))
                    return profile.flags[i].Substring(keyPrefix.Length);
            return null;
        }

        /// <summary>True if this flag string is locker state (so flag-scanners like RILL can skip it).</summary>
        public static bool IsLockerFlag(string flag) =>
            !string.IsNullOrEmpty(flag) && flag.StartsWith(Prefix);
    }
}
