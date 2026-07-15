using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryRenderSnapshotMetrics
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string scene;
        public string cameraPath;
        public int width;
        public int height;
        public string pngSha256;
        public int pngBytes;
        public double averageLuminance;
        public double luminanceStandardDeviation;
        public double minimumLuminance;
        public double maximumLuminance;
        public double dynamicRange;
        public double nearBlackRatio;
        public double darkRatio;
        public double nearWhiteRatio;
        public double transparentRatio;
        public int quantizedColorCount;
        public string clearFlags;
        public string backgroundColor;
        public float fieldOfView;
        public float nearClipPlane;
        public float farClipPlane;
        public string[] activeSceneNames;
    }

    public sealed class RecoveryRenderSnapshotPaths
    {
        public string PngPath { get; }
        public string JsonPath { get; }

        public RecoveryRenderSnapshotPaths(string pngPath, string jsonPath)
        {
            PngPath = pngPath;
            JsonPath = jsonPath;
        }
    }

    /// <summary>
    /// Test-owned renderer-capable R1 snapshot seam. It renders the actual supplied camera into a
    /// temporary target, writes a PNG plus objective frame metrics, and restores every camera/render
    /// state it touched. The metadata makes blank, uniformly dark, clipped-white and low-information
    /// frames machine-visible before visual review; it does not attempt to grade artistic quality.
    /// </summary>
    public static class RecoveryRenderSnapshot
    {
        public const string ArtifactDirectoryName = "recovery-snapshots";
        public const int DefaultWidth = 960;
        public const int DefaultHeight = 960;

        public static RecoveryRenderSnapshotPaths Capture(
            Camera camera,
            string label,
            string stem,
            int width = DefaultWidth,
            int height = DefaultHeight)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Snapshot label is required.", nameof(label));
            if (width < 64 || height < 64)
                throw new ArgumentOutOfRangeException(nameof(width), "Snapshot dimensions must be at least 64 pixels.");

            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                ArtifactDirectoryName));
            Directory.CreateDirectory(directory);
            string safeStem = SanitizeStem(stem);
            string pngPath = Path.Combine(directory, safeStem + ".png");
            string jsonPath = Path.Combine(directory, safeStem + ".json");

            RenderTexture previousTarget = camera.targetTexture;
            StereoTargetEyeMask previousStereo = camera.stereoTargetEye;
            RenderTexture previousActive = RenderTexture.active;
            float previousAspect = camera.aspect;

            var renderTarget = new RenderTexture(
                width,
                height,
                24,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB)
            {
                name = "__RECOVERY_SNAPSHOT_RT",
                antiAliasing = 1,
                useMipMap = false,
                autoGenerateMips = false
            };
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false)
            {
                name = "__RECOVERY_SNAPSHOT_TEXTURE"
            };

            try
            {
                renderTarget.Create();
                camera.targetTexture = renderTarget;
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                camera.aspect = width / (float)height;
                camera.Render();

                RenderTexture.active = renderTarget;
                texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
                texture.Apply(false, false);

                byte[] png = texture.EncodeToPNG();
                if (png == null || png.Length == 0)
                    throw new InvalidOperationException("Unity returned an empty PNG payload.");
                File.WriteAllBytes(pngPath, png);

                RecoveryRenderSnapshotMetrics metrics = BuildMetrics(
                    camera,
                    label,
                    width,
                    height,
                    png,
                    texture.GetPixels32());
                File.WriteAllText(
                    jsonPath,
                    JsonUtility.ToJson(metrics, true) + Environment.NewLine,
                    Encoding.UTF8);

                Debug.Log("ZIPTIDE: RECOVERY_SNAPSHOT label=" + label
                    + " scene=" + metrics.scene
                    + " camera=" + metrics.cameraPath
                    + " avg=" + metrics.averageLuminance.ToString("F4")
                    + " dark=" + metrics.darkRatio.ToString("F4")
                    + " colors=" + metrics.quantizedColorCount
                    + " pngBytes=" + metrics.pngBytes);
                return new RecoveryRenderSnapshotPaths(pngPath, jsonPath);
            }
            finally
            {
                camera.targetTexture = previousTarget;
                camera.stereoTargetEye = previousStereo;
                camera.aspect = previousAspect;
                RenderTexture.active = previousActive;
                renderTarget.Release();
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(renderTarget);
            }
        }

        public static RecoveryRenderSnapshotMetrics ReadMetrics(string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath))
                throw new FileNotFoundException("Recovery snapshot metadata was not found.", jsonPath);
            RecoveryRenderSnapshotMetrics metrics =
                JsonUtility.FromJson<RecoveryRenderSnapshotMetrics>(File.ReadAllText(jsonPath));
            if (metrics == null)
                throw new InvalidDataException("Recovery snapshot metadata did not deserialize: " + jsonPath);
            return metrics;
        }

        private static RecoveryRenderSnapshotMetrics BuildMetrics(
            Camera camera,
            string label,
            int width,
            int height,
            byte[] png,
            Color32[] pixels)
        {
            if (pixels == null || pixels.Length != width * height)
                throw new InvalidDataException("Snapshot pixel count does not match its dimensions.");

            double sum = 0d;
            double sumSquares = 0d;
            double min = 1d;
            double max = 0d;
            int nearBlack = 0;
            int dark = 0;
            int nearWhite = 0;
            int transparent = 0;
            var colors = new HashSet<int>();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color32 pixel = pixels[i];
                double r = pixel.r / 255d;
                double g = pixel.g / 255d;
                double b = pixel.b / 255d;
                double luminance = 0.2126d * r + 0.7152d * g + 0.0722d * b;
                sum += luminance;
                sumSquares += luminance * luminance;
                if (luminance < min) min = luminance;
                if (luminance > max) max = luminance;
                if (luminance <= 0.01d) nearBlack++;
                if (luminance <= 0.05d) dark++;
                if (luminance >= 0.97d) nearWhite++;
                if (pixel.a <= 2) transparent++;

                int key = ((pixel.r >> 3) << 10) | ((pixel.g >> 3) << 5) | (pixel.b >> 3);
                colors.Add(key);
            }

            double count = pixels.Length;
            double average = sum / count;
            double variance = Math.Max(0d, sumSquares / count - average * average);
            string[] sceneNames = new string[SceneManager.sceneCount];
            for (int i = 0; i < SceneManager.sceneCount; i++)
                sceneNames[i] = SceneManager.GetSceneAt(i).name;
            Array.Sort(sceneNames, StringComparer.Ordinal);

            return new RecoveryRenderSnapshotMetrics
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                scene = camera.gameObject.scene.IsValid()
                    ? camera.gameObject.scene.name
                    : "INVALID_SCENE",
                cameraPath = RecoveryRuntimeCensus.HierarchyPath(camera.transform),
                width = width,
                height = height,
                pngSha256 = Sha256(png),
                pngBytes = png.Length,
                averageLuminance = average,
                luminanceStandardDeviation = Math.Sqrt(variance),
                minimumLuminance = min,
                maximumLuminance = max,
                dynamicRange = max - min,
                nearBlackRatio = nearBlack / count,
                darkRatio = dark / count,
                nearWhiteRatio = nearWhite / count,
                transparentRatio = transparent / count,
                quantizedColorCount = colors.Count,
                clearFlags = camera.clearFlags.ToString(),
                backgroundColor = ColorUtility.ToHtmlStringRGBA(camera.backgroundColor),
                fieldOfView = camera.fieldOfView,
                nearClipPlane = camera.nearClipPlane,
                farClipPlane = camera.farClipPlane,
                activeSceneNames = sceneNames
            };
        }

        private static string Sha256(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) builder.Append(hash[i].ToString("x2"));
                return builder.ToString();
            }
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "recovery-snapshot";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }
    }
}
