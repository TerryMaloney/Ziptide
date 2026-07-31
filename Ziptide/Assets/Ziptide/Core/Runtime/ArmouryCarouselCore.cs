namespace Ziptide.Core
{
    /// <summary>
    /// THE RACK SPINS (⚖ Terry: *"the gun rack rotate or something like that … they can cycle through
    /// them but it only takes up a small amount of space"*).
    ///
    /// The approved concept is a 2.4 m wall rail. That is a lot of bulkhead for a cramped salvage ship,
    /// and a FIXED eight-slot rail is wrong at both ends of the campaign: mostly blanking plates on
    /// hour one, and out of room the moment the arsenal grows past eight. A drum fixes both — the
    /// footprint stops depending on how many weapons you own.
    ///
    /// Two rings of four, turned by hand:
    ///   • 8 detents at the same 0.34 m spacing the rail used, so the concept's slot module is reused
    ///     unchanged rather than redrawn;
    ///   • <see cref="Diameter"/> of floor/bulkhead instead of 2.4 m of span — about a fifth of the
    ///     wall for the same arsenal;
    ///   • every weapon keeps ONE detent forever (see <see cref="WeaponOwnership.SlotIndexOf"/>), so
    ///     earning a new gun never moves the one you already reach for without looking.
    ///
    /// Pure because a carousel's whole feel is in the arithmetic — where it stops, which way it turns,
    /// and whether two weapons can ever occupy the same arc. A headset tells you the drum feels good;
    /// only a test tells you it cannot overlap or renumber itself mid-campaign.
    /// </summary>
    public static class ArmouryCarouselCore
    {
        /// <summary>Detents per ring. Four presents one weapon squarely to the player at a time.</summary>
        public const int FacesPerRing = 4;

        /// <summary>Rings stacked vertically — upper at chest, lower at hip.</summary>
        public const int Rings = 2;

        /// <summary>Total detents. Matches the authored arsenal and the concept's eight slots.</summary>
        public const int Capacity = FacesPerRing * Rings;

        /// <summary>Drum diameter in metres. Sized so the arc between faces clears the rail's spacing.</summary>
        public const float Diameter = 0.52f;

        /// <summary>Vertical gap between the two rings, metres.</summary>
        public const float RingGap = 0.42f;

        /// <summary>Height of the LOWER ring above the deck; the upper sits a RingGap above it.</summary>
        public const float LowerRingHeight = 0.94f;

        /// <summary>Degrees between adjacent detents.</summary>
        public const float StepDegrees = 360f / FacesPerRing;

        /// <summary>Arc length between adjacent faces — must clear the rail's 0.34 m slot spacing.</summary>
        public static float FaceArcMetres => (float)(System.Math.PI * Diameter / FacesPerRing);

        /// <summary>Which ring a permanent slot index lives on (0 = lower).</summary>
        public static int RingOf(int slotIndex) => Wrap(slotIndex, Capacity) / FacesPerRing;

        /// <summary>Which face within its ring.</summary>
        public static int FaceOf(int slotIndex) => Wrap(slotIndex, Capacity) % FacesPerRing;

        /// <summary>Height above the deck for a slot.</summary>
        public static float HeightOf(int slotIndex) => LowerRingHeight + RingOf(slotIndex) * RingGap;

        /// <summary>Drum rotation, in degrees, that brings a slot to the front.</summary>
        public static float AngleForSlot(int slotIndex) => Wrap01Degrees(-FaceOf(slotIndex) * StepDegrees);

        /// <summary>Which face is presented to the player at this rotation.</summary>
        public static int FaceAtFront(float angleDegrees)
        {
            float step = StepDegrees;
            int face = (int)System.Math.Round(-Wrap01Degrees(angleDegrees) / step);
            return Wrap(face, FacesPerRing);
        }

        /// <summary>Nearest detent to a free rotation — the drum always settles square, never between.</summary>
        public static float SnapToDetent(float angleDegrees)
        {
            float step = StepDegrees;
            return Wrap01Degrees((float)System.Math.Round(angleDegrees / step) * step);
        }

        /// <summary>
        /// Shortest signed rotation from one angle to another, in [-180, 180]. A drum that takes the
        /// long way round to move one click reads as broken, and in VR it also reads as motion the
        /// player did not ask for.
        /// </summary>
        public static float ShortestDelta(float fromDegrees, float toDegrees)
        {
            float d = Wrap01Degrees(toDegrees - fromDegrees);
            return d > 180f ? d - 360f : d;
        }

        private static float Wrap01Degrees(float degrees)
        {
            float d = degrees % 360f;
            return d < 0f ? d + 360f : d;
        }

        private static int Wrap(int value, int modulus)
        {
            if (modulus <= 0) return 0;
            int v = value % modulus;
            return v < 0 ? v + modulus : v;
        }
    }
}
