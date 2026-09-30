using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven job/tutorial: title and ordered steps (GoToMarker, Collect, Deliver, ShootTargets).
    /// </summary>
    public class JobDefinition : ScriptableObject
    {
        [Tooltip("Unique job id within the pack.")]
        public string jobId = "job_01";

        [Tooltip("Checkpoint contract revision. Zero disables checkpoint capture until stable step IDs are authored.")]
        public int checkpointRevision;

        public MissionReplayPolicy replayPolicy = MissionReplayPolicy.Unspecified;
        public bool grantsWorldCompletion;

        [Tooltip("Display title (e.g. 'First delivery').")]
        public string title = "Job";

        [Tooltip("Ordered steps. Use GoToMarker, CollectItemIdCount, DeliverToSocket, ShootTargetsCount assets.")]
        public List<JobStepDefinition> steps = new List<JobStepDefinition>();

        [Tooltip("Resources granted to the player profile when the job completes (e.g. passage credits). " +
                 "Resolved by JobRewards.Grant — data-driven, so a designer tunes the payout, not code.")]
        public List<ResourceCost> reward = new List<ResourceCost>();

        [Tooltip("Optional PlayerProfile flag set when the job completes (e.g. 'toxiccity_complete'). " +
                 "Blank = no flag. Used by story/progression gates.")]
        public string completionFlag = "";
    }
}
