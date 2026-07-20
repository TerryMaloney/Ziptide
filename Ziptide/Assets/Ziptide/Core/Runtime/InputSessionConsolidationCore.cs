namespace Ziptide.Core
{
    /// <summary>Pure decision seam for whether the canonical InputActionManager must be mutated.</summary>
    public static class InputSessionConsolidationCore
    {
        public static bool NeedsPrimaryMutation(bool primaryEnabled, bool assetListsMatch,
            int fullyDisabledAssetCount)
        {
            return !primaryEnabled || !assetListsMatch || fullyDisabledAssetCount > 0;
        }
    }
}
