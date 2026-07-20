using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Editor.Audit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class BuildProfileTravelAuditRulesTests
    {
        [Test]
        public void SceneNamesFromPaths_UsesExactArtifactUniverse()
        {
            HashSet<string> names = BuildProfileTravelAuditRules.SceneNamesFromPaths(new[]
            {
                "Assets/Ziptide/Scenes/_Boot.unity",
                "Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity",
                "Assets/Ziptide/Scenes/ToxicCity.unity",
            });

            CollectionAssert.AreEquivalent(new[] { "_Boot", "W000_DriftIn", "ToxicCity" }, names);
            Assert.That(BuildProfileTravelAuditRules.IsDestinationAllowed("W000_DriftIn", names), Is.True);
            Assert.That(BuildProfileTravelAuditRules.IsDestinationAllowed("MilestoneA_GrabCube", names), Is.False);
        }

        [Test]
        public void DeliberatelyBrokenTrigger_FiresProfileBlocker()
        {
            GameObject go = null;
            try
            {
                go = new GameObject("BrokenGoldenExit");
                ProximityTravelTrigger trigger = go.AddComponent<ProximityTravelTrigger>();
                trigger.SetDestination("MilestoneA_GrabCube");
                var report = new SceneAuditReport { sceneName = "ToxicCity" };
                var allowed = new HashSet<string> { "_Boot", "W000_DriftIn", "ToxicCity" };

                BuildProfileTravelAuditRules.ValidateActiveScene(report, allowed);

                Assert.That(report.blockerCount, Is.EqualTo(1));
                Assert.That(report.findings[0].code,
                    Is.EqualTo(BuildProfileTravelAuditRules.DestinationBlocker));
                StringAssert.Contains("MilestoneA_GrabCube", report.findings[0].message);
                StringAssert.Contains("W000_DriftIn", report.findings[0].message);
            }
            finally
            {
                if (go != null) Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void GoldenTriggerDestination_PassesExactProfile()
        {
            GameObject go = null;
            try
            {
                go = new GameObject("ValidGoldenExit");
                ProximityTravelTrigger trigger = go.AddComponent<ProximityTravelTrigger>();
                trigger.SetDestination("W000_DriftIn");
                var report = new SceneAuditReport { sceneName = "ToxicCity" };
                var allowed = new HashSet<string> { "_Boot", "W000_DriftIn", "ToxicCity" };

                BuildProfileTravelAuditRules.ValidateActiveScene(report, allowed);

                Assert.That(report.blockerCount, Is.Zero);
            }
            finally
            {
                if (go != null) Object.DestroyImmediate(go);
            }
        }
    }
}
