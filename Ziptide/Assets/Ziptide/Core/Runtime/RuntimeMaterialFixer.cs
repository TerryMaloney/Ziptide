using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Replaces null or non-URP materials on renderers at runtime so scene objects are visible on Quest.
    /// Uses Universal Render Pipeline/Lit and assigns distinguishable colors by object name.
    /// </summary>
    public static class RuntimeMaterialFixer
    {
        private const string URPShaderName = "Universal Render Pipeline/Lit";
        private const string ShaderBaseColor = "_BaseColor";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            Shader urpLit = Shader.Find(URPShaderName);
            if (urpLit == null)
            {
                Debug.LogWarning("[Ziptide] RuntimeMaterialFixer: URP Lit shader not found. Materials will not be replaced.");
                return;
            }

            Renderer[] renderers = Object.FindObjectsOfType<Renderer>(true);
            int replaced = 0;
            foreach (Renderer r in renderers)
            {
                if (r == null) continue;

                bool needsReplace = false;
                for (int i = 0; i < r.sharedMaterials.Length; i++)
                {
                    Material mat = r.sharedMaterials[i];
                    if (mat == null || mat.shader == null)
                    {
                        needsReplace = true;
                        break;
                    }
                    string shaderName = mat.shader.name ?? "";
                    if (!shaderName.Contains("Universal Render Pipeline"))
                    {
                        needsReplace = true;
                        break;
                    }
                }

                if (!needsReplace) continue;

                Material fallback = new Material(urpLit);
                fallback.name = "RuntimeMaterialFixer_" + r.gameObject.name;
                fallback.hideFlags = HideFlags.HideAndDontSave;
                Color color = ColorForName(r.gameObject.name);
                if (fallback.HasProperty(Shader.PropertyToID(ShaderBaseColor)))
                    fallback.SetColor(ShaderBaseColor, color);

                r.sharedMaterial = fallback;
                replaced++;
            }

            Debug.Log($"[Ziptide] RuntimeMaterialFixer: replaced materials on {replaced} renderer(s), checked {renderers.Length} total.");
        }

        private static Color ColorForName(string name)
        {
            if (name == null) return Color.gray;
            string n = name.ToLowerInvariant();
            if (n.Contains("ground") || n.Contains("plane") || n.Contains("floor"))
                return new Color(0.45f, 0.55f, 0.45f, 1f);
            if (n.Contains("cube") || n.Contains("grabbable"))
                return new Color(0.1f, 0.6f, 1f, 1f);
            return new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
}
