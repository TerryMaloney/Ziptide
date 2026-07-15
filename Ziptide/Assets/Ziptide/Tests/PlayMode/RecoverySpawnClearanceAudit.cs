using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Tests.PlayMode
{
    [Serializable]
    public sealed class RecoverySpawnClearanceFinding
    {
        public string severity;
        public string code;
        public string hierarchyPath;
        public string message;
    }

    [Serializable]
    public sealed class RecoverySpawnClearanceReport
    {
        public string schemaVersion = "1";
        public string label;
        public string capturedAtUtc;
        public string scene;
        public string rigPath;
        public string cameraPath;
        public bool floorFound;
        public string floorColliderPath;
        public float floorY;
        public float headY;
        public float headHeightAboveFloor;
        public float capsuleRadius;
        public float capsuleBottomY;
        public float capsuleTopY;
        public string[] torsoOverlapPaths;
        public List<RecoverySpawnClearanceFinding> findings =
            new List<RecoverySpawnClearanceFinding>();
    }

    /// <summary>
    /// Test-owned standing-volume proof for actual Golden arrivals. It verifies that the tracked head
    /// has a non-rig floor beneath it, is at a plausible standing height, and that a conservative
    /// torso capsule is not embedded in world collision. Trigger volumes and the persistent rig's own
    /// colliders are excluded; the world floor used by the downward ray is recorded explicitly.
    /// </summary>
    public static class RecoverySpawnClearanceAudit
    {
        private const float MinimumHeadHeight = 1.10f;
        private const float MaximumHeadHeight = 2.20f;
        private const float FloorProbeStartOffset = 0.25f;
        private const float FloorProbeDistance = 6f;
        private const float TorsoRadius = 0.22f;
        private const float TorsoBottomAboveFloor = 0.36f;
        private const float TorsoTopAboveFloor = 1.45f;

        public static RecoverySpawnClearanceReport Capture(
            Transform rigRoot,
            Camera headCamera,
            string label)
        {
            if (rigRoot == null) throw new ArgumentNullException(nameof(rigRoot));
            if (headCamera == null) throw new ArgumentNullException(nameof(headCamera));
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Spawn-clearance label is required.", nameof(label));

            var report = new RecoverySpawnClearanceReport
            {
                label = label,
                capturedAtUtc = DateTimeOffset.UtcNow.UtcDateTime.ToString("O"),
                scene = SceneManager.GetActiveScene().name,
                rigPath = RecoveryRuntimeCensus.HierarchyPath(rigRoot),
                cameraPath = RecoveryRuntimeCensus.HierarchyPath(headCamera.transform),
                headY = headCamera.transform.position.y,
                capsuleRadius = TorsoRadius,
                torsoOverlapPaths = Array.Empty<string>()
            };

            if (!TryFindWorldFloor(rigRoot, headCamera.transform.position, out RaycastHit floorHit))
            {
                AddFinding(
                    report,
                    "BLOCKER",
                    "SPAWN_FLOOR_MISSING",
                    report.cameraPath,
                    "No non-trigger world collider was found beneath the tracked head within " +
                    FloorProbeDistance.ToString("F1") + "m.");
                return report;
            }

            report.floorFound = true;
            report.floorColliderPath = RecoveryRuntimeCensus.HierarchyPath(floorHit.collider.transform);
            report.floorY = floorHit.point.y;
            report.headHeightAboveFloor = report.headY - report.floorY;
            if (report.headHeightAboveFloor < MinimumHeadHeight ||
                report.headHeightAboveFloor > MaximumHeadHeight)
            {
                AddFinding(
                    report,
                    "BLOCKER",
                    "SPAWN_HEAD_HEIGHT_INVALID",
                    report.cameraPath,
                    "Tracked head is " + report.headHeightAboveFloor.ToString("F3") +
                    "m above the floor; expected " + MinimumHeadHeight.ToString("F2") +
                    "m to " + MaximumHeadHeight.ToString("F2") + "m.");
            }

            Vector3 head = headCamera.transform.position;
            Vector3 bottom = new Vector3(head.x, report.floorY + TorsoBottomAboveFloor, head.z);
            Vector3 top = new Vector3(head.x, report.floorY + TorsoTopAboveFloor, head.z);
            report.capsuleBottomY = bottom.y;
            report.capsuleTopY = top.y;

            Collider[] overlaps = Physics.OverlapCapsule(
                bottom,
                top,
                TorsoRadius,
                ~0,
                QueryTriggerInteraction.Ignore);
            var overlapPaths = new List<string>();
            for (int i = 0; i < overlaps.Length; i++)
            {
                Collider collider = overlaps[i];
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                if (IsUnder(collider.transform, rigRoot)) continue;
                if (collider == floorHit.collider) continue;
                string path = RecoveryRuntimeCensus.HierarchyPath(collider.transform);
                if (!overlapPaths.Contains(path)) overlapPaths.Add(path);
            }
            overlapPaths.Sort(StringComparer.Ordinal);
            report.torsoOverlapPaths = overlapPaths.ToArray();
            for (int i = 0; i < overlapPaths.Count; i++)
            {
                AddFinding(
                    report,
                    "BLOCKER",
                    "SPAWN_TORSO_OCCLUDED",
                    overlapPaths[i],
                    "The standing torso capsule overlaps non-trigger world collision at the canonical arrival.");
            }

            return report;
        }

        public static string WriteArtifact(RecoverySpawnClearanceReport report, string stem)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            string directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "playmode-test-results",
                "recovery-spawn-clearance"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, SanitizeStem(stem) + ".json");
            File.WriteAllText(
                path,
                JsonUtility.ToJson(report, true) + Environment.NewLine,
                Encoding.UTF8);
            return path;
        }

        private static bool TryFindWorldFloor(
            Transform rigRoot,
            Vector3 headPosition,
            out RaycastHit floorHit)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                headPosition + Vector3.up * FloorProbeStartOffset,
                Vector3.down,
                FloorProbeDistance,
                ~0,
                QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                if (IsUnder(collider.transform, rigRoot)) continue;
                floorHit = hits[i];
                return true;
            }
            floorHit = default;
            return false;
        }

        private static bool IsUnder(Transform value, Transform root)
        {
            return value == root || value.IsChildOf(root);
        }

        private static void AddFinding(
            RecoverySpawnClearanceReport report,
            string severity,
            string code,
            string hierarchyPath,
            string message)
        {
            report.findings.Add(new RecoverySpawnClearanceFinding
            {
                severity = severity,
                code = code,
                hierarchyPath = hierarchyPath,
                message = message
            });
        }

        private static string SanitizeStem(string stem)
        {
            if (string.IsNullOrWhiteSpace(stem)) stem = "spawn-clearance";
            var builder = new StringBuilder(stem.Length);
            for (int i = 0; i < stem.Length; i++)
            {
                char c = stem[i];
                builder.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            }
            return builder.ToString();
        }
    }
}
