using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// EXCELLENCE_MAP gate-gap closures #3 and #5 (2026-07-10):
    ///   #3 STORY-BEAT COVERAGE — every shipped story world must carry authored jobs/beats
    ///      (`WorldJobLibrary.HasJobsFor`). A world without beats is scenery, not story.
    ///   #5 BOARD STALENESS — a 🟡 (in-progress) board row whose claim date is >14 days old is a
    ///      broken promise: either finish it, re-date it with a fresh HANDOFF note, or mark it
    ///      🔴 blocked / ⬜ unclaimed. Stale claims silently block other operators from a lane.
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
        public void GateGap5_NoBoardClaim_RotsSilently()
        {
            // 🟡 rows carry their claim date in prose ("CLAIMED … (2026-07-09)"). Older than 14
            // days = stale. Tests may read the clock (the no-wall-clock law binds GENERATORS).
            string docs = Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs"));
            if (!Directory.Exists(docs)) Assert.Ignore("docs/ not present in this checkout");

            var datePattern = new Regex(@"20\d\d-\d\d-\d\d");
            var stale = new List<string>();
            foreach (var board in Directory.GetFiles(docs, "SPRINT*.md"))
                foreach (var line in File.ReadAllLines(board))
                {
                    if (!line.Contains("🟡")) continue;
                    DateTime newest = DateTime.MinValue;
                    foreach (Match m in datePattern.Matches(line))
                        if (DateTime.TryParse(m.Value, out var d) && d > newest) newest = d;
                    if (newest == DateTime.MinValue) continue;   // undated rows: not gated in v1
                    if ((DateTime.UtcNow - newest).TotalDays > 14)
                        stale.Add(Path.GetFileName(board) + ": " +
                                  line.Trim().Substring(0, Math.Min(90, line.Trim().Length)));
                }

            Assert.IsEmpty(stale,
                "🟡 board claims older than 14 days — finish, re-date (with a HANDOFF note), or " +
                "release the row (🔴/⬜):\n  " + string.Join("\n  ", stale));
        }
    }
}
