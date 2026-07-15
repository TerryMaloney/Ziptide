using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// The WEATHER half of the skyscape — haze, motes, and body glow at real stereo depths.
    /// Transparent layers use fixed shipped Ziptide shaders rather than runtime URP keyword/blend
    /// mutation, so Android renders the same blend behavior verified in the editor.
    /// </summary>
    public class SkyAtmosphereRig : MonoBehaviour
    {
        private const string AlphaShaderName = "Ziptide/TransparentAlphaUnlit";
        private const string AdditiveShaderName = "Ziptide/AdditiveUnlit";
        private const int HazeSegments = 24;
        private const int GlowTextureSize = 128;
        private const float MoteFollowLag = 2.2f;

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
            var atmosphere = vista != null ? vista.atmosphere : null;
            if (atmosphere == null || !atmosphere.enabled)
            {
                Clear();
                return;
            }

            _spec = SkyAtmosphere.ForHazard(atmosphere.hazardTag, atmosphere.intensity);
            if (atmosphere.hazeTintOverride.a > 0f)
                _spec.HazeTint = atmosphere.hazeTintOverride;
            _seed = string.IsNullOrEmpty(vista.vistaId) ? 1 : vista.vistaId.GetHashCode();
            _moteCenter = _player.position;

            BuildHaze();
            BuildMotes();
            BuildGlow(atmosphere.bodyGlow);
            Debug.Log("ZIPTIDE: SKY_ATMO hazard=" + atmosphere.hazardTag +
                      " motes=" + _spec.MoteCount +
                      " haze=" + (_spec.HazeEnabled ? 1 : 0) +
                      " glow=" + (atmosphere.bodyGlow ? 1 : 0));
        }

        private void Update()
        {
            if (_player == null) return;
            Vector3 center = _player.position;

            if (_hazeGo != null)
                _hazeGo.transform.position = center;

            if (_moteGo != null)
            {
                _moteCenter = Vector3.Lerp(
                    _moteCenter,
                    center,
                    Time.deltaTime * MoteFollowLag);
                _moteGo.transform.position = _moteCenter;
                UpdateMoteVertices();
            }

            if (_glowGo != null &&
                _vista != null &&
                _vista.bodies != null &&
                _vista.bodies.Count > 0)
            {
                var definition = _vista.bodies[0];
                Vector3 direction = definition.direction.sqrMagnitude > 0.01f
                    ? definition.direction.normalized
                    : new Vector3(0f, 0.45f, 0.89f);
                const float glowDistance = 430f;
                _glowGo.transform.position = center + direction * glowDistance;
                _glowGo.transform.rotation = Quaternion.LookRotation(direction);
                float bodyDiameter = Mathf.Tan(
                    definition.angularSizeDeg * Mathf.Deg2Rad * 0.5f) *
                    glowDistance * 2f;
                _glowGo.transform.localScale = Vector3.one * bodyDiameter * 1.9f;
            }
        }

        private void BuildHaze()
        {
            DestroyLayer(ref _hazeGo, ref _hazeMat, ref _hazeTex, null);
            if (!_spec.HazeEnabled) return;

            var mesh = new Mesh { name = "SkyAtmo_HazeRing" };
            float radius = _spec.HazeDistance;
            float height = _spec.HazeHeight;
            var vertices = new Vector3[(HazeSegments + 1) * 2];
            var uvs = new Vector2[vertices.Length];
            var triangles = new int[HazeSegments * 6];

            for (int i = 0; i <= HazeSegments; i++)
            {
                float angle = i / (float)HazeSegments * Mathf.PI * 2f;
                Vector3 rim = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);
                vertices[i * 2] = rim + Vector3.down * (height * 0.35f);
                vertices[i * 2 + 1] = rim + Vector3.up * height;
                uvs[i * 2] = new Vector2(i / (float)HazeSegments, 0f);
                uvs[i * 2 + 1] = new Vector2(i / (float)HazeSegments, 1f);
            }

            for (int i = 0; i < HazeSegments; i++)
            {
                int vertex = i * 2;
                int triangle = i * 6;
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + 1;
                triangles[triangle + 2] = vertex + 2;
                triangles[triangle + 3] = vertex + 1;
                triangles[triangle + 4] = vertex + 3;
                triangles[triangle + 5] = vertex + 2;
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(
                Vector3.zero,
                new Vector3(radius * 2.2f, height * 3f, radius * 2.2f));

            _hazeTex = MakeVerticalFadeTexture();
            _hazeMat = MakeTransparentUnlit(
                "SkyAtmo_Haze",
                _hazeTex,
                _spec.HazeTint,
                additive: false);
            if (_hazeMat == null)
            {
                Destroy(mesh);
                Destroy(_hazeTex);
                _hazeTex = null;
                return;
            }
            _hazeGo = MakeLayerObject("SkyAtmoHaze", mesh, _hazeMat, 2900);
        }

        private void BuildMotes()
        {
            DestroyLayer(ref _moteGo, ref _moteMat, ref _moteTex, _moteMesh);
            _moteMesh = null;
            if (_spec.MoteCount <= 0) return;

            int count = _spec.MoteCount;
            _moteMesh = new Mesh { name = "SkyAtmo_Motes" };
            _moteVerts = new Vector3[count * 4];
            var uvs = new Vector2[count * 4];
            var triangles = new int[count * 6];

            for (int i = 0; i < count; i++)
            {
                uvs[i * 4] = new Vector2(0f, 0f);
                uvs[i * 4 + 1] = new Vector2(1f, 0f);
                uvs[i * 4 + 2] = new Vector2(1f, 1f);
                uvs[i * 4 + 3] = new Vector2(0f, 1f);
                int vertex = i * 4;
                int triangle = i * 6;
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + 2;
                triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex;
                triangles[triangle + 4] = vertex + 3;
                triangles[triangle + 5] = vertex + 2;
            }

            _moteMesh.vertices = _moteVerts;
            _moteMesh.uv = uvs;
            _moteMesh.triangles = triangles;
            _moteMesh.MarkDynamic();
            _moteMesh.bounds = new Bounds(
                Vector3.zero,
                Vector3.one * (_spec.ShellFar * 2.5f));

            _moteTex = MakeSoftDotTexture();
            _moteMat = MakeTransparentUnlit(
                "SkyAtmo_Motes",
                _moteTex,
                _spec.MoteTint,
                additive: false);
            if (_moteMat == null)
            {
                DestroyLayer(ref _moteGo, ref _moteMat, ref _moteTex, _moteMesh);
                _moteMesh = null;
                return;
            }

            _moteGo = MakeLayerObject(
                "SkyAtmoMotes",
                _moteMesh,
                _moteMat,
                2901);
            UpdateMoteVertices();
        }

        private void UpdateMoteVertices()
        {
            if (_moteMesh == null || _player == null) return;
            var camera = Camera.main;
            Transform eye = camera != null ? camera.transform : _player;
            Vector3 right = eye.right * (_spec.MoteSize * 0.5f);
            Vector3 up = eye.up * (_spec.MoteSize * 0.5f);
            float time = Time.time;
            float eyeY = eye.position.y - _moteGo.transform.position.y;

            for (int i = 0; i < _spec.MoteCount; i++)
            {
                Vector3 position = SkyAtmosphere.MotePosition(
                    _seed,
                    i,
                    time,
                    in _spec);
                position.y += eyeY;
                int vertex = i * 4;
                _moteVerts[vertex] = position - right - up;
                _moteVerts[vertex + 1] = position + right - up;
                _moteVerts[vertex + 2] = position + right + up;
                _moteVerts[vertex + 3] = position - right + up;
            }

            _moteMesh.vertices = _moteVerts;
        }

        private void BuildGlow(bool enabled)
        {
            DestroyLayer(ref _glowGo, ref _glowMat, ref _glowTex, null);
            if (!enabled ||
                _vista == null ||
                _vista.bodies == null ||
                _vista.bodies.Count == 0)
                return;

            var quad = new Mesh { name = "SkyAtmo_Glow" };
            quad.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
            };
            quad.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            };
            quad.triangles = new[] { 0, 1, 2, 0, 2, 3 };

            Color glow = _vista.bodies[0].baseColor;
            glow.a = 0.55f;
            _glowTex = MakeRadialGlowTexture();
            _glowMat = MakeTransparentUnlit(
                "SkyAtmo_Glow",
                _glowTex,
                glow,
                additive: true);
            if (_glowMat == null)
            {
                Destroy(quad);
                Destroy(_glowTex);
                _glowTex = null;
                return;
            }
            _glowGo = MakeLayerObject("SkyAtmoBodyGlow", quad, _glowMat, 1000);
        }

        private static Texture2D MakeVerticalFadeTexture()
        {
            const int height = 64;
            var texture = new Texture2D(2, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                float alpha = Mathf.SmoothStep(1f, 0f, v) *
                              Mathf.SmoothStep(0f, 1f, Mathf.Min(1f, v * 6f + 0.25f));
                var color = new Color(1f, 1f, 1f, alpha);
                texture.SetPixel(0, y, color);
                texture.SetPixel(1, y, color);
            }
            texture.Apply(false, false);
            return texture;
        }

        private static Texture2D MakeSoftDotTexture()
        {
            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(
                        new Vector2(x, y),
                        new Vector2(size / 2f, size / 2f)) / (size / 2f);
                    texture.SetPixel(
                        x,
                        y,
                        new Color(1f, 1f, 1f, Mathf.Clamp01(1f - distance * distance * 1.4f)));
                }
            }
            texture.Apply(false, false);
            return texture;
        }

        private static Texture2D MakeRadialGlowTexture()
        {
            const int size = GlowTextureSize;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(
                        new Vector2(x, y),
                        new Vector2(size / 2f, size / 2f)) / (size / 2f);
                    float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 2.2f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply(false, false);
            return texture;
        }

        private GameObject MakeLayerObject(
            string name,
            Mesh mesh,
            Material material,
            int renderQueue)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            material.renderQueue = renderQueue;
            return go;
        }

        private static Material MakeTransparentUnlit(
            string name,
            Texture2D texture,
            Color tint,
            bool additive)
        {
            string shaderName = additive ? AdditiveShaderName : AlphaShaderName;
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError("ZIPTIDE: SHADER_MISSING name=" + shaderName +
                               " effect=SkyAtmosphereRig layer=" + name);
                return null;
            }

            var material = new Material(shader)
            {
                name = name,
                hideFlags = HideFlags.HideAndDontSave
            };
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", tint);
            return material;
        }

        private void DestroyLayer(
            ref GameObject go,
            ref Material material,
            ref Texture2D texture,
            Mesh mesh)
        {
            if (go != null) Destroy(go);
            if (material != null) Destroy(material);
            if (texture != null) Destroy(texture);
            if (mesh != null) Destroy(mesh);
            go = null;
            material = null;
            texture = null;
        }

        private void Clear()
        {
            DestroyLayer(ref _hazeGo, ref _hazeMat, ref _hazeTex, null);
            DestroyLayer(ref _moteGo, ref _moteMat, ref _moteTex, _moteMesh);
            DestroyLayer(ref _glowGo, ref _glowMat, ref _glowTex, null);
            _moteMesh = null;
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
