using System;

namespace Ziptide.Multiplayer
{
    /// <summary>
    /// The transport REGISTRY (A6) — one place gameplay asks "what network am I on?".
    /// Defaults to <see cref="LoopbackPvpTransport"/> (solo/bot: everything echoes locally); the Photon
    /// adapter (Assets/ZiptideNet, compiled only under ZIPTIDE_PHOTON) registers itself on room join.
    /// Registration points INTO this assembly so gameplay never references the adapter or Photon.
    ///
    /// Going online is decoupled the same way: the adapter installs an <see cref="OnlineStarter"/> at
    /// startup, and gameplay (the arena lobby's ONLINE tile) calls <see cref="StartOnline"/> — which
    /// is a graceful no-op when the build has no Photon, so a non-networked build never breaks.
    /// </summary>
    public static class PvpNetHub
    {
        private static IPvpTransport _active;

        /// <summary>Never null — solo play is the loopback transport.</summary>
        public static IPvpTransport Active => _active ?? (_active = new LoopbackPvpTransport());

        /// <summary>True once a real (non-loopback) transport registered — HUD/lobby can show "ONLINE".</summary>
        public static bool IsOnline => _active != null && !(_active is LoopbackPvpTransport);

        public static event Action<IPvpTransport> TransportChanged;

        public static void SetTransport(IPvpTransport transport)
        {
            _active = transport; // null = back to loopback on next Active read
            TransportChanged?.Invoke(Active);
        }

        // ── Going online (installed by the ZiptideNet Photon adapter under ZIPTIDE_PHOTON) ──────────

        /// <summary>Room code the last <see cref="StartOnline"/> requested (for HUD display).</summary>
        public static string RoomCode { get; private set; } = "ZIP-001";

        /// <summary>Short human net state ("offline", "connecting…", "in room 2/2", "net error: …").
        /// The adapter updates this as its callbacks fire; the lobby polls it.</summary>
        public static string Status { get; set; } = "offline";

        /// <summary>Installed by the adapter at startup. Given a room code, connects + joins and
        /// registers the transport on success. Null when the build has no networking compiled in.</summary>
        public static Func<string, bool> OnlineStarter;

        /// <summary>Installed by the adapter — tears the connection down. Null without networking.</summary>
        public static Action OnlineStopper;

        /// <summary>Begin an online session for <paramref name="roomCode"/>. Returns false (and stays
        /// on loopback) when no networking is compiled in — the caller can show "ONLINE N/A".</summary>
        public static bool StartOnline(string roomCode)
        {
            if (!string.IsNullOrEmpty(roomCode)) RoomCode = roomCode;
            if (OnlineStarter == null) { Status = "offline (no netcode in build)"; return false; }
            Status = "connecting…";
            return OnlineStarter(RoomCode);
        }

        /// <summary>Leave the online session and fall back to loopback (solo keeps working).</summary>
        public static void StopOnline()
        {
            OnlineStopper?.Invoke();
            SetTransport(null);
            Status = "offline";
        }
    }
}
