using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Independent-review amendment gates: device evidence must survive logcat rotation, and the
    /// three rb26 probes must remain executable in the exact Golden checkpoint candidate.
    /// </summary>
    public sealed class RecoveryCheckpointEvidenceTests
    {
        [Test]
        public void DiagnosticRingPolicy_CapturesOnlyZiptideTags_AndRotatesAtBound()
        {
            Assert.IsTrue(PersistentDiagnosticRing.ShouldCapture("ZIPTIDE: BOOT_HOLD on"));
            Assert.IsTrue(PersistentDiagnosticRing.ShouldCapture("ZIPTIDE_DIAG {json}"));
            Assert.IsFalse(PersistentDiagnosticRing.ShouldCapture("ordinary Unity message"));
            Assert.IsFalse(PersistentDiagnosticRing.ShouldCapture(null));

            Assert.IsFalse(PersistentDiagnosticRing.ShouldRotate(90, 10, 100),
                "An exact-fit append must stay in the current segment.");
            Assert.IsTrue(PersistentDiagnosticRing.ShouldRotate(91, 10, 100),
                "The append that crosses the bound must rotate first.");
            Assert.IsFalse(PersistentDiagnosticRing.ShouldRotate(0, 101, 100),
                "A single first line is retained rather than causing an empty-file rotate loop.");
        }

        [Test]
        public void DiagnosticRingFormat_EscapesEmbeddedLines_AndRecordsTypeAndUtc()
        {
            var timestamp = new DateTimeOffset(2026, 7, 15, 12, 34, 56, TimeSpan.Zero);
            string line = PersistentDiagnosticRing.FormatLine(
                timestamp, LogType.Warning, "ZIPTIDE: alpha\r\nbeta");

            StringAssert.StartsWith("2026-07-15T12:34:56.0000000Z\tWarning\t", line);
            StringAssert.Contains("ZIPTIDE: alpha\\r\\nbeta", line);
            string payload = line.Substring(0, line.Length - Environment.NewLine.Length);
            Assert.IsFalse(payload.Contains("\r"), "A diagnostic payload may not create a second record.");
            Assert.IsFalse(payload.Contains("\n"), "A diagnostic payload may not create a second record.");
        }

        [Test]
        public void RecoveryGate_InstallsPersistentRingBeforeFirstExposureLog()
        {
            string path = Path.Combine(Application.dataPath,
                "Ziptide/Core/Runtime/Recovery/RecoveryRuntimeGate.cs");
            string source = File.ReadAllText(path);
            int install = source.IndexOf("PersistentDiagnosticRing.Install()", StringComparison.Ordinal);
            int exposure = source.IndexOf(
                "Debug.Log(BuildLogLine(\"RECOVERY_EXPOSURE\"))", StringComparison.Ordinal);

            Assert.GreaterOrEqual(install, 0, "Recovery boot does not install the persistent ring.");
            Assert.Greater(exposure, install,
                "The first exposure record can reach logcat before persistent capture is installed.");
        }

        [Test]
        public void GoldenCheckpoint_KeepsBoardRepairAndFlightEvidenceProbesAlive()
        {
            Assert.IsTrue(RecoveryExposureProfiles.GoldenSlice.Allows(RecoveryFeatureId.DevWarpBoard),
                "BOARD_PROBE is unreachable in the exact Golden checkpoint profile.");

            string root = Path.Combine(Application.dataPath, "Ziptide");
            int board = CountRuntimeLogCalls(root, "BOARD_PROBE");
            int repair = CountRuntimeLogCalls(root, "REPAIR_TRACE");
            int flight = CountRuntimeLogCalls(root, "FLIGHT_TRACE");

            Assert.GreaterOrEqual(board, 3,
                "BOARD_PROBE must retain hover/select plus periodic actual-ray evidence.");
            Assert.GreaterOrEqual(repair, 4,
                "REPAIR_TRACE no longer covers enough hops to distinguish state from presentation.");
            Assert.GreaterOrEqual(flight, 1,
                "FLIGHT_TRACE was removed from the ship-turn evidence path.");
        }

        private static int CountRuntimeLogCalls(string root, string tag)
        {
            var pattern = new Regex(
                @"Debug\.Log(?:Warning|Error)?\(\s*\""ZIPTIDE: " + Regex.Escape(tag),
                RegexOptions.CultureInvariant);
            int count = 0;
            foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string normalized = file.Replace('\\', '/');
                if (normalized.Contains("/Editor/") || normalized.Contains("/Tests/")) continue;
                count += pattern.Matches(File.ReadAllText(file)).Count;
            }
            return count;
        }
    }
}
