using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    public enum CreatureArchetype { Swarmer, WallCrawler, Flyer, Bruiser }

    [CreateAssetMenu(fileName = "CreatureDefinition", menuName = "Ziptide/Definitions/Creature")]
    public class CreatureDefinition : Definition
    {
        public CreatureArchetype archetype = CreatureArchetype.Swarmer;

        public float maxHealth = 30f;
        public float moveSpeed = 3f;
        public float damage = 5f;

        [Tooltip("Biome this creature is native to (BiomeDefinition.id).")]
        public string biomeId = "";

        [Tooltip("Loot dropped on death.")]
        public List<ResourceCost> loot = new List<ResourceCost>();

        [Tooltip("Can it be disabled by the taser (IShockable)?")]
        public bool shockable = true;
    }
}
