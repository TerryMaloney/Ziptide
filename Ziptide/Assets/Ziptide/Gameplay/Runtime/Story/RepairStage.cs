using System;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Public read-only vocabulary for the existing RepairableMachine state. Numeric order preserves
    /// the original private enum and the physical Panel → Part → Power → Running progression.
    /// </summary>
    public enum RepairStage
    {
        Panel = 0,
        Part = 1,
        Power = 2,
        Running = 3
    }

    /// <summary>
    /// Neutral adapter helpers for repair-stage notifications and exact scanner identity matching.
    /// This type owns no machine state, scanner state, tutorial progression, profile data or I/O.
    /// </summary>
    public static class RepairStageSignals
    {
        /// <summary>
        /// True only when the immutable pulse result contains the exact designated machine instance.
        /// Matching machine IDs, names, transforms or ScanKind values are deliberately insufficient.
        /// </summary>
        public static bool ContainsDesignatedMachine(
            WristScanResult result,
            RepairableMachine designatedMachine)
        {
            if (result == null || designatedMachine == null) return false;

            for (int i = 0; i < result.Targets.Count; i++)
                if (ReferenceEquals(result.Targets[i].Source, designatedMachine))
                    return true;

            return false;
        }

        /// <summary>
        /// Publish a designated-machine scan only on an exact identity match. The publisher is optional;
        /// this helper remains a no-op when the later progression translator is absent.
        /// </summary>
        public static bool TryPublishDesignatedMachineScan(
            WristScanResult result,
            RepairableMachine designatedMachine,
            Action<RepairableMachine> publish)
        {
            if (!ContainsDesignatedMachine(result, designatedMachine)) return false;
            publish?.Invoke(designatedMachine);
            return true;
        }

        /// <summary>
        /// Invoke every stage subscriber independently so presentation/progression failures can never
        /// interrupt the physical repair owner or prevent later subscribers from observing the stage.
        /// </summary>
        public static int PublishStageSafely(
            Action<RepairStage> subscribers,
            RepairStage stage,
            Action<Exception> onSubscriberFailure = null)
        {
            if (subscribers == null) return 0;

            int successful = 0;
            foreach (Action<RepairStage> subscriber in subscribers.GetInvocationList())
            {
                try
                {
                    subscriber(stage);
                    successful++;
                }
                catch (Exception ex)
                {
                    onSubscriberFailure?.Invoke(ex);
                }
            }

            return successful;
        }
    }
}
