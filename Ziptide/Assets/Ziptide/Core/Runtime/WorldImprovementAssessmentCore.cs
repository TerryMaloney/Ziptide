using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>One module's measurable evidence for one declared world-quality aspect.</summary>
    public readonly struct WorldAspectEvidence
    {
        public readonly string Aspect;
        public readonly int ModuleCount;
        public readonly int ObjectCount;

        public WorldAspectEvidence(string aspect, int moduleCount, int objectCount)
        {
            Aspect = aspect ?? string.Empty;
            ModuleCount = Math.Max(0, moduleCount);
            ObjectCount = Math.Max(0, objectCount);
        }
    }

    /// <summary>Deterministic evidence score. This measures coverage depth, not subjective artistic quality.</summary>
    public readonly struct WorldAspectAssessment
    {
        public readonly string Aspect;
        public readonly int ModuleCount;
        public readonly int ObjectCount;
        public readonly int Score;

        public WorldAspectAssessment(string aspect, int moduleCount, int objectCount, int score)
        {
            Aspect = aspect ?? string.Empty;
            ModuleCount = Math.Max(0, moduleCount);
            ObjectCount = Math.Max(0, objectCount);
            Score = Math.Max(0, Math.Min(100, score));
        }
    }

    /// <summary>
    /// Portable weakest-dimension selector for improvement rounds. Missing aspects score zero; additional
    /// independent modules and owned output increase evidence depth. Results are always weakest-first with
    /// ordinal aspect-name tie breaking, so identical recipes produce identical next-round priorities.
    /// </summary>
    public static class WorldImprovementAssessmentCore
    {
        public static WorldAspectAssessment[] Assess(
            IReadOnlyList<string> requiredAspects,
            IReadOnlyList<WorldAspectEvidence> evidence)
        {
            var names = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (requiredAspects != null)
            {
                for (int i = 0; i < requiredAspects.Count; i++)
                {
                    string value = (requiredAspects[i] ?? string.Empty).Trim();
                    if (value.Length > 0 && seen.Add(value)) names.Add(value);
                }
            }

            var results = new List<WorldAspectAssessment>(names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                string aspect = names[i];
                int modules = 0;
                int objects = 0;
                if (evidence != null)
                {
                    for (int e = 0; e < evidence.Count; e++)
                    {
                        WorldAspectEvidence item = evidence[e];
                        if (!string.Equals(item.Aspect, aspect, StringComparison.OrdinalIgnoreCase)) continue;
                        modules += item.ModuleCount;
                        objects += item.ObjectCount;
                    }
                }

                int score = 0;
                if (modules > 0)
                {
                    // One independent module establishes the aspect; additional modules prove depth rather
                    // than merely increasing primitive count. Object contribution is intentionally capped.
                    score = 45 + Math.Min(3, modules) * 15 + Math.Min(20, objects);
                    score = Math.Min(100, score);
                }
                results.Add(new WorldAspectAssessment(aspect, modules, objects, score));
            }

            results.Sort((a, b) =>
            {
                int score = a.Score.CompareTo(b.Score);
                return score != 0 ? score : string.Compare(a.Aspect, b.Aspect, StringComparison.Ordinal);
            });
            return results.ToArray();
        }

        public static string[] Weakest(WorldAspectAssessment[] assessments, int maximum)
        {
            if (assessments == null || maximum <= 0) return Array.Empty<string>();
            int count = Math.Min(maximum, assessments.Length);
            var output = new string[count];
            for (int i = 0; i < count; i++) output[i] = assessments[i].Aspect;
            return output;
        }
    }
}
