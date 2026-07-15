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
        public const int BatchModeRetainedSceneObjectLimit = 8;
        public const int BatchModeRetainedMaterialLimit = 8;

        private static readonly List<GameObject> BatchModeRetainedSceneObjects =
            new List<GameObject>();
        private static readonly List<Material> BatchModeRetainedMaterials =
            new List<Material>();

        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly List<Material> _materials = new List<Material>();

        public static int BatchModeRetainedSceneObjectCount =>
            BatchModeRetainedSceneObjects.Count;
        public static int BatchModeRetainedMaterialCount =>
            BatchModeRetainedMaterials.Count;

        // The Linux headless runner hangs after a valid PNG whenever these test-owned renderer
        // objects or materials are destroyed, whether immediate or queued. Batch mode therefore
        // detaches, deactivates, moves the bounded fixture to DontDestroyOnLoad and retains it until
        // process exit. Interactive PlayMode keeps normal deferred cleanup. Counts are exposed to the
        // recovery census so this workaround cannot silently grow into an unbounded test-process leak.
        [TearDown]
        public void TearDown()
        {
            if (Application.isBatchMode)
            {
                RetainBatchModeFixture();
                return;
            }

            int objectCount = 0;
            int materialCount = 0;
            for (int i = 0; i < _objects.Count; i++)
            {
                GameObject go = _objects[i];
                if (go == null) continue;
                DetachAndDeactivate(go);
                Object.Destroy(go);
                objectCount++;
            }
            _objects.Clear();

            for (int i = 0; i < _materials.Count; i++)
            {
                Material material = _materials[i];
                if (material == null) continue;
                Object.Destroy(material);
                materialCount++;
            }
            _materials.Clear();

            Debug.Log("ZIPTIDE: RECOVERY_CONTROLLED_RENDERER_CLEANUP_QUEUED objects="
                + objectCount + " materials=" + materialCount);
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
                "Controlled frame is effectively white/clipped.");
            Assert.Less(metrics.transparentRatio, 0.01d,
                "Controlled camera capture unexpectedly produced transparent pixels.");

            Debug.Log("ZIPTIDE: RECOVERY_CONTROLLED_RENDERER_ASSERTIONS_OK colors="
                + metrics.quantizedColorCount
                + " dynamicRange=" + metrics.dynamicRange.ToString("F4")
                + " pngBytes=" + metrics.pngBytes);
        }

        private void RetainBatchModeFixture()
        {
            int incomingObjects = 0;
            int incomingMaterials = 0;
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null) incomingObjects++;
            for (int i = 0; i < _materials.Count; i++)
                if (_materials[i] != null) incomingMaterials++;

            Assert.LessOrEqual(
                BatchModeRetainedSceneObjects.Count + incomingObjects,
                BatchModeRetainedSceneObjectLimit,
                "Controlled renderer scene-object retention exceeded its explicit batch-mode cap.");
            Assert.LessOrEqual(
                BatchModeRetainedMaterials.Count + incomingMaterials,
                BatchModeRetainedMaterialLimit,
                "Controlled renderer material retention exceeded its explicit batch-mode cap.");

            for (int i = 0; i < _objects.Count; i++)
            {
                GameObject go = _objects[i];
                if (go == null) continue;
                DetachRenderState(go);
                go.hideFlags = HideFlags.HideAndDontSave;
                Object.DontDestroyOnLoad(go);
                go.SetActive(false);
                BatchModeRetainedSceneObjects.Add(go);
            }
            _objects.Clear();

            for (int i = 0; i < _materials.Count; i++)
            {
                Material material = _materials[i];
                if (material == null) continue;
                material.hideFlags = HideFlags.HideAndDontSave;
                BatchModeRetainedMaterials.Add(material);
            }
            _materials.Clear();

            Debug.Log("ZIPTIDE: RECOVERY_CONTROLLED_RENDERER_RETAINED objects="
                + BatchModeRetainedSceneObjects.Count + "/" + BatchModeRetainedSceneObjectLimit
                + " materials=" + BatchModeRetainedMaterials.Count + "/"
                + BatchModeRetainedMaterialLimit);
        }

        private static void DetachAndDeactivate(GameObject go)
        {
            DetachRenderState(go);
            go.SetActive(false);
        }

        private static void DetachRenderState(GameObject go)
        {
            Camera camera = go.GetComponent<Camera>();
            if (camera != null)
            {
                camera.enabled = false;
                camera.targetTexture = null;
            }

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
                if (renderers[rendererIndex] != null)
                    renderers[rendererIndex].sharedMaterial = null;
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
