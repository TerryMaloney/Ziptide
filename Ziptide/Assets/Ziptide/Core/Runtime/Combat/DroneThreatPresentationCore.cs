using UnityEngine;

namespace Ziptide.Core
{
    public readonly struct DroneThreatPresentation
    {
        public readonly bool Visible;
        public readonly bool ShowAimLine;
        public readonly float OrbScale;
        public readonly float LineWidth;
        public readonly float Intensity;

        public DroneThreatPresentation(bool visible, bool showAimLine,
            float orbScale, float lineWidth, float intensity)
        {
            Visible = visible;
            ShowAimLine = showAimLine;
            OrbScale = orbScale;
            LineWidth = lineWidth;
            Intensity = intensity;
        }
    }

    /// <summary>
    /// Pure presentation mapping for the existing drone combat FSM. It owns no timing or decisions;
    /// it converts normalized telegraph progress and the short post-fire flash flag into bounded visuals.
    /// </summary>
    public static class DroneThreatPresentationCore
    {
        public const float AimLineThreshold = 0.28f;

        public static DroneThreatPresentation Resolve(float telegraphProgress,
            bool isTelegraphing, bool shotFlash)
        {
            if (shotFlash)
                return new DroneThreatPresentation(true, false, 0.42f, 0.022f, 2.4f);
            if (!isTelegraphing)
                return new DroneThreatPresentation(false, false, 0f, 0f, 0f);

            float p = Mathf.Clamp01(telegraphProgress);
            float eased = p * p * (3f - 2f * p);
            return new DroneThreatPresentation(
                true,
                p >= AimLineThreshold,
                Mathf.Lerp(0.07f, 0.35f, eased),
                Mathf.Lerp(0.003f, 0.012f, eased),
                Mathf.Lerp(0.55f, 2.0f, eased));
        }
    }
}
