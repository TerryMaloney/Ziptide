#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using Ziptide.Content;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Project-wide WorldPack integrity — the build-time half of <see cref="WorldPackValidator"/>.
    ///
    /// The validator itself is not new and is not unused: <c>JobDirector</c> calls it at world entry.
    /// But that is a RUNTIME check, on device, after the player has already travelled there — which is
    /// how ToxicCity shipped a contract whose step 4 asked to repair a machine the pack never spawned
    /// (MISS_LEDGER #21). Nothing checked packs at build time, and nothing checked ALL of them.
    ///
    /// Packs are generated assets, so this only sees them once the patchers have run. That is exactly
    /// the order <c>BuildAndroid.PatchScenesThenAPK</c> uses: patch every scene, then audit.
    ///
    /// WARN-only for now, per the PerfBudgetAuditRules ratchet: a new blocker in the audit aborts
    /// Terry's local build, so it earns promotion after one clean run rather than on the way in.
    /// </summary>
    public static class WorldPackAuditRules
    {
        public const string InvalidCode = "WORLD_PACK_INVALID";
        public const string NoneFoundCode = "WORLD_PACK_NONE_FOUND";

        public const string PackFolder = "Assets/Ziptide/Content/Worlds/Packs";

        /// <summary>How many issues to spell out per pack before the message is truncated.</summary>
        private const int MaxIssuesPerPack = 8;

        public static void Run(SceneAuditReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var packs = new List<WorldPackDefinition>();
            var paths = new List<string>();
            foreach (string guid in AssetDatabase.FindAssets("t:WorldPackDefinition"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
                if (pack == null) continue;
                packs.Add(pack);
                paths.Add(path);
            }

            Run(report, packs, paths);
        }

        /// <summary>Dependency seam for deterministic EditMode tests (no AssetDatabase).</summary>
        public static void Run(
            SceneAuditReport report,
            IList<WorldPackDefinition> packs,
            IList<string> paths = null)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            if (packs == null || packs.Count == 0)
            {
                // Not a blocker: a tree that has never been baked legitimately has no pack assets.
                // Saying so is still worth a line, because "the audit found nothing" and "the audit
                // checked nothing" look identical in a green report otherwise.
                report.Warning(
                    NoneFoundCode,
                    "No WorldPackDefinition assets found. Packs are generated, so this means the scene "
                    + "patchers have not run in this session — pack integrity was NOT checked.",
                    PackFolder);
                return;
            }

            for (int i = 0; i < packs.Count; i++)
            {
                WorldPackDefinition pack = packs[i];
                string path = paths != null && i < paths.Count ? paths[i] : PackFolder;

                List<string> issues;
                try { issues = WorldPackValidator.Validate(pack); }
                catch (Exception ex)
                {
                    report.Warning(InvalidCode,
                        "WorldPackValidator threw on this pack: " + ex.Message, path);
                    continue;
                }

                if (issues == null || issues.Count == 0) continue;

                string id = pack != null && !string.IsNullOrEmpty(pack.packId) ? pack.packId : "<no packId>";
                report.Warning(InvalidCode, Describe(id, issues), path);
            }
        }

        private static string Describe(string packId, List<string> issues)
        {
            var text = new System.Text.StringBuilder();
            text.Append("Pack '").Append(packId).Append("' has ").Append(issues.Count)
                .Append(" integrity issue(s) — an un-completable step here locks the world's "
                    + "contract and every flag behind it: ");
            int shown = Math.Min(issues.Count, MaxIssuesPerPack);
            for (int i = 0; i < shown; i++)
            {
                if (i > 0) text.Append(" · ");
                text.Append(issues[i]);
            }
            if (issues.Count > shown) text.Append(" · (+").Append(issues.Count - shown).Append(" more)");
            return text.ToString();
        }
    }
}
#endif
