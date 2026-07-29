using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FH-S08 — the director is the piece that turns twelve individually CI-green owners into an
    /// actual guided first hour. Its whole job is translation, so the failure that matters is a
    /// signal string that does not match the contract: every owner would still fire, every test would
    /// still pass, and the tutorial would simply stop advancing. These tests read the contract JSON
    /// and the director source and refuse to let them drift.
    /// </summary>
    public sealed class FirstHourDirectorTests
    {
        private static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));

        private static string ReadRepoFile(string relative)
        {
            string path = Path.Combine(RepoRoot, relative.Replace('/', Path.DirectorySeparatorChar));
            Assert.That(File.Exists(path), Is.True, "Missing: " + relative);
            return File.ReadAllText(path);
        }

        private static string DirectorSource =>
            ReadRepoFile("Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs");

        private static string OrchestratorSource =>
            ReadRepoFile("Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs");

        /// <summary>Beat id + completion-signal id, in contract order, straight out of the JSON.</summary>
        private static List<KeyValuePair<string, string>> ContractBeats()
        {
            string json = ReadRepoFile("docs/first_hour/first_hour_beats.json");
            var beats = new List<KeyValuePair<string, string>>();
            foreach (Match m in Regex.Matches(json,
                         "\"id\"\\s*:\\s*\"(FH_[A-Z0-9_]+)\".*?\"completionSignal\"\\s*:\\s*\\{[^}]*?\"id\"\\s*:\\s*\"([A-Z0-9_]+)\"",
                         RegexOptions.Singleline))
            {
                beats.Add(new KeyValuePair<string, string>(m.Groups[1].Value, m.Groups[2].Value));
            }
            return beats;
        }

        // The three beats measured by the observation adapter arrive through its SignalCompleted
        // event rather than a literal Accept(...) call in the director.
        private static readonly HashSet<string> AdapterOwnedSignals = new HashSet<string>
        {
            "PLAYER_LOOKED_AT_RILL",
            "PLAYER_MOVED_SAFE_DISTANCE",
            "W001_ARRIVAL_ORIENTATION_COMPLETE",
        };

        [Test]
        public void EveryBeatInTheContract_HasAProducerWiredInTheDirector()
        {
            var beats = ContractBeats();
            Assert.AreEqual(22, beats.Count, "the v1 contract is 22 beats");

            string source = DirectorSource + OrchestratorSource;
            foreach (var beat in beats)
            {
                if (AdapterOwnedSignals.Contains(beat.Value)) continue;
                StringAssert.Contains("Accept(\"" + beat.Value + "\")", source,
                    "beat " + beat.Key + " has no producer: nothing ever raises " + beat.Value);
            }
        }

        [Test]
        public void ThePayoffBeat_RequiresTheVisibleDecal_AndNeverAutoCompletes()
        {
            // "Saved flag AND visible result" is the envelope's law. A payoff you cannot see is not a
            // payoff, and auto-completing it would hide the exact bug it exists to catch.
            string source = OrchestratorSource;
            StringAssert.Contains("FirstContractDecal = \"decal_first_contract\"", source);
            StringAssert.Contains("JourneyDecals", source);
            StringAssert.Contains("beat left OPEN", source);
            StringAssert.Contains("Accept(\"FIRST_HOUR_PAYOFF_OBSERVED\")", source);
        }

        [Test]
        public void TheEncounterBeat_IsFilteredToTheSignatureCreature()
        {
            StringAssert.Contains("orchestrator.Signature != creature", DirectorSource);
        }

        [Test]
        public void TheObservationWindow_RequiresDistanceNotContact()
        {
            // Reading a creature means watching it from outside its reach. Rewarding a player for
            // walking into a hostile's face teaches exactly the wrong instinct.
            string source = OrchestratorSource;
            StringAssert.Contains("MinimumSafeDistance", source);
            StringAssert.Contains("RequiredDwellSeconds", source);
            Assert.GreaterOrEqual(
                Ziptide.Gameplay.FirstHourW001Orchestrator.MinimumSafeDistance, 2f,
                "personal space must stay at least the design canon's 2 m");
        }

        [Test]
        public void TheObservationBeats_ArePipedThroughTheirAdapter()
        {
            string source = DirectorSource;
            StringAssert.Contains("FirstHourObservationAdapter.Instance.SignalCompleted += Accept", source);
            StringAssert.Contains("FirstHourObservationAdapter.Instance.SignalCompleted -= Accept", source);
            StringAssert.Contains("adapter.BeginBeat(_watchedBeatId)", source);
        }

        [Test]
        public void EverySubscription_HasAMatchingUnsubscribe()
        {
            // A director that subscribes twice double-counts a repair stage; one that never
            // unsubscribes keeps a dead world's zipline alive. Both are silent.
            string source = DirectorSource;
            var subscribed = new HashSet<string>();
            foreach (Match m in Regex.Matches(source, @"^\s*([A-Za-z_][\w\.]*)\s*\+=\s*(\w+);", RegexOptions.Multiline))
                subscribed.Add(m.Groups[1].Value + "|" + m.Groups[2].Value);

            var unsubscribed = new HashSet<string>();
            foreach (Match m in Regex.Matches(source, @"^\s*([A-Za-z_][\w\.]*)\s*-=\s*(\w+);", RegexOptions.Multiline))
                unsubscribed.Add(m.Groups[1].Value + "|" + m.Groups[2].Value);

            Assert.Greater(subscribed.Count, 8, "the director should be listening to the real owners");
            foreach (string pair in subscribed)
                Assert.IsTrue(unsubscribed.Contains(pair),
                    "subscribed but never unsubscribed: " + pair.Replace('|', ' '));
        }

        [Test]
        public void TheContractsDoneFlag_IsTranslatedToTheGamesOwnFlag()
        {
            string source = DirectorSource;
            StringAssert.Contains("ContractDoneFlag = \"TUTORIAL_DONE\"", source);
            StringAssert.Contains("profile.SetFlag(ZiptideFlags.TUTORIAL_COMPLETE)", source);
        }

        [Test]
        public void CompletedBeats_PersistSoAReloadResumesMidHour()
        {
            string source = DirectorSource;
            StringAssert.Contains("BeatFlagPrefix = \"FH_BEAT_\"", source);
            StringAssert.Contains("CompletedBeatIdsFromProfile", source);
        }

        [Test]
        public void TheDirectorNeverLocksInputOrMovesTheRig()
        {
            // Hard safety values in the contract: allowsInputLock=false, movesPlayerRig=false on
            // every beat. A conductor that grabs the player is not a conductor.
            string source = DirectorSource;
            StringAssert.DoesNotContain("transform.position =", source);
            StringAssert.DoesNotContain("SetParent(", source);
            StringAssert.DoesNotContain(".Disable()", source);
        }

        [Test]
        public void DrivingEverySignalInOrder_CompletesTheContract()
        {
            var beats = ContractBeats();
            var mapped = new List<FirstHourProgressBeat>();
            for (int i = 0; i < beats.Count; i++)
            {
                // Sequential prerequisites mirror the shipped contract's contiguous chain closely
                // enough to prove the signal ORDER the director produces is accepted end to end.
                var prerequisites = i == 0
                    ? new List<string>()
                    : new List<string> { beats[i - 1].Key };
                mapped.Add(new FirstHourProgressBeat(
                    beats[i].Key, true, prerequisites, beats[i].Value, new List<string>(), 10d, null));
            }

            var core = new FirstHourProgressCore(mapped);
            Assert.IsTrue(core.IsEnabled, core.DisabledReasonCode);

            for (int i = 0; i < beats.Count; i++)
            {
                FirstHourSignalResult result = core.AcceptSignal(beats[i].Value);
                Assert.IsTrue(result.Advanced,
                    "signal " + beats[i].Value + " did not advance beat " + beats[i].Key
                    + " (code " + result.Code + ")");
            }

            Assert.IsTrue(core.IsComplete, "the full ordered signal run must finish the first hour");
        }

        [Test]
        public void AnOutOfOrderSignal_IsIgnoredRatherThanSkippingTheTutorial()
        {
            var beats = ContractBeats();
            var mapped = new List<FirstHourProgressBeat>();
            for (int i = 0; i < beats.Count; i++)
                mapped.Add(new FirstHourProgressBeat(
                    beats[i].Key, true,
                    i == 0 ? new List<string>() : new List<string> { beats[i - 1].Key },
                    beats[i].Value, new List<string>(), 10d, null));

            var core = new FirstHourProgressCore(mapped);

            // The payoff signal arriving first must not end the tutorial before it began.
            FirstHourSignalResult jumped = core.AcceptSignal(beats[beats.Count - 1].Value);
            Assert.IsFalse(jumped.Advanced);
            Assert.AreEqual(FirstHourSignalResultCode.Early, jumped.Code);
            Assert.IsFalse(core.IsComplete);
            Assert.AreEqual(beats[0].Key, core.CurrentBeatId);
        }
    }
}
