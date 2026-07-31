using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Audit;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The build-time half of pack integrity (MISS_LEDGER #21). The defect this exists to catch is the
    /// one that actually shipped: ToxicCity's contract step 4 asked to repair a machine the pack never
    /// spawned, so the whole back half of the first level was unreachable by playing — and the only
    /// check that could have said so ran at world entry, on device, too late to stop the build.
    /// </summary>
    public class WorldPackAuditRulesTests
    {
        private static WorldPackDefinition Pack(string packId = "toxic_city", string scene = "ToxicCity")
        {
            var p = ScriptableObject.CreateInstance<WorldPackDefinition>();
            p.packId = packId;
            p.sceneName = scene;
            return p;
        }

        private static SceneAuditReport Audit(params WorldPackDefinition[] packs)
        {
            var report = new SceneAuditReport { sceneName = "__WORLD_PACKS__" };
            WorldPackAuditRules.Run(report, new List<WorldPackDefinition>(packs));
            return report;
        }

        private static int Count(SceneAuditReport report, string code)
        {
            int n = 0;
            foreach (AuditFinding f in report.findings)
                if (f.code == code) n++;
            return n;
        }

        [Test]
        public void AHealthyPack_ProducesNoFindings()
        {
            var go = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            go.markerId = "dispatch_inside";
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "job_01";
            job.steps.Add(go);

            var pack = Pack();
            pack.jobs.Add(job);
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player" });

            Assert.AreEqual(0, Audit(pack).findings.Count);
        }

        [Test]
        public void TheLevelLock_IsCaught_ARepairStepWithNoMachineInThePack()
        {
            // Verbatim the ToxicCity defect: the step names a machine, the pack spawns none.
            var repair = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
            repair.machineId = "signal_relay";
            repair.count = 1;
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "toxiccity_contract";
            job.steps.Add(repair);

            var pack = Pack();
            pack.jobs.Add(job);

            SceneAuditReport report = Audit(pack);
            Assert.AreEqual(1, Count(report, WorldPackAuditRules.InvalidCode));
            StringAssert.Contains("signal_relay", report.findings[0].message);
            StringAssert.Contains("toxic_city", report.findings[0].message);
        }

        [Test]
        public void PairingTheMachine_ClearsTheFinding()
        {
            // The mutation that proves the guard tracks the real invariant rather than the step's shape.
            var repair = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
            repair.machineId = "signal_relay";
            repair.count = 1;
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "toxiccity_contract";
            job.steps.Add(repair);

            var pack = Pack();
            pack.jobs.Add(job);
            pack.machines.Add(new MachineSpawnDefinition { machineId = "signal_relay" });

            Assert.AreEqual(0, Audit(pack).findings.Count);
        }

        [Test]
        public void EveryPackIsChecked_NotJustTheOneSomeoneRemembered()
        {
            // The whole point of hoisting this to a project-wide rule: world #7 gets the same guard
            // as the world that happened to be under a headset that night.
            var bad = ScriptableObject.CreateInstance<CollectItemIdCountStepDefinition>();
            bad.itemId = "artifact_half_b";
            bad.count = 2;                       // pack spawns none
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "expedition";
            job.steps.Add(bad);

            var healthy = Pack("w000_drift_in", "W000_DriftIn");
            var broken = Pack("w002_dry_cistern", "W002_DryCistern");
            broken.jobs.Add(job);

            SceneAuditReport report = Audit(healthy, broken);
            Assert.AreEqual(1, Count(report, WorldPackAuditRules.InvalidCode));
            StringAssert.Contains("w002_dry_cistern", report.findings[0].message);
        }

        [Test]
        public void FindingsAreWarnings_NotBlockers()
        {
            // Deliberate, per the PerfBudgetAuditRules ratchet: a brand-new blocker in the audit aborts
            // Terry's local build. This is promoted only after a clean run.
            var repair = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
            repair.machineId = "ghost";
            repair.count = 1;
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "j";
            job.steps.Add(repair);
            var pack = Pack();
            pack.jobs.Add(job);

            SceneAuditReport report = Audit(pack);
            Assert.AreEqual(1, report.findings.Count);
            Assert.AreEqual(AuditSeverity.Warning, report.findings[0].severity);
        }

        [Test]
        public void NoPacksAtAll_SaysSoInsteadOfPassingSilently()
        {
            // "The audit found nothing" and "the audit checked nothing" must not look identical.
            var report = new SceneAuditReport { sceneName = "__WORLD_PACKS__" };
            WorldPackAuditRules.Run(report, new List<WorldPackDefinition>());
            Assert.AreEqual(1, Count(report, WorldPackAuditRules.NoneFoundCode));
        }
    }
}
