using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Repair-step contract (GAME_PLAN M2): a machine can be fixed at any time — before the job is
    /// accepted, before its step is current — and the work is never lost (repair bank, mirroring the
    /// collect bank). machineId filters; blank = any machine counts.
    /// </summary>
    public class JobRuntimeRepairTests
    {
        private static GoToMarkerStepDefinition Go(string markerId)
        {
            var s = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            s.markerId = markerId;
            s.arriveDistance = 2f;
            return s;
        }

        private static RepairMachineCountStepDefinition Repair(string machineId, int count = 1)
        {
            var s = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
            s.machineId = machineId;
            s.count = count;
            return s;
        }

        private static JobDefinition Job(params JobStepDefinition[] steps)
        {
            var j = ScriptableObject.CreateInstance<JobDefinition>();
            j.jobId = "test_job";
            foreach (var s in steps) j.steps.Add(s);
            return j;
        }

        [Test]
        public void Repair_InOrder_CompletesTheStep()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Repair("cistern_pump"), Go("exit")));

            rt.ReportRepair("cistern_pump");
            Assert.AreEqual(1, rt.CurrentStepIndex, "the repair advances to the Go step");
        }

        [Test]
        public void Repair_WrongMachine_DoesNotCount()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Repair("cistern_pump")));

            rt.ReportRepair("fuel_rig");
            Assert.AreEqual(0, rt.RepairProgress);
            Assert.IsFalse(rt.IsComplete);

            rt.ReportRepair("cistern_pump");
            Assert.IsTrue(rt.IsComplete);
        }

        [Test]
        public void Repair_DoneEarly_BanksAndCreditsWhenStepGoesLive()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Go("gate"), Repair("cistern_pump")));

            rt.ReportRepair("cistern_pump"); // fixed before reaching the gate
            Assert.AreEqual(0, rt.CurrentStepIndex, "still on the Go step");

            rt.ReportGoToArrived("gate");
            Assert.IsTrue(rt.IsComplete, "the banked repair completes the step the moment it goes live");
        }

        [Test]
        public void Repair_BeforeJobAccepted_CreditsAtStart()
        {
            var rt = new JobRuntime();
            rt.ReportRepair("cistern_pump"); // fixed before visiting the kiosk

            rt.StartJob(Job(Repair("cistern_pump")));
            Assert.IsTrue(rt.IsComplete);
        }

        [Test]
        public void Repair_BlankMachineId_AnyMachineCounts_IncludingBanked()
        {
            var rt = new JobRuntime();
            rt.ReportRepair("fuel_rig");        // banked under its own id
            rt.StartJob(Job(Repair("", 2)));    // any 2 machines

            Assert.AreEqual(1, rt.RepairProgress, "the banked fuel_rig repair counts toward 'any'");
            rt.ReportRepair("cistern_pump");
            Assert.IsTrue(rt.IsComplete);
        }
    }
}
