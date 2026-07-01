namespace Ziptide.Core
{
    /// <summary>
    /// RILL's memory arc (MASTER_BUILD_PLAN §5.1). The state is DERIVED from story flags, never stored —
    /// so saves can't desync from progression and any system (line selection, behavior, audio register)
    /// reads the same truth.
    /// </summary>
    public enum RillMemoryState
    {
        Dormant,      // W001–W004: short functional responses only
        Stirring,     // W005–W012: RILL asks questions
        Remembering,  // W013–W028: partial memory access, emotional glitches
        Unsealing,    // W029–W051: full vocabulary, chooses a name
        Integrated,   // W052–W068: equals with Cal, delivers revelation monologues
        EndgameA,     // Ending A: RILL stays in the network
        EndgameB,     // Ending B: RILL crosses with Cal
        EndgameC,     // Ending C: RILL chooses to forget
        EndgameD      // Ending D: RILL becomes the Pattern
    }

    /// <summary>
    /// Pure derivation of <see cref="RillMemoryState"/> from the profile's story flags. Chapter-capstone
    /// flags advance the state (they're granted by the world contracts via WorldGating/WorldJobLibrary):
    /// Stirring ⇐ W004 done · Remembering ⇐ the W012 containment reveal (or the W013 memory shard) ·
    /// Unsealing ⇐ W028 · Integrated ⇐ RILL named at W051 · Endgame ⇐ the W063 ending branch flags.
    /// Highest reached state wins; the arc never regresses. Headless + deterministic — EditMode-tested.
    /// </summary>
    public static class RillState
    {
        public static RillMemoryState Compute(PlayerProfile profile)
        {
            if (profile == null) return RillMemoryState.Dormant;

            // Endgame branches are terminal and mutually exclusive (ChoiceStation writes exactly one).
            if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_A)) return RillMemoryState.EndgameA;
            if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_B)) return RillMemoryState.EndgameB;
            if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_C)) return RillMemoryState.EndgameC;
            if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_D)) return RillMemoryState.EndgameD;

            if (profile.HasFlag(ZiptideFlags.C6_W051_RILL_NAMED)) return RillMemoryState.Integrated;
            if (profile.HasFlag(ZiptideFlags.W028_COMPLETE) ||
                profile.HasFlag(ZiptideFlags.C4_W028_NO_JOB)) return RillMemoryState.Unsealing;
            if (profile.HasFlag(ZiptideFlags.C2_CONTAINMENT_REVEALED) ||
                profile.HasFlag(ZiptideFlags.C3_W013_MEMORY_SHARD)) return RillMemoryState.Remembering;
            if (profile.HasFlag(ZiptideFlags.W004_COMPLETE)) return RillMemoryState.Stirring;
            return RillMemoryState.Dormant;
        }
    }
}
