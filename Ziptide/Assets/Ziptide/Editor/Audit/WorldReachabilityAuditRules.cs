#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// WORLDS_50 #13 (Additions Bank pull) — POI REACHABILITY, WARN-only. Complements the
    /// TERRAIN_SLOPE_UNWALKABLE blocker (which measures walkable AREA): a world can be mostly walkable
    /// yet strand a POI behind a ravine. This samples the built terrain into a coarse walkable+height
    /// grid, floods from the player spawn, and WARNs for any POI on a disconnected island.
    ///
    /// TRAVERSAL-AWARE since 1.4f (the hardwiring board's "extend with zip edges" ask): the flood now
    /// runs through the pure <see cref="MultiLevelReachability"/> graph — same per-cell step rule as
    /// <see cref="GridReachability"/>, plus a ONE-WAY edge for every <see cref="ZiplineRuntime"/> in the
    /// scene (start→end: you ride down, you don't ride back up). A POI whose island is fed by a zipline
    /// no longer false-warns; a POI you can only LEAVE by zipline still warns, correctly.
    ///
    /// DELIBERATELY A WARNING, NOT A BLOCKER: the grid is a coarse raycast sample, so a narrow corridor
    /// could be missed and produce a false "unreachable" — a warning surfaces that for a human to check
    /// without ever failing a good build. If it proves reliable on device, promote to a blocker later.
    /// Only runs for experience-enabled worlds that actually built terrain.
    /// </summary>
    public static class WorldReachabilityAuditRules
    {
        private const float TargetCellSize = 6f;   // meters; ~corridor width, keeps false-warns low
        private const int MaxGrid = 128;           // bound the raycast count per world
        private const float StepTolerance = 4f;    // cliff > this breaks connectivity; gentle grade doesn't

        public static void Run(SceneAuditReport report)
        {
            var kit = FindLayoutForScene(report.sceneName);
            var ex = kit != null ? kit.experience : null;
            if (ex == null || !ex.enabled) return;
            if (kit.pois == null || kit.pois.Count == 0) return;

            // Skip if terrain wasn't built (the TERRAIN_MISSING blocker already covers that case).
            if (GameObject.Find("ExperienceTerrain") == null) return;

            var spawnGo = GameObject.Find(ZiptideConstants.GoSpawnPlayer);
            if (spawnGo == null) return; // spawn checks live elsewhere; nothing to flood from

            float r = Mathf.Max(60f, ex.worldRadius);
            int n = Mathf.Clamp(Mathf.CeilToInt((2f * r) / TargetCellSize), 8, MaxGrid);
            float cell = (2f * r) / n;

            var walkable = new bool[n * n];
            var heights = new float[n * n];
            for (int gy = 0; gy < n; gy++)
            {
                for (int gx = 0; gx < n; gx++)
                {
                    float wx = -r + (gx + 0.5f) * cell;
                    float wz = -r + (gy + 0.5f) * cell;
                    var origin = new Vector3(wx, 300f, wz);
                    if (Physics.Raycast(origin, Vector3.down, out var hit, 600f, ~0, QueryTriggerInteraction.Ignore))
                    {
                        walkable[gy * n + gx] = true;
                        heights[gy * n + gx] = hit.point.y;
                    }
                }
            }

            int sx = CellOf(spawnGo.transform.position.x, r, cell, n);
            int sy = CellOf(spawnGo.transform.position.z, r, cell, n);

            // The traversal-aware flood: one grid layer (same step rule as GridReachability) plus a
            // one-way edge per zipline. With zero ziplines this behaves identically to the old flood.
            var graph = new MultiLevelReachability(n, n, 1);
            graph.SetLayer(0, walkable, heights, StepTolerance);
            int zipEdges = 0;
            foreach (var zip in Object.FindObjectsOfType<ZiplineRuntime>())
            {
                if (zip == null) continue;
                int ax = CellOf(zip.startAnchor.x, r, cell, n), ay = CellOf(zip.startAnchor.z, r, cell, n);
                int bx = CellOf(zip.endAnchor.x, r, cell, n), by = CellOf(zip.endAnchor.z, r, cell, n);
                graph.AddEdge(new TraversalEdge(graph.NodeId(0, ax, ay), graph.NodeId(0, bx, by),
                    TraversalKind.Zipline, bidirectional: false)); // you ride DOWN — not back up
                zipEdges++;
            }

            var reached = graph.Flood(graph.NodeId(0, sx, sy));
            if (MultiLevelReachability.Count(reached) == 0) return; // spawn cell had no ground sample — bail quietly

            foreach (var poi in kit.pois)
            {
                if (poi == null || string.IsNullOrEmpty(poi.id)) continue;
                var marker = GameObject.Find("__SPAWN_poi_" + poi.id);
                Vector3 p = marker != null ? marker.transform.position : poi.position;
                int px = CellOf(p.x, r, cell, n);
                int py = CellOf(p.z, r, cell, n);
                if (!reached[graph.NodeId(0, px, py)])
                    report.Warning("POI_MAYBE_UNREACHABLE",
                        "POI '" + poi.id + "' at (" + p.x.ToString("F0") + "," + p.z.ToString("F0") +
                        ") appears disconnected from the player spawn by the reachability sample " +
                        "(cell " + cell.ToString("F1") + "m, step " + StepTolerance + "m, ziplines counted: " +
                        zipEdges + "). Walk it in-headset; if reachable, this is a coarse-grid false " +
                        "positive — otherwise add a graded corridor (or a zipline).");
            }
        }

        private static int CellOf(float world, float r, float cell, int n)
            => Mathf.Clamp(Mathf.FloorToInt((world + r) / cell), 0, n - 1);

        private static CityLayoutDefinition FindLayoutForScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return null;
            foreach (var guid in AssetDatabase.FindAssets("t:CityLayoutDefinition"))
            {
                var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (kit != null && kit.sceneName == sceneName) return kit;
            }
            return null;
        }
    }
}
#endif
