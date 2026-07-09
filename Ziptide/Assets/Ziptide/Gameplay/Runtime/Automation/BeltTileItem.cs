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

        /// <summary>True once a hand has taken this tile — the dispenser's restock signal.</summary>
        public bool WasGrabbed { get; private set; }

        /// <summary>Spawn a ready-to-grab tile (dispenser + belt-pickup both use this).</summary>
        public static BeltTileItem Spawn(Vector3 at)
        {
            var go = new GameObject("BeltTileItem");
            go.transform.position = at;
            var item = go.AddComponent<BeltTileItem>();
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

            if (floor.PlaceBeltFromHand(transform.position, transform.forward))
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
