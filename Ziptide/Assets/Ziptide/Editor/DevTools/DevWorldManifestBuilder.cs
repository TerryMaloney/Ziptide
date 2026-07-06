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

                // Copy the world's sky colors so runtime systems (THE ZIPTIDE gate tint) can see
                // them without loading Visuals assets. Alpha stays 0 when no vista is authored.
                var vista = AssetDatabase.LoadAssetAtPath<Ziptide.Visuals.SkyVistaDefinition>(
                    Ziptide.Editor.Patching.SkyVistaLibrary.AssetPathFor(pack.sceneName));
                if (vista != null && vista.skyGradient != null)
                {
                    entry.skyHorizon = Opaque(vista.skyGradient.Evaluate(0f));
                    entry.skyZenith = Opaque(vista.skyGradient.Evaluate(1f));
                }

                manifest.worlds.Add(entry);
            }

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] DevWorldManifest rebuilt: " + manifest.worlds.Count + " world(s).");
        }

        private static Color Opaque(Color c) => new Color(c.r, c.g, c.b, 1f);
    }
}
#endif
