namespace Ziptide.Core
{
    /// <summary>
    /// THE COCKPIT DECK, as numbers. One set of them, read by the builder, by the boarding teleport
    /// and by the tests.
    ///
    /// Why this exists (2026-08-02, device pass): BOARD SHIP threw the player off the ship. The log
    /// read `SHIP_BOARD` and then `FALL_SAFETY y=-60.2 below floor=-60.0 - forcing respawn` — a sixty
    /// metre drop straight out of a button press. The deck was built from one literal set of offsets
    /// and the teleport aimed at a DIFFERENT, serialized set, so nothing in the codebase could tell
    /// you whether the point you were teleporting to had a floor under it. It also had rails on three
    /// sides out of four: walking forward off the bow was a legal move.
    ///
    /// Two laws come out of that:
    ///   1. The stand point is DERIVED from the deck that actually exists, never from a remembered
    ///      offset (ShipBoardingStation resolves it off the deck collider's bounds, then proves it
    ///      with a downward ray). An unproven point is refused, not taken.
    ///   2. The deck is CLOSED — four rails, not three. A standing VR player leans; the deck must
    ///      not punish that.
    /// </summary>
    public static class ShipDeckCore
    {
        /// <summary>Deck plate is square; this is its full width and length in metres.</summary>
        public const float DeckSize = 3.4f;

        /// <summary>Plate thickness. The plate's TOP surface is the deck plane (local y = 0).</summary>
        public const float DeckThickness = 0.2f;

        /// <summary>Rail height above the deck plane. Above hip, below the sightline.</summary>
        public const float RailHeight = 0.9f;

        /// <summary>Rail plate thickness.</summary>
        public const float RailThickness = 0.1f;

        /// <summary>How far above the deck plane the rig root is placed when boarding.</summary>
        public const float StandClearance = 0.1f;

        /// <summary>
        /// A boarding/disembark point is only accepted if solid ground is within this distance below
        /// it. Generous enough for a stepped deck, tight enough that "the void" fails the check.
        /// </summary>
        public const float MaxStandDrop = 2.5f;

        /// <summary>Ray starts this far above the candidate so a point already 1 cm inside the plate still hits.</summary>
        public const float StandProbeLift = 0.6f;

        public static float DeckHalf => DeckSize * 0.5f;

        /// <summary>Local Y of the deck plate's CENTRE, given that its top is the deck plane.</summary>
        public static float PlateCentreY => -DeckThickness * 0.5f;

        /// <summary>Local Y of a rail's centre.</summary>
        public static float RailCentreY => RailHeight * 0.5f;

        /// <summary>Distance from deck centre to a rail's centre line.</summary>
        public static float RailOffset => DeckHalf;

        /// <summary>Total length of the probe ray used to prove a stand point.</summary>
        public static float StandProbeLength => StandProbeLift + MaxStandDrop;

        /// <summary>True when a point (deck-local X/Z) is inside the railed footprint.</summary>
        public static bool IsInsideRails(float localX, float localZ)
        {
            float limit = DeckHalf - RailThickness * 0.5f;
            return localX >= -limit && localX <= limit && localZ >= -limit && localZ <= limit;
        }

        /// <summary>
        /// The four rail centre offsets from the deck centre, in the order left, right, aft, FORWARD.
        /// The fourth entry is the one that was missing when the player could walk off the bow.
        /// </summary>
        public static void RailOffsets(out float[] x, out float[] z)
        {
            float o = RailOffset;
            x = new[] { -o, o, 0f, 0f };
            z = new[] { 0f, 0f, -o, o };
        }

        /// <summary>Rail plate size for rail index i (0=left, 1=right, 2=aft, 3=forward).</summary>
        public static void RailSize(int index, out float sx, out float sy, out float sz)
        {
            sy = RailHeight;
            bool sideRail = index == 0 || index == 1;
            sx = sideRail ? RailThickness : DeckSize;
            sz = sideRail ? DeckSize : RailThickness;
        }

        /// <summary>
        /// Given the top surface Y of the deck the player is actually standing on, the Y the rig root
        /// should be moved to. Kept as a function so the +clearance rule has exactly one home.
        /// </summary>
        public static float StandY(float surfaceY) => surfaceY + StandClearance;

        /// <summary>
        /// Is a resolved stand point trustworthy? `groundFound` comes from a downward probe, and
        /// `dropToGround` is how far below the candidate that ground sits. Refuse anything else —
        /// a boarding button that does nothing is a bug; one that drops you sixty metres is worse.
        /// </summary>
        public static bool StandIsProven(bool groundFound, float dropToGround)
        {
            return groundFound && dropToGround >= -StandProbeLift && dropToGround <= MaxStandDrop;
        }
    }
}
