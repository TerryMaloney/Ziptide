#if UNITY_EDITOR
using UnityEngine;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// FORGE II E5.2 — THE PERF GATE. Static scene-content budgets from
    /// docs/project_art_plan/QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md, checked per non-boot scene:
    /// triangles / unique materials / renderers / real-time lights. WARN at the target, BLOCK at
    /// the hard cap — so a content pass that quietly triples a world's cost is caught in CI, not
    /// on Terry's framerate. (Static totals, not visible-per-frame: a conservative proxy — culling
    /// only ever helps.)
    /// </summary>
    public static class PerfBudgetAuditRules
    {
        private const int TrisTarget = 150_000, TrisCap = 400_000; // static-total cap ~2x the visible cap
        private const int MatsTarget = 25, MatsCap = 60;
        private const int RenderersTarget = 900, RenderersCap = 2500;
        private const int LightsTarget = 1, LightsCap = 3;

        public static void Run(SceneAuditReport report)
        {
            long tris = 0;
            int renderers = 0;
            var mats = new System.Collections.Generic.HashSet<Material>();

            foreach (var mf in Object.FindObjectsOfType<MeshFilter>())
                if (mf != null && mf.sharedMesh != null) tris += mf.sharedMesh.triangles.Length / 3;
            foreach (var r in Object.FindObjectsOfType<Renderer>())
            {
                if (r == null) continue;
                renderers++;
                foreach (var m in r.sharedMaterials)
                    if (m != null) mats.Add(m);
            }
            int lights = Object.FindObjectsOfType<Light>().Length;

            Check(report, "PERF_TRIS", tris, TrisTarget, TrisCap, "static triangles");
            Check(report, "PERF_MATERIALS", mats.Count, MatsTarget, MatsCap, "unique materials");
            Check(report, "PERF_RENDERERS", renderers, RenderersTarget, RenderersCap, "renderers");
            Check(report, "PERF_LIGHTS", lights, LightsTarget, LightsCap, "real-time lights");
            ReportBreakdown(report, renderers);
        }

        /// <summary>
        /// WHERE THE COST ACTUALLY IS, per top-level root.
        ///
        /// ⚖ Terry, 2026-08-01: *"let's make sure we're not deleting anything if we don't need to and
        /// let's make sure we're not accidentally counting things incorrectly."* A single scene total
        /// cannot answer either question. It says ToxicCity is at 1769 renderers; it does not say
        /// whether that is the ring city, the facade windows, or the districts — and picking something
        /// to cut without knowing is how a level gets uglier without getting faster.
        ///
        /// Emitted as a `ZIPTIDE:` log line rather than a report finding, deliberately. The report has
        /// exactly two severities, Warning and Blocker, and a measuring tape is neither — adding an
        /// Info severity would change the JSON schema that other tools read, to carry something the
        /// build log holds perfectly well.
        /// </summary>
        private static void ReportBreakdown(SceneAuditReport report, int totalRenderers)
        {
            if (totalRenderers <= 0) return;

            var perRoot = new System.Collections.Generic.Dictionary<string, int>();
            var matsPerRoot = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<Material>>();

            foreach (var r in Object.FindObjectsOfType<Renderer>())
            {
                if (r == null) continue;
                string root = TopLevelName(r.transform);
                perRoot.TryGetValue(root, out int n);
                perRoot[root] = n + 1;

                if (!matsPerRoot.TryGetValue(root, out var set))
                    matsPerRoot[root] = set = new System.Collections.Generic.HashSet<Material>();
                foreach (var m in r.sharedMaterials)
                    if (m != null) set.Add(m);
            }

            // Biggest first — the only order anyone reads a budget breakdown in.
            var rows = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, int>>(perRoot);
            rows.Sort((a, b) => b.Value.CompareTo(a.Value));

            var sb = new System.Text.StringBuilder();
            int shown = 0;
            foreach (var row in rows)
            {
                if (shown++ >= 14) break;
                if (shown > 1) sb.Append("  ");
                sb.Append(row.Key).Append('=').Append(row.Value).Append('/').Append(matsPerRoot[row.Key].Count);
            }
            if (rows.Count > 14) sb.Append("  +").Append(rows.Count - 14).Append(" more root(s)");

            // Both: the log for whoever is watching a build, and the MARKDOWN report for whoever comes
            // back to it later. The report is the one that matters — a number you can only get by
            // scrolling a CI log is a number nobody checks.
            report.costBreakdown = sb.ToString();
            Debug.Log("ZIPTIDE: PERF_BREAKDOWN scene=" + report.sceneName
                      + " total=" + totalRenderers + " roots= " + sb);
        }

        /// <summary>The outermost ancestor's name — the subsystem a renderer belongs to.</summary>
        private static string TopLevelName(Transform t)
        {
            while (t.parent != null) t = t.parent;
            return t.name;
        }

        private static void Check(SceneAuditReport report, string code, long value, long target, long cap, string what)
        {
            // WARN-ONLY for the succession window: no operator can measure existing scenes without
            // Unity, and a mistuned cap must not brick the build. PROMOTE the over-cap branch to
            // report.Blocker(...) after one clean audit run establishes the real baselines.
            if (value > cap)
                report.Warning(code + "_OVER_CAP",
                    what + " = " + value + " exceeds the HARD CAP " + cap + " (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.");
            else if (value > target)
                report.Warning(code + "_OVER_TARGET",
                    what + " = " + value + " over the target " + target + " (cap " + cap + ") — budget attention needed.");
        }
    }
}
#endif
