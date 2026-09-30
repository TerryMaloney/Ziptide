using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Base class for a single job step. Use derived types for GoToMarker, Collect, Deliver, ShootTargets.
    /// </summary>
    public abstract class JobStepDefinition : ScriptableObject
    {
        [Tooltip("Stable ID within the job. Preserve across bakes; never derive from step order or display text.")]
        public string stepId = "";

        [Tooltip("Short label for UI (e.g. 'Go to plaza').")]
        public string stepLabel = "Step";
    }
}
