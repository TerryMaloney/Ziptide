using UnityEngine;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Content
{
    /// <summary>
    /// A4.5 — an AUGMENT as data (spec ABILITIES_AND_ARSENAL §2): the ability-item category. The
    /// `effectId` routes to the pure <see cref="AugmentEffects"/> hooks / the controller's active
    /// behaviors — scene code never hardcodes an augment id. Balance numbers live HERE per-asset
    /// (they're the augment's identity), slot scarcity lives in the pure AugmentLoadout.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Items/Augment", fileName = "Augment")]
    public class AugmentDefinition : ItemDefinition
    {
        public AugmentKind augmentKind = AugmentKind.Passive;

        [Tooltip("Actives: seconds after the effect ends before it can fire again.")]
        public float cooldownSeconds = 20f;

        [Tooltip("Actives: how long the effect runs (0 = instant burst, e.g. Surge Dash).")]
        public float durationSeconds = 0f;

        [Tooltip("Routes the effect: surge_dash / bubble_guard / overclock / magnet_palm / sure_step / sixth_sense.")]
        public string effectId = "";

        [Tooltip("The effect's one number: dash meters, recharge scale, pull reach, resistance, cd scale…")]
        public float magnitude = 1f;

        [Tooltip("Pickup gem tint (also the belt orb's color while equipped).")]
        public Color gemColor = new Color(0.7f, 0.5f, 0.95f);
    }
}
