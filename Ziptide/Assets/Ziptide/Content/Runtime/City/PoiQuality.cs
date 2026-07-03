using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Pure heuristics behind the POI quality gates (Quality Bar P1c/P1f) — shared by the audit rules
    /// and the EditMode tests so the bar is testable without a scene. Numbers are deliberately simple
    /// and documented in docs/WORLD_RECIPE.md: a future authoring session tunes WORLDS to pass, never
    /// this math.
    /// </summary>
    public static class PoiQuality
    {
        /// <summary>Comfortable VR walk speed used for route time (m/min).</summary>
        public const float WalkMetersPerMinute = 140f;

        /// <summary>How many distinct verbs (PoiType values) the set covers.</summary>
        public static int DistinctVerbCount(List<PoiDef> pois)
        {
            if (pois == null) return 0;
            var seen = new HashSet<PoiType>();
            foreach (var p in pois)
                if (p != null) seen.Add(p.type);
            return seen.Count;
        }

        /// <summary>Minutes of engagement a single POI is worth, by verb and tier.</summary>
        public static float PoiMinutes(PoiDef p)
        {
            if (p == null) return 0f;
            float baseMin;
            switch (p.type)
            {
                case PoiType.CombatCamp: baseMin = 2.5f; break;
                case PoiType.HarvestGrove: baseMin = 2.0f; break;
                case PoiType.MachineSite: baseMin = 2.0f; break;
                case PoiType.StoryAnchor: baseMin = 2.0f; break;
                case PoiType.CaveSecret: baseMin = 1.5f; break;
                case PoiType.RuinCache: baseMin = 1.5f; break;
                default: baseMin = 0.5f; break; // TravelBerth is a doorway, not gameplay
            }
            return baseMin * (1f + 0.25f * Mathf.Clamp(p.tier, 0, 2));
        }

        /// <summary>
        /// Estimated play minutes: per-POI engagement + walking a nearest-neighbor route from spawn
        /// through every POI. The EST_PLAY_MINUTES_LOW gate fails a world under 8.
        /// </summary>
        public static float EstimatePlayMinutes(List<PoiDef> pois, Vector3 spawn)
        {
            if (pois == null || pois.Count == 0) return 0f;

            float verbs = 0f;
            var remaining = new List<PoiDef>();
            foreach (var p in pois)
                if (p != null) { verbs += PoiMinutes(p); remaining.Add(p); }

            float route = 0f;
            var at = new Vector2(spawn.x, spawn.z);
            while (remaining.Count > 0)
            {
                int best = 0;
                float bestDist = float.MaxValue;
                for (int i = 0; i < remaining.Count; i++)
                {
                    var q = new Vector2(remaining[i].position.x, remaining[i].position.z);
                    float d = Vector2.Distance(at, q);
                    if (d < bestDist) { bestDist = d; best = i; }
                }
                route += bestDist;
                at = new Vector2(remaining[best].position.x, remaining[best].position.z);
                remaining.RemoveAt(best);
            }

            return verbs + route / WalkMetersPerMinute;
        }

        /// <summary>True if the set contains at least one StoryAnchor (the world's beat, staged).</summary>
        public static bool HasStoryAnchor(List<PoiDef> pois)
        {
            if (pois == null) return false;
            foreach (var p in pois)
                if (p != null && p.type == PoiType.StoryAnchor) return true;
            return false;
        }
    }
}
