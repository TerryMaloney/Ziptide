using System.Collections.Generic;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    public partial class JobRuntime
    {
        /// <summary>Capture an independent logical snapshot, including work banked before acceptance.
        /// The caller owns world/run identity and persistence. Legacy definitions must first opt in.</summary>
        public bool TryCaptureCheckpoint(string worldId, string runId, out JobCheckpoint checkpoint, out string error)
        {
            checkpoint = null;
            if (string.IsNullOrWhiteSpace(worldId) || string.IsNullOrWhiteSpace(runId))
            { error = "CHECKPOINT_SCOPE_MISSING"; return false; }
            if (!TryBuildCheckpointContract(Definition, out var steps, out error)) return false;
            var candidate = new JobCheckpoint
            {
                worldId = worldId, runId = runId,
                jobId = Definition != null ? Definition.jobId : "",
                definitionRevision = Definition != null ? Definition.checkpointRevision : 0,
                currentStepId = GetCurrentStep() != null ? GetCurrentStep().stepId : "",
                isComplete = IsComplete,
                progress = CollectProgress + DeliverProgress + ShootProgress + DroneProgress + RepairProgress,
                steps = steps,
                collectBank = CopyCheckpointBank(_collectBank),
                repairBank = CopyCheckpointBank(_repairBank),
            };
            if (!ValidateCheckpoint(Definition, worldId, runId, candidate, out _, out error)) return false;
            checkpoint = candidate;
            return true;
        }

        /// <summary>Validate fully before replacing runtime state. Restoration emits no gameplay,
        /// step or reward events and does not drain banks; callers refresh presentation afterward.</summary>
        public bool TryRestoreCheckpoint(JobDefinition definition, string worldId, string runId,
            JobCheckpoint checkpoint, out string error)
        {
            if (!ValidateCheckpoint(definition, worldId, runId, checkpoint, out int index, out error)) return false;
            Definition = definition;
            CurrentStepIndex = index;
            IsComplete = checkpoint.isComplete;
            CollectProgress = DeliverProgress = ShootProgress = DroneProgress = RepairProgress = 0;
            if (definition != null && !IsComplete)
            {
                switch (checkpoint.steps[index].kind)
                {
                    case "collect": CollectProgress = checkpoint.progress; break;
                    case "deliver": DeliverProgress = checkpoint.progress; break;
                    case "shoot": ShootProgress = checkpoint.progress; break;
                    case "drone": DroneProgress = checkpoint.progress; break;
                    case "repair": RepairProgress = checkpoint.progress; break;
                }
            }
            RestoreCheckpointBank(_collectBank, checkpoint.collectBank);
            RestoreCheckpointBank(_repairBank, checkpoint.repairBank);
            if (IsComplete) StepText = "Complete!";
            else RefreshStepText();
            return true;
        }

        private static bool ValidateCheckpoint(JobDefinition definition, string worldId, string runId,
            JobCheckpoint checkpoint, out int index, out string error)
        {
            index = 0;
            error = "CHECKPOINT_INVALID";
            if (checkpoint == null || checkpoint.version != JobCheckpoint.CurrentVersion) return false;
            if (string.IsNullOrWhiteSpace(worldId) || string.IsNullOrWhiteSpace(runId) ||
                checkpoint.worldId != worldId || checkpoint.runId != runId)
            { error = "CHECKPOINT_SCOPE_MISMATCH"; return false; }
            if (!TryBuildCheckpointContract(definition, out var expected, out error)) return false;
            error = "CHECKPOINT_CONTRACT_MISMATCH";
            if (checkpoint.jobId != (definition != null ? definition.jobId : "") ||
                checkpoint.definitionRevision != (definition != null ? definition.checkpointRevision : 0) ||
                checkpoint.steps == null || checkpoint.steps.Count != expected.Count) return false;
            for (int i = 0; i < expected.Count; i++)
            {
                var a = expected[i]; var b = checkpoint.steps[i];
                if (b == null || a.stepId != b.stepId || a.kind != b.kind || a.targetId != b.targetId ||
                    a.secondaryId != b.secondaryId || a.requiredCount != b.requiredCount ||
                    a.arriveDistance != b.arriveDistance) return false;
            }
            error = "CHECKPOINT_PROGRESS_INVALID";
            if (checkpoint.progress < 0) return false;
            if (definition == null)
            {
                if (checkpoint.currentStepId != "" || checkpoint.isComplete || checkpoint.progress != 0) return false;
            }
            else
            {
                index = expected.FindIndex(s => s.stepId == checkpoint.currentStepId);
                if (index < 0) return false;
                if (checkpoint.isComplete)
                {
                    if (index != expected.Count - 1 || checkpoint.progress != 0) return false;
                }
                else if (checkpoint.progress >= expected[index].requiredCount) return false;
            }
            error = "CHECKPOINT_BANK_INVALID";
            if (!ValidCheckpointBank(checkpoint.collectBank, false) || !ValidCheckpointBank(checkpoint.repairBank, true)) return false;
            error = null;
            return true;
        }

        private static bool TryBuildCheckpointContract(JobDefinition definition, out List<JobCheckpointStep> steps, out string error)
        {
            steps = new List<JobCheckpointStep>();
            error = null;
            if (definition == null) return true;
            error = "CHECKPOINT_DEFINITION_UNCONFIGURED";
            if (string.IsNullOrWhiteSpace(definition.jobId) || definition.checkpointRevision <= 0 ||
                definition.steps == null || definition.steps.Count == 0) return false;
            var ids = new HashSet<string>();
            foreach (var step in definition.steps)
            {
                if (step == null || string.IsNullOrWhiteSpace(step.stepId) || !ids.Add(step.stepId)) return false;
                var record = new JobCheckpointStep { stepId = step.stepId };
                if (step is GoToMarkerStepDefinition go)
                {
                    record.kind = "go"; record.targetId = go.markerId; record.requiredCount = 1;
                    record.arriveDistance = go.arriveDistance;
                    if (float.IsNaN(go.arriveDistance) || float.IsInfinity(go.arriveDistance) || go.arriveDistance <= 0) return false;
                }
                else if (step is CollectItemIdCountStepDefinition collect)
                { record.kind = "collect"; record.targetId = collect.itemId; record.requiredCount = collect.count; }
                else if (step is DeliverToSocketStepDefinition deliver)
                {
                    record.kind = "deliver"; record.targetId = deliver.socketId;
                    record.secondaryId = deliver.itemId; record.requiredCount = deliver.count;
                    if (string.IsNullOrWhiteSpace(deliver.itemId)) return false;
                }
                else if (step is ShootTargetsCountStepDefinition shoot)
                { record.kind = "shoot"; record.requiredCount = shoot.count; }
                else if (step is DisableDronesCountStepDefinition drone)
                { record.kind = "drone"; record.requiredCount = drone.count; }
                else if (step is RepairMachineCountStepDefinition repair)
                { record.kind = "repair"; record.targetId = repair.machineId ?? ""; record.requiredCount = repair.count; }
                else return false;
                if (record.requiredCount <= 0) return false;
                if ((record.kind == "go" || record.kind == "collect" || record.kind == "deliver") &&
                    string.IsNullOrWhiteSpace(record.targetId)) return false;
                steps.Add(record);
            }
            error = null;
            return true;
        }

        private static List<JobCheckpointCount> CopyCheckpointBank(Dictionary<string, int> bank)
        {
            var result = new List<JobCheckpointCount>();
            foreach (var entry in bank) result.Add(new JobCheckpointCount { id = entry.Key, count = entry.Value });
            result.Sort((a, b) => string.CompareOrdinal(a.id, b.id));
            return result;
        }

        private static bool ValidCheckpointBank(List<JobCheckpointCount> bank, bool allowEmptyId)
        {
            if (bank == null) return false;
            var ids = new HashSet<string>();
            long total = 0;
            foreach (var entry in bank)
            {
                if (entry == null || entry.id == null || (!allowEmptyId && string.IsNullOrWhiteSpace(entry.id)) ||
                    entry.count <= 0 || !ids.Add(entry.id)) return false;
                total += entry.count;
                if (total > int.MaxValue) return false;
            }
            return true;
        }

        private static void RestoreCheckpointBank(Dictionary<string, int> bank, List<JobCheckpointCount> saved)
        {
            bank.Clear();
            foreach (var entry in saved) bank.Add(entry.id, entry.count);
        }
    }
}
