namespace Ziptide.Gameplay
{
    /// <summary>
    /// PURE arming decision for the PUNCH IT cast-off (the boarded fuel-cell/coupler gate —
    /// PRIORITIES #3 fragment). One law, pinned by CastOffArmingTests: the gate only ever blocks
    /// when it is configured AND its machine is present in the scene AND that machine is still
    /// broken. No gate configured → armed. Machine absent (dev warp, non-tutorial berth, layout
    /// drift) → armed — a missing coupler must NEVER strand the launch.
    /// </summary>
    public static class CastOffArming
    {
        public static bool IsArmed(bool gateConfigured, bool machineFound, bool machineRepaired)
            => !gateConfigured || !machineFound || machineRepaired;
    }
}
