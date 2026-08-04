using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryWorldCanvasTextAuditTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
                if (_objects[i] != null) UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator TextMeshProUgui_ProducesRectBoundsAndFacingEvidence()
        {
            GameObject cameraHost = NewObject("__RECOVERY_CANVAS_UI_CAMERA");
            Camera camera = cameraHost.AddComponent<Camera>();
            camera.transform.position = Vector3.zero;
            camera.transform.rotation = Quaternion.identity;
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 50f;

            GameObject canvasHost = new GameObject(
                "__RECOVERY_WORLD_CANVAS",
                typeof(RectTransform),
                typeof(Canvas));
            _objects.Add(canvasHost);
            RectTransform canvasRect = canvasHost.GetComponent<RectTransform>();
            canvasRect.position = new Vector3(0f, 0f, 3f);
            canvasRect.rotation = Quaternion.identity;
            canvasRect.localScale = Vector3.one * 0.005f;
            canvasRect.sizeDelta = new Vector2(600f, 500f);
            Canvas canvas = canvasHost.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;

            AddTmpUgui(canvasRect, "TMP UGUI READABLE", new Vector2(0f, 100f), Quaternion.identity);
            AddTmpUgui(canvasRect, "TMP UGUI AWAY", new Vector2(0f, -100f),
                Quaternion.Euler(0f, 180f, 0f));

            yield return null;
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();

            RecoveryUiSpatialReport report = RecoveryUiSpatialAudit.Capture(
                camera,
                "R1_8_CONTROLLED_WORLD_CANVAS_TMP");
            RecoveryWorldCanvasTextAudit.Append(camera, report);

            RecoveryUiSpatialRecord readable = FindText(report, "TMP UGUI READABLE");
            RecoveryUiSpatialRecord away = FindText(report, "TMP UGUI AWAY");
            Assert.IsNotNull(readable, "Readable TextMeshProUGUI was not captured.");
            Assert.IsNotNull(away, "Backward TextMeshProUGUI was not captured.");
            Assert.IsFalse(string.IsNullOrEmpty(readable.rendererPath),
                "CanvasRenderer evidence did not replace the unsupported renderer record.");
            Assert.IsTrue(readable.visibleInFrustum);
            Assert.IsTrue(readable.hasViewportBounds);
            Assert.Greater(readable.worldWidth, 0f);
            Assert.Greater(readable.worldHeight, 0f);
            Assert.Greater(readable.facingDot, 0.9f);
            Assert.Less(away.facingDot, -0.9f);
            Assert.IsFalse(HasFinding(report, "TEXT_RENDERER_MISSING", readable.hierarchyPath),
                "Supported world-space TMP UGUI still emitted TEXT_RENDERER_MISSING.");
            Assert.IsTrue(HasFinding(report, "TEXT_FACING_AWAY", away.hierarchyPath),
                "Backward world-space TMP UGUI did not trigger the facing blocker.");
        }

        private void AddTmpUgui(
            RectTransform parent,
            string value,
            Vector2 anchoredPosition,
            Quaternion localRotation)
        {
            Type tmpType = ResolveType("TMPro.TextMeshProUGUI");
            Assert.IsNotNull(tmpType,
                "TextMeshProUGUI type was not loaded from the installed TMP package.");
            Assert.IsTrue(RecoveryUiSpatialAudit.IsTmpTextType(tmpType));

            GameObject go = new GameObject(
                "__RECOVERY_" + value.Replace(' ', '_'),
                typeof(RectTransform),
                typeof(CanvasRenderer));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260f, 80f);
            rect.anchoredPosition = anchoredPosition;
            rect.localRotation = localRotation;

            MonoBehaviour component = go.AddComponent(tmpType) as MonoBehaviour;
            Assert.IsNotNull(component);
            var textProperty = tmpType.GetProperty("text");
            Assert.IsNotNull(textProperty);
            textProperty.SetValue(component, value, null);
            var fontSizeProperty = tmpType.GetProperty("fontSize");
            if (fontSizeProperty != null) fontSizeProperty.SetValue(component, 36f, null);
        }

        private GameObject NewObject(string name)
        {
            var go = new GameObject(name);
            _objects.Add(go);
            return go;
        }

        private static Type ResolveType(string fullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type type = assemblies[i].GetType(fullName, false);
                if (type != null) return type;
            }
            return null;
        }

        private static RecoveryUiSpatialRecord FindText(
            RecoveryUiSpatialReport report,
            string text)
        {
            for (int i = 0; i < report.records.Count; i++)
                if (report.records[i].text == text) return report.records[i];
            return null;
        }

        private static bool HasFinding(
            RecoveryUiSpatialReport report,
            string code,
            string hierarchyPath)
        {
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryUiSpatialFinding finding = report.findings[i];
                if (finding.code == code && finding.hierarchyPath == hierarchyPath) return true;
            }
            return false;
        }
    }
}
