using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A4.5 — an augment gem in the world: select it (ray or touch) and it equips into your loadout
    /// via <see cref="AugmentController"/> (1 active + 1 passive; equipping over a full slot swaps —
    /// the displaced id is logged; respawning the displaced gem is a queued nicety). The gem then
    /// vanishes — augments are equipment, not carriables, so they never fight the holster/belt.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class AugmentPickupRuntime : MonoBehaviour
    {
        private AugmentDefinition _def;   // held directly — ItemRuntime force-requires a grab body,
                                          // and a gem is selected, never carried.

        public void Init(AugmentDefinition def) => _def = def;

        private void Start()
        {
            GetComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Take());
        }

        private void Take()
        {
            if (_def == null) return;
            AugmentController.Ensure().Equip(_def);
            Destroy(gameObject);
        }
    }
}
