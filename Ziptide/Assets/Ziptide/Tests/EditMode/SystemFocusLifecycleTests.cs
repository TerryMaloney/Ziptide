using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins the pure lifecycle derivation (contract §3/§4, slice S1): the signal-sequence tables
    /// from docs/runtime_lifecycle/quest_system_focus_contract.json transitions, including the
    /// rb36 lesson that NOTHING returns straight to Active without passing through Resuming.
    /// </summary>
    public class SystemFocusLifecycleTests
    {
        private static LifecycleState Step(LifecycleState prev, bool focused, bool paused, bool tracking = true)
            => SystemFocusStateMachine.Next(prev, focused, paused, tracking);

        [Test]
        public void Active_Stays_WhileFocusedUnpausedTracked()
        {
            Assert.AreEqual(LifecycleState.Active, Step(LifecycleState.Active, true, false));
        }

        [Test]
        public void FocusLoss_WhileRunning_IsSystemOverlay()
        {
            Assert.AreEqual(LifecycleState.SystemOverlay, Step(LifecycleState.Active, false, false));
        }

        [Test]
        public void Pause_IsHeadsetRemoved_AndDominatesFocus()
        {
            Assert.AreEqual(LifecycleState.HeadsetRemoved, Step(LifecycleState.Active, true, true));
            Assert.AreEqual(LifecycleState.HeadsetRemoved, Step(LifecycleState.Active, false, true));
            Assert.AreEqual(LifecycleState.HeadsetRemoved, Step(LifecycleState.SystemOverlay, false, true));
        }

        [Test]
        public void Backgrounded_StaysSticky_WhilePaused()
        {
            Assert.AreEqual(LifecycleState.Backgrounded, Step(LifecycleState.Backgrounded, true, true));
        }

        [Test]
        public void TrackingLoss_WhileOtherwiseActive_IsTrackingLost()
        {
            Assert.AreEqual(LifecycleState.TrackingLost, Step(LifecycleState.Active, true, false, tracking: false));
        }

        [Test]
        public void Pause_Dominates_TrackingLoss()
        {
            Assert.AreEqual(LifecycleState.HeadsetRemoved, Step(LifecycleState.Active, true, true, tracking: false));
        }

        [Test]
        public void EveryRecovery_RoutesThroughResuming_NeverStraightToActive()
        {
            // The rb36 class made structural: overlay close, unpause, and tracking recovery all
            // land in Resuming; only the owner's explicit checklist completes to Active.
            Assert.AreEqual(LifecycleState.Resuming, Step(LifecycleState.SystemOverlay, true, false));
            Assert.AreEqual(LifecycleState.Resuming, Step(LifecycleState.HeadsetRemoved, true, false));
            Assert.AreEqual(LifecycleState.Resuming, Step(LifecycleState.Backgrounded, true, false));
            Assert.AreEqual(LifecycleState.Resuming, Step(LifecycleState.TrackingLost, true, false));
        }

        [Test]
        public void Resuming_HoldsUntilOwnerCompletes_DerivationNeverPromotesIt()
        {
            Assert.AreEqual(LifecycleState.Resuming, Step(LifecycleState.Resuming, true, false));
        }

        [Test]
        public void Resuming_ReInterrupted_FallsBackToTheInterruptState()
        {
            Assert.AreEqual(LifecycleState.SystemOverlay, Step(LifecycleState.Resuming, false, false));
            Assert.AreEqual(LifecycleState.HeadsetRemoved, Step(LifecycleState.Resuming, true, true));
        }

        [Test]
        public void ContractSequence_DoffMidOverlay_ThenResume()
        {
            // Active → overlay → doff → resume: the §3 diagram's longest path.
            var s = Step(LifecycleState.Active, false, false);
            Assert.AreEqual(LifecycleState.SystemOverlay, s);
            s = Step(s, false, true);
            Assert.AreEqual(LifecycleState.HeadsetRemoved, s);
            s = Step(s, true, false);
            Assert.AreEqual(LifecycleState.Resuming, s);
        }
    }
}
