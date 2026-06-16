#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay.DevTools;

namespace Ziptide.Editor.DevTools
{
    /// <summary>
    /// Regenerates the runtime <see cref="DevWorldManifest"/> from every WorldPackDefinition in the
    /// project, so the in-VR Dev Menu has an on-device list of worlds + markers. Run via
    /// <c>Ziptide → Dev → Rebuild Dev World Manifest</c> whenever worlds are added/renamed.
    /// </summary>
    public static class DevWorldManifestBuilder
    {
        private const string ResourcesFolder = "Assets/Ziptide/Resources";
        private const string AssetPath = "Assets/Ziptide/Resources/DevWorldManifest.asset";

        [MenuItem("Ziptide/Dev/Rebuild Dev World Manifest")]
        public static void Rebuild()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
                AssetDatabase.CreateFolder("Assets/Ziptide", "Resources");

            var manifest = AssetDatabase.LoadAssetAtPath<DevWorldManifest>(AssetPath);
            if (manifest == null)
            {
                manifest = ScriptableObject.CreateInstance<DevWorldManifest>();
                AssetDatabase.CreateAsset(manifest, AssetPath);
            }

            manifest.worlds.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:WorldPackDefinition"))
            {
                var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (pack == null || string.IsNullOrEmpty(pack.sceneName)) continue;

                var entry = new DevWorldManifest.Entry
                {
                    sceneName = pack.sceneName,
                    displayName = string.IsNullOrEmpty(pack.displayName) ? pack.packId : pack.displayName
                };
                if (pack.spawnMarkers != null)
                    foreach (var m in pack.spawnMarkers)
                        if (m != null && !string.IsNullOrEmpty(m.markerId)) entry.markerIds.Add(m.markerId);

                manifest.worlds.Add(entry);
            }

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] DevWorldManifest rebuilt: " + manifest.worlds.Count + " world(s).");
        }
    }
}
#endif
