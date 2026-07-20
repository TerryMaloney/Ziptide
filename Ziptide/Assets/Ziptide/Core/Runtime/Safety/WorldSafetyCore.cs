using UnityEngine;

namespace Ziptide.Core
{
    public readonly struct WorldSafetyDecision
    {
        public readonly bool ShouldRecover;
        public readonly bool HasSafePose;
        public readonly Vector3 SafePosition;
        public readonly Quaternion SafeRotation;
        public readonly string Reason;
        public readonly float UnsupportedSeconds;

        public WorldSafetyDecision(bool shouldRecover, bool hasSafePose, Vector3 safePosition,
            Quaternion safeRotation, string reason, float unsupportedSeconds)
        {
            ShouldRecover = shouldRecover;
            HasSafePose = hasSafePose;
            SafePosition = safePosition;
            SafeRotation = safeRotation;
            Reason = reason;
            UnsupportedSeconds = unsupportedSeconds;
        }
    }

    /// <summary>
    /// Pure unsupported-space recovery state. Supported poses continuously become the safe return point.
    /// A brief unsupported interval is legal (jumping, stepping over a seam); recovery is requested only
    /// after both a grace period and a meaningful drop, or immediately below the world's hard fall floor.
    /// </summary>
    public sealed class WorldSafetyCore
    {
        public const float DefaultUnsupportedGrace = 0.55f;
        public const float DefaultMinimumDrop = 0.85f;

        public float UnsupportedGraceSeconds { get; }
        public float MinimumDropMeters { get; }
        public bool HasSafePose { get; private set; }
        public Vector3 LastSafePosition { get; private set; }
        public Quaternion LastSafeRotation { get; private set; } = Quaternion.identity;
        public float UnsupportedSeconds { get; private set; }

        public WorldSafetyCore(float unsupportedGraceSeconds = DefaultUnsupportedGrace,
            float minimumDropMeters = DefaultMinimumDrop)
        {
            UnsupportedGraceSeconds = Mathf.Max(0.05f, unsupportedGraceSeconds);
            MinimumDropMeters = Mathf.Max(0.10f, minimumDropMeters);
        }

        public void ObserveSupported(Vector3 position, Quaternion rotation)
        {
            HasSafePose = true;
            LastSafePosition = position;
            LastSafeRotation = rotation;
            UnsupportedSeconds = 0f;
        }

        public WorldSafetyDecision Tick(Vector3 currentPosition, Quaternion currentRotation,
            bool supported, float deltaTime, float hardFallY)
        {
            if (supported)
            {
                ObserveSupported(currentPosition, currentRotation);
                return None();
            }

            UnsupportedSeconds += Mathf.Max(0f, deltaTime);
            bool belowHardFloor = currentPosition.y < hardFallY;
            bool droppedFromSafe = HasSafePose &&
                                   LastSafePosition.y - currentPosition.y >= MinimumDropMeters;
            bool gapFall = HasSafePose &&
                           UnsupportedSeconds >= UnsupportedGraceSeconds &&
                           droppedFromSafe;

            if (!belowHardFloor && !gapFall) return None();

            return new WorldSafetyDecision(
                true,
                HasSafePose,
                LastSafePosition,
                LastSafeRotation,
                belowHardFloor ? "hard_floor" : "unsupported_gap",
                UnsupportedSeconds);
        }

        public void ResetAfterRecovery(Vector3 recoveredPosition, Quaternion recoveredRotation)
        {
            ObserveSupported(recoveredPosition, recoveredRotation);
        }

        private WorldSafetyDecision None()
        {
            return new WorldSafetyDecision(false, HasSafePose, LastSafePosition,
                LastSafeRotation, string.Empty, UnsupportedSeconds);
        }
    }
}
