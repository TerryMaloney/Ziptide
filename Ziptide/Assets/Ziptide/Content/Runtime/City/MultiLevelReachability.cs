using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>How two nodes are connected in a multi-level world (the board's 1.4 edge kinds).</summary>
    public enum TraversalKind { Step, Zipline, Climb, Elevator, Grapple, JumpPad }

    /// <summary>A traversal link between two global node ids. Ziplines/jump-pads are usually ONE-WAY
    /// (you ride down, you can't ride back up), which is why <see cref="bidirectional"/> exists — a
    /// world reachable "down" is not automatically reachable "back up".</summary>
    public struct TraversalEdge
    {
        public int From;
        public int To;
        public TraversalKind Kind;
        public bool Bidirectional;

        public TraversalEdge(int from, int to, TraversalKind kind, bool bidirectional = true)
        {
            From = from; To = to; Kind = kind; Bidirectional = bidirectional;
        }
    }

    /// <summary>
    /// PURE layered-world reachability (Hardwiring Phase 1.4). A heightmap is single-level, so
    /// <see cref="GridReachability"/> can only flood ONE walkable plane; stacked worlds (caverns under
    /// mesas, sky-bridges, multi-floor interiors) need a graph whose edges cross layers via
    /// step/zip/climb/elevator/grapple/jump links. This composes any number of walkable grid LAYERS
    /// (each flooded with GridReachability's own 4-connectivity + maxStep rule) with explicit vertical
    /// <see cref="TraversalEdge"/>s, then floods the union from a start node. The editor audit feeds a
    /// world's sampled layers + its authored traversal links here to WARN when an elevated/underground
    /// POI is provably unreachable. No UnityEngine → EditMode-testable.
    ///
    /// Node id convention: <c>layer * (w*h) + y*w + x</c>. Use <see cref="Build"/> to assemble the graph
    /// from layers, add cross-layer edges, then <see cref="Flood"/>.
    /// </summary>
    public sealed class MultiLevelReachability
    {
        private readonly int _w, _h, _layerSize, _layers;
        private readonly bool[] _walkable;                 // length layers*w*h
        // Adjacency built lazily from same-layer step rules + explicit edges.
        private readonly List<int>[] _adj;

        /// <summary>Nodes-per-layer (w*h) and layer count are fixed at construction.</summary>
        public MultiLevelReachability(int width, int height, int layers)
        {
            _w = Math.Max(0, width);
            _h = Math.Max(0, height);
            _layers = Math.Max(0, layers);
            _layerSize = _w * _h;
            int n = _layerSize * _layers;
            _walkable = new bool[Math.Max(0, n)];
            _adj = new List<int>[Math.Max(0, n)];
        }

        public int NodeCount => _layerSize * _layers;
        public int NodeId(int layer, int x, int y) => layer * _layerSize + y * _w + x;

        /// <summary>Register a layer's walkable mask + heights, auto-linking 4-connected same-layer
        /// steps within maxStep (the GridReachability rule, applied per layer).</summary>
        public void SetLayer(int layer, bool[] walkable, float[] heights, float maxStep)
        {
            if (layer < 0 || layer >= _layers || walkable == null || walkable.Length != _layerSize) return;
            int baseId = layer * _layerSize;
            for (int i = 0; i < _layerSize; i++) _walkable[baseId + i] = walkable[i];

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            for (int y = 0; y < _h; y++)
            for (int x = 0; x < _w; x++)
            {
                int c = y * _w + x;
                if (!walkable[c]) continue;
                for (int k = 0; k < 4; k++)
                {
                    int nx = x + dx[k], ny = y + dy[k];
                    if (nx < 0 || ny < 0 || nx >= _w || ny >= _h) continue;
                    int nc = ny * _w + nx;
                    if (!walkable[nc]) continue;
                    if (heights != null && Math.Abs(heights[nc] - heights[c]) > maxStep) continue;
                    Link(baseId + c, baseId + nc); // same-layer steps are always bidirectional
                    Link(baseId + nc, baseId + c);
                }
            }
        }

        /// <summary>Add a cross-layer (or long-range) traversal link. Both endpoints are forced
        /// walkable (a zipline platform is standable even if its cell wasn't part of a walkable grid).</summary>
        public void AddEdge(TraversalEdge e)
        {
            if (!InRange(e.From) || !InRange(e.To)) return;
            _walkable[e.From] = true;
            _walkable[e.To] = true;
            Link(e.From, e.To);
            if (e.Bidirectional) Link(e.To, e.From);
        }

        /// <summary>BFS from a start node over the assembled graph. Returns reached[] of length NodeCount.</summary>
        public bool[] Flood(int start)
        {
            var reached = new bool[Math.Max(0, NodeCount)];
            if (!InRange(start) || !_walkable[start]) return reached;

            var q = new Queue<int>();
            reached[start] = true;
            q.Enqueue(start);
            while (q.Count > 0)
            {
                int c = q.Dequeue();
                var links = _adj[c];
                if (links == null) continue;
                for (int i = 0; i < links.Count; i++)
                {
                    int n = links[i];
                    if (reached[n] || !_walkable[n]) continue;
                    reached[n] = true;
                    q.Enqueue(n);
                }
            }
            return reached;
        }

        /// <summary>Audit helper: the walkable nodes NOT reached from start (stranded POIs live here).</summary>
        public List<int> Unreached(int start)
        {
            var reached = Flood(start);
            var outList = new List<int>();
            for (int i = 0; i < reached.Length; i++)
                if (_walkable[i] && !reached[i]) outList.Add(i);
            return outList;
        }

        public static int Count(bool[] reached)
        {
            if (reached == null) return 0;
            int c = 0;
            for (int i = 0; i < reached.Length; i++) if (reached[i]) c++;
            return c;
        }

        private bool InRange(int id) => id >= 0 && id < NodeCount;

        private void Link(int a, int b)
        {
            if (_adj[a] == null) _adj[a] = new List<int>(4);
            if (!_adj[a].Contains(b)) _adj[a].Add(b);
        }
    }
}
