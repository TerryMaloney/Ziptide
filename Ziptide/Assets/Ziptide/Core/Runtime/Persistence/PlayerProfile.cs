using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// Serializable player save data — the unit of future online sync (designed online-ready now,
    /// server-authoritative later for the "Risk for planets" layer). JsonUtility-friendly: public
    /// fields, [Serializable] nested types, List&lt;&gt; instead of Dictionary. All content is
    /// addressed by string id (resources, worlds), consistent with ItemFactory / the registries.
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        /// <summary>Bump when the shape changes; ProfileSerializer migrates older saves forward.</summary>
        public const int CurrentSchemaVersion = 2; // v2: META-LOOP — transaction ledger (additive)

        public int schemaVersion = CurrentSchemaVersion;
        public string playerId = "";
        public string displayName = "Cal";
        public long createdAtUnix;
        public long lastSavedAtUnix;

        public List<string> flags = new List<string>();
        public List<ResourceAmount> resources = new List<ResourceAmount>();
        public List<WorldState> worlds = new List<WorldState>();
        /// <summary>Meta-Loop v2: the transaction ledger (ring-capped; see ResourceLedger).
        /// Mutate ONLY through RewardRouter — the mode contract's single economy path.</summary>
        public List<LedgerEntry> ledger = new List<LedgerEntry>();
        /// <summary>MP100 §F / A5: arena career (wins per difficulty, streaks, day stamps for the
        /// first-win/daily bonuses). Additive with a neutral default: a fresh block = zero career,
        /// old saves untouched. Pure rules live in Ziptide.Multiplayer.PvpProgression.</summary>
        public PvpCareerState pvpCareer = new PvpCareerState();

        // ── Flags ───────────────────────────────────────────────────────────
        public bool HasFlag(string flag) => !string.IsNullOrEmpty(flag) && flags.Contains(flag);

        public void SetFlag(string flag)
        {
            if (!string.IsNullOrEmpty(flag) && !flags.Contains(flag)) flags.Add(flag);
        }

        public void ClearFlag(string flag)
        {
            if (!string.IsNullOrEmpty(flag)) flags.Remove(flag);
        }

        // ── Resources ───────────────────────────────────────────────────────
        public double GetResource(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            for (int i = 0; i < resources.Count; i++)
                if (resources[i].id == id) return resources[i].amount;
            return 0;
        }

        /// <summary>Add (or subtract) an amount; clamps at 0. Returns the new total.</summary>
        public double AddResource(string id, double delta)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].id == id)
                {
                    resources[i].amount = Math.Max(0, resources[i].amount + delta);
                    return resources[i].amount;
                }
            }
            double v = Math.Max(0, delta);
            resources.Add(new ResourceAmount { id = id, amount = v });
            return v;
        }

        // ── Worlds ──────────────────────────────────────────────────────────
        public WorldState GetWorld(string worldId, bool createIfMissing = false)
        {
            if (string.IsNullOrEmpty(worldId)) return null;
            for (int i = 0; i < worlds.Count; i++)
                if (worlds[i].worldId == worldId) return worlds[i];
            if (!createIfMissing) return null;
            var ws = new WorldState { worldId = worldId };
            worlds.Add(ws);
            return ws;
        }
    }

    /// <summary>A resource balance, addressed by string id (e.g. "scrap", "ore").</summary>
    [Serializable]
    public class ResourceAmount
    {
        public string id;
        public double amount;
    }

    /// <summary>MP100 §F / A5 — the arena career: what the unlock ladder and the daily/first-win
    /// bonuses are computed FROM. JsonUtility-friendly flat ints; day stamps are UTC day numbers
    /// (unix/86400). Mutated only by the progression runtime at match end.</summary>
    [Serializable]
    public class PvpCareerState
    {
        public int matches;
        public int winsRookie, winsRegular, winsVeteran, winsNightmare;
        public int bestStreakEver;
        public int lastFirstWinDay;   // last UTC day a first-win bonus was paid
        public int lastDailyWinDay;   // last UTC day the daily challenge was beaten
    }

    /// <summary>Per-world save state: discovery, ownership (future conquest), and the idle anchor.</summary>
    [Serializable]
    public class WorldState
    {
        public string worldId;
        public bool discovered;
        public bool owned;
        public string ownerId = "";        // for future async planet-conquest
        public long lastResolvedAtUnix;     // idle-accrual anchor (advance mines/gardens since this)

        public List<MineState> mines = new List<MineState>();   // placed extractors (idle production)
        public List<PlotState> plots = new List<PlotState>();   // garden plots (time-based growth)
        /// <summary>Meta-Loop v2: this world's factory layout + batch progress. The graph LAYOUT is
        /// the save-truth (visuals are a skin); ProductionGraph (Content) simulates it.</summary>
        public List<MachineNodeState> factory = new List<MachineNodeState>();
        /// <summary>HARDWIRING 4.1f: player edits to this world's belt floors — a dynamic overlay
        /// per floor (patchers stay the canonical layout; see BeltFloorSave). Additive with a
        /// neutral default: empty = pre-persistence behavior exactly, old saves untouched.</summary>
        public List<BeltFloorState> beltFloors = new List<BeltFloorState>();
        /// <summary>ECOLOGY 4.3c: this world's recorded disturbances (creature disables) — the
        /// population engine reads these so a hunted zone stays thin ACROSS SESSIONS until the wild
        /// heals. Pruned by EcologyPressureLedger (spent after ~48h, hard-capped). Additive with a
        /// neutral default: empty = the undisturbed ecology, old saves untouched.</summary>
        public List<EcologyPressure> ecologyPressures = new List<EcologyPressure>();
    }
}
