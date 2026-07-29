using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>One authored stretch of navigable water: an axis-aligned rectangle on the XZ plane.</summary>
    public struct CanalRect
    {
        public Vector2 Center;
        public Vector2 HalfSize;

        public CanalRect(Vector3 center, Vector2 size)
        {
            Center = new Vector2(center.x, center.z);
            HalfSize = size * 0.5f;
        }
    }

    /// <summary>
    /// THE WATERWAY (⚖ Terry: "the lizard creature interacts with the player's BOAT in the canals").
    /// The canals were decoration — sludge rectangles you walked past. This is the rule that makes
    /// them a route: where the skiff may go, and where it gets pushed back to when it tries to
    /// leave. The city's ring canal is the real highway (a full circuit at canalRingRadius), with
    /// the authored canal rectangles hanging off it.
    ///
    /// Pure, because "can the boat be here" has to be answerable in a test rather than discovered
    /// on a headset by a boat sitting on a plaza. The correction is a PUSH toward water, never a
    /// teleport or a hard stop — a boat that snaps or freezes is a comfort problem, and being
    /// nudged back into the channel reads as the hull grounding out.
    /// </summary>
    public static class CanalWaterCore
    {
        /// <summary>How far outside the water the hull may drift before it is pushed back (m).</summary>
        public const float ShoreTolerance = 1.2f;

        /// <summary>Metres per second the correction pushes at full commitment.</summary>
        public const float CorrectionSpeed = 3.5f;

        /// <summary>Inside the ring canal's channel? ringWidth is the full width of the water.</summary>
        public static bool InRingCanal(Vector3 position, float ringRadius, float ringWidth)
        {
            if (ringRadius <= 0f || ringWidth <= 0f) return false;
            float r = new Vector2(position.x, position.z).magnitude;
            return Mathf.Abs(r - ringRadius) <= ringWidth * 0.5f + ShoreTolerance;
        }

        public static bool InRect(Vector3 position, CanalRect rect)
        {
            float dx = Mathf.Abs(position.x - rect.Center.x);
            float dz = Mathf.Abs(position.z - rect.Center.y);
            return dx <= rect.HalfSize.x + ShoreTolerance && dz <= rect.HalfSize.y + ShoreTolerance;
        }

        public static bool IsNavigable(Vector3 position, IList<CanalRect> rects, float ringRadius, float ringWidth)
        {
            if (InRingCanal(position, ringRadius, ringWidth)) return true;
            if (rects == null) return false;
            for (int i = 0; i < rects.Count; i++)
                if (InRect(position, rects[i])) return true;
            return false;
        }

        /// <summary>
        /// Direction (XZ, normalized; zero when already navigable) to nudge a hull back toward the
        /// nearest water. Prefers the ring canal because it is the through-route: a boat pushed
        /// into a dead-end rectangle would be "rescued" into a trap.
        /// </summary>
        public static Vector3 CorrectionDirection(Vector3 position, IList<CanalRect> rects,
            float ringRadius, float ringWidth)
        {
            if (IsNavigable(position, rects, ringRadius, ringWidth)) return Vector3.zero;

            Vector3 best = Vector3.zero;
            float bestDistance = float.MaxValue;

            if (ringRadius > 0f && ringWidth > 0f)
            {
                Vector2 flat = new Vector2(position.x, position.z);
                float r = flat.magnitude;
                if (r > 0.001f)
                {
                    float d = Mathf.Abs(r - ringRadius);
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        Vector2 toward = flat.normalized * (r > ringRadius ? -1f : 1f);
                        best = new Vector3(toward.x, 0f, toward.y);
                    }
                }
            }

            if (rects != null)
            {
                for (int i = 0; i < rects.Count; i++)
                {
                    Vector2 c = rects[i].Center;
                    Vector2 delta = c - new Vector2(position.x, position.z);
                    float d = delta.magnitude;
                    if (d < bestDistance && d > 0.001f)
                    {
                        bestDistance = d;
                        best = new Vector3(delta.normalized.x, 0f, delta.normalized.y);
                    }
                }
            }

            return best;
        }
    }
}
