#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    public class FirstHourProgressCoreTests
    {
        private FirstHourContractData _contract;
        private List<FirstHourProgressBeat> _beats;

        [SetUp]
        public void SetUp()
        {
            FirstHourContractImportResult imported = FirstHourContractAuthor.ImportSource();
            Assert.IsTrue(imported.success, imported.code + ": " + imported.message);
            _contract = imported.data;
            _beats = Map(_contract.beats);
        }

        [Test]
        public void ApprovedContract_AdvancesAllTwentyTwoBeatsInOrder()
        {
            var core = new FirstHourProgressCore(_beats);
            Assert.IsTrue(core.IsEnabled, core.DisabledReasonCode);
            Assert.AreEqual(22, _beats.Count);

            for (int i = 0; i < _contract.beats.Count; i++)
            {
                FirstHourBeatDefinition expected = _contract.beats[i];
                Assert.AreEqual(expected.id, core.CurrentBeatId, "current beat at index " + i);
                Assert.AreEqual(expected.completionSignal.id, core.CurrentSignalId);

                FirstHourSignalResult result = core.AcceptSignal(expected.completionSignal.id);
                Assert.AreEqual(FirstHourSignalResultCode.Accepted, result.Code, expected.id);
                Assert.IsTrue(result.Advanced);
                Assert.AreEqual(expected.id, result.CompletedBeatId);
                CollectionAssert.AreEqual(expected.setsFlags, result.GrantedFlags);
            }

            Assert.IsTrue(core.IsComplete);
            Assert.IsFalse(core.IsBlocked);
            Assert.IsNull(core.CurrentBeatId);
            Assert.AreEqual(22, core.CompletedBeatIds.Count);
        }

        [Test]
        public void EarlyUnknownEmptyAndDuplicateSignals_AreNoOps()
        {
            var core = new FirstHourProgressCore(_beats);
            string firstBeat = core.CurrentBeatId;

            FirstHourSignalResult early = core.AcceptSignal(_contract.beats[5].completionSignal.id);
            Assert.AreEqual(FirstHourSignalResultCode.Early, early.Code);
            Assert.AreEqual(firstBeat, core.CurrentBeatId);

            FirstHourSignalResult unknown = core.AcceptSignal("NOT_A_REAL_FIRST_HOUR_SIGNAL");
            Assert.AreEqual(FirstHourSignalResultCode.Unknown, unknown.Code);
            Assert.AreEqual(firstBeat, core.CurrentBeatId);

            FirstHourSignalResult empty = core.AcceptSignal("   ");
            Assert.AreEqual(FirstHourSignalResultCode.EmptySignal, empty.Code);
            Assert.AreEqual(firstBeat, core.CurrentBeatId);

            FirstHourSignalResult accepted = core.AcceptSignal(_contract.beats[0].completionSignal.id);
            Assert.AreEqual(FirstHourSignalResultCode.Accepted, accepted.Code);
            string secondBeat = core.CurrentBeatId;

            FirstHourSignalResult duplicate = core.AcceptSignal(_contract.beats[0].completionSignal.id);
            Assert.AreEqual(FirstHourSignalResultCode.Duplicate, duplicate.Code);
            Assert.AreEqual(secondBeat, core.CurrentBeatId);
            Assert.AreEqual(1, core.CompletedBeatIds.Count);
        }

        [Test]
        public void Hint_FiresAtThresholdOnceAndResetsAfterAdvance()
        {
            var completed = new[] { _contract.beats[0].id, _contract.beats[1].id };
            var core = new FirstHourProgressCore(_beats, completed);
            FirstHourBeatDefinition teaching = _contract.beats[2];

            Assert.AreEqual(teaching.id, core.CurrentBeatId);
            Assert.AreEqual(teaching.rillLine.id, core.CurrentHintLineId);
            Assert.AreEqual(teaching.hesitationSeconds, core.CurrentHintDelaySeconds, 0.0001d);
            Assert.IsFalse(core.ShouldOfferHint(teaching.hesitationSeconds - 0.01d));
            Assert.IsTrue(core.ShouldOfferHint(teaching.hesitationSeconds));
            Assert.IsTrue(core.MarkHintDelivered());
            Assert.IsFalse(core.MarkHintDelivered(), "same beat hint latches once");
            Assert.IsFalse(core.ShouldOfferHint(teaching.hesitationSeconds + 100d));

            FirstHourSignalResult accepted = core.AcceptSignal(teaching.completionSignal.id);
            Assert.AreEqual(FirstHourSignalResultCode.Accepted, accepted.Code);

            FirstHourBeatDefinition nextTeaching = _contract.beats[3];
            Assert.AreEqual(nextTeaching.id, core.CurrentBeatId);
            Assert.IsTrue(core.ShouldOfferHint(nextTeaching.hesitationSeconds),
                "advancing resets the hint latch for the next beat");
        }

        [Test]
        public void Hint_NoLineOrInvalidElapsedNeverOffers()
        {
            var core = new FirstHourProgressCore(_beats);
            Assert.IsNull(core.CurrentHintLineId);
            Assert.IsFalse(core.ShouldOfferHint(999d));
            Assert.IsFalse(core.ShouldOfferHint(double.NaN));
            Assert.IsFalse(core.ShouldOfferHint(double.PositiveInfinity));
            Assert.IsFalse(core.MarkHintDelivered());
        }

        [Test]
        public void Reload_ResumesAtFirstIncompleteRequiredBeatAndRebuildsFlags()
        {
            const int completedCount = 10;
            var completedIds = new List<string>();
            var expectedFlags = new List<string>();
            var seenFlags = new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < completedCount; i++)
            {
                FirstHourBeatDefinition beat = _contract.beats[i];
                completedIds.Add(beat.id);
                foreach (string flag in beat.setsFlags)
                {
                    if (seenFlags.Add(flag)) expectedFlags.Add(flag);
                }
            }

            // Unknown IDs are ignored for forward-compatible profile loading.
            completedIds.Add("SOME_UNRELATED_PROFILE_FLAG");
            var core = new FirstHourProgressCore(_beats, completedIds);

            Assert.IsTrue(core.IsEnabled, core.DisabledReasonCode);
            Assert.AreEqual(_contract.beats[completedCount].id, core.CurrentBeatId);
            Assert.AreEqual(completedCount, core.CompletedBeatIds.Count);
            CollectionAssert.AreEqual(expectedFlags, core.GrantedFlags);
            Assert.IsTrue(core.IsBeatComplete(_contract.beats[0].id));
            Assert.IsFalse(core.IsBeatComplete(_contract.beats[completedCount].id));
        }

        [Test]
        public void NullCompletedSet_StartsEmpty()
        {
            var core = new FirstHourProgressCore(_beats, null);
            Assert.IsTrue(core.IsEnabled);
            Assert.AreEqual(_contract.beats[0].id, core.CurrentBeatId);
            Assert.AreEqual(0, core.CompletedBeatIds.Count);
            Assert.AreEqual(0, core.GrantedFlags.Count);
        }

        [Test]
        public void FinalCompletion_ReturnsAndAggregatesAllFlags()
        {
            var expectedFlags = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (FirstHourBeatDefinition beat in _contract.beats)
                foreach (string flag in beat.setsFlags)
                    if (seen.Add(flag)) expectedFlags.Add(flag);

            var core = new FirstHourProgressCore(_beats);
            FirstHourSignalResult final = null;
            foreach (FirstHourBeatDefinition beat in _contract.beats)
                final = core.AcceptSignal(beat.completionSignal.id);

            Assert.IsNotNull(final);
            Assert.AreEqual(FirstHourSignalResultCode.Accepted, final.Code);
            Assert.IsTrue(final.IsComplete);
            Assert.IsTrue(core.IsComplete);
            CollectionAssert.AreEqual(expectedFlags, core.GrantedFlags);

            FirstHourSignalResult after = core.AcceptSignal(_contract.beats[21].completionSignal.id);
            Assert.AreEqual(FirstHourSignalResultCode.Complete, after.Code);
            Assert.IsFalse(after.Advanced);
        }

        [Test]
        public void InvalidInput_DisablesWithStableReasonCodes()
        {
            var missing = new FirstHourProgressCore(null);
            Assert.IsFalse(missing.IsEnabled);
            Assert.AreEqual(FirstHourProgressCore.DisabledBeatsMissing, missing.DisabledReasonCode);
            Assert.AreEqual(FirstHourSignalResultCode.Disabled, missing.AcceptSignal("ANY").Code);

            var duplicateId = new List<FirstHourProgressBeat>
            {
                Beat("A", "SIG_A"),
                Beat("A", "SIG_B")
            };
            var duplicateIdCore = new FirstHourProgressCore(duplicateId);
            Assert.AreEqual(FirstHourProgressCore.DisabledBeatDuplicate, duplicateIdCore.DisabledReasonCode);

            var duplicateSignal = new List<FirstHourProgressBeat>
            {
                Beat("A", "SIG"),
                Beat("B", "SIG", new[] { "A" })
            };
            var duplicateSignalCore = new FirstHourProgressCore(duplicateSignal);
            Assert.AreEqual(FirstHourProgressCore.DisabledSignalDuplicate, duplicateSignalCore.DisabledReasonCode);

            var forwardPrerequisite = new List<FirstHourProgressBeat>
            {
                Beat("A", "SIG_A", new[] { "B" }),
                Beat("B", "SIG_B")
            };
            var prerequisiteCore = new FirstHourProgressCore(forwardPrerequisite);
            Assert.AreEqual(FirstHourProgressCore.DisabledPrerequisite, prerequisiteCore.DisabledReasonCode);

            var negativeHint = new List<FirstHourProgressBeat>
            {
                new FirstHourProgressBeat("A", true, null, "SIG_A", null, -1d, "LINE")
            };
            var hintCore = new FirstHourProgressCore(negativeHint);
            Assert.AreEqual(FirstHourProgressCore.DisabledHintDelay, hintCore.DisabledReasonCode);
        }

        [Test]
        public void OptionalUnfinishedPrerequisite_CreatesBlockedNotCompleteState()
        {
            var beats = new List<FirstHourProgressBeat>
            {
                new FirstHourProgressBeat("OPTIONAL", false, null, "SIG_OPTIONAL", null, 0d, null),
                new FirstHourProgressBeat("REQUIRED", true, new[] { "OPTIONAL" }, "SIG_REQUIRED", null, 0d, null)
            };
            var core = new FirstHourProgressCore(beats);

            Assert.IsTrue(core.IsEnabled);
            Assert.IsFalse(core.IsComplete);
            Assert.IsTrue(core.IsBlocked);
            Assert.IsNull(core.CurrentBeatId);
            Assert.AreEqual(FirstHourSignalResultCode.Blocked, core.AcceptSignal("SIG_REQUIRED").Code);
        }

        [Test]
        public void CoreSource_HasNoUnityEngineReference()
        {
            string repoRoot = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(FirstHourContractAuthor.SourceJsonPath),
                "..",
                ".."));
            string corePath = Path.Combine(
                repoRoot,
                "Ziptide",
                "Assets",
                "Ziptide",
                "Core",
                "Runtime",
                "Tutorial",
                "FirstHourProgressCore.cs");

            Assert.IsTrue(File.Exists(corePath), corePath);
            string source = File.ReadAllText(corePath);
            StringAssert.DoesNotContain("UnityEngine", source);
            StringAssert.DoesNotContain("Resources.Load", source);
            StringAssert.DoesNotContain("DateTime", source);
            StringAssert.DoesNotContain("Debug.Log", source);
        }

        private static List<FirstHourProgressBeat> Map(List<FirstHourBeatDefinition> source)
        {
            var result = new List<FirstHourProgressBeat>();
            foreach (FirstHourBeatDefinition beat in source)
            {
                result.Add(new FirstHourProgressBeat(
                    beat.id,
                    beat.required,
                    beat.prerequisites,
                    beat.completionSignal.id,
                    beat.setsFlags,
                    beat.hesitationSeconds,
                    beat.rillLine != null ? beat.rillLine.id : null));
            }
            return result;
        }

        private static FirstHourProgressBeat Beat(
            string id,
            string signal,
            IEnumerable<string> prerequisites = null)
        {
            return new FirstHourProgressBeat(id, true, prerequisites, signal, null, 0d, null);
        }
    }
}
#endif
