using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// THE TRACTOR PULL — the confirmation the salvage loop never had. Flying close to a downed
    /// drone paid resources silently, so on device the only way to know salvage existed was to
    /// read the log. Now the wreck answers: it brightens as you close (the approach cue), and the
    /// moment it pays out, a short tractor draw runs from the wreck to the cockpit with a pluck.
    ///
    /// Tracer-built and procedurally voiced like every other effect here — no assets, nothing
    /// parented to the head, nothing that outlives its own draw.
    /// </summary>
    public static class SalvageTractorFx
    {
        private const int Strands = 6;
        private static readonly Color Beam = new Color(0.45f, 0.9f, 0.75f, 0.9f);
        private static AudioClip _pluck;

        /// <summary>Draw the pull from a wreck to the cockpit and voice it once.</summary>
        public static void Play(Vector3 fromWorld, Vector3 toWorld)
        {
            for (int i = 0; i < Strands; i++)
            {
                // Strands bow outward slightly so the pull reads as a beam with volume rather
                // than one hairline between two points.
                Vector3 mid = Vector3.Lerp(fromWorld, toWorld, 0.5f) + Random.insideUnitSphere * 0.9f;
                TracerFx.Spawn(fromWorld, mid, Beam, 0.03f, 0.28f);
                TracerFx.Spawn(mid, toWorld, Beam, 0.03f, 0.28f);
            }

            if (_pluck == null) _pluck = MakePluck();
            if (_pluck != null) AudioSource.PlayClipAtPoint(_pluck, toWorld, 0.6f);
        }

        /// <summary>0.35 s rising pluck — a "got it" that can't be confused with taking a hit.</summary>
        private static AudioClip MakePluck()
        {
            const int rate = 22050;
            int n = (int)(rate * 0.35f);
            var s = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                phase += Mathf.Lerp(420f, 900f, t * t) / rate;   // rises: acquisition, not damage
                float env = Mathf.Exp(-5.5f * t);
                s[i] = Mathf.Sin(2f * Mathf.PI * phase) * env * 0.6f;
            }
            var clip = AudioClip.Create("SalvagePluck", n, 1, rate, false);
            clip.SetData(s, 0);
            return clip;
        }
    }
}
