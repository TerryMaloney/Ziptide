using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Ship
{
    /// <summary>
    /// Keeps the tide skiff in the water (<see cref="CanalWaterCore"/>). Without it the boat is a
    /// ground vehicle that happens to be boat-shaped and can be driven up the plaza steps, which
    /// makes the canals scenery again.
    ///
    /// The lock is a NUDGE, not a wall: drift out of the channel and the hull is pushed back as if
    /// it grounded out on the bank. No teleporting, no freezing, no camera authority — all three
    /// would be comfort violations in a headset, and none of them would teach the player where the
    /// water is. Serialized by the city bake from the layout's own canal data, so the water the
    /// lock knows about is exactly the water the builder drew.
    /// </summary>
    [RequireComponent(typeof(VehicleRuntime))]
    public class SkiffWaterLockRuntime : MonoBehaviour
    {
        [Tooltip("Ring canal radius from the layout's rings block (0 = no ring canal).")]
        [SerializeField] private float ringCanalRadius;

        [Tooltip("Ring canal width from the layout's rings block.")]
        [SerializeField] private float ringCanalWidth;

        [Tooltip("Authored canal rectangles: centre XZ packed as (x, z) with size (x, z).")]
        [SerializeField] private List<Vector4> canalRects = new List<Vector4>();

        private readonly List<CanalRect> _rects = new List<CanalRect>();
        private bool _wasAshore;

        /// <summary>Author entry point (public Init idiom — the no-reflection law).</summary>
        public void Configure(float radius, float width, IList<Vector4> rects)
        {
            ringCanalRadius = radius;
            ringCanalWidth = width;
            canalRects.Clear();
            if (rects != null) canalRects.AddRange(rects);
            Rebuild();
        }

        private void Start() => Rebuild();

        private void Rebuild()
        {
            _rects.Clear();
            foreach (var packed in canalRects)
                _rects.Add(new CanalRect(new Vector3(packed.x, 0f, packed.y),
                    new Vector2(packed.z, packed.w)));
        }

        private void FixedUpdate()
        {
            if (_rects.Count == 0 && ringCanalRadius <= 0f) return;

            Vector3 pos = transform.position;
            bool navigable = CanalWaterCore.IsNavigable(pos, _rects, ringCanalRadius, ringCanalWidth);

            if (navigable)
            {
                if (_wasAshore)
                {
                    _wasAshore = false;
                    Debug.Log("ZIPTIDE: SKIFF_WATER state=afloat");
                }
                return;
            }

            if (!_wasAshore)
            {
                _wasAshore = true;
                Debug.Log("ZIPTIDE: SKIFF_WATER state=aground pos=" + pos.ToString("F1"));
            }

            Vector3 push = CanalWaterCore.CorrectionDirection(pos, _rects, ringCanalRadius, ringCanalWidth);
            if (push.sqrMagnitude < 0.0001f) return;
            transform.position = pos + push * (CanalWaterCore.CorrectionSpeed * Time.fixedDeltaTime);
        }
    }
}
