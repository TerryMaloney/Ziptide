namespace Ziptide.Core
{
    /// <summary>
    /// Pure decision rule for the reentry/landing handoff beat (FIRST_LEVEL_PRODUCT_CONTRACT §4
    /// "Reentry/landing handoff" — previously red with no canonical owner). The presentation plays
    /// only when the player arrives in a WORLD directly from the space leg; every other route into
    /// the same scene (gate travel, dev warp, boot) stays untouched. Kept pure so EditMode tests can
    /// prove the routing table without loading a scene.
    /// </summary>
    public static class ReentryArrivalCore
    {
        /// <summary>Stand-in presentation length; the art pass retunes it with the real veil.</summary>
        public const float PresentationSeconds = 2.5f;

        public static bool ShouldPlay(string previousScene, string currentScene, string spaceScene)
        {
            if (string.IsNullOrEmpty(previousScene)) return false;   // cold boot / unknown origin
            if (string.IsNullOrEmpty(currentScene)) return false;
            if (string.IsNullOrEmpty(spaceScene)) return false;
            if (previousScene != spaceScene) return false;           // only the space leg reenters
            if (currentScene == spaceScene) return false;            // never inside the leg itself
            return true;
        }
    }
}
