using System;

namespace Ziptide.Core
{
    /// <summary>Vertical facade role for a generated building storey.</summary>
    public enum CityFacadeBand
    {
        Base,
        Middle,
        Cap,
    }

    /// <summary>Closed, material-budgeted window vocabulary for City Stage A.</summary>
    public enum CityWindowLight
    {
        Dark,
        HomeWarm,
        ShopCool,
        IndustrialNeutral,
    }

    /// <summary>
    /// Pure deterministic presentation grammar for generated city buildings. The editor translator
    /// consumes these decisions; no Unity random state or wall clock can influence a building.
    /// </summary>
    public static class CityBuildingPresentationCore
    {
        public static CityFacadeBand BandForStorey(int storey, int storeyCount)
        {
            int count = Math.Max(1, storeyCount);
            int clamped = Math.Max(0, Math.Min(storey, count - 1));
            if (clamped == 0) return CityFacadeBand.Base;
            if (clamped == count - 1) return CityFacadeBand.Cap;
            return CityFacadeBand.Middle;
        }

        public static CityWindowLight WindowFor(
            int seed,
            int storey,
            int face,
            int cell,
            float litChance)
        {
            float chance = Math.Max(0f, Math.Min(1f, litChance));
            uint value = Mix(seed, storey, face, cell, 0x9E3779B9u);
            float unit = Unit(value);
            if (unit >= chance) return CityWindowLight.Dark;

            uint warmth = Mix(seed, storey, face, cell, 0x85EBCA6Bu);
            switch (warmth % 3u)
            {
                case 0u: return CityWindowLight.HomeWarm;
                case 1u: return CityWindowLight.ShopCool;
                default: return CityWindowLight.IndustrialNeutral;
            }
        }

        public static int RoofClutterCount(int seed, int minimum, int maximum)
        {
            int min = Math.Max(0, minimum);
            int max = Math.Max(min, maximum);
            if (min == max) return min;
            uint span = (uint)(max - min + 1);
            return min + (int)(Mix(seed, 17, 29, 43, 0xC2B2AE35u) % span);
        }

        public static bool HasAntenna(int seed)
        {
            return (Mix(seed, 3, 5, 7, 0x27D4EB2Fu) & 3u) == 0u;
        }

        public static bool UsesAlternateWall(int seed, int storey, int face)
        {
            return (Mix(seed, storey, face, 11, 0x165667B1u) & 3u) == 0u;
        }

        public static float CrownScale(int seed)
        {
            uint value = Mix(seed, 19, 23, 31, 0xD3A2646Cu);
            return 0.56f + Unit(value) * 0.18f;
        }

        public static float SignedOffset(int seed, int axis)
        {
            uint value = Mix(seed, axis, 37, 41, 0xA24BAED5u);
            return Unit(value) * 2f - 1f;
        }

        private static uint Mix(int seed, int a, int b, int c, uint salt)
        {
            unchecked
            {
                uint x = (uint)seed ^ salt;
                x ^= (uint)a * 0x9E3779B9u;
                x = RotateLeft(x, 13);
                x ^= (uint)b * 0x85EBCA6Bu;
                x = RotateLeft(x, 11);
                x ^= (uint)c * 0xC2B2AE35u;
                x ^= x >> 16;
                x *= 0x7FEB352Du;
                x ^= x >> 15;
                x *= 0x846CA68Bu;
                x ^= x >> 16;
                return x;
            }
        }

        private static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        private static float Unit(uint value)
        {
            return (value & 0x00FFFFFFu) / 16777216f;
        }
    }
}
