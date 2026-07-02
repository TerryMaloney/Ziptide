using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Ambient attacker context for N-way kill credit (M7a A3-scene). A weapon reports its firer's
    /// combatant index immediately before calling <see cref="IPvpDamageable.ReceiveHit"/>; the death
    /// report that follows SYNCHRONOUSLY inside that call reads it back. Same-frame validity only, so
    /// stale credit can never leak into a later death. Chosen over widening the ReceiveHit signature
    /// because CreatureRuntime (story lane) implements the interface too — this adds attacker identity
    /// with zero cross-lane edits and zero churn for netcode (a remote avatar reports its own index).
    /// </summary>
    public static class PvpHitSource
    {
        private static int _attacker = -1;
        private static int _frame = -1;

        /// <summary>Call right before ReceiveHit with the FIRER's combatant index (player rig = 0).</summary>
        public static void Report(int attackerIndex)
        {
            _attacker = attackerIndex;
            _frame = Time.frameCount;
        }

        /// <summary>The attacker reported THIS frame, or -1 (caller falls back to 1v1 inference).</summary>
        public static int Current => _frame == Time.frameCount ? _attacker : -1;
    }
}
