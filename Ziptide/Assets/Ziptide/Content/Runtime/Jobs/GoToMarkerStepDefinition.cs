using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: go to a named marker within a distance.
    /// </summary>
    public class GoToMarkerStepDefinition : JobStepDefinition
    {
        [Tooltip("SpawnMarkerDefinition.markerId to go to.")]
        public string markerId = "plaza";

        [Tooltip("Distance in meters to count as arrived.")]
        public float arriveDistance = 1.5f;
    }
}
