#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class RepairableMachineSignalTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null)
                    UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
        }

        [Test]
        public void RepairStage_PublicOrderMatchesPhysicalProgression()
        {
            Assert.AreEqual(0, (int)RepairStage.Panel);
            Assert.AreEqual(1, (int)RepairStage.Part);
            Assert.AreEqual(2, (int)RepairStage.Power);
            Assert.AreEqual(3, (int)RepairStage.Running);
            CollectionAssert.AreEqual(
                new[] { "Panel", "Part", "Power", "Running" },
                Enum.GetNames(typeof(RepairStage)));
        }

        [Test]
        public void CurrentStage_IsRepairedAndScannableShareTheExistingStageSource()
        {
            RepairableMachine machine = CreateMachine("machine");

            Assert.AreEqual(RepairStage.Panel, machine.CurrentStage);
            Assert.IsFalse(machine.IsRepaired);
            Assert.IsTrue(machine.ScanActive);
            Assert.AreSame(machine.transform, machine.ScanTransform);
            Assert.AreEqual(ScanKind.Objective, machine.ScanKind);

            SetStage(machine, RepairStage.Part);
            Assert.AreEqual(RepairStage.Part, machine.CurrentStage);
            Assert.IsFalse(machine.IsRepaired);
            Assert.IsTrue(machine.ScanActive);

            SetStage(machine, RepairStage.Power);
            Assert.AreEqual(RepairStage.Power, machine.CurrentStage);
            Assert.IsFalse(machine.IsRepaired);
            Assert.IsTrue(machine.ScanActive);

            SetStage(machine, RepairStage.Running);
            Assert.AreEqual(RepairStage.Running, machine.CurrentStage);
            Assert.IsTrue(machine.IsRepaired);
            Assert.IsFalse(machine.ScanActive);
        }

        [Test]
        public void ScanMatch_RequiresExactDesignatedMachineIdentity()
        {
            RepairableMachine designated = CreateMachine("same_id");
            RepairableMachine lookalike = CreateMachine("same_id");

            var lookalikeOnly = new WristScanResult(new[]
            {
                Target(lookalike)
            });
            var designatedIncluded = new WristScanResult(new[]
            {
                Target(lookalike),
                Target(designated)
            });

            Assert.IsFalse(RepairStageSignals.ContainsDesignatedMachine(
                WristScanResult.Empty,
                designated));
            Assert.IsFalse(RepairStageSignals.ContainsDesignatedMachine(
                lookalikeOnly,
                designated));
            Assert.IsTrue(RepairStageSignals.ContainsDesignatedMachine(
                designatedIncluded,
                designated));
        }

        [Test]
        public void DesignatedScan_PublishesOnceOnlyOnIdentityMatch()
        {
            RepairableMachine designated = CreateMachine("designated");
            RepairableMachine other = CreateMachine("other");
            int calls = 0;
            RepairableMachine seen = null;

            Assert.IsFalse(RepairStageSignals.TryPublishDesignatedMachineScan(
                new WristScanResult(new[] { Target(other) }),
                designated,
                machine => { calls++; seen = machine; }));
            Assert.AreEqual(0, calls);

            Assert.IsTrue(RepairStageSignals.TryPublishDesignatedMachineScan(
                new WristScanResult(new[] { Target(other), Target(designated) }),
                designated,
                machine => { calls++; seen = machine; }));
            Assert.AreEqual(1, calls);
            Assert.AreSame(designated, seen);
        }

        [Test]
        public void StageSubscriberFailure_DoesNotBlockLaterSubscriber()
        {
            int failures = 0;
            int laterCalls = 0;
            RepairStage laterStage = RepairStage.Panel;
            Action<RepairStage> subscribers = stage =>
            {
                throw new InvalidOperationException("expected");
            };
            subscribers += stage =>
            {
                laterCalls++;
                laterStage = stage;
            };

            int successful = RepairStageSignals.PublishStageSafely(
                subscribers,
                RepairStage.Power,
                exception => failures++);

            Assert.AreEqual(1, successful);
            Assert.AreEqual(1, failures);
            Assert.AreEqual(1, laterCalls);
            Assert.AreEqual(RepairStage.Power, laterStage);
        }

        [Test]
        public void RuntimeSource_PreservesRepairOwnerAndAddsOnlyPostTransitionSignals()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Story",
                "RepairableMachine.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);

            StringAssert.Contains(
                "public class RepairableMachine : MonoBehaviour, IScannable",
                source);
            StringAssert.Contains("private RepairStage _stage = RepairStage.Panel;", source);
            Assert.AreEqual(1, Count(source, "private RepairStage _stage"));
            StringAssert.DoesNotContain("private enum Stage", source);
            StringAssert.Contains("public RepairStage CurrentStage => _stage;", source);
            StringAssert.Contains("public bool IsRepaired => _stage == RepairStage.Running;", source);
            StringAssert.Contains("public bool ScanActive => _stage != RepairStage.Running;", source);
            StringAssert.Contains("public ScanKind ScanKind => Ziptide.Gameplay.ScanKind.Objective;", source);
            StringAssert.Contains("public Transform ScanTransform => transform;", source);

            int panelAssign = source.IndexOf("_stage = RepairStage.Part;", StringComparison.Ordinal);
            int panelLog = source.IndexOf("stage=panel_off", panelAssign, StringComparison.Ordinal);
            int panelLabel = source.IndexOf("UpdateLabel();", panelLog, StringComparison.Ordinal);
            int panelPublish = source.IndexOf("PublishStageChanged();", panelLabel, StringComparison.Ordinal);

            int partAssign = source.IndexOf("_stage = RepairStage.Power;", panelPublish, StringComparison.Ordinal);
            int destroyPart = source.IndexOf("Destroy(_part.gameObject);", partAssign, StringComparison.Ordinal);
            int partLog = source.IndexOf("stage=part_seated", destroyPart, StringComparison.Ordinal);
            int partLabel = source.IndexOf("UpdateLabel();", partLog, StringComparison.Ordinal);
            int partPublish = source.IndexOf("PublishStageChanged();", partLabel, StringComparison.Ordinal);

            int runningAssign = source.IndexOf("_stage = RepairStage.Running;", partPublish, StringComparison.Ordinal);
            int reportRepair = source.IndexOf("_director.ReportRepair(_def.machineId);", runningAssign, StringComparison.Ordinal);
            int repairedLog = source.IndexOf("ZIPTIDE: MACHINE_REPAIRED id=", reportRepair, StringComparison.Ordinal);
            int runningLabel = source.IndexOf("UpdateLabel();", repairedLog, StringComparison.Ordinal);
            int runningPublish = source.IndexOf("PublishStageChanged();", runningLabel, StringComparison.Ordinal);

            Assert.GreaterOrEqual(panelAssign, 0);
            Assert.Greater(panelLog, panelAssign);
            Assert.Greater(panelLabel, panelLog);
            Assert.Greater(panelPublish, panelLabel);

            Assert.Greater(partAssign, panelPublish);
            Assert.Greater(destroyPart, partAssign);
            Assert.Greater(partLog, destroyPart);
            Assert.Greater(partLabel, partLog);
            Assert.Greater(partPublish, partLabel);

            Assert.Greater(runningAssign, partPublish);
            Assert.Greater(reportRepair, runningAssign);
            Assert.Greater(repairedLog, reportRepair);
            Assert.Greater(runningLabel, repairedLog);
            Assert.Greater(runningPublish, runningLabel);
            Assert.AreEqual(3, Count(source, "PublishStageChanged();"));

            // Existing physical owner chokepoints remain singular.
            Assert.AreEqual(1, Count(source, "Vector3.Distance(_part.position, _socket.position) <= SeatDistance"));
            Assert.AreEqual(1, Count(source, "panelGrab.selectEntered.AddListener"));
            Assert.AreEqual(1, Count(source, "swInteractable.selectEntered.AddListener"));
            Assert.AreEqual(1, Count(source, "_director.ReportRepair(_def.machineId);"));

            // The machine remains neutral and never owns tutorial/progression/scanner pulse state.
            StringAssert.DoesNotContain("FirstHour", source);
            StringAssert.DoesNotContain("Tutorial", source);
            StringAssert.DoesNotContain("SaveSystem", source);
            StringAssert.DoesNotContain("PlayerProfile", source);
            StringAssert.DoesNotContain("WristScanner", source);
            StringAssert.DoesNotContain("ScanResultPublished", source);
        }

        [Test]
        public void RepairStageAdapterSource_IsNeutralAndOwnsNoSecondState()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Story",
                "RepairStage.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);
            StringAssert.Contains("ReferenceEquals(result.Targets[i].Source, designatedMachine)", source);
            StringAssert.DoesNotContain("SaveSystem", source);
            StringAssert.DoesNotContain("PlayerProfile", source);
            StringAssert.DoesNotContain("FirstHourProgressCore", source);
            StringAssert.DoesNotContain("SceneManager", source);
            StringAssert.DoesNotContain("new WristScanner", source);
            StringAssert.DoesNotContain("private RepairStage _stage", source);
        }

        private RepairableMachine CreateMachine(string name)
        {
            var go = new GameObject(name);
            _objects.Add(go);
            return go.AddComponent<RepairableMachine>();
        }

        private static WristScanTarget Target(RepairableMachine machine)
        {
            return new WristScanTarget(
                machine,
                machine.transform,
                ScanKind.Objective,
                machine.transform.position,
                0f);
        }

        private static void SetStage(RepairableMachine machine, RepairStage stage)
        {
            FieldInfo field = typeof(RepairableMachine).GetField(
                "_stage",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(machine, stage);
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
