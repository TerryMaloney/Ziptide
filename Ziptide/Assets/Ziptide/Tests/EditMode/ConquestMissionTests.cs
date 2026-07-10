using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// TIDEFRONT B3 — the risk-mission ("gulag") contract: offers are deterministic per world+side,
    /// the attempt state machine pays +2/−1/0 (mirrored for defenders), timeouts lose, declining and
    /// abandoning cost nothing, underdogs get better offers, and a mission-tilted battle resolves
    /// EXACTLY like a direct resolve with the same modifier (the pending battle adds no new rules).
    /// </summary>
    public class ConquestMissionTests
    {
        // toxic_city: attack = Scan (3 objectives), defense = Repair (4) — multi-objective on both
        // sides, which the abandon/overshoot tests below rely on.
        private static MissionAttempt Attempt(MissionSide side, bool underdog = false) =>
            new MissionAttempt(ConquestMissionLibrary.Offer("toxic_city", side, underdog));

        // ── Offers ──────────────────────────────────────────────────────────
        [Test]
        public void Offer_IsDeterministic_AndSideShaped()
        {
            var a1 = ConquestMissionLibrary.Offer("glass_shelf", MissionSide.Attack, false);
            var a2 = ConquestMissionLibrary.Offer("glass_shelf", MissionSide.Attack, false);
            Assert.AreEqual(a1.kind, a2.kind);
            Assert.AreEqual(a1.winTilt, a2.winTilt);
            Assert.AreEqual(a1.objectiveCount, a2.objectiveCount);

            // Sides draw from disjoint verb sets.
            Assert.Contains(a1.kind, new[] { MissionKind.Sabotage, MissionKind.Scan, MissionKind.Beacon },
                "attackers strike/scan/carry");
            var d = ConquestMissionLibrary.Offer("glass_shelf", MissionSide.Defense, false);
            Assert.Contains(d.kind, new[] { MissionKind.DroneDefense, MissionKind.Repair, MissionKind.SpaceDefense },
                "defenders shoot down, fix, or FLY");
        }

        [Test]
        public void Kinds_SpanTheWholeCatalog_AcrossTheGalaxy()
        {
            // The richness bar: shipping 2 of 5 contract kinds is a skeleton. Across the canonical
            // 12 worlds, every kind must actually be reachable by a player.
            var attack = new System.Collections.Generic.HashSet<MissionKind>();
            var defense = new System.Collections.Generic.HashSet<MissionKind>();
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
            {
                attack.Add(ConquestMissionLibrary.Offer(w.WorldId, MissionSide.Attack, false).kind);
                defense.Add(ConquestMissionLibrary.Offer(w.WorldId, MissionSide.Defense, false).kind);
            }
            Assert.AreEqual(3, attack.Count, "all three attack verbs appear in Ch.1–2");
            Assert.AreEqual(3, defense.Count, "all three defense verbs appear in Ch.1–2 (incl. the helm)");
            // Pinned samples (char-sum picks, verified offline).
            Assert.AreEqual(MissionKind.Scan, ConquestMissionLibrary.Offer("toxic_city", MissionSide.Attack, false).kind);
            Assert.AreEqual(MissionKind.Beacon, ConquestMissionLibrary.Offer("dry_cistern", MissionSide.Attack, false).kind);
            Assert.AreEqual(MissionKind.Sabotage, ConquestMissionLibrary.Offer("broadcast_tomb", MissionSide.Attack, false).kind);
            Assert.AreEqual(MissionKind.Repair, ConquestMissionLibrary.Offer("toxic_city", MissionSide.Defense, false).kind);
            // Terry's original ideal, finally real: defend some worlds FROM THE HELM.
            Assert.AreEqual(MissionKind.SpaceDefense, ConquestMissionLibrary.Offer("dry_cistern", MissionSide.Defense, false).kind);
            Assert.AreEqual("SpaceLane_Trial", ConquestSession.SceneForMission(
                ConquestMissionLibrary.Offer("dry_cistern", MissionSide.Defense, false)),
                "space-defense plays in the space lane, not the planet");
            Assert.AreEqual("W002_DryCistern", ConquestSession.SceneForMission(
                ConquestMissionLibrary.Offer("dry_cistern", MissionSide.Attack, false)),
                "ground contracts still play in the contested world");
        }

        [Test]
        public void Underdog_GetsABetterOffer()
        {
            var even = ConquestMissionLibrary.Offer("the_hum", MissionSide.Attack, false);
            var down = ConquestMissionLibrary.Offer("the_hum", MissionSide.Attack, true);
            Assert.AreEqual(ConquestMissionRules.WinTilt, even.winTilt);
            Assert.AreEqual(ConquestMissionRules.UnderdogWinTilt, down.winTilt);
            Assert.Greater(down.winTilt, even.winTilt, "anti-snowball: behind = better contracts");
        }

        // ── The attempt state machine ───────────────────────────────────────
        [Test]
        public void WinPath_PaysWinTilt()
        {
            var m = Attempt(MissionSide.Attack);
            m.Accept();
            for (int i = 0; i < m.mission.objectiveCount; i++) m.CompleteObjective();
            Assert.AreEqual(MissionPhase.Won, m.phase);
            Assert.AreEqual(ConquestMissionRules.WinTilt, m.ResultTilt());
        }

        [Test]
        public void Timeout_IsALoss_PaysLoseTilt()
        {
            var m = Attempt(MissionSide.Attack);
            m.Accept();
            m.Tick(m.mission.timeLimitSeconds + 1f);
            Assert.AreEqual(MissionPhase.Lost, m.phase);
            Assert.AreEqual(ConquestMissionRules.LoseTilt, m.ResultTilt());
        }

        [Test]
        public void DeclineAndAbandon_CostNothing()
        {
            var declined = Attempt(MissionSide.Attack);
            declined.Decline();
            Assert.AreEqual(0, declined.ResultTilt(), "decline: your odds stay the same");

            var walked = Attempt(MissionSide.Attack);
            walked.Accept();
            walked.CompleteObjective();       // partial progress doesn't count
            walked.Abandon();
            Assert.AreEqual(MissionPhase.Declined, walked.phase);
            Assert.AreEqual(0, walked.ResultTilt(), "walking out mid-mission = decline");
        }

        [Test]
        public void DefenderResults_AreMirrored()
        {
            var win = Attempt(MissionSide.Defense);
            win.Accept();
            for (int i = 0; i < win.mission.objectiveCount; i++) win.CompleteObjective();
            Assert.AreEqual(-ConquestMissionRules.WinTilt, win.ResultTilt(),
                "a won defense LOWERS the attacker's odds");

            var loss = Attempt(MissionSide.Defense);
            loss.Accept();
            loss.Tick(999f);
            Assert.AreEqual(-ConquestMissionRules.LoseTilt, loss.ResultTilt(),
                "a botched defense hands the attacker a point");
        }

        [Test]
        public void TerminalPhases_AreFrozen()
        {
            var m = Attempt(MissionSide.Attack);
            m.Accept();
            for (int i = 0; i < m.mission.objectiveCount + 3; i++) m.CompleteObjective();
            Assert.AreEqual(m.mission.objectiveCount, m.objectivesDone, "no overshoot after Won");
            m.Tick(9999f);
            Assert.AreEqual(MissionPhase.Won, m.phase, "the clock can't kill a finished mission");
            m.Decline();
            Assert.AreEqual(MissionPhase.Won, m.phase, "no un-winning");
        }

        // ── The resolution contract ─────────────────────────────────────────
        [Test]
        public void PendingBattle_ResolvesIdenticallyToADirectTiltedResolve()
        {
            // Two identical wars, same seed: one resolves directly with modifier +2, the other goes
            // through the full offer→accept→win pipeline. Reports must match field for field.
            var s1 = Skirmish(out var direct);
            direct.missionModifier = ConquestMissionRules.WinTilt;
            var r1 = ConquestResolver.Resolve(s1, direct, seed: 777);

            var s2 = Skirmish(out var flown);
            var attempt = new MissionAttempt(ConquestMissionLibrary.Offer("b", MissionSide.Attack, false));
            attempt.Accept();
            for (int i = 0; i < attempt.mission.objectiveCount; i++) attempt.CompleteObjective();
            flown.missionModifier = attempt.ResultTilt();
            var r2 = ConquestResolver.Resolve(s2, flown, seed: 777);

            Assert.AreEqual(r1.outcome, r2.outcome);
            Assert.AreEqual(r1.odds, r2.odds, 1e-6f);
            Assert.AreEqual(r1.roll, r2.roll, 1e-6f);
            Assert.AreEqual(r1.attackScore, r2.attackScore);
        }

        // ── The session holder ──────────────────────────────────────────────
        [Test]
        public void Session_KnowsWhichSceneShouldSpawnTheMission()
        {
            try
            {
                var attempt = new MissionAttempt(ConquestMissionLibrary.Offer("dry_cistern", MissionSide.Attack, false));
                attempt.Accept();
                ConquestSession.Pending = new PendingBattle { attempt = attempt, seed = 5 };
                Assert.IsTrue(ConquestSession.MissionActiveFor("W002_DryCistern"));
                Assert.IsFalse(ConquestSession.MissionActiveFor("W003_GlassShelf"));

                attempt.CompleteObjective(); attempt.CompleteObjective(); attempt.CompleteObjective();
                Assert.AreEqual(MissionPhase.Won, attempt.phase);
                Assert.IsFalse(ConquestSession.MissionActiveFor("W002_DryCistern"),
                    "a finished mission must not respawn on re-entry");
            }
            finally { ConquestSession.Clear(); }
        }

        [Test]
        public void EveryPlanet_MapsToAShippedScene()
        {
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
            {
                Assert.IsFalse(string.IsNullOrEmpty(w.SceneName), w.WorldId + " has no scene");
                Assert.AreEqual(w.SceneName, ConquestSession.SceneForPlanet(w.WorldId));
            }
        }

        private static ConquestState Skirmish(out AttackOrder order)
        {
            var s = new ConquestState();
            s.players.Add(new ConquestPlayer { playerId = 0, flux = 20, alloy = 20 });
            s.players.Add(new ConquestPlayer { playerId = 1 });
            var a = new PlanetNode { planetId = "a", ownerId = 0 };
            var b = new PlanetNode { planetId = "b", ownerId = 1, defenseLevel = 2 };
            a.adjacentPlanetIds.Add("b"); b.adjacentPlanetIds.Add("a");
            s.planets.Add(a); s.planets.Add(b);
            s.GetPlayer(0).fleetVesselIds.Add("pulse_frigate");
            order = new AttackOrder
            {
                attackerId = 0, fromPlanetId = "a", targetPlanetId = "b",
                vesselIds = new List<string> { "pulse_frigate" },
            };
            return s;
        }
    }
}
