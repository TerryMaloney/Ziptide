using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// The surface-family color law (ART_DIRECTION_MASTER_PLAN.md) in testable form: known family keys
    /// + per-family palette conformance so a Forge recipe can't drift off-brand. Strict families
    /// (AlienOrigami) enforce tight bands; lived-in families are lenient by design — grime varies,
    /// mathematics doesn't.
    /// </summary>
    public static class ForgePalettes
    {
        public const string FamilySalvage = "Salvage";
        public const string FamilyToxicIndustrial = "ToxicIndustrial";
        public const string FamilyUpperClassGlass = "UpperClassGlass";
        public const string FamilyAlienOrigami = "AlienOrigami";
        public const string FamilyCosmic = "Cosmic";
        public const string FamilyWaterworld = "Waterworld";
        public const string FamilyCeremonialStone = "CeremonialStone";
        public const string FamilyBlackHoleFringe = "BlackHoleFringe";

        public static readonly string[] KnownFamilies =
        {
            FamilySalvage, FamilyToxicIndustrial, FamilyUpperClassGlass, FamilyAlienOrigami,
            FamilyCosmic, FamilyWaterworld, FamilyCeremonialStone, FamilyBlackHoleFringe
        };

        public static bool IsKnownFamily(string family)
        {
            foreach (var f in KnownFamilies)
                if (f == family) return true;
            return false;
        }

        /// <summary>
        /// Family conformance for one palette color. Lenient families always pass (their identity
        /// lives in shape/grime, not hue-band law); AlienOrigami is the strict canon: matte black,
        /// metallic gold, deep teal, amber, or the cool-white glyph tone (ALIEN_ORIGAMI_SURFACE_BRIEF).
        /// </summary>
        public static bool IsAllowed(string family, Color c)
        {
            if (family != FamilyAlienOrigami) return true;
            return IsNearBlack(c) || IsGold(c) || IsTeal(c) || IsAmber(c) || IsCoolWhite(c);
        }

        private static bool IsNearBlack(Color c) => Max3(c) < 0.16f;

        private static bool IsGold(Color c) =>
            c.r > 0.5f && c.g > 0.32f && c.b < c.g * 0.75f && c.r >= c.g && c.g > c.b;

        private static bool IsTeal(Color c) =>
            c.b > 0.3f && c.g > 0.3f && c.r < c.g * 0.6f && c.r < c.b * 0.6f;

        private static bool IsAmber(Color c) =>
            c.r > 0.6f && c.g > 0.3f && c.g < c.r * 0.85f && c.b < c.g * 0.5f;

        private static bool IsCoolWhite(Color c) =>
            Min3(c) > 0.7f && c.b >= c.r;

        private static float Max3(Color c) => Mathf.Max(c.r, Mathf.Max(c.g, c.b));
        private static float Min3(Color c) => Mathf.Min(c.r, Mathf.Min(c.g, c.b));
    }
}
