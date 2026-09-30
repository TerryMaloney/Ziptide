using System;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>Adapter owned by JobDirector for one explicitly configured campaign contract.
    /// All logical and physical mutations settle before Commit; disk IO remains SaveSystem's job.</summary>
    public sealed class MissionCheckpointSession
    {
        public JobRuntime Runtime { get; } = new JobRuntime();
        public PlayerProfile Profile { get; private set; }
        public string WorldId { get; private set; }
        public string RunId { get; private set; }
        private JobDefinition _job;
        private IList<string> _worldFlags;
        private readonly Dictionary<string, CollectibleSpawnDefinition> _pickups = new Dictionary<string, CollectibleSpawnDefinition>();
        private readonly HashSet<string> _consumed = new HashSet<string>();
        private readonly Dictionary<string, int> _repairs = new Dictionary<string, int>();
        private string _lastCommitted;

        public static bool TryOpen(PlayerProfile profile, string worldId, WorldPackDefinition pack,
            out MissionCheckpointSession session, out string error)
        {
            session = null; error = "MISSION_CONFIGURATION_INVALID";
            if (profile == null || profile.worlds == null || profile.flags == null || profile.resources == null || string.IsNullOrWhiteSpace(worldId) || pack == null ||
                pack.jobs == null || pack.jobs.Count != 1 || pack.jobs[0] == null) return false;
            var job = pack.jobs[0];
            if (job.replayPolicy != MissionReplayPolicy.OneTime) return false;
            var candidate = new MissionCheckpointSession
            { Profile = profile, WorldId = worldId, RunId = job.jobId + ":campaign", _job = job, _worldFlags = pack.flagsGranted };
            var probe = new JobRuntime(); probe.StartJob(job);
            if (!probe.TryCaptureCheckpoint(worldId, candidate.RunId, out var completeTemplate, out error)) return false;
            error = "MISSION_PLACEMENT_INVALID";
            if (pack.collectibles != null)
                foreach (var pickup in pack.collectibles)
                {
                    if (pickup == null || string.IsNullOrWhiteSpace(pickup.placementId) || string.IsNullOrWhiteSpace(pickup.itemId) ||
                        candidate._pickups.ContainsKey(pickup.placementId)) return false;
                    candidate._pickups.Add(pickup.placementId, pickup);
                }
            error = "MISSION_MACHINE_INVALID";
            if (pack.machines != null)
                foreach (var machine in pack.machines)
                {
                    if (machine == null || string.IsNullOrWhiteSpace(machine.machineId) || candidate._repairs.ContainsKey(machine.machineId)) return false;
                    candidate._repairs.Add(machine.machineId, 0);
                }
            // This rollout supports pack-authored collect/repair work only. Reject a missing source
            // before enabling persistence, rather than turning that omission into a saved soft lock.
            var issues = WorldPackValidator.Validate(pack);
            if (issues.Count > 0) { error = "MISSION_PACK_INVALID: " + issues[0]; return false; }
            WorldState world = null;
            foreach (var item in profile.worlds)
            {
                if (item == null) { error = "MISSION_WORLD_INVALID"; return false; }
                if (item.worldId != worldId) continue;
                if (world != null) { error = "MISSION_WORLD_DUPLICATE"; return false; }
                world = item;
            }
            JobCheckpoint saved = null;
            if (world?.jobCheckpoints != null)
                foreach (var checkpoint in world.jobCheckpoints)
                {
                    if (checkpoint == null) { error = "MISSION_CHECKPOINT_INVALID"; return false; }
                    if (checkpoint.runId != candidate.RunId && checkpoint.jobId != job.jobId) continue;
                    if (saved != null) { error = "MISSION_CHECKPOINT_DUPLICATE"; return false; }
                    saved = checkpoint;
                }
            if (saved != null)
            {
                if (!candidate.RestorePhysical(saved, out error)) return false;
                if (!candidate.Runtime.TryRestoreCheckpoint(saved.jobId == "" ? null : job,
                    worldId, candidate.RunId, saved, out error)) return false;
                if (!candidate.PhysicalProgressAgrees(saved)) { error = "MISSION_STATE_DIVERGED"; return false; }
                candidate._lastCommitted = JsonUtility.ToJson(saved);
            }
            else
            {
                bool hasReceipt = false;
                if (world?.missionReceipts != null)
                    foreach (var receipt in world.missionReceipts)
                        if (receipt != null && receipt.jobId == job.jobId) hasReceipt = true;
                bool legacyCompleted = !string.IsNullOrEmpty(job.completionFlag) && profile.HasFlag(job.completionFlag);
                if (hasReceipt || legacyCompleted)
                {
                    // An explicitly one-time campaign flag is migrated without awarding money.
                    // Do not replay the job's actions just to reconstruct completion.
                    completeTemplate.currentStepId = job.steps[job.steps.Count - 1].stepId;
                    completeTemplate.isComplete = true;
                    if (!candidate.Runtime.TryRestoreCheckpoint(job, worldId, candidate.RunId, completeTemplate, out error)) return false;
                    foreach (var id in candidate._pickups.Keys) candidate._consumed.Add(id);
                    var ids = new List<string>(candidate._repairs.Keys);
                    foreach (var id in ids) candidate._repairs[id] = 3;
                    if (!MissionRewards.TryGrant(profile, job, worldId, candidate.RunId, pack.flagsGranted,
                        out _, out error, importLegacy: !hasReceipt)) return false;
                }
            }
            session = candidate; error = null; return true;
        }

        public void StartOrResume()
        {
            if (Runtime.Definition == null) Runtime.StartJob(_job);
        }
        public bool IsConsumed(string placementId) => placementId != null && _consumed.Contains(placementId);
        public int RepairStageFor(string machineId) => machineId != null && _repairs.TryGetValue(machineId, out int value) ? value : 0;
        public bool Collect(string placementId)
        {
            if (placementId == null || !_pickups.TryGetValue(placementId, out var pickup) || !_consumed.Add(placementId)) return false;
            Profile.SetFlag(pickup.flagOnCollect);
            TransmissionProgress.SyncClarityFlags(Profile);
            Runtime.ReportCollect(pickup.itemId);
            return true;
        }
        public bool SetRepairStage(string machineId, int stage)
        {
            if (machineId == null || !_repairs.TryGetValue(machineId, out int previous) || stage < 0 || stage > 3 || stage <= previous) return false;
            _repairs[machineId] = stage;
            if (stage == 3) Runtime.ReportRepair(machineId);
            return true;
        }
        public bool MatchesProfile(PlayerProfile profile) => ReferenceEquals(Profile, profile);

        public bool TryCommit(out bool changed, out string error)
        {
            changed = false;
            if (!Runtime.TryCaptureCheckpoint(WorldId, RunId, out var snapshot, out error)) return false;
            snapshot.consumedPickupIds = new List<string>(_consumed); snapshot.consumedPickupIds.Sort(StringComparer.Ordinal);
            var ids = new List<string>(_repairs.Keys); ids.Sort(StringComparer.Ordinal);
            foreach (var id in ids) snapshot.repairStages.Add(new JobCheckpointCount { id = id, count = _repairs[id] });
            string json = JsonUtility.ToJson(snapshot);
            // Always check completion receipts, even after opening a completed checkpoint. This can
            // recover an unpaid completion after a prior rejected reward batch; it never repays a receipt.
            bool receiptChanged = false;
            if (Runtime.IsComplete)
            {
                if (!MissionRewards.TryGrant(Profile, _job, WorldId, RunId, _worldFlags,
                    out var result, out error, nowUnix: DateTimeOffset.UtcNow.ToUnixTimeSeconds())) return false;
                receiptChanged = result == MissionRewardResult.Granted || result == MissionRewardResult.ImportedLegacy;
                TransmissionProgress.SyncClarityFlags(Profile);
            }
            if (json == _lastCommitted && !receiptChanged) return true;
            var world = Profile.GetWorld(WorldId, true);
            if (world.jobCheckpoints == null) world.jobCheckpoints = new List<JobCheckpoint>();
            int index = world.jobCheckpoints.FindIndex(c => c != null && c.runId == RunId);
            if (index < 0) world.jobCheckpoints.Add(snapshot); else world.jobCheckpoints[index] = snapshot;
            _lastCommitted = json; changed = true; error = null; return true;
        }

        private bool PhysicalProgressAgrees(JobCheckpoint snapshot)
        {
            var collected = new Dictionary<string, long>();
            foreach (var id in _consumed)
            {
                string item = _pickups[id].itemId;
                collected.TryGetValue(item, out long n); collected[item] = n + 1;
            }
            var required = new Dictionary<string, long>();
            var repaired = new Dictionary<string, long>();
            if (Runtime.Definition != null)
                for (int i = 0; i < _job.steps.Count; i++)
                {
                    bool finished = Runtime.IsComplete || i < Runtime.CurrentStepIndex;
                    bool current = !Runtime.IsComplete && i == Runtime.CurrentStepIndex;
                    if (_job.steps[i] is CollectItemIdCountStepDefinition collect)
                    {
                        required.TryGetValue(collect.itemId, out long n);
                        required[collect.itemId] = n + (finished ? collect.count : current ? Runtime.CollectProgress : 0);
                    }
                    if (_job.steps[i] is RepairMachineCountStepDefinition repair)
                    {
                        string id = repair.machineId ?? "";
                        repaired.TryGetValue(id, out long n);
                        repaired[id] = n + (finished ? repair.count : current ? Runtime.RepairProgress : 0);
                    }
                }
            foreach (var entry in snapshot.collectBank)
            { required.TryGetValue(entry.id, out long n); required[entry.id] = n + entry.count; }
            foreach (var entry in snapshot.repairBank)
            { repaired.TryGetValue(entry.id, out long n); repaired[entry.id] = n + entry.count; }
            foreach (var entry in required)
            {
                collected.TryGetValue(entry.Key, out long have);
                if (entry.Value > have) return false;
            }
            if (!Runtime.IsComplete)
                foreach (var entry in collected)
                { required.TryGetValue(entry.Key, out long need); if (entry.Value != need) return false; }
            long totalNeeded = 0, running = 0;
            foreach (var stage in _repairs.Values) if (stage == 3) running++;
            foreach (var entry in repaired)
            {
                totalNeeded += entry.Value;
                if (entry.Key != "" && entry.Value > (RepairStageFor(entry.Key) == 3 ? 1 : 0)) return false;
            }
            return Runtime.IsComplete ? totalNeeded <= running : totalNeeded == running;
        }

        private bool RestorePhysical(JobCheckpoint snapshot, out string error)
        {
            error = "MISSION_PHYSICAL_STATE_INVALID";
            if (snapshot.consumedPickupIds == null || snapshot.repairStages == null) return false;
            foreach (var id in snapshot.consumedPickupIds)
                if (id == null || !_pickups.ContainsKey(id) || !_consumed.Add(id)) return false;
            var seen = new HashSet<string>();
            foreach (var repair in snapshot.repairStages)
            {
                if (repair == null || repair.id == null || !_repairs.ContainsKey(repair.id) || !seen.Add(repair.id) ||
                    repair.count < 0 || repair.count > 3) return false;
                _repairs[repair.id] = repair.count;
            }
            if (seen.Count != _repairs.Count) return false;
            error = null; return true;
        }
    }
}
