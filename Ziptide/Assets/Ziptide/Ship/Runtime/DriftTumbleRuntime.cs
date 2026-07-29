using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>
    /// Slow deterministic tumble for scrap debris. Zero-g junk that hangs perfectly still reads as
    /// level geometry; a lazy rotation reads as a place where something broke. Rates derive from
    /// the object's own start position so a field of these never turns in unison, and no rate is
    /// fast enough to fight the comfort rules — this is drift, not spin.
    /// </summary>
    public class DriftTumbleRuntime : MonoBehaviour
    {
        /// <summary>Degrees per second, per axis, at the fastest.</summary>
        public const float MaxRateDegrees = 9f;

        private Vector3 _rate;

        private void Awake()
        {
            _rate = RateFor(transform.localPosition);
        }

        private void Update()
        {
            transform.localRotation *= Quaternion.Euler(_rate * Time.deltaTime);
        }

        /// <summary>Pure: a stable per-object tumble rate, each axis within ±MaxRateDegrees.</summary>
        public static Vector3 RateFor(Vector3 seedPosition)
        {
            float a = Mathf.Sin(seedPosition.x * 12.9898f + seedPosition.z * 78.233f);
            float b = Mathf.Sin(seedPosition.y * 39.3467f + seedPosition.x * 4.1414f);
            float c = Mathf.Sin(seedPosition.z * 27.1828f + seedPosition.y * 31.4159f);
            return new Vector3(a, b, c) * MaxRateDegrees;
        }
    }
}
