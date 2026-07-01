using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A physical pickup authored as PACK DATA (GAME_PLAN M1): JobDirector spawns a grabbable
    /// CollectibleRuntime for each entry at scene start (the Marker_&lt;id&gt; pattern — pure data, no
    /// scene objects, survives regeneration). Collecting it credits CollectItemIdCount job steps and
    /// optionally sets a story flag — this is how Transmission fragments become real objects you grab.
    /// </summary>
    [Serializable]
    public class CollectibleSpawnDefinition
    {
        [Tooltip("Item id — must match the job's CollectItemIdCountStepDefinition.itemId to count.")]
        public string itemId = "sample";

        [Tooltip("Short label shown floating over the pickup (blank = derived from itemId).")]
        public string displayName = "";

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;

        [Tooltip("Optional story flag set the moment it's collected (e.g. FRAGMENT_T1_FOUND). Blank = none.")]
        public string flagOnCollect = "";

        [Tooltip("Glow/accent color of the pickup (alpha 0 = default cyan).")]
        public Color accentColor = new Color(0f, 0f, 0f, 0f);
    }
}
