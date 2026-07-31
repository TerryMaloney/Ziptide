using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// COMBAT_HEALTH_PLAN Phase B's device shell — the moment the campaign player stops being
    /// invincible.
    ///
    /// Until now `PlayerStunReceiver` said it plainly: *"NO health, NO death."* Every creature in the
    /// game carried an authored `damage` that was applied to nobody, so combat had no stakes: you
    /// could stand in a drone patrol and nothing could happen to you.
    ///
    /// This is deliberately a THIN TRANSLATOR, as <see cref="PlayerCombatState"/> instructs: forward
    /// hits in, read outcomes out, decide nothing. Every rule — armor drains, overkill only breaks,
    /// a hit at zero kills, regen after breaking contact, spawn protection — lives in the pure core
    /// and is already unit-tested there. Nothing about the model is re-decided here, because a rule
    /// implemented twice is a rule that will disagree with itself.
    ///
    /// Hosted by <see cref="PlayerStunReceiver"/> on the rig rather than bootstrapping itself, so it
    /// inherits an existing automatic owner instead of becoming a new one.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerArmor : MonoBehaviour
    {
        /// <summary>Fires on every hit that mattered, for HUD/audio. Outcome, then armor 0..1.</summary>
        public static event System.Action<PlayerHitOutcome, float> OnHit;

        private readonly PlayerCombatState _state = new PlayerCombatState();
        private PlayerStunReceiver _stun;

        public PlayerCombatState State => _state;
        public float Armor01 => (float)_state.Armor.Fraction;
        public bool IsBroken => _state.IsBroken;

        /// <summary>Ensure the shell on the rig. Idempotent — the stun receiver calls this.</summary>
        public static PlayerArmor EnsureOn(GameObject rig)
        {
            if (rig == null) return null;
            PlayerArmor armor = rig.GetComponent<PlayerArmor>();
            if (armor == null)
            {
                armor = rig.AddComponent<PlayerArmor>();
                Debug.Log("ZIPTIDE: ARMOR_READY owner=PlayerStunReceiver max="
                    + armor._state.Armor.MaxCharge);
            }
            // Mortality without a readout is not an unfinished feature, it is an unfair one — so the
            // vignette is ensured WITH the armor, never separately, and cannot be forgotten.
            ArmorVignetteRuntime.EnsureOn(rig);
            return armor;
        }

        private void Awake() => _stun = GetComponent<PlayerStunReceiver>();

        /// <summary>
        /// The ONE entry point for anything that can hurt the player. Damage is on the canonical
        /// integer scale (`PvpRules`), and <paramref name="sourcePos"/> drives the existing
        /// incoming-fire direction line so the player can tell where it came from.
        /// </summary>
        public PlayerHitOutcome ApplyDamage(int amount, Vector3 sourcePos)
        {
            PlayerHitOutcome outcome = _state.ApplyDamage(amount, Time.timeAsDouble);
            if (outcome == PlayerHitOutcome.Ignored) return outcome;

            // NOTE: the stun/flash is NOT applied here. The caller owns it, because each attacker has
            // its own stun duration and slow, and applying a second one here would double-stun every
            // hit. PlayerStunReceiver.ApplyHit is the seam that pairs them.
            OnHit?.Invoke(outcome, Armor01);
            Debug.Log("ZIPTIDE: ARMOR outcome=" + outcome + " damage=" + amount
                + " armor=" + Armor01.ToString("F2") + " deaths=" + _state.Deaths);

            if (outcome == PlayerHitOutcome.Killed) Die();
            return outcome;
        }

        private void Die()
        {
            // §2's serverless checkpoint: the world's own __SPAWN_PLAYER marker, which travel-arrival
            // already teleports to. No metadata server, no new save field, no mid-world checkpoint
            // state to keep consistent.
            var world = Object.FindObjectOfType<WorldRuntime>();
            if (world != null) world.RespawnPlayer(transform);
            else Debug.LogWarning("ZIPTIDE: ARMOR_DEATH no_world_runtime — player stays where they fell");

            _state.Respawn(Time.timeAsDouble);
            if (_stun != null) _stun.ClearStun();
            Debug.Log("ZIPTIDE: ARMOR_DEATH deaths=" + _state.Deaths + " respawned=1 protected=1");
        }

        private void Update() => _state.Tick(Time.timeAsDouble, Time.deltaTime);
    }
}
