using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryUiSpatialAuditTests
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
        public IEnumerator ControlledWorldText_ProducesFacingAndOverlapEvidence()
        {
            var cameraHost = new GameObject("__RECOVERY_UI_CAMERA");
            _objects.Add(cameraHost);
            Camera camera = cameraHost.AddComponent<Camera>();
            camera.transform.position = Vector3.zero;
            camera.transform.rotation = Quaternion.identity;
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 50f;

            AddText("READABLE", new Vector3(-0.9f, 0.4f, 3f), Quaternion.identity, 0.06f);
            AddText("FACING AWAY", new Vector3(-0.9f, -0.4f, 3f),
                Quaternion.Euler(0f, 180f, 0f), 0.06f);
            AddText("OVERLAP A", new Vector3(0.55f, 0.1f, 3f), Quaternion.identity, 0.08f);
            AddText("OVERLAP B", new Vector3(0.56f, 0.1f, 3f), Quaternion.identity, 0.08f);

            yield return null;
            yield return null;

            RecoveryUiSpatialReport report = RecoveryUiSpatialAudit.Capture(
                camera,
                "R1_8_CONTROLLED_WORLD_TEXT");
            RecoveryUiSpatialArtifactPaths paths = RecoveryUiSpatialAudit.WriteArtifacts(
                report,
                "r1_8_controlled_world_text");

            Assert.IsTrue(File.Exists(paths.JsonPath));
            Assert.IsTrue(File.Exists(paths.MarkdownPath));
            Assert.GreaterOrEqual(report.records.Count, 4);
            Assert.IsTrue(HasCode(report, "TEXT_FACING_AWAY"),
                "The camera-space audit did not catch backward TextMesh orientation.");
            Assert.IsTrue(HasCode(report, "TEXT_SCREEN_OVERLAP"),
                "The camera-space audit did not catch overlapping visible labels.");
        }

        private void AddText(
            string value,
            Vector3 position,
            Quaternion rotation,
            float characterSize)
        {
            var go = new GameObject("__RECOVERY_UI_" + value.Replace(' ', '_'));
            _objects.Add(go);
            go.transform.position = position;
            go.transform.rotation = rotation;
            TextMesh text = go.AddComponent<TextMesh>();
            text.text = value;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = characterSize;
            text.fontSize = 64;
            text.color = Color.white;
        }

        private static bool HasCode(RecoveryUiSpatialReport report, string code)
        {
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].code == code) return true;
            return false;
        }
    }
}
