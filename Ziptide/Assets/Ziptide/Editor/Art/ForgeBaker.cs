#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE II E1.4 — TEXTURES ON DEVICE. Bakes every ForgeRecipeDefinition into shippable
    /// assets under Resources/ForgeBaked/&lt;id&gt;/. The folder is regenerated from scratch for
    /// every build. Any failed bake, missing output, missing importer, or count mismatch aborts the
    /// build so stale/primitive fallback content cannot hide behind a successful APK result.
    /// </summary>
    public static class ForgeBaker
    {
        public const string BakedRoot = "Assets/Ziptide/Resources/ForgeBaked";
        private const int AtlasSize = 1024;
        private const int WaterNormalSize = 512;

        [MenuItem("Ziptide/Art/Bake Forge Assets (mesh + maps + prefabs)")]
        public static void BakeAllFromMenu()
        {
            int n = BakeAll();
            EditorUtility.DisplayDialog("Forge Baker", n + " asset(s) baked to " + BakedRoot, "OK");
        }

        /// <summary>
        /// Build-hooked deterministic bake. Returns the verified number of recipe, styled-body,
        /// and shared-water outputs. There are no best-effort catches in this path.
        /// </summary>
        public static int BakeAll()
        {
            ResetBakedRoot();

            int expected = 0;
            int baked = 0;

            foreach (string guid in AssetDatabase.FindAssets(
                         "t:ForgeRecipeDefinition",
                         new[] { "Assets/Ziptide/Resources/Forge" }))
            {
                var recipe = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (recipe == null || string.IsNullOrEmpty(recipe.recipeId)) continue;

                expected++;
                Bake(recipe);
                VerifyRecipeOutputs(recipe.recipeId);
                baked++;
            }

            // Creature bodies: maps + one material. Runtime builds the skinned mesh itself.
            foreach (string guid in AssetDatabase.FindAssets(
                         "t:ForgeCreatureBody",
                         new[] { "Assets/Ziptide/Resources/Forge/Bodies" }))
            {
                var body = AssetDatabase.LoadAssetAtPath<ForgeCreatureBody>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (body == null || string.IsNullOrEmpty(body.bodyId)) continue;
                if (body.slotStyles == null || body.slotStyles.Length == 0) continue;

                expected++;
                var synth = ForgeSkinnedBuilder.SyntheticRecipe(body);
                try
                {
                    string dir = BakedRoot + "/body_" + body.bodyId;
                    Directory.CreateDirectory(dir);
                    BakeMapsAndMaterial(synth, dir);
                    VerifyBodyOutputs(body.bodyId);
                    baked++;
                }
                finally
                {
                    if (synth != null) UnityEngine.Object.DestroyImmediate(synth);
                }
            }

            expected++;
            BakeWaterNormal();
            VerifyRequiredFile(BakedRoot + "/water_normal.png");
            baked++;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (baked != expected)
                throw new InvalidOperationException(
                    "Forge bake count mismatch. expected=" + expected + " baked=" + baked);

            Debug.Log("ZIPTIDE: FORGE_BAKE_VERIFIED expected=" + expected +
                      " baked=" + baked + " root=" + BakedRoot);
            return baked;
        }

        private static void ResetBakedRoot()
        {
            if (Directory.Exists(BakedRoot))
                FileUtil.DeleteFileOrDirectory(BakedRoot);
            if (File.Exists(BakedRoot + ".meta"))
                FileUtil.DeleteFileOrDirectory(BakedRoot + ".meta");

            Directory.CreateDirectory(BakedRoot);
            AssetDatabase.Refresh();

            if (!Directory.Exists(BakedRoot))
                throw new DirectoryNotFoundException(
                    "Forge baked root could not be recreated: " + BakedRoot);
        }

        private static void BakeWaterNormal()
        {
            Directory.CreateDirectory(BakedRoot);
            var px = WaterSurface.BakeNormalMap(WaterNormalSize, 1.4f);
            string path = BakedRoot + "/water_normal.png";
            var tex = new Texture2D(WaterNormalSize, WaterNormalSize, TextureFormat.RGBA32, false);
            try
            {
                tex.SetPixels32(px);
                File.WriteAllBytes(path, tex.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tex);
            }

            AssetDatabase.Refresh();
            ConfigureImporter(path, TextureImporterType.NormalMap, srgb: false);
        }

        private static void Bake(ForgeRecipeDefinition recipe)
        {
            string dir = BakedRoot + "/" + recipe.recipeId;
            Directory.CreateDirectory(dir);

            var mesh = ForgeMesh.BuildSingle(recipe);
            if (mesh == null)
                throw new InvalidOperationException(
                    "ForgeMesh.BuildSingle returned null for " + recipe.recipeId);

            string meshPath = dir + "/mesh.asset";
            var existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (existingMesh != null)
            {
                EditorUtility.CopySerialized(mesh, existingMesh);
                UnityEngine.Object.DestroyImmediate(mesh);
                mesh = existingMesh;
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, meshPath);
            }

            var mat = BakeMapsAndMaterial(recipe, dir);

            var go = new GameObject("ForgeBaked_" + recipe.recipeId);
            try
            {
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                string prefabPath = dir + "/prefab.prefab";
                GameObject saved = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                if (saved == null)
                    throw new InvalidOperationException(
                        "PrefabUtility.SaveAsPrefabAsset returned null for " + recipe.recipeId);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        private static Material BakeMapsAndMaterial(ForgeRecipeDefinition recipe, string dir)
        {
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

            string matPath = dir + "/material.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                    throw new InvalidOperationException(
                        "Required shader not found: Universal Render Pipeline/Lit");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, matPath);
            }

            Texture2D albedo = RequireTexture(albedoPath);
            Texture2D normal = RequireTexture(normalPath);
            Texture2D msa = RequireTexture(msaPath);
            Texture2D emissive = RequireTexture(emissivePath);

            mat.SetTexture("_BaseMap", albedo);
            mat.SetColor("_BaseColor", Color.white);
            mat.SetTexture("_BumpMap", normal);
            mat.EnableKeyword("_NORMALMAP");
            mat.SetTexture("_MetallicGlossMap", msa);
            mat.SetFloat("_Smoothness", 1f);
            mat.EnableKeyword("_METALLICGLOSSMAP");

            if (ForgeTexture.HasLeafSlot(recipe))
            {
                mat.SetFloat("_AlphaClip", 1f);
                mat.SetFloat("_Cutoff", 0.5f);
                mat.EnableKeyword("_ALPHATEST_ON");
            }

            float maxI = ForgeTexture.MaxEmissiveIntensity(recipe);
            if (maxI > 0f)
            {
                mat.SetTexture("_EmissionMap", emissive);
                mat.SetColor("_EmissionColor", Color.white * Mathf.Min(maxI, 2f));
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static string WritePng(string path, Color32[] px)
        {
            var tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.RGBA32, false);
            try
            {
                tex.SetPixels32(px);
                File.WriteAllBytes(path, tex.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tex);
            }

            VerifyRequiredFile(path);
            return path;
        }

        private static void ConfigureImporter(
            string path,
            TextureImporterType type,
            bool srgb)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                throw new InvalidOperationException(
                    "Texture importer was not created for required bake output: " + path);

            bool dirty = importer.textureType != type ||
                         importer.sRGBTexture != srgb ||
                         !importer.mipmapEnabled;
            importer.textureType = type;
            importer.sRGBTexture = srgb;
            importer.mipmapEnabled = true;
            if (dirty) importer.SaveAndReimport();
        }

        private static Texture2D RequireTexture(string path)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
                throw new InvalidOperationException(
                    "Required baked texture failed to import: " + path);
            return texture;
        }

        private static void VerifyRecipeOutputs(string recipeId)
        {
            string dir = BakedRoot + "/" + recipeId;
            VerifyRequiredFile(dir + "/mesh.asset");
            VerifyRequiredFile(dir + "/material.mat");
            VerifyRequiredFile(dir + "/prefab.prefab");
            VerifyMapOutputs(dir);
        }

        private static void VerifyBodyOutputs(string bodyId)
        {
            string dir = BakedRoot + "/body_" + bodyId;
            VerifyRequiredFile(dir + "/material.mat");
            VerifyMapOutputs(dir);
        }

        private static void VerifyMapOutputs(string dir)
        {
            VerifyRequiredFile(dir + "/albedo.png");
            VerifyRequiredFile(dir + "/normal.png");
            VerifyRequiredFile(dir + "/msa.png");
            VerifyRequiredFile(dir + "/emissive.png");
        }

        private static void VerifyRequiredFile(string assetPath)
        {
            string absolutePath = Path.GetFullPath(assetPath);
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException(
                    "Required Forge bake output is missing.", absolutePath);
            if (new FileInfo(absolutePath).Length <= 0)
                throw new InvalidDataException(
                    "Required Forge bake output is empty: " + absolutePath);
        }
    }
}
#endif
