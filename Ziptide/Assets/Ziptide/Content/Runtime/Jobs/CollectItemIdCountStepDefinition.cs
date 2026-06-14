using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: collect a count of items by itemId (e.g. pick up collectibles).
    /// </summary>
    public class CollectItemIdCountStepDefinition : JobStepDefinition
    {
        [Tooltip("Item id to collect (must match ItemRuntime definition).")]
        public string itemId = "crate";

        [Tooltip("Number required to complete step.")]
        public int count = 1;
    }
}
