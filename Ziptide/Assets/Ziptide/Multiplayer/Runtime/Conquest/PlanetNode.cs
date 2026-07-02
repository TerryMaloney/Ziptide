using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Conquest
{
    public enum ConquestResource { Flux, Alloy, Bloommatter }

    public enum ConflictState { Peaceful, Contested, UnderAttack }

    /// <summary>
    /// One planet on the Tidefront map — the 14-field spec from docs/10_TIDEFRONT.md verbatim (plus the
    /// built-structure list the spec implies). The galaxy IS the story worlds: each node is built from a
    /// shipped world's identity (see ConquestGalaxy). Pure + [Serializable] with Lists (no Dictionary)
    /// so the Gameplay-side save can JsonUtility it, same as PlayerProfile.
    /// </summary>
    [Serializable]
    public class PlanetNode
    {
        public string planetId = "";
        public string displayName = "";
        public string biomeType = "";
        public int ownerId = -1;                      // -1 = neutral
        public ConquestResource resourceType = ConquestResource.Alloy;
        public float resourceProductionRate = 2f;     // per turn, before instability penalty
        public int defenseLevel = 1;
        public int orbitalShieldLevel = 0;
        public int stationedDefenseUnits = 0;
        public string specialTraitId = "";
        public List<string> adjacentPlanetIds = new List<string>();
        public ConflictState conflictState = ConflictState.Peaceful;
        public float instabilityLevel = 0f;           // 0..1 — fresh captures produce less
        public float bloomContaminationLevel = 0f;    // 0..1 — Bloom Barrier's price
        public List<string> builtDefenseIds = new List<string>();
    }
}
