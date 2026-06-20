using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A hammer-breakable interior wall: a 3x3 grid of panel segments. A hammer hit escalates
    /// Intact → SmallHole (center segment gone, shoot-through) → LargeHole (center column gone,
    /// pass-through); the hole regenerates after the regen window. Pure timing lives in
    /// <see cref="WallState"/>; this just shows it.
    /// </summary>
    public class BreakableWall : MonoBehaviour
    {
        public Vector3 wallSize = new Vector3(4f, 3f, 0.3f);
        public Color color = new Color(0.40f, 0.42f, 0.46f);

        private readonly WallState _state = new WallState();
        private GameObject[] _segments; // index = row*3 + col

        private void Awake()
        {
            BuildSegments();
            ApplyStage();
        }

        private void Update()
        {
            var prev = _state.Stage;
            _state.Tick(Time.time);
            if (_state.Stage != prev) ApplyStage();
        }

        /// <summary>Called by the hammer on a solid swing-hit.</summary>
        public void HitFromHammer()
        {
            var prev = _state.Stage;
            _state.Hit(Time.time);
            if (_state.Stage != prev)
            {
                ApplyStage();
                Debug.Log("ZIPTIDE: PVP_WALL_BREAK stage=" + _state.Stage);
            }
        }

        private void BuildSegments()
        {
            _segments = new GameObject[9];
            float sx = wallSize.x / 3f, sy = wallSize.y / 3f;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    seg.name = "Seg_" + row + "_" + col;
                    seg.transform.SetParent(transform, false);
                    seg.transform.localPosition = new Vector3((col - 1) * sx, (row - 1) * sy, 0f);
                    seg.transform.localScale = new Vector3(sx * 0.98f, sy * 0.98f, wallSize.z);
                    var r = seg.GetComponent<Renderer>();
                    if (r != null)
                    {
                        ItemFactory.ApplyURPColor(seg, color);
                        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    _segments[row * 3 + col] = seg;
                }
            }
        }

        private void ApplyStage()
        {
            for (int i = 0; i < _segments.Length; i++)
                if (_segments[i] != null) _segments[i].SetActive(true);

            if (_state.Stage == WallStage.SmallHole)
            {
                SetSeg(1, 1, false); // center gap — shoot through
            }
            else if (_state.Stage == WallStage.LargeHole)
            {
                SetSeg(0, 1, false); // center column gone — walk through
                SetSeg(1, 1, false);
                SetSeg(2, 1, false);
            }
        }

        private void SetSeg(int row, int col, bool active)
        {
            var s = _segments[row * 3 + col];
            if (s != null) s.SetActive(active);
        }
    }
}
