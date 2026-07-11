using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.4 — the BLOB SHADOW: a soft dark radial decal parented at a body's base so a
    /// creature (or the player) reads as standing ON the ground, not floating above it. The plan's
    /// cheapest "20% more grounded" dial. One shared material + quad across all shadows (1 texture,
    /// alpha-blended, no depth write, no real shadow). A LOOK, never a stat: no collider.
    /// </summary>
    public static class GroundShadow
    {
        public const string ChildName = "GroundShadow";
        private static Material _mat;
        private static Mesh _quad;

        /// <summary>Attach a blob shadow of the given XZ <paramref name="radius"/> to
        /// <paramref name="host"/>, sitting at local y=<paramref name="y"/> (just above the ground).
        /// Idempotent-ish: adds one child named <see cref="ChildName"/>.</summary>
        public static void Attach(GameObject host, float radius, float y = 0.02f)
        {
            if (host == null || radius <= 0f) return;
            var mat = SharedMat();
            if (mat == null) return;

            var go = new GameObject(ChildName);
            go.transform.SetParent(host.transform, false);
            go.transform.localPosition = new Vector3(0f, y, 0f);
            go.transform.localScale = new Vector3(radius * 2f, 1f, radius * 2f);
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = QuadMesh();
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static Material SharedMat()
        {
            if (_mat != null) return _mat;
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            if (lit == null) return null;

            const int s = 128;
            var px = GroundDecal.BakeAlpha(s, stain: false);
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            tex.SetPixels32(px); tex.Apply(false, false);

            _mat = new Material(lit) { name = "GroundShadow" };
            _mat.SetColor("_BaseColor", new Color(0f, 0f, 0f, 0.55f)); // dark, ≤55% opacity at the core
            _mat.SetTexture("_BaseMap", tex);
            _mat.SetFloat("_Surface", 1f); // transparent
            _mat.SetOverrideTag("RenderType", "Transparent");
            _mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _mat.SetInt("_ZWrite", 0);
            _mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            _mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            return _mat;
        }

        private static Mesh QuadMesh()
        {
            if (_quad != null) return _quad;
            _quad = new Mesh { name = "GroundShadowQuad" };
            _quad.vertices = new[]
            {
                new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, 0.5f), new Vector3(-0.5f, 0f, 0.5f),
            };
            _quad.uv = new[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f) };
            _quad.triangles = new[] { 0, 2, 1, 0, 3, 2 }; // +Y up-facing
            _quad.normals = new[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            _quad.RecalculateBounds();
            return _quad;
        }
    }
}
