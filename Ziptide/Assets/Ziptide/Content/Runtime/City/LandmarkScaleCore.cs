using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// WHY A BIG THING READS AS BIG — ⚖ Terry, 2026-08-01: *"if this is the first thing we see outside
    /// the ship we really want to build a sense of awe and perspective … the hangar was pretty much
    /// just like a bunch of sort of box areas, not very good."*
    ///
    /// The hangar's crane is authored 16 m tall and 2 m wide. In VR that is not a crane, it is a stick,
    /// and the reason is measurable rather than artistic: <b>a shape carries no scale of its own.</b> A
    /// 16 m box and a 1.6 m box are the same box until something in the frame has a size the player
    /// already knows. Ladder rungs are the cheapest such thing ever invented — every human alive knows
    /// a rung is about a foot apart, so a mast wearing rungs is instantly sixteen metres and everything
    /// standing next to it inherits that ruler.
    ///
    /// So this core owns two numbers and refuses to let either drift:
    ///   • <b>rung pitch</b> — real ladders are 25–35 cm. Outside that band the rungs stop being a
    ///     known quantity and the ruler lies, which is worse than no ruler at all.
    ///   • <b>slenderness</b> — height ÷ width. Past about 4:1 a tower reads as a pole no matter how
    ///     tall it is, because there is no surface left to put detail on.
    ///
    /// And one budget: detail exists only in the band a standing player can actually resolve. Rungs at
    /// 30 m are three pixels and cost a draw call each — putting them there is how a scale pass turns
    /// into a performance regression. Everything above <see cref="DetailCeiling"/> is silhouette.
    ///
    /// Pure, because "does the yard have a ruler in it" is a question a test can answer and a screenshot
    /// cannot.
    /// </summary>
    public static class LandmarkScaleCore
    {
        /// <summary>Rung spacing. Real ladders live in 25–35 cm; this is the middle of the band.</summary>
        public const float RungPitch = 0.3f;

        /// <summary>First rung off the deck — a step up, not a climb.</summary>
        public const float FirstRungHeight = 0.4f;

        /// <summary>How far a rung sticks out from the mast face.</summary>
        public const float RungReach = 0.22f;

        /// <summary>Rung stock thickness.</summary>
        public const float RungThickness = 0.05f;

        /// <summary>
        /// Above this, detail is pixels. The scale cue has already landed at eye level; carrying it to
        /// the top would triple the object count and change nothing the player can see.
        /// </summary>
        public const float DetailCeiling = 7.5f;

        /// <summary>Past this ratio a landmark reads as a pole, whatever its height.</summary>
        public const float MaxSlenderness = 4f;

        /// <summary>Nothing needs to be wider than this to read; past it we are just spending triangles.</summary>
        public const float MaxUsefulWidth = 6f;

        /// <summary>Below this height a landmark is furniture and needs no ruler on it.</summary>
        public const float MinDetailedHeight = 6f;

        /// <summary>Walkway height as a fraction of the mast — mid-body, where the eye already is.</summary>
        private const float WalkwayFraction = 0.45f;

        /// <summary>
        /// True when a landmark is tall enough to need scale cues and thin enough that it will not
        /// read without them. This is the check that catches the shipped crane: 16 × 2 is 8:1.
        /// </summary>
        public static bool ReadsAsStick(float height, float width)
        {
            if (height < MinDetailedHeight) return false;
            if (width <= 0f) return true;
            return height / width > MaxSlenderness;
        }

        /// <summary>
        /// The narrowest width at which a landmark of this height still reads as a structure. Capped:
        /// a 40 m tower does not need to be 10 m thick, it needs to not be a pencil.
        /// </summary>
        public static float MinimumReadableWidth(float height)
        {
            if (height <= 0f) return 0f;
            return Mathf.Min(MaxUsefulWidth, height / MaxSlenderness);
        }

        /// <summary>How many rungs fit under the detail ceiling for a mast of this height.</summary>
        public static int RungCount(float height)
        {
            float top = Mathf.Min(height, DetailCeiling);
            if (top <= FirstRungHeight) return 0;
            return Mathf.FloorToInt((top - FirstRungHeight) / RungPitch) + 1;
        }

        /// <summary>Local Y of rung <paramref name="index"/> above the landmark's base.</summary>
        public static float RungY(int index) => FirstRungHeight + index * RungPitch;

        /// <summary>
        /// Mid-body walkway height, kept under the detail ceiling so the handrail is still legible and
        /// so a very tall tower does not put its only readable feature out of sight.
        /// </summary>
        public static float WalkwayY(float height)
            => Mathf.Min(height * WalkwayFraction, DetailCeiling);

        /// <summary>A one-person operator cab: shoulders-wide, head-high. Another known quantity.</summary>
        public static Vector3 CabSize(float width)
        {
            float w = Mathf.Clamp(width * 0.55f, 1.2f, 2.2f);
            return new Vector3(w, 2.1f, w);
        }

        /// <summary>Cab sits ON the mast top, not inside it.</summary>
        public static float CabY(float height) => height + CabSize(1f).y * 0.5f;

        /// <summary>
        /// Total detail pieces a landmark will add. The builder does not need this; the TEST does —
        /// an unbounded scale pass is a renderer-budget regression wearing a nice hat.
        /// </summary>
        public static int DetailPieceCount(float height, float width)
        {
            if (height < MinDetailedHeight) return 0;
            // rungs + 2 stringers + walkway deck + 4 rail posts + cab
            return RungCount(height) + 8;
        }
    }
}
