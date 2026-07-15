using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryRuntimeArtifactGuardTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            DestroyNamed("Ziptide_DebugHUD");
            DestroyNamed("PhotonMono");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DestroyNamed("Ziptide_DebugHUD");
            DestroyNamed("PhotonMono");
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [Test]
        public void GoldenGuard_ReportsAndPurgesPreGoldenDebugAndPhotonArtifacts()
        {
            var debug = new GameObject("Ziptide_DebugHUD");
            debug.AddComponent<Canvas>();
            var photon = new GameObject("PhotonMono");

            RecoveryRuntimeArtifactReport before =
                RecoveryRuntimeArtifactGuard.Capture("R1_5_CONTROLLED_STALE_ARTIFACTS");
            string artifact = RecoveryRuntimeArtifactGuard.WriteArtifact(
                before,
                "r1_5_controlled_stale_artifacts");

            Assert.AreEqual(2, before.findings.Count,
                "The guard did not identify both modeled stale runtime artifacts.");
            Assert.IsTrue(File.Exists(artifact), "The runtime-artifact JSON was not written.");
            Assert.IsTrue(HasCode(before, "FORBIDDEN_DEBUG_HUD_ARTIFACT"));
            Assert.IsTrue(HasCode(before, "FORBIDDEN_PHOTON_RUNTIME_ARTIFACT"));

            int removed = RecoveryRuntimeArtifactGuard.DestroyForbiddenArtifactsImmediate();
            Assert.AreEqual(2, removed);
            RecoveryRuntimeArtifactReport after =
                RecoveryRuntimeArtifactGuard.Capture("R1_5_CONTROLLED_STALE_ARTIFACTS_AFTER_PURGE");
            Assert.AreEqual(0, after.findings.Count,
                "The fresh-Golden purge left a forbidden runtime artifact active.");
        }

        private static bool HasCode(RecoveryRuntimeArtifactReport report, string code)
        {
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].code == code) return true;
            return false;
        }

        private static void DestroyNamed(string name)
        {
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go != null && go.scene.IsValid() && go.name == name)
                    Object.DestroyImmediate(go);
            }
        }
    }
}
