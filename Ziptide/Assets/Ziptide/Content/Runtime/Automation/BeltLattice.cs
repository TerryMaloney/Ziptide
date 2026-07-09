using System;
using System.Collections.Generic;

namespace Ziptide.Content.Automation
{
    /// <summary>Cardinal flow direction of a belt cell.</summary>
    public enum BeltDir { North = 0, East = 1, South = 2, West = 3 }

    /// <summary>What occupies a lattice cell.</summary>
    public enum CellKind
    {
        Empty = 0,
        Belt = 1,     // moves items along its direction
        Source = 2,   // emits items (a machine's output port)
        Sink = 3,     // consumes items (a machine's input port)
    }

    /// <summary>One item riding the lattice. <see cref="Progress"/> runs 0→1 across its cell; the
    /// scene translator lerps the visual between cell centers from exactly this state.</summary>
    public sealed class BeltItem
    {
        public string ResourceId;
        public int X, Z;
        public float Progress;
    }

    /// <summary>
    /// HARDWIRING 4.1a / AUTOMATION_CONVEYORS — the conveyor layer's pure heart. A bounded grid of
    /// belt cells; items ride cell-to-cell at belt speed with HEAD BLOCKING (an item can't enter an
    /// occupied cell, so jams compress upstream exactly like the factory games that make belts
    /// satisfying), junction merges are round-robin FAIR, sources emit on a cadence, sinks consume
    /// and count. Deterministic: same layout + same tick sequence = same world, no randomness at all.
    ///
    /// TRUTH CONTRACT: the economy's truth stays ProductionGraph (MachineNodeState on the profile).
    /// The lattice is the PHYSICAL, watchable, hand-editable layer between machine ports — sinks
    /// report what arrived; a runtime adapter feeds those counts into graph batches. Visuals are a
    /// skin over <see cref="Items"/> (GamePool pucks lerped by Progress) — never the truth.
    /// </summary>
    public sealed class BeltLattice
    {
        public const float DefaultSpeed = 1.6f;      // cells per second — readable at VR walk pace
        public const float SourcePeriod = 1.25f;     // seconds between emissions when unblocked

        public int Width { get; }
        public int Depth { get; }
        public float Speed { get; }

        private readonly CellKind[] _kind;
        private readonly BeltDir[] _dir;
        private readonly string[] _sourceResource;   // per-cell resource a Source emits
        private readonly float[] _sourceClock;
        private readonly int[] _mergePick;           // round-robin cursor per cell (fair junctions)
        private readonly BeltItem[] _occupant;       // one item per cell — belts are single-file

        private readonly List<BeltItem> _items = new List<BeltItem>();
        private readonly Dictionary<string, int> _sunk = new Dictionary<string, int>();

        public IReadOnlyList<BeltItem> Items => _items;

        public BeltLattice(int width, int depth, float speed = DefaultSpeed)
        {
            Width = Math.Max(1, width);
            Depth = Math.Max(1, depth);
            Speed = speed > 0f ? speed : DefaultSpeed;
            int n = Width * Depth;
            _kind = new CellKind[n];
            _dir = new BeltDir[n];
            _sourceResource = new string[n];
            _sourceClock = new float[n];
            _mergePick = new int[n];
            _occupant = new BeltItem[n];
        }

        private int Idx(int x, int z) => z * Width + x;
        public bool InBounds(int x, int z) => x >= 0 && x < Width && z >= 0 && z < Depth;

        // ── Building (the hand-placement API the scene translator calls) ────────────────────────

        public bool PlaceBelt(int x, int z, BeltDir dir)
        {
            if (!InBounds(x, z)) return false;
            int i = Idx(x, z);
            _kind[i] = CellKind.Belt;
            _dir[i] = dir;
            return true;
        }

