using System;

namespace Ziptide.Multiplayer
{
    /// <summary>
    /// A no-network <see cref="IPvpTransport"/> that echoes every Send* straight back through the
    /// matching On* event. Lets the PvP gameplay run through the network seam in solo/bot mode today
    /// (so Phase 2 doesn't special-case "no net"), and gives the contract a headless test double. The
    /// Photon adapter replaces this in Phase 3 with no gameplay changes.
    /// </summary>
    public sealed class LoopbackPvpTransport : IPvpTransport
    {
        public PvpNetRole Role => PvpNetRole.Host;
        public bool IsHost => true;
        public int LocalPlayerId => 0;

        public event Action<PlayerPoseMsg> OnPose;
        public event Action<FireMsg> OnFire;
        public event Action<HitMsg> OnHit;
        public event Action<ScoreMsg> OnScore;
        public event Action<WallMsg> OnWall;

        public void SendPose(PlayerPoseMsg msg) => OnPose?.Invoke(msg);
        public void SendFire(FireMsg msg) => OnFire?.Invoke(msg);
        public void SendHit(HitMsg msg) => OnHit?.Invoke(msg);
        public void SendScore(ScoreMsg msg) => OnScore?.Invoke(msg);
        public void SendWall(WallMsg msg) => OnWall?.Invoke(msg);
    }
}
