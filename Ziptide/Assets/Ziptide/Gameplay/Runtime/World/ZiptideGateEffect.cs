using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE ZIPTIDE — the game's namesake moment (Terry: "like Stargate — every time the gate
    /// opens it's still awesome"). Wraps EVERY scene travel via TravelCoordinator:
    ///
    /// DEPARTURE (~1.6 s): a tide of teal energy pillars rises out of the floor in a ring around
    /// the player, orbiting faster and faster while the ring CONTRACTS — a whirlpool stood on its
    /// edge — streak lines spiral in, everything accelerates to a white-hot crest... and the scene
    /// cut happens INSIDE the flash, so the hard load reads as the tide taking you.
    /// ARRIVAL (~1.0 s): the same tide recedes — pillars burst outward from around you, slowing
    /// and sinking as the new world fades in through them.
    ///
    /// All primitive-built + unlit (zero assets), world-anchored (the camera NEVER moves — VR
    /// comfort law), with a procedurally-generated audio riser/boom (no clips exist yet; the
    /// samples are deterministic). Cheap: ~40 renderers for under 2 seconds, then gone.
    /// Logs ZIPTIDE_GATE depart/arrive.
    /// </summary>
    public class ZiptideGateEffect : MonoBehaviour
    {
        private const int Pillars = 26;
        private static readonly Color Teal = new Color(0.30f, 0.85f, 0.95f);
        private static readonly Color Crest = new Color(0.85f, 0.98f, 1f);
        private static AudioClip _riser, _boom;

        private Transform[] _pillars;
        private Material _mat;
        private Vector3 _center;
        private float _t, _duration;
        private bool _departure;
        private float _streakTimer;

        /// <summary>Start the departure tide around <paramref name="center"/>. Returns the lead
        /// time until the crest — the caller cuts the scene exactly then.</summary>
        public static float PlayDeparture(Vector3 center)
        {
            Spawn(center, departure: true, duration: 1.6f);
            Debug.Log("ZIPTIDE: ZIPTIDE_GATE depart");
            return 1.45f; // cut just inside the crest flash
        }

        /// <summary>The receding tide in the new world, around the arrival point.</summary>
        public static void PlayArrival(Vector3 center)
        {
            Spawn(center, departure: false, duration: 1.0f);
            Debug.Log("ZIPTIDE: ZIPTIDE_GATE arrive");
        }

        private static void Spawn(Vector3 center, bool departure, float duration)
        {
            var go = new GameObject(departure ? "__ZiptideDepart" : "__ZiptideArrive");
            var fx = go.AddComponent<ZiptideGateEffect>();
            fx._center = center;
            fx._departure = departure;
            fx._duration = duration;
            fx.Build();
            fx.PlayAudio();
        }

        private void Build()
        {
            _mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            _mat.SetColor("_BaseColor", Teal);

            _pillars = new Transform[Pillars];
            for (int i = 0; i < Pillars; i++)
            {
                var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
                p.name = "Tide_" + i;
                var col = p.GetComponent<Collider>();
                if (col != null) Destroy(col);
                p.transform.SetParent(transform, false);
                var r = p.GetComponent<Renderer>();
                r.sharedMaterial = _mat;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _pillars[i] = p.transform;
            }
            _t = 0f;
            Pose(0f);
        }

        private void Update()
        {
            _t += Time.deltaTime;
            float k = Mathf.Clamp01(_t / _duration);
            Pose(k);

            // Spiral streaks — denser as the tide builds (departure) or right at the burst (arrival).
            _streakTimer -= Time.deltaTime;
            float streakRate = _departure ? Mathf.Lerp(0.14f, 0.035f, k) : 0.08f;
            if (_streakTimer <= 0f && k < 0.97f)
            {
                _streakTimer = streakRate;
                SpawnStreaks(k);
            }

            if (_t >= _duration + 0.1f)
            {
                Destroy(_mat);
                Destroy(gameObject);
            }
        }

        /// <summary>All the choreography, as one function of normalized time.</summary>
        private void Pose(float k)
        {
            // Departure: radius contracts 2.7→1.05 m, orbit accelerates 25→300 °/s (integrated
            // below as a quadratic phase), pillars rise 0→2.3 m with a staggered wave.
            // Arrival mirrors it: burst out from 1.05→3.2 m, decelerating, sinking.
            float radius, spin, rise, heat;
            if (_departure)
            {
                float e = k * k; // ease-in — the tide GATHERS
                radius = Mathf.Lerp(2.7f, 1.05f, e);
                spin = 25f * _t + 90f * _t * _t * 2f;       // integrated accelerating orbit
                rise = Mathf.Clamp01(k * 1.6f);
                heat = Mathf.SmoothStep(0f, 1f, (k - 0.75f) / 0.25f); // white-hot crest at the end
            }
            else
            {
                float e = 1f - (1f - k) * (1f - k); // ease-out — the tide RELEASES
                radius = Mathf.Lerp(1.05f, 3.2f, e);
                spin = 300f * _t - 110f * _t * _t;          // decelerating
                rise = 1f - Mathf.Clamp01((k - 0.35f) / 0.65f);
                heat = 1f - Mathf.SmoothStep(0f, 1f, k * 1.4f); // arrives hot, cools fast
            }

            _mat.SetColor("_BaseColor", Color.Lerp(Teal, Crest, heat));

            for (int i = 0; i < Pillars; i++)
            {
                float ang = (i / (float)Pillars) * 360f + spin;
                // Staggered wave: neighboring pillars lead/lag so the wall reads as WATER.
                float wave = 0.75f + 0.25f * Mathf.Sin(ang * Mathf.Deg2Rad * 3f + _t * 9f);
                float h = Mathf.Max(0.02f, 2.3f * rise * wave);
                Vector3 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), 0f, Mathf.Sin(ang * Mathf.Deg2Rad));
                _pillars[i].position = _center + dir * radius + Vector3.up * (h * 0.5f);
                _pillars[i].rotation = Quaternion.LookRotation(dir);
                // Thin at the base state, swelling toward the crest.
                float w = 0.05f + 0.10f * heat;
                _pillars[i].localScale = new Vector3(w * 3.2f, h, w);
            }
        }

        private void SpawnStreaks(float k)
        {
            // Tangential chords spiraling around the ring — quantized colors so TracerFx's
            // material cache stays tiny.
            float radius = _departure ? Mathf.Lerp(2.5f, 1.2f, k) : Mathf.Lerp(1.2f, 3f, k);
            int step = Mathf.RoundToInt(k * 4f);
            Color c = Color.Lerp(Teal, Crest, step / 4f);
            for (int i = 0; i < 3; i++)
            {
                float a = Random.Range(0f, Mathf.PI * 2f);
                float y = Random.Range(0.2f, 2.1f);
                Vector3 p0 = _center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius + Vector3.up * y;
                float a2 = a + (_departure ? 0.7f : -0.7f);
                Vector3 p1 = _center + new Vector3(Mathf.Cos(a2), 0f, Mathf.Sin(a2)) * (radius * 0.92f)
                    + Vector3.up * (y + Random.Range(-0.2f, 0.2f));
                TracerFx.Spawn(p0, p1, c, 0.02f, 0.16f);
            }
        }

        // ── Procedural audio (no clips exist in the project — synthesize the tide) ──────────

        private void PlayAudio()
        {
            // Explicit null checks — ??= would skip Unity's overloaded lifetime null.
            if (_departure && _riser == null) _riser = MakeRiser();
            if (!_departure && _boom == null) _boom = MakeBoom();
            var clip = _departure ? _riser : _boom;
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, _center + Vector3.up * 1.5f, 0.85f);
        }

        /// <summary>1.6 s riser: filtered noise swell + a sine sweep 70→750 Hz. Deterministic.</summary>
        private static AudioClip MakeRiser()
        {
            const int rate = 22050;
            int n = (int)(rate * 1.6f);
            var s = new float[n];
            var rng = new System.Random(777);
            float lp = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
                lp = Mathf.Lerp(lp, noise, 0.04f + 0.25f * t); // filter opens as it builds
                float freq = Mathf.Lerp(70f, 750f, t * t);
                float sweep = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                float env = t * t; // swells into the crest
                s[i] = (lp * 0.55f + sweep * 0.45f) * env * 0.9f;
            }
            var clip = AudioClip.Create("ZiptideRiser", n, 1, rate, false);
            clip.SetData(s, 0);
            return clip;
        }

        /// <summary>1.0 s arrival boom: the riser reversed in spirit — hot start, long soft tail.</summary>
        private static AudioClip MakeBoom()
        {
            const int rate = 22050;
            int n = (int)(rate * 1.0f);
            var s = new float[n];
            var rng = new System.Random(778);
            float lp = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
                lp = Mathf.Lerp(lp, noise, 0.3f - 0.26f * t); // filter closes as it fades
                float freq = Mathf.Lerp(500f, 60f, Mathf.Sqrt(t));
                float sweep = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                float env = (1f - t) * (1f - t);
                s[i] = (lp * 0.4f + sweep * 0.6f) * env * 0.9f;
            }
            var clip = AudioClip.Create("ZiptideBoom", n, 1, rate, false);
            clip.SetData(s, 0);
            return clip;
        }
    }
}
