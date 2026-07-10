using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1c — the belt tile in your HAND: a grabbable slab that shows a snapped ghost
    /// (cell + direction from your wrist's yaw) on the nearest belt floor while held, and clicks
    /// into the grid on release over a valid empty cell. Released anywhere else it just drops —
    /// physical, pick it up again. Placement itself is the fun: snap, haptic, done.
    /// Build order honors gotcha #6 (collider + rigidbody exist before the interactable).
    /// </summary>
    public class BeltTileItem : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private BeltFloorRuntime _hoverFloor;

        /// <summary>What this tile places (4.1l): Belt or Splitter — a picked-up splitter comes
        /// back as a splitter tile, nothing degrades on the round trip.</summary>
        public Ziptide.Content.Automation.CellKind kind = Ziptide.Content.Automation.CellKind.Belt;

        /// <summary>True once a hand has taken this tile — the dispenser's restock signal.</summary>
        public bool WasGrabbed { get; private set; }

        /// <summary>Spawn a ready-to-grab tile (dispenser + belt-pickup both use this).</summary>
        public static BeltTileItem Spawn(Vector3 at,
            Ziptide.Content.Automation.CellKind kind = Ziptide.Content.Automation.CellKind.Belt)
        {
            var go = new GameObject("BeltTileItem");
            go.transform.position = at;
            var item = go.AddComponent<BeltTileItem>();
            item.kind = kind;
            item.Build();
            return item;
        }

        private void Build()
        {
            // The slab visual — reads as a belt segment in miniature.
            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = "Slab";
            Object.Destroy(slab.GetComponent<Collider>());
            slab.transform.SetParent(transform, false);
            slab.transform.localScale = new Vector3(0.30f, 0.06f, 0.30f);
            ItemFactory.ApplyURPColor(slab, new Color(0.20f, 0.22f, 0.24f));
            var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Stripe";
            Object.Destroy(stripe.GetComponent<Collider>());
            stripe.transform.SetParent(transform, false);
            stripe.transform.localPosition = new Vector3(0f, 0.035f, 0f);
            stripe.transform.localScale = new Vector3(0.08f, 0.02f, 0.22f);
            ItemFactory.ApplyURPColor(stripe, new Color(0.35f, 0.95f, 0.75f)); // points along +z = flow
            if (kind == Ziptide.Content.Automation.CellKind.Splitter)
            {
                // The fork stripe — a splitter tile reads as a splitter in the hand.
                var fork = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fork.name = "ForkStripe";
                Object.Destroy(fork.GetComponent<Collider>());
                fork.transform.SetParent(transform, false);
                fork.transform.localPosition = new Vector3(0.06f, 0.035f, 0f);
                fork.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                fork.transform.localScale = new Vector3(0.08f, 0.02f, 0.16f);
                ItemFactory.ApplyURPColor(fork, new Color(0.35f, 0.95f, 0.75f));
            }
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Grab volume + physics BEFORE the interactable (gotcha #6).
            var col = gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(0.32f, 0.10f, 0.32f);
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.6f;
            _grab = gameObject.AddComponent<XRGrabInteractable>();
            _grab.selectEntered.AddListener(_ => WasGrabbed = true);
            _grab.selectExited.AddListener(OnReleased);
        }

        private void Update()
        {
            if (_grab == null || !_grab.isSelected)
            {
                if (_hoverFloor != null) { _hoverFloor.HideGhost(); _hoverFloor = null; }
                return;
            }

            // Held: ghost on the floor whose grid we're over (first match wins — floors don't overlap).
            BeltFloorRuntime over = null;
            for (int i = 0; i < BeltFloorRuntime.Active.Count; i++)
            {
                var f = BeltFloorRuntime.Active[i];
                if (f != null && f.TryWorldToCell(transform.position, out _, out _)) { over = f; break; }
            }
            if (over != _hoverFloor && _hoverFloor != null) _hoverFloor.HideGhost();
            _hoverFloor = over;
            if (_hoverFloor != null) _hoverFloor.ShowGhost(transform.position, transform.forward);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            var floor = _hoverFloor;
            if (_hoverFloor != null) { _hoverFloor.HideGhost(); _hoverFloor = null; }
            if (floor == null) return; // dropped in the open — stays physical

            if (floor.PlaceCellFromHand(transform.position, transform.forward, kind))
            {
                var hand = args.interactorObject as XRBaseControllerInteractor;
                if (hand != null) hand.SendHapticImpulse(0.55f, 0.06f); // the CLICK
                Destroy(gameObject); // the tile became part of the floor
            }
        }

        private void OnDestroy()
        {
            if (_hoverFloor != null) _hoverFloor.HideGhost();
        }
    }
}
