using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: disable (shock) a number of drones.
    /// </summary>
    public class DisableDronesCountStepDefinition : JobStepDefinition
    {
        [Tooltip("Number of drones to disable to complete step.")]
        public int count = 3;
    }
}
