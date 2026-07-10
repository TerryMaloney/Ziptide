using System.Collections.Generic;

namespace Ziptide.Content.Automation
{
    /// <summary>A lattice cell coordinate on a traced route.</summary>
    public struct BeltCoord
    {
        public int X, Z;
        public BeltCoord(int x, int z) { X = x; Z = z; }
    }

    /// <summary>
    /// HARDWIRING 4.1h / AUTOMATION_CONVEYORS "conductor mode" — pure route tracing: follow a line
    /// exactly the way an item would ride it. Belts and sources follow their direction; a splitter
    /// follows its PRIMARY direction (the inspection walks the main line; the fork is visible as it
    /// passes); a sink is the terminus (included). The trace ends at the lattice edge, at any
    /// non-carrier cell, or when a cell repeats (loop guard) — deterministic, no clock, no random.
    /// </summary>
    public static class BeltRoute
    {
        public static List<BeltCoord> Trace(BeltLattice lattice, int startX, int startZ, int maxSteps = 512)
        {
            var path = new List<BeltCoord>();
            if (lattice == null || !lattice.InBounds(startX, startZ)) return path;

            var visited = new HashSet<long>();
            int x = startX, z = startZ;
            for (int step = 0; step < maxSteps; step++)
            {
                var kind = lattice.KindAt(x, z);
                if (kind == CellKind.Empty) break;
                if (!visited.Add(((long)z << 32) | (uint)x)) break; // loop — the ride ends where it began
                path.Add(new BeltCoord(x, z));
                if (kind == CellKind.Sink) break; // the terminus — items end here, so does the ride

                var dir = lattice.DirAt(x, z); // splitter: primary exit — the main line
                int dx = dir == BeltDir.East ? 1 : dir == BeltDir.West ? -1 : 0;
                int dz = dir == BeltDir.North ? 1 : dir == BeltDir.South ? -1 : 0;
                x += dx; z += dz;
                if (!lattice.InBounds(x, z)) break; // end of the line — park at the lip, like an item
            }
            return path;
        }
    }

    /// <summary>
    /// The conductor's glide along a traced route, as pure kinematics: constant speed in CELLS per
    /// second — by default exactly <see cref="BeltLattice.DefaultSpeed"/>, so you ride your line at
    /// the ore's-eye pace (well under every comfort cap; the zipline's hard-cap law is inherited by
    /// construction). Position is an interpolated grid coordinate; the scene translator converts to
    /// world space and delta-translates the rig (never parents — the locked law).
    /// <see cref="BoundaryCrossings"/> counts cell lips crossed, for the per-cell haptic tick.
    /// </summary>
    public sealed class ConductorRide
    {
        private readonly IList<BeltCoord> _path;
        public float Speed { get; }               // cells per second
        public float ProgressCells { get; private set; }
        public int BoundaryCrossings { get; private set; }

        public ConductorRide(IList<BeltCoord> path, float cellsPerSecond = BeltLattice.DefaultSpeed)
        {
            _path = path ?? new List<BeltCoord>();
            Speed = cellsPerSecond > 0f ? cellsPerSecond : BeltLattice.DefaultSpeed;
        }

        public int PathCount => _path.Count;
        public float TotalCells => _path.Count < 2 ? 0f : _path.Count - 1;
        public bool Arrived => ProgressCells >= TotalCells;

        public void Step(float dt)
        {
            if (dt <= 0f || Arrived) return;
            int before = (int)ProgressCells;
            ProgressCells += Speed * dt;
            if (ProgressCells > TotalCells) ProgressCells = TotalCells;
            BoundaryCrossings += (int)ProgressCells - before;
        }

        /// <summary>Interpolated grid position (cell coordinates; the cell CENTER is x.0).</summary>
        public void GridPosition(out float gx, out float gz)
        {
            if (_path.Count == 0) { gx = 0f; gz = 0f; return; }
            if (_path.Count == 1 || Arrived)
            {
                var last = _path[_path.Count - 1];
                gx = last.X; gz = last.Z; return;
            }
            int i = (int)ProgressCells;
            float t = ProgressCells - i;
            var a = _path[i];
            var b = _path[i + 1];
            gx = a.X + (b.X - a.X) * t;
            gz = a.Z + (b.Z - a.Z) * t;
        }
    }
}
