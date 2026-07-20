using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Ship
{
    public readonly struct VehicleVisualProfile
    {
        public readonly string Family;
        public readonly Color Body;
        public readonly Color Accent;
        public readonly Color Glow;
        public readonly Vector3 ApproximateBounds;
        public readonly int MinimumParts;

        public VehicleVisualProfile(string family, Color body, Color accent, Color glow,
            Vector3 approximateBounds, int minimumParts)
        {
            Family = family;
            Body = body;
            Accent = accent;
            Glow = glow;
            ApproximateBounds = approximateBounds;
            MinimumParts = minimumParts;
        }
    }

    /// <summary>Pure visual vocabulary for the data-driven ground fleet.</summary>
    public static class VehiclePresentationCore
    {
        public static VehicleVisualProfile Resolve(VehicleArchetype archetype)
        {
            switch (archetype)
            {
                case VehicleArchetype.Skiff:
                    return new VehicleVisualProfile("skiff",
                        new Color(0.20f, 0.42f, 0.48f),
                        new Color(0.90f, 0.58f, 0.16f),
                        new Color(0.20f, 0.90f, 1f),
                        new Vector3(2.4f, 1.7f, 3.2f), 15);
                case VehicleArchetype.DrillCrawler:
                    return new VehicleVisualProfile("crawler",
                        new Color(0.42f, 0.34f, 0.22f),
                        new Color(0.92f, 0.70f, 0.18f),
                        new Color(1f, 0.38f, 0.12f),
                        new Vector3(2.5f, 2.0f, 3.4f), 20);
                case VehicleArchetype.Hoverbike:
                    return new VehicleVisualProfile("hoverbike",
                        new Color(0.32f, 0.25f, 0.48f),
                        new Color(0.20f, 0.78f, 0.95f),
                        new Color(0.45f, 0.90f, 1f),
                        new Vector3(1.7f, 1.6f, 3.0f), 14);
                default:
                    return new VehicleVisualProfile("utility",
                        new Color(0.35f, 0.38f, 0.42f),
                        new Color(0.82f, 0.56f, 0.20f),
                        new Color(0.35f, 0.80f, 1f),
                        new Vector3(2.2f, 1.8f, 3.0f), 12);
            }
        }
    }
}
