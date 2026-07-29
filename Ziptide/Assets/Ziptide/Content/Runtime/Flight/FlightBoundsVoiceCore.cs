using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>What the voice core remembers between frames. Serializable so a translator can hold
    /// it as a plain field and tests can drive it without a scene.</summary>
    [Serializable]
    public struct FlightBoundsVoiceState
    {
        public FlightBoundKind SpokenKind;
        public FlightBoundLevel SpokenLevel;
        public float LastSpokenTime;
        public int Cursor;          // advances once per delivered line — drives the shuffle walk
        public float ClearSince;    // when the reading first went clear (for the re-arm delay)
    }

    /// <summary>
    /// RILL's side of the bounds ladder (Terry, 2026-07-29: *"there should be a variety of
    /// responses from Rill so it doesn't get too repetitive and stale"*).
    ///
    /// Two mechanisms do that work, and both are pure:
    /// 1. **She only speaks on a RISE.** Holding position inside a tier is silent; the line comes
    ///    when the situation gets worse, or when a different thing starts being the problem.
    /// 2. **The pool is walked, not rolled.** <see cref="PickIndex"/> steps a pool by a stride
    ///    coprime with its size, so every line in a pool is spent before ANY line repeats — which
    ///    a random pick cannot promise and is exactly how a companion starts to sound like a loop.
    /// </summary>
    public static class FlightBoundsVoiceCore
    {
        /// <summary>Minimum gap between two bounds lines. Long enough that a scrape along a wreck
        /// field is a conversation, not a stutter.</summary>
        public const float CooldownSeconds = 6f;

        /// <summary>How long the reading must stay clear before the same tier can speak again —
        /// stops a pilot hovering exactly on a threshold from re-triggering the line.</summary>
        public const float ReArmSeconds = 4f;

        /// <summary>Lines per pool. Every (kind, tier) has this many, so no pool can run dry.</summary>
        public const int PoolSize = 4;

        /// <summary>The largest stride below <paramref name="count"/> that is coprime with it. Any
        /// such stride visits every index exactly once before repeating.</summary>
        public static int StrideFor(int count)
        {
            if (count <= 2) return 1;
            for (int s = count / 2 + 1; s < count; s++)
                if (Gcd(s, count) == 1) return s;
            return 1;
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0) { int t = a % b; a = b; b = t; }
            return a;
        }

        /// <summary>Walk a pool. Consecutive cursors never land on the same index, and a full lap
        /// of the cursor is a permutation of the pool.</summary>
        public static int PickIndex(int count, int cursor)
        {
            if (count <= 1) return 0;
            int i = (cursor * StrideFor(count)) % count;
            return i < 0 ? i + count : i;
        }

        /// <summary>
        /// Should RILL say something about this reading right now, and if so which line?
        /// Advances the state when it returns true, so the caller can drive it straight from Update.
        /// </summary>
        public static bool ShouldSpeak(ref FlightBoundsVoiceState v, FlightBoundsReading r, float now,
            out string lineId)
        {
            lineId = null;

            if (r.Level == FlightBoundLevel.Clear)
            {
                // Track how long we have been out of trouble; only a sustained clear re-arms.
                if (v.SpokenLevel != FlightBoundLevel.Clear)
                {
                    if (v.ClearSince <= 0f) v.ClearSince = now;
                    else if (now - v.ClearSince >= ReArmSeconds)
                    {
                        v.SpokenKind = FlightBoundKind.None;
                        v.SpokenLevel = FlightBoundLevel.Clear;
                        v.ClearSince = 0f;
                    }
                }
                return false;
            }

            v.ClearSince = 0f;

            bool rise = r.Level > v.SpokenLevel || r.Kind != v.SpokenKind;
            if (!rise) return false;
            if (now - v.LastSpokenTime < CooldownSeconds) return false;

            lineId = LineId(r.Kind, r.Level, PickIndex(PoolSize, v.Cursor));
            v.SpokenKind = r.Kind;
            v.SpokenLevel = r.Level;
            v.LastSpokenTime = now;
            v.Cursor++;
            return true;
        }

        /// <summary>The authored RILL line id for a pool slot. HARD shares CORRECTING's pool —
        /// by the time she is taking the ship back the words are the same, only later.</summary>
        public static string LineId(FlightBoundKind kind, FlightBoundLevel level, int index)
        {
            string tier = level >= FlightBoundLevel.Correcting ? "COR" : "ADV";
            return "BOUNDS_" + kind.ToString().ToUpperInvariant() + "_" + tier + "_" + index;
        }
    }
}
