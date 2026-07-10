using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// The WEATHER half of the skyscape (SKYSCAPE_DESIGN.md §3) — three cheap layers at REAL
    /// world-space depths so stereo VR separates them the moment the player leans:
    ///   · HAZE RING — one 24-segment cylinder band at ~40m, soft-alpha procedural texture, tinted
    ///     by the hazard default (or the vista's override). Follows the player 1:1 like the dome.
    ///   · MOTE FIELD — ONE mesh of billboard quads at 8–15m whose positions are re-evaluated every
    ///     frame from the pure SkyAtmosphere.MotePosition(seed, index, time) — no particle system,
    ///     no simulation state, and the exact math CI already pinned is what the player sees.
    ///   · BODY GLOW — a soft additive quad behind body 0 (Signature tier only) so the giant's edge
    ///     reads as light scattering through air instead of a cardboard cutout.
    /// Budget: 3 renderers, 3 materials, ZERO lights (the §3 law). Created by SkyVistaRig when the
    /// vista's atmosphere block is enabled.
    /// </summary>
    public class SkyAtmosphereRig : MonoBehaviour
    {
        private const string URPUnlitShaderName = "Universal Render Pipeline/Unlit";
        private const int HazeSegments = 24;
        private const int GlowTextureSize = 128;
        private const float MoteFollowLag = 2.2f;   // s^-1 — motes trail the player so leaning parallaxes them

        private AtmosphereSpec _spec;
        private SkyVistaDefinition _vista;
        private Transform _player;
        private int _seed;

        private GameObject _hazeGo;
        private Material _hazeMat;
        private Texture2D _hazeTex;

        private GameObject _moteGo;
        private Mesh _moteMesh;
        private Material _moteMat;
        private Texture2D _moteTex;
        private Vector3[] _moteVerts;
        private Vector3 _moteCenter;

        private GameObject _glowGo;
        private Material _glowMat;
        private Texture2D _glowTex;

        public void Apply(SkyVistaDefinition vista, Transform player)
        {
            _vista = vista;
            _player = player != null ? player : transform;
            var atmo = vista != null ? vista.atmosphere : null;
            if (atmo == null || !atmo.enabled) { Clear(); return; }

            _spec = SkyAtmosphere.ForHazard(atmo.hazardTag, atmo.intensity);
            if (atmo.hazeTintOverride.a > 0f) _spec.HazeTint = atmo.hazeTintOverride;
            _seed = string.IsNullOrEmpty(vista.vistaId) ? 1 : vista.vistaId.GetHashCode();
            _moteCenter = _player.position;

            BuildHaze();
            BuildMotes();
            BuildGlow(atmo.bodyGlow);
            Debug.Log("ZIPTIDE: SKY_ATMO hazard=" + atmo.hazardTag + " motes=" + _spec.MoteCount +
                      " haze=" + (_spec.HazeEnabled ? 1 : 0) + " glow=" + (atmo.bodyGlow ? 1 : 0));
        }

        private void Update()
        {
            if (_player == null) return;
            Vector3 center = _player.position;

            // The haze is "the air" — it is always around you (binocular depth still reads at 40m).
            if (_hazeGo != null)
                _hazeGo.transform.position = new Vector3(center.x, center.y, center.z);

            // Motes follow LOOSELY: quick head motion out-runs the field and buys true parallax,
            // walking never escapes the cloud.
            if (_moteGo != null)
            {
                _moteCenter = Vector3.Lerp(_moteCenter, center, Time.deltaTime * MoteFollowLag);
                _moteGo.transform.position = _moteCenter;
                UpdateMoteVerts();
            }

            if (_glowGo != null && _vista != null && _vista.bodies != null && _vista.bodies.Count > 0)
            {
                var def = _vista.bodies[0];
                Vector3 dir = def.direction.sqrMagnitude > 0.01f ? def.direction.normalized
                                                                 : new Vector3(0f, 0.45f, 0.89f);
                const float glowDistance = 430f;   // just behind the body shell (420) — renders first
                _glowGo.transform.position = center + dir * glowDistance;
                _glowGo.transform.rotation = Quaternion.LookRotation(dir);
                float bodyDiameter = Mathf.Tan(def.angularSizeDeg * Mathf.Deg2Rad * 0.5f) * glowDistance * 2f;
                _glowGo.transform.localScale = Vector3.one * bodyDiameter * 1.9f;
            }
        }

        // ── Haze ring ────────────────────────────────────────────────────────
        private void BuildHaze()
        {
            DestroyLayer(ref _hazeGo, ref _hazeMat, ref _hazeTex, null);
            if (!_spec.HazeEnabled) return;

            var mesh = new Mesh { name = "SkyAtmo_HazeRing" };
            float r = _spec.HazeDistance, h = _spec.HazeHeight;
            var verts = new Vector3[(HazeSegments + 1) * 2];
            var uvs = new Vector2[verts.Length];
            var tris = new int[HazeSegments * 6];
            for (int i = 0; i <= HazeSegments; i++)
            {
                float a = i / (float)HazeSegments * Mathf.PI * 2f;
                Vector3 rim = new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
                verts[i * 2] = rim + Vector3.down * (h * 0.35f);   // dips below eye line
                verts[i * 2 + 1] = rim + Vector3.up * h;
                uvs[i * 2] = new Vector2(i / (float)HazeSegments, 0f);
                uvs[i * 2 + 1] = new Vector2(i / (float)HazeSegments, 1f);
            }
            for (int i = 0; i < HazeSegments; i++)
            {
                int v = i * 2, t = i * 6;
                // Wound to face INWARD at the player.
                tris[t] = v; tris[t + 1] = v + 1; tris[t + 2] = v + 2;
                tris[t + 3] = v + 1; tris[t + 4] = v + 3; tris[t + 5] = v + 2;
            }
            mesh.vertices = verts; mesh.uv = uvs; mesh.triangles = tris;
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(r * 2.2f, h * 3f, r * 2.2f));

            _hazeTex = MakeVerticalFadeTexture();
            _hazeMat = MakeTransparentUnlit("SkyAtmo_Haze", _hazeTex, _spec.HazeTint, additive: false);
            _hazeGo = MakeLayerObject("SkyAtmoHaze", mesh, _hazeMat, renderQueue: 2900);
        }

        // ── Mote field (one mesh, pure-function motion) ──────────────────────
        private void BuildMotes()
        {
            DestroyLayer(ref _moteGo, ref _moteMat, ref _moteTex, _moteMesh);
            _moteMesh = null;
            if (_spec.MoteCount <= 0) return;

            int n = _spec.MoteCount;
            _moteMesh = new Mesh { name = "SkyAtmo_Motes" };
            _moteVerts = new Vector3[n * 4];
            var uvs = new Vector2[n * 4];
            var tris = new int[n * 6];
            for (int i = 0; i < n; i++)
            {
                uvs[i * 4] = new Vector2(0f, 0f); uvs[i * 4 + 1] = new Vector2(1f, 0f);
                uvs[i * 4 + 2] = new Vector2(1f, 1f); uvs[i * 4 + 3] = new Vector2(0f, 1f);
                int v = i * 4, t = i * 6;
                tris[t] = v; tris[t + 1] = v + 2; tris[t + 2] = v + 1;
                tris[t + 3] = v; tris[t + 4] = v + 3; tris[t + 5] = v + 2;
            }
            _moteMesh.vertices = _moteVerts;   // filled by UpdateMoteVerts each frame
            _moteMesh.uv = uvs;
            _moteMesh.triangles = tris;
            _moteMesh.MarkDynamic();
            _moteMesh.bounds = new Bounds(Vector3.zero, Vector3.one * (_spec.ShellFar * 2.5f));

            _moteTex = MakeSoftDotTexture();
            _moteMat = MakeTransparentUnlit("SkyAtmo_Motes", _moteTex, _spec.MoteTint, additive: false);
            _moteGo = MakeLayerObject("SkyAtmoMotes", _moteMesh, _moteMat, renderQueue: 2901);
            UpdateMoteVerts();
        }

        private void UpdateMoteVerts()
        {
            if (_moteMesh == null || _player == null) return;
            var cam = Camera.main;
            Transform eye = cam != null ? cam.transform : _player;
            Vector3 right = eye.right * (_spec.MoteSize * 0.5f);
            Vector3 up = eye.up * (_spec.MoteSize * 0.5f);
            float t = Time.time;
            float eyeY = eye.position.y - _moteGo.transform.position.y;

            int n = _spec.MoteCount;
            for (int i = 0; i < n; i++)
            {
                Vector3 p = SkyAtmosphere.MotePosition(_seed, i, t, in _spec);
                p.y += eyeY;   // the shell rides at eye height, whatever the terrain does
                int v = i * 4;
                _moteVerts[v] = p - right - up;
                _moteVerts[v + 1] = p + right - up;
                _moteVerts[v + 2] = p + right + up;
                _moteVerts[v + 3] = p - right + up;
            }
            _moteMesh.vertices = _moteVerts;
        }

        // ── Body glow ────────────────────────────────────────────────────────
        private void BuildGlow(bool enabled)
        {
            DestroyLayer(ref _glowGo, ref _glowMat, ref _glowTex, null);
            if (!enabled || _vista == null || _vista.bodies == null || _vista.bodies.Count == 0) return;

            var quad = new Mesh { name = "SkyAtmo_Glow" };
            quad.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f),
            };
            quad.uv = new[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f) };
            quad.triangles = new[] { 0, 1, 2, 0, 2, 3 };   // faces -Z; LookRotation(dir) turns -Z back at the player

            Color glow = _vista.bodies[0].baseColor;
            glow.a = 0.55f;
            _glowTex = MakeRadialGlowTexture();
            _glowMat = MakeTransparentUnlit("SkyAtmo_Glow", _glowTex, glow, additive: true);
            _glowGo = MakeLayerObject("SkyAtmoBodyGlow", quad, _glowMat, renderQueue: 1000); // with the dome, behind the body
        }

        // ── Procedural textures (tiny, generated once) ───────────────────────
        private static Texture2D MakeVerticalFadeTexture()
        {
            const int h = 64;
            var tex = new Texture2D(2, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < h; y++)
            {
                float v = y / (float)(h - 1);
                // Solid low band easing to nothing at the top; soft toe at the very bottom.
                float a = Mathf.SmoothStep(1f, 0f, v) * Mathf.SmoothStep(0f, 1f, Mathf.Min(1f, v * 6f + 0.25f));
                var c = new Color(1f, 1f, 1f, a);
                tex.SetPixel(0, y, c); tex.SetPixel(1, y, c);
            }
            tex.Apply(false, false);
            return tex;
        }

        private static Texture2D MakeSoftDotTexture()
        {
            const int s = 32;
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(s / 2f, s / 2f)) / (s / 2f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - d * d * 1.4f)));
                }
            tex.Apply(false, false);
            return tex;
        }

        private static Texture2D MakeRadialGlowTexture()
        {
            const int s = GlowTextureSize;
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(s / 2f, s / 2f)) / (s / 2f);
                    float a = Mathf.Pow(Mathf.Clamp01(1f - d), 2.2f);   // hot center, long soft falloff
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            tex.Apply(false, false);
            return tex;
        }

        // ── Plumbing ─────────────────────────────────────────────────────────
        private GameObject MakeLayerObject(string name, Mesh mesh, Material mat, int renderQueue)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
            mat.renderQueue = renderQueue;
            return go;
        }

        private static Material MakeTransparentUnlit(string name, Texture2D tex, Color tint, bool additive)
        {
            Shader unlit = Shader.Find(URPUnlitShaderName);
            if (unlit == null) unlit = Shader.Find("Unlit/Transparent");
            var mat = new Material(unlit) { name = name, hideFlags = HideFlags.HideAndDontSave };
            // URP/Unlit transparent setup (the SkyVistaRig sibling pattern, plus alpha blending).
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);   // Transparent
            if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", additive ? 2f : 0f); // Additive : Alpha
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", additive ? (int)UnityEngine.Rendering.BlendMode.One
                                             : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            if (mat.HasProperty("_Cull")) mat.SetInt("_Cull", 0);   // double-sided: haze ring seen from inside, billboards from any lean
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            else mat.mainTexture = tex;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", tint);
            else mat.color = tint;
            return mat;
        }

        private void DestroyLayer(ref GameObject go, ref Material mat, ref Texture2D tex, Mesh mesh)
        {
            if (go != null) Destroy(go);
            if (mat != null) Destroy(mat);
            if (tex != null) Destroy(tex);
            if (mesh != null) Destroy(mesh);
            go = null; mat = null; tex = null;
        }

        private void Clear()
        {
            DestroyLayer(ref _hazeGo, ref _hazeMat, ref _hazeTex, null);
            DestroyLayer(ref _moteGo, ref _moteMat, ref _moteTex, _moteMesh);
            DestroyLayer(ref _glowGo, ref _glowMat, ref _glowTex, null);
            _moteMesh = null;
        }

        private void OnDestroy() => Clear();
    }
}
