using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Base class for a single job step. Use derived types for GoToMarker, Collect, Deliver, ShootTargets.
    /// </summary>
    public abstract class JobStepDefinition : ScriptableObject
    {
        [Tooltip("Short label for UI (e.g. 'Go to plaza').")]
        public string stepLabel = "Step";
    }
}
