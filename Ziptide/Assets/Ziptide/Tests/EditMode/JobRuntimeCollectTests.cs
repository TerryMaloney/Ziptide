using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Collect-step contract for PHYSICAL pickups (GAME_PLAN M1): a collectible can be grabbed at any
    /// time — before the job is accepted, before its step is current — and must never be lost. Early
    /// grabs bank and credit the moment a matching Collect step goes live.
    /// </summary>
    public class JobRuntimeCollectTests
    {
        private static GoToMarkerStepDefinition Go(string markerId)
        {
            var s = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            s.markerId = markerId;
            s.arriveDistance = 2f;
            return s;
        }

        private static CollectItemIdCountStepDefinition Collect(string itemId, int count)
        {
            var s = ScriptableObject.CreateInstance<CollectItemIdCountStepDefinition>();
            s.itemId = itemId;
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

        [TestCase("collect")]
        [TestCase("deliver")]
        [TestCase("shoot")]
        [TestCase("drone")]
        [TestCase("repair")]
        public void PartialProgress_EventSeesUpdatedText(string kind)
        {
            var rt = new JobRuntime();
            JobStepDefinition step;
            System.Action report;
            switch (kind)
            {
                case "collect":
                    step = Collect("sample", 2);
                    report = () => rt.ReportCollect("sample");
                    break;
                case "deliver":
                    var delivery = ScriptableObject.CreateInstance<DeliverToSocketStepDefinition>();
                    delivery.itemId = "sample"; delivery.socketId = "dock"; delivery.count = 2;
                    step = delivery;
                    report = () => rt.ReportDeliver("dock", "sample");
                    break;
                case "shoot":
                    var shoot = ScriptableObject.CreateInstance<ShootTargetsCountStepDefinition>();
                    shoot.count = 2; step = shoot; report = rt.ReportTargetHit;
                    break;
                case "drone":
                    var drone = ScriptableObject.CreateInstance<DisableDronesCountStepDefinition>();
                    drone.count = 2; step = drone; report = rt.ReportDroneDisabled;
                    break;
                default:
                    var repair = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
                    repair.machineId = "pump"; repair.count = 2; step = repair;
                    report = () => rt.ReportRepair("pump");
                    break;
            }
            var job = Job(step);
            try
            {
                rt.StartJob(job);
                string observedText = null;
                int changes = 0, completions = 0;
                rt.StepChanged += () => { changes++; observedText = rt.StepText; };
                rt.JobCompleted += () => completions++;
                report();
                Assert.AreEqual(1, changes);
                StringAssert.Contains("(1/2)", observedText);
                Assert.IsFalse(rt.IsComplete);
                report();
                Assert.AreEqual(1, completions);
                Assert.AreEqual("Complete!", rt.StepText);
                report();
                Assert.AreEqual(1, completions, "reports after completion must not replay completion");
            }
            finally
            {
                Object.DestroyImmediate(job);
                Object.DestroyImmediate(step);
            }
        }

        [Test]
        public void Collect_InOrder_CountsAndAdvances()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Collect("mineral_sample", 2), Go("exit")));

            rt.ReportCollect("mineral_sample");
            Assert.AreEqual(1, rt.CollectProgress);
            Assert.AreEqual(0, rt.CurrentStepIndex);

            rt.ReportCollect("mineral_sample");
            Assert.AreEqual(1, rt.CurrentStepIndex, "full count must advance to the Go step");
            Assert.IsFalse(rt.IsComplete);
        }

        [Test]
        public void Collect_GrabbedEarly_BanksAndCreditsWhenStepGoesLive()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Go("gate"), Collect("mineral_sample", 3), Go("exit")));

            // Player grabs all three pickups while still on the Go step — nothing may be lost.
            rt.ReportCollect("mineral_sample");
            rt.ReportCollect("mineral_sample");
            rt.ReportCollect("mineral_sample");
            Assert.AreEqual(0, rt.CurrentStepIndex, "still on the Go step");

            rt.ReportGoToArrived("gate");
            // Arriving makes the Collect step current; the bank must instantly satisfy it.
            Assert.AreEqual(2, rt.CurrentStepIndex, "banked grabs complete the Collect step immediately");
        }

        [Test]
        public void Collect_GrabbedBeforeJobAccepted_CreditsAtStart()
        {
            var rt = new JobRuntime();
            rt.ReportCollect("transmission_fragment"); // grabbed before visiting the kiosk

            rt.StartJob(Job(Collect("transmission_fragment", 1), Go("exit")));
            Assert.AreEqual(1, rt.CurrentStepIndex, "pre-accept grab credits the first Collect step");
        }

        [Test]
        public void Collect_PartialBank_LeavesRemainderOnTheStep()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Go("gate"), Collect("mineral_sample", 3)));

            rt.ReportCollect("mineral_sample"); // one early grab
            rt.ReportGoToArrived("gate");

            Assert.AreEqual(1, rt.CurrentStepIndex);
            Assert.AreEqual(1, rt.CollectProgress, "one banked grab credited; two still needed");
            Assert.IsFalse(rt.IsComplete);

            rt.ReportCollect("mineral_sample");
            rt.ReportCollect("mineral_sample");
            Assert.IsTrue(rt.IsComplete, "final Collect step completion completes the job");
        }

        [Test]
        public void Collect_WrongItemId_NeverCredits()
        {
            var rt = new JobRuntime();
            rt.StartJob(Job(Collect("mineral_sample", 1)));

            rt.ReportCollect("carapace");
            Assert.AreEqual(0, rt.CollectProgress);
            Assert.IsFalse(rt.IsComplete);

            rt.ReportCollect("mineral_sample");
            Assert.IsTrue(rt.IsComplete);
        }
    }
}
