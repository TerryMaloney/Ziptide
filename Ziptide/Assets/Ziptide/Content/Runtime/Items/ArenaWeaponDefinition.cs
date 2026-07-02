using UnityEngine;

namespace Ziptide.Content
{
    public enum ArenaWeaponKind { StaticNet, SonicThumper, PrismBeam }

    /// <summary>
    /// The A4 arsenal weapons (design docs/design/ABILITIES_AND_ARSENAL.md §1) as one data-driven
    /// definition: the kind picks the runtime in ItemFactory; the numbers here are per-asset feel
    /// tuning. Damage/zone/charge BALANCE lives in PvpRules (pure, tested) — not here — so netcode
    /// and bots read the same truth.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Items/Arena Weapon", fileName = "ArenaWeapon")]
    public class ArenaWeaponDefinition : ItemDefinition
    {
        public ArenaWeaponKind kind = ArenaWeaponKind.StaticNet;

        [Tooltip("Seconds between uses (the net's throw rate / the thumper's swing debounce).")]
        public float fireCooldown = 1.2f;

        [Tooltip("Static Net: throw speed of the lobbed net (arcs under gravity).")]
        public float netThrowSpeed = 9f;

        [Tooltip("Sonic Thumper: swing speed (m/s of the head) that triggers the shockwave.")]
        public float swingSpeed = 1.6f;

        [Tooltip("Prism Beam: beam visual thickness.")]
        public float beamThickness = 0.05f;
    }
}
