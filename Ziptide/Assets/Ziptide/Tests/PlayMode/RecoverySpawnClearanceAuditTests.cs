using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoverySpawnClearanceAuditTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null) Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator ControlledStandingPose_SeparatesClearAndBuriedTorsoStates()
        {
            var rig = new GameObject("__RECOVERY_SPAWN_RIG");
            _objects.Add(rig);
            var cameraHost = new GameObject("__RECOVERY_SPAWN_HEAD");
            _objects.Add(cameraHost);
            cameraHost.transform.SetParent(rig.transform, false);
            cameraHost.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            Camera camera = cameraHost.AddComponent<Camera>();

            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _objects.Add(floor);
            floor.name = "__RECOVERY_SPAWN_FLOOR";
            floor.transform.position = new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(8f, 0.1f, 8f);
            Physics.SyncTransforms();

            RecoverySpawnClearanceReport clear = RecoverySpawnClearanceAudit.Capture(
                rig.transform,
                camera,
                "R1_6_CONTROLLED_CLEAR_SPAWN");
            string clearPath = RecoverySpawnClearanceAudit.WriteArtifact(
                clear,
                "r1_6_controlled_clear_spawn");
            Assert.IsTrue(File.Exists(clearPath));
            Assert.AreEqual(0, BlockerCount(clear), Format(clear));
            Assert.IsTrue(clear.floorFound);
            Assert.That(clear.headHeightAboveFloor, Is.EqualTo(1.65f).Within(0.02f));

            var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _objects.Add(obstacle);
            obstacle.name = "__RECOVERY_SPAWN_TORSO_OBSTACLE";
            obstacle.transform.position = new Vector3(0.16f, 0.9f, 0f);
            obstacle.transform.localScale = new Vector3(0.18f, 0.8f, 0.18f);
            Physics.SyncTransforms();

            RecoverySpawnClearanceReport blocked = RecoverySpawnClearanceAudit.Capture(
                rig.transform,
                camera,
                "R1_6_CONTROLLED_BLOCKED_SPAWN");
            string blockedPath = RecoverySpawnClearanceAudit.WriteArtifact(
                blocked,
                "r1_6_controlled_blocked_spawn");
            Assert.IsTrue(File.Exists(blockedPath));
            Assert.IsTrue(HasCode(blocked, "SPAWN_TORSO_OCCLUDED"), Format(blocked));

            yield return null;
        }

        private static int BlockerCount(RecoverySpawnClearanceReport report)
        {
            int count = 0;
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].severity == "BLOCKER") count++;
            return count;
        }

        private static bool HasCode(RecoverySpawnClearanceReport report, string code)
        {
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].code == code) return true;
            return false;
        }

        private static string Format(RecoverySpawnClearanceReport report)
        {
            var values = new List<string>();
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoverySpawnClearanceFinding finding = report.findings[i];
                values.Add(finding.code + " path=" + finding.hierarchyPath +
                           " message=" + finding.message);
            }
            return string.Join("\n", values);
        }
    }
}
