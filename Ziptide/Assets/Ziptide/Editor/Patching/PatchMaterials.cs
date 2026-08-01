#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// ONE MATERIAL PER LOOK, FOR THE WHOLE BAKE.
    ///
    /// ToxicCity audits at 333 unique materials against a hard cap of 60, and the cause is not content.
    /// Every patcher that paints a cube used to own a private cache — or no cache at all — so the same
    /// worn grey authored in four files was four materials, and two authors created a fresh one for
    /// every single object they painted (nineteen lanterns × three parts = 57 materials for two
    /// colours; five berth pads = another thirty-odd for four).
    ///
    /// This is the shared cache. It is keyed on what actually makes two materials different at draw
    /// time — the colour and the emission — so anything that looks the same IS the same instance, and
    /// the renderer count can then collapse into static batches instead of standing as separate draws.
    ///
    /// <b>Nothing is deleted and nothing changes colour.</b> The scene renders identically; it just
    /// stops describing the same grey over and over.
    ///
    /// Call <see cref="Reset"/> at the top of a bake. Materials created here are scene-embedded (no
    /// AssetDatabase entry), so an instance must never be carried from one scene into the next.
    /// </summary>
    public static class PatchMaterials
    {
        private readonly struct Key : System.IEquatable<Key>
        {
            private readonly Color _color;
            private readonly float _emission;

            public Key(Color color, float emission) { _color = color; _emission = emission; }

            public bool Equals(Key other) => _color == other._color && _emission == other._emission;
            public override bool Equals(object obj) => obj is Key k && Equals(k);
            public override int GetHashCode() => _color.GetHashCode() ^ _emission.GetHashCode();
        }

        private static readonly Dictionary<Key, Material> _cache = new Dictionary<Key, Material>();

        /// <summary>How many distinct looks this bake has asked for. The number the budget cares about.</summary>
        public static int Count => _cache.Count;

        /// <summary>Drop everything. MUST run per scene — these materials live inside the scene file.</summary>
        public static void Reset() => _cache.Clear();

        /// <summary>An opaque unlit-shaded colour.</summary>
        public static Material Get(Color color) => Get(color, 0f);

        /// <summary>
        /// A colour, optionally emissive. <paramref name="emission"/> is the multiplier applied to the
        /// colour for `_EmissionColor`; 0 means no emission. It is part of the key because a glowing
        /// amber globe and a painted amber post are genuinely two different draws.
        /// </summary>
        public static Material Get(Color color, float emission)
        {
            var key = new Key(color, emission);
            if (_cache.TryGetValue(key, out var cached) && cached != null) return cached;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return null;

            var m = new Material(shader)
            {
                name = "City_" + ColorUtility.ToHtmlStringRGB(color) + (emission > 0f ? "_E" : ""),
            };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", color);

            if (emission > 0f && m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", color * emission);
            }

            _cache[key] = m;
            return m;
        }

        /// <summary>
        /// Paint a renderer and turn its shadow off. Shadow casting is off across the city on purpose:
        /// a stand-in cube that casts is a cube that costs, and the lighting is doing none of the work.
        /// </summary>
        public static void Paint(GameObject go, Color color, float emission = 0f)
        {
            if (go == null) return;
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var m = Get(color, emission);
            if (m != null) r.sharedMaterial = m;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
