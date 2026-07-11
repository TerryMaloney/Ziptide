using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 commit 3 — the runtime water body. Assembles the flat ripple plane
    /// (<see cref="ZiptideWaterMesh"/>), a URP/Lit water material (deep tint + high smoothness +
    /// the tileable ripple normal — baked <c>ForgeBaked/water_normal</c> preferred, runtime
    /// fallback), a scrolling normal offset (<see cref="WaterMotion"/>), a gentle low-res vertex
    /// bob, and a flat foam band (<see cref="WaterFoamMesh"/>). Data in, look out — WaterAuthor adds
    /// this and sets the size; everything is built in Awake so no generated mesh/material serializes
    /// into a scene. A LOOK, never a stat: no collider (swimming/water gameplay is out of scope).
    /// </summary>
    public class ZiptideWater : MonoBehaviour
    {
        [Header("Extent (meters) — WaterAuthor sets these from the layout's water rect")]
        public float sizeX = 6f;
        public float sizeZ = 6f;
        [Tooltip("Grid cells per side. Low — the fine ripple is the scrolling normal; cells only feed the bob.")]
        public int cells = 16;

        [Header("Look")]
        public Color deepColor = new Color(0.06f, 0.16f, 0.18f, 1f);
        public float smoothness = 0.85f;
        [Tooltip("Normal-map flow direction (deg) and speed (tiles/sec).")]
        public float scrollDir = 20f;
        public float scrollSpeed = 0.02f;

        [Header("Swell")]
        public float bobAmp = 0.03f;
        public float bobFreq = 0.6f;

        [Header("Foam")]
        public bool foam = true;
        public float foamBand = 0.5f;
        public Color foamColor = Color.white;

        private Mesh _mesh;
        private Vector3[] _baseVerts, _workVerts;
        private Material _waterMat;
        private Material _foamMat;
        private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");

        private void Awake() => Assemble();

        /// <summary>Build the mesh + materials + foam child from the serialized fields. Awake calls
        /// this; it is public so an EditMode test can assemble the body without play mode.</summary>
        public void Assemble()
        {
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            if (lit == null) return; // no URP → leave the GameObject inert rather than error

            _mesh = ZiptideWaterMesh.Build(sizeX, sizeZ, cells);
            _baseVerts = _mesh.vertices;
            _workVerts = (Vector3[])_baseVerts.Clone();

            var mf = GetComponent<MeshFilter>();
            if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = _mesh;

            var mr = GetComponent<MeshRenderer>();
            if (mr == null) mr = gameObject.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            _waterMat = new Material(lit) { name = "ZiptideWater" };
            _waterMat.SetColor("_BaseColor", deepColor);
            _waterMat.SetFloat("_Smoothness", smoothness);
            _waterMat.SetFloat("_Metallic", 0f);
            _waterMat.SetTexture("_BumpMap", NormalTex());
            _waterMat.EnableKeyword("_NORMALMAP");
            mr.sharedMaterial = _waterMat;

            if (foam) BuildFoam(lit);
        }

        private void LateUpdate()
        {
            if (_waterMat == null) return;
            _waterMat.SetTextureOffset(BumpMapId, WaterMotion.ScrollOffset(scrollDir, scrollSpeed, Time.time));

            if (_baseVerts != null && bobAmp > 0f)
            {
                float t = Time.time;
                for (int i = 0; i < _baseVerts.Length; i++)
                {
                    var b = _baseVerts[i];
                    _workVerts[i] = new Vector3(b.x, WaterMotion.BobHeight(b.x, b.z, t, bobAmp, bobFreq), b.z);
                }
                _mesh.SetVertices(_workVerts);
            }
        }

        private void BuildFoam(Shader lit)
        {
            var go = new GameObject("Foam");
            go.transform.SetParent(transform, false);
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = WaterFoamMesh.BuildBand(sizeX, sizeZ, foamBand, 3f, 0.02f);
            var mr = go.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            const int fs = 128;
            var px = new Color32[fs * fs];
            for (int y = 0; y < fs; y++)
                for (int x = 0; x < fs; x++)
                {
                    float a = WaterFoamMesh.FoamAlpha((x + 0.5f) / fs, (y + 0.5f) / fs);
                    px[y * fs + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            var tex = new Texture2D(fs, fs, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Repeat };
            tex.SetPixels32(px); tex.Apply(false, false);

            _foamMat = new Material(lit) { name = "ZiptideWaterFoam" };
            _foamMat.SetColor("_BaseColor", foamColor);
            _foamMat.SetTexture("_BaseMap", tex);
            _foamMat.SetFloat("_AlphaClip", 1f);
            _foamMat.SetFloat("_Cutoff", 0.5f);
            _foamMat.EnableKeyword("_ALPHATEST_ON");
            _foamMat.SetFloat("_Smoothness", 0f);
            _foamMat.SetFloat("_Metallic", 0f);
            mr.sharedMaterial = _foamMat;
        }

        /// <summary>Prefer the build-baked normal (imported NormalMap → platform-correct on Quest);
        /// fall back to a runtime bake (desktop DXT5nm swizzle) when the bake is absent (dev/editor).</summary>
        private Texture2D NormalTex()
        {
            var baked = Resources.Load<Texture2D>("ForgeBaked/water_normal");
            if (baked != null) return baked;

            const int n = 256;
            var px = WaterSurface.BakeNormalMap(n, 1.4f);
            // Editor/desktop URP/Lit unpacks _BumpMap DXT5nm-style (x in A, y in G).
            for (int i = 0; i < px.Length; i++) px[i] = new Color32(255, px[i].g, 255, px[i].r);
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false, true) { wrapMode = TextureWrapMode.Repeat };
            tex.SetPixels32(px); tex.Apply(false, false);
            return tex;
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return; // edit-mode test cleanup is DestroyImmediate(go); Destroy is illegal here
            if (_waterMat != null) Destroy(_waterMat);
            if (_foamMat != null) Destroy(_foamMat);
            if (_mesh != null) Destroy(_mesh);
        }
    }
}
