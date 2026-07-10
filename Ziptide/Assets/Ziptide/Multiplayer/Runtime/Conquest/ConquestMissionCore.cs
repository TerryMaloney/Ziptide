using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Conquest
{
    public enum MissionSide { Attack, Defense }

    /// <summary>The contract vocabulary (the spec's full catalog, not just two): attackers draw
    /// Sabotage / Scan / Beacon, defenders draw DroneDefense / Repair — each a different VERB
    /// (shoot · hold ground · carry · shoot flyers · hands-on fix).</summary>
    public enum MissionKind { Sabotage, DroneDefense, Scan, Beacon, Repair, SpaceDefense }

    public enum MissionPhase { Offered, Accepted, Won, Lost, Declined }

    /// <summary>All the risk-mission knobs in one place (the ConquestRules idiom).</summary>
    public static class ConquestMissionRules
    {
        public const int WinTilt = 2;            // ≈ +10% odds (each net point ≈ 5%)
        public const int UnderdogWinTilt = 3;    // behind players get better offers (anti-snowball)
        public const int LoseTilt = -1;          // a botched raid stings — but less than a win helps
        public const float TimeLimitSeconds = 150f;   // "a couple minutes of gameplay"
        public const int SabotageObjectives = 3;      // shield pylons to disable
        public const int DroneDefenseObjectives = 5;  // scout drones to shoot down
        public const int ScanObjectives = 3;          // grid nodes to hold position at
        public const int BeaconObjectives = 1;        // one beacon, carried to the uplink pad
        public const int RepairObjectives = 4;        // arcing conduits to fix by hand
        public const int SpaceDefenseObjectives = 3;  // interceptors to disable FROM THE HELM
    }

    /// <summary>One offered contract: what you'd play, where, and what winning/losing is worth.</summary>
    [Serializable]
    public class ConquestMission
    {
        public string planetId = "";
        public MissionSide side;
        public MissionKind kind;
        public int objectiveCount;
        public float timeLimitSeconds;
        public int winTilt;   // magnitude; sign is applied by MissionAttempt.ResultTilt per side
        public int loseTilt;  // magnitude (negative), same convention
        public string title = "";
        public string brief = "";
    }

    /// <summary>
    /// TIDEFRONT B3 — the "gulag": before a battle resolves, either side may fly an OPTIONAL 2–3
    /// minute VR mission in the actual contested world to tilt the odds. Decline = odds unchanged.
    /// Deterministic: the same planet + side + standing always produces the same offer.
    /// </summary>
    public static class ConquestMissionLibrary
    {
        public static ConquestMission Offer(string planetId, MissionSide side, bool isUnderdog)
        {
            // Deterministic per world+side (a char-sum, so the pick is reasoned about offline):
            // every planet always offers the same contract, and the catalog varies ACROSS planets.
            int charsum = 0;
            if (!string.IsNullOrEmpty(planetId)) foreach (char c in planetId) charsum += c;
            MissionKind kind = side == MissionSide.Attack
                ? (charsum % 3 == 0 ? MissionKind.Sabotage
                 : charsum % 3 == 1 ? MissionKind.Scan : MissionKind.Beacon)
                : (charsum % 3 == 0 ? MissionKind.DroneDefense
                 : charsum % 3 == 1 ? MissionKind.Repair : MissionKind.SpaceDefense);

            var m = new ConquestMission
            {
                planetId = planetId,
                side = side,
                kind = kind,
                timeLimitSeconds = ConquestMissionRules.TimeLimitSeconds,
                winTilt = isUnderdog ? ConquestMissionRules.UnderdogWinTilt : ConquestMissionRules.WinTilt,
                loseTilt = ConquestMissionRules.LoseTilt,
            };
            switch (kind)
            {
                case MissionKind.Sabotage:
                    m.objectiveCount = ConquestMissionRules.SabotageObjectives;
                    m.title = "SABOTAGE THE SHIELD";
                    m.brief = "Disable the shield pylons on the surface before the garrison locks down.";
                    break;
                case MissionKind.Scan:
                    m.objectiveCount = ConquestMissionRules.ScanObjectives;
                    m.title = "SCAN THE DEFENSE GRID";
                    m.brief = "Hold position at each grid node until the sweep completes. Stay close — the lock drops if you drift.";
                    break;
                case MissionKind.Beacon:
                    m.objectiveCount = ConquestMissionRules.BeaconObjectives;
                    m.title = "PLANT THE BEACON";
                    m.brief = "Grab the strike beacon and carry it to the uplink pad. It's heavy, it hums, and they'll know.";
                    break;
                case MissionKind.Repair:
                    m.objectiveCount = ConquestMissionRules.RepairObjectives;
                    m.title = "PATCH THE CONDUITS";
                    m.brief = "Their probes cut your shield conduits. Slap each arcing junction back into its socket — three good hits each.";
                    break;
                case MissionKind.SpaceDefense:
                    m.objectiveCount = ConquestMissionRules.SpaceDefenseObjectives;
                    m.title = "SCRAMBLE THE SHIP";
                    m.brief = "Their strike wing is inbound. Take the helm - stun bolts, three interceptors, before the clock.";
                    break;
                default: // DroneDefense
                    m.objectiveCount = ConquestMissionRules.DroneDefenseObjectives;
                    m.title = "REPEL THE SCOUTS";
                    m.brief = "Their scout drones are mapping your defenses. Shoot every one of them down.";
                    break;
            }
            return m;
        }
    }

    /// <summary>
    /// The state machine for one played (or declined) mission. Pure — the scene runtime just calls
    /// Tick/CompleteObjective and reads the phase. ResultTilt is the signed value AttackOrder
    /// .missionModifier expects: positive helps the attacker, negative reinforces the defense.
    /// </summary>
    [Serializable]
    public class MissionAttempt
    {
        public ConquestMission mission;
        public MissionPhase phase = MissionPhase.Offered;
        public int objectivesDone;
        public float clock;

        public MissionAttempt(ConquestMission m) { mission = m; }

        public bool IsTerminal => phase == MissionPhase.Won || phase == MissionPhase.Lost
                               || phase == MissionPhase.Declined;

        public void Accept() { if (phase == MissionPhase.Offered) phase = MissionPhase.Accepted; }

        public void Decline() { if (!IsTerminal) phase = MissionPhase.Declined; }

        /// <summary>Walking away mid-mission = a decline: your odds stay the same (Terry's rule).</summary>
        public void Abandon() => Decline();

        /// <summary>Advance the mission clock (only while Accepted). Timeout = Lost.</summary>
        public void Tick(float dt)
        {
            if (phase != MissionPhase.Accepted) return;
            clock += dt;
            if (clock >= mission.timeLimitSeconds) phase = MissionPhase.Lost;
        }

        public float SecondsRemaining => mission == null ? 0f
            : Math.Max(0f, mission.timeLimitSeconds - clock);

        public void CompleteObjective()
        {
            if (phase != MissionPhase.Accepted) return;
            objectivesDone++;
            if (objectivesDone >= mission.objectiveCount) phase = MissionPhase.Won;
        }

        /// <summary>The signed missionModifier: win pays winTilt, loss pays loseTilt, a decline pays
        /// nothing. Defense-side results are mirrored (a won defense LOWERS the attacker's odds; a
        /// botched defense hands the attacker a point).</summary>
        public int ResultTilt()
        {
            int raw = phase == MissionPhase.Won ? mission.winTilt
                    : phase == MissionPhase.Lost ? mission.loseTilt : 0;
            return mission.side == MissionSide.Attack ? raw : -raw;
        }
    }

    /// <summary>A battle whose resolution is held open while its mission gets flown.</summary>
    [Serializable]
    public class PendingBattle
    {
        public AttackOrder order;
        public int seed;
        public MissionAttempt attempt;
        public bool rivalInitiated;   // true = the AI attacked and the PLAYER is flying the defense
    }

    /// <summary>
    /// The cross-scene holder (the PvpNoise static-channel idiom): the war table stashes the live
    /// campaign + the pending battle here before TravelCoordinator carries the player to the mission
    /// world, and restores both when they return. In-memory only — travel survives, app restart
    /// doesn't (disk save/resume is a separate queued row).
    /// </summary>
    public static class ConquestSession
    {
        public static ConquestState State;
        public static PendingBattle Pending;
        public static string ReturnScene = "";
        public static bool Hotseat;       // B4: both admirals human, pass the headset
        public static int ActiveSide;     // whose half-turn the table is showing (0 or 1)

        /// <summary>True when the given scene should spawn a mission (accepted, not yet finished).</summary>
        public static bool MissionActiveFor(string sceneName)
        {
            if (Pending == null || Pending.attempt == null) return false;
            if (Pending.attempt.phase != MissionPhase.Accepted) return false;
            return SceneForMission(Pending.attempt.mission) == sceneName;
        }

        /// <summary>Where a mission is PLAYED: space-defense flies the space lane; everything else
        /// happens in the contested world itself.</summary>
        public static string SceneForMission(ConquestMission m) =>
            m != null && m.kind == MissionKind.SpaceDefense ? "SpaceLane_Trial" : SceneForPlanet(m == null ? "" : m.planetId);

        /// <summary>planetId → scene name via the canonical seed list (single source of truth).</summary>
        public static string SceneForPlanet(string planetId)
        {
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
                if (w.WorldId == planetId) return w.SceneName;
            return "";
        }

        public static void Clear()
        { State = null; Pending = null; ReturnScene = ""; Hotseat = false; ActiveSide = 0; }
    }
}
