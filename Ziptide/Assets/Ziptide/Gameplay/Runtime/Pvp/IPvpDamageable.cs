using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Anything a PvP weapon can damage: the local player (index 0) or the opponent (index 1 — the bot
    /// now, a networked remote avatar in Phase 4). Weapons route hits through this so the same plumbing
    /// works for bot and netcode. The implementer owns a <see cref="PvpCombatant"/> and the authoritative
    /// kill/respawn rules.
    /// </summary>
    public interface IPvpDamageable
    {
        int PlayerIndex { get; }
        bool IsAlive { get; }
        void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir);
    }
}
