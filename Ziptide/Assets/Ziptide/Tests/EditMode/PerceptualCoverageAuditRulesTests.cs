using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Audit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class PerceptualCoverageAuditRulesTests
    {
        [Test]
        public void DeliberatelyInvisibleItem_FiresCoverageBlocker()
        {
            GameObject root = null;
            try
            {
                root = new GameObject("InvisibleKeyItem");
                ItemRuntime item = root.AddComponent<ItemRuntime>();
                var report = new SceneAuditReport { sceneName = "BrokenCoverageFixture" };

                PerceptualCoverageAuditRules.ValidateOwner(item, true, report);

                Assert.That(Contains(report, PerceptualCoverageAuditRules.ZeroRenderer), Is.True);
                Assert.That(report.blockerCount, Is.EqualTo(1));
            }
            finally
            {
                if (root != null) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void BarePrimitiveItem_IsVisibleButRecordedAsDebt()
        {
            GameObject root = null;
            try
            {
                root = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ItemRuntime item = root.AddComponent<ItemRuntime>();
                var report = new SceneAuditReport { sceneName = "FallbackCoverageFixture" };

                PerceptualCoverageAuditRules.ValidateOwner(item, true, report);

                Assert.That(report.blockerCount, Is.Zero);
                Assert.That(Contains(report, PerceptualCoverageAuditRules.PrimitiveFallback), Is.True);
                Assert.That(report.warningCount, Is.EqualTo(1));
            }
            finally
            {
                if (root != null) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void MultiPartItem_PassesCoverageGate()
        {
            GameObject root = null;
            try
            {
                root = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ItemRuntime item = root.AddComponent<ItemRuntime>();
                GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                child.transform.SetParent(root.transform, false);
                var report = new SceneAuditReport { sceneName = "RichCoverageFixture" };

                PerceptualCoverageAuditRules.ValidateOwner(item, true, report);

                Assert.That(report.blockerCount, Is.Zero);
                Assert.That(report.warningCount, Is.Zero);
            }
            finally
            {
                if (root != null) Object.DestroyImmediate(root);
            }
        }

        private static bool Contains(SceneAuditReport report, string code)
        {
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].code == code) return true;
            return false;
        }
    }
}
