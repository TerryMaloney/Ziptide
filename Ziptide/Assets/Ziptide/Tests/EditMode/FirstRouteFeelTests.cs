#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Source-contract gates for the first-route feel sprint. Device feel remains headset-yellow;
    /// these tests prevent ownership drift and presentation code from becoming a second gameplay state.
    /// </summary>
    public class FirstRouteFeelTests
    {
        [Test]
        public void RepairFeedback_IsOwnedByTheRealInteractionCallbacks()
        {
            string source = Read("Gameplay", "Runtime", "Story", "RepairableMachine.cs");

            StringAssert.Contains("OnPanelPulled(panel, panelRb, SelectingController(args))", source);
            StringAssert.Contains("_lastPartHand = SelectingController(args)", source);
            StringAssert.Contains("OnSwitchFlipped(SelectingController(args))", source);
            StringAssert.Contains("hand.SendHapticImpulse(amplitude, duration)", source);
            StringAssert.Contains("HAPTIC verb=", source);
            StringAssert.Contains("verb=\" + verb", source);
            StringAssert.DoesNotContain("HapticManager", source);
            StringAssert.DoesNotContain("FindObjectsOfType<XRBaseControllerInteractor>", source);
        }

        [Test]
        public void RepairFeedback_HasOneBoundedLocalSoundPerStage()
        {
            string source = Read("Gameplay", "Runtime", "Story", "RepairableMachine.cs");

            StringAssert.Contains("Repair_PanelRelease", source);
            StringAssert.Contains("Repair_PartSeat", source);
            StringAssert.Contains("Repair_PowerOn", source);
            StringAssert.Contains("_feelAudio.spatialBlend = 1f", source);
            StringAssert.Contains("_feelAudio.maxDistance = 8f", source);
            StringAssert.Contains("AudioClip.Create(name", source);
            StringAssert.DoesNotContain("AudioSource.PlayClipAtPoint", source);
        }

        [Test]
        public void ObjectiveBoard_PresentsCanonicalRuntimeWithoutOwningProgress()
        {
            string source = Read("Gameplay", "Runtime", "Jobs", "ObjectiveBoard.cs");

            StringAssert.Contains("jobDirector.Runtime.JobCompleted += OnJobCompleted", source);
            StringAssert.Contains("CONTRACT_TOAST", source);
            StringAssert.Contains("NEW OBJECTIVE", source);
            StringAssert.Contains("CONTRACT COMPLETE", source);
            StringAssert.Contains("REWARD SECURED", source);
            StringAssert.Contains("_announcedStep", source);
            StringAssert.Contains("WaitForSecondsRealtime", source);
            StringAssert.DoesNotContain("StartJob(", source);
            StringAssert.DoesNotContain("AdvanceStep(", source);
            StringAssert.DoesNotContain("JobRewards.Grant", source);
            StringAssert.DoesNotContain("SetFlag(", source);
            StringAssert.DoesNotContain("AddResource", source);
        }

        [Test]
        public void ObjectiveBoard_CompletionPresentationIsOneShot()
        {
            string source = Read("Gameplay", "Runtime", "Jobs", "ObjectiveBoard.cs");

            StringAssert.Contains("if (!_completionPresented)", source);
            StringAssert.Contains("_completionPresented = true", source);
            StringAssert.Contains("StartBoardPulse();", source);
            StringAssert.Contains("Mathf.Sin(Mathf.PI", source);
        }

        private static string Read(params string[] path)
        {
            string full = Path.Combine(Application.dataPath, "Ziptide");
            for (int i = 0; i < path.Length; i++) full = Path.Combine(full, path[i]);
            Assert.IsTrue(File.Exists(full), full);
            return File.ReadAllText(full);
        }
    }
}
#endif
