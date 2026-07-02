using System;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// State machine for a single job: current step, progress counters, completion. Not a MonoBehaviour.
    /// </summary>
    public class JobRuntime
    {
        public event Action StepChanged;
        public event Action JobCompleted;

        public JobDefinition Definition { get; private set; }
        public int CurrentStepIndex { get; private set; }
        public bool IsComplete { get; private set; }
        public string StepText { get; private set; } = "";

        public int CollectProgress { get; private set; }
        public int DeliverProgress { get; private set; }
        public int ShootProgress { get; private set; }
        public int DroneProgress { get; private set; }
        public int RepairProgress { get; private set; }

        // Physical pickups can be grabbed BEFORE their Collect step is current (they exist in the world
        // from scene start). Early grabs bank here and are credited the moment a matching Collect step
        // becomes current — otherwise a destroyed pickup would soft-lock the contract.
        private readonly Dictionary<string, int> _collectBank = new Dictionary<string, int>();

        // Same protection for machines: a player can fix a machine before its Repair step is current
        // (or before accepting the job) — a repaired machine stays repaired, so bank it.
        private readonly Dictionary<string, int> _repairBank = new Dictionary<string, int>();

        public void StartJob(JobDefinition definition)
        {
            Definition = definition;
            CurrentStepIndex = 0;
            IsComplete = false;
            CollectProgress = 0;
            DeliverProgress = 0;
            ShootProgress = 0;
            DroneProgress = 0;
            RepairProgress = 0;
            RefreshStepText();
            StepChanged?.Invoke();
            // Pickups grabbed / machines fixed BEFORE accepting the job are already banked — credit them
            // now (the banks deliberately survive StartJob: done physical work stays done).
            ApplyCollectBank();
            ApplyRepairBank();
        }

        public void ReportGoToArrived(string markerId)
        {
            if (IsComplete || Definition == null) return;
            var step = GetCurrentStep();
            if (step is GoToMarkerStepDefinition goTo && goTo.markerId == markerId)
                AdvanceStep();
        }

        public void ReportCollect(string itemId)
        {
            if (IsComplete || string.IsNullOrEmpty(itemId)) return;
            var step = Definition != null ? GetCurrentStep() : null;
            if (step is CollectItemIdCountStepDefinition collect && collect.itemId == itemId)
            {
                CollectProgress++;
                if (CollectProgress >= collect.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
            }
            else
            {
                // Grabbed before the job started or before its step is current — bank it;
                // StartJob/ApplyCollectBank credit it the moment a matching Collect step is live.
                _collectBank.TryGetValue(itemId, out int n);
                _collectBank[itemId] = n + 1;
            }
        }

        /// <summary>A machine finished its hands-on repair (RepairableMachine's final stage).</summary>
        public void ReportRepair(string machineId)
        {
            if (IsComplete) return;
            var step = Definition != null ? GetCurrentStep() : null;
            if (step is RepairMachineCountStepDefinition repair &&
                (string.IsNullOrEmpty(repair.machineId) || repair.machineId == machineId))
            {
                RepairProgress++;
                if (RepairProgress >= repair.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
            }
            else
            {
                string key = machineId ?? "";
                _repairBank.TryGetValue(key, out int n);
                _repairBank[key] = n + 1;
            }
        }

        public void ReportDeliver(string socketId, string itemId)
        {
            if (IsComplete || Definition == null) return;
            var step = GetCurrentStep();
            if (step is DeliverToSocketStepDefinition deliver && deliver.socketId == socketId && deliver.itemId == itemId)
            {
                DeliverProgress++;
                if (DeliverProgress >= deliver.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
            }
        }

        public void ReportTargetHit()
        {
            if (IsComplete || Definition == null) return;
            var step = GetCurrentStep();
            if (step is ShootTargetsCountStepDefinition shoot)
            {
                ShootProgress++;
                if (ShootProgress >= shoot.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
            }
        }

        public void ReportDroneDisabled()
        {
            if (IsComplete || Definition == null) return;
            var step = GetCurrentStep();
            if (step is DisableDronesCountStepDefinition drone)
            {
                DroneProgress++;
                if (DroneProgress >= drone.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
            }
        }

        public JobStepDefinition GetCurrentStep()
        {
            if (Definition == null || Definition.steps == null || CurrentStepIndex < 0 || CurrentStepIndex >= Definition.steps.Count)
                return null;
            return Definition.steps[CurrentStepIndex];
        }

        private void AdvanceStep()
        {
            CollectProgress = 0;
            DeliverProgress = 0;
            ShootProgress = 0;
            DroneProgress = 0;
            RepairProgress = 0;
            if (Definition == null || Definition.steps == null || CurrentStepIndex >= Definition.steps.Count - 1)
            {
                IsComplete = true;
                StepText = "Complete!";
                JobCompleted?.Invoke();
                return;
            }
            CurrentStepIndex++;
            RefreshStepText();
            StepChanged?.Invoke();
            ApplyCollectBank();
            ApplyRepairBank();
        }

        // Credit banked early repairs against the step that just became current (see ApplyCollectBank).
        private void ApplyRepairBank()
        {
            if (IsComplete) return;
            var step = GetCurrentStep() as RepairMachineCountStepDefinition;
            if (step == null) return;

            int banked;
            if (string.IsNullOrEmpty(step.machineId))
            {
                // Any machine counts — drain across all banked ids.
                banked = 0;
                foreach (var kv in _repairBank) banked += kv.Value;
                if (banked <= 0) return;
                int need = step.count - RepairProgress;
                int take = banked < need ? banked : need;
                RepairProgress += take;
                DrainRepairBank(take);
            }
            else
            {
                if (!_repairBank.TryGetValue(step.machineId, out banked) || banked <= 0) return;
                int need = step.count - RepairProgress;
                int take = banked < need ? banked : need;
                RepairProgress += take;
                banked -= take;
                if (banked > 0) _repairBank[step.machineId] = banked;
                else _repairBank.Remove(step.machineId);
            }

            if (RepairProgress >= step.count)
            {
                AdvanceStep();
            }
            else
            {
                RefreshStepText();
                StepChanged?.Invoke();
            }
        }

        private void DrainRepairBank(int take)
        {
            var keys = new List<string>(_repairBank.Keys);
            foreach (var k in keys)
            {
                if (take <= 0) break;
                int have = _repairBank[k];
                int use = have < take ? have : take;
                take -= use;
                if (have - use > 0) _repairBank[k] = have - use;
                else _repairBank.Remove(k);
            }
        }

        // Credit banked early grabs against the step that just became current. May advance again
        // (recursion terminates: each pass consumes bank and/or moves the step index forward).
        private void ApplyCollectBank()
        {
            if (IsComplete) return;
            var step = GetCurrentStep() as CollectItemIdCountStepDefinition;
            if (step == null || string.IsNullOrEmpty(step.itemId)) return;
            if (!_collectBank.TryGetValue(step.itemId, out int banked) || banked <= 0) return;

            int need = step.count - CollectProgress;
            int take = banked < need ? banked : need;
            CollectProgress += take;
            banked -= take;
            if (banked > 0) _collectBank[step.itemId] = banked;
            else _collectBank.Remove(step.itemId);

            if (CollectProgress >= step.count)
            {
                AdvanceStep();
            }
            else
            {
                RefreshStepText();
                StepChanged?.Invoke();
            }
        }

        private void RefreshStepText()
        {
            var step = GetCurrentStep();
            if (step == null)
            {
                StepText = "";
                return;
            }
            string label = step.stepLabel;
            if (step is GoToMarkerStepDefinition goTo)
                label = "Go to: " + goTo.markerId;
            else if (step is CollectItemIdCountStepDefinition collect)
                label = string.Format("Collect {0} x{1} ({2}/{3})", collect.itemId, collect.count, CollectProgress, collect.count);
            else if (step is DeliverToSocketStepDefinition deliver)
                label = string.Format("Deliver {0} to {1} ({2}/{3})", deliver.itemId, deliver.socketId, DeliverProgress, deliver.count);
            else if (step is ShootTargetsCountStepDefinition shoot)
                label = string.Format("Shoot targets ({0}/{1})", ShootProgress, shoot.count);
            else if (step is DisableDronesCountStepDefinition drone)
                label = string.Format("Disable drones ({0}/{1})", DroneProgress, drone.count);
            else if (step is RepairMachineCountStepDefinition repair)
                label = string.Format("Repair {0} ({1}/{2})",
                    string.IsNullOrEmpty(repair.machineId) ? "machines" : repair.machineId.Replace('_', ' '),
                    RepairProgress, repair.count);
            StepText = label;
        }
    }
}
