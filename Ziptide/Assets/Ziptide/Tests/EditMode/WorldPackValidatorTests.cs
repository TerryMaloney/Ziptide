using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// WorldPack fail-loud validation contract: bad data (empty ids, null steps, zero counts) is
    /// reported; a well-formed pack (the W001 shape) passes clean. Pure, headless.
    /// </summary>
    public class WorldPackValidatorTests
    {
        private static WorldPackDefinition Pack(string packId = "toxic_city", string sceneName = "ToxicCity")
        {
            var p = ScriptableObject.CreateInstance<WorldPackDefinition>();
            p.packId = packId;
            p.sceneName = sceneName;
            return p;
        }

        private static JobDefinition JobWith(params JobStepDefinition[] steps)
        {
            var j = ScriptableObject.CreateInstance<JobDefinition>();
            j.jobId = "job_01";
            foreach (var s in steps) j.steps.Add(s);
            return j;
        }

        [Test]
        public void WellFormedPack_PassesClean()
        {
            var go = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            go.markerId = "dispatch_inside";
            var dd = ScriptableObject.CreateInstance<DisableDronesCountStepDefinition>();
            dd.count = 5;

            var pack = Pack();
            pack.jobs.Add(JobWith(go, dd));
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player" });
            pack.flagsGranted.Add("W001_COMPLETE");

            Assert.IsEmpty(WorldPackValidator.Validate(pack));
        }

        [Test]
        public void MissingIdentity_IsReported()
        {
            var issues = WorldPackValidator.Validate(Pack(packId: "", sceneName: ""));
            Assert.AreEqual(2, issues.Count);
        }

        [Test]
        public void BadSteps_AreReported()
        {
            var go = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            go.markerId = "";                                              // empty marker
            var dd = ScriptableObject.CreateInstance<DisableDronesCountStepDefinition>();
            dd.count = 0;                                                  // zero count
            var ci = ScriptableObject.CreateInstance<CollectItemIdCountStepDefinition>();
            ci.itemId = ""; ci.count = 0;                                  // two problems

            var pack = Pack();
            var job = JobWith(go, dd, ci);
            job.steps.Add(null);                                           // null step
            pack.jobs.Add(job);

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(5, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void JobWithNoSteps_AndNullJob_AreReported()
        {
            var pack = Pack();
            pack.jobs.Add(JobWith());   // no steps
            pack.jobs.Add(null);        // null entry

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(2, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void RewardWithAmountButNoResource_IsReported()
        {
            var go = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            go.markerId = "x";
            var pack = Pack();
            var job = JobWith(go);
            job.reward.Add(new ResourceCost { resourceId = "", amount = 100 });
            pack.jobs.Add(job);

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(1, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void BlankGatingEntries_AreReported()
        {
            var pack = Pack();
            pack.flagsRequired.Add("");
            pack.flagsGranted.Add(null);

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(2, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void NullPack_IsSafe()
        {
            var issues = WorldPackValidator.Validate(null);
            Assert.AreEqual(1, issues.Count);
        }
    }
}
