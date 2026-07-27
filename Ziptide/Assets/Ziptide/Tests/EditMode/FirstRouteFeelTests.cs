#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Source-contract gates for the first-route feel sprint. Device feel remains headset-yellow;
    /// these tests prevent ownership drift and presentation code from becoming a second gameplay state.
    /// Comments are removed before matching so prose cannot accidentally satisfy a runtime contract.
    /// </summary>
    public class FirstRouteFeelTests
    {
        [Test]
        public void RepairFeedback_IsOwnedByTheRealInteractionCallbacks()
        {
            string source = ReadCode("Gameplay", "Runtime", "Story", "RepairableMachine.cs");

            StringAssert.Contains("OnPanelPulled(panel, panelRb, SelectingController(args))", source);
            StringAssert.Contains("_lastPartHand = SelectingController(args)", source);
            StringAssert.Contains("OnSwitchFlipped(SelectingController(args))", source);
            StringAssert.Contains("if (hand != null)", source);
            StringAssert.Contains("hand.SendHapticImpulse(amplitude, duration)", source);
            StringAssert.Contains("SendFeedback(hand, \"repair_panel\", 0.22f, 0.045f", source);
            StringAssert.Contains("SendFeedback(_lastPartHand, \"repair_part_seated\", 0.38f, 0.075f", source);
            StringAssert.Contains("SendFeedback(hand, \"repair_power_on\", 0.62f, 0.12f", source);
            StringAssert.Contains("HAPTIC verb=", source);
            StringAssert.DoesNotContain("HapticManager", source);
            StringAssert.DoesNotContain("FindObjectsOfType<XRBaseControllerInteractor>", source);
        }

        [Test]
        public void RepairFeedback_HasBoundedLocalResourcesAndCleanup()
        {
            string source = ReadCode("Gameplay", "Runtime", "Story", "RepairableMachine.cs");

            StringAssert.Contains("Repair_PanelRelease", source);
            StringAssert.Contains("Repair_PartSeat", source);
            StringAssert.Contains("Repair_PowerOn", source);
            StringAssert.Contains("_feelAudio.spatialBlend = 1f", source);
            StringAssert.Contains("_feelAudio.maxDistance = 8f", source);
            StringAssert.Contains("AudioClip.Create(name", source);
            StringAssert.Contains("DestroyOwnedClip(ref _panelReleaseClip)", source);
            StringAssert.Contains("DestroyOwnedClip(ref _partSeatClip)", source);
            StringAssert.Contains("DestroyOwnedClip(ref _powerOnClip)", source);
            StringAssert.Contains("new MaterialPropertyBlock()", source);
            StringAssert.Contains("StopSurfacePulseAndRestore()", source);
            StringAssert.Contains("Destroy(_ownedMaterials[i])", source);
            StringAssert.DoesNotContain("static AudioClip _panelReleaseClip", source);
            StringAssert.DoesNotContain("AudioSource.PlayClipAtPoint", source);
            StringAssert.DoesNotContain("renderer.material", source);
        }

        [Test]
        public void ObjectiveBoard_PresentsCanonicalRuntimeWithoutOwningProgress()
        {
            string source = ReadCode("Gameplay", "Runtime", "Jobs", "ObjectiveBoard.cs");

            StringAssert.Contains("runtime.JobCompleted += OnJobCompleted", source);
            StringAssert.Contains("CONTRACT_TOAST", source);
            StringAssert.Contains("NEW OBJECTIVE", source);
            StringAssert.Contains("CONTRACT COMPLETE", source);
            StringAssert.Contains("ROUTE COMPLETE", source);
            StringAssert.Contains("All objectives complete", source);
            StringAssert.Contains("_announcedStep", source);
            StringAssert.Contains("WaitForSecondsRealtime", source);
            StringAssert.Contains("Destroy(_toastMaterial)", source);
            StringAssert.DoesNotContain("REWARD SECURED", source);
            StringAssert.DoesNotContain("StartJob(", source);
            StringAssert.DoesNotContain("AdvanceStep(", source);
            StringAssert.DoesNotContain("JobRewards.Grant", source);
            StringAssert.DoesNotContain("SetFlag(", source);
            StringAssert.DoesNotContain("AddResource", source);
            StringAssert.DoesNotContain("renderer.material", source);
        }

        [Test]
        public void ObjectiveBoard_SubscriptionAndCompletionPresentationAreOneShot()
        {
            string source = ReadCode("Gameplay", "Runtime", "Jobs", "ObjectiveBoard.cs");

            StringAssert.Contains("if (_subscribedRuntime == runtime)", source);
            StringAssert.Contains("UnsubscribeFromRuntime()", source);
            StringAssert.Contains("private void OnDisable()", source);
            StringAssert.Contains("if (!_completionPresented)", source);
            StringAssert.Contains("_completionPresented = true", source);
            StringAssert.Contains("StartBoardPulse();", source);
            StringAssert.Contains("Mathf.Sin(Mathf.PI", source);
        }

        private static string ReadCode(params string[] path)
        {
            string full = Path.Combine(Application.dataPath, "Ziptide");
            for (int i = 0; i < path.Length; i++) full = Path.Combine(full, path[i]);
            Assert.IsTrue(File.Exists(full), full);
            return StripComments(File.ReadAllText(full));
        }

        private static string StripComments(string source)
        {
            return Regex.Replace(source, @"/\*.*?\*/|//[^\r\n]*", string.Empty,
                RegexOptions.Singleline);
        }
    }
}
#endif
