using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Data-only description of a single pod's narrative content. This is a SCHEMA: it carries
    /// no engine logic and references no concrete prefabs/materials. Story/lore here is content
    /// and may be swapped or removed without affecting traversal/building (see 00_LOCKED_CONTRACTS).
    /// </summary>
    [CreateAssetMenu(fileName = "PodNarrative", menuName = "Ziptide/Pod Narrative", order = 20)]
    public class PodNarrative : ScriptableObject
    {
        [Tooltip("Stable machine id used to look this pod up, e.g. \"pod_001\". Not shown to players.")]
        public string podId;

        [Tooltip("Human-readable label. Content — safe to localize or replace.")]
        public string displayName;

        [TextArea(1, 6)]
        [Tooltip("Narrative / voice lines for this pod. Pure content; the engine never branches on the literal text.")]
        public string[] narrativeLines = new string[0];

        [Tooltip("Theme requested by this pod, addressed by id (NOT a prefab or material). " +
                 "Resolved by a theme registry / IThemeProvider. Leave empty to inherit the world default.")]
        public string themeId;
    }
}
