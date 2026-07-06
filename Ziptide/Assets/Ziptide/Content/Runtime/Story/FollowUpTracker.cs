using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>
    /// Pure bookkeeping for <see cref="RillTrigger.FollowUp"/> lines — no Unity/scene dependency, so it's
    /// directly unit-testable. RillCompanion owns the wiring: registers a line the moment its watched flag
    /// is first noticed, then ticks once per gate crossing (<see cref="RillCompanion.OnGateDeparture"/>
    /// already fires on every travel) until the line's <see cref="RillLine.crossingsDelay"/> elapses.
    /// Not persisted across save/load (in-memory on the companion only) — a follow-up pending when the
    /// player quits mid-count won't survive a reload in this version; documented, not silently papered over.
    /// </summary>
    public class FollowUpTracker
    {
        private class Pending
        {
            public RillLine line;
            public int remaining;
        }

        private readonly List<Pending> _pending = new List<Pending>();
        private readonly HashSet<string> _registered = new HashSet<string>();

        /// <summary>Start the countdown for a line the moment its watched flag is first seen. Idempotent —
        /// registering the same line id twice (e.g. a repeated poll hit) only starts the clock once.</summary>
        public void Register(RillLine line)
        {
            if (line == null || string.IsNullOrEmpty(line.id) || !_registered.Add(line.id)) return;
            _pending.Add(new Pending { line = line, remaining = Math.Max(1, line.crossingsDelay) });
        }

        /// <summary>Call once per gate departure. Appends any lines whose delay just elapsed to `ready`
        /// (removing them from tracking — each fires exactly once).</summary>
        public void TickGateCrossing(List<RillLine> ready)
        {
            if (ready == null) return;
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                _pending[i].remaining--;
                if (_pending[i].remaining <= 0)
                {
                    ready.Add(_pending[i].line);
                    _pending.RemoveAt(i);
                }
            }
        }

        public int PendingCount => _pending.Count;
    }
}
