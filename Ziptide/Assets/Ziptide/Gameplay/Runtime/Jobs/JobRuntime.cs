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

        // Physical pickups can be grabbed BEFORE their Collect step is current (they exist in the world
        // from scene start). Early grabs bank here and are credited the moment a matching Collect step
        // becomes current — otherwise a destroyed pickup would soft-lock the contract.
        private readonly Dictionary<string, int> _collectBank = new Dictionary<string, int>();

        public void StartJob(JobDefinition definition)
        {
            Definition = definition;
            CurrentStepIndex = 0;
            IsComplete = false;
            CollectProgress = 0;
            DeliverProgress = 0;
            ShootProgress = 0;
            DroneProgress = 0;
            RefreshStepText();
            StepChanged?.Invoke();
            // Pickups grabbed BEFORE accepting the job are already banked — credit them now (the bank
            // deliberately survives StartJob: a collected physical item stays collected).
            ApplyCollectBank();
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
            StepText = label;
        }
    }
}
