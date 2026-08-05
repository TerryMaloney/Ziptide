using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ziptide.Editor.Setup
{
    /// <summary>
    /// Forces the URP assets through their upgrade path and PERSISTS the result.
    ///
    /// WHY THIS EXISTS. The Unity 6 move (2022.3.62f3 + URP 14 -> 6000.2.9f1 + URP 17.2) left every
    /// render-pipeline asset serialized at its old k_AssetVersion (11-13). URP refuses to build
    /// while any of them is behind:
    ///
    ///   BuildFailedException: The UniversalRenderPipelineAsset with 'URP-HighFidelity(...)'
    ///   is not at last version.
    ///
    /// URP migrates on deserialize, but nothing marks the asset dirty afterwards, so simply opening
    /// the project does not write the upgraded version back to disk - the build fails again on the
    /// next run. ForceReserializeAssets loads each asset and rewrites it with the CURRENT
    /// serialization, which both runs the migration and saves it.
    ///
    /// This is deliberately written with no URP type references, so it compiles whether or not the
    /// Editor assembly can see com.unity.render-pipelines.universal.
    ///
    /// Menu: Ziptide > Setup > Migrate render pipeline assets
    /// Batch: -executeMethod Ziptide.Editor.Setup.MigrateRenderPipelineAssets.MigrateAll
    /// </summary>
    public static class MigrateRenderPipelineAssets
    {
        private const string MenuPath = "Ziptide/Setup/Migrate render pipeline assets";

        /// <summary>Folders and single files that hold render-pipeline configuration.</summary>
        private static readonly string[] SearchFolders = { "Assets/Settings" };

        private static readonly string[] SingleAssets =
        {
            "Assets/UniversalRenderPipelineGlobalSettings.asset"
        };

        [MenuItem(MenuPath)]
        public static void MigrateAll()
        {
            var paths = new List<string>();

            foreach (string folder in SearchFolders)
            {
                if (!Directory.Exists(folder)) continue;
                foreach (string file in Directory.GetFiles(folder, "*.asset", SearchOption.AllDirectories))
                    paths.Add(file.Replace('\\', '/'));
            }

            foreach (string single in SingleAssets)
                if (File.Exists(single))
                    paths.Add(single);

            if (paths.Count == 0)
            {
                Debug.Log("ZIPTIDE: RP_MIGRATE_SKIP no render pipeline assets found");
                return;
            }

            // Re-serializing an already-current asset is a no-op, so this is safe to run every time.
            AssetDatabase.ForceReserializeAssets(paths, ForceReserializeAssetsOptions.ReserializeAssets);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("ZIPTIDE: RP_MIGRATE_OK reserialized=" + paths.Count);
            foreach (string path in paths)
                Debug.Log("ZIPTIDE: RP_MIGRATE_ASSET path=" + path + " version=" + ReadAssetVersion(path));
        }

        /// <summary>Reads k_AssetVersion straight out of the YAML, for an honest before/after log.</summary>
        private static string ReadAssetVersion(string path)
        {
            if (!File.Exists(path)) return "missing";
            foreach (string line in File.ReadAllLines(path))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("k_AssetVersion:") || trimmed.StartsWith("m_AssetVersion:"))
                    return trimmed;
            }
            return "n/a";
        }
    }
}