        public bool PlaceSource(int x, int z, BeltDir dir, string resourceId)
        {
            if (!InBounds(x, z) || string.IsNullOrEmpty(resourceId)) return false;
            int i = Idx(x, z);
            _kind[i] = CellKind.Source;
            _dir[i] = dir;
            _sourceResource[i] = resourceId;
            _sourceClock[i] = 0f;
            return true;
        }

        public bool PlaceSink(int x, int z)
        {
            if (!InBounds(x, z)) return false;
            _kind[Idx(x, z)] = CellKind.Sink;
            return true;
        }

        /// <summary>Remove whatever occupies the cell (the pick-it-back-up verb). Any item on the
        /// cell is returned to the caller's hand (removed from the lattice and handed back).</summary>
        public BeltItem Clear(int x, int z)
        {
            if (!InBounds(x, z)) return null;
            int i = Idx(x, z);
            _kind[i] = CellKind.Empty;
            _sourceResource[i] = null;
            var held = _occupant[i];
            if (held != null)
            {
                _occupant[i] = null;
                _items.Remove(held);
            }
            return held;
        }

        public CellKind KindAt(int x, int z) => InBounds(x, z) ? _kind[Idx(x, z)] : CellKind.Empty;
        public BeltDir DirAt(int x, int z) => InBounds(x, z) ? _dir[Idx(x, z)] : BeltDir.North;
        public BeltItem OccupantAt(int x, int z) => InBounds(x, z) ? _occupant[Idx(x, z)] : null;

        /// <summary>Items a sink has consumed for a resource — the ProductionGraph adapter reads and
        /// drains this.</summary>
        public int SunkCount(string resourceId)
            => resourceId != null && _sunk.TryGetValue(resourceId, out int c) ? c : 0;

        public int DrainSunk(string resourceId)
        {
            int c = SunkCount(resourceId);
            if (c > 0) _sunk[resourceId] = 0;
            return c;
        }

        /// <summary>Drop an item onto a belt/sink by hand (the reach-into-the-flow verb). Fails if
        /// the cell is occupied or not a carrier.</summary>
        public bool HandPlaceItem(int x, int z, string resourceId)
        {
            if (!InBounds(x, z) || string.IsNullOrEmpty(resourceId)) return false;
            int i = Idx(x, z);
            if (_kind[i] != CellKind.Belt && _kind[i] != CellKind.Sink) return false;
            if (_occupant[i] != null) return false;
            if (_kind[i] == CellKind.Sink) { Consume(resourceId); return true; }
            var item = new BeltItem { ResourceId = resourceId, X = x, Z = z, Progress = 0.5f };
            _occupant[i] = item;
            _items.Add(item);
            return true;
        }

        // ── Simulation ──────────────────────────────────────────────────────────────────────────

        private static void Step(BeltDir d, out int dx, out int dz)
        {
            dx = d == BeltDir.East ? 1 : d == BeltDir.West ? -1 : 0;
            dz = d == BeltDir.North ? 1 : d == BeltDir.South ? -1 : 0;
        }

