using System;

namespace Ziptide.Multiplayer
{
    /// <summary>
    /// The transport REGISTRY (A6-prep) — one place gameplay asks "what network am I on?".
    /// Defaults to <see cref="LoopbackPvpTransport"/> (solo/bot: everything echoes locally); the Photon
    /// adapter (Assets/ZiptideNet, compiled only under ZIPTIDE_PHOTON) registers itself on room join.
    /// Registration points INTO this assembly so gameplay never references the adapter or Photon.
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
    }
}
