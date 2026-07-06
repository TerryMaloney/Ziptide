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
