using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure observation check (CI-testable) for gaze-reactive creatures (Witness-mite, Light-grazer).
    /// "Observed" = inside the viewer's forward cone AND within range — the graybox stand-in for
    /// eye-tracking/scanner observation.
    /// </summary>
    public static class GazeMath
    {
        /// <param name="viewForward">Viewer's normalized forward.</param>
        /// <param name="toTarget">Vector from viewer to target (not normalized).</param>
        /// <param name="cosHalfFov">Cosine of the half-FOV that counts as "looking at" (e.g. 0.5 = 60°).</param>
        /// <param name="maxRange">Beyond this it's unobserved regardless of angle.</param>
        public static bool IsObserved(Vector3 viewForward, Vector3 toTarget, float cosHalfFov, float maxRange)
        {
            float dist = toTarget.magnitude;
            if (dist > maxRange || dist < 0.0001f) return false;
            float cos = Vector3.Dot(viewForward.normalized, toTarget / dist);
            return cos >= cosHalfFov;
        }
    }
}
