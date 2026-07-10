using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure breakable-wall state: a GRID OF BRICKS, each with its own hit points (no Unity → CI-testable).
    /// A hammer hit damages ONE brick — the one nearest the impact — and a brick only breaks after
    /// <see cref="BrickHits"/> swings, so a wall comes apart a small piece at a time over several hits
    /// (Terry's ask: more & smaller bricks, localized damage, multiple swings each). The whole wall heals
    /// back to intact <see cref="RegenSeconds"/> after the last hit. Deterministic from a passed-in clock.
    ///
    /// Replaces the old whole-wall Intact→SmallHole→LargeHole staging. (The future PvP netcode wall-sync
    /// message-model will serialize this brick grid instead of the old 3-stage enum — see HANDOFF.)
    /// </summary>
    public class WallState
    {
        public int Cols { get; }
        public int Rows { get; }
        public int BrickHits { get; }
        public double RegenSeconds { get; }

        private readonly int[] _hits; // hits taken per brick, index = row * Cols + col
        private double _lastHitAt;
        private bool _anyDamaged;

        public WallState(int cols = 6, int rows = 5, int brickHits = 2, double regenSeconds = -1.0)
        {
            Cols = cols < 1 ? 1 : cols;
            Rows = rows < 1 ? 1 : rows;
            BrickHits = brickHits < 1 ? 1 : brickHits;
            RegenSeconds = regenSeconds > 0 ? regenSeconds : PvpRules.WallHoleRegenSeconds;
            _hits = new int[Cols * Rows];
        }

        public int BrickCount => _hits.Length;

        public bool InBounds(int col, int row) => col >= 0 && col < Cols && row >= 0 && row < Rows;
        private int Index(int col, int row) => row * Cols + col;

        /// <summary>Hits landed on a brick so far (0..BrickHits). Out-of-bounds returns 0.</summary>
        public int HitsOn(int col, int row) => InBounds(col, row) ? _hits[Index(col, row)] : 0;

        /// <summary>True once a brick has taken BrickHits swings (it's a gap). Out-of-bounds returns false.</summary>
        public bool IsBroken(int col, int row) => InBounds(col, row) && _hits[Index(col, row)] >= BrickHits;

        /// <summary>How many bricks are fully broken (open gaps).</summary>
        public int BrokenCount
        {
            get
            {
                int n = 0;
                for (int i = 0; i < _hits.Length; i++) if (_hits[i] >= BrickHits) n++;
                return n;
            }
        }

        /// <summary>True if any brick has taken at least one hit (used to decide whether to run regen).</summary>
        public bool AnyDamaged => _anyDamaged;

        /// <summary>
        /// Damage ONE brick. Returns true if that brick just broke on this hit. Out-of-bounds is ignored
        /// (returns false). An already-broken brick stays broken but still refreshes the regen clock so the
        /// hole doesn't heal while someone keeps swinging nearby. Any landed hit resets the regen clock.
        /// </summary>
        public bool HitBrick(int col, int row, double now)
        {
            if (!InBounds(col, row)) return false;
            int idx = Index(col, row);
            _lastHitAt = now;
            _anyDamaged = true;
            if (_hits[idx] >= BrickHits) return false; // already a gap
            _hits[idx]++;
            return _hits[idx] >= BrickHits;
        }

        /// <summary>
        /// DESTRUCTION V2 (Terry: "chunks that break off in a way that makes sense"): after a break,
        /// any brick with no 4-connected path of INTACT bricks down to the BOTTOM ROW has lost its
        /// support and breaks too. So an arch over a hole HOLDS (the load path goes around), but sever
        /// a full band and the slab above avalanches, and a hanging island crumbles. Pure flood fill
        /// from the bottom row; deterministic. Returns exactly the bricks that collapsed on THIS call
        /// (the view spawns falling debris for those), already marked broken and counted for regen.
        /// </summary>
        public System.Collections.Generic.List<(int col, int row)> CollapseUnsupported(double now)
        {
            var supported = new bool[_hits.Length];
            var stack = new System.Collections.Generic.Stack<int>();

            void Consider(int c, int r)
            {
                if (!InBounds(c, r)) return;
                int j = Index(c, r);
                if (supported[j] || _hits[j] >= BrickHits) return;
                supported[j] = true;
                stack.Push(j);
            }

            for (int c = 0; c < Cols; c++) Consider(c, 0); // the floor is the anchor
            while (stack.Count > 0)
            {
                int i = stack.Pop();
                int col = i % Cols, row = i / Cols;
                Consider(col - 1, row);
                Consider(col + 1, row);
                Consider(col, row - 1);
                Consider(col, row + 1);
            }

            var fell = new System.Collections.Generic.List<(int col, int row)>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    int i = Index(c, r);
                    if (_hits[i] < BrickHits && !supported[i])
                    {
                        _hits[i] = BrickHits;
                        fell.Add((c, r));
                    }
                }
            if (fell.Count > 0) { _lastHitAt = now; _anyDamaged = true; } // a collapse resets regen too
            return fell;
        }

        /// <summary>Advance the clock; once the regen window elapses from the last hit, restore ALL bricks.</summary>
        public void Tick(double now)
        {
            if (!_anyDamaged) return;
            if (now - _lastHitAt >= RegenSeconds)
            {
                for (int i = 0; i < _hits.Length; i++) _hits[i] = 0;
                _anyDamaged = false;
            }
        }
    }
}
