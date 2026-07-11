using System;

namespace Ziptide.Gameplay.Tutorial
{
    /// <summary>Small engine-free vector used by first-hour observation math.</summary>
    public readonly struct FirstHourObservationVector
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public FirstHourObservationVector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static FirstHourObservationVector operator -(
            FirstHourObservationVector a,
            FirstHourObservationVector b)
        {
            return new FirstHourObservationVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }

    /// <summary>
    /// FH-S01 pure observation math. It owns no Unity objects, clock, input, travel, profile or
    /// presentation state. Callers provide samples and elapsed duration explicitly.
    /// </summary>
    public sealed class FirstHourObservationCore
    {
        public const double DefaultLookConeDegrees = 18d;
        public const double DefaultLookDwellSeconds = 0.75d;
        public const double MovementCumulativeMeters = 1.5d;
        public const double MovementNetMeters = 1.0d;
        public const double ArrivalDwellSeconds = 3d;
        public const double ArrivalDirectionDegrees = 25d;

        private readonly double _lookConeDegrees;
        private readonly double _lookDwellRequired;

        private double _lookDwell;
        private bool _lookComplete;

        private bool _movementStarted;
        private FirstHourObservationVector _movementOrigin;
        private FirstHourObservationVector _movementLast;
        private double _movementCumulative;
        private double _movementNet;
        private bool _movementComplete;

        private bool _arrivalStarted;
        private FirstHourObservationVector _arrivalInitialForward;
        private double _arrivalElapsed;
        private double _arrivalMaxDirectionChange;
        private bool _arrivalComplete;

        public double LookDwellSeconds => _lookDwell;
        public double LookProgress01 => Clamp01(_lookDwell / _lookDwellRequired);
        public bool LookComplete => _lookComplete;

        public double MovementCumulativeDistance => _movementCumulative;
        public double MovementNetDistance => _movementNet;
        public double MovementProgress01 => Clamp01(Math.Max(
            _movementCumulative / MovementCumulativeMeters,
            _movementNet / MovementNetMeters));
        public bool MovementComplete => _movementComplete;

        public double ArrivalElapsedSeconds => _arrivalElapsed;
        public double ArrivalMaxDirectionChangeDegrees => _arrivalMaxDirectionChange;
        public double ArrivalProgress01 => Clamp01(Math.Min(
            _arrivalElapsed / ArrivalDwellSeconds,
            _arrivalMaxDirectionChange / ArrivalDirectionDegrees));
        public bool ArrivalComplete => _arrivalComplete;

        public FirstHourObservationCore(
            double lookConeDegrees = DefaultLookConeDegrees,
            double lookDwellSeconds = DefaultLookDwellSeconds)
        {
            _lookConeDegrees = IsFinitePositive(lookConeDegrees)
                ? Math.Min(180d, lookConeDegrees)
                : DefaultLookConeDegrees;
            _lookDwellRequired = IsFinitePositive(lookDwellSeconds)
                ? lookDwellSeconds
                : DefaultLookDwellSeconds;
        }

        public void ResetLook()
        {
            _lookDwell = 0d;
            _lookComplete = false;
        }

        public bool SampleLook(
            FirstHourObservationVector headForward,
            FirstHourObservationVector directionToTarget,
            double deltaSeconds)
        {
            if (_lookComplete) return true;

            double angle;
            if (!TryPlanarOrSpatialAngle(headForward, directionToTarget, planar: false, out angle) ||
                angle > _lookConeDegrees)
            {
                _lookDwell = 0d;
                return false;
            }

            _lookDwell += SafeDelta(deltaSeconds);
            if (_lookDwell < _lookDwellRequired) return false;

            _lookDwell = _lookDwellRequired;
            _lookComplete = true;
            return true;
        }

        public void ResetMovement()
        {
            _movementStarted = false;
            _movementOrigin = default;
            _movementLast = default;
            _movementCumulative = 0d;
            _movementNet = 0d;
            _movementComplete = false;
        }

