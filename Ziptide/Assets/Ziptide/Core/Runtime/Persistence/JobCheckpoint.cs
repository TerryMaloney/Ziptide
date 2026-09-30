using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>Logical job snapshot only. The world/run scope is supplied by the owning adapter;
    /// this is not a payout receipt or a snapshot of physical scene objects.</summary>
    [Serializable]
    public class JobCheckpoint
    {
        public const int CurrentVersion = 2;
        public int version = CurrentVersion;
        public string worldId = "";
        public string runId = "";
        public string jobId = ""; // empty means only pre-accept banks have been captured
        public int definitionRevision;
        public string currentStepId = "";
        public bool isComplete;
        public int progress;
        public List<string> consumedPickupIds = new List<string>();
        public List<JobCheckpointCount> repairStages = new List<JobCheckpointCount>(); // count is stage 0..3
        public List<JobCheckpointStep> steps = new List<JobCheckpointStep>();
        public List<JobCheckpointCount> collectBank = new List<JobCheckpointCount>();
        public List<JobCheckpointCount> repairBank = new List<JobCheckpointCount>();
    }

    /// <summary>Ordered semantic contract, so unversioned target/count/type/order changes are rejected.</summary>
    [Serializable]
    public class JobCheckpointStep
    {
        public string stepId = "";
        public string kind = "";
        public string targetId = "";
        public string secondaryId = "";
        public int requiredCount;
        public float arriveDistance;
    }

    [Serializable]
    public class JobCheckpointCount
    {
        public string id = "";
        public int count;
    }
}
