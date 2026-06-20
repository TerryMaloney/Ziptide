using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Job reward payout contract tests (W001 "first contract" bounty → passage credits). Pure,
    /// headless — guards that completing a job grants its resources + sets its completion flag, and is
    /// null-safe. No scene, no device.
    /// </summary>
    public class JobRewardsTests
    {
        private static JobDefinition Job(string flag, params (string id, double amt)[] rewards)
        {
            var j = ScriptableObject.CreateInstance<JobDefinition>();
            j.jobId = "toxiccity_contract";
            j.completionFlag = flag;
            foreach (var (id, amt) in rewards)
                j.reward.Add(new ResourceCost { resourceId = id, amount = amt });
            return j;
        }

        [Test]
        public void Grant_AddsRewardResources_ToProfile()
        {
            var profile = new PlayerProfile();
            var job = Job("", ("credits", 50), ("scrap", 3));

            JobRewards.Grant(job, profile);

            Assert.AreEqual(50, profile.GetResource("credits"));
            Assert.AreEqual(3, profile.GetResource("scrap"));
        }

        [Test]
        public void Grant_SetsCompletionFlag()
        {
            var profile = new PlayerProfile();
            var job = Job("toxiccity_complete", ("credits", 50));

            JobRewards.Grant(job, profile);

            Assert.IsTrue(profile.HasFlag("toxiccity_complete"));
            Assert.AreEqual(50, profile.GetResource("credits"));
        }

        [Test]
        public void Grant_BlankFlag_SetsNoFlag()
        {
            var profile = new PlayerProfile();
            JobRewards.Grant(Job(""), profile);
            Assert.AreEqual(0, profile.flags.Count);
        }

        [Test]
        public void Grant_StacksOntoExistingResources()
        {
            var profile = new PlayerProfile();
            profile.AddResource("credits", 10);
            JobRewards.Grant(Job("", ("credits", 50)), profile);
            Assert.AreEqual(60, profile.GetResource("credits"));
        }

        [Test]
        public void Grant_SkipsEmptyOrZeroEntries()
        {
            var profile = new PlayerProfile();
            var job = Job("", ("", 99), ("credits", 0));
            JobRewards.Grant(job, profile);
            Assert.AreEqual(0, profile.resources.Count, "blank id and zero amount are ignored");
        }

        [Test]
        public void Grant_IsNullSafe()
        {
            var profile = new PlayerProfile();
            Assert.DoesNotThrow(() => JobRewards.Grant(null, profile));
            Assert.DoesNotThrow(() => JobRewards.Grant(Job("", ("credits", 1)), null));
        }
    }
}
