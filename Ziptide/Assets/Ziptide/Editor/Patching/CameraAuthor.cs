#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FIELD CAMERA — create-only author (the ArenaWeaponAuthor pattern): seeds the
    /// CameraDefinition asset into Resources/Items (the ItemFactory registry path) if missing, never
    /// overwrites — the asset is the editable truth for feel tuning. Build-hooked in BuildAndroid so
    /// CI ships it; menu for editor preview.
    /// </summary>
    public static class CameraAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Items";
        private const string ItemId = "handheld_camera";

        [MenuItem("Ziptide/Items/Author Field Camera (create-only)")]
        public static void EnsureAuthored()
        {
            string path = Folder + "/HandheldCamera.asset";
            if (AssetDatabase.LoadAssetAtPath<CameraDefinition>(path) != null) return;
            Directory.CreateDirectory(Folder);
            var def = ScriptableObject.CreateInstance<CameraDefinition>();
            def.itemId = ItemId;
            def.mass = 0.4f;
            AssetDatabase.CreateAsset(def, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] CameraAuthor: created " + path);
        }
    }
}
#endif
