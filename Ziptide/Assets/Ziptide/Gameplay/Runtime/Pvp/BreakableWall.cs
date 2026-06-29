using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A hammer-breakable interior wall built from a fine grid of small bricks. Each hammer hit damages
    /// only the brick NEAREST the impact point; a brick breaks (opens a gap) after a couple of swings and
    /// darkens as it cracks, so the wall comes apart a small piece at a time exactly where you hit it.
    /// Pure timing/HP lives in <see cref="WallState"/>; this just maps impact points to bricks and shows it.
    /// The whole wall regenerates after the regen window.
    /// </summary>
    public class BreakableWall : MonoBehaviour
    {
        public Vector3 wallSize = new Vector3(4f, 3f, 0.3f);
        public Color color = new Color(0.40f, 0.42f, 0.46f);

        [Tooltip("Brick grid resolution (more = smaller pieces).")]
        public int cols = 8;
        public int rows = 6;
        [Tooltip("Hammer swings needed to break one brick.")]
        public int brickHits = 4;

        private static readonly Color CrackedColor = new Color(0.12f, 0.08f, 0.08f);

        private WallState _state;
        private GameObject[] _bricks;   // index = row * cols + col
        private Material[] _mats;       // cached per brick to tint cracks without churning materials

        private void Awake()
        {
            _state = new WallState(cols, rows, brickHits);
            BuildBricks();
            ApplyState();
        }

        private void Update()
        {
            int before = _state.BrokenCount;
            bool wasDamaged = _state.AnyDamaged;
            _state.Tick(Time.time);
            // Re-show everything when the wall heals back, or refresh if the broken set changed.
            if (_state.BrokenCount != before || (wasDamaged && !_state.AnyDamaged)) ApplyState();
        }

        /// <summary>Called by the hammer with the world-space impact point — damages the nearest brick.</summary>
        public void HitFromHammer(Vector3 worldHitPoint)
        {
            if (_state == null) return;
            NearestBrick(worldHitPoint, out int col, out int row);
            bool broke = _state.HitBrick(col, row, Time.time);
            ApplyState();
            Debug.Log("ZIPTIDE: PVP_WALL_HIT col=" + col + " row=" + row + " broke=" + broke
                      + " broken=" + _state.BrokenCount + "/" + _state.BrickCount);
        }

        /// <summary>Back-compat overload (no impact point) — damages the brick at the wall's center.</summary>
        public void HitFromHammer() { HitFromHammer(transform.position); }

        // Map a world impact point onto a brick column/row (clamped to the grid).
        private void NearestBrick(Vector3 worldHitPoint, out int col, out int row)
        {
            Vector3 local = transform.InverseTransformPoint(worldHitPoint);
            float sx = wallSize.x / cols, sy = wallSize.y / rows;
            float fx = (local.x + wallSize.x * 0.5f) / sx;
            float fy = (local.y + wallSize.y * 0.5f) / sy;
            col = Mathf.Clamp(Mathf.FloorToInt(fx), 0, cols - 1);
            row = Mathf.Clamp(Mathf.FloorToInt(fy), 0, rows - 1);
        }

        private void BuildBricks()
        {
            int count = cols * rows;
            _bricks = new GameObject[count];
            _mats = new Material[count];
            float sx = wallSize.x / cols, sy = wallSize.y / rows;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    seg.name = "Brick_" + row + "_" + col;
                    seg.transform.SetParent(transform, false);
                    float x = -wallSize.x * 0.5f + sx * (col + 0.5f);
                    float y = -wallSize.y * 0.5f + sy * (row + 0.5f);
                    seg.transform.localPosition = new Vector3(x, y, 0f);
                    seg.transform.localScale = new Vector3(sx * 0.96f, sy * 0.96f, wallSize.z);
                    int i = row * cols + col;
                    var r = seg.GetComponent<Renderer>();
                    if (r != null)
                    {
                        var shader = Shader.Find("Universal Render Pipeline/Lit");
                        if (shader == null) shader = Shader.Find("Standard");
                        if (shader != null)
                        {
                            var mat = new Material(shader);
                            SetMatColor(mat, color);
                            r.sharedMaterial = mat;
                            _mats[i] = mat;
                        }
                        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    _bricks[i] = seg;
                }
            }
        }

        private void ApplyState()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int i = row * cols + col;
                    var b = _bricks[i];
                    if (b == null) continue;
                    bool broken = _state.IsBroken(col, row);
                    if (b.activeSelf == broken) b.SetActive(!broken);
                    if (!broken && _mats[i] != null)
                    {
                        int hits = _state.HitsOn(col, row);
                        float t = brickHits > 0 ? (float)hits / brickHits : 0f;
                        SetMatColor(_mats[i], Color.Lerp(color, CrackedColor, t));
                    }
                }
            }
        }

        private static void SetMatColor(Material mat, Color c)
        {
            if (mat == null) return;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
        }
    }
}
