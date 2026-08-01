using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>What a piece of quay junk is. Shape only — the author picks the geometry.</summary>
    public enum ClutterKind
    {
        Crate = 0,
        CableSpool = 1,
        Toolbox = 2,
    }

    /// <summary>One placed piece, in the walk's own space (Z along the walk, X across it).</summary>
    public readonly struct ClutterPiece
    {
        public readonly Vector3 Position;
        public readonly float Size;
        public readonly ClutterKind Kind;

        public ClutterPiece(Vector3 position, float size, ClutterKind kind)
        {
            Position = position; Size = size; Kind = kind;
        }
    }

    /// <summary>
    /// THE FOREGROUND BAND — the third of the three depth bands a space needs before it reads as a
    /// place (HANGAR_AND_COUPLER_PASS §2.4). The quay had midground (berths) and background (skyline)
    /// and nothing at all within arm's reach, which is why it felt like a diagram of a shipyard rather
    /// than a shipyard.
    ///
    /// The interesting part is not the scatter, it is the CORRIDOR. Junk in a doorway is the cheapest
    /// way to turn a first walk into a soft-lock, and in VR it is worse than in a flat game: the player
    /// cannot see their own feet, so a knee-high crate on the walking line is invisible until it stops
    /// them. So placement is a pure function with a clear lane down the middle, and the law is a test
    /// rather than a habit.
    ///
    /// Deterministic without UnityEngine.Random on purpose — the bake must be reproducible, and a
    /// scatter that shifts every time someone re-patches makes "did I break the walk" unanswerable.
    /// </summary>
    public static class ApproachClutterCore
    {
        /// <summary>
        /// Half-width of the lane that stays empty. This is not an arbitrary comfort number — it is
        /// <b>the ship's beam plus clearance</b>. The walk being dressed is the berth deck, and the
        /// thing standing in the middle of it is your own hull, so the lane the player actually uses
        /// runs down the flanks and everything inside this is already occupied.
        /// </summary>
        public const float CorridorHalfWidth = 3f;

        /// <summary>
        /// Past this the piece is scenery, not clutter — it no longer passes within reach. Also keeps
        /// junk off the deck edge and clear of the gantry columns at ±9.4 m.
        /// </summary>
        public const float MaxOffset = 4.6f;

        /// <summary>The teaching crate is hand-sized, because the lesson is "your hands work".</summary>
        public const float TeachingCrateSize = 0.4f;

        /// <summary>Enough to dress a walk, few enough to stay inside the renderer budget.</summary>
        public const int PieceCount = 9;

        public const float MinSize = 0.35f;
        public const float MaxSize = 0.9f;

        /// <summary>
        /// Places <see cref="PieceCount"/> pieces between two points on the walk, alternating sides so
        /// the eye gets parallax on both flanks rather than a hedge down one.
        /// </summary>
        public static List<ClutterPiece> Place(float centreX, float fromZ, float toZ)
        {
            var pieces = new List<ClutterPiece>();
            if (Mathf.Approximately(fromZ, toZ)) return pieces;

            uint seed = 0x5EED_C0DEu;
            for (int i = 0; i < PieceCount; i++)
            {
                // Spread along the walk, never right at either end — the ramp foot and the district
                // mouth are exactly where a player is turning, and that is the worst place to trip.
                float t = (i + 1f) / (PieceCount + 1f);
                float z = Mathf.Lerp(fromZ, toZ, t);

                float side = (i % 2 == 0) ? -1f : 1f;
                float size = Mathf.Lerp(MinSize, MaxSize, Next01(ref seed));
                var kind = (ClutterKind)(i % 3);
                Vector3 footprint = Footprint(kind, size);

                // Offset the piece's NEAR EDGE past the corridor, not its centre — and measure that
                // edge off the FOOTPRINT, not the nominal size. Both of those were bugs first: placing
                // centres put half of a 0.9 m crate back in the lane, and using `size` for a toolbox
                // that the author then stretches 1.5x across the walk put the rest of it back.
                // The +2 cm is not fussiness: without it the nearest legal placement sits EXACTLY on
                // the corridor edge, and whether that counts as clear comes down to float rounding.
                float half = footprint.x * 0.5f;
                float lo = CorridorHalfWidth + half + 0.02f;
                float hi = MaxOffset - half;
                float offset = hi > lo ? Mathf.Lerp(lo, hi, Next01(ref seed)) : lo;

                pieces.Add(new ClutterPiece(
                    new Vector3(centreX + side * offset, footprint.y * 0.5f, z),
                    size,
                    kind));
            }
            return pieces;
        }

        /// <summary>
        /// The piece's actual box, which is NOT a cube. Silhouette is the only thing telling junk
        /// apart at stand-in fidelity: a spool is a wide, squat drum and a toolbox is long and low.
        /// The core owns this rather than the author because the corridor law has to measure the
        /// shape that gets built, not the number it was derived from.
        /// </summary>
        public static Vector3 Footprint(ClutterKind kind, float size)
        {
            switch (kind)
            {
                case ClutterKind.CableSpool: return new Vector3(size * 1.3f, size * 0.75f, size * 1.3f);
                case ClutterKind.Toolbox: return new Vector3(size * 1.5f, size * 0.5f, size * 0.7f);
                default: return new Vector3(size, size, size);
            }
        }

        /// <summary>Footprint of an already-placed piece.</summary>
        public static Vector3 Footprint(ClutterPiece p) => Footprint(p.Kind, p.Size);

        /// <summary>
        /// Where the ONE grabbable, worthless crate goes: on the walk, inside reach, still outside the
        /// corridor. A player who picks up a crate that does nothing has learned the grab verb at zero
        /// cost and without being told — which is the whole trick (§2.5).
        /// </summary>
        public static Vector3 TeachingCratePosition(float centreX, float fromZ, float toZ)
        {
            // Early on the walk — the lesson is worth nothing if it lands after the player has already
            // walked past everything grabbable.
            float z = Mathf.Lerp(fromZ, toZ, 0.22f);
            float offset = CorridorHalfWidth + TeachingCrateSize * 0.5f + 0.1f;
            return new Vector3(centreX + offset, TeachingCrateSize * 0.5f + 0.05f, z);
        }

        /// <summary>Is this piece standing in the lane the player walks down?</summary>
        public static bool BlocksTheWalk(Vector3 position, float centreX, float size)
            => Mathf.Abs(position.x - centreX) - size * 0.5f < CorridorHalfWidth;

        /// <summary>xorshift — small, deterministic, and not UnityEngine.Random's global state.</summary>
        private static float Next01(ref uint state)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return (state & 0xFFFFFF) / (float)0x1000000;
        }
    }
}
