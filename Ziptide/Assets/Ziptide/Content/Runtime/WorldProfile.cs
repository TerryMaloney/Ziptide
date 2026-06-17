using System.Collections.Generic;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven world config: spawn, play area, respawn rules, and theme list for switching.
    /// </summary>
    public class WorldProfile : ScriptableObject
    {
        [Header("Spawn")]
        public Vector3 spawnPosition = Vector3.zero;
        public Vector3 spawnEuler = Vector3.zero;

        [Header("Play Area")]
        [Tooltip("Width (X) and length (Z) of the play area in meters.")]
        public Vector2 playAreaSize = new Vector2(4f, 4f);
        public float groundY = 0f;
        [Tooltip("Build invisible boundary walls around the play area. OFF for open worlds — the " +
                 "global fall-safety net handles falling. Only enable for a true roomscale box.")]
        public bool usePlayAreaBounds = false;

        [Header("Respawn")]
        public bool respawnOnFall = true;
        [Tooltip("When player Y drops below this, respawn triggers.")]
        public float fallYThreshold = -2f;
        [Tooltip("Optional delay/fade before respawn (0 = instant).")]
        public float respawnFadeSeconds = 0f;

        [Header("Themes")]
        public VisualThemeProfile defaultTheme;
        [Tooltip("Themes available at the Theme Switch Station.")]
        public List<VisualThemeProfile> availableThemes = new List<VisualThemeProfile>();
    }
}
