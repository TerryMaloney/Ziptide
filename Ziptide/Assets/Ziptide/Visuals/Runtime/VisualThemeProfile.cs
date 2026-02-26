using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Data-driven world visuals: sky, planet, ground tint. Swappable without changing gameplay.
    /// </summary>
    public class VisualThemeProfile : ScriptableObject
    {
        [Header("Ground")]
        [Tooltip("Tint applied to the ground plane material.")]
        public Color groundTint = new Color(0.45f, 0.52f, 0.45f, 1f);

        [Header("Sky")]
        [Tooltip("Gradient for sky sphere (vertical: bottom = horizon, top = zenith).")]
        public Gradient skyGradient;

        [Header("Planet")]
        public PlanetSettings planet = new PlanetSettings();

        private void OnValidate()
        {
            if (planet.direction.sqrMagnitude > 0.01f)
                planet.direction = planet.direction.normalized;
        }

        [System.Serializable]
        public class PlanetSettings
        {
            [Tooltip("Main planet color.")]
            public Color baseColor = new Color(0.4f, 0.5f, 0.7f, 1f);

            [Tooltip("Accent/secondary color (e.g. stripes).")]
            public Color accentColor = new Color(0.25f, 0.35f, 0.5f, 1f);

            [Tooltip("Apparent size in degrees.")]
            [Range(1f, 60f)]
            public float angularSizeDegrees = 15f;

            [Tooltip("Distance from player.")]
            public float distance = 50f;

            [Tooltip("Direction from player (normalized).")]
            public Vector3 direction = new Vector3(0f, 0.5f, 0.866f);

            [Tooltip("Rotation speed around local up (deg/s).")]
            public float rotationSpeed = 5f;

            [Tooltip("If true, planet follows player so it stays in sky.")]
            public bool followPlayer = true;
        }
    }
}
