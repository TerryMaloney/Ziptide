using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE CANALS AS A ROUTE (⚖ Terry: the stalker interacts with the player's BOAT in the canals).
    /// Two rules make that real and both fail invisibly on a headset: the skiff has to stay in
    /// water it can actually travel, and the stalker has to escalate over trips instead of
    /// ambushing on trip one. These pin both — including the promises that the boat is never
    /// teleported or trapped and the creature is never lethal.
    /// </summary>
    public sealed class CanalWaterwayTests
    {
        private const float RingRadius = 74f;
        private const float RingWidth = 9f;

        private static List<CanalRect> Rects() => new List<CanalRect>
        {
            new CanalRect(new Vector3(-14f, 0f, 16f), new Vector2(14f, 30f)),
            new CanalRect(new Vector3(24f, 0f, -41f), new Vector2(24f, 6f)),
        };

        // ── the water lock ────────────────────────────────────────────────────────

        [Test]
        public void RingCanal_IsNavigableAllTheWayAround()
        {
            for (float deg = 0f; deg < 360f; deg += 15f)
            {
                float a = deg * Mathf.Deg2Rad;
                var p = new Vector3(Mathf.Cos(a) * RingRadius, 0f, Mathf.Sin(a) * RingRadius);
                Assert.IsTrue(CanalWaterCore.InRingCanal(p, RingRadius, RingWidth),
                    "the ring canal is the through-route; it must be navigable at " + deg + "°");
            }
        }

        [Test]
        public void DryLand_IsNotNavigable()
        {
            Assert.IsFalse(CanalWaterCore.IsNavigable(Vector3.zero, Rects(), RingRadius, RingWidth),
                "the city centre is not water");
            Assert.IsFalse(CanalWaterCore.IsNavigable(new Vector3(0f, 0f, 40f), Rects(), RingRadius, RingWidth));
        }

        [Test]
        public void AuthoredCanalRectangles_AreNavigable()
        {
            Assert.IsTrue(CanalWaterCore.IsNavigable(new Vector3(-14f, 0f, 16f), Rects(), RingRadius, RingWidth));
            Assert.IsTrue(CanalWaterCore.IsNavigable(new Vector3(24f, 0f, -41f), Rects(), RingRadius, RingWidth));
        }

        [Test]
        public void CorrectionIsZeroWhileAfloat_AndPointsBackWhenAground()
        {
            var afloat = new Vector3(RingRadius, 0f, 0f);
            Assert.AreEqual(Vector3.zero, CanalWaterCore.CorrectionDirection(afloat, Rects(), RingRadius, RingWidth));

            // Outside the ring: the push must point INWARD, back toward the channel.
            var outside = new Vector3(RingRadius + 20f, 0f, 0f);
            var push = CanalWaterCore.CorrectionDirection(outside, Rects(), RingRadius, RingWidth);
            Assert.Less(push.x, 0f, "a hull beached outside the ring is nudged back in");
            Assert.AreEqual(0f, push.y, "the correction is horizontal — never a lift");

            // Inside the ring: outward.
            var inside = new Vector3(RingRadius - 30f, 0f, 0f);
            Assert.Greater(CanalWaterCore.CorrectionDirection(inside, Rects(), RingRadius, RingWidth).x, 0f);
        }

        [Test]
        public void CorrectionIsAlwaysAUnitNudge_NeverATeleport()
        {
            for (float x = -200f; x <= 200f; x += 17f)
                for (float z = -200f; z <= 200f; z += 23f)
                {
                    var push = CanalWaterCore.CorrectionDirection(new Vector3(x, 0f, z), Rects(), RingRadius, RingWidth);
                    if (push == Vector3.zero) continue;
                    Assert.AreEqual(1f, push.magnitude, 0.001f,
                        "the lock pushes at a fixed speed; a variable-length vector would be a yank");
                }
        }

        [Test]
        public void NoWaterAuthored_MeansNoLock()
        {
            Assert.IsFalse(CanalWaterCore.IsNavigable(Vector3.zero, null, 0f, 0f));
            Assert.AreEqual(Vector3.zero, CanalWaterCore.CorrectionDirection(Vector3.zero, null, 0f, 0f),
                "a world with no canals must not push the boat anywhere");
        }

        // ── the stalker ───────────────────────────────────────────────────────────

        [Test]
        public void RideOne_IsOnlyEverTheShadow()
        {
            Assert.AreEqual(StalkerStage.Shadow,
                CanalStalkerCore.Stage(1, 5f, boatPresent: true, stunned: false),
                "the first trip must never touch the player — that is the whole 'escort, not ambush' rule");
            Assert.IsFalse(CanalStalkerCore.Contacts(StalkerStage.Shadow));
        }

        [Test]
        public void RepeatRides_EscalateToTheBump()
        {
            Assert.AreEqual(StalkerStage.Bump,
                CanalStalkerCore.Stage(CanalStalkerCore.RidesBeforeBump, 5f, true, false));
        }

        [Test]
        public void LingeringEscalatesWithinASingleRide()
        {
            Assert.AreEqual(StalkerStage.Bump,
                CanalStalkerCore.Stage(1, CanalStalkerCore.LingerSecondsToEscalate + 1f, true, false));
        }

        [Test]
        public void BlockingNeedsBothHistoryAndLingering()
        {
            Assert.AreEqual(StalkerStage.Block, CanalStalkerCore.Stage(
                CanalStalkerCore.RidesBeforeBlock, CanalStalkerCore.LingerSecondsToEscalate + 1f, true, false));
            Assert.AreNotEqual(StalkerStage.Block,
                CanalStalkerCore.Stage(CanalStalkerCore.RidesBeforeBlock, 1f, true, false),
                "just being a repeat visitor must not get the channel blocked");
        }

        [Test]
        public void NoBoat_MeansNoStalker()
        {
            Assert.AreEqual(StalkerStage.Absent, CanalStalkerCore.Stage(9, 999f, boatPresent: false, stunned: false),
                "on foot it is not there at all — this animal plays against the hull");
        }

        [Test]
        public void StunningItAlwaysWorks()
        {
            Assert.AreEqual(StalkerStage.Shadow, CanalStalkerCore.Stage(9, 999f, true, stunned: true),
                "the player's counter has to work at every escalation, or the block becomes a trap");
        }

        [Test]
        public void TheBlockAlwaysYields_AndTheBumpStaysComfortable()
        {
            Assert.Greater(CanalStalkerCore.BlockYieldSeconds, 0f);
            Assert.Less(CanalStalkerCore.BlockYieldSeconds, 15f, "a long block is a trap wearing a story's clothes");
            Assert.LessOrEqual(CanalStalkerCore.BumpYawDegrees, 15f,
                "a bigger shove than this is a comfort event, not a warning");
        }
    }
}
