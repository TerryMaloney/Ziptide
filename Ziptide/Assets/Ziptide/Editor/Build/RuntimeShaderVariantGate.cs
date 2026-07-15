#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ziptide.Build
{
    /// <summary>
    /// Android shader-variant safety gate. Runtime code may create materials from shipped fixed
    /// shaders, but it may not turn opaque URP shaders into transparent/additive materials through
    /// keywords or blend-state mutation. Those variants can exist in the desktop editor and still
    /// be stripped from the Quest player.
    /// </summary>
    public static class RuntimeShaderVariantGate
    {
        private const string AlphaShaderPath =
            "Assets/Ziptide/Resources/ZiptideTransparentAlphaUnlit.shader";
        private const string AdditiveShaderPath =
            "Assets/Ziptide/Resources/ZiptideAdditiveUnlit.shader";
        private const string AlphaShaderName = "Ziptide/TransparentAlphaUnlit";
        private const string AdditiveShaderName = "Ziptide/AdditiveUnlit";

        private static readonly string[] ForbiddenRuntimeTokens =
        {
            "\"_SURFACE_TYPE_TRANSPARENT\"",
            "\"_ALPHAPREMULTIPLY_ON\"",
            "\"_SrcBlend\"",
            "\"_DstBlend\"",
            "SetFloat(\"_Surface\"",
            "SetFloat(\"_Blend\"",
            "SetOverrideTag(\"RenderType\""
        };

        public static void Validate()
        {
            ValidateShader(AlphaShaderPath, AlphaShaderName);
            ValidateShader(AdditiveShaderPath, AdditiveShaderName);

            string firstPartyRoot = Path.Combine(Application.dataPath, "Ziptide");
            if (!Directory.Exists(firstPartyRoot))
                throw new DirectoryNotFoundException(
                    "First-party source root is missing: " + firstPartyRoot);

            var findings = new List<string>();
            int scanned = 0;

            foreach (string path in Directory.EnumerateFiles(
                         firstPartyRoot,
                         "*.cs",
                         SearchOption.AllDirectories))
            {
                string normalized = path.Replace('\\', '/');
                if (normalized.Contains("/Editor/") ||
                    normalized.Contains("/Tests/") ||
                    normalized.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    continue;

                scanned++;
                string source = File.ReadAllText(path);
                foreach (string token in ForbiddenRuntimeTokens)
                {
                    int searchFrom = 0;
                    while (searchFrom < source.Length)
                    {
                        int index = source.IndexOf(
                            token,
                            searchFrom,
                            StringComparison.Ordinal);
                        if (index < 0) break;

                        int line = 1;
                        for (int i = 0; i < index; i++)
                            if (source[i] == '\n') line++;

                        string relative = "Assets" + normalized.Substring(
                            Application.dataPath.Replace('\\', '/').Length);
                        findings.Add(relative + ":" + line + " token=" + token);
                        searchFrom = index + token.Length;
                    }
                }
            }

            if (findings.Count > 0)
            {
                string evidence = string.Join(
                    Environment.NewLine,
                    findings.OrderBy(value => value, StringComparer.Ordinal));
                Debug.LogError("ZIPTIDE: SHADER_VARIANT_GATE_FAIL findings=" +
                               findings.Count + Environment.NewLine + evidence);
                throw new InvalidOperationException(
                    "Runtime transparent shader mutation is forbidden. Use a committed fixed " +
                    "Ziptide shader instead." + Environment.NewLine + evidence);
            }

            Debug.Log("ZIPTIDE: SHADER_VARIANT_GATE_OK scanned=" + scanned +
                      " shaders=" + AlphaShaderName + "," + AdditiveShaderName);
        }

        private static void ValidateShader(string assetPath, string expectedName)
        {
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
            if (shader == null)
                throw new FileNotFoundException(
                    "Required shipped shader asset did not import.",
                    assetPath);
            if (!string.Equals(shader.name, expectedName, StringComparison.Ordinal))
                throw new InvalidDataException(
                    "Shipped shader name mismatch at " + assetPath +
                    ". expected=" + expectedName + " actual=" + shader.name);
            if (!shader.isSupported)
                throw new InvalidOperationException(
                    "Required shipped shader is unsupported in the active build target: " +
                    expectedName);
        }
    }
}
#endif
