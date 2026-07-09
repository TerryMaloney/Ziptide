using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Conquest
{
    public enum MissionSide { Attack, Defense }
    public enum MissionKind { Sabotage, DroneDefense }
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
            bool attack = side == MissionSide.Attack;
            return new ConquestMission
            {
                planetId = planetId,
                side = side,
                kind = attack ? MissionKind.Sabotage : MissionKind.DroneDefense,
                objectiveCount = attack ? ConquestMissionRules.SabotageObjectives
                                        : ConquestMissionRules.DroneDefenseObjectives,
                timeLimitSeconds = ConquestMissionRules.TimeLimitSeconds,
                winTilt = isUnderdog ? ConquestMissionRules.UnderdogWinTilt : ConquestMissionRules.WinTilt,
                loseTilt = ConquestMissionRules.LoseTilt,
                title = attack ? "SABOTAGE THE SHIELD" : "REPEL THE SCOUTS",
                brief = attack
                    ? "Disable the shield pylons on the surface before the garrison locks down."
                    : "Their scout drones are mapping your defenses. Shoot every one of them down.",
            };
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

        /// <summary>True when the given scene should spawn a mission (accepted, not yet finished).</summary>
        public static bool MissionActiveFor(string sceneName)
        {
            if (Pending == null || Pending.attempt == null) return false;
            if (Pending.attempt.phase != MissionPhase.Accepted) return false;
            return SceneForPlanet(Pending.attempt.mission.planetId) == sceneName;
        }

        /// <summary>planetId → scene name via the canonical seed list (single source of truth).</summary>
        public static string SceneForPlanet(string planetId)
        {
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
                if (w.WorldId == planetId) return w.SceneName;
            return "";
        }

        public static void Clear() { State = null; Pending = null; ReturnScene = ""; }
    }
}
