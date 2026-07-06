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

        // Destination tint (v4): the tide is colored by WHERE YOU'RE GOING — wall from the
        // destination sky's horizon, streaks from its zenith. Teal when no vista is authored.
        private Color _tintWall = Teal;
        private Color _tintStreak = Teal;

        // Door anchor (v6): when a physical door/gate started the travel, the tide visibly
        // pours out of that doorway toward the ring during the dial-in. Null = ring only.
        private Vector3? _doorPos;

        // The crest flash shell (v7): an OPAQUE white sphere around the camera with culling
        // off (we're inside it), parented to the camera so it rides the persistent rig ACROSS
        // the scene load — the synchronous-load freeze happens on white, not on a frozen world
        // view. Opaque + cull-off is pure render state, so it can't be lost to URP shader
        // variant stripping the way a runtime transparent fade could.
        private static GameObject _flash;
        private bool _flashDone;

        private Transform[] _pillars;
        private Transform _pool; // the glowing tide pool underfoot
        private TextMesh _label; // destination name riding the tide (departure only)
        private Material _mat;
        private Vector3 _center;
        private float _t, _duration;
        private bool _departure;
        private float _streakTimer;
        private float _hapticTimer;

        /// <summary>Start the departure tide around <paramref name="center"/>. Returns the lead
        /// time until the crest — the caller cuts the scene exactly then. Pass the destination
        /// sky colors (from the DevWorldManifest entry) to tint the tide toward where you're
        /// going; alpha-0 colors fall back to Ziptide teal.</summary>
        public static float PlayDeparture(Vector3 center, string destinationName = null,
            Color destSkyHorizon = default, Color destSkyZenith = default, Vector3? gatePos = null)
        {
            var fx = Spawn(center, departure: true, duration: 1.6f);
            fx.SetTint(destSkyHorizon, destSkyZenith);
            fx._doorPos = gatePos; // v6: the tide pours OUT OF THE DOORWAY toward the ring
            if (!string.IsNullOrEmpty(destinationName)) fx.BuildDestinationLabel(destinationName);
            Debug.Log("ZIPTIDE: ZIPTIDE_GATE depart dest=" + (destinationName ?? "?"));
            return 1.45f; // cut just inside the crest flash
        }

        /// <summary>The receding tide in the new world, around the arrival point. Tinted with
        /// THIS world's sky — the tide relaxes into the sky it brought you to.</summary>
        public static void PlayArrival(Vector3 center,
            Color skyHorizon = default, Color skyZenith = default)
        {
            var fx = Spawn(center, departure: false, duration: 1.0f);
            fx.SetTint(skyHorizon, skyZenith);
            Debug.Log("ZIPTIDE: ZIPTIDE_GATE arrive");
        }

        /// <summary>Blend the tide toward a destination sky, kept luminous (a cave world's
        /// near-black horizon still has to read as energy, not shadow).</summary>
        private void SetTint(Color skyHorizon, Color skyZenith)
        {
            _tintWall = TideTint(skyHorizon);
            _tintStreak = TideTint(skyZenith);
        }

        private static Color TideTint(Color sky)
        {
            if (sky.a <= 0f) return Teal; // no vista authored — the tide stays Ziptide teal
            float max = Mathf.Max(sky.r, Mathf.Max(sky.g, sky.b));
            Color bright = max < 0.55f && max > 0.001f ? sky * (0.55f / max) : sky;
            bright.a = 1f;
            return Color.Lerp(Teal, bright, 0.75f);
        }

        private static ZiptideGateEffect Spawn(Vector3 center, bool departure, float duration)
        {
            var go = new GameObject(departure ? "__ZiptideDepart" : "__ZiptideArrive");
            var fx = go.AddComponent<ZiptideGateEffect>();
            fx._center = center;
            fx._departure = departure;
            fx._duration = duration;
            fx.Build();
            fx.PlayAudio();
            return fx;
        }

        /// <summary>The destination's name rides the tide — fades in above the ring, billboarded.</summary>
        private void BuildDestinationLabel(string destinationName)
        {
            var go = new GameObject("__Destination");
            go.transform.SetParent(transform, false);
            _label = go.AddComponent<TextMesh>();
            _label.text = destinationName.ToUpperInvariant();
            _label.characterSize = 0.045f; // readable at ~2.4 m (characterSize x fontSize law)
            _label.fontSize = 64;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(Crest.r, Crest.g, Crest.b, 0f);
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

            // The tide pool: a thin glowing disk underfoot that swells with the ring.
            var pool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pool.name = "TidePool";
            var pc = pool.GetComponent<Collider>();
            if (pc != null) Destroy(pc);
            pool.transform.SetParent(transform, false);
            var pr = pool.GetComponent<Renderer>();
            pr.sharedMaterial = _mat;
            pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _pool = pool.transform;

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

            // Destination name: fades in over the dial-in, hovers above the ring, faces the player.
            if (_label != null)
            {
                var cam = Camera.main;
                _label.transform.position = _center + Vector3.up * 2.55f;
                if (cam != null)
                    _label.transform.rotation = Quaternion.LookRotation(_label.transform.position - cam.transform.position);
                var lc = _label.color;
                lc.a = Mathf.Clamp01(k * 3f) * (1f - Mathf.SmoothStep(0f, 1f, (k - 0.85f) / 0.15f));
                _label.color = lc;
            }

            // Haptics build with the tide — you FEEL the gate before you see the crest.
            _hapticTimer -= Time.deltaTime;
            if (_hapticTimer <= 0f)
            {
                _hapticTimer = 0.09f;
                float amp = _departure ? 0.08f + 0.55f * k * k : 0.5f * (1f - k) * (1f - k);
                foreach (var c in Object.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>())
                    if (c != null) c.SendHapticImpulse(Mathf.Clamp01(amp), 0.1f);
            }

            // v7 — THE FLASH: just before the cut, the crest whites out the whole view. The
            // shell rides the camera across the load; the arrival tide lifts it 0.12 s in,
            // resolving white → white-hot pillars → the new world.
            if (_departure && !_flashDone && k >= 0.88f)
            {
                _flashDone = true;
                SpawnFlash();
            }
            if (!_departure && !_flashDone && _t >= 0.12f)
            {
                _flashDone = true;
                ClearFlash();
            }

            if (_t >= _duration + 0.1f)
            {
                Destroy(_mat);
                Destroy(gameObject);
            }
        }

        private static void SpawnFlash()
        {
            if (_flash != null) return;
            var cam = Camera.main;
            if (cam == null) return;
            _flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _flash.name = "__ZiptideFlash";
            var col = _flash.GetComponent<Collider>();
            if (col != null) Destroy(col);
            _flash.transform.SetParent(cam.transform, false);
            _flash.transform.localPosition = Vector3.zero;
            _flash.transform.localScale = Vector3.one * 1.2f; // radius 0.6 m — past the near plane, around the head
            var r = _flash.GetComponent<Renderer>();
            var m = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            m.SetColor("_BaseColor", Crest);
            m.SetFloat("_Cull", (float)UnityEngine.Rendering.CullMode.Off); // we're INSIDE the sphere
            r.sharedMaterial = m;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _flash.AddComponent<FlashTimeout>(); // safety net: never strand a white screen
        }

        private static void ClearFlash()
        {
            if (_flash != null)
            {
                Destroy(_flash);
                _flash = null;
            }
        }

        /// <summary>If arrival never plays (failed travel, missing rig), the flash must still
        /// lift — a stranded white screen would be worse than any missing polish.</summary>
        private sealed class FlashTimeout : MonoBehaviour
        {
            private float _life = 3f;

            private void Update()
            {
                _life -= Time.deltaTime;
                if (_life <= 0f) Destroy(gameObject);
            }

            private void OnDestroy()
            {
                var r = GetComponent<Renderer>();
                if (r != null && r.sharedMaterial != null) Destroy(r.sharedMaterial);
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
                // Dial-in beat: the first ~15% is streaks + the pool only (the chevrons locking),
                // THEN the wall rises — anticipation before the event.
                rise = Mathf.Clamp01((k - 0.15f) * 1.9f);
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

            // Departure: the tide starts Ziptide teal and BECOMES the destination sky as it
            // gathers — the gate is colored by where you're going. Arrival: the tide is already
            // that sky, cooling from the crest into it.
            Color baseCol = _departure ? Color.Lerp(Teal, _tintWall, k) : _tintWall;
            _mat.SetColor("_BaseColor", Color.Lerp(baseCol, Crest, heat));

            // The tide pool underfoot swells with the ring (thin — it's a sheen, not a wall).
            if (_pool != null)
            {
                _pool.position = _center + Vector3.up * 0.012f;
                float poolR = _departure ? Mathf.Lerp(0.4f, radius, Mathf.Clamp01(k * 3f)) : radius;
                _pool.localScale = new Vector3(poolR * 2f, 0.008f, poolR * 2f);
            }

            for (int i = 0; i < Pillars; i++)
            {
                float ang = (i / (float)Pillars) * 360f + spin;
                // Staggered wave: neighboring pillars lead/lag so the wall reads as WATER.
                float wave = 0.75f + 0.25f * Mathf.Sin(ang * Mathf.Deg2Rad * 3f + _t * 9f);
                float h = Mathf.Max(0.02f, 2.3f * rise * wave);
                // Crest eruption: at the flash, every third pillar JETS skyward — the wave breaks.
                if (i % 3 == 0) h *= 1f + 1.7f * heat;
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
            // material cache stays tiny (the tint is quantized to 0.2 steps so different
            // destinations share cache entries instead of each minting ~5 new materials).
            float radius = _departure ? Mathf.Lerp(2.5f, 1.2f, k) : Mathf.Lerp(1.2f, 3f, k);
            int step = Mathf.RoundToInt(k * 4f);
            Color tint = new Color(
                Mathf.Round(_tintStreak.r * 5f) / 5f,
                Mathf.Round(_tintStreak.g * 5f) / 5f,
                Mathf.Round(_tintStreak.b * 5f) / 5f);
            Color c = Color.Lerp(tint, Crest, step / 4f);
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

            // v6 — the doorway torrent: when a physical door started this travel, the tide
            // visibly pours OUT of it toward the ring. Strongest during the dial-in, handing
            // over to the ring as the crest takes charge.
            if (_departure && _doorPos.HasValue && k < 0.8f)
            {
                Vector3 door = _doorPos.Value;
                int jets = k < 0.5f ? 2 : 1;
                for (int i = 0; i < jets; i++)
                {
                    float y = Random.Range(0.25f, 2.0f);
                    Vector3 p0 = door + Vector3.up * y + Random.insideUnitSphere * 0.25f;
                    float a = Random.Range(0f, Mathf.PI * 2f);
                    Vector3 p1 = _center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius
                        + Vector3.up * (y * 0.8f);
                    TracerFx.Spawn(p0, p1, c, 0.025f, 0.22f);
                }
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
