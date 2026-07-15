using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.4 — a soft contact shadow at a body's base. The shared material uses the
    /// committed <c>Ziptide/TransparentAlphaUnlit</c> shader, so Android never has to retain a
    /// runtime-created URP transparent variant.
    /// </summary>
    public static class GroundShadow
    {
        public const string ChildName = "GroundShadow";
        private const string AlphaShaderName = "Ziptide/TransparentAlphaUnlit";

        private static Material _material;
        private static Texture2D _texture;
        private static Mesh _quad;

        /// <summary>Attach a blob shadow of the given XZ <paramref name="radius"/> to
        /// <paramref name="host"/>, sitting just above the ground.</summary>
        public static void Attach(GameObject host, float radius, float y = 0.02f)
        {
            if (host == null || radius <= 0f) return;
            Material material = SharedMaterial();
            if (material == null) return;

            var go = new GameObject(ChildName);
            go.transform.SetParent(host.transform, false);
            go.transform.localPosition = new Vector3(0f, y, 0f);
            go.transform.localScale = new Vector3(radius * 2f, 1f, radius * 2f);

            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = QuadMesh();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        private static Material SharedMaterial()
        {
            if (_material != null) return _material;

            Shader shader = Shader.Find(AlphaShaderName);
            if (shader == null)
            {
                Debug.LogError("ZIPTIDE: SHADER_MISSING name=" + AlphaShaderName +
                               " effect=GroundShadow");
                return null;
            }

            const int size = 128;
            var pixels = GroundDecal.BakeAlpha(size, stain: false);
            _texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "GroundShadowRadial",
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            _texture.SetPixels32(pixels);
            _texture.Apply(false, false);

            _material = new Material(shader)
            {
                name = "GroundShadow",
                hideFlags = HideFlags.HideAndDontSave
            };
            _material.SetColor("_BaseColor", new Color(0f, 0f, 0f, 0.55f));
            _material.SetTexture("_BaseMap", _texture);
            return _material;
        }

        private static Mesh QuadMesh()
        {
            if (_quad != null) return _quad;
            _quad = new Mesh
            {
                name = "GroundShadowQuad",
                hideFlags = HideFlags.HideAndDontSave
            };
            _quad.vertices = new[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(-0.5f, 0f, 0.5f),
            };
            _quad.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            };
            _quad.triangles = new[] { 0, 2, 1, 0, 3, 2 };
            _quad.normals = new[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            _quad.RecalculateBounds();
            return _quad;
        }
    }
}
