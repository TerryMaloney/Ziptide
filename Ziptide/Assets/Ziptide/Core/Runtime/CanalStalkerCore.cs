namespace Ziptide.Core
{
    /// <summary>What the stalker is doing to your boat right now.</summary>
    public enum StalkerStage
    {
        /// <summary>Nothing yet — it is somewhere else in the canal.</summary>
        Absent,
        /// <summary>THE SHADOW: a wake running parallel. It never touches you.</summary>
        Shadow,
        /// <summary>THE BUMP: a shoulder against the hull. A warning, not an attack.</summary>
        Bump,
        /// <summary>THE BLOCK: it puts itself across the channel and waits.</summary>
        Block,
    }

    /// <summary>
    /// THE CANAL STALKER's escalation (⚖ Terry: the lizard-thing interacts with the BOAT, in the
    /// water, never on land). Three stages that build over repeated trips rather than over a single
    /// scare: it escorts you, then it warns you, then it stands in your way — and it is never
    /// lethal at any point. The counter is to stun it or simply to drift; it always yields.
    ///
    /// Pure because the whole design lives in the thresholds. A jump scare and an escort read
    /// identically in code and completely differently in a headset, and the difference is exactly
    /// these numbers: how many rides, how long you linger, how quickly it gives up.
    /// </summary>
    public static class CanalStalkerCore
    {
        /// <summary>Rides before it starts bumping (ride one is always just the shadow).</summary>
        public const int RidesBeforeBump = 2;

        /// <summary>Seconds of lingering in its stretch before it escalates within a single ride.</summary>
        public const float LingerSecondsToEscalate = 45f;

        /// <summary>Rides before it will actually block the channel.</summary>
        public const int RidesBeforeBlock = 3;

        /// <summary>How close it swims while shadowing (metres).</summary>
        public const float ShadowDistance = 9f;

        /// <summary>Seconds a block lasts before it gives way — it never traps you.</summary>
        public const float BlockYieldSeconds = 6f;

        /// <summary>Yaw the bump imparts, degrees. Small: assist recovers it, comfort survives it.</summary>
        public const float BumpYawDegrees = 10f;

        /// <summary>
        /// The stage for this moment. rideIndex is 1-based (the current trip), lingerSeconds is how
        /// long the boat has been in the stalker's water this trip, and a stunned stalker always
        /// falls back to shadowing — being stunned is the player's answer and it has to work.
        /// </summary>
        public static StalkerStage Stage(int rideIndex, float lingerSeconds, bool boatPresent, bool stunned)
        {
            if (!boatPresent) return StalkerStage.Absent;
            if (stunned) return StalkerStage.Shadow;

            bool longLinger = lingerSeconds >= LingerSecondsToEscalate;
            if (rideIndex >= RidesBeforeBlock && longLinger) return StalkerStage.Block;
            if (rideIndex >= RidesBeforeBump || longLinger) return StalkerStage.Bump;
            return StalkerStage.Shadow;
        }

        /// <summary>True when the stage may physically touch the hull. Shadow never does.</summary>
        public static bool Contacts(StalkerStage stage) =>
            stage == StalkerStage.Bump || stage == StalkerStage.Block;
    }
}
