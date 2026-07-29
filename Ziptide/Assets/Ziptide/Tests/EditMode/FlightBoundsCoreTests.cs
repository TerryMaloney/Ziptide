using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests
{
    /// <summary>
    /// THE BOUNDS LADDER's laws (⚖ Terry, 2026-07-29: auto-correction plus a warning from RILL,
    /// with enough variety that she does not go stale). The rules pinned here are the ones a later
    /// tuning pass could quietly break: warned-before-corrected, exploring-is-not-a-mistake, and
    /// no correction ever rotates the ship.
    /// </summary>
    public class FlightBoundsCoreTests
    {
        private static readonly Vector3[] Path =
        {
            Vector3.zero,
            new Vector3(0f, 3f, 60f),
            new Vector3(14f, 7f, 130f),
        };

        private static FlightBoundsParams P => FlightBoundsParams.Default;

        // ── The corridor ───────────────────────────────────────────────────────────────────────

        [Test]
        public void OnTheLine_IsClear()
        {
            var s = FlightBoundsSample.Empty;
            s.CorridorDistance = FlightBoundsCore.CorridorDistance(new Vector3(0f, 1.5f, 30f), Path);
            Assert.Less(s.CorridorDistance, 1f);
            Assert.IsTrue(FlightBoundsCore.Evaluate(new Vector3(0f, 1.5f, 30f), s, P).IsClear);
        }

        [Test]
        public void PastTheLastRing_TheCorridorKeepsRunning_SoTheOverrunIsNotStraying()
        {
            // 400 m beyond the final node, straight on: that is THE OVERRUN, which is where the
            // salvage and the artifact are. Treating it as "off the lane" would nag through the
            // whole discovery.
            Vector3 onward = Path[2] + (Path[2] - Path[1]).normalized * 400f;
            Assert.Less(FlightBoundsCore.CorridorDistance(onward, Path), 1f);
        }

        [Test]
        public void BehindTheDock_DoesNotExtend_SoLeavingOutTheBackIsStraying()
        {
            Vector3 behind = new Vector3(0f, 0f, -400f);
            Assert.Greater(FlightBoundsCore.CorridorDistance(behind, Path), 300f);
        }

        [Test]
        public void StrayingOffTheSweptLane_IsAdvisoryForeverAndNeverPushes()
        {
            var s = FlightBoundsSample.Empty;
            var p = P;
            p.laneRadius = 100000f; // isolate the corridor from the outer sphere

            foreach (float distance in new[] { 120f, 400f, 5000f })
            {
                s.CorridorDistance = distance;
                var r = FlightBoundsCore.Evaluate(new Vector3(distance, 0f, 30f), s, p);
                Assert.AreEqual(FlightBoundKind.Corridor, r.Kind);
                Assert.AreEqual(FlightBoundLevel.Advisory, r.Level,
                    "wandering off the swept lane must never escalate — exploring is not a mistake");
                Assert.AreEqual(Vector3.zero, r.Push);
            }
        }

        // ── Hulls ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void ClosingOnAHull_WarnsBeforeItCorrects()
        {
            var s = FlightBoundsSample.Empty;
            s.StructureAway = Vector3.right;

            s.StructureClearance = 8f;
            Assert.AreEqual(FlightBoundLevel.Advisory,
                FlightBoundsCore.Evaluate(Vector3.zero, s, P).Level);

            s.StructureClearance = 3.5f;
            Assert.AreEqual(FlightBoundLevel.Correcting,
                FlightBoundsCore.Evaluate(Vector3.zero, s, P).Level);

            s.StructureClearance = 1f;
            Assert.AreEqual(FlightBoundLevel.Hard,
                FlightBoundsCore.Evaluate(Vector3.zero, s, P).Level);
        }

        [Test]
        public void FlyingTheMiddleOfTheSweptLane_IsSilent()
        {
            // The debris field is authored to stand off ~14 m, and the biggest piece is ~3 m of
            // radius — so the normal act of flying the course must produce NO structure warning.
            // A companion who narrates ordinary flying is worse than one who says nothing.
            var s = FlightBoundsSample.Empty;
            s.StructureClearance = 11f;
            s.StructureAway = Vector3.right;
            Assert.IsTrue(FlightBoundsCore.Evaluate(Vector3.zero, s, P).IsClear);
        }

        [Test]
        public void AnAdvisoryNeverMovesTheShip()
        {
            var s = FlightBoundsSample.Empty;
            s.StructureClearance = 8f;
            s.StructureAway = Vector3.right;
            var r = FlightBoundsCore.Evaluate(Vector3.zero, s, P);

            var before = new FlightState { position = Vector3.zero, speed = 30f };
            var after = FlightBoundsCore.Apply(before, r, P, 0.1f);
            Assert.AreEqual(before.position, after.position);
            Assert.AreEqual(before.speed, after.speed);
        }

        [Test]
        public void ACorrectionPushesAwayAndBleedsSpeed()
        {
            var s = FlightBoundsSample.Empty;
            s.StructureClearance = 4f;
            s.StructureAway = Vector3.right;
            var r = FlightBoundsCore.Evaluate(Vector3.zero, s, P);

            var after = FlightBoundsCore.Apply(
                new FlightState { position = Vector3.zero, speed = 30f }, r, P, 0.5f);
            Assert.Greater(after.position.x, 0.1f);
            Assert.Less(after.speed, 30f);
        }

        [Test]
        public void NoCorrectionEverRotatesTheShip()
        {
            // Comfort law: yaw is snap-only and only the player snaps it. A bounds push that
            // touched heading would swing the horizon under a player who did not ask for it.
            var s = FlightBoundsSample.Empty;
            s.StructureClearance = 0.5f;
            s.StructureAway = Vector3.up;
            var r = FlightBoundsCore.Evaluate(Vector3.zero, s, P);

            var before = new FlightState { position = Vector3.zero, speed = 30f, yawDeg = 91f, pitchDeg = -12f };
            var after = FlightBoundsCore.Apply(before, r, P, 0.2f);
            Assert.AreEqual(before.yawDeg, after.yawDeg);
            Assert.AreEqual(before.pitchDeg, after.pitchDeg);
            Assert.AreEqual(before.rollDeg, after.rollDeg);
            Assert.AreEqual(before.rollDirection, after.rollDirection);
        }

        // ── Gates ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void TheGateOnlyBitesInsideItsSlab()
        {
            var center = new Vector3(0f, 3f, 60f);
            Assert.IsFalse(FlightBoundsCore.TryGateFill(new Vector3(5.9f, 3f, 40f), center,
                Vector3.forward, 6f, 4f, out _, out _));
            Assert.IsTrue(FlightBoundsCore.TryGateFill(new Vector3(5.9f, 3f, 61f), center,
                Vector3.forward, 6f, 4f, out float fill, out _));
            Assert.AreEqual(0.983f, fill, 0.01f);
        }

        [Test]
        public void DeadCentreOfTheBore_IsClear_AndTheRimCorrectsTowardTheAxis()
        {
            var s = FlightBoundsSample.Empty;
            s.GateFill01 = 0f;
            s.GateOutward = Vector3.right;
            Assert.IsTrue(FlightBoundsCore.Evaluate(Vector3.zero, s, P).IsClear);

            s.GateFill01 = 0.95f;
            var r = FlightBoundsCore.Evaluate(Vector3.zero, s, P);
            Assert.AreEqual(FlightBoundKind.Gate, r.Kind);
            Assert.AreEqual(FlightBoundLevel.Correcting, r.Level);
            Assert.AreEqual(Vector3.left, r.Push, "the gate pushes back toward the middle of the hole");
        }

        // ── Ground and the outer sphere ────────────────────────────────────────────────────────

        [Test]
        public void GroundIsOffWhenTheLaneHasNoFloor()
        {
            var p = P;
            Assert.IsFalse(p.hasGround, "orbit has no floor — the default must not invent one");
            Assert.IsTrue(FlightBoundsCore.Evaluate(new Vector3(0f, -900f, 0f),
                FlightBoundsSample.Empty, p).Kind != FlightBoundKind.Ground);
        }

        [Test]
        public void WithAFloor_DroppingTowardItWarnsThenLifts()
        {
            var p = P;
            p.hasGround = true;
            p.groundY = 0f;

            Assert.AreEqual(FlightBoundLevel.Advisory,
                FlightBoundsCore.Evaluate(new Vector3(0f, 40f, 0f), FlightBoundsSample.Empty, p).Level);

            var r = FlightBoundsCore.Evaluate(new Vector3(0f, 15f, 0f), FlightBoundsSample.Empty, p);
            Assert.AreEqual(FlightBoundKind.Ground, r.Kind);
            Assert.AreEqual(FlightBoundLevel.Correcting, r.Level);
            Assert.AreEqual(Vector3.up, r.Push);
        }

        [Test]
        public void TheOuterSphereWarnsLongBeforeFlightModelsHardClamp()
        {
            var p = P;
            float advisoryAt = p.laneRadius * p.deepAdvisoryFraction;
            var r = FlightBoundsCore.Evaluate(new Vector3(0f, 0f, advisoryAt + 10f),
                FlightBoundsSample.Empty, p);
            Assert.AreEqual(FlightBoundKind.Deep, r.Kind);
            Assert.AreEqual(FlightBoundLevel.Advisory, r.Level,
                "the silent 1.8 km snap must never be the first thing the pilot learns about");
            Assert.Less(advisoryAt, p.laneRadius);
        }

        [Test]
        public void TheWorstTierWins()
        {
            var s = FlightBoundsSample.Empty;
            s.CorridorDistance = 5000f;      // advisory
            s.StructureClearance = 4f;       // correcting
            s.StructureAway = Vector3.right;
            Assert.AreEqual(FlightBoundKind.Structure,
                FlightBoundsCore.Evaluate(Vector3.zero, s, P).Kind);
        }

        // ── Ring facing ────────────────────────────────────────────────────────────────────────

        [Test]
        public void ARingIsSquaredToTheTrajectory_NotToTheWorld()
        {
            // Node 1 sits on a path that turns, so its facing must be the bisector — neither the
            // incoming nor the outgoing segment, and certainly not +Z.
            var axis = FlightBoundsCore.PathAxis(Path, 1);
            Assert.AreEqual(1f, axis.magnitude, 0.001f);
            Assert.Greater(axis.z, 0.5f);
            Assert.Greater(axis.x, 0.01f, "the path turns right after node 1; the ring must lean into it");
            Assert.AreNotEqual(Vector3.forward, axis);
        }

        [Test]
        public void TheFirstAndLastNodesUseTheirOneSegment()
        {
            Assert.Greater(Vector3.Dot(FlightBoundsCore.PathAxis(Path, 0),
                (Path[1] - Path[0]).normalized), 0.9999f);
            Assert.Greater(Vector3.Dot(FlightBoundsCore.PathAxis(Path, 2),
                (Path[2] - Path[1]).normalized), 0.9999f);
        }
    }
}
