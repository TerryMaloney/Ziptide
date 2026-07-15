using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.1b — the practical-light trick, parts ② and ③: a soft additive HALO billboard
    /// at the fixture head (the "air glow") and a LIGHT-POOL quad on the surface the fixture
    /// illuminates. Both use the shipped <c>Ziptide/AdditiveUnlit</c> shader; runtime code never
    /// attempts to manufacture a URP transparency variant that Android may strip.
    /// </summary>
    public class PracticalLight : MonoBehaviour
    {
        private const string AdditiveShaderName = "Ziptide/AdditiveUnlit";

        [Tooltip("Glow color — PracticalAuthor passes the fixture's warm lamp color.")]
        public Color glowColor = new Color(1f, 0.76f, 0.44f);
        [Tooltip("Halo diameter in meters (≈2.5× the fixture head).")]
        public float haloSize = 0.5f;

        [Header("Authored placement data (PracticalAuthor bakes DATA; the quads + their runtime\n"
            + "materials are built here in Awake — generated materials must never serialize into scenes)")]
        [Tooltip("Fixture-head position (local) for the halo.")]
        public Vector3 headLocal;
        [Tooltip("When true, a light-pool quad is laid at poolPoint/poolNormal on Awake.")]
        public bool hasPool;
        public Vector3 poolPoint;
        public Vector3 poolNormal = Vector3.up;
        public float poolSize = 1.4f;

        private Renderer _halo;
        private Renderer _pool;
        private bool _lit = true;

        private static Texture2D _radial;
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            InitHalo(headLocal);
            if (hasPool) InitPool(poolPoint, poolNormal, poolSize);
        }

        /// <summary>Build the halo at <paramref name="headLocal"/> (fixture-local).</summary>
        public void InitHalo(Vector3 headLocal)
        {
            if (_halo != null) return;
            _halo = MakeQuad("PracticalHalo", transform, headLocal, haloSize, glowColor, 0.55f);
        }

        /// <summary>Lay the light pool on a surface (world-space point + normal, authored at build time).</summary>
        public void InitPool(Vector3 worldPoint, Vector3 worldNormal, float size)
        {
            if (_pool != null) return;
            _pool = MakeQuad("PracticalPool", transform, Vector3.zero, size, glowColor, 0.22f);
            if (_pool == null) return;
            _pool.transform.position = worldPoint + worldNormal * 0.01f;
            _pool.transform.rotation = Quaternion.LookRotation(-worldNormal);
        }

        /// <summary>ONE switch for halo, pool, and the fixture's emissive. Idempotent.</summary>
        public void SetLit(bool lit)
        {
            _lit = lit;
            if (_halo != null) _halo.enabled = lit;
            if (_pool != null) _pool.enabled = lit;

            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == _halo || renderer == _pool) continue;
                if (lit)
                {
                    renderer.SetPropertyBlock(null);
                }
                else
                {
                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block);
                    block.SetColor(EmissionColorId, Color.black);
                    renderer.SetPropertyBlock(block);
                }
            }
        }

        public bool IsLit => _lit;

        private void LateUpdate()
        {
            if (_halo == null || !_lit) return;
            var cam = Camera.main;
            if (cam != null)
                _halo.transform.rotation = Quaternion.LookRotation(
                    _halo.transform.position - cam.transform.position);
        }

        private static Renderer MakeQuad(
            string name,
            Transform parent,
            Vector3 localPosition,
            float size,
            Color color,
            float alpha)
        {
            Material material = AdditiveMat(color, alpha);
            if (material == null) return null;

            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                if (Application.isPlaying) Destroy(collider);
                else DestroyImmediate(collider);
            }

            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = Vector3.one * size;

            var renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            return renderer;
        }

        /// <summary>
        /// Soft radial falloff on the fixed additive shader. No runtime blend-state or shader-keyword
        /// mutation is involved, so Quest receives the exact same shader behavior as the editor.
        /// </summary>
        private static Material AdditiveMat(Color color, float alpha)
        {
            Shader shader = Shader.Find(AdditiveShaderName);
            if (shader == null)
            {
                Debug.LogError("ZIPTIDE: SHADER_MISSING name=" + AdditiveShaderName +
                               " effect=PracticalLight");
                return null;
            }

            if (_radial == null)
            {
                const int size = 64;
                _radial = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    name = "PracticalLightRadial",
                    wrapMode = TextureWrapMode.Clamp
                };
                var pixels = new Color32[size * size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = (x + 0.5f) / size - 0.5f;
                        float dy = (y + 0.5f) / size - 0.5f;
                        float t = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy) * 2f);
                        byte a = (byte)Mathf.RoundToInt(255f * t * t);
                        pixels[y * size + x] = new Color32(255, 255, 255, a);
                    }
                }
                _radial.SetPixels32(pixels);
                _radial.Apply(false, false);
            }

            var material = new Material(shader)
            {
                name = "PracticalLight_Additive",
                hideFlags = HideFlags.HideAndDontSave
            };
            material.SetTexture(BaseMapId, _radial);
            material.SetColor(BaseColorId, new Color(color.r, color.g, color.b, alpha));
            return material;
        }

        private void OnDestroy()
        {
            DestroyRuntimeMaterial(_halo);
            DestroyRuntimeMaterial(_pool);
        }

        private static void DestroyRuntimeMaterial(Renderer renderer)
        {
            if (renderer == null || renderer.sharedMaterial == null) return;
            Material material = renderer.sharedMaterial;
            renderer.sharedMaterial = null;
            if (Application.isPlaying) Destroy(material);
            else DestroyImmediate(material);
        }
    }
}
