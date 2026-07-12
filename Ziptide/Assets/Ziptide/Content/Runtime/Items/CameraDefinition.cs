using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// FIELD CAMERA — the handheld vista camera, data-driven like every other item. The capture
    /// camera + viewfinder (a later commit) read <see cref="captureFov"/>; feel/look tune through
    /// the base ItemDefinition fields (visualScale/visualColor/gripLocalEuler). Its `itemId` is
    /// "handheld_camera", which the holster allowlist admits so it travels between worlds.
    /// </summary>
    public class CameraDefinition : ItemDefinition
    {
        [Header("Field Camera")]
        [Tooltip("Capture / viewfinder field of view in degrees.")]
        public float captureFov = 60f;

        [Tooltip("Seconds between shots (the shutter cadence).")]
        public float shutterCooldown = 0.6f;

        [Tooltip("Viewfinder screen tint while idle (the live feed replaces it, later commit).")]
        public Color viewfinderTint = new Color(0.10f, 0.12f, 0.14f, 1f);
    }
}
