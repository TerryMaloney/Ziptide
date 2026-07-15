using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryRenderSnapshotTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly List<Material> _materials = new List<Material>();

        // This cleanup is entirely synchronous. Using a coroutine UnityTearDown here caused the
        // headless runner to wait for the global 180-second coroutine timeout even though the test
        // body had already written and validated its PNG. Keep the same destruction work, but do it
        // as a normal NUnit teardown so cleanup cannot become the result under test.
        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null) Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
            for (int i = 0; i < _materials.Count; i++)
                if (_materials[i] != null) Object.DestroyImmediate(_materials[i]);
            _materials.Clear();
        }

        [UnityTest]
        public IEnumerator ControlledRenderer_WritesNonBlankPngAndObjectiveMetrics()
        {
            var cameraHost = new GameObject("__RECOVERY_SNAPSHOT_CAMERA");
            _objects.Add(cameraHost);
            Camera camera = cameraHost.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 1.2f, -5f);
            camera.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.015f, 0.03f, 0.07f, 1f);
            camera.fieldOfView = 55f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 50f;

            MakePrimitive(
                PrimitiveType.Cube,
                "__RECOVERY_SNAPSHOT_RED",
                new Vector3(-1.2f, 1.2f, 1.5f),
                new Vector3(1.1f, 1.6f, 0.8f),
                new Color(0.95f, 0.08f, 0.05f, 1f));
            MakePrimitive(
                PrimitiveType.Sphere,
                "__RECOVERY_SNAPSHOT_GREEN",
                new Vector3(0.2f, 1.1f, 1.2f),
                Vector3.one * 1.25f,
                new Color(0.08f, 0.92f, 0.18f, 1f));
            MakePrimitive(
                PrimitiveType.Capsule,
                "__RECOVERY_SNAPSHOT_BLUE",
                new Vector3(1.35f, 1.2f, 1.7f),
                new Vector3(0.8f, 1.5f, 0.8f),
                new Color(0.08f, 0.25f, 0.98f, 1f));
            MakePrimitive(
                PrimitiveType.Plane,
                "__RECOVERY_SNAPSHOT_GROUND",
                new Vector3(0f, 0f, 2f),
                new Vector3(0.5f, 1f, 0.5f),
                new Color(0.18f, 0.20f, 0.24f, 1f));

            yield return null;
            yield return new WaitForEndOfFrame();

            RecoveryRenderSnapshotPaths paths = RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_CONTROLLED_RENDERER",
                "r1_7_controlled_renderer",
                640,
                640);
            Assert.IsTrue(File.Exists(paths.PngPath), "Snapshot PNG was not written.");
            Assert.IsTrue(File.Exists(paths.JsonPath), "Snapshot metadata JSON was not written.");
            Assert.Greater(new FileInfo(paths.PngPath).Length, 1000,
                "Snapshot PNG is suspiciously small.");

            RecoveryRenderSnapshotMetrics metrics =
                RecoveryRenderSnapshot.ReadMetrics(paths.JsonPath);
            Assert.AreEqual("R1_7_CONTROLLED_RENDERER", metrics.label);
            Assert.AreEqual(640, metrics.width);
            Assert.AreEqual(640, metrics.height);
            Assert.AreEqual(64, metrics.pngSha256.Length);
            Assert.Greater(metrics.quantizedColorCount, 16,
                "Controlled frame has too little color information to prove rendering.");
            Assert.Greater(metrics.dynamicRange, 0.20d,
                "Controlled frame has insufficient luminance range.");
            Assert.Greater(metrics.averageLuminance, 0.02d,
                "Controlled frame is effectively black.");
            Assert.Less(metrics.averageLuminance, 0.90d,
                "Controlled frame is effectively white/clipped.");
            Assert.Less(metrics.nearBlackRatio, 0.98d,
                "Controlled frame is almost entirely black.");
            Assert.Less(metrics.nearWhiteRatio, 0.98d,
                "Controlled frame is almost entirely white.");
            Assert.Less(metrics.transparentRatio, 0.01d,
                "Controlled camera capture unexpectedly produced transparent pixels.");
        }

        private void MakePrimitive(
            PrimitiveType type,
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            _objects.Add(go);

            Renderer renderer = go.GetComponent<Renderer>();
            Assert.IsNotNull(renderer, name + " has no renderer.");
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Standard");
            Assert.IsNotNull(shader, "No supported snapshot test shader was found.");
            var material = new Material(shader) { name = name + "_Material" };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            else material.color = color;
            renderer.sharedMaterial = material;
            _materials.Add(material);
        }
    }
}
