using System.Collections.Generic;

namespace Ziptide.Content.Automation
{
    /// <summary>
    /// HARDWIRING 4.1k / AUTOMATION_CONVEYORS "Blueprints" — the "I built this once, now clone it"
    /// payoff, pure. <see cref="Capture"/> floods the 4-connected component of PLAYER-BUILDABLE
    /// cells (Belt + Splitter — ports/sinks are machine-bound and never cloned) from a seed cell,
    /// normalized to its bounding-box origin with the seed offset remembered, so a stamp lands
    /// exactly where the hand points. Deterministic: fixed flood order + cells sorted (z,x); no
    /// clock, no randomness. <see cref="StampInto"/> is all-or-nothing — a blueprint never
    /// half-lands on a crowded grid.
    /// </summary>
    public sealed class BeltBlueprint
    {
        public struct Cell
        {
            public int Dx, Dz;      // offset from the blueprint's normalized origin
            public CellKind Kind;   // Belt or Splitter only
            public BeltDir Dir;
        }

        private readonly List<Cell> _cells;
        /// <summary>Where the capture seed sits inside the normalized footprint — the stamp anchor.</summary>
        public int SeedDx { get; }
        public int SeedDz { get; }

        public IReadOnlyList<Cell> Cells => _cells;
        public int Count => _cells.Count;

        private BeltBlueprint(List<Cell> cells, int seedDx, int seedDz)
        {
            _cells = cells;
            SeedDx = seedDx;
            SeedDz = seedDz;
        }

        private static bool Capturable(CellKind k) => k == CellKind.Belt || k == CellKind.Splitter;

        /// <summary>Capture the connected line under the seed. Null when the seed isn't a
        /// capturable cell or the component exceeds <paramref name="maxCells"/> (never a partial
        /// capture — the player should trust what the wand holds).</summary>
        public static BeltBlueprint Capture(BeltLattice lattice, int seedX, int seedZ, int maxCells = 64)
        {
            if (lattice == null || !Capturable(lattice.KindAt(seedX, seedZ))) return null;

            var visited = new HashSet<long>();
            var queue = new Queue<(int x, int z)>();
            var found = new List<(int x, int z)>();
            long Key(int x, int z) => ((long)z << 32) | (uint)x;

            queue.Enqueue((seedX, seedZ));
            visited.Add(Key(seedX, seedZ));
            while (queue.Count > 0)
            {
                var (x, z) = queue.Dequeue();
                found.Add((x, z));
                if (found.Count > maxCells) return null; // too big to hold — no partial captures

                // Fixed expansion order (N,E,S,W) — deterministic flood.
                for (int d = 0; d < 4; d++)
                {
                    int nx = x + (d == 1 ? 1 : d == 3 ? -1 : 0);
                    int nz = z + (d == 0 ? 1 : d == 2 ? -1 : 0);
                    if (!lattice.InBounds(nx, nz) || !Capturable(lattice.KindAt(nx, nz))) continue;
                    if (visited.Add(Key(nx, nz))) queue.Enqueue((nx, nz));
                }
            }

            int minX = int.MaxValue, minZ = int.MaxValue;
            foreach (var (x, z) in found) { if (x < minX) minX = x; if (z < minZ) minZ = z; }

            var cells = new List<Cell>(found.Count);
            foreach (var (x, z) in found)
                cells.Add(new Cell
                {
                    Dx = x - minX, Dz = z - minZ,
                    Kind = lattice.KindAt(x, z), Dir = lattice.DirAt(x, z)
                });
            cells.Sort((a, b) => a.Dz != b.Dz ? a.Dz - b.Dz : a.Dx - b.Dx); // stable identity

            return new BeltBlueprint(cells, seedX - minX, seedZ - minZ);
        }

        /// <summary>True when every cell of the footprint lands in-bounds on EMPTY ground.</summary>
        public bool CanStampAt(BeltLattice lattice, int originX, int originZ)
        {
            if (lattice == null || _cells.Count == 0) return false;
            foreach (var c in _cells)
            {
                int x = originX + c.Dx, z = originZ + c.Dz;
                if (!lattice.InBounds(x, z) || lattice.KindAt(x, z) != CellKind.Empty) return false;
            }
            return true;
        }

        /// <summary>Stamp into the lattice (all-or-nothing). Returns the placed cells in world grid
        /// coords, or an empty list when the footprint didn't fit — the scene layer builds visuals
        /// and persistence from exactly this list.</summary>
        public List<Cell> StampInto(BeltLattice lattice, int originX, int originZ)
        {
            var placed = new List<Cell>();
            if (!CanStampAt(lattice, originX, originZ)) return placed;
            foreach (var c in _cells)
            {
                int x = originX + c.Dx, z = originZ + c.Dz;
                if (c.Kind == CellKind.Splitter) lattice.PlaceSplitter(x, z, c.Dir);
                else lattice.PlaceBelt(x, z, c.Dir);
                placed.Add(new Cell { Dx = x, Dz = z, Kind = c.Kind, Dir = c.Dir });
            }
            return placed;
        }
    }
}
