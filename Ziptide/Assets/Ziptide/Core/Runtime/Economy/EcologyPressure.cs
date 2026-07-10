using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>A recorded player disturbance: creatures DISABLED (never killed — non-lethal canon)
    /// suppress the local population until the wild heals. Lives in Core so WorldState can carry it
    /// (moved from Content.Ecology in 4.3c; EcologyCore consumes it from here).</summary>
    [Serializable]
    public struct EcologyPressure
    {
        public string creatureId;
        public int disabled;
        public long atUnix;
    }

    /// <summary>
    /// ECOLOGY 4.3c — the pure ledger law for pressures in a world's save: record one disable per
    /// entry-coalesced hour, and PRUNE so a long campaign never bloats the save — entries older
    /// than the heal horizon are spent (8 half-lives ≈ 0.4% residual), and the list is hard-capped
    /// with the oldest dropped first. Pinned by EcologyCoreTests.
    /// </summary>
    public static class EcologyPressureLedger
    {
        public const float SpentAfterHours = 48f;  // ≈ 8 half-lives at 6h — pressure is gone
        public const int MaxEntries = 64;          // save-size cap per world

        /// <summary>Record a disable: coalesces into the newest entry for the same species within
        /// the same hour (a hunting spree is one event, not forty rows), then prunes.</summary>
        public static void Record(List<EcologyPressure> ledger, string creatureId, long nowUnix)
        {
            if (ledger == null || string.IsNullOrEmpty(creatureId)) return;

            for (int i = ledger.Count - 1; i >= 0; i--)
            {
                if (ledger[i].creatureId != creatureId) continue;
                if (nowUnix - ledger[i].atUnix <= 3600)
                {
                    var e = ledger[i];
                    e.disabled += 1;
                    e.atUnix = nowUnix;
                    ledger[i] = e;
                    Prune(ledger, nowUnix);
                    return;
                }
                break; // newest same-species entry is older than an hour — start a fresh one
            }

            ledger.Add(new EcologyPressure { creatureId = creatureId, disabled = 1, atUnix = nowUnix });
            Prune(ledger, nowUnix);
        }

        /// <summary>Drop spent entries (older than the heal horizon), then enforce the hard cap
        /// oldest-first. A decade of play stays a handful of rows.</summary>
        public static void Prune(List<EcologyPressure> ledger, long nowUnix)
        {
            if (ledger == null) return;
            long horizon = (long)(SpentAfterHours * 3600f);
            ledger.RemoveAll(e => nowUnix - e.atUnix > horizon);
            while (ledger.Count > MaxEntries)
                ledger.RemoveAt(0);
        }
    }
}
