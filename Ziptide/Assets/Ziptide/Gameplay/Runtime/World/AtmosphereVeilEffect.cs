using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE ATMOSPHERE VEIL — reentry fire and the ascent burn, built the way the Ziptide gate is
    /// built: primitives, unlit, procedural audio, zero assets, and the camera NEVER moves.
    ///
    /// The plasma is a CORONA — sixteen opaque glowing blades ringing the view at arm's length,
    /// parented to the camera so they ride the persistent rig across the scene load. They reach
    /// inward as the veil builds (never closing the center on the reentry leg — you always keep a
    /// window to fly through), lick with a per-blade flicker, and stream tracer sparks outward
    /// past the canopy. Opaque + cull-off is the same render-state choice the gate's crest flash
    /// made: a runtime transparent fade is the thing URP shader stripping eats on device.
    ///
    /// Fail-safe by construction (<see cref="AtmosphereVeilCore"/>): every instance self-destructs
    /// at the hard cap whatever else happens, so a veil can never strand travel or leave the view
    /// burning. Logs ZIPTIDE: VEIL leg=… phase=… .
    /// </summary>
    public class AtmosphereVeilEffect : MonoBehaviour
    {
        private const int Blades = 16;
        private static readonly Color Ember = new Color(1f, 0.42f, 0.10f);
        private static readonly Color WhiteHot = new Color(1f, 0.94f, 0.80f);

        /// <summary>Ring radius in head space — outside the near plane, inside comfortable focus.</summary>
        private const float RingRadius = 0.85f;

        private static AudioClip _roar;

        private VeilLeg _leg;
        private float _t;
        private Transform[] _blades;
        private Material _mat;
        private Transform _cam;
        private VeilPhase _phase = VeilPhase.Build;
        private float _sparkTimer;

        /// <summary>
        /// Play the veil for one leg. Returns the seconds until the caller should cut the scene
        /// (ascent) or simply the length (reentry). Safe to call with no camera — it no-ops.
        /// </summary>
        public static float Play(VeilLeg leg)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                Debug.Log("ZIPTIDE: VEIL leg=" + leg + " phase=skipped reason=no_camera");
                return leg == VeilLeg.Ascent ? AtmosphereVeilCore.CutLeadSeconds : 0f;
            }

            var go = new GameObject("__AtmosphereVeil_" + leg);
            var fx = go.AddComponent<AtmosphereVeilEffect>();
            fx._leg = leg;
            fx._cam = cam.transform;
            fx.Build();
            fx.PlayAudio();
            Debug.Log("ZIPTIDE: VEIL leg=" + leg + " phase=start seconds="
                + AtmosphereVeilCore.TotalSeconds(leg).ToString("F2"));
            return leg == VeilLeg.Ascent
                ? AtmosphereVeilCore.CutLeadSeconds
                : AtmosphereVeilCore.TotalSeconds(leg);
        }

        private static Material UnlitMaterial()
        {
            // Same hard fallback the gate uses: a stripped shader would make new Material(null)
            // throw, and this runs on the travel path.
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            return new Material(shader);
        }

        private void Build()
        {
            _mat = UnlitMaterial();
            _mat.SetColor("_BaseColor", Ember);

            _blades = new Transform[Blades];
            for (int i = 0; i < Blades; i++)
            {
                var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                b.name = "Blade_" + i;
                var col = b.GetComponent<Collider>();
                if (col != null) Destroy(col);
                b.transform.SetParent(_cam, false); // rides the head across the scene load
                var r = b.GetComponent<Renderer>();
                r.sharedMaterial = _mat;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _blades[i] = b.transform;
            }
            Pose(0f);
        }

        private void Update()
        {
            _t += Time.deltaTime;

            // The one safety rule, enforced first: nothing outlives the cap, whatever the state.
            if (_t >= AtmosphereVeilCore.HardCapSeconds
                || AtmosphereVeilCore.PhaseAt(_leg, _t) == VeilPhase.Done)
            {
                Debug.Log("ZIPTIDE: VEIL leg=" + _leg + " phase=done");
                Cleanup();
                return;
            }

            var phase = AtmosphereVeilCore.PhaseAt(_leg, _t);
            if (phase != _phase)
            {
                _phase = phase;
                Debug.Log("ZIPTIDE: VEIL leg=" + _leg + " phase=" + phase);
            }

            float intensity = AtmosphereVeilCore.Intensity(_leg, _t);
            Pose(intensity);
            SpawnSparks(intensity);
        }

        /// <summary>Blade reach + heat as one function of intensity. No camera motion anywhere —
        /// the burn happens around a head that never gets pushed.</summary>
        private void Pose(float intensity)
        {
            if (_blades == null) return;

            _mat.SetColor("_BaseColor", Color.Lerp(Ember, WhiteHot, intensity * intensity));

            for (int i = 0; i < Blades; i++)
            {
                var blade = _blades[i];
                if (blade == null) continue;

                float ang = (i / (float)Blades) * Mathf.PI * 2f;
                // Per-blade flicker so the corona licks instead of pulsing as one solid iris.
                float lick = 0.78f + 0.22f * Mathf.Sin(_t * 11f + i * 1.7f);
                float reach = intensity * lick;

                // Blades hang at the ring and grow INWARD; the center window closes only as the
                // ascent peaks (where the travel cut lands anyway), never on reentry.
                float inward = _leg == VeilLeg.Ascent ? reach * 0.95f : reach * 0.62f;
                float r = RingRadius * (1f - inward * 0.55f);

                blade.localPosition = new Vector3(Mathf.Cos(ang) * r, Mathf.Sin(ang) * r, 1.15f);
                blade.localRotation = Quaternion.Euler(0f, 0f, ang * Mathf.Rad2Deg);
                blade.localScale = new Vector3(
                    0.06f + 0.30f * reach,          // thickness along the ring
                    0.10f + 0.62f * reach,          // reach inward
                    0.02f);
            }
        }

        private void SpawnSparks(float intensity)
        {
            if (intensity <= 0.05f || _cam == null) return;
            _sparkTimer -= Time.deltaTime;
            if (_sparkTimer > 0f) return;
            _sparkTimer = Mathf.Lerp(0.13f, 0.035f, intensity);

            // Embers streaming BACK past the canopy: the ship's speed read, borrowed from the
            // cast-off streaks so ascent and reentry share one visual language.
            Color c = Color.Lerp(Ember, WhiteHot, intensity);
            for (int i = 0; i < 3; i++)
            {
                float a = Random.Range(0f, Mathf.PI * 2f);
                float r = RingRadius * Random.Range(0.5f, 1.6f);
                Vector3 side = _cam.right * Mathf.Cos(a) + _cam.up * Mathf.Sin(a);
                Vector3 from = _cam.position + _cam.forward * Random.Range(3f, 9f) + side * (r * 4f);
                float len = 1.5f + 5f * intensity;
                TracerFx.Spawn(from, from - _cam.forward * len, c, 0.02f, 0.14f);
            }
        }

        private void Cleanup()
        {
            if (_blades != null)
                foreach (var b in _blades)
                    if (b != null) Destroy(b.gameObject);
            if (_mat != null) Destroy(_mat);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            // Belt and braces: blades are parented to the CAMERA, not to us, so if this object is
            // destroyed by anything other than Cleanup they would otherwise survive on the rig.
            if (_blades == null) return;
            foreach (var b in _blades)
                if (b != null) Destroy(b.gameObject);
        }

        // ── Procedural audio (the project owns no clips — synthesize the burn) ──────────────

        private void PlayAudio()
        {
            if (_roar == null) _roar = MakeRoar();
            if (_roar == null || _cam == null) return;
            AudioSource.PlayClipAtPoint(_roar, _cam.position, 0.7f);
        }

        /// <summary>2.4 s atmospheric roar: broadband buffet through a filter that opens with the
        /// heat, a low body that keeps it in the chest, and a swell-then-settle envelope. One clip
        /// serves both legs — reentry simply plays it against a veil that is already clearing.</summary>
        private static AudioClip MakeRoar()
        {
            const int rate = 22050;
            int n = (int)(rate * 2.4f);
            var s = new float[n];
            var rng = new System.Random(4711);
            float lp = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                float sec = i / (float)rate;
                float noise = (float)(rng.NextDouble() * 2.0 - 1.0);

                // The buffet: filter opens toward the middle of the burn, then closes again.
                float open = 0.06f + 0.30f * Mathf.Sin(t * Mathf.PI);
                lp = Mathf.Lerp(lp, noise, open);

                float body = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(48f, 33f, t) * sec) * 0.5f;
                float env = Mathf.Sin(t * Mathf.PI);           // swell in, settle out
                float mix = (lp * 0.85f + body * 0.45f) * env;
                s[i] = (float)System.Math.Tanh(mix * 1.5f) * 0.8f;
            }
            var clip = AudioClip.Create("AtmosphereRoar", n, 1, rate, false);
            clip.SetData(s, 0);
            return clip;
        }
    }
}
