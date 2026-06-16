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
        public const int CurrentSchemaVersion = 1;

        public int schemaVersion = CurrentSchemaVersion;
        public string playerId = "";
        public string displayName = "Cal";
        public long createdAtUnix;
        public long lastSavedAtUnix;

        public List<string> flags = new List<string>();
        public List<ResourceAmount> resources = new List<ResourceAmount>();
        public List<WorldState> worlds = new List<WorldState>();

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

    /// <summary>Per-world save state: discovery, ownership (future conquest), and the idle anchor.</summary>
    [Serializable]
    public class WorldState
    {
        public string worldId;
        public bool discovered;
        public bool owned;
        public string ownerId = "";        // for future async planet-conquest
        public long lastResolvedAtUnix;     // idle-accrual anchor (advance mines/gardens since this)
    }
}
