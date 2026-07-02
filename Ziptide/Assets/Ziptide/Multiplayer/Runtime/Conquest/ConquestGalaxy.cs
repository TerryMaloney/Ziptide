using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>A story world's identity, as the galaxy builder needs it (the Gameplay/Editor side maps
    /// WorldPackDefinitions/layouts to these — this assembly stays Unity-free).</summary>
    [Serializable]
    public struct WorldSeed
    {
        public string WorldId;        // packId (e.g. "dry_cistern")
        public string DisplayName;
        public string Biome;
        public ConquestResource Resource;
        public float ProductionRate;
    }

    /// <summary>
    /// Builds the Tidefront map FROM THE STORY WORLDS (the whole point — a planet matters strategically
    /// because you've walked it). Adjacency = the chapter chain plus cross-links every third node so the
    /// map is a web, not a line. Deterministic: the same seeds produce the same galaxy.
    /// </summary>
    public static class ConquestGalaxy
    {
        /// <summary>Standard 2-player setup: player 0 holds the first world, player 1 the last,
        /// everything between starts neutral with scaling defenses.</summary>
        public static ConquestState BuildTwoPlayer(List<WorldSeed> worlds, string p0Name = "You", string p1Name = "Rival")
        {
            var state = new ConquestState();
            state.players.Add(new ConquestPlayer { playerId = 0, displayName = p0Name });
            state.players.Add(new ConquestPlayer { playerId = 1, displayName = p1Name });

            for (int i = 0; i < worlds.Count; i++)
            {
                var w = worlds[i];
                var node = new PlanetNode
                {
                    planetId = w.WorldId,
                    displayName = w.DisplayName,
                    biomeType = w.Biome,
                    resourceType = w.Resource,
                    resourceProductionRate = w.ProductionRate <= 0f ? 2f : w.ProductionRate,
                    ownerId = i == 0 ? 0 : (i == worlds.Count - 1 ? 1 : -1),
                    defenseLevel = 1 + (i % 3),               // neutral worlds vary in bite
                    stationedDefenseUnits = i == 0 || i == worlds.Count - 1 ? 2 : (i % 2),
                };
                state.planets.Add(node);
            }

            // Chain adjacency (the chapter route)...
            for (int i = 0; i < state.planets.Count - 1; i++)
                Link(state.planets[i], state.planets[i + 1]);
            // ...plus a cross-link every 3rd node so there are flanking routes.
            for (int i = 0; i + 3 < state.planets.Count; i += 3)
                Link(state.planets[i], state.planets[i + 3]);

            return state;
        }

        private static void Link(PlanetNode a, PlanetNode b)
        {
            if (!a.adjacentPlanetIds.Contains(b.planetId)) a.adjacentPlanetIds.Add(b.planetId);
            if (!b.adjacentPlanetIds.Contains(a.planetId)) b.adjacentPlanetIds.Add(a.planetId);
        }

        /// <summary>The Ch.1–2 story galaxy (W001–W012) — ids/biomes/resources match the shipped worlds.
        /// The Gameplay side can also build this straight from the WorldPack assets; this canonical seed
        /// list keeps the pure sim testable and is the hotseat default map.</summary>
        public static List<WorldSeed> ChapterOneTwoSeeds() => new List<WorldSeed>
        {
            new WorldSeed { WorldId = "toxic_city",      DisplayName = "Toxic Venice",     Biome = "city",        Resource = ConquestResource.Alloy, ProductionRate = 3f },
            new WorldSeed { WorldId = "dry_cistern",     DisplayName = "The Dry Cistern",  Biome = "underground", Resource = ConquestResource.Alloy, ProductionRate = 2f },
            new WorldSeed { WorldId = "glass_shelf",     DisplayName = "Glass Shelf",      Biome = "exterior",    Resource = ConquestResource.Flux,  ProductionRate = 2f },
            new WorldSeed { WorldId = "broadcast_tomb",  DisplayName = "The Broadcast Tomb", Biome = "interior",  Resource = ConquestResource.Flux,  ProductionRate = 2f },
            new WorldSeed { WorldId = "oxidized_canopy", DisplayName = "Oxidized Canopy",  Biome = "forest",      Resource = ConquestResource.Bloommatter, ProductionRate = 2f },
            new WorldSeed { WorldId = "mirror_flats",    DisplayName = "The Mirror Flats", Biome = "exterior",    Resource = ConquestResource.Flux,  ProductionRate = 2f },
            new WorldSeed { WorldId = "sable_station",   DisplayName = "Sable Station",    Biome = "station",     Resource = ConquestResource.Flux,  ProductionRate = 3f },
            new WorldSeed { WorldId = "sealed_archive",  DisplayName = "The Sealed Archive", Biome = "interior",  Resource = ConquestResource.Alloy, ProductionRate = 2f },
            new WorldSeed { WorldId = "chitinwall",      DisplayName = "Chitinwall",       Biome = "city",        Resource = ConquestResource.Bloommatter, ProductionRate = 3f },
            new WorldSeed { WorldId = "tidal_array",     DisplayName = "Tidal Array",      Biome = "coastal",     Resource = ConquestResource.Flux,  ProductionRate = 2f },
            new WorldSeed { WorldId = "the_hum",         DisplayName = "The Hum",          Biome = "underground", Resource = ConquestResource.Alloy, ProductionRate = 2f },
            new WorldSeed { WorldId = "maras_last_jump", DisplayName = "Mara's Last Jump", Biome = "void",        Resource = ConquestResource.Flux,  ProductionRate = 3f },
        };
    }
}
