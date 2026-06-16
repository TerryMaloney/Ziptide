using System.Collections.Generic;
using UnityEngine;

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
    }
}
