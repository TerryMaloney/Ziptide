using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// EXCELLENCE_MAP gate-gap closures #3 and #5 (2026-07-10):
    ///   #3 STORY-BEAT COVERAGE — every shipped story world must carry authored jobs/beats
    ///      (`WorldJobLibrary.HasJobsFor`). A world without beats is scenery, not story.
    ///   #5 BOARD STALENESS — the >14-day 🟡 claim rule. The CHECK now lives in
    ///      tools/factory_governance_gate.py (seconds, in Fast Preflight) because it only reads
    ///      docs/**; the test below guards that the enforcement still exists and is still wired.
    /// </summary>
    public class GateGapTests
    {

        [Test]
        public void GateGap3_EveryStoryWorld_CarriesAuthoredBeats()
        {
            var missing = new List<string>();
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
            {
                // ToxicCity predates WorldJobLibrary — its beats live in ToxicCityContractBuilder
                // (the library's own header says it generalizes that builder). Covered, elsewhere.
                if (w.SceneName == "ToxicCity") continue;
                if (!Ziptide.Editor.Patching.WorldJobLibrary.HasJobsFor(w.SceneName))
                    missing.Add(w.SceneName);
            }
            Assert.IsEmpty(missing,
                "Shipped story worlds with NO authored jobs/beats (scenery, not story):\n  " +
                string.Join("\n  ", missing) + "\nAdd a Spec in WorldJobLibrary.SpecFor.");
        }

        [Test]
        public void GateGap5_BoardStaleness_IsEnforcedByThePythonPreflight()
        {
            // MOVED OUT OF UNITY 2026-07-28 (docs/FAST_LANE.md §4.2). This check only ever read
            // docs/**, but living here it cost a full Unity boot + the whole EditMode suite to
            // report a stale date — and it blocked the pipeline while it did. That is what caused
            // the 27-hour red streak of 2026-07-25/26. tools/factory_governance_gate.py performs
            // the IDENTICAL check (paused-lane manifest validation + the 14-day 🟡 claim rule) in
            // about one second, and runs in Fast Preflight. This test now guards only that the
            // enforcement still exists and is still wired, so it cannot be quietly deleted.
            string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
            string gate = Path.Combine(repo, "tools", "factory_governance_gate.py");
            Assert.IsTrue(File.Exists(gate), "The board-staleness gate is missing: " + gate);

            string gateSource = File.ReadAllText(gate);
            StringAssert.Contains("SPRINT_CLAIM_STALE", gateSource,
                "factory_governance_gate.py no longer raises SPRINT_CLAIM_STALE.");
            StringAssert.Contains("paused_sprint_lanes.json", gateSource,
                "factory_governance_gate.py no longer validates the recovery-pause manifest.");

            string preflight = Path.Combine(repo, ".github", "workflows", "fast-preflight.yml");
            if (!File.Exists(preflight)) Assert.Ignore("fast-preflight.yml not present in this checkout");
            StringAssert.Contains("factory_governance_gate.py", File.ReadAllText(preflight),
                "The board-staleness gate is no longer wired into Fast Preflight.");
        }

    }
}
