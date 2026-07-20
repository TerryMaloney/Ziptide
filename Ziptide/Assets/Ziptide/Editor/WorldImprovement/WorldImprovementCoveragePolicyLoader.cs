#if UNITY_EDITOR
using System;
using System.IO;
using Ziptide.Content;

namespace Ziptide.Editor.WorldImprovement
{
    public static class WorldImprovementCoveragePolicyLoader
    {
        public static string PolicyPath => Path.Combine(WorldImprovementCompiler.ManifestFolder,
            "_coverage.policy.json");

        public static WorldImprovementCoveragePolicy LoadRequired()
        {
            if (!File.Exists(PolicyPath))
                throw new FileNotFoundException("World improvement coverage policy is missing.", PolicyPath);
            WorldImprovementCoveragePolicy policy =
                WorldImprovementCoveragePolicy.FromJson(File.ReadAllText(PolicyPath));
            var issues = policy.Validate();
            if (issues.Count > 0)
                throw new InvalidOperationException("World improvement coverage policy rejected: "
                    + string.Join(" | ", issues));
            return policy;
        }
    }
}
#endif
