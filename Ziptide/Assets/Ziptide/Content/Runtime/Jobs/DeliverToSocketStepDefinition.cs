using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: deliver a count of items to a socket (e.g. delivery cradle).
    /// </summary>
    public class DeliverToSocketStepDefinition : JobStepDefinition
    {
        [Tooltip("Socket id (e.g. delivery_cradle) that accepts the delivery.")]
        public string socketId = "delivery_cradle";

        [Tooltip("Item id that must be delivered.")]
        public string itemId = "crate";

        [Tooltip("Number required to complete step.")]
        public int count = 1;
    }
}
