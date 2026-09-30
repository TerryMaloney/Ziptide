using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class MissionResumeTests
    {
        private readonly List<UnityEngine.Object> _owned = new List<UnityEngine.Object>();
        private string _dir;
        private T Asset<T>() where T : ScriptableObject
        { var item = ScriptableObject.CreateInstance<T>(); _owned.Add(item); return item; }
        private GameObject Object(string name)
        { var item = new GameObject(name); _owned.Add(item); return item; }
        [SetUp] public void Setup()
        { _dir = Path.Combine(Path.GetTempPath(), "ziptide_mission_" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(_dir); }
        [TearDown] public void Cleanup()
        {
            foreach (var item in _owned) if (item != null) UnityEngine.Object.DestroyImmediate(item);
            _owned.Clear(); Directory.Delete(_dir, true);
        }
        private WorldPackDefinition Pack()
        {
            var pack = Asset<WorldPackDefinition>(); pack.packId = "first_pack"; pack.sceneName = "FirstWorld";
            var job = Asset<JobDefinition>(); job.jobId = "first"; job.checkpointRevision = 1;
            job.replayPolicy = MissionReplayPolicy.OneTime; job.completionFlag = "first_done"; job.grantsWorldCompletion = true;
            job.reward.Add(new ResourceCost { resourceId = "credits", amount = 100 });
            var go = Asset<GoToMarkerStepDefinition>(); go.stepId = "helm"; go.markerId = "helm";
            var collect = Asset<CollectItemIdCountStepDefinition>(); collect.stepId = "manifest"; collect.itemId = "manifest"; collect.count = 1;
            var repair = Asset<RepairMachineCountStepDefinition>(); repair.stepId = "coupler"; repair.machineId = "coupler";
            job.steps.AddRange(new JobStepDefinition[] { go, collect, repair }); pack.jobs.Add(job);
            pack.collectibles.Add(new CollectibleSpawnDefinition { placementId = "manifest_primary", itemId = "manifest" });
            pack.machines.Add(new MachineSpawnDefinition { machineId = "coupler" }); pack.flagsGranted.Add("world_done");
            return pack;
        }
        private MissionCheckpointSession Open(PlayerProfile profile, WorldPackDefinition pack)
        {
            Assert.IsTrue(MissionCheckpointSession.TryOpen(profile, "FirstWorld", pack, out var session, out string error), error);
            return session;
        }
        private static void Commit(MissionCheckpointSession session)
        { Assert.IsTrue(session.TryCommit(out _, out string error), error); }
        private static void Finish(MissionCheckpointSession session)
        {
            session.StartOrResume(); session.Runtime.ReportGoToArrived("helm");
            session.Collect("manifest_primary"); session.SetRepairStage("coupler", 3); Commit(session);
        }

        [Test]
        public void EarlyPickupAndPartialRepair_ReloadThenComplete_PaysOnce()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); var session = Open(profile, pack);
            Assert.IsTrue(session.Collect("manifest_primary")); Assert.IsFalse(session.Collect("manifest_primary"));
            session.SetRepairStage("coupler", 2); Commit(session);
            var loaded = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(profile));
            var restored = Open(loaded, pack);
            Assert.IsTrue(restored.IsConsumed("manifest_primary")); Assert.AreEqual(2, restored.RepairStageFor("coupler"));
            restored.StartOrResume(); restored.Runtime.ReportGoToArrived("helm");
            Assert.AreEqual(2, restored.Runtime.CurrentStepIndex);
            restored.SetRepairStage("coupler", 3); Commit(restored);
            Assert.AreEqual(100, loaded.GetResource("credits")); Assert.IsTrue(loaded.HasFlag("world_done"));
            Assert.IsTrue(restored.Runtime.IsComplete);
            Assert.IsFalse(restored.SetRepairStage("coupler", 3));
            var again = Open(ProfileSerializer.Deserialize(ProfileSerializer.Serialize(loaded)), pack);
            again.StartOrResume(); Commit(again);
            Assert.IsTrue(again.Runtime.IsComplete); Assert.AreEqual(100, again.Profile.GetResource("credits"));
            Assert.AreEqual(1, again.Profile.GetWorld("FirstWorld").missionReceipts.Count);
            Assert.AreEqual(1, again.Profile.ledger.Count);
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void EveryRepairStage_RoundTripsBeforeAcceptance(int stage)
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); var session = Open(profile, pack);
            session.SetRepairStage("coupler", stage); Commit(session);
            var restored = Open(ProfileSerializer.Deserialize(ProfileSerializer.Serialize(profile)), pack);
            Assert.AreEqual(stage, restored.RepairStageFor("coupler"));
            Assert.IsNull(restored.Runtime.Definition);
            if (stage == 3)
            {
                restored.Collect("manifest_primary"); restored.StartOrResume(); restored.Runtime.ReportGoToArrived("helm"); Commit(restored);
                Assert.IsTrue(restored.Runtime.IsComplete);
            }
        }

        [Test]
        public void FailedDiskSave_RetryDoesNotRepay_AndBackupRemainsCoherent()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); var session = Open(profile, pack); Commit(session);
            string path = Path.Combine(_dir, "profile.json");
            Assert.IsTrue(SaveFileStore.TryWriteProfile(path, profile, 10, out _));
            Finish(session); Directory.CreateDirectory(SaveFileStore.TmpPath(path));
            Assert.IsFalse(SaveFileStore.TryWriteProfile(path, profile, 20, out _));
            var old = ProfileSerializer.Deserialize(File.ReadAllText(path));
            Assert.AreEqual(0, old.GetResource("credits")); Assert.IsEmpty(old.GetWorld("FirstWorld").missionReceipts);
            Assert.IsFalse(Open(old, pack).Runtime.IsComplete);
            Commit(session); Assert.AreEqual(100, profile.GetResource("credits")); Assert.AreEqual(1, profile.ledger.Count);
            Directory.Delete(SaveFileStore.TmpPath(path));
            Assert.IsTrue(SaveFileStore.TryWriteProfile(path, profile, 30, out _));
            var current = Open(ProfileSerializer.Deserialize(File.ReadAllText(path)), pack);
            Assert.IsTrue(current.Runtime.IsComplete); Assert.IsTrue(current.IsConsumed("manifest_primary"));
            Assert.AreEqual(3, current.RepairStageFor("coupler"));
            File.WriteAllText(path, "corrupt");
            string backup = SaveFileStore.ReadBestVersion(path, x => ProfileSerializer.TryDeserialize(x, out _), out bool recovered);
            Assert.IsTrue(recovered); Assert.AreEqual(0, ProfileSerializer.Deserialize(backup).GetResource("credits"));
        }

        [Test]
        public void LegacyCompletedFlag_ImportsWithoutPaying_AndNewProfileDoesNotMatch()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); profile.SetFlag("first_done");
            RewardRouter.Grant(profile, LedgerSource.Debug, "credits", 23);
            var session = Open(profile, pack); Commit(session);
            Assert.IsTrue(session.Runtime.IsComplete); Assert.IsTrue(session.IsConsumed("manifest_primary"));
            Assert.AreEqual(3, session.RepairStageFor("coupler")); Assert.AreEqual(23, profile.GetResource("credits"));
            Assert.IsTrue(profile.GetWorld("FirstWorld").missionReceipts[0].importedLegacy);
            Assert.IsTrue(session.MatchesProfile(profile)); Assert.IsFalse(session.MatchesProfile(ProfileSerializer.NewProfile()));
        }

        [Test]
        public void InvalidPhysicalRecord_IsRejectedWithoutOverwritingSavedData()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); Commit(Open(profile, pack));
            profile.GetWorld("FirstWorld").jobCheckpoints[0].repairStages[0].count = 99;
            string before = ProfileSerializer.Serialize(profile);
            Assert.IsFalse(MissionCheckpointSession.TryOpen(profile, "FirstWorld", pack, out _, out _));
            Assert.AreEqual(before, ProfileSerializer.Serialize(profile));
        }

        [TestCase(double.NaN)] [TestCase(double.PositiveInfinity)] [TestCase(-1d)]
        public void InvalidSecondReward_RejectsEntireBatch(double invalid)
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile();
            pack.jobs[0].reward.Add(new ResourceCost { resourceId = "ore", amount = invalid });
            string before = ProfileSerializer.Serialize(profile);
            Assert.IsFalse(MissionRewards.TryGrant(profile, pack.jobs[0], "FirstWorld", "run", pack.flagsGranted, out _, out _));
            Assert.AreEqual(before, ProfileSerializer.Serialize(profile));
        }

        [Test]
        public void AggregateOverflow_IsRejectedBeforeAnyMutation()
        {
            var pack = Pack(); var job = pack.jobs[0]; var profile = ProfileSerializer.NewProfile();
            job.reward.Clear(); job.reward.Add(new ResourceCost { resourceId = "credits", amount = double.MaxValue });
            job.reward.Add(new ResourceCost { resourceId = "credits", amount = double.MaxValue });
            string before = ProfileSerializer.Serialize(profile);
            Assert.IsFalse(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out _, out _));
            Assert.AreEqual(before, ProfileSerializer.Serialize(profile));
        }

        [Test]
        public void DuplicateResourceRows_AggregateAndReceiptSurvivesLedgerPruning()
        {
            var pack = Pack(); var job = pack.jobs[0]; var profile = ProfileSerializer.NewProfile();
            job.reward.Add(new ResourceCost { resourceId = "credits", amount = 25 });
            Assert.IsTrue(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out var result, out _));
            Assert.AreEqual(MissionRewardResult.Granted, result); Assert.AreEqual(125, profile.GetResource("credits"));
            for (int i = 0; i < 501; i++) RewardRouter.Grant(profile, LedgerSource.Debug, "other", 1);
            profile = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(profile));
            Assert.IsTrue(MissionRewards.TryGrant(profile, job, "FirstWorld", "another-run", null, out result, out _));
            Assert.AreEqual(MissionRewardResult.AlreadyGranted, result); Assert.AreEqual(125, profile.GetResource("credits"));
        }

        [Test]
        public void RepeatablePolicy_PaysOncePerExplicitRun_AndPolicyChangesAreRejected()
        {
            var pack = Pack(); var job = pack.jobs[0]; job.replayPolicy = MissionReplayPolicy.Repeatable;
            var profile = ProfileSerializer.NewProfile();
            foreach (var run in new[] { "one", "one", "two" })
                Assert.IsTrue(MissionRewards.TryGrant(profile, job, "FirstWorld", run, null, out _, out _));
            Assert.AreEqual(200, profile.GetResource("credits"));
            job.replayPolicy = MissionReplayPolicy.OneTime;
            Assert.IsFalse(MissionRewards.TryGrant(profile, job, "FirstWorld", "three", null, out _, out _));
        }

        [Test]
        public void OptionalJob_DoesNotGrantWorldFlags_ZeroRewardStillGetsReceipt()
        {
            var pack = Pack(); var job = pack.jobs[0]; job.grantsWorldCompletion = false; job.reward[0].amount = 0;
            var profile = ProfileSerializer.NewProfile();
            Assert.IsTrue(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", pack.flagsGranted, out _, out _));
            Assert.IsFalse(profile.HasFlag("world_done")); Assert.IsTrue(profile.HasFlag("first_done"));
            Assert.IsEmpty(profile.ledger); Assert.AreEqual(1, profile.GetWorld("FirstWorld").missionReceipts.Count);
        }

        [Test]
        public void SchemaFour_ReceivesEmptyReceiptList()
        {
            var profile = ProfileSerializer.Deserialize("{\"schemaVersion\":4,\"playerId\":\"p\",\"worlds\":[{\"worldId\":\"w\"}]}");
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, profile.schemaVersion);
            Assert.IsEmpty(profile.GetWorld("w").missionReceipts);
        }

        [Test]
        public void LogicalAndPhysicalDivergence_IsRejected()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); var session = Open(profile, pack);
            session.Collect("manifest_primary"); Commit(session);
            profile.GetWorld("FirstWorld").jobCheckpoints[0].consumedPickupIds.Clear();
            Assert.IsFalse(MissionCheckpointSession.TryOpen(profile, "FirstWorld", pack, out _, out string error));
            Assert.AreEqual("MISSION_STATE_DIVERGED", error);
        }

        [Test]
        public void SavePreparation_CapturesPendingMissionAndLateListenerFlags()
        {
            var pack = Pack(); var profile = ProfileSerializer.NewProfile(); var session = Open(profile, pack);
            session.Collect("manifest_primary"); session.SetRepairStage("coupler", 2);
            profile.SetFlag("later_tutorial_listener");
            string path = Path.Combine(_dir, "profile.json");
            Assert.IsTrue(SaveFileStore.TryWriteProfile(path, profile, 10, out _, () => Commit(session)));
            var saved = ProfileSerializer.Deserialize(File.ReadAllText(path));
            Assert.IsTrue(saved.HasFlag("later_tutorial_listener"));
            var restored = Open(saved, pack);
            Assert.IsTrue(restored.IsConsumed("manifest_primary")); Assert.AreEqual(2, restored.RepairStageFor("coupler"));
        }

        [Test]
        public void FailedPreparation_DoesNotReplaceTheLastDiskSave()
        {
            var profile = ProfileSerializer.NewProfile(); string path = Path.Combine(_dir, "profile.json");
            Assert.IsTrue(SaveFileStore.TryWriteProfile(path, profile, 10, out _));
            string before = File.ReadAllText(path);
            Assert.IsFalse(SaveFileStore.TryWriteProfile(path, profile, 20, out string error,
                () => { throw new InvalidOperationException("participant failed"); }));
            StringAssert.Contains("participant failed", error); Assert.AreEqual(before, File.ReadAllText(path));
            Assert.AreEqual(10, profile.lastSavedAtUnix);
        }

        [Test]
        public void SameJobInDifferentWorlds_HasSeparateReceipts()
        {
            var job = Pack().jobs[0]; var profile = ProfileSerializer.NewProfile();
            Assert.IsTrue(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out _, out _));
            Assert.IsTrue(MissionRewards.TryGrant(profile, job, "SecondWorld", "run", null, out _, out _));
            Assert.AreEqual(200, profile.GetResource("credits"));
        }

        [Test]
        public void MissingLegacyProof_AndUnspecifiedPolicy_CannotPay()
        {
            var job = Pack().jobs[0]; var profile = ProfileSerializer.NewProfile();
            Assert.IsFalse(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out _, out _, importLegacy: true));
            job.replayPolicy = MissionReplayPolicy.Unspecified;
            Assert.IsFalse(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out _, out _));
            Assert.IsEmpty(profile.worlds); Assert.IsEmpty(profile.ledger);
        }

        [Test]
        public void ExistingBalanceOverflow_RejectsWholeBatch()
        {
            var job = Pack().jobs[0]; var profile = ProfileSerializer.NewProfile();
            RewardRouter.Grant(profile, LedgerSource.Debug, "credits", double.MaxValue);
            job.reward[0].amount = double.MaxValue;
            string before = ProfileSerializer.Serialize(profile);
            Assert.IsFalse(MissionRewards.TryGrant(profile, job, "FirstWorld", "run", null, out _, out _));
            Assert.AreEqual(before, ProfileSerializer.Serialize(profile));
        }

        [TestCase(RepairStage.Panel)] [TestCase(RepairStage.Part)]
        [TestCase(RepairStage.Power)] [TestCase(RepairStage.Running)]
        public void RepairPresentation_RestoresSilently(RepairStage stage)
        {
            var root = Object("machine"); var machine = root.AddComponent<RepairableMachine>();
            var panel = Object("panel"); var socket = Object("socket"); var part = Object("part"); var power = Object("power");
            // Supply the presentation references directly: no XR selection, physics or global save is involved.
            Set(machine, "_def", new MachineSpawnDefinition { machineId = "coupler" });
            Set(machine, "_panel", panel); Set(machine, "_socket", socket.transform);
            Set(machine, "_part", part.transform); Set(machine, "_powerSwitch", power);
            int signals = 0; machine.StageChanged += _ => signals++;
            Assert.IsTrue(machine.RestoreStage(stage)); Assert.AreEqual(stage, machine.CurrentStage); Assert.AreEqual(0, signals);
            Assert.AreEqual(stage == RepairStage.Panel, panel.activeSelf);
            Assert.AreEqual((int)stage >= 1, socket.activeSelf); Assert.AreEqual((int)stage >= 2, power.activeSelf);
            Assert.AreEqual((int)stage >= 2, part == null);
        }
        private static void Set(object target, string name, object value)
        { target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value); }
    }
}
