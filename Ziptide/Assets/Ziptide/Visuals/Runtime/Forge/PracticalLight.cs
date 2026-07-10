using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.1b — the practical-light trick, parts ② and ③: a soft additive HALO billboard
    /// at the fixture head (the "air glow") and a LIGHT-POOL quad on the surface the fixture
    /// illuminates (ground pool under lanterns/poles, wall wash above sconces) — how a light
    /// affects its environment on Quest without a real Light. One shared lit/dead state:
    /// <see cref="SetLit"/> kills or revives halo + pool + the fixture's emissive together (the
    /// unified death is what sells a shot-out lamp; F3.6's ReactiveProp calls it).
    /// A LOOK, never a stat: no colliders, no Light component — the F3.1b hero budget (≤2 real
    /// points per world) is assigned by PracticalAuthor, not here.
    /// </summary>
    public class PracticalLight : MonoBehaviour
    {
        [Tooltip("Glow color — PracticalAuthor passes the fixture's warm lamp color.")]
        public Color glowColor = new Color(1f, 0.76f, 0.44f);
        [Tooltip("Halo diameter in meters (≈2.5× the fixture head).")]
        public float haloSize = 0.5f;

        private Renderer _halo;
        private Renderer _pool;
        private bool _lit = true;

        private static Texture2D _radial;
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        /// <summary>Build the halo at <paramref name="headLocal"/> (fixture-local). Call once from
        /// the author/factory after the fixture look is applied.</summary>
        public void InitHalo(Vector3 headLocal)
        {
            if (_halo != null) return;
            _halo = MakeQuad("PracticalHalo", transform, headLocal, haloSize, glowColor, 0.55f);
        }

        /// <summary>Lay the light pool on a surface (world-space point + normal, from the author's
        /// build-time raycast — never a runtime cast).</summary>
        public void InitPool(Vector3 worldPoint, Vector3 worldNormal, float size)
        {
            if (_pool != null) return;
            _pool = MakeQuad("PracticalPool", transform, Vector3.zero, size, glowColor, 0.22f);
            _pool.transform.position = worldPoint + worldNormal * 0.01f;
            _pool.transform.rotation = Quaternion.LookRotation(-worldNormal);
        }

        /// <summary>ONE switch for the whole practical: halo, pool, and the fixture's emissive
        /// live and die together. Idempotent.</summary>
        public void SetLit(bool lit)
        {
            _lit = lit;
            if (_halo != null) _halo.enabled = lit;
            if (_pool != null) _pool.enabled = lit;
            // Per-instance emissive kill via property block — the baked material is SHARED.
            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                if (r == _halo || r == _pool) continue;
                if (lit)
                {
                    r.SetPropertyBlock(null); // restore the material's own emission
                }
                else
                {
                    var mpb = new MaterialPropertyBlock();
                    r.GetPropertyBlock(mpb);
                    mpb.SetColor(EmissionColorId, Color.black);
                    r.SetPropertyBlock(mpb);
                }
            }
        }

        public bool IsLit => _lit;

        private void LateUpdate()
        {
            // Halo billboards the camera; the pool stays glued to its surface.
            if (_halo == null || !_lit) return;
            var cam = Camera.main;
            if (cam != null) _halo.transform.rotation =
                Quaternion.LookRotation(_halo.transform.position - cam.transform.position);
        }

        // ── quad + additive material plumbing ─────────────────────────────────

        private static Renderer MakeQuad(string name, Transform parent, Vector3 localPos,
            float size, Color color, float alpha)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            var col = go.GetComponent<Collider>();
            if (col != null) // edit-mode-safe (headless tests build practicals too)
            {
                if (Application.isPlaying) Destroy(col);
                else DestroyImmediate(col);
            }
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = Vector3.one * size;
            var r = go.GetComponent<Renderer>();
            r.sharedMaterial = AdditiveMat(color, alpha);
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return r;
        }

        /// <summary>Soft radial falloff sprite (64px, generated once) on an additive URP/Unlit —
        /// additive quads never write depth, so halos can't face-fight geometry.</summary>
        private static Material AdditiveMat(Color c, float alpha)
        {
            if (_radial == null)
            {
                const int n = 64;
                _radial = new Texture2D(n, n, TextureFormat.RGBA32, false);
                var px = new Color32[n * n];
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float dx = (x + 0.5f) / n - 0.5f, dy = (y + 0.5f) / n - 0.5f;
                        float t = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy) * 2f);
                        byte a = (byte)Mathf.RoundToInt(255f * t * t); // quadratic soft edge
                        px[y * n + x] = new Color32(255, 255, 255, a);
                    }
                _radial.SetPixels32(px);
                _radial.Apply(false, false);
                _radial.wrapMode = TextureWrapMode.Clamp;
            }

            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetTexture(BaseMapId, _radial);
            mat.SetColor(BaseColorId, new Color(c.r, c.g, c.b, alpha));
            // URP/Unlit runtime additive: transparent surface, One/One blend, no depth write.
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 2f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            return mat;
        }
    }
}
