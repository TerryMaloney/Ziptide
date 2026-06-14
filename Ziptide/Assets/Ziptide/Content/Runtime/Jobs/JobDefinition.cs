using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven job/tutorial: title and ordered steps (GoToMarker, Collect, Deliver, ShootTargets).
    /// </summary>
    public class JobDefinition : ScriptableObject
    {
        [Tooltip("Unique job id within the pack.")]
        public string jobId = "job_01";

        [Tooltip("Display title (e.g. 'First delivery').")]
        public string title = "Job";

        [Tooltip("Ordered steps. Use GoToMarker, CollectItemIdCount, DeliverToSocket, ShootTargetsCount assets.")]
        public List<JobStepDefinition> steps = new List<JobStepDefinition>();
    }
}
