using NUnit.Framework;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The bot brain's competence contract (design: PVP_ARENA_AAA §A1) — provable without a headset:
    /// reaction time, hunt-to-last-known-position, cover discipline, dodge rolls, retreat, leading,
    /// aim error bounds, and full determinism from a seed.
    /// </summary>
    public class BotBrainTests
    {
        private static BotPerception See(float now, float dist = 10f, bool ready = true) => new BotPerception
        {
            Now = now, MyPos = new Vec3(0, 0, 0), MyHealth = 6, WeaponReady = ready,
            CanSeeTarget = true, TargetPos = new Vec3(0, 0, dist), TargetVel = Vec3.Zero,
            HasCover = true, NearestCoverPos = new Vec3(5, 0, 0),
            PatrolPoint = new Vec3(-5, 0, -5),
        };

        private static BotPerception Blind(float now) => new BotPerception
        {
            Now = now, MyPos = new Vec3(0, 0, 0), MyHealth = 6, WeaponReady = true,
            CanSeeTarget = false, HasCover = true, NearestCoverPos = new Vec3(5, 0, 0),
            PatrolPoint = new Vec3(-5, 0, -5),
        };

        [Test]
        public void ReactionTime_GatesEngagement()
        {
            var b = new BotBrain(BotProfileData.Regular, seed: 7); // reaction 0.6s
            b.Tick(See(now: 0f));
            Assert.AreEqual(BotState.Patrol, b.State, "sighted but not yet reacted");
            b.Tick(See(now: 0.5f));
            Assert.AreEqual(BotState.Patrol, b.State, "still inside reaction window");
            b.Tick(See(now: 0.7f));
            Assert.AreEqual(BotState.Engage, b.State, "reaction elapsed → engage");
        }

        [Test]
        public void LosLost_HuntsToLastKnownPosition_ThenSearches_ThenPatrols()
        {
            var b = new BotBrain(BotProfileData.Regular, seed: 7);
            b.Tick(See(0f)); b.Tick(See(1f));                       // engaged
            b.Tick(Blind(2f));                                      // transition tick
            Assert.AreEqual(BotState.Hunt, b.State);
            var d = b.Tick(Blind(2.1f));                            // first full Hunt tick
            Assert.AreEqual(10f, d.MoveTarget.Z, 0.01f, "moves to the last seen position");

            var atLkp = Blind(3f); atLkp.AtMoveTarget = true;
            b.Tick(atLkp);                                          // arrives — starts the search timer
            var later = Blind(3f + BotProfileData.Regular.SearchSeconds + 0.1f); later.AtMoveTarget = true;
            b.Tick(later);
            Assert.AreEqual(BotState.Patrol, b.State, "cold trail → back to patrol");
        }

        [Test]
        public void HeardFire_PullsPatrolIntoHunt()
        {
            var b = new BotBrain(BotProfileData.Regular, seed: 7);
            var p = Blind(0f); p.HeardFire = true; p.HeardFireAt = new Vec3(3, 0, 3);
            var d = b.Tick(p);
            Assert.AreEqual(BotState.Hunt, b.State);
            Assert.AreEqual(3f, d.MoveTarget.X, 0.01f);
        }

        [Test]
        public void Damaged_WithFullDiscipline_BreaksToCover_ZeroNever()
        {
            var disciplined = new BotBrain(BotProfileData.Nightmare, seed: 7); // discipline 1.0
            disciplined.Tick(See(0f)); disciplined.Tick(See(1f));
            disciplined.NotifyDamaged(1f);
            Assert.AreEqual(BotState.TakeCover, disciplined.State);

            var rookieProfile = BotProfileData.Rookie; rookieProfile.CoverDiscipline = 0f;
            var reckless = new BotBrain(rookieProfile, seed: 7);
            reckless.Tick(See(0f)); reckless.Tick(See(2f));
            reckless.NotifyDamaged(2f);
            Assert.AreEqual(BotState.Engage, reckless.State, "discipline 0 never covers");
        }

        [Test]
        public void CoverCycle_HideThenPeek()
        {
            var b = new BotBrain(BotProfileData.Nightmare, seed: 7); // hide 0.8 / peek 0.7
            b.Tick(See(0f)); b.Tick(See(1f));
            b.NotifyDamaged(1f);                                     // → TakeCover
            var atCover = See(1.1f); atCover.AtMoveTarget = true;
            b.Tick(atCover);                                         // arrival starts hide timer
            b.Tick(MakeAt(atCover, 2.0f));                           // 0.9s later > HideSeconds
            Assert.AreEqual(BotState.Peek, b.State);
            b.Tick(MakeAt(atCover, 2.8f));                           // peek window over
            Assert.AreNotEqual(BotState.Peek, b.State, "peek ends");
        }

        private static BotPerception MakeAt(BotPerception src, float now) { src.Now = now; return src; }

        [Test]
        public void LowHealth_Retreats_AndStays()
        {
            var b = new BotBrain(BotProfileData.Veteran, seed: 7);   // retreat below 2
            var hurt = See(0f); hurt.MyHealth = 2;
            b.Tick(hurt);
            Assert.AreEqual(BotState.Retreat, b.State);
            var healedView = See(1f); healedView.MyHealth = 2;
            b.Tick(healedView);
            Assert.AreEqual(BotState.Retreat, b.State, "retreat is sticky until respawn resets the brain");
        }

        [Test]
        public void Dodge_CertaintyAndNever()
        {
            var always = BotProfileData.Nightmare; always.DodgeChance = 1f;
            var b = new BotBrain(always, seed: 7);
            var threat = See(0f); threat.IncomingThreat = true; threat.ThreatVel = new Vec3(0, 0, -5);
            var d = b.Tick(threat);
            Assert.IsTrue(d.WantDodge);
            Assert.AreEqual(0f, d.DodgeDir.Z, 0.01f, "dodge is perpendicular to the incoming bolt");
            Assert.AreEqual(1f, System.Math.Abs(d.DodgeDir.X), 0.01f);

            var never = BotProfileData.Rookie; // dodge 0
            var b2 = new BotBrain(never, seed: 7);
            var d2 = b2.Tick(threat);
            Assert.IsFalse(d2.WantDodge);
        }

        [Test]
        public void Engage_HoldsTheBand()
        {
            var b = new BotBrain(BotProfileData.Regular, seed: 7);   // standoff 8 ± 1.5
            b.Tick(See(0f, dist: 14f));
            b.Tick(See(1f, dist: 14f));                              // transition into Engage
            Assert.AreEqual(BotState.Engage, b.State);
            var far = b.Tick(See(1.5f, dist: 14f));                  // first full Engage tick
            Assert.AreEqual(14f, far.MoveTarget.Z, 0.01f, "too far → closes toward the target");

            var close = b.Tick(See(2f, dist: 5f));
            Assert.IsTrue(close.MoveTarget.Z < 0f, "too close → backs away");

            var inBand = b.Tick(See(3f, dist: 8f));
            Assert.AreNotEqual(0f, inBand.StrafeSign, "in the band → holds and strafes");
        }

        [Test]
        public void Fire_RequiresReadyReactedAndRange()
        {
            var b = new BotBrain(BotProfileData.Regular, seed: 7);
            b.Tick(See(0f));
            var notReady = See(1f, ready: false);
            Assert.IsFalse(b.Tick(notReady).WantFire, "no charge → no fire");
            Assert.IsTrue(b.Tick(See(2f)).WantFire, "ready + reacted + in range → fires");
            Assert.IsFalse(b.Tick(See(3f, dist: 30f)).WantFire, "out of range → holds");
        }

        [Test]
        public void CloseRange_Rushes()
        {
            var b = new BotBrain(BotProfileData.Veteran, seed: 7);   // rush < 3.5
            b.Tick(See(0f)); b.Tick(See(1f));
            b.Tick(See(2f, dist: 3f));
            Assert.AreEqual(BotState.Rush, b.State);
            b.Tick(See(3f, dist: 9f));
            Assert.AreEqual(BotState.Engage, b.State, "target opened distance → back to engage");
        }

        [Test]
        public void Leading_ProjectsVelocity_OnlyWhenProfiled()
        {
            var moving = See(0f, dist: 10f); moving.TargetVel = new Vec3(2f, 0f, 0f);

            var vet = new BotBrain(BotProfileData.Veteran, seed: 7); // leads, 2.5° err
            var led = vet.ComputeAimPoint(moving, boltSpeed: 5f);    // t = 2s → +4m X before error
            Assert.IsTrue(led.X > 3f, "veteran leads the strafing target: " + led.X);

            var noErrNoLead = BotProfileData.Rookie; noErrNoLead.AimErrorDegrees = 0f;
            var rook = new BotBrain(noErrNoLead, seed: 7);
            var direct = rook.ComputeAimPoint(moving, boltSpeed: 5f);
            Assert.AreEqual(0f, direct.X, 0.01f, "rookie fires at the current position");
        }

        [Test]
        public void AimError_IsBoundedByTheCone()
        {
            var p = BotProfileData.Nightmare; p.AimErrorDegrees = 5f; p.LeadTargets = false;
            var b = new BotBrain(p, seed: 42);
            var view = See(0f, dist: 10f);
            float maxOff = 10f * (float)System.Math.Tan(5f * System.Math.PI / 180.0) + 0.01f;
            for (int i = 0; i < 200; i++)
            {
                var aim = b.ComputeAimPoint(view);
                Assert.LessOrEqual(System.Math.Abs(aim.X), maxOff, "lateral error inside the cone");
            }
        }

        [Test]
        public void Determinism_SameSeedSameStory()
        {
            var a = new BotBrain(BotProfileData.Veteran, seed: 99);
            var b = new BotBrain(BotProfileData.Veteran, seed: 99);
            for (int i = 0; i < 50; i++)
            {
                var p = See(i * 0.1f, dist: 6f + (i % 7));
                p.IncomingThreat = i % 5 == 0; p.ThreatVel = new Vec3(0, 0, -5);
                var da = a.Tick(p); var db = b.Tick(p);
                Assert.AreEqual(da.State, db.State, "tick " + i);
                Assert.AreEqual(da.WantFire, db.WantFire, "tick " + i);
                Assert.AreEqual(da.WantDodge, db.WantDodge, "tick " + i);
                Assert.AreEqual(da.AimPoint.X, db.AimPoint.X, 1e-5f, "tick " + i);
            }
        }

        [Test]
        public void Reposition_FlanksAroundTheTarget()
        {
            var p = BotProfileData.Nightmare; p.RepositionEvery = 1f;
            var b = new BotBrain(p, seed: 7);
            b.Tick(See(0f)); b.Tick(See(0.5f));                      // engage at 0.2 reaction
            b.Tick(See(2f));                                          // engage time > RepositionEvery → transition
            Assert.AreEqual(BotState.Reposition, b.State);
            var d = b.Tick(See(2.1f));                                // first full Reposition tick
            Assert.AreNotEqual(0f, d.MoveTarget.X, "flank point is off the direct line");
        }
    }
}
