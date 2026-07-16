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
    ///      broken promise: either finish it, re-date it with a fresh HANDOFF note, mark it
    ///      🔴 blocked / ⬜ unclaimed, or record an explicit recovery pause with a reason and resume gate.
    /// </summary>
    public class GateGapTests
    {
        [Serializable]
        private sealed class PausedSprintLaneManifest
        {
            public string schemaVersion;
            public PausedSprintLane[] paused;
        }

        [Serializable]
        private sealed class PausedSprintLane
        {
            public string board;
            public string pausedSince;
            public string reason;
            public string resumeGate;
        }

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

            HashSet<string> explicitlyPaused = LoadAndValidatePausedLanes(docs);
            var datePattern = new Regex(@"20\d\d-\d\d-\d\d");
            var stale = new List<string>();
            foreach (var board in Directory.GetFiles(docs, "SPRINT*.md"))
            {
                string boardName = Path.GetFileName(board);
                if (explicitlyPaused.Contains(boardName)) continue;

                foreach (var line in File.ReadAllLines(board))
                {
                    if (!line.Contains("🟡")) continue;
                    DateTime newest = DateTime.MinValue;
                    foreach (Match m in datePattern.Matches(line))
                        if (DateTime.TryParse(m.Value, out var d) && d > newest) newest = d;
                    if (newest == DateTime.MinValue) continue;   // undated rows: not gated in v1
                    if ((DateTime.UtcNow - newest).TotalDays > 14)
                        stale.Add(boardName + ": " +
                                  line.Trim().Substring(0, Math.Min(90, line.Trim().Length)));
                }
            }

            Assert.IsEmpty(stale,
                "🟡 board claims older than 14 days — finish, re-date (with a HANDOFF note), " +
                "release the row (🔴/⬜), or add a validated recovery pause:\n  " +
                string.Join("\n  ", stale));
        }

        private static HashSet<string> LoadAndValidatePausedLanes(string docsDirectory)
        {
            string path = Path.Combine(docsDirectory, "recovery", "paused_sprint_lanes.json");
            Assert.IsTrue(File.Exists(path),
                "Recovery pause manifest is missing: " + path);

            PausedSprintLaneManifest manifest = JsonUtility.FromJson<PausedSprintLaneManifest>(
                File.ReadAllText(path));
            Assert.IsNotNull(manifest, "Recovery pause manifest did not deserialize.");
            Assert.AreEqual("1", manifest.schemaVersion,
                "Unsupported recovery pause manifest schema.");
            Assert.IsNotNull(manifest.paused,
                "Recovery pause manifest omitted its paused array.");

            var result = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < manifest.paused.Length; i++)
            {
                PausedSprintLane lane = manifest.paused[i];
                Assert.IsNotNull(lane, "Recovery pause entry " + i + " is null.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(lane.board),
                    "Recovery pause entry " + i + " has no board.");
                Assert.IsTrue(result.Add(lane.board),
                    "Recovery pause manifest repeats board " + lane.board + ".");
                Assert.IsTrue(File.Exists(Path.Combine(docsDirectory, lane.board)),
                    "Recovery pause references missing board " + lane.board + ".");
                Assert.IsTrue(DateTime.TryParse(lane.pausedSince, out DateTime pausedSince),
                    "Recovery pause has invalid pausedSince for " + lane.board + ".");
                Assert.LessOrEqual(pausedSince.Date, DateTime.UtcNow.Date,
                    "Recovery pause is future-dated for " + lane.board + ".");
                Assert.IsFalse(string.IsNullOrWhiteSpace(lane.reason),
                    "Recovery pause has no reason for " + lane.board + ".");
                Assert.IsFalse(string.IsNullOrWhiteSpace(lane.resumeGate),
                    "Recovery pause has no resume gate for " + lane.board + ".");
            }
            return result;
        }
    }
}
