namespace Ziptide.Content.Photo
{
    /// <summary>How good a captured shot is — the band drives the reward + the frame the Quarters
    /// wall hangs it in.</summary>
    public enum PhotoRating { Snapshot = 0, Postcard = 1, Masterpiece = 2 }

    /// <summary>What the camera has in frame at capture time. The scene translator
    /// (CameraRuntime, later commit) fills this by testing the lens frustum against the sky rig +
    /// world POIs/creatures; this struct is pure so the scoring is deterministic + EditMode-tested.</summary>
    public struct PhotoSubjects
    {
        public int SkyTier;             // 0 interior · 1 standard · 2 signature (the Prospect tiers)
        public bool HorizonInFrame;     // the hazy horizon line is composed in shot
        public bool OccludedBodyInFrame;// a planet/moon/station body sits in frame
        public bool LandmarkInFrame;    // a hero POI/landmark is framed
        public bool CreatureInFrame;    // a creature is in shot
        public float FramingCentered;   // 0..1 — how well the main subject sits in the frame
    }

    /// <summary>The scored result.</summary>
    public struct PhotoScore
    {
        public int Total;
        public PhotoRating Rating;
        public bool EarnsDiscovery;     // Postcard+ earns the discovery reward (a great shot pays)
    }

    /// <summary>
    /// FIELD CAMERA — pure composition scoring (no UnityEngine, deterministic, EditMode-tested).
    /// A shot scores higher the more of the Prospect-bar skyscape it frames well — this is the
    /// "teaches players to LOOK" hook: point the camera at the signature sky + its occluded body,
    /// keep the horizon and your subject composed, and the shot rates a Masterpiece. Purely additive
    /// with non-negative parts, so a better-composed frame can never score worse (pinned by test).
    /// </summary>
    public static class PhotoComposition
    {
        public const int Base = 10;                 // you framed and took a shot
        public const int HorizonBonus = 8;          // the horizon line — core Prospect composition
        public const int OccludedBodyBonus = 15;    // the occluded body — the signature vista element
        public const int LandmarkBonus = 12;
        public const int CreatureBonus = 10;
        public const int CenteredMax = 10;          // up to +10 for a well-centered subject
        public static readonly int[] SkyTierBonus = { 0, 10, 20 }; // interior / standard / signature

        public const int PostcardThreshold = 30;
        public const int MasterpieceThreshold = 55;

        public static PhotoScore Evaluate(PhotoSubjects s)
        {
            int tier = s.SkyTier < 0 ? 0 : s.SkyTier >= SkyTierBonus.Length ? SkyTierBonus.Length - 1 : s.SkyTier;
            float centered = s.FramingCentered < 0f ? 0f : s.FramingCentered > 1f ? 1f : s.FramingCentered;

            int total = Base
                + SkyTierBonus[tier]
                + (s.HorizonInFrame ? HorizonBonus : 0)
                + (s.OccludedBodyInFrame ? OccludedBodyBonus : 0)
                + (s.LandmarkInFrame ? LandmarkBonus : 0)
                + (s.CreatureInFrame ? CreatureBonus : 0)
                + (int)(centered * CenteredMax);

            PhotoRating rating = total >= MasterpieceThreshold ? PhotoRating.Masterpiece
                               : total >= PostcardThreshold ? PhotoRating.Postcard
                               : PhotoRating.Snapshot;

            return new PhotoScore
            {
                Total = total,
                Rating = rating,
                EarnsDiscovery = total >= PostcardThreshold,
            };
        }
    }
}
