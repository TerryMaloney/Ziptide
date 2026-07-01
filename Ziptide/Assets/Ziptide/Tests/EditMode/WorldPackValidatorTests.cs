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

        // ── M1: collectibles + choices ──────────────────────────────────────

        [Test]
        public void CollectStep_WithEnoughPickups_PassesClean()
        {
            var ci = ScriptableObject.CreateInstance<CollectItemIdCountStepDefinition>();
            ci.itemId = "mineral_sample"; ci.count = 3;

            var pack = Pack();
            pack.jobs.Add(JobWith(ci));
            for (int i = 0; i < 3; i++)
                pack.collectibles.Add(new CollectibleSpawnDefinition { itemId = "mineral_sample" });

            Assert.IsEmpty(WorldPackValidator.Validate(pack));
        }

        [Test]
        public void CollectStep_WithTooFewPickups_IsReported()
        {
            var ci = ScriptableObject.CreateInstance<CollectItemIdCountStepDefinition>();
            ci.itemId = "transmission_fragment"; ci.count = 1;

            var pack = Pack();
            pack.jobs.Add(JobWith(ci)); // no pickup authored — un-completable

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(1, issues.Count, string.Join(" | ", issues));
            StringAssert.Contains("un-completable", issues[0]);
        }

        [Test]
        public void Collectible_WithEmptyItemId_IsReported()
        {
            var pack = Pack();
            pack.collectibles.Add(new CollectibleSpawnDefinition { itemId = "" });

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(1, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void Choice_WritingNoFlags_OrTheSameFlag_IsReported()
        {
            var pack = Pack();
            pack.choices.Add(new ChoiceSpawnDefinition { choiceId = "c1", flagA = "", flagB = "" });
            pack.choices.Add(new ChoiceSpawnDefinition { choiceId = "c2", flagA = "SAME", flagB = "SAME" });

            var issues = WorldPackValidator.Validate(pack);
            Assert.AreEqual(2, issues.Count, string.Join(" | ", issues));
        }

        [Test]
        public void WellFormedChoice_PassesClean()
        {
            var pack = Pack();
            pack.choices.Add(new ChoiceSpawnDefinition
            {
                choiceId = "ending", flagA = "C12_W063_ENDING_A", flagB = "C12_W063_ENDING_B"
            });

            Assert.IsEmpty(WorldPackValidator.Validate(pack));
        }
    }
}
