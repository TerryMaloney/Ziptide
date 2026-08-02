#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Editor.Audit;

namespace Ziptide.Tests.EditMode
{
    public class UiReadabilityAuditRulesTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int i = _created.Count - 1; i >= 0; i--)
                if (_created[i] != null)
                    Object.DestroyImmediate(_created[i]);
            _created.Clear();
        }

        [Test]
        public void Thresholds_AreLiteralAndStable()
        {
            Assert.AreEqual(1.2f, UiReadabilityAuditRules.MinimumEffectiveTextScale);
            Assert.AreEqual(0.10f, UiReadabilityAuditRules.MinimumTargetFaceMeters);
            Assert.AreEqual(0.30f, UiReadabilityAuditRules.MaximumLabelSeparationMeters);

            Assert.AreEqual(1.6f,
                UiReadabilityAuditRules.EffectiveTextScale(0.025f, 64), 0.0001f);
            Assert.AreEqual(1.6f,
                UiReadabilityAuditRules.EffectiveTextScale(-0.025f, 64), 0.0001f);
            Assert.AreEqual(0.025f,
                UiReadabilityAuditRules.EffectiveTextScale(0.025f, 0), 0.0001f);
        }

        [Test]
        public void TargetFace_RequiresTwoReachableDimensions()
        {
            Assert.IsTrue(UiReadabilityAuditRules.TargetFaceIsLargeEnough(
                new Vector3(0.58f, 0.28f, 0.12f)));
            Assert.IsTrue(UiReadabilityAuditRules.TargetFaceIsLargeEnough(
                new Vector3(0.10f, 0.10f, 0.01f)));
            Assert.IsFalse(UiReadabilityAuditRules.TargetFaceIsLargeEnough(
                new Vector3(2f, 0.09f, 0.01f)),
                "one long axis is a needle, not a reachable tile face");
        }

        [Test]
        public void Run_ReportsSmallNonEmptyText_AndIgnoresEmptyPlaceholder()
        {
            Scene scene = SceneManager.GetActiveScene();
            var small = NewObject("UI_TEST_SMALL_TEXT");
            var smallText = small.AddComponent<TextMesh>();
            smallText.text = "READ ME";
            smallText.characterSize = 0.01f;
            smallText.fontSize = 64;

            var empty = NewObject("UI_TEST_EMPTY_TEXT");
            var emptyText = empty.AddComponent<TextMesh>();
            emptyText.text = "";
            emptyText.characterSize = 0.001f;
            emptyText.fontSize = 1;

            var report = new SceneAuditReport { sceneName = scene.name };
            UiReadabilityAuditRules.Run(report, scene);

            Assert.IsTrue(HasFinding(report, "UI_TEXT_TOO_SMALL", "UI_TEST_SMALL_TEXT"));
            Assert.IsFalse(HasFinding(report, "UI_TEXT_TOO_SMALL", "UI_TEST_EMPTY_TEXT"));
            Assert.AreEqual(0, report.blockerCount, "v1 findings are warning-only");
        }

        [Test]
        public void Run_ReportsTextInteractableWithoutCollider()
        {
            Scene scene = SceneManager.GetActiveScene();
            var tile = NewObject("UI_TEST_NO_COLLIDER");
            AddSimpleInteractable(tile);
            AddReadableLabel(tile.transform, Vector3.zero);

            var report = new SceneAuditReport { sceneName = scene.name };
            UiReadabilityAuditRules.Run(report, scene);

            Assert.IsTrue(HasFinding(report, "UI_INTERACTABLE_NO_COLLIDER", "UI_TEST_NO_COLLIDER"));
            Assert.AreEqual(0, report.blockerCount);
        }

        [Test]
        public void Run_ReportsNeedleTargetAndDetachedLabel()
        {
            Scene scene = SceneManager.GetActiveScene();
            var tile = NewObject("UI_TEST_NEEDLE");
            var box = tile.AddComponent<BoxCollider>();
            box.size = new Vector3(2f, 0.05f, 0.04f);
            AddSimpleInteractable(tile);
            AddReadableLabel(tile.transform, Vector3.right * 1.5f);

            var report = new SceneAuditReport { sceneName = scene.name };
            UiReadabilityAuditRules.Run(report, scene);

            Assert.IsTrue(HasFinding(report, "UI_TARGET_TOO_SMALL", "UI_TEST_NEEDLE"));
            Assert.IsTrue(HasFinding(report, "UI_LABEL_DETACHED", "UI_TEST_NEEDLE"));
            Assert.AreEqual(0, report.blockerCount);
        }

        [Test]
        public void ReadableAttachedTile_ProducesNoFindingsForItsPath()
        {
            Scene scene = SceneManager.GetActiveScene();
            var tile = NewObject("UI_TEST_GOOD_TILE");
            var box = tile.AddComponent<BoxCollider>();
            box.size = new Vector3(0.58f, 0.28f, 0.12f);
            AddSimpleInteractable(tile);
            AddReadableLabel(tile.transform, new Vector3(0f, 0f, -0.07f));

            var report = new SceneAuditReport { sceneName = scene.name };
            UiReadabilityAuditRules.Run(report, scene);

            foreach (var finding in report.findings)
                Assert.IsFalse(finding.objectPath.Contains("UI_TEST_GOOD_TILE"), finding.ToString());
        }

        [Test]
        public void BuildProcessor_IsDeterministicAndWarnOnly()
        {
            var processor = new UiReadabilityBuildProcessor();
            Assert.AreEqual(850, processor.callbackOrder);

            var report = new SceneAuditReport { sceneName = "test" };
            report.Warning("UI_TEST", "warning");
            Assert.AreEqual(0, report.blockerCount);
            Assert.AreEqual(1, report.warningCount);
        }

        private GameObject NewObject(string name)
        {
            var go = new GameObject(name);
            _created.Add(go);
            return go;
        }

        private static void AddSimpleInteractable(GameObject go)
        {
            // Tests intentionally avoid a direct XRI asmdef dependency; production Editor already owns
            // the package reference. Reflection here verifies the installed package type and creates the
            // same component without widening the entire test assembly's reference surface.
            System.Type type = System.Type.GetType(
                "UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable, Unity.XR.Interaction.Toolkit");
            Assert.IsNotNull(type, "XRI 3.x XRSimpleInteractable type must be installed");
            go.AddComponent(type);
        }

        private void AddReadableLabel(Transform parent, Vector3 localPosition)
        {
            var label = NewObject(parent.name + "_LABEL");
            label.transform.SetParent(parent, false);
            label.transform.localPosition = localPosition;
            var text = label.AddComponent<TextMesh>();
            text.text = "SELECT";
            text.characterSize = 0.025f;
            text.fontSize = 64;
        }

        private static bool HasFinding(SceneAuditReport report, string code, string pathToken)
        {
            foreach (var finding in report.findings)
                if (finding.code == code && finding.objectPath.Contains(pathToken))
                    return true;
            return false;
        }
    }
}
#endif
