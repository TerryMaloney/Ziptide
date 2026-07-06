using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The arena's shared "did you hear that" channel (MP100 wave 1). Weapons report a position when
    /// they make noise (a melee swing, a thump, a net landing); bots poll the latest report during
    /// their perception tick and feed it to <c>BotPerception.HeardFire</c> — which finally wires the
    /// hook that shipped with the A1 bot brain but was never fed ("HeardFire = false, // hook").
    /// Same same-frame static-channel idiom as <see cref="PvpHitSource"/>; single-writer-latest-wins
    /// is fine because a bot investigating ANY recent noise is correct behavior.
    /// </summary>
    public static class PvpNoise
    {
        private static Vector3 _lastPos;
        private static float _lastAt = float.NegativeInfinity;

        public static void Report(Vector3 position)
        {
            _lastPos = position;
            _lastAt = Time.time;
        }

        /// <summary>The most recent noise, if it's still fresh (PvpRules.NoiseFreshSeconds).</summary>
        public static bool TryGetRecent(out Vector3 position, out float age)
        {
            age = Time.time - _lastAt;
            position = _lastPos;
            return age <= (float)PvpRules.NoiseFreshSeconds;
        }
    }
}
