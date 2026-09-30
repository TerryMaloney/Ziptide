using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class JobCheckpointTests
    {
        private readonly List<UnityEngine.Object> _assets = new List<UnityEngine.Object>();
        private T Asset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>(); _assets.Add(asset); return asset;
        }
        [TearDown]
        public void Cleanup()
        {
            foreach (var asset in _assets) UnityEngine.Object.DestroyImmediate(asset);
            _assets.Clear();
        }
        private JobDefinition Job(params JobStepDefinition[] steps)
        {
            var job = Asset<JobDefinition>(); job.jobId = "contract"; job.checkpointRevision = 1;
            job.steps.AddRange(steps); return job;
        }
        private CollectItemIdCountStepDefinition Collect(string id = "collect", int count = 3)
        {
            var step = Asset<CollectItemIdCountStepDefinition>();
            step.stepId = id; step.itemId = "sample"; step.count = count; return step;
        }
        private JobCheckpoint Capture(JobRuntime runtime)
        {
            Assert.IsTrue(runtime.TryCaptureCheckpoint("world", "run", out var saved, out string error), error);
            return saved;
        }

        [TestCase("go")]
        [TestCase("collect")]
        [TestCase("deliver")]
        [TestCase("shoot")]
        [TestCase("drone")]
        [TestCase("repair")]
        public void Objective_RoundTripsThroughProfile_ThenContinues(string kind)
        {
            var source = new JobRuntime(); var restored = new JobRuntime();
            JobStepDefinition step;
            Action<JobRuntime> report;
            switch (kind)
            {
                case "go":
                    var go = Asset<GoToMarkerStepDefinition>(); go.markerId = "exit";
                    step = go; report = r => r.ReportGoToArrived("exit"); break;
                case "collect":
                    step = Collect(); report = r => r.ReportCollect("sample"); break;
                case "deliver":
                    var deliver = Asset<DeliverToSocketStepDefinition>();
                    deliver.socketId = "dock"; deliver.itemId = "sample"; deliver.count = 3;
                    step = deliver; report = r => r.ReportDeliver("dock", "sample"); break;
                case "shoot":
                    var shoot = Asset<ShootTargetsCountStepDefinition>(); shoot.count = 3;
                    step = shoot; report = r => r.ReportTargetHit(); break;
                case "drone":
                    var drone = Asset<DisableDronesCountStepDefinition>(); drone.count = 3;
                    step = drone; report = r => r.ReportDroneDisabled(); break;
                default:
                    var repair = Asset<RepairMachineCountStepDefinition>(); repair.machineId = "pump"; repair.count = 3;
                    step = repair; report = r => r.ReportRepair("pump"); break;
            }
            step.stepId = "objective";
            var job = Job(step); source.StartJob(job);
            if (kind != "go") report(source);
            var profile = ProfileSerializer.NewProfile();
            profile.GetWorld("world", true).jobCheckpoints.Add(Capture(source));
            var loaded = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(profile));
            int changed = 0, completed = 0;
            restored.StepChanged += () => changed++;
            restored.JobCompleted += () => completed++;
            Assert.IsTrue(restored.TryRestoreCheckpoint(job, "world", "run",
                loaded.GetWorld("world").jobCheckpoints[0], out string error), error);
            Assert.AreEqual(0, changed); Assert.AreEqual(0, completed);
            Assert.AreEqual(source.StepText, restored.StepText);
            report(restored);
            if (kind != "go") report(restored);
            Assert.IsTrue(restored.IsComplete);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void PreAcceptBanks_RestoreAndCreditWhenJobStarts()
        {
            var source = new JobRuntime();
            source.ReportCollect("sample"); source.ReportRepair("pump");
            var saved = Capture(source);
            var restored = new JobRuntime();
            Assert.IsTrue(restored.TryRestoreCheckpoint(null, "world", "run", saved, out string error), error);
            var repair = Asset<RepairMachineCountStepDefinition>(); repair.stepId = "repair"; repair.machineId = "pump";
            restored.StartJob(Job(Collect(count: 2), repair));
            Assert.AreEqual(1, restored.CollectProgress);
            restored.ReportCollect("sample");
            Assert.IsTrue(restored.IsComplete);
        }

        [Test]
        public void CompletedCheckpoint_RestoresWithoutCompletionCallbacks()
        {
            var job = Job(Collect(count: 1)); var source = new JobRuntime(); source.StartJob(job);
            source.ReportCollect("sample"); var restored = new JobRuntime();
            int callbacks = 0;
            restored.JobCompleted += () => callbacks++;
            restored.StepChanged += () => callbacks++;
            Assert.IsTrue(restored.TryRestoreCheckpoint(job, "world", "run", Capture(source), out _));
            restored.ReportCollect("sample");
            Assert.AreEqual(0, callbacks); Assert.IsTrue(restored.IsComplete);
            Assert.AreEqual("Complete!", restored.StepText);
        }

        [TestCase("world")]
        [TestCase("run")]
        [TestCase("job")]
        [TestCase("revision")]
        [TestCase("version")]
        [TestCase("step")]
        [TestCase("negative")]
        [TestCase("already_full")]
        [TestCase("target")]
        [TestCase("type")]
        [TestCase("count")]
        [TestCase("null_bank")]
        [TestCase("duplicate_bank")]
        [TestCase("invalid_bank")]
        public void InvalidCheckpoint_DoesNotMutateDestination(string corruption)
        {
            var job = Job(Collect()); var source = new JobRuntime(); source.StartJob(job);
            source.ReportCollect("sample"); var saved = Capture(source);
            switch (corruption)
            {
                case "world": saved.worldId = "other"; break;
                case "run": saved.runId = "other"; break;
                case "job": saved.jobId = "other"; break;
                case "revision": saved.definitionRevision++; break;
                case "version": saved.version++; break;
                case "step": saved.currentStepId = "missing"; break;
                case "negative": saved.progress = -1; break;
                case "already_full": saved.progress = 3; break;
                case "target": saved.steps[0].targetId = "other"; break;
                case "type": saved.steps[0].kind = "shoot"; break;
                case "count": saved.steps[0].requiredCount++; break;
                case "null_bank": saved.collectBank = null; break;
                case "duplicate_bank":
                    saved.collectBank.Add(new JobCheckpointCount { id = "x", count = 1 });
                    saved.collectBank.Add(new JobCheckpointCount { id = "x", count = 1 }); break;
                case "invalid_bank": saved.repairBank.Add(new JobCheckpointCount { id = "pump", count = -1 }); break;
            }
            var destination = new JobRuntime(); destination.ReportCollect("untouched");
            string before = JsonUtility.ToJson(Capture(destination));
            Assert.IsFalse(destination.TryRestoreCheckpoint(job, "world", "run", saved, out string error));
            Assert.IsNotEmpty(error);
            Assert.AreEqual(before, JsonUtility.ToJson(Capture(destination)));
        }

        [Test]
        public void ReorderedDefinition_IsRejectedEvenWithoutRevisionBump()
        {
            var first = Collect("first"); var second = Collect("second");
            var job = Job(first, second); var source = new JobRuntime(); source.StartJob(job);
            var saved = Capture(source); job.steps.Reverse();
            Assert.IsFalse(new JobRuntime().TryRestoreCheckpoint(job, "world", "run", saved, out _));
        }

        [Test]
        public void SnapshotAndRestoredBanks_AreIndependentCopies()
        {
            var source = new JobRuntime(); source.ReportCollect("sample");
            var saved = Capture(source); source.ReportCollect("sample");
            Assert.AreEqual(1, saved.collectBank[0].count);
            var restored = new JobRuntime();
            Assert.IsTrue(restored.TryRestoreCheckpoint(null, "world", "run", saved, out _));
            saved.collectBank[0].count = 99;
            restored.StartJob(Job(Collect()));
            Assert.AreEqual(1, restored.CollectProgress);
        }

        [TestCase("revision")]
        [TestCase("missing_id")]
        [TestCase("duplicate_id")]
        public void UnconfiguredDefinition_CannotBeCaptured(string missing)
        {
            var first = Collect("first"); var second = Collect("second");
            var job = Job(first, second);
            if (missing == "revision") job.checkpointRevision = 0;
            if (missing == "missing_id") first.stepId = "";
            if (missing == "duplicate_id") second.stepId = first.stepId;
            var runtime = new JobRuntime(); runtime.StartJob(job);
            Assert.IsFalse(runtime.TryCaptureCheckpoint("world", "run", out var saved, out string error));
            Assert.IsNull(saved); Assert.IsNotEmpty(error);
        }

        [Test]
        public void LegacyProfile_MigratesWithEmptyCheckpointsAndPreservesProgress()
        {
            const string json = "{\"schemaVersion\":3,\"playerId\":\"legacy\",\"flags\":[\"done\"]," +
                "\"resources\":[{\"id\":\"credits\",\"amount\":7}],\"worlds\":[{\"worldId\":\"world\"}]}";
            Assert.IsTrue(ProfileSerializer.TryDeserialize(json, out var profile));
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, profile.schemaVersion);
            Assert.AreEqual(7, profile.GetResource("credits")); Assert.IsTrue(profile.HasFlag("done"));
            Assert.IsNotNull(profile.GetWorld("world").jobCheckpoints);
            Assert.IsEmpty(profile.GetWorld("world").jobCheckpoints);
        }
    }
}
