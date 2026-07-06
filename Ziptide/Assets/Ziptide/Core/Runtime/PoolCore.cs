using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// ARCHITECTURE V2 Q4a — the PURE, generic pooling core (no UnityEngine). A free-list with a
    /// factory + optional get/release hooks + a retained-cap, plus deterministic bookkeeping
    /// (Created / Reused / Live / Free). This is the tested brain; <see cref="GamePool"/> is the thin
    /// GameObject translator on top of it (the "pure core first" law — BotBrain/Conquest pattern).
    ///
    /// Behaviour contract (what the tests pin):
    ///  - Acquire pops a free item if any (Reused++), else builds one via the factory (Created++);
    ///    either way Live++ and the onGet hook fires.
    ///  - Release runs onRelease, then RETAINS the item on the free list unless maxRetained is set and
    ///    the free list is already full — in which case it is DROPPED. Returns true=retained,
    ///    false=dropped, so the GameObject wrapper knows when to actually Destroy.
    ///  - Release(null) is a no-op returning false; Live never goes negative.
    /// </summary>
    public sealed class PoolCore<T> where T : class
    {
        private readonly Stack<T> _free = new Stack<T>();
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly int _maxRetained; // 0 = unlimited retained free objects

        public int Created { get; private set; }
        public int Reused { get; private set; }
        public int Live { get; private set; }        // currently acquired (out on loan)
        public int Free => _free.Count;

        public PoolCore(Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, int maxRetained = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onRelease = onRelease;
            _maxRetained = maxRetained < 0 ? 0 : maxRetained;
        }

        public T Acquire()
        {
            T item;
            if (_free.Count > 0) { item = _free.Pop(); Reused++; }
            else { item = _factory(); Created++; }
            Live++;
            _onGet?.Invoke(item);
            return item;
        }

        /// <summary>Return an item. True = kept for reuse; false = dropped (caller destroys it).</summary>
        public bool Release(T item)
        {
            if (item == null) return false;
            if (Live > 0) Live--;
            _onRelease?.Invoke(item);
            if (_maxRetained > 0 && _free.Count >= _maxRetained) return false; // over cap → drop
            _free.Push(item);
            return true;
        }

        /// <summary>Build up to <paramref name="count"/> free items now (no Live change). Returns how
        /// many were actually added (respects the retained cap).</summary>
        public int Prewarm(int count)
        {
            int made = 0;
            for (int i = 0; i < count; i++)
            {
                if (_maxRetained > 0 && _free.Count >= _maxRetained) break;
                _free.Push(_factory());
                Created++;
                made++;
            }
            return made;
        }

        /// <summary>Drop the free list (does NOT touch loaned items). Returns the count dropped so the
        /// wrapper can destroy them.</summary>
        public T[] DrainFree()
        {
            var arr = _free.ToArray();
            _free.Clear();
            return arr;
        }
    }
}
