#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>Shared-material, batching-static primitive translator for ToxicCity Stage A.</summary>
    internal sealed class CityStageAPrimitiveFactory
    {
        private readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        public int MaterialCount => _materials.Count;

        public GameObject Cube(Transform parent, string name, Vector3 position, Vector3 scale,
            Color color, bool collider = false, bool emissive = false)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;
            var col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = MaterialFor(color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            return go;
        }

        public Color WindowColor(CityWindowLight mode, GlobalPalette palette)
        {
            switch (mode)
            {
                case CityWindowLight.HomeWarm:
                    return Color.Lerp(palette.accent, new Color(1f, 0.42f, 0.14f), 0.48f);
                case CityWindowLight.ShopCool:
                    return Color.Lerp(palette.facadeWindow, new Color(0.20f, 0.82f, 1f), 0.72f);
                case CityWindowLight.IndustrialNeutral:
                    return Color.Lerp(palette.facadeWindow, new Color(0.84f, 0.90f, 0.82f), 0.62f);
                default:
                    return Color.Lerp(palette.facadeWindow, Color.black, 0.20f);
            }
        }

        private Material MaterialFor(Color color, bool emissive)
        {
            string key = ColorUtility.ToHtmlStringRGBA(color) + (emissive ? "_E" : "_L");
            if (_materials.TryGetValue(key, out Material material) && material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = "CityStageA_" + key };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.8f);
            }
            _materials[key] = material;
            return material;
        }
    }
}
#endif