        /// <summary>Advance the whole lattice by dt. Deterministic; call at a fixed cadence for
        /// perfectly reproducible factories.</summary>
        public void Tick(float dt)
        {
            if (dt <= 0f) return;

            // 1. Items advance within their cells; at the boundary they try to transfer.
            //    Iterate by index for determinism (insertion order — no dictionary iteration).
            for (int n = 0; n < _items.Count; n++)
            {
                var item = _items[n];
                int i = Idx(item.X, item.Z);
                if (_kind[i] != CellKind.Belt) continue; // stranded (belt removed) — holds in place

                float next = item.Progress + Speed * dt;
                if (next < 1f) { item.Progress = next; continue; }

                Step(_dir[i], out int dx, out int dz);
                int tx = item.X + dx, tz = item.Z + dz;
                if (!InBounds(tx, tz)) { item.Progress = 1f; continue; }        // end of the line
                int ti = Idx(tx, tz);

                if (_kind[ti] == CellKind.Sink)
                {
                    Consume(item.ResourceId);
                    _occupant[i] = null;
                    _items.RemoveAt(n); n--;
                    continue;
                }
                if (_kind[ti] != CellKind.Belt || _occupant[ti] != null)
                {
                    item.Progress = 1f;   // HEAD BLOCKING — wait at the lip; upstream compresses
                    continue;
                }
                // Junction fairness: when several neighbors feed one cell, the round-robin cursor
                // decides whose turn it is this pass. Single-feeder cells always pass.
                if (!MergeTurn(ti, item.X, item.Z)) { item.Progress = 1f; continue; }

                _occupant[i] = null;
                _occupant[ti] = item;
                item.X = tx; item.Z = tz;
                item.Progress = next - 1f;
            }

            // 2. Sources emit onto their facing cell when it's free.
            for (int z = 0; z < Depth; z++)
            for (int x = 0; x < Width; x++)
            {
                int i = Idx(x, z);
                if (_kind[i] != CellKind.Source) continue;
                _sourceClock[i] += dt;
                if (_sourceClock[i] < SourcePeriod) continue;

                Step(_dir[i], out int dx, out int dz);
                int tx = x + dx, tz = z + dz;
                if (!InBounds(tx, tz)) continue;
                int ti = Idx(tx, tz);
                if (_kind[ti] != CellKind.Belt || _occupant[ti] != null) continue; // blocked: hold, no overflow
                if (!MergeTurn(ti, x, z)) continue; // sources queue at junctions like everyone else

                _sourceClock[i] -= SourcePeriod;
                var item = new BeltItem { ResourceId = _sourceResource[i], X = tx, Z = tz, Progress = 0f };
                _occupant[ti] = item;
                _items.Add(item);
            }
        }

        /// <summary>
        /// Round-robin arbitration for cells fed by multiple belts/sources — FAIR but never
        /// starving: the preferred feeder passes and advances the cursor; anyone else passes only
        /// when the preferred feeder can't deliver right now (no item at its lip / source not
        /// ready), so an idle line never blocks a busy one. Deterministic.
        /// </summary>
        private bool MergeTurn(int targetIdx, int fromX, int fromZ)
        {
            Span<int> feeders = stackalloc int[4];
            int count = 0, mine = -1;
            int tx = targetIdx % Width, tz = targetIdx / Width;
            for (int d = 0; d < 4; d++)
            {
                Step((BeltDir)d, out int dx, out int dz);
                int nx = tx - dx, nz = tz - dz; // the cell that would feed via direction d
                if (!InBounds(nx, nz)) continue;
                int ni = Idx(nx, nz);
                if ((_kind[ni] == CellKind.Belt || _kind[ni] == CellKind.Source) && _dir[ni] == (BeltDir)d)
                {
                    if (nx == fromX && nz == fromZ) mine = count;
                    feeders[count++] = ni;
                }
            }
            if (count <= 1) return true;              // no contention
            if (mine < 0) return true;                // not a tracked feeder (hand-placed) — pass

            int preferred = _mergePick[targetIdx] % count;
            if (preferred == mine)
            {
                _mergePick[targetIdx]++;              // my turn consumed — next feeder up
                return true;
            }
            if (CanDeliver(feeders[preferred])) return false; // their turn and they're ready — wait
            return true;                              // preferred is idle — don't starve the line
        }

        /// <summary>Whether a feeder cell could hand an item into its target this instant.</summary>
        private bool CanDeliver(int feederIdx)
        {
            if (_kind[feederIdx] == CellKind.Source) return _sourceClock[feederIdx] >= SourcePeriod;
            var occ = _occupant[feederIdx];
            return occ != null && occ.Progress >= 1f - 1e-4f;
        }

        private void Consume(string resourceId)
        {
            if (string.IsNullOrEmpty(resourceId)) return;
            _sunk.TryGetValue(resourceId, out int c);
            _sunk[resourceId] = c + 1;
        }
    }
}
