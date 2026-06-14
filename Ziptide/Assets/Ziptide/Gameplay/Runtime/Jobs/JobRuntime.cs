using System;
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
            if (IsComplete || Definition == null) return;
            var step = GetCurrentStep();
            if (step is CollectItemIdCountStepDefinition collect && collect.itemId == itemId)
            {
                CollectProgress++;
                if (CollectProgress >= collect.count)
                    AdvanceStep();
                else
                    StepChanged?.Invoke();
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
