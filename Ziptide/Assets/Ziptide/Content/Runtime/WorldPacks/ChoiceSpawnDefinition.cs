using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A two-option story choice set-piece authored as PACK DATA (GAME_PLAN M1): JobDirector spawns a
    /// ChoiceStation for each entry at scene start. Selecting an option writes its flag (use
    /// ZiptideFlags constants — e.g. the W063 ending branch, PLAYER_HELPED_* alignments) and locks the
    /// station; RILL reacts through its FlagSet lines. Covers every branch beat in the 80-world catalog.
    /// </summary>
    [Serializable]
    public class ChoiceSpawnDefinition
    {
        [Tooltip("Stable id for logs; also naming the spawned station.")]
        public string choiceId = "choice";

        [Tooltip("The question posed above the two panels.")]
        public string prompt = "Choose.";

        [Tooltip("Left option label.")]
        public string labelA = "A";
        [Tooltip("Flag written when the left option is chosen (ZiptideFlags constant).")]
        public string flagA = "";

        [Tooltip("Right option label.")]
        public string labelB = "B";
        [Tooltip("Flag written when the right option is chosen (ZiptideFlags constant).")]
        public string flagB = "";

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;
    }
}
