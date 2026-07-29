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

        /// <summary>
        /// The SECOND gate, for the berth the first hour ends at (FIRST_HOUR_DIRECTORS_CUT §2.5).
        /// A ship whose route only the artifact knows must not leave until the key is seated — but
        /// unlike the coupler, a missing key is not "drift" to be forgiven: the whole beat is that
        /// the ship CANNOT go yet. So this gate blocks on absence, where the machine gate arms on it.
        ///
        /// Kept beside the machine rule because the two are read together and the difference between
        /// them is the easiest thing in this system to get backwards.
        /// </summary>
        public static bool IsArmed(bool gateConfigured, bool machineFound, bool machineRepaired,
            bool keyRequired, bool keySeated)
        {
            if (keyRequired && !keySeated) return false;
            return IsArmed(gateConfigured, machineFound, machineRepaired);
        }
    }
}
