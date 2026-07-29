using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>
    /// THE HELM COMPASS RIBBON (LEVEL1_SPATIAL_SCRIPT §2 — "how do we figure out where we're
    /// going"). Open space has no landmarks, so a pilot who turns away from the course has nothing
    /// to steer back by. The ribbon is one strip across the cockpit frame with a marker that slides
    /// to where the next ring is: centered means you're pointed at it, hard left means swing left,
    /// and a distinct BEHIND state means you've overshot — turn around.
    ///
    /// Pure so the whole read is provable without flying: bearing in, ribbon position out. Yaw only
    /// (pitch stays out of it) because a two-axis reticle in a headset reads as a HUD, and this has
    /// to read as an instrument on the ship.
    /// </summary>
    public static class CompassRibbonCore
    {
        /// <summary>Half-width of the ribbon's field, degrees. Past this the marker pins to the end.</summary>
        public const float HalfFieldDegrees = 75f;

        /// <summary>Beyond this the target is BEHIND you and the ribbon says so instead of lying.</summary>
        public const float BehindDegrees = 110f;

        /// <summary>
        /// Signed bearing from forward to the target, degrees: negative = left, positive = right,
        /// zero = dead ahead. Flattened to the ship's horizontal plane.
        /// </summary>
        public static float BearingDegrees(Vector3 forward, Vector3 fromPosition, Vector3 targetPosition)
        {
            Vector3 to = targetPosition - fromPosition;
            to.y = 0f;
            Vector3 fwd = forward;
            fwd.y = 0f;
            if (to.sqrMagnitude < 0.0001f || fwd.sqrMagnitude < 0.0001f) return 0f;
            return Vector3.SignedAngle(fwd.normalized, to.normalized, Vector3.up);
        }

        /// <summary>Marker position on the ribbon, -1 (hard left) .. +1 (hard right), clamped.</summary>
        public static float RibbonOffset(float bearingDegrees)
        {
            return Mathf.Clamp(bearingDegrees / HalfFieldDegrees, -1f, 1f);
        }

        /// <summary>True when the course is behind the pilot — the ribbon shows the turn-around state.</summary>
        public static bool IsBehind(float bearingDegrees) => Mathf.Abs(bearingDegrees) >= BehindDegrees;

        /// <summary>Marker color: on-course green through amber to the behind-you red.</summary>
        public static Color MarkerColor(float bearingDegrees)
        {
            float off = Mathf.Abs(RibbonOffset(bearingDegrees));
            if (IsBehind(bearingDegrees)) return new Color(0.95f, 0.30f, 0.20f);
            return Color.Lerp(new Color(0.35f, 0.95f, 0.50f), new Color(0.95f, 0.75f, 0.25f), off);
        }
    }
}
