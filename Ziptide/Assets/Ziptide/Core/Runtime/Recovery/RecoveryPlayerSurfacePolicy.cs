namespace Ziptide.Core
{
    /// <summary>
    /// Player-facing surfaces created by otherwise-canonical owners. Automatic-owner gating alone
    /// is not enough when a surviving owner (for example PlayerRigPersistence) can still inject a
    /// forbidden HUD into the Golden view.
    /// </summary>
    public enum RecoveryPlayerSurfaceId
    {
        CreditsHud = 0
    }

    public static class RecoveryPlayerSurfacePolicy
    {
        public static bool Allows(RecoveryPlayerSurfaceId surfaceId)
        {
            switch (surfaceId)
            {
                case RecoveryPlayerSurfaceId.CreditsHud:
                    // The legacy always-on balance readout is useful only in the compatibility
                    // profile. GoldenSlice and Diagnostic inherit the clean player-view contract.
                    return ReferenceEquals(
                        RecoveryRuntimeGate.ActiveProfile,
                        RecoveryExposureProfiles.FullDevelopment);
                default:
                    return false;
            }
        }
    }
}
