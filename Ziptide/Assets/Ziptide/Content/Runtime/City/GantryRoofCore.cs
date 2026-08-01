using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// THE ROOF YOU HAVE TO LOOK UP AT (HANGAR_AND_COUPLER_PASS §2.2).
    ///
    /// The berth is a *hangar* and had nothing overhead at all. Vertical layering is one of the four
    /// things that manufacture scale in VR, and it is the one the yard was missing completely:
    /// <b>a space feels big when you have to look up.</b> Nothing above the player means the eye never
    /// leaves the horizontal, and a space read entirely on the horizontal reads flat however wide it is.
    ///
    /// The trap, and the reason this is a core rather than four numbers in a patcher: a roof over the
    /// WHOLE berth would close the sky. The skyscape is the thing Terry named as a reason this game
    /// exists, and the acid haze over W001 was built two days ago — a gantry that quietly seals it off
    /// would be a scale win that costs the game its best asset. So the roof covers the LANDWARD half
    /// only, open seaward, and "does the sky survive this" is a test.
    /// </summary>
    public static class GantryRoofCore
    {
        /// <summary>Trusses overhead. High enough to be architecture, low enough to have to look.</summary>
        public const float TrussHeight = 6.2f;

        /// <summary>Lamps hang below the trusses — and well above a raised hand.</summary>
        public const float LampHeight = 4.6f;

        /// <summary>Cover more of the berth than this and the sky stops being the backdrop.</summary>
        public const float MaxCoveredFraction = 0.55f;

        /// <summary>Enough to read as a structure; few enough to stay cheap.</summary>
        public const int TrussCount = 3;

        /// <summary>Roof columns stand this far inside the berth edge.</summary>
        public const float EdgeInset = 0.6f;

        /// <summary>
        /// The covered span, landward half only. <paramref name="landwardZ"/> is the direction the
        /// player walks off the ramp — the seaward end stays open so the horizon (and the haze on it)
        /// is still what you see from under the roof.
        /// </summary>
        public static void CoveredSpan(float berthCentreZ, float berthDepth, out float fromZ, out float toZ)
        {
            float covered = berthDepth * MaxCoveredFraction;
            toZ = berthCentreZ + berthDepth * 0.5f;   // the landward edge
            fromZ = toZ - covered;
        }

        /// <summary>Z of truss <paramref name="index"/>, spread evenly across the covered span.</summary>
        public static float TrussZ(int index, float fromZ, float toZ)
        {
            if (TrussCount <= 1) return (fromZ + toZ) * 0.5f;
            return Mathf.Lerp(fromZ, toZ, index / (float)(TrussCount - 1));
        }

        /// <summary>Is enough of the berth still open to the sky?</summary>
        public static bool LeavesTheSkyOpen(float coveredDepth, float berthDepth)
        {
            if (berthDepth <= 0.01f) return true;
            return coveredDepth / berthDepth <= MaxCoveredFraction + 0.001f;
        }

        /// <summary>Is the overhead high enough to walk under and low enough to notice?</summary>
        public static bool ReadsAsOverhead(float trussHeight)
            => trussHeight >= 4.5f && trussHeight <= 9f;
    }
}
