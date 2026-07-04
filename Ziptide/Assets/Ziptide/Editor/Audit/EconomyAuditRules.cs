#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// META-LOOP gates (project-wide, once per audit like ForgeAuditRules):
    ///  - RESOURCE_ID_UNREGISTERED (blocker): any resource id used by jobs/mines/plants/recipes/
    ///    creatures that has no ResourceDefinition in Resources/Economy. The one-economy law.
    ///  - RESOURCE_NO_SOURCE / RESOURCE_NO_SINK / RESOURCE_UNUSED (warnings): computed from the
    ///    pure EconomyFlowModel — a resource nothing produces or nothing consumes is a design bug
    ///    in waiting.
    /// Also writes docs/_generated/ECONOMY_FLOW_REPORT.md — Terry's readable answer to
    /// "where does everything come from and go?", including affected-by-world staleness lookups.
    /// </summary>
    public static class EconomyAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            var registered = new HashSet<string>();
            foreach (var def in Resources.LoadAll<ResourceDefinition>("Economy"))
                if (def != null && !string.IsNullOrEmpty(def.id)) registered.Add(def.id);

            var flow = new EconomyFlowModel();
            var used = new HashSet<string>();

            void Use(string id, bool isSource, string via)
            {
                if (string.IsNullOrEmpty(id)) return;
                used.Add(id);
                if (isSource) flow.AddSource(id, via); else flow.AddSink(id, via);
            }

            foreach (var guid in AssetDatabase.FindAssets("t:WorldPackDefinition"))
            {
                var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (pack == null) continue;
                string w = pack.sceneName;
                foreach (var job in pack.jobs)
                    if (job != null && job.reward != null)
                        foreach (var r in job.reward)
                            if (r != null) Use(r.resourceId, true, "job:" + job.jobId + "@" + w);
                foreach (var m in pack.mines)
                    if (m != null) Use(m.resourceId, true, "mine:" + m.id + "@" + w);
                foreach (var s in pack.sockets)
                    if (s != null)
                    {
                        Use(s.resourceId, true, "socket:" + s.id + "@" + w);
                        Use("credits", false, "socket:" + s.id + "@" + w);
                    }
            }
            foreach (var plant in Resources.LoadAll<PlantDefinition>("Garden"))
                if (plant != null && plant.harvestYield != null)
                    foreach (var y in plant.harvestYield)
                        if (y != null)
                            Use(y.resourceId, true, "plant:" + plant.id +
                                (plant.biomeId != "" ? "@" + plant.biomeId : ""));
            foreach (var recipe in Resources.LoadAll<RecipeDefinition>("Recipes"))
            {
                if (recipe == null) continue;
                string tagged = recipe.id + (recipe.sourceWorlds.Count > 0 ? "@" + recipe.sourceWorlds[0] : "");
                foreach (var c in recipe.costs)
                    if (c != null) Use(c.resourceId, false, "recipe:" + tagged);
                Use(recipe.producesId, true, "recipe:" + tagged);
            }
            foreach (var creature in Resources.LoadAll<CreatureDefinition>("Enemies"))
                if (creature != null && creature.loot != null)
                    foreach (var l in creature.loot)
                        if (l != null) Use(l.resourceId, true, "creature:" + creature.id);

            foreach (var id in used)
                if (!registered.Contains(id))
                    report.Blocker("RESOURCE_ID_UNREGISTERED",
                        "Resource id '" + id + "' is used by content but has no ResourceDefinition in " +
                        "Resources/Economy. Add it via EconomyAuthor (the one-economy law).");

            var exempt = new HashSet<string> { "credits" };
            foreach (var id in flow.NoSource(registered, exempt))
                report.Warning("RESOURCE_NO_SOURCE", "Nothing produces '" + id + "'.");
            foreach (var id in flow.NoSink(registered, exempt))
                report.Warning("RESOURCE_NO_SINK", "Nothing consumes '" + id + "' — dead-end resource.");
            foreach (var id in flow.Unused(registered))
                report.Warning("RESOURCE_UNUSED", "'" + id + "' is registered but appears nowhere.");

            WriteReport(registered, flow);
        }

        private static void WriteReport(HashSet<string> registered, EconomyFlowModel flow)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string dir = Path.Combine(Path.GetFullPath(Path.Combine(projectRoot, "..")), "docs", "_generated");
                Directory.CreateDirectory(dir);
                var sb = new StringBuilder();
                sb.AppendLine("# ECONOMY FLOW REPORT (generated by EconomyAuditRules — do not hand-edit)");
                sb.AppendLine();
                sb.AppendLine("| resource | sources | sinks |");
                sb.AppendLine("|---|---|---|");
                var ids = new List<string>(registered);
                ids.Sort();
                foreach (var id in ids)
                {
                    flow.Flows.TryGetValue(id, out var f);
                    sb.Append("| ").Append(id).Append(" | ")
                      .Append(f == null ? "—" : string.Join("<br>", f.Sources))
                      .Append(" | ")
                      .Append(f == null ? "—" : string.Join("<br>", f.Sinks))
                      .AppendLine(" |");
                }
                sb.AppendLine();
                sb.AppendLine("Staleness lookups: EconomyFlowModel.AffectedBy(\"W00X\"/storyTag) — " +
                              "see docs/design/ZIPTIDE_META_LOOP.md.");
                File.WriteAllText(Path.Combine(dir, "ECONOMY_FLOW_REPORT.md"), sb.ToString());
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[Ziptide] Economy flow report write failed: " + ex.Message);
            }
        }
    }
}
#endif
