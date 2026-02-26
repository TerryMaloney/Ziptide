using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ziptide.Editor.Validation
{
    /// <summary>
    /// Enforces 00_LOCKED_CONTRACTS: Core must not reference Visuals.
    /// Run via menu Ziptide / Validate dependencies or before build.
    /// </summary>
    public static class DependencyValidator
    {
        private const string MenuPath = "Ziptide/Validate dependencies";

        [MenuItem(MenuPath)]
        public static void ValidateFromMenu()
        {
            if (Validate(out string message))
                Debug.Log("[Ziptide] Dependency validation passed.");
            else
                EditorUtility.DisplayDialog("Ziptide Dependency Validation", message, "OK");
        }

        /// <summary>
        /// Call from build pipeline (e.g. IPreprocessBuildWithReport) to fail build on violation.
        /// </summary>
        public static bool Validate(out string errorMessage)
        {
            errorMessage = null;
            string dataPath = Application.dataPath;
            string projectRoot = Directory.GetParent(dataPath)?.FullName;
            if (string.IsNullOrEmpty(projectRoot))
            {
                errorMessage = "Could not resolve project root.";
                return false;
            }

            string assetsPath = Path.Combine(projectRoot, "Assets");
            string[] asmdefPaths = Directory.GetFiles(assetsPath, "*.asmdef", SearchOption.AllDirectories);

            foreach (string path in asmdefPaths)
            {
                string json = File.ReadAllText(path);
                if (!TryParseAsmdefReferences(json, path, out string assemblyName, out string[] references))
                    continue;

                // Rule: Ziptide.Core must not reference Ziptide.Visuals (or any other Ziptide.* that would create a cycle or violate contracts)
                if (assemblyName == "Ziptide.Core")
                {
                    if (references.Any(r => r == "Ziptide.Visuals"))
                    {
                        errorMessage = "00_LOCKED_CONTRACTS violation: Ziptide.Core must not reference Ziptide.Visuals. Core must not depend on Visuals.";
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool TryParseAsmdefReferences(string json, string pathForLog, out string name, out string[] references)
        {
            name = null;
            references = Array.Empty<string>();
            try
            {
                var data = JsonUtility.FromJson<AsmDefStub>(json);
                if (data == null)
                    return false;
                name = data.name;
                references = data.references ?? Array.Empty<string>();
                return !string.IsNullOrEmpty(name);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Ziptide] Could not parse asmdef at {pathForLog}: {e.Message}");
                return false;
            }
        }

        [Serializable]
        private class AsmDefStub
        {
            public string name;
            public string[] references;
        }
    }
}
