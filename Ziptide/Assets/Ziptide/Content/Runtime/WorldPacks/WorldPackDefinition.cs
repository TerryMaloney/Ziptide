using System.Collections.Generic;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven world pack: scene name, themes, jobs, and spawn markers. Used by JobDirector and travel.
    /// </summary>
    public class WorldPackDefinition : ScriptableObject
    {
        [Tooltip("Unique pack id (e.g. d0_city, test_room).")]
        public string packId = "world";

        [Tooltip("Display name for travel doors. Falls back to packId if empty.")]
        public string displayName = "";

        [Tooltip("Scene name to load (must match name in Build Settings).")]
        public string sceneName = "Main";

        [Header("Themes")]
        [Tooltip("Theme applied when entering this world.")]
        public VisualThemeProfile defaultTheme;

        [Tooltip("Themes available at theme switch in this world.")]
        public List<VisualThemeProfile> availableThemes = new List<VisualThemeProfile>();

        [Header("Jobs")]
        [Tooltip("Jobs offered in this world (e.g. at DispatchKiosk).")]
        public List<JobDefinition> jobs = new List<JobDefinition>();

        [Header("Spawn markers")]
        [Tooltip("Named spawn points for GoToMarker steps and travel spawn.")]
        public List<SpawnMarkerDefinition> spawnMarkers = new List<SpawnMarkerDefinition>();

        [Header("Collectibles")]
        [Tooltip("Physical pickups spawned at scene start (collect steps / Transmission fragments). " +
                 "Pure data — JobDirector materializes CollectibleRuntime objects, like spawnMarkers.")]
        public List<CollectibleSpawnDefinition> collectibles = new List<CollectibleSpawnDefinition>();

        [Header("Choices")]
        [Tooltip("Two-option story set-pieces spawned at scene start (branch beats / endings). " +
                 "Pure data — JobDirector materializes ChoiceStation objects, like spawnMarkers.")]
        public List<ChoiceSpawnDefinition> choices = new List<ChoiceSpawnDefinition>();

        [Header("Audio")]
        [Tooltip("Background music for this world. Null = silence.")]
        public AudioProfile audioProfile;

        [Header("Story gating")]
        [Tooltip("Flags (use Ziptide.Core.ZiptideFlags constants) that must ALL be set before this world " +
                 "is available. Empty = always available. Checked by WorldGating.MeetsRequirements.")]
        public List<string> flagsRequired = new List<string>();

        [Tooltip("Flags set when this world's content is completed (e.g. RILL beat + Signal threshold + " +
                 "W###_COMPLETE). Granted by WorldGating.GrantWorldFlags on job completion. These are the " +
                 "world-level flags that a single JobDefinition.completionFlag can't all carry.")]
        public List<string> flagsGranted = new List<string>();
    }
}
