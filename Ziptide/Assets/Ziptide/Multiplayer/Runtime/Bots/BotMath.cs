using System;

namespace Ziptide.Multiplayer.Bots
{
    /// <summary>
    /// Minimal pure vector for the bot brain (the Multiplayer assembly has NO UnityEngine — the scene
    /// layer converts to/from UnityEngine.Vector3 at the seam, same as the PvpNet DTOs).
    /// </summary>
    [Serializable]
    public struct Vec3
    {
        public float X, Y, Z;
        public Vec3(float x, float y, float z) { X = x; Y = y; Z = z; }

        public static readonly Vec3 Zero = new Vec3(0f, 0f, 0f);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator *(Vec3 a, float s) => new Vec3(a.X * s, a.Y * s, a.Z * s);

        public float Length() => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vec3 Normalized()
        {
            float len = Length();
            return len < 1e-6f ? Zero : new Vec3(X / len, Y / len, Z / len);
        }

        /// <summary>Horizontal (XZ) distance — bots reason on the ground plane.</summary>
        public static float FlatDistance(Vec3 a, Vec3 b)
        {
            float dx = a.X - b.X, dz = a.Z - b.Z;
            return (float)Math.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>Perpendicular of the XZ component (left of the direction). For strafing/dodging.</summary>
        public Vec3 FlatPerp() => new Vec3(-Z, 0f, X).Normalized();

        /// <summary>Rotate the XZ component around Y by degrees (for flank-point math).</summary>
        public Vec3 RotatedY(float degrees)
        {
            double r = degrees * Math.PI / 180.0;
            float c = (float)Math.Cos(r), s = (float)Math.Sin(r);
            return new Vec3(X * c - Z * s, Y, X * s + Z * c);
        }
    }

    /// <summary>Deterministic xorshift RNG — the brain must replay identically from a seed.</summary>
    public sealed class BotRng
    {
        private uint _state;
        public BotRng(int seed) { _state = (uint)(seed == 0 ? 2463534242 : seed); }

        public uint NextUint()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        /// <summary>[0, 1)</summary>
        public float NextFloat() => (NextUint() & 0xFFFFFF) / 16777216f;

        /// <summary>[-1, 1)</summary>
        public float NextSigned() => NextFloat() * 2f - 1f;
    }
}
