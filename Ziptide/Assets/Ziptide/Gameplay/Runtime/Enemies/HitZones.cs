using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>Which part of a creature took a hit, in the creature's own local frame.</summary>
    public enum HitZone { Center, Top, Bottom, Front, Back, Left, Right }

    /// <summary>
    /// Reusable hit-location classifier shared by all creatures. Given a world-space hit point and the
    /// creature's transform, returns which zone was struck (in the creature's local frame, so "Front"
    /// is the creature's forward regardless of how it's facing). Drives location-based death reactions.
    /// </summary>
    public static class HitZones
    {
        /// <summary>
        /// Classify a hit. <paramref name="centerRadius"/> is the local-space radius around the
        /// centroid treated as a dead-center core hit (tune to the creature's size).
        /// </summary>
        public static HitZone Classify(Transform body, Vector3 worldPoint, float centerRadius = 0.08f)
        {
            if (body == null) return HitZone.Center;

            Vector3 local = body.InverseTransformPoint(worldPoint);
            if (local.magnitude <= centerRadius) return HitZone.Center;

            float ax = Mathf.Abs(local.x);
            float ay = Mathf.Abs(local.y);
            float az = Mathf.Abs(local.z);

            if (ay >= ax && ay >= az) return local.y >= 0f ? HitZone.Top : HitZone.Bottom;
            if (ax >= ay && ax >= az) return local.x >= 0f ? HitZone.Right : HitZone.Left;
            return local.z >= 0f ? HitZone.Front : HitZone.Back;
        }
    }
}
