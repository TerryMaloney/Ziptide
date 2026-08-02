using System.Collections.Generic;
using UnityEngine;

using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE SHIP IS THE ARMOURY (⚖ Terry). The rack aboard your ship holds the arsenal, and the
    /// hatch does not open until something off it is on your belt.
    ///
    /// Counts three populations every check:
    ///   • on the belt   — a holster socket with something selected;
    ///   • in the hands  — a weapon currently gripped;
    ///   • reachable     — weapons still on the rack or loose in the compartment.
    /// and asks <see cref="ShipArmouryCore"/> what the ramp should do. All the design lives in that
    /// pure verdict; this only observes and reports.
    ///
    /// The barrier is authored, not invented: the gate toggles an existing collider. A gate that
    /// spawns its own wall is a gate nobody can find in a scene when it goes wrong.
    /// </summary>
    [DisallowMultipleComponent]
    public class ShipArmouryRuntime : MonoBehaviour
    {
        // 1 s, not 0.25 s. Each check runs two FindObjectsOfType scans (see CountOnBelt /
        // CountInHands), and this component lives in the ship for the whole session — so the old
        // rate was eight full-scene type scans per second, forever, on a mobile GPU. The gate that
        // caught it is tools/frame_cost_gate.py, added the same day.
        private const float CheckInterval = 1f;

        [SerializeField]
        [Tooltip("Collider that physically holds the player aboard. Disabled the moment they are armed.")]
        private GameObject _barrier;

        [SerializeField]
        [Tooltip("Rack root. Weapons parented under here count as reachable.")]
        private Transform _rack;

        private float _nextCheck;
        private DepartureVerdict _last = DepartureVerdict.Armed;
        private bool _spoken;

        public void Configure(GameObject barrier, Transform rack)
        {
            _barrier = barrier;
            _rack = rack;
        }

        public DepartureVerdict Current => _last;

        /// <summary>
        /// One-shot verdict for a caller that owns its own moment — the ship's DISEMBARK panel asks
        /// this the instant it is pressed. Shared with the polling path above so a button press and
        /// a barrier can never disagree about whether the player is armed.
        /// </summary>
        public static DepartureVerdict EvaluateNow(Transform rack)
        {
            return ShipArmouryCore.Evaluate(CountOnBelt(), CountInHands(), CountReachableUnder(rack));
        }

        private static int CountReachableUnder(Transform rack)
        {
            if (rack == null) return 0;
            int n = 0;
            foreach (var item in rack.GetComponentsInChildren<ItemRuntime>(true))
                if (item != null && IsWeapon(item.gameObject)) n++;
            return n;
        }

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + CheckInterval;

            DepartureVerdict verdict = ShipArmouryCore.Evaluate(
                CountOnBelt(), CountInHands(), CountReachable());

            bool blocks = ShipArmouryCore.Blocks(verdict);
            if (_barrier != null && _barrier.activeSelf != blocks) _barrier.SetActive(blocks);

            if (verdict == _last) return;
            _last = verdict;

            // Speak once per transition, never on a timer — the design law is that the game teaches
            // by waiting, and a player who just does the thing hears nothing at all.
            string cue = ShipArmouryCore.CueFor(verdict);
            if (!string.IsNullOrEmpty(cue))
            {
                var rill = Object.FindObjectOfType<RillCompanion>();
                if (rill != null) rill.SayById(cue);
                _spoken = true;
            }

            Debug.Log("ZIPTIDE: ARMOURY_GATE verdict=" + verdict + " blocks=" + blocks
                + " cue=" + (string.IsNullOrEmpty(cue) ? "none" : cue) + " spoken=" + _spoken);
        }

        private static int CountOnBelt()
        {
            int n = 0;
            foreach (var socket in Object.FindObjectsOfType<HolsterSocketInteractor>())
                if (socket != null && socket.hasSelection && IsWeaponSelection(socket)) n++;
            return n;
        }

        private static bool IsWeaponSelection(HolsterSocketInteractor socket)
        {
            IReadOnlyList<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable> selected = socket.interactablesSelected;
            for (int i = 0; i < selected.Count; i++)
            {
                var mb = selected[i] as MonoBehaviour;
                if (mb != null && IsWeapon(mb.gameObject)) return true;
            }
            return false;
        }

        private static int CountInHands()
        {
            int n = 0;
            foreach (var grab in Object.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>())
            {
                if (grab == null || !grab.isSelected) continue;
                // A holster is also a selector; only a hand counts as "in your fist".
                bool heldByHand = false;
                IReadOnlyList<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor> by = grab.interactorsSelecting;
                for (int i = 0; i < by.Count; i++)
                    if (!(by[i] is UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor)) { heldByHand = true; break; }
                if (heldByHand && IsWeapon(grab.gameObject)) n++;
            }
            return n;
        }

        private int CountReachable() => CountReachableUnder(_rack);

        // One source of truth, shared with the holster's allowlist. ItemDefinition carries no weapon
        // flag and its `damage` field is unserialized on every weapon asset, so the catalog IS the
        // predicate — and adding a weapon there makes it rackable, beltable and counted at once.
        private static bool IsWeapon(GameObject go)
        {
            var item = go.GetComponentInParent<ItemRuntime>();
            return item != null && item.Definition != null
                && WeaponCatalog.IsWeapon(item.Definition.itemId);
        }
    }
}
