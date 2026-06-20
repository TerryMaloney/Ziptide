using System;
using UnityEngine;

namespace Ziptide.Multiplayer
{
    /// <summary>
    /// PvP networking CONTRACT (Phase 3 prep) — transport-agnostic message DTOs + the
    /// <see cref="IPvpTransport"/> seam. No Photon here: the PUN2 adapter (Phase 3) implements
    /// IPvpTransport and maps these to RPCs; the gameplay layer (PvpMatchDirector / avatar / weapons)
    /// only ever talks to IPvpTransport, so swapping the bot for a remote player — or PUN2 for another
    /// stack — touches one class. Reuses the pure rules enums (PvpWeapon / PvpPhase).
    ///
    /// Sync set (kept tiny per research — ~head+2 hands + events): pose, fire, hit, score, wall state.
    /// </summary>
    public enum PvpNetRole { None, Host, Client }

    /// <summary>Networked VR avatar pose — head + 2 hands + which gun is held (IK fills the body).</summary>
    [Serializable]
    public struct PlayerPoseMsg
    {
        public int playerId;
        public Vector3 headPos;   public Quaternion headRot;
        public Vector3 lHandPos;  public Quaternion lHandRot;
        public Vector3 rHandPos;  public Quaternion rHandRot;
        public int heldWeapon;    // (int)PvpWeapon, or -1 for empty hands
    }

    /// <summary>A shot was fired (for tracer/FX + host-authoritative hit validation).</summary>
    [Serializable]
    public struct FireMsg
    {
        public int shooterId;
        public PvpWeapon weapon;
        public Vector3 origin;
        public Vector3 direction;
    }

    /// <summary>A resolved hit on a player (host-authoritative). amount/killed come from PvpCombatant.</summary>
    [Serializable]
    public struct HitMsg
    {
        public int attackerId;
        public int targetId;
        public PvpWeapon weapon;
        public int amount;
        public bool killed;
    }

    /// <summary>Authoritative score + match phase snapshot (drives both HUDs / end screen).</summary>
    [Serializable]
    public struct ScoreMsg
    {
        public int score0;
        public int score1;
        public PvpPhase phase;
        public int winnerIndex; // -1 until ended
    }

    /// <summary>A breakable wall panel changed state. state: 0=Intact, 1=SmallHole, 2=LargeHole.</summary>
    [Serializable]
    public struct WallMsg
    {
        public int panelId;
        public int state;
    }

    /// <summary>
    /// The seam between PvP gameplay and the network. Implemented by the Photon adapter (Phase 3) and by
    /// <see cref="LoopbackPvpTransport"/> (solo/bot + tests). Send* transmits; On* fires on receipt.
    /// </summary>
    public interface IPvpTransport
    {
        PvpNetRole Role { get; }
        bool IsHost { get; }
        int LocalPlayerId { get; }

        void SendPose(PlayerPoseMsg msg);
        void SendFire(FireMsg msg);
        void SendHit(HitMsg msg);
        void SendScore(ScoreMsg msg);
        void SendWall(WallMsg msg);

        event Action<PlayerPoseMsg> OnPose;
        event Action<FireMsg> OnFire;
        event Action<HitMsg> OnHit;
        event Action<ScoreMsg> OnScore;
        event Action<WallMsg> OnWall;
    }
}
