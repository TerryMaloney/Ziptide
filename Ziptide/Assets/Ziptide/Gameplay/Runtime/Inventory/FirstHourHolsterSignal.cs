using System;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// XR-free first-holster decision seam. The socket calls this only after normal XR selection has
    /// succeeded; tests can verify idempotency without referencing XR Interaction Toolkit assemblies.
    /// This helper never persists the profile.
    /// </summary>
    public static class FirstHourHolsterSignal
    {
        public static bool TryReport(
            string itemId,
            PlayerProfile profile,
            ref bool alreadyReported,
            Action<string> publish)
        {
            if (alreadyReported || string.IsNullOrEmpty(itemId)) return false;

            if (profile != null && profile.HasFlag(ZiptideFlags.FIRST_HOLSTER))
            {
                alreadyReported = true;
                return false;
            }

            alreadyReported = true;
            profile?.SetFlag(ZiptideFlags.FIRST_HOLSTER);
            publish?.Invoke(itemId);
            return true;
        }
    }
}
