using UnityEngine;

namespace Ziptide.Content
{
    public enum TurnMode { Smooth, Snap }

    /// <summary>
    /// Data-driven locomotion settings applied by LocomotionDirector at runtime.
    /// Swap profiles to change controls per-world without touching gameplay code.
    /// </summary>
    [CreateAssetMenu(fileName = "LocomotionProfile", menuName = "Ziptide/Locomotion Profile")]
    public class LocomotionProfile : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Base walk speed m/s. Test Day 1: 1.75 felt like wading — Fortnite-class base is ~3.")]
        public float moveSpeed = 3f;
        public bool useGravity = true;
        public bool enableStrafe = false;

        [Header("Sprint (hold/click LEFT thumbstick)")]
        [Tooltip("Sprint speed = moveSpeed x this. ~2.0 lands near Fortnite sprint (6 m/s).")]
        public float sprintMultiplier = 2f;

        [Header("Crouch / Slide / Auto-run (CONTROL_SCHEME.md)")]
        [Tooltip("Crouched move speed = moveSpeed x this.")]
        public float crouchSpeedFactor = 0.55f;
        [Tooltip("Slide entry speed = sprint speed x this, decaying over slideSeconds.")]
        public float slideBoost = 1.35f;
        [Tooltip("How long a slide carries before settling into the crouch.")]
        public float slideSeconds = 0.8f;
        [Tooltip("Double-click window on L3 that toggles auto-run (seconds).")]
        public float autoRunDoubleTapWindow = 0.35f;

        [Header("Turning")]
        public TurnMode turnMode = TurnMode.Smooth;
        public float smoothTurnSpeed = 120f;
        public float snapTurnAngle = 45f;

        [Header("Dash / Hop")]
        public bool dashEnabled = true;
        public float dashDistance = 3f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 0.5f;
        public float dashVerticalLift = 0.1f;
    }
}
