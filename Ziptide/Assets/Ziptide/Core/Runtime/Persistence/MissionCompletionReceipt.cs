using System;

namespace Ziptide.Core
{
    public enum MissionReplayPolicy { Unspecified = 0, OneTime = 1, Repeatable = 2 }

    /// <summary>Permanent evidence of completion, independent of the capped economy ledger.
    /// World scope is the containing WorldState. Legacy imports acknowledge completion without pay.</summary>
    [Serializable]
    public class MissionCompletionReceipt
    {
        public string jobId = "";
        public string runId = "";
        public MissionReplayPolicy policy;
        public int definitionRevision;
        public long atUnix;
        public bool importedLegacy;
    }
}
