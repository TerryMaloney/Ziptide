using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Content
{
    [CreateAssetMenu(fileName = "RecipeDefinition", menuName = "Ziptide/Definitions/Recipe")]
    public class RecipeDefinition : Definition
    {
        [Tooltip("Resources consumed to craft / build / repair.")]
        public List<ResourceCost> costs = new List<ResourceCost>();

        [Tooltip("Real seconds to complete (0 = instant).")]
        public double craftSeconds = 0;

        [Tooltip("Resource / item / machine id produced (optional).")]
        public string producesId = "";

        [Tooltip("Amount produced.")]
        public double producesAmount = 1;

        // ── META-LOOP D (additive; defaults keep every existing recipe/caller unchanged) ──────────
        [Header("Factory (Meta-Loop)")]
        [Tooltip("Machine type that can run this recipe (None = hand-craft / legacy cost-recipe).")]
        public MachineType requiredMachineType = MachineType.None;
        [Tooltip("Production-graph ticks to complete (0 = derive from craftSeconds).")]
        public int durationTicks = 0;
        [Tooltip("Profile flag required before this recipe is usable (empty = always).")]
        public string unlockFlag = "";
        [Tooltip("Staleness: worlds/story tags this recipe belongs to.")]
        public List<string> sourceWorlds = new List<string>();
        public List<string> storyTags = new List<string>();
        [Header("Mode reach")]
        public bool campaignUse = true;
        public bool multiplayerUse = false;
    }
}
