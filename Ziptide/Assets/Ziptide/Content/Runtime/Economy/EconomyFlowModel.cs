using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>Where one resource comes from and goes to — computed, never stored (Meta-Loop law).</summary>
    public class ResourceFlow
    {
        public readonly HashSet<string> Sources = new HashSet<string>(); // "job:w002_pumps", "plant:dew_bulb"…
        public readonly HashSet<string> Sinks = new HashSet<string>();   // "recipe:stun_charge_cell", "socket:…"
    }

    /// <summary>
    /// META-LOOP flow analysis (pure): callers feed FACTS ("resource R is produced/consumed by X")
    /// gathered from jobs, plants, mines, recipes, sockets and conquest income; the model answers
    /// Terry's questions — sources, sinks, orphans (no source), dead ends (no sink), unused ids, and
    /// what a world/story change touches. The audit surfaces these as RESOURCE_NO_SOURCE /
    /// RESOURCE_NO_SINK / RESOURCE_UNUSED warnings; the economy flow report prints the full table.
    /// </summary>
    public class EconomyFlowModel
    {
        public readonly Dictionary<string, ResourceFlow> Flows = new Dictionary<string, ResourceFlow>();

        public void AddSource(string resourceId, string via) => Get(resourceId).Sources.Add(via);
        public void AddSink(string resourceId, string via) => Get(resourceId).Sinks.Add(via);

        private ResourceFlow Get(string id)
        {
            if (!Flows.TryGetValue(id, out var f)) { f = new ResourceFlow(); Flows[id] = f; }
            return f;
        }

        /// <summary>Registered ids nothing produces. "credits"-like seed currencies can be exempted.</summary>
        public List<string> NoSource(IEnumerable<string> registeredIds, HashSet<string> exempt = null)
        {
            var outIds = new List<string>();
            foreach (var id in registeredIds)
                if ((exempt == null || !exempt.Contains(id)) &&
                    (!Flows.TryGetValue(id, out var f) || f.Sources.Count == 0))
                    outIds.Add(id);
            return outIds;
        }

        /// <summary>Registered ids nothing consumes — usually a design bug in waiting.</summary>
        public List<string> NoSink(IEnumerable<string> registeredIds, HashSet<string> exempt = null)
        {
            var outIds = new List<string>();
            foreach (var id in registeredIds)
                if ((exempt == null || !exempt.Contains(id)) &&
                    (!Flows.TryGetValue(id, out var f) || f.Sinks.Count == 0))
                    outIds.Add(id);
            return outIds;
        }

        /// <summary>Registered ids with neither source nor sink.</summary>
        public List<string> Unused(IEnumerable<string> registeredIds)
        {
            var outIds = new List<string>();
            foreach (var id in registeredIds)
                if (!Flows.TryGetValue(id, out var f) || (f.Sources.Count == 0 && f.Sinks.Count == 0))
                    outIds.Add(id);
            return outIds;
        }

        /// <summary>Every fact (source or sink label) whose via-string mentions the changed token —
        /// "what does changing W003 / the toxic arc touch?" Token match is by substring on the via
        /// labels plus the definitions' declared sourceWorlds/storyTags (caller passes those in
        /// as facts too, prefixed "tag:").</summary>
        public Dictionary<string, List<string>> AffectedBy(string token)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var kv in Flows)
            {
                var hits = new List<string>();
                foreach (var s in kv.Value.Sources) if (s.Contains(token)) hits.Add(s);
                foreach (var s in kv.Value.Sinks) if (s.Contains(token)) hits.Add(s);
                if (hits.Count > 0) result[kv.Key] = hits;
            }
            return result;
        }
    }
}
