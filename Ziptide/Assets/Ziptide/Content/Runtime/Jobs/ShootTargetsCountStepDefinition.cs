using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: shoot a number of targets (count hits).
    /// </summary>
    public class ShootTargetsCountStepDefinition : JobStepDefinition
    {
        [Tooltip("Number of target hits required to complete step.")]
        public int count = 3;
    }
}
