using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content.Automation;
using Ziptide.Editor.Audit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1g — pins the belt gate: a sandbox-shaped floor sails through clean, oversized
    /// grids and cell counts block, authoring typos (out-of-bounds / duplicate cells) are caught in
    /// CI instead of silently no-opping on device, and the 4.1f save-identity integrity holds
    /// (no two floors may share a floorId; a persistent-less floor with content warns).
    /// </summary>
    public class AutomationAuditRulesTests
    {
        private readonly List<GameObject> _made = new List<GameObject>();

        private BeltFloorRuntime Floor(string name, int width, int depth, string floorId)
        {
            var go = new GameObject(name);
            _made.Add(go);
            var f = go.AddComponent<BeltFloorRuntime>();
            f.width = width; f.depth = depth; f.floorId = floorId;
            return f;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in _made) if (go != null) Object.DestroyImmediate(go);
            _made.Clear();
        }

        private static bool Has(SceneAuditReport r, string code)
            => r.findings.Exists(f => f.code == code);

        [Test]
        public void SandboxShapedFloor_PassesClean()
        {
            var f = Floor("CleanFloor", 8, 4, "sandbox_belt");
            f.AuthorPort(0, 1, BeltDir.East);
            f.AuthorBelt(1, 1, BeltDir.East);
            f.AuthorSplitter(2, 1, BeltDir.East);
            f.AuthorSink(3, 1, "scrap");
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.AreEqual(0, report.blockerCount, ReportText(report));
            Assert.AreEqual(0, report.warningCount, ReportText(report));
        }

        [Test]
        public void OversizedGrid_Blocks()
        {
            Floor("HugeFloor", 32, 9, "huge"); // 288 cells > cap 256
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_AREA_OVER_CAP"), ReportText(report));
        }

        [Test]
        public void BigButNotHugeGrid_WarnsOnly()
        {
            Floor("BigFloor", 10, 10, "big"); // 100 > target 64, <= cap
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_AREA_OVER_TARGET"), ReportText(report));
            Assert.AreEqual(0, report.blockerCount, ReportText(report));
        }

        [Test]
        public void OutOfBoundsAuthoredCell_Blocks()
        {
            var f = Floor("TypoFloor", 8, 4, "typo");
            f.AuthorBelt(8, 1, BeltDir.East); // x == width — silently no-ops at runtime
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_CELL_OOB"), ReportText(report));
        }

        [Test]
        public void DuplicateAuthoredCell_Warns()
        {
            var f = Floor("DupFloor", 8, 4, "dup");
            f.AuthorBelt(1, 1, BeltDir.East);
            f.AuthorBelt(1, 1, BeltDir.North); // overwrites the first at runtime
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_CELL_DUP"), ReportText(report));
        }

        [Test]
        public void ContentWithoutFloorId_Warns()
        {
            var f = Floor("Amnesiac", 8, 4, "");
            f.AuthorBelt(1, 1, BeltDir.East);
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_NO_FLOORID"), ReportText(report));
        }

        [Test]
        public void SharedFloorId_Blocks()
        {
            Floor("FloorA", 8, 4, "same_id");
            Floor("FloorB", 8, 4, "same_id");
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_FLOORID_DUP"), ReportText(report));
        }

        [Test]
        public void SceneCellTotal_OverCap_Blocks()
        {
            var f = Floor("Carpet", 16, 16, "carpet"); // area 256 = at cap (no area blocker)
            for (int z = 0; z < 16; z++)
                for (int x = 0; x < 16; x++)
                    f.AuthorBelt(x, z, BeltDir.East);
            var g = Floor("Overflow", 2, 1, "overflow");
            g.AuthorBelt(0, 0, BeltDir.East); // 257th authored cell in the scene
            var report = new SceneAuditReport();
            AutomationAuditRules.Run(report);
            Assert.IsTrue(Has(report, "BELT_CELLS_OVER_CAP"), ReportText(report));
            Assert.IsFalse(Has(report, "BELT_AREA_OVER_CAP"), "at-cap area must not block: " + ReportText(report));
        }

        private static string ReportText(SceneAuditReport r)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var f in r.findings) sb.AppendLine(f.ToString());
            return sb.ToString();
        }
    }
}
