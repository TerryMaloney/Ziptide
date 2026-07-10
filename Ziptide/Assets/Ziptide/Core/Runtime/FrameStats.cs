using System;

namespace Ziptide.Core
{
    /// <summary>
    /// Pure frame-time statistics over a sliding window (the runtime-health core). Push one frame
    /// duration per frame; read averages, percentiles, and dropped-frame counts. No UnityEngine —
    /// CI pins the math headlessly. Window is a ring: once full, oldest samples fall off.
    /// </summary>
    public class FrameStats
    {
        private readonly float[] _ms;
        private int _next;
        private int _count;

        public FrameStats(int windowSize = 720)   // ~10s at 72Hz
        {
            _ms = new float[Math.Max(8, windowSize)];
        }

        public int Count => _count;
        public int Window => _ms.Length;

        public void Push(float frameMs)
        {
            if (frameMs < 0f) return;
            _ms[_next] = frameMs;
            _next = (_next + 1) % _ms.Length;
            if (_count < _ms.Length) _count++;
        }

        public void Reset() { _next = 0; _count = 0; }

        public float AverageMs
        {
            get
            {
                if (_count == 0) return 0f;
                float sum = 0f;
                for (int i = 0; i < _count; i++) sum += _ms[i];
                return sum / _count;
            }
        }

        public float WorstMs
        {
            get
            {
                float worst = 0f;
                for (int i = 0; i < _count; i++) if (_ms[i] > worst) worst = _ms[i];
                return worst;
            }
        }

        /// <summary>Frame time at the given percentile (0..1) — 0.99 is "the 1% low" when read as
        /// FPS. Copies + sorts the window; call at reporting cadence, not per frame.</summary>
        public float PercentileMs(float p)
        {
            if (_count == 0) return 0f;
            var copy = new float[_count];
            Array.Copy(_ms, copy, _count);
            Array.Sort(copy);
            int idx = (int)Math.Round(Math.Max(0f, Math.Min(1f, p)) * (_count - 1));
            return copy[idx];
        }

        /// <summary>How many frames in the window blew the budget (13.9ms = 72Hz).</summary>
        public int DroppedFrames(float budgetMs)
        {
            int n = 0;
            for (int i = 0; i < _count; i++) if (_ms[i] > budgetMs) n++;
            return n;
        }

        public float AverageFps => AverageMs > 0f ? 1000f / AverageMs : 0f;

        /// <summary>FPS at the slow tail (the number that actually predicts VR comfort).</summary>
        public float OnePercentLowFps
        {
            get
            {
                float ms = PercentileMs(0.99f);
                return ms > 0f ? 1000f / ms : 0f;
            }
        }
    }
}
