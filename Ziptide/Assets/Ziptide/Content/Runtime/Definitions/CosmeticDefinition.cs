using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>What a cosmetic reskins. Kinds are browse-categories in the Quarters bays.</summary>
    public enum CosmeticKind
    {
        WeaponSkin, // targets one itemId (taser_dart_gun, gravity_gun, pistol, …)
        ShipLivery, // hull color identity
        Trail,      // movement/projectile trails (visual layer lands with M6 VFX)
        Emblem      // profile badge shown on the locker board
    }

    /// <summary>
    /// One cosmetic (GAME_PLAN M4 / QUARTERS.md): a LOOK, never a stat — skins change color/scale
    /// accents only, so cosmetics can cross into any mode (PvP included) without balance questions.
    /// Lives under Resources/Cosmetics/ (the registry convention); the pure equip state is
    /// Ziptide.Core.CosmeticLocker (profile flags — saves/loads for free); ItemFactory applies an
    /// equipped WeaponSkin at creation. NONE ARE AUTHORED YET — the Quarters bays show the "no items"
    /// stub until a drop is authored (create a CosmeticAuthor entry like every other data author).
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Cosmetic", fileName = "Cosmetic")]
    public class CosmeticDefinition : ScriptableObject
    {
        [Tooltip("Unique cosmetic id (e.g. taser_rustline, hull_tidebreak).")]
        public string cosmeticId = "cosmetic";

        [Tooltip("Name shown in the Quarters bay.")]
        public string displayName = "";

        public CosmeticKind kind = CosmeticKind.WeaponSkin;

        [Tooltip("WeaponSkin only: the itemId this skin applies to (matches Resources/Items ids).")]
        public string targetItemId = "";

        [Header("The look (alpha 0 / zero = leave that channel unchanged)")]
        [Tooltip("Body tint override.")]
        public Color bodyColor = new Color(0f, 0f, 0f, 0f);
        [Tooltip("Accent tint override (muzzle/lamp/detail parts where supported).")]
        public Color accentColor = new Color(0f, 0f, 0f, 0f);
        [Tooltip("Uniform visual scale multiplier (0 = unchanged). Cosmetic ONLY — colliders untouched.")]
        public float scaleMultiplier = 0f;

        [Header("Unlock")]
        [Tooltip("Profile flag required to OWN this cosmetic (blank = owned by default once authored). " +
                 "Story rewards, bounty milestones, and any future shop all just set flags.")]
        public string ownedFlag = "";
    }
}
