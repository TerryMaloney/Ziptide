#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE II E1.4 — TEXTURES ON DEVICE. Bakes every ForgeRecipeDefinition into shippable
    /// assets under Resources/ForgeBaked/&lt;id&gt;/: the single-submesh mesh, the four atlas maps
    /// as PNGs (imported with proper types → mips + platform ASTC), ONE URP/Lit material, and a
    /// prefab the runtime applier prefers. The folder is GITIGNORED — regenerated in every build
    /// workspace from the recipes (deterministic), never committed. This is what turns the
    /// headset's flat-color proxies into the textured assets the photo booth has been showing.
    /// </summary>
    public static class ForgeBaker
    {
        public const string BakedRoot = "Assets/Ziptide/Resources/ForgeBaked";
        private const int AtlasSize = 1024;

        [MenuItem("Ziptide/Art/Bake Forge Assets (mesh + maps + prefabs)")]
        public static void BakeAllFromMenu()
        {
            int n = BakeAll();
            EditorUtility.DisplayDialog("Forge Baker", n + " recipe(s) baked to " + BakedRoot, "OK");
        }

        /// <summary>Build-hooked: bake every recipe asset. Returns the count baked.</summary>
        public static int BakeAll()
        {
            Directory.CreateDirectory(BakedRoot);
            int baked = 0;
            foreach (string guid in AssetDatabase.FindAssets("t:ForgeRecipeDefinition",
                         new[] { "Assets/Ziptide/Resources/Forge" }))
            {
                var recipe = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (recipe == null || string.IsNullOrEmpty(recipe.recipeId)) continue;
                try { Bake(recipe); baked++; }
                catch (System.Exception e)
                {
                    Debug.LogWarning("[Ziptide] ForgeBaker: bake failed for '" + recipe.recipeId + "': " + e.Message);
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] ForgeBaker: baked " + baked + " recipe(s) → " + BakedRoot);
            return baked;
        }

        private static void Bake(ForgeRecipeDefinition recipe)
        {
            string dir = BakedRoot + "/" + recipe.recipeId;
            Directory.CreateDirectory(dir);

            // ── Mesh (single submesh — one material) ─────────────────────────
            var mesh = ForgeMesh.BuildSingle(recipe);
            string meshPath = dir + "/mesh.asset";
            var existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (existingMesh != null) { EditorUtility.CopySerialized(mesh, existingMesh); mesh = existingMesh; }
            else AssetDatabase.CreateAsset(mesh, meshPath);

            // ── The four maps ────────────────────────────────────────────────
            var meta = ForgeTexture.BakeMeta(recipe, AtlasSize);
            var px = new Color32[AtlasSize * AtlasSize];

            ForgeTexture.BakeAlbedo(recipe, meta, AtlasSize, px);
            string albedoPath = WritePng(dir + "/albedo.png", px);
            ForgeTexture.BakeNormal(recipe, meta, AtlasSize, px);
            string normalPath = WritePng(dir + "/normal.png", px);
            ForgeTexture.BakeMSA(recipe, meta, AtlasSize, px);
            string msaPath = WritePng(dir + "/msa.png", px);
            ForgeTexture.BakeEmissive(recipe, meta, AtlasSize, px);
            string emissivePath = WritePng(dir + "/emissive.png", px);

            AssetDatabase.Refresh();
            ConfigureImporter(albedoPath, TextureImporterType.Default, srgb: true);
            ConfigureImporter(normalPath, TextureImporterType.NormalMap, srgb: false);
            ConfigureImporter(msaPath, TextureImporterType.Default, srgb: false);
            ConfigureImporter(emissivePath, TextureImporterType.Default, srgb: true);

            // ── ONE material ─────────────────────────────────────────────────
            string matPath = dir + "/material.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, matPath);
            }
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(albedoPath));
            mat.SetColor("_BaseColor", Color.white);
            mat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath));
            mat.EnableKeyword("_NORMALMAP");
            mat.SetTexture("_MetallicGlossMap", AssetDatabase.LoadAssetAtPath<Texture2D>(msaPath));
            mat.SetFloat("_Smoothness", 1f); // the baked A channel is the truth
            mat.EnableKeyword("_METALLICGLOSSMAP");
            float maxI = ForgeTexture.MaxEmissiveIntensity(recipe);
            if (maxI > 0f)
            {
                mat.SetTexture("_EmissionMap", AssetDatabase.LoadAssetAtPath<Texture2D>(emissivePath));
                mat.SetColor("_EmissionColor", Color.white * Mathf.Min(maxI, 2f));
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            EditorUtility.SetDirty(mat);

            // ── Prefab (what ForgeVisualApplier instantiates) ────────────────
            var go = new GameObject("ForgeBaked_" + recipe.recipeId);
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Quest budget
            PrefabUtility.SaveAsPrefabAsset(go, dir + "/prefab.prefab");
            Object.DestroyImmediate(go);
        }

        private static string WritePng(string path, Color32[] px)
        {
            var tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.RGBA32, false);
            tex.SetPixels32(px);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            return path;
        }

        private static void ConfigureImporter(string path, TextureImporterType type, bool srgb)
        {
            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) return;
            bool dirty = ti.textureType != type || ti.sRGBTexture != srgb || !ti.mipmapEnabled;
            ti.textureType = type;
            ti.sRGBTexture = srgb;
            ti.mipmapEnabled = true; // ASTC per the project's Android platform defaults
            if (dirty) ti.SaveAndReimport();
        }
    }
}
#endif