        public bool SampleMovement(FirstHourObservationVector worldPosition)
        {
            if (_movementComplete) return true;
            if (!IsFinite(worldPosition)) return false;

            if (!_movementStarted)
            {
                _movementStarted = true;
                _movementOrigin = worldPosition;
                _movementLast = worldPosition;
                return false;
            }

            _movementCumulative += PlanarDistance(_movementLast, worldPosition);
            _movementNet = PlanarDistance(_movementOrigin, worldPosition);
            _movementLast = worldPosition;

            if (_movementCumulative < MovementCumulativeMeters && _movementNet < MovementNetMeters)
                return false;

            _movementComplete = true;
            return true;
        }

        public void ResetArrival()
        {
            _arrivalStarted = false;
            _arrivalInitialForward = default;
            _arrivalElapsed = 0d;
            _arrivalMaxDirectionChange = 0d;
            _arrivalComplete = false;
        }

        public bool SampleArrival(FirstHourObservationVector headForward, double deltaSeconds)
        {
            if (_arrivalComplete) return true;

            FirstHourObservationVector planar = new FirstHourObservationVector(
                headForward.X,
                0d,
                headForward.Z);
            FirstHourObservationVector normalizedPlanar;
            if (!TryNormalize(planar, out normalizedPlanar)) return false;

            if (!_arrivalStarted)
            {
                _arrivalStarted = true;
                _arrivalInitialForward = normalizedPlanar;
            }

            _arrivalElapsed += SafeDelta(deltaSeconds);

            double angle;
            if (TryPlanarOrSpatialAngle(
                _arrivalInitialForward,
                normalizedPlanar,
                planar: true,
                out angle))
            {
                _arrivalMaxDirectionChange = Math.Max(_arrivalMaxDirectionChange, angle);
            }

            if (_arrivalElapsed < ArrivalDwellSeconds ||
                _arrivalMaxDirectionChange < ArrivalDirectionDegrees)
            {
                return false;
            }

            _arrivalElapsed = Math.Max(_arrivalElapsed, ArrivalDwellSeconds);
            _arrivalComplete = true;
            return true;
        }

        private static bool TryPlanarOrSpatialAngle(
            FirstHourObservationVector a,
            FirstHourObservationVector b,
            bool planar,
            out double angleDegrees)
        {
            if (planar)
            {
                a = new FirstHourObservationVector(a.X, 0d, a.Z);
                b = new FirstHourObservationVector(b.X, 0d, b.Z);
            }

            FirstHourObservationVector normalizedA;
            FirstHourObservationVector normalizedB;
            if (!TryNormalize(a, out normalizedA) || !TryNormalize(b, out normalizedB))
            {
                angleDegrees = 0d;
                return false;
            }

            double dot = normalizedA.X * normalizedB.X +
                         normalizedA.Y * normalizedB.Y +
                         normalizedA.Z * normalizedB.Z;
            dot = Math.Max(-1d, Math.Min(1d, dot));
            angleDegrees = Math.Acos(dot) * (180d / Math.PI);
            return true;
        }

        private static bool TryNormalize(
            FirstHourObservationVector value,
            out FirstHourObservationVector normalized)
        {
            if (!IsFinite(value))
            {
                normalized = default;
                return false;
            }

            double magnitudeSquared = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            if (magnitudeSquared <= 0.000000000001d)
            {
                normalized = default;
                return false;
            }

            double inverse = 1d / Math.Sqrt(magnitudeSquared);
            normalized = new FirstHourObservationVector(
                value.X * inverse,
                value.Y * inverse,
                value.Z * inverse);
            return true;
        }

        private static double PlanarDistance(
            FirstHourObservationVector a,
            FirstHourObservationVector b)
        {
            double x = b.X - a.X;
            double z = b.Z - a.Z;
            return Math.Sqrt(x * x + z * z);
        }

        private static bool IsFinite(FirstHourObservationVector value)
        {
            return IsFinite(value.X) && IsFinite(value.Y) && IsFinite(value.Z);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static bool IsFinitePositive(double value)
        {
            return IsFinite(value) && value > 0d;
        }

        private static double SafeDelta(double value)
        {
            return IsFinite(value) && value > 0d ? value : 0d;
        }

        private static double Clamp01(double value)
        {
            if (double.IsNaN(value)) return 0d;
            return Math.Max(0d, Math.Min(1d, value));
        }
    }
}
