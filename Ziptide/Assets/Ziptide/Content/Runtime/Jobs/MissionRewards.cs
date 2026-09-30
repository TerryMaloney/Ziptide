using System.Collections.Generic;
using Ziptide.Core;

namespace Ziptide.Content
{
    public enum MissionRewardResult { Rejected, Granted, AlreadyGranted, ImportedLegacy }

    /// <summary>Synchronous completion transaction for explicitly configured jobs. Validate the whole
    /// batch before mutation; the caller persists its checkpoint and this same profile together.</summary>
    public static class MissionRewards
    {
        public static bool TryGrant(PlayerProfile profile, JobDefinition job, string worldId, string runId,
            IList<string> worldFlags, out MissionRewardResult result, out string error,
            bool importLegacy = false, long nowUnix = 0)
        {
            result = MissionRewardResult.Rejected; error = "MISSION_REWARD_INVALID";
            if (profile == null || job == null || profile.worlds == null || profile.resources == null ||
                profile.flags == null || string.IsNullOrWhiteSpace(worldId) || string.IsNullOrWhiteSpace(runId) ||
                string.IsNullOrWhiteSpace(job.jobId) || job.checkpointRevision <= 0 ||
                (job.replayPolicy != MissionReplayPolicy.OneTime && job.replayPolicy != MissionReplayPolicy.Repeatable)) return false;
            WorldState world = null;
            foreach (var candidate in profile.worlds)
            {
                if (candidate == null) { error = "MISSION_WORLD_INVALID"; return false; }
                if (candidate.worldId != worldId) continue;
                if (world != null) { error = "MISSION_WORLD_DUPLICATE"; return false; }
                world = candidate;
            }
            MissionCompletionReceipt existing = null;
            if (world?.missionReceipts != null)
                foreach (var receipt in world.missionReceipts)
                {
                    if (receipt == null || string.IsNullOrWhiteSpace(receipt.jobId) || string.IsNullOrWhiteSpace(receipt.runId))
                    { error = "MISSION_RECEIPT_INVALID"; return false; }
                    if (receipt.jobId != job.jobId) continue;
                    if (receipt.policy != job.replayPolicy) { error = "MISSION_POLICY_CHANGED"; return false; }
                    if (job.replayPolicy == MissionReplayPolicy.OneTime || receipt.runId == runId)
                    {
                        if (existing != null) { error = "MISSION_RECEIPT_DUPLICATE"; return false; }
                        existing = receipt;
                    }
                }
            if (existing != null) { result = MissionRewardResult.AlreadyGranted; error = null; return true; }
            if (importLegacy && (job.replayPolicy != MissionReplayPolicy.OneTime ||
                string.IsNullOrWhiteSpace(job.completionFlag) || !profile.HasFlag(job.completionFlag)))
            { error = "MISSION_LEGACY_PROOF_MISSING"; return false; }

            var totals = new Dictionary<string, double>();
            if (!importLegacy && job.reward != null)
                foreach (var reward in job.reward)
                {
                    if (reward == null || string.IsNullOrWhiteSpace(reward.resourceId) || !Finite(reward.amount) || reward.amount < 0)
                        return false;
                    totals.TryGetValue(reward.resourceId, out double previous);
                    double total = previous + reward.amount;
                    if (!Finite(total)) return false;
                    totals[reward.resourceId] = total;
                }
            foreach (var grant in totals)
            {
                double balance = 0; bool found = false;
                foreach (var entry in profile.resources)
                {
                    if (entry == null) return false;
                    if (entry.id != grant.Key) continue;
                    if (found) return false;
                    found = true; balance = entry.amount;
                }
                if (!Finite(balance) || balance < 0 || !Finite(balance + grant.Value)) return false;
            }
            if (job.grantsWorldCompletion && worldFlags != null)
                foreach (var flag in worldFlags)
                    if (string.IsNullOrWhiteSpace(flag)) { error = "MISSION_WORLD_FLAG_INVALID"; return false; }

            // No callbacks or file IO occur between these mutations. Do not replace existing world
            // objects: mine/garden/belt runtimes may hold their references.
            foreach (var grant in totals)
                if (grant.Value > 0)
                    RewardRouter.Grant(profile, LedgerSource.Campaign, grant.Key, grant.Value,
                        reason: job.jobId, relatedId: runId, worldId: worldId, nowUnix: nowUnix);
            profile.SetFlag(job.completionFlag);
            if (job.grantsWorldCompletion && worldFlags != null)
                foreach (var flag in worldFlags) profile.SetFlag(flag);
            if (world == null) world = profile.GetWorld(worldId, true);
            if (world.missionReceipts == null) world.missionReceipts = new List<MissionCompletionReceipt>();
            world.missionReceipts.Add(new MissionCompletionReceipt
            {
                jobId = job.jobId, runId = runId, policy = job.replayPolicy,
                definitionRevision = job.checkpointRevision, atUnix = nowUnix, importedLegacy = importLegacy,
            });
            result = importLegacy ? MissionRewardResult.ImportedLegacy : MissionRewardResult.Granted;
            error = null; return true;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
