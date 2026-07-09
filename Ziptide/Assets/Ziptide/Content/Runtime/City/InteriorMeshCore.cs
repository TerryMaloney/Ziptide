using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// HARDWIRING 1.3 / BUILDING_INTERIORS Tier B — the pure geometry step between
    /// <see cref="RoomPartitioner"/>'s walkable plan and a builder's cubes: everything in the
    /// footprint that is NOT walkable is wall mass. The footprint is rasterized on a coarse grid,
    /// walkable cells are cleared, and the remaining wall cells are greedy-merged into a SMALL list
    /// of axis-aligned boxes (row runs, then vertical run-merge) — Quest cares about renderer count,
    /// so fewer, fatter boxes beat many voxels. Deterministic; no randomness of its own.
    /// </summary>
    public static class InteriorMeshCore
    {
        /// <summary>Raster cell size (m). Smaller = tighter walls but more boxes; 0.35 matches
        /// RoomPartitioner.WallInset so single-inset walls survive rasterization.</summary>
        public const float CellSize = 0.35f;

        /// <summary>
        /// Connect the building's REAL doorway to the plan: carves an L-corridor from
        /// <paramref name="entry"/> (the doorway's inner face, footprint space) to the nearest room
        /// center, so the exterior door always opens into the walkable network (the access rule
        /// extends to the street). No-op on an empty plan.
        /// </summary>
        public static void WithEntry(InteriorPlan plan, Vector2 entry, float corridorWidth)
        {
            if (plan.IsEmpty) return;
            float half = Mathf.Max(0.4f, corridorWidth * 0.5f);

            Vector2 nearest = plan.Rooms[0].center;
            float best = float.MaxValue;
            for (int i = 0; i < plan.Rooms.Count; i++)
            {
                float d = (plan.Rooms[i].center - entry).sqrMagnitude;
                if (d < best) { best = d; nearest = plan.Rooms[i].center; }
            }

            // L-shape: horizontal leg then vertical (RoomPartitioner's carve recipe).
            if (Mathf.Abs(entry.x - nearest.x) > 0.001f)
                plan.Corridors.Add(new Rect(Mathf.Min(entry.x, nearest.x) - half, entry.y - half,
                    Mathf.Abs(nearest.x - entry.x) + half * 2f, half * 2f));
            if (Mathf.Abs(entry.y - nearest.y) > 0.001f)
                plan.Corridors.Add(new Rect(nearest.x - half, Mathf.Min(entry.y, nearest.y) - half,
                    half * 2f, Mathf.Abs(nearest.y - entry.y) + half * 2f));
        }

        /// <summary>
        /// The wall boxes for a plan: footprint minus (rooms + corridors), as merged rectangles in
        /// footprint space. A builder extrudes each to storey height with a collider.
        /// </summary>
        public static List<Rect> RasterizeWalls(Rect footprint, InteriorPlan plan)
        {
            var walls = new List<Rect>();
            int nx = Mathf.Max(1, Mathf.RoundToInt(footprint.width / CellSize));
            int ny = Mathf.Max(1, Mathf.RoundToInt(footprint.height / CellSize));
            float cw = footprint.width / nx, ch = footprint.height / ny;

            // Mark walkable cells (center-sample against every walkable rect).
            var wall = new bool[nx * ny];
            for (int gy = 0; gy < ny; gy++)
            for (int gx = 0; gx < nx; gx++)
            {
                var c = new Vector2(footprint.x + (gx + 0.5f) * cw, footprint.y + (gy + 0.5f) * ch);
                wall[gy * nx + gx] = !IsWalkable(plan, c);
            }

            // Greedy merge: horizontal runs per row, then extend runs downward while the next row
            // has an identical run (classic rect-cover; small output, deterministic).
            var consumed = new bool[nx * ny];
            for (int gy = 0; gy < ny; gy++)
            {
                for (int gx = 0; gx < nx; gx++)
                {
                    int i = gy * nx + gx;
                    if (!wall[i] || consumed[i]) continue;

                    int runLen = 0;
                    while (gx + runLen < nx && wall[gy * nx + gx + runLen] && !consumed[gy * nx + gx + runLen])
                        runLen++;

                    int rows = 1;
                    while (gy + rows < ny && RowRunMatches(wall, consumed, nx, gy + rows, gx, runLen))
                        rows++;

                    for (int ry = 0; ry < rows; ry++)
                        for (int rx = 0; rx < runLen; rx++)
                            consumed[(gy + ry) * nx + gx + rx] = true;

                    walls.Add(new Rect(footprint.x + gx * cw, footprint.y + gy * ch,
                        runLen * cw, rows * ch));
                }
            }
            return walls;
        }

        private static bool RowRunMatches(bool[] wall, bool[] consumed, int nx, int row, int gx, int runLen)
        {
            for (int rx = 0; rx < runLen; rx++)
            {
                int i = row * nx + gx + rx;
                if (!wall[i] || consumed[i]) return false;
            }
            return true;
        }

        private static bool IsWalkable(InteriorPlan plan, Vector2 p)
        {
            if (plan.Rooms != null)
                for (int i = 0; i < plan.Rooms.Count; i++)
                    if (plan.Rooms[i].Contains(p)) return true;
            if (plan.Corridors != null)
                for (int i = 0; i < plan.Corridors.Count; i++)
                    if (plan.Corridors[i].Contains(p)) return true;
            return false;
        }
    }
}
