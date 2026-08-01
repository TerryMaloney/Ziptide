using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// THE CITY'S COMPASS — the pure half of the wayfinding pass (LEVEL1_SPATIAL_SCRIPT §3).
    ///
    /// Toxic City has no map, no HUD arrow and no minimap, on purpose. It navigates by two devices,
    /// and both are geometry:
    ///
    /// 1. **THE LANTERN ROUTE.** Amber lanterns hang over the contract's walk and nowhere else, so
    ///    "the job goes this way" is something the street tells you. Their ABSENCE is the other
    ///    half of the message: unlit street means you are exploring, which is allowed and is
    ///    supposed to feel different.
    /// 2. **THE SIGHTLINE TRIPLE.** From the dispatch plaza, three landmarks are visible at once —
    ///    the relay mast's fault strobe, the north tower, and your own ship's floodlights. Knowing
    ///    which way is home is then a matter of looking up.
    ///
    /// Both devices have a failure mode that a screenshot will not show and a playtest finds the
    /// expensive way, so both are laws here rather than intentions:
    /// - a lantern leg that does not follow an authored connection hangs lamps through a wall;
    /// - three landmarks bunched inside a narrow arc are not a compass, they are a smear.
    /// </summary>
    public static class WayfindingCore
    {
        /// <summary>Two landmarks closer together than this cannot be told apart at a glance, so
        /// the triple stops being a compass. Pinned by CityWayfindingTests against the real spec.</summary>
        public const float MinLandmarkSeparationDegrees = 45f;

        /// <summary>Metres between hanging lanterns. Close enough that the next one is always in
        /// view down a street, far enough that the route reads as marked rather than as lit.</summary>
        public const float LanternSpacing = 12f;

        /// <summary>
        /// Lantern positions along a route polyline: one at every node (corners must be marked —
        /// a turn you cannot see is where a player gets lost) plus evenly spaced lamps between.
        /// Returns positions in the route's own space; the author lifts them to hanging height.
        /// </summary>
        public static List<Vector3> LanternPositions(IList<Vector3> route, float spacing)
        {
            var result = new List<Vector3>();
            if (route == null || route.Count == 0) return result;
            if (spacing <= 0.01f) spacing = LanternSpacing;

            result.Add(route[0]);
            for (int i = 0; i < route.Count - 1; i++)
            {
                Vector3 a = route[i], b = route[i + 1];
                float length = Vector3.Distance(a, b);
                int steps = Mathf.FloorToInt(length / spacing);
                for (int s = 1; s <= steps; s++)
                {
                    float t = (s * spacing) / length;
                    if (t >= 0.999f) break;   // the node itself is added below, never twice
                    result.Add(Vector3.Lerp(a, b, t));
                }
                result.Add(b);
            }
            return result;
        }

        /// <summary>Two lanterns closer than this are one lantern with z-fighting.</summary>
        public const float MergeEpsilon = 0.5f;

        /// <summary>
        /// Combine two lit walks into one set of lantern positions, dropping any that would land on
        /// top of another. Needed the moment the city has more than one lit walk: the arrival walk
        /// ends where the contract's loop begins, and both want a lamp on that corner.
        ///
        /// It de-duplicates WITHIN each list too, which fixes something that was already wrong: the
        /// job route is a loop, so it names Dispatch twice and has been building two coincident
        /// lanterns there since the compass shipped. Coincident transparent globes z-fight, and
        /// z-fighting is the class of bug that is invisible on a monitor and obvious in a headset.
        /// </summary>
        public static List<Vector3> MergeLanterns(IList<Vector3> placed, IList<Vector3> extra)
        {
            var result = new List<Vector3>();
            float sqrEps = MergeEpsilon * MergeEpsilon;

            for (int pass = 0; pass < 2; pass++)
            {
                IList<Vector3> src = pass == 0 ? placed : extra;
                if (src == null) continue;
                for (int i = 0; i < src.Count; i++)
                {
                    bool duplicate = false;
                    for (int j = 0; j < result.Count && !duplicate; j++)
                        duplicate = (result[j] - src[i]).sqrMagnitude < sqrEps;
                    if (!duplicate) result.Add(src[i]);
                }
            }
            return result;
        }

        /// <summary>Compass bearing from one point to another: 0° = +Z, increasing clockwise, so
        /// 90° is east and 180° is south. Y is ignored — this is a floor plan, not a slope.</summary>
        public static float BearingDegrees(Vector3 from, Vector3 to)
        {
            Vector3 d = to - from;
            if (Mathf.Abs(d.x) < 1e-5f && Mathf.Abs(d.z) < 1e-5f) return 0f;
            return Mathf.Repeat(Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg, 360f);
        }

        /// <summary>The smallest angle between any two of these bearings. A triple whose minimum
        /// falls under <see cref="MinLandmarkSeparationDegrees"/> has stopped working.</summary>
        public static float MinPairwiseSeparation(IList<float> bearings)
        {
            if (bearings == null || bearings.Count < 2) return 360f;
            float min = 360f;
            for (int i = 0; i < bearings.Count; i++)
                for (int j = i + 1; j < bearings.Count; j++)
                    min = Mathf.Min(min, Mathf.Abs(Mathf.DeltaAngle(bearings[i], bearings[j])));
            return min;
        }

        /// <summary>
        /// Does every leg of this route follow a street somebody authored? Connections are
        /// undirected — a street links two districts, it does not point. A false here means the
        /// lantern route would cut across geometry the city builder never made walkable.
        /// </summary>
        public static bool EveryLegIsConnected(IList<string> routeDistrictIds,
            IList<(string From, string To)> connections)
        {
            if (routeDistrictIds == null || routeDistrictIds.Count < 2) return false;
            if (connections == null) return false;

            for (int i = 0; i < routeDistrictIds.Count - 1; i++)
            {
                string a = routeDistrictIds[i], b = routeDistrictIds[i + 1];
                bool found = false;
                for (int c = 0; c < connections.Count && !found; c++)
                {
                    var conn = connections[c];
                    found = (conn.From == a && conn.To == b) || (conn.From == b && conn.To == a);
                }
                if (!found) return false;
            }
            return true;
        }
    }
}
