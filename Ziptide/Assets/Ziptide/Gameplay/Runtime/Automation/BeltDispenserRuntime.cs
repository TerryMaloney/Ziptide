using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1c — the belt-tile dispenser: a pedestal that always has a fresh grabbable
    /// <see cref="BeltTileItem"/> waiting on top. Take one and the next appears after a beat
    /// (capped so a mischievous player can't carpet the world in loose slabs). Patch-time = this
    /// component + a position only; everything builds at runtime in Start().
    /// </summary>
    public class BeltDispenserRuntime : MonoBehaviour
    {
        [Tooltip("Max loose (unplaced) tiles allowed in the scene at once.")]
        public int maxLoose = 6;

        private const float RestockDelay = 0.6f;
        private BeltTileItem _current;
        private float _restockAt = -1f;

        private void Start()
        {
            // The pedestal.
            var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "DispenserPost";
            post.transform.SetParent(transform, false);
            post.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            post.transform.localScale = new Vector3(0.45f, 1.0f, 0.45f);
            ItemFactory.ApplyURPColor(post, new Color(0.22f, 0.24f, 0.26f));
            var band = GameObject.CreatePrimitive(PrimitiveType.Cube);
            band.name = "DispenserBand";
            var bc = band.GetComponent<Collider>();
            if (bc != null) Destroy(bc);
            band.transform.SetParent(transform, false);
            band.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            band.transform.localScale = new Vector3(0.47f, 0.05f, 0.47f);
            ItemFactory.ApplyURPColor(band, new Color(0.35f, 0.95f, 0.75f));
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            Restock();
        }

        private void Update()
        {
            // Current tile taken (a hand grabbed it, or it was placed/destroyed)? Queue the next.
            bool gone = _current == null || _current.WasGrabbed;
            if (gone && _restockAt < 0f) _restockAt = Time.time + RestockDelay;

            if (_restockAt > 0f && Time.time >= _restockAt)
            {
                _restockAt = -1f;
                if (CountLoose() < maxLoose) Restock();
                else _restockAt = Time.time + 2f; // world is carpeted — check again later
            }
        }

        private void Restock()
        {
            _current = BeltTileItem.Spawn(transform.position + Vector3.up * 1.15f);
        }

        private static int CountLoose()
            => Object.FindObjectsOfType<BeltTileItem>().Length;
    }
}
