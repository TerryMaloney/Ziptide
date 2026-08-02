using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Automation;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1k — the BLUEPRINT WAND: copy a line you built, stamp it again and again.
    /// Held over a floor it shows the cell ghost; release over a BELT/SPLITTER cell to CAPTURE that
    /// whole connected line into the wand (the head glows with the cargo teal and shows the cell
    /// count), release over EMPTY grid to STAMP the held line seed-anchored at that cell
    /// (all-or-nothing — a crowded footprint refuses, red-buzz). Capture again anytime to swap what
    /// the wand holds. Off-grid it just drops — physical, like every belt tool. Stamped cells run
    /// the normal placement path, so 4.1f persistence and the 4.1g caps apply untouched.
    /// Build order honors gotcha #6 (collider + rigidbody before the interactable).
    /// </summary>
    public class BeltBlueprintWandItem : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private BeltFloorRuntime _hoverFloor;
        private BeltBlueprint _held;
        private Renderer _headR;
        private TextMesh _countLabel;

        public static BeltBlueprintWandItem Spawn(Vector3 at)
        {
            var go = new GameObject("BeltBlueprintWand");
            go.transform.position = at;
            var item = go.AddComponent<BeltBlueprintWandItem>();
            item.Build();
            return item;
        }

        private void Build()
        {
            // The wand: grip shaft, guard disc, and a scanner head that shows the loaded state.
            var shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "WandShaft";
            Object.Destroy(shaft.GetComponent<Collider>());
            shaft.transform.SetParent(transform, false);
            shaft.transform.localScale = new Vector3(0.035f, 0.14f, 0.035f);
            ItemFactory.ApplyURPColor(shaft, new Color(0.22f, 0.24f, 0.26f));

            var guard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            guard.name = "WandGuard";
            Object.Destroy(guard.GetComponent<Collider>());
            guard.transform.SetParent(transform, false);
            guard.transform.localPosition = new Vector3(0f, 0.13f, 0f);
            guard.transform.localScale = new Vector3(0.10f, 0.012f, 0.10f);
            ItemFactory.ApplyURPColor(guard, new Color(0.35f, 0.95f, 0.75f));

            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "WandHead";
            Object.Destroy(head.GetComponent<Collider>());
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0f, 0.20f, 0f);
            head.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            head.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            _headR = head.GetComponent<Renderer>();
            SetHeadEmpty();

            var labelGo = new GameObject("WandCount");
            _countLabel = labelGo.AddComponent<TextMesh>();
            _countLabel.characterSize = 0.02f;
            _countLabel.fontSize = 40;
            _countLabel.anchor = TextAnchor.MiddleCenter;
            _countLabel.color = new Color(0.55f, 0.95f, 0.75f);
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 0.30f, 0f);
            _countLabel.text = "";

            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 0.34f; col.radius = 0.06f; col.center = new Vector3(0f, 0.08f, 0f);
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            _grab = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _grab.selectExited.AddListener(OnReleased);
        }

        private void SetHeadEmpty()
            => ItemFactory.ApplyURPColor(_headR.gameObject, new Color(0.30f, 0.34f, 0.38f));
        private void SetHeadLoaded()
            => ItemFactory.ApplyURPColor(_headR.gameObject, new Color(0.45f, 1.0f, 0.85f));

        private void Update()
        {
            if (_grab == null || !_grab.isSelected)
            {
                if (_hoverFloor != null) { HideGhosts(_hoverFloor); _hoverFloor = null; }
                return;
            }
            BeltFloorRuntime over = null;
            for (int i = 0; i < BeltFloorRuntime.Active.Count; i++)
            {
                var f = BeltFloorRuntime.Active[i];
                if (f != null && f.TryWorldToCell(transform.position, out _, out _)) { over = f; break; }
            }
            if (over != _hoverFloor && _hoverFloor != null) HideGhosts(_hoverFloor);
            _hoverFloor = over;
            if (_hoverFloor != null)
            {
                // 4.1l: loaded = the WHOLE footprint previews (teal fits / red refuses);
                // empty = the single-cell cursor.
                if (_held != null) _hoverFloor.ShowBlueprintGhost(_held, transform.position);
                else _hoverFloor.ShowGhost(transform.position, transform.forward);
            }

            // The head slowly spins while loaded — "I'm holding your line."
            if (_held != null && _headR != null)
                _headR.transform.Rotate(0f, 90f * Time.deltaTime, 0f, Space.Self);
        }

        private static void HideGhosts(BeltFloorRuntime floor)
        {
            floor.HideGhost();
            floor.HideBlueprintGhost();
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            var floor = _hoverFloor;
            if (_hoverFloor != null) { HideGhosts(_hoverFloor); _hoverFloor = null; }
            if (floor == null || floor.Lattice == null) return; // dropped in the open — stays physical
            if (!floor.TryWorldToCell(transform.position, out int x, out int z)) return;

            var hand = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
            var kind = floor.Lattice.KindAt(x, z);
            if (kind == CellKind.Belt || kind == CellKind.Splitter)
            {
                // CAPTURE the connected line under the wand.
                var bp = BeltBlueprint.Capture(floor.Lattice, x, z);
                if (bp != null)
                {
                    _held = bp;
                    SetHeadLoaded();
                    if (_countLabel != null) _countLabel.text = "x" + bp.Count;
                    if (hand != null) { hand.SendHapticImpulse(0.4f, 0.05f); hand.SendHapticImpulse(0.4f, 0.05f); }
                    Debug.Log("ZIPTIDE: BELT_BLUEPRINT_CAPTURE cells=" + bp.Count);
                }
                else if (hand != null) hand.SendHapticImpulse(0.8f, 0.12f); // too big to hold
                return;
            }
            if (kind == CellKind.Empty && _held != null)
            {
                // STAMP the held line, seed-anchored where the wand points.
                int placed = floor.StampBlueprint(_held, x, z);
                if (placed > 0)
                {
                    if (hand != null) hand.SendHapticImpulse(0.6f, 0.08f); // the heavy CLICK
                }
                else if (hand != null) hand.SendHapticImpulse(0.8f, 0.12f); // doesn't fit — red buzz
            }
        }

        private void OnDestroy()
        {
            if (_hoverFloor != null) HideGhosts(_hoverFloor);
        }
    }

    /// <summary>The wand's home: a small stand that keeps one wand available beside the dispenser.
    /// Patch-time = component + position only (the standing law); everything builds in Start().</summary>
    public class BeltWandStandRuntime : MonoBehaviour
    {
        private BeltBlueprintWandItem _wand;
        private float _nextCheck;

        private void Start()
        {
            var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "WandStand";
            post.transform.SetParent(transform, false);
            post.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            post.transform.localScale = new Vector3(0.09f, 0.45f, 0.09f);
            ItemFactory.ApplyURPColor(post, new Color(0.22f, 0.24f, 0.26f));
            var cup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cup.name = "WandCup";
            var cc = cup.GetComponent<Collider>();
            if (cc != null) Destroy(cc);
            cup.transform.SetParent(transform, false);
            cup.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            cup.transform.localScale = new Vector3(0.14f, 0.03f, 0.14f);
            ItemFactory.ApplyURPColor(cup, new Color(0.35f, 0.95f, 0.75f));
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Restock();
        }

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + 3f;
            // The wand never self-destructs, but it can be thrown into the void — restock when it's
            // gone or far away (the old one stays wherever it landed; two loose wands max in spirit).
            if (_wand == null ||
                (transform.position - _wand.transform.position).sqrMagnitude > 30f * 30f)
                Restock();
        }

        private void Restock()
            => _wand = BeltBlueprintWandItem.Spawn(transform.position + Vector3.up * 1.05f);
    }
}
