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
            int count = BakeAll();
            EditorUtility.DisplayDialog(
                "Forge Baker",
                count + " asset(s) baked to " + BakedRoot,
                "OK");
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

            foreach (string guid in AssetDatabase.FindAssets(
                         "t:ForgeCreatureBody",
                         new[] { "Assets/Ziptide/Resources/Forge/Bodies" }))
            {
                var body = AssetDatabase.LoadAssetAtPath<ForgeCreatureBody>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (body == null || string.IsNullOrEmpty(body.bodyId)) continue;
                if (body.slotStyles == null || body.slotStyles.Length == 0) continue;

                expected++;
                var synthetic = ForgeSkinnedBuilder.SyntheticRecipe(body);
                try
                {
                    string directory = BakedRoot + "/body_" + body.bodyId;
                    Directory.CreateDirectory(directory);
                    BakeMapsAndMaterial(synthetic, directory);
                    VerifyBodyOutputs(body.bodyId);
                    baked++;
                }
                finally
                {
                    if (synthetic != null)
                        UnityEngine.Object.DestroyImmediate(synthetic);
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
            var pixels = WaterSurface.BakeNormalMap(WaterNormalSize, 1.4f);
            string path = BakedRoot + "/water_normal.png";
            var texture = new Texture2D(
                WaterNormalSize,
                WaterNormalSize,
                TextureFormat.RGBA32,
                false);
            try
            {
                texture.SetPixels32(pixels);
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }

            AssetDatabase.Refresh();
            ConfigureImporter(path, TextureImporterType.NormalMap, srgb: false);
        }

        private static void Bake(ForgeRecipeDefinition recipe)
        {
            string directory = BakedRoot + "/" + recipe.recipeId;
            Directory.CreateDirectory(directory);

            var mesh = ForgeMesh.BuildSingle(recipe);
            if (mesh == null)
                throw new InvalidOperationException(
                    "ForgeMesh.BuildSingle returned null for " + recipe.recipeId);

            string meshPath = directory + "/mesh.asset";
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

            var material = BakeMapsAndMaterial(recipe, directory);

            var go = new GameObject("ForgeBaked_" + recipe.recipeId);
            try
            {
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;
                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                string prefabPath = directory + "/prefab.prefab";
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

        private static Material BakeMapsAndMaterial(
            ForgeRecipeDefinition recipe,
            string directory)
        {
            var metadata = ForgeTexture.BakeMeta(recipe, AtlasSize);
            var pixels = new Color32[AtlasSize * AtlasSize];

            ForgeTexture.BakeAlbedo(recipe, metadata, AtlasSize, pixels);
            string albedoPath = WritePng(directory + "/albedo.png", pixels);
            ForgeTexture.BakeNormal(recipe, metadata, AtlasSize, pixels);
            string normalPath = WritePng(directory + "/normal.png", pixels);
            ForgeTexture.BakeMSA(recipe, metadata, AtlasSize, pixels);
            string msaPath = WritePng(directory + "/msa.png", pixels);
            ForgeTexture.BakeEmissive(recipe, metadata, AtlasSize, pixels);
            string emissivePath = WritePng(directory + "/emissive.png", pixels);

            AssetDatabase.Refresh();
            ConfigureImporter(albedoPath, TextureImporterType.Default, srgb: true);
            ConfigureImporter(normalPath, TextureImporterType.NormalMap, srgb: false);
            ConfigureImporter(msaPath, TextureImporterType.Default, srgb: false);
            ConfigureImporter(emissivePath, TextureImporterType.Default, srgb: true);

            string materialPath = directory + "/material.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                    throw new InvalidOperationException(
                        "Required shader not found: Universal Render Pipeline/Lit");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }

            Texture2D albedo = RequireTexture(albedoPath);
            Texture2D normal = RequireTexture(normalPath);
            Texture2D msa = RequireTexture(msaPath);
            Texture2D emissive = RequireTexture(emissivePath);

            material.SetTexture("_BaseMap", albedo);
            material.SetColor("_BaseColor", Color.white);
            material.SetTexture("_BumpMap", normal);
            material.EnableKeyword("_NORMALMAP");
            material.SetTexture("_MetallicGlossMap", msa);
            material.SetFloat("_Smoothness", 1f);
            material.EnableKeyword("_METALLICGLOSSMAP");

            if (ForgeTexture.HasLeafSlot(recipe))
            {
                material.SetFloat("_AlphaClip", 1f);
                material.SetFloat("_Cutoff", 0.5f);
                material.EnableKeyword("_ALPHATEST_ON");
            }

            float maximumEmission = ForgeTexture.MaxEmissiveIntensity(recipe);
            if (maximumEmission > 0f)
            {
                material.SetTexture("_EmissionMap", emissive);
                material.SetColor(
                    "_EmissionColor",
                    Color.white * Mathf.Min(maximumEmission, 2f));
                material.EnableKeyword("_EMISSION");
                material.globalIlluminationFlags =
                    MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static string WritePng(string path, Color32[] pixels)
        {
            var texture = new Texture2D(
                AtlasSize,
                AtlasSize,
                TextureFormat.RGBA32,
                false);
            try
            {
                texture.SetPixels32(pixels);
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
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
            string directory = BakedRoot + "/" + recipeId;
            VerifyRequiredFile(directory + "/mesh.asset");
            VerifyRequiredFile(directory + "/material.mat");
            VerifyRequiredFile(directory + "/prefab.prefab");
            VerifyMapOutputs(directory);
        }

        private static void VerifyBodyOutputs(string bodyId)
        {
            string directory = BakedRoot + "/body_" + bodyId;
            VerifyRequiredFile(directory + "/material.mat");
            VerifyMapOutputs(directory);
        }

        private static void VerifyMapOutputs(string directory)
        {
            VerifyRequiredFile(directory + "/albedo.png");
            VerifyRequiredFile(directory + "/normal.png");
            VerifyRequiredFile(directory + "/msa.png");
            VerifyRequiredFile(directory + "/emissive.png");
        }

        private static void VerifyRequiredFile(string assetPath)
        {
            string absolutePath = Path.GetFullPath(assetPath);
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException(
                    "Required Forge bake output is missing.",
                    absolutePath);
            if (new FileInfo(absolutePath).Length <= 0)
                throw new InvalidDataException(
                    "Required Forge bake output is empty: " + absolutePath);
        }
    }
}
#endif
