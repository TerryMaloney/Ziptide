using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>A player's side of the war: stockpiles + fleet. List/int fields = JsonUtility-friendly.</summary>
    [Serializable]
    public class ConquestPlayer
    {
        public int playerId;
        public string displayName = "";
        public int flux = 5, alloy = 5, bloommatter = 1;   // starting stock
        public List<string> fleetVesselIds = new List<string>();
        public int attacksThisTurn = 0;

        public int Stock(ConquestResource r) =>
            r == ConquestResource.Flux ? flux : r == ConquestResource.Alloy ? alloy : bloommatter;

        public void AddStock(ConquestResource r, int amount)
        {
            if (r == ConquestResource.Flux) flux = Math.Max(0, flux + amount);
            else if (r == ConquestResource.Alloy) alloy = Math.Max(0, alloy + amount);
            else bloommatter = Math.Max(0, bloommatter + amount);
        }

        public bool CanAfford(int f, int a, int b) => flux >= f && alloy >= a && bloommatter >= b;

        public void Pay(int f, int a, int b) { flux -= f; alloy -= a; bloommatter -= b; }
    }

    /// <summary>
    /// The whole war, pure and serializable: planets + players + turn. All mutation goes through the
    /// action methods below (they validate); the resolver applies battle outcomes. The galaxy itself is
    /// built by <see cref="ConquestGalaxy"/> from the shipped story worlds.
    /// </summary>
    [Serializable]
    public class ConquestState
    {
        public List<PlanetNode> planets = new List<PlanetNode>();
        public List<ConquestPlayer> players = new List<ConquestPlayer>();
        public int turn = 1;

        public PlanetNode GetPlanet(string id)
        {
            for (int i = 0; i < planets.Count; i++)
                if (planets[i].planetId == id) return planets[i];
            return null;
        }

        public ConquestPlayer GetPlayer(int id)
        {
            for (int i = 0; i < players.Count; i++)
                if (players[i].playerId == id) return players[i];
            return null;
        }

        public int CountOwned(int playerId)
        {
            int n = 0;
            for (int i = 0; i < planets.Count; i++) if (planets[i].ownerId == playerId) n++;
            return n;
        }

        // ── Turn economy ────────────────────────────────────────────────────
        /// <summary>Produce on every owned planet (instability-penalized), decay instability, pay fleet
        /// upkeep, reset attack counters, advance the turn.</summary>
        public void EndTurn()
        {
            foreach (var p in planets)
            {
                if (p.ownerId < 0) continue;
                var owner = GetPlayer(p.ownerId);
                if (owner == null) continue;
                float penalty = 1f - p.instabilityLevel * ConquestRules.InstabilityProductionPenalty;
                int produced = (int)Math.Floor(p.resourceProductionRate * Math.Max(0f, penalty));
                owner.AddStock(p.resourceType, produced);
                p.instabilityLevel = Math.Max(0f, p.instabilityLevel - ConquestRules.InstabilityDecayPerTurn);
                if (p.conflictState == ConflictState.UnderAttack) p.conflictState = ConflictState.Contested;
            }
            foreach (var pl in players)
            {
                int upkeep = pl.fleetVesselIds.Count / ConquestRules.VesselsPerUpkeepFlux;
                pl.flux = Math.Max(0, pl.flux - upkeep);
                pl.attacksThisTurn = 0;
            }
            turn++;
        }

        // ── Build actions (validated; false = rejected, state untouched) ────
        public bool BuildDefense(int playerId, string planetId, string defenseId)
        {
            var planet = GetPlanet(planetId);
            var player = GetPlayer(playerId);
            var spec = ConquestCatalog.Defense(defenseId);
            if (planet == null || player == null || spec == null) return false;
            if (planet.ownerId != playerId) return false;
            var d = spec.Value;
            if (!player.CanAfford(d.CostFlux, d.CostAlloy, d.CostBloom)) return false;
            if (planet.builtDefenseIds.Contains(defenseId)) return false; // one of each per planet

            player.Pay(d.CostFlux, d.CostAlloy, d.CostBloom);
            planet.builtDefenseIds.Add(defenseId);
            planet.defenseLevel += d.DefenseBonus;
            planet.bloomContaminationLevel = Math.Min(1f, planet.bloomContaminationLevel + d.ContaminationAdded);
            return true;
        }

        public bool BuildVessel(int playerId, string vesselId)
        {
            var player = GetPlayer(playerId);
            var spec = ConquestCatalog.Vessel(vesselId);
            if (player == null || spec == null) return false;
            var v = spec.Value;
            if (!player.CanAfford(v.CostFlux, v.CostAlloy, v.CostBloom)) return false;

            player.Pay(v.CostFlux, v.CostAlloy, v.CostBloom);
            player.fleetVesselIds.Add(vesselId);
            return true;
        }

        /// <summary>Attacks must come FROM an owned planet INTO an adjacent one you don't own.</summary>
        public bool CanAttack(int playerId, string fromId, string targetId)
        {
            var from = GetPlanet(fromId);
            var target = GetPlanet(targetId);
            var player = GetPlayer(playerId);
            if (from == null || target == null || player == null) return false;
            if (from.ownerId != playerId || target.ownerId == playerId) return false;
            if (!from.adjacentPlanetIds.Contains(targetId)) return false;
            if (player.attacksThisTurn >= ConquestRules.MaxAttacksPerTurn) return false;
            return true;
        }
    }
}
