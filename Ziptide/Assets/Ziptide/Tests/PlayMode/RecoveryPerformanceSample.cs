using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoveryPerformanceSampleRecord
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string scene;
        public string healthSweepEvidence;
        public int sampledFrames;
        public float averageFrameMs;
        public float medianFrameMs;
        public float p95FrameMs;
        public float maximumFrameMs;
        public int materials;
        public int textures;
        public int meshes;
        public int audioClips;
        public int activeRenderers;
        public int activeAudioSources;
        public long totalAllocatedMB;
        public long totalReservedMB;
        public long monoUsedMB;
        public long monoHeapMB;
        public int gcGen0Collections;
        public int gcGen1Collections;
        public int gcGen2Collections;
        public int retainedSnapshotGpuObjects;
        public int retainedControlledRendererObjects;
        public int retainedControlledRendererMaterials;
        public string interpretation;
    }

    public sealed class RecoveryPerformanceArtifactPaths
    {
        public string JsonPath { get; }
        public string MarkdownPath { get; }

        public RecoveryPerformanceArtifactPaths(string jsonPath, string markdownPath)
        {
            JsonPath = jsonPath;
            MarkdownPath = markdownPath;
        }
    }

    /// <summary>
    /// R1.10 reference-run sampler. It records objective resource, memory and frame-distribution
    /// evidence only after the caller has observed the matching RuntimeHealthMonitor HEALTH_SWEEP.
    /// These Linux reference-renderer numbers are a regression baseline, never a Quest budget claim.
    /// </summary>
    public static class RecoveryPerformanceSample
    {
        public const string ArtifactDirectoryName = "recovery-performance";

        public static RecoveryPerformanceSampleRecord Capture(
            string label,
            string healthSweepEvidence,
            IList<float> frameMilliseconds)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Performance label is required.", nameof(label));
            if (string.IsNullOrWhiteSpace(healthSweepEvidence) ||
                healthSweepEvidence.IndexOf("ZIPTIDE: HEALTH_SWEEP", StringComparison.Ordinal) < 0)
                throw new ArgumentException(
                    "A matching HEALTH_SWEEP log line is required before performance capture.",
                    nameof(healthSweepEvidence));
            if (frameMilliseconds == null || frameMilliseconds.Count == 0)
                throw new ArgumentException("At least one frame sample is required.", nameof(frameMilliseconds));

            var sorted = new List<float>(frameMilliseconds.Count);
            double total = 0d;
            float maximum = 0f;
            for (int i = 0; i < frameMilliseconds.Count; i++)
            {
                float value = frameMilliseconds[i];
                if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(frameMilliseconds),
                        "Frame samples must be finite non-negative milliseconds.");
                sorted.Add(value);
                total += value;
                maximum = Mathf.Max(maximum, value);
            }
            sorted.Sort();

            Renderer[] renderers = UnityEngine.Object.FindObjectsOfType<Renderer>();
            AudioSource[] audioSources = UnityEngine.Object.FindObjectsOfType<AudioSource>();
            return new RecoveryPerformanceSampleRecord
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                scene = SceneManager.GetActiveScene().name,
                healthSweepEvidence = healthSweepEvidence,
                sampledFrames = frameMilliseconds.Count,
                averageFrameMs = (float)(total / frameMilliseconds.Count),
                medianFrameMs = Percentile(sorted, 0.50f),
                p95FrameMs = Percentile(sorted, 0.95f),
                maximumFrameMs = maximum,
                materials = Resources.FindObjectsOfTypeAll<Material>().Length,
                textures = Resources.FindObjectsOfTypeAll<Texture2D>().Length,
                meshes = Resources.FindObjectsOfTypeAll<Mesh>().Length,
                audioClips = Resources.FindObjectsOfTypeAll<AudioClip>().Length,
                activeRenderers = renderers != null ? renderers.Length : 0,
                activeAudioSources = audioSources != null ? audioSources.Length : 0,
                totalAllocatedMB = ToMB(Profiler.GetTotalAllocatedMemoryLong()),
                totalReservedMB = ToMB(Profiler.GetTotalReservedMemoryLong()),
                monoUsedMB = ToMB(Profiler.GetMonoUsedSizeLong()),
                monoHeapMB = ToMB(Profiler.GetMonoHeapSizeLong()),
                gcGen0Collections = GC.CollectionCount(0),
                gcGen1Collections = GC.CollectionCount(1),
                gcGen2Collections = GC.CollectionCount(2),
                retainedSnapshotGpuObjects = RecoveryRenderSnapshot.BatchModeRetainedGpuObjectCount,
                retainedControlledRendererObjects = RecoveryRenderSnapshotTests.BatchModeRetainedSceneObjectCount,
                retainedControlledRendererMaterials = RecoveryRenderSnapshotTests.BatchModeRetainedMaterialCount,
                interpretation = "Linux headless/reference-renderer regression sample after HEALTH_SWEEP; not a Quest device budget."
            };
        }

        public static RecoveryPerformanceArtifactPaths WriteArtifacts(
            RecoveryPerformanceSampleRecord sample,
            string stem)
        {
            if (sample == null) throw new ArgumentNullException(nameof(sample));
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                ArtifactDirectoryName));
            Directory.CreateDirectory(directory);
            string safeStem = SanitizeStem(stem);
            string jsonPath = Path.Combine(directory, safeStem + ".json");
            string markdownPath = Path.Combine(directory, safeStem + ".md");
            File.WriteAllText(jsonPath, JsonUtility.ToJson(sample, true) + Environment.NewLine,
                Encoding.UTF8);
            File.WriteAllText(markdownPath, BuildMarkdown(sample), Encoding.UTF8);
            Debug.Log("ZIPTIDE: RECOVERY_PERF_SAMPLE label=" + sample.label
                + " scene=" + sample.scene
                + " frames=" + sample.sampledFrames
                + " avgMs=" + sample.averageFrameMs.ToString("F3")
                + " p95Ms=" + sample.p95FrameMs.ToString("F3")
                + " memMB=" + sample.totalAllocatedMB);
            return new RecoveryPerformanceArtifactPaths(jsonPath, markdownPath);
        }

        internal static float Percentile(IList<float> sortedValues, float percentile)
        {
            if (sortedValues == null || sortedValues.Count == 0)
                throw new ArgumentException("Sorted values are required.", nameof(sortedValues));
            float clamped = Mathf.Clamp01(percentile);
            int index = Mathf.CeilToInt(clamped * sortedValues.Count) - 1;
            index = Mathf.Clamp(index, 0, sortedValues.Count - 1);
            return sortedValues[index];
        }

        private static long ToMB(long bytes)
        {
            return bytes / (1024L * 1024L);
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "performance-sample";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char value = stem[i];
                builder.Append(char.IsLetterOrDigit(value) || value == '-' || value == '_'
                    ? value
                    : '_');
            }
            return builder.ToString();
        }

        private static string BuildMarkdown(RecoveryPerformanceSampleRecord sample)
        {
            var text = new StringBuilder(1024);
            text.AppendLine("# Recovery Performance Sample — " + sample.label);
            text.AppendLine();
            text.AppendLine("- Captured: `" + sample.capturedAtUtc + "`");
            text.AppendLine("- Scene: `" + sample.scene + "`");
            text.AppendLine("- Evidence: `" + sample.healthSweepEvidence.Replace("`", "'") + "`");
            text.AppendLine("- Interpretation: " + sample.interpretation);
            text.AppendLine();
            text.AppendLine("## Frame sample");
            text.AppendLine();
            text.AppendLine("| Frames | Average ms | Median ms | P95 ms | Maximum ms |");
            text.AppendLine("|---:|---:|---:|---:|---:|");
            text.AppendLine("| " + sample.sampledFrames + " | " + sample.averageFrameMs.ToString("F3")
                + " | " + sample.medianFrameMs.ToString("F3")
                + " | " + sample.p95FrameMs.ToString("F3")
                + " | " + sample.maximumFrameMs.ToString("F3") + " |");
            text.AppendLine();
            text.AppendLine("## Runtime resources and memory");
            text.AppendLine();
            text.AppendLine("| Materials | Textures | Meshes | Clips | Active renderers | Active audio | Allocated MB | Reserved MB | Mono used MB | Mono heap MB |");
            text.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
            text.AppendLine("| " + sample.materials + " | " + sample.textures + " | " + sample.meshes
                + " | " + sample.audioClips + " | " + sample.activeRenderers + " | "
                + sample.activeAudioSources + " | " + sample.totalAllocatedMB + " | "
                + sample.totalReservedMB + " | " + sample.monoUsedMB + " | "
                + sample.monoHeapMB + " |");
            text.AppendLine();
            text.AppendLine("## Explicit test-process retention");
            text.AppendLine();
            text.AppendLine("- Snapshot GPU objects: " + sample.retainedSnapshotGpuObjects);
            text.AppendLine("- Controlled renderer objects: " + sample.retainedControlledRendererObjects);
            text.AppendLine("- Controlled renderer materials: " + sample.retainedControlledRendererMaterials);
            text.AppendLine("- GC collections: gen0=" + sample.gcGen0Collections + ", gen1="
                + sample.gcGen1Collections + ", gen2=" + sample.gcGen2Collections);
            return text.ToString();
        }
    }
}
