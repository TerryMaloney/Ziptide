namespace Ziptide.Content.Automation
{
    /// <summary>
    /// HARDWIRING 4.1j (LAW 6 — the richness bar) — deterministic per-resource puck styling, pure:
    /// the same resource id always rides the belt as the same shape + color on every device, with no
    /// registry to maintain (hash-derived, so EVERY resource in the economy gets a distinct look for
    /// free — catalog breadth by construction). The scene translator maps ShapeIndex to a primitive
    /// and applies the color.
    /// </summary>
    public static class BeltPuckStyle
    {
        public const int ShapeCount = 3; // 0 = cube (ingots), 1 = cylinder (drums), 2 = capsule (pods)

        /// <summary>FNV-1a — stable across platforms/sessions (string.GetHashCode is neither).</summary>
        public static uint Hash(string resourceId)
        {
            uint h = 2166136261u;
            if (resourceId != null)
                for (int i = 0; i < resourceId.Length; i++)
                {
                    h ^= resourceId[i];
                    h *= 16777619u;
                }
            return h;
        }

        public static int ShapeIndex(string resourceId) => (int)(Hash(resourceId) % ShapeCount);

        /// <summary>Color as HSV-ish components in [0,1] — hue from the hash, kept warm-bright so
        /// pucks read against the dark tiles (the scene converts to a Color).</summary>
        public static void ColorOf(string resourceId, out float hue, out float sat, out float val)
        {
            uint h = Hash(resourceId);
            hue = ((h >> 8) & 0x3FF) / 1023f;         // 10 bits of hue — well spread
            sat = 0.45f + ((h >> 18) & 0xFF) / 255f * 0.35f;  // 0.45–0.80: colorful, never neon
            val = 0.70f + ((h >> 26) & 0x3F) / 63f * 0.25f;   // 0.70–0.95: always readable
        }
    }
}
