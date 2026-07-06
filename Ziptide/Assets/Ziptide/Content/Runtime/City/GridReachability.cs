using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>
    /// PURE grid flood-fill reachability (Additions Bank pull WORLDS_50 #13). Given a walkable grid +
    /// per-cell heights, BFS from a start cell over 4-connectivity, allowing a step to a neighbour only
    /// when both cells are walkable AND the height difference is within maxStep — so a cliff/chasm
    /// breaks connectivity but a gentle grade does not. No UnityEngine → EditMode-testable. The editor
    /// audit (`WorldReachabilityAuditRules`) samples the built terrain into a grid and feeds it here to
    /// WARN when a POI is on a disconnected island; the same core also serves creature/bot nav sanity.
    ///
    /// This complements TerrainField.WalkableFraction (which measures how much AREA is walkable-slope):
    /// a world can be 90% walkable yet strand a POI behind a ravine — that's what this catches.
    /// </summary>
    public static class GridReachability
    {
        /// <summary>Flood from (startX,startY). Returns a reached[] of length w*h (row-major).
        /// heights may be null (skip the step check — pure connectivity of walkable cells).</summary>
        public static bool[] Flood(int w, int h, bool[] walkable, float[] heights,
            int startX, int startY, float maxStep)
        {
            var reached = new bool[Math.Max(0, w * h)];
            if (w <= 0 || h <= 0 || walkable == null || walkable.Length != w * h) return reached;
            if (startX < 0 || startY < 0 || startX >= w || startY >= h) return reached;
            int start = startY * w + startX;
            if (!walkable[start]) return reached; // standing on a wall → nothing reachable

            var q = new Queue<int>();
            reached[start] = true;
            q.Enqueue(start);
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (q.Count > 0)
            {
                int c = q.Dequeue();
                int cx = c % w, cy = c / w;
                for (int k = 0; k < 4; k++)
                {
                    int nx = cx + dx[k], ny = cy + dy[k];
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                    int n = ny * w + nx;
                    if (reached[n] || !walkable[n]) continue;
                    if (heights != null && Math.Abs(heights[n] - heights[c]) > maxStep) continue;
                    reached[n] = true;
                    q.Enqueue(n);
                }
            }
            return reached;
        }

        public static int Count(bool[] reached)
        {
            if (reached == null) return 0;
            int c = 0;
            for (int i = 0; i < reached.Length; i++) if (reached[i]) c++;
            return c;
        }

        public static bool IsReached(bool[] reached, int w, int x, int y)
        {
            if (reached == null || w <= 0 || x < 0 || y < 0 || x >= w) return false;
            int i = y * w + x;
            return i >= 0 && i < reached.Length && reached[i];
        }
    }
}
