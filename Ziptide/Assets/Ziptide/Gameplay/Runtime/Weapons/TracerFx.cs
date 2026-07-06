using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// One-call hitscan tracer: a bright line from muzzle to impact that lives ~90 ms. Test Day 1:
    /// "the grey weapon doesn't show anything shooting out of it" — every shot must be visible.
    /// No prefabs; materials cached per color (ForgeMaterials pattern) so firing never allocates
    /// more than the throwaway line object.
    /// </summary>
    public static class TracerFx
    {
        private static readonly Dictionary<Color, Material> MatCache = new Dictionary<Color, Material>();

        public static void Spawn(Vector3 from, Vector3 to, Color color, float width = 0.012f, float life = 0.09f)
        {
            var go = new GameObject("__Tracer");
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
            lr.startWidth = width;
            lr.endWidth = width * 0.4f;
            lr.material = Mat(color);
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Object.Destroy(go, life);
        }

        private static Material Mat(Color color)
        {
            if (MatCache.TryGetValue(color, out var cached) && cached != null) return cached;
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            var mat = shader != null ? new Material(shader) : new Material(Shader.Find("Sprites/Default"));
            mat.SetColor("_BaseColor", color);
            mat.color = color;
            MatCache[color] = mat;
            return mat;
        }
    }
}
