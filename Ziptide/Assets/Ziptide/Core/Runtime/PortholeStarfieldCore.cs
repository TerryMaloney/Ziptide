namespace Ziptide.Core
{
    /// <summary>
    /// THE PORTHOLE (LEVEL1_SPATIAL_SCRIPT §1 — the "no space in a space game" fix). Cal wakes up
    /// aboard a ship in W000 and, until now, could not see space from inside it: the first hour's
    /// opening room had no window, so the game's premise was invisible for its first ten minutes.
    ///
    /// This bakes the view through that window: a deterministic starfield with a faint galactic
    /// band, generated rather than authored so it costs no assets and always matches the Moss
    /// system's night sky. Pure and side-effect free (fills a caller-owned byte buffer, RGBA) so
    /// EditMode tests can prove star density and determinism without a texture or a scene.
    /// </summary>
    public static class PortholeStarfieldCore
    {
        /// <summary>Fraction of pixels that become stars at density 1.</summary>
        public const float MaxStarFraction = 0.02f;

        /// <summary>
        /// Fill <paramref name="rgba"/> (length width*height*4) with the porthole view.
        /// Deterministic in (width, height, seed, density).
        /// </summary>
        public static void Bake(byte[] rgba, int width, int height, int seed, float density)
        {
            if (rgba == null || width <= 0 || height <= 0) return;
            if (rgba.Length < width * height * 4) return;
            if (density < 0f) density = 0f;
            if (density > 1f) density = 1f;

            var rng = new System.Random(seed);
            float bandCenter = height * 0.52f;
            float bandWidth = height * 0.22f;

            for (int y = 0; y < height; y++)
            {
                // The galactic band: a soft brightening across the middle of the view.
                float d = (y - bandCenter) / bandWidth;
                float band = (float)System.Math.Exp(-d * d) * 0.16f;

                for (int x = 0; x < width; x++)
                {
                    int i = (y * width + x) * 4;
                    float r = 0.012f + band * 0.55f;
                    float g = 0.016f + band * 0.60f;
                    float b = 0.030f + band * 0.85f;

                    if (rng.NextDouble() < MaxStarFraction * density)
                    {
                        // Stars vary: most are faint, a few carry the view.
                        float mag = 0.35f + (float)rng.NextDouble() * 0.65f;
                        r = g = b = mag;
                        b = mag > 0.85f ? mag : mag * 0.96f + 0.04f; // a touch of blue-white
                    }

                    rgba[i] = ToByte(r);
                    rgba[i + 1] = ToByte(g);
                    rgba[i + 2] = ToByte(b);
                    rgba[i + 3] = 255;
                }
            }
        }

        /// <summary>How many pixels the bake lit above <paramref name="threshold"/> (0..255).</summary>
        public static int CountBright(byte[] rgba, int width, int height, byte threshold)
        {
            if (rgba == null) return 0;
            int n = 0;
            int px = width * height;
            for (int i = 0; i < px; i++)
                if (rgba[i * 4] >= threshold) n++;
            return n;
        }

        private static byte ToByte(float v)
        {
            if (v <= 0f) return 0;
            if (v >= 1f) return 255;
            return (byte)(v * 255f);
        }
    }
}
