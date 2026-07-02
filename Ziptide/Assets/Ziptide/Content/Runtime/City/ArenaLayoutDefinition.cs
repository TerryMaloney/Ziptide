using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven PvP arena (M7a A2 — design docs/design/PVP_ARENA_AAA.md §A2): geometry, bot
    /// navigation, spawns, objective zones (A3 modes), weapon pads (A4 arsenal), hazards (mutators),
    /// and sky identity. ScenePatcherArena consumes it exactly like CityBuilder consumes
    /// CityLayoutDefinition — edit the asset (or author a new one in ArenaLayoutLibrary) to re-shape
    /// or add an arena with zero patcher code.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Arena Layout", fileName = "ArenaLayout")]
    public class ArenaLayoutDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string arenaId = "arena";
        [Tooltip("Scene to generate (Scenes/Arenas/<sceneName>.unity). Must be unique.")]
        public string sceneName = "";
        public string displayName = "";
        [Tooltip("BotProfileDefinition name in Resources/Bots (rookie/regular/veteran/nightmare).")]
        public string botDifficulty = "regular";

        [Header("Sky / mood (per-arena identity, same model as world layouts)")]
        public Color skyHorizonColor = new Color(0.30f, 0.32f, 0.38f);
        public Color skyTopColor = new Color(0.12f, 0.13f, 0.18f);
        public Color groundTint = new Color(0.30f, 0.31f, 0.34f);
        public bool planetVisible = false;
        public Color planetBaseColor = new Color(0.4f, 0.5f, 0.7f);
        public Color planetAccentColor = new Color(0.25f, 0.35f, 0.5f);
        public float planetAngularSize = 12f;
        public bool fogEnabled = false;
        public Color fogColor = new Color(0.15f, 0.16f, 0.2f);
        public float fogDensity = 0.02f;

        [Header("Palette")]
        public Color floorColor = new Color(0.26f, 0.27f, 0.30f);
        public Color wallColor = new Color(0.32f, 0.33f, 0.37f);
        public Color coverColor = new Color(0.45f, 0.30f, 0.20f);
        public Color platformColor = new Color(0.30f, 0.34f, 0.38f);

        [Header("Geometry")]
        [Tooltip("Floor footprint (X × Z). Perimeter walls are derived from it.")]
        public Vector2 floorSize = new Vector2(34f, 34f);
        public float wallHeight = 5f;
        public List<ArenaBlockDef> platforms = new List<ArenaBlockDef>();
        public List<ArenaRampDef> ramps = new List<ArenaRampDef>();
        public List<ArenaBlockDef> covers = new List<ArenaBlockDef>();
        public List<ArenaWallDef> breakableWalls = new List<ArenaWallDef>();

        [Header("Spawns")]
        public Vector3 playerSpawn = new Vector3(0f, 0.1f, -12f);
        public Vector3 botSpawn = new Vector3(0f, 1.2f, 12f);   // capsule pivot at center — keep ~1.1m up

        [Header("Bot navigation (Way_*/Cover_P* — consumed by PvpBot's brain)")]
        public List<Vector3> waypoints = new List<Vector3>();
        public List<Vector3> coverPoints = new List<Vector3>();

        [Header("Mode data (A3) & weapon pads (A4)")]
        public List<ObjectiveZoneDef> objectiveZones = new List<ObjectiveZoneDef>();
        public List<WeaponPadDef> weaponPads = new List<WeaponPadDef>();

        [Header("Hazard mutators (same defs the worlds use)")]
        public List<HazardZoneDef> hazards = new List<HazardZoneDef>();

        /// <summary>Structural sanity (pure — the patcher skips invalid arenas loudly; tests run this).</summary>
        public List<string> Validate()
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(arenaId)) issues.Add("empty arenaId");
            if (string.IsNullOrEmpty(sceneName)) issues.Add("empty sceneName");
            if (floorSize.x < 20f || floorSize.y < 20f) issues.Add("floor under 20m — too small to fight in");
            if (waypoints.Count < 4) issues.Add("fewer than 4 bot waypoints");
            if (Mathf.Abs(playerSpawn.x) > floorSize.x / 2f || Mathf.Abs(playerSpawn.z) > floorSize.y / 2f)
                issues.Add("player spawn outside the floor");
            if (Mathf.Abs(botSpawn.x) > floorSize.x / 2f || Mathf.Abs(botSpawn.z) > floorSize.y / 2f)
                issues.Add("bot spawn outside the floor");
            float dx = playerSpawn.x - botSpawn.x, dz = playerSpawn.z - botSpawn.z;
            if (Math.Sqrt(dx * dx + dz * dz) < 12f) issues.Add("spawns closer than 12m — instant fights");
            foreach (var pad in weaponPads)
                if (pad != null && string.IsNullOrEmpty(pad.itemId)) issues.Add("weapon pad with empty itemId");
            return issues;
        }
    }

    [Serializable]
    public class ArenaBlockDef
    {
        public string blockName = "Block";
        public Vector3 position = Vector3.zero;
        public Vector3 size = new Vector3(2f, 1.8f, 2f);
    }

    [Serializable]
    public class ArenaRampDef
    {
        public string rampName = "Ramp";
        public Vector3 position = Vector3.zero;
        public Vector3 size = new Vector3(4f, 0.4f, 6f);
        public float tiltX = 15f;
        public float rotY = 0f;
    }

    [Serializable]
    public class ArenaWallDef
    {
        public string wallName = "BreakWall";
        public Vector3 position = Vector3.zero;
        public float rotY = 0f;
        public Vector3 size = new Vector3(4f, 3f, 0.3f);
    }

    /// <summary>A mode objective (KotH hill, fragment base…). A3 consumes; harmless data until then.</summary>
    [Serializable]
    public class ObjectiveZoneDef
    {
        public string zoneId = "hill_a";
        public Vector3 position = Vector3.zero;
        public float radius = 3f;
    }

    /// <summary>A weapon pickup spot. A2 spawns the item there once; A4 turns pads into respawners.</summary>
    [Serializable]
    public class WeaponPadDef
    {
        public string itemId = "taser_dart_gun";
        public Vector3 position = Vector3.zero;
        public float respawnSeconds = 20f;
    }
}
