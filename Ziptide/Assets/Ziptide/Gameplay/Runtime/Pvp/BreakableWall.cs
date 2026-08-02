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

        /// <summary>Called by the hammer with the world-space impact point — damages the nearest brick.
        /// DESTRUCTION V2: a breaking brick bursts into tumbling chunks (not a blink-out), and any
        /// bricks that lose their support path to the floor collapse as falling chunks right after.</summary>
        public void HitFromHammer(Vector3 worldHitPoint)
        {
            if (_state == null) return;
            NearestBrick(worldHitPoint, out int col, out int row);
            bool broke = _state.HitBrick(col, row, Time.time);
            if (broke) SpawnBrickDebris(col, row, burst: true);

            var fell = _state.CollapseUnsupported(Time.time);
            foreach (var (fc, fr) in fell) SpawnBrickDebris(fc, fr, burst: false);

            ApplyState();
            Debug.Log("ZIPTIDE: PVP_WALL_HIT col=" + col + " row=" + row + " broke=" + broke
                      + " collapsed=" + fell.Count
                      + " broken=" + _state.BrokenCount + "/" + _state.BrickCount);
        }

        // ── Debris (destruction v2) — chunks, shared-cap'd for Quest, never lethal ─────────────────

        private const float DebrisLifetime = WorldDebrisBudget.DefaultLifetime;

        /// <summary>Chunks for one broken brick. burst = hit directly (3 fragments kicked outward);
        /// otherwise it lost support and drops as one whole chunk with a little shear.</summary>
        private void SpawnBrickDebris(int col, int row, bool burst)
        {
            float sx = wallSize.x / cols, sy = wallSize.y / rows;
            Vector3 local = new Vector3(-wallSize.x * 0.5f + sx * (col + 0.5f),
                                        -wallSize.y * 0.5f + sy * (row + 0.5f), 0f);
            Vector3 world = transform.TransformPoint(local);
            Vector3 brickScale = new Vector3(sx * 0.96f, sy * 0.96f, wallSize.z);
            int seed = col * 73 + row * 131;

            if (burst)
            {
                // Three uneven fragments, kicked slightly out of the wall plane both ways + down —
                // reads as the brick SHATTERING where you hit it.
                for (int f = 0; f < 3; f++)
                {
                    Vector3 fragScale = Vector3.Scale(brickScale,
                        new Vector3(0.5f + 0.2f * Hash01(seed + f), 0.55f, 0.9f));
                    Vector3 jitter = new Vector3((Hash01(seed + f + 7) - 0.5f) * sx * 0.5f,
                                                 (Hash01(seed + f + 13) - 0.5f) * sy * 0.5f, 0f);
                    Vector3 kick = transform.forward * ((Hash01(seed + f + 29) - 0.5f) * 2.2f)
                                 + transform.up * (-0.4f - Hash01(seed + f + 41) * 0.6f)
                                 + transform.right * ((Hash01(seed + f + 53) - 0.5f) * 0.8f);
                    Chunk(world + transform.TransformVector(jitter), fragScale, kick, seed + f);
                }
            }
            else
            {
                // A support-loss chunk: the whole brick drops, shearing slightly sideways — the
                // avalanche read when a slab lets go.
                Vector3 kick = Vector3.down * (0.2f + Hash01(seed + 3) * 0.4f)
                             + transform.right * ((Hash01(seed + 17) - 0.5f) * 0.6f);
                Chunk(world, brickScale, kick, seed);
            }
        }

        private void Chunk(Vector3 worldPos, Vector3 scale, Vector3 velocity, int seed)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "WallChunk";
            go.transform.position = worldPos;
            go.transform.rotation = transform.rotation;
            go.transform.localScale = scale;
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    SetMatColor(mat, Color.Lerp(color, CrackedColor, 0.35f)); // broken faces read darker
                    r.sharedMaterial = mat;
                }
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 2f;
            rb.linearVelocity = velocity;
            rb.angularVelocity = new Vector3(Hash01(seed + 61) - 0.5f, Hash01(seed + 67) - 0.5f,
                                             Hash01(seed + 71) - 0.5f) * 4f;
            go.AddComponent<WallChunkDebris>().lifetime = DebrisLifetime;

            // F3.6: walls and shootable dressing now share ONE hard 24-object physics rail.
            WorldDebrisBudget.Register(go);
        }

        /// <summary>Deterministic int hash → [0,1) (the ForgeMesh idiom, local — Gameplay can't ref Visuals).</summary>
        private static float Hash01(int i)
        {
            unchecked
            {
                int h = i * 374761393 + 1013904223;
                h = (h ^ (h >> 13)) * 1103515245;
                h ^= h >> 16;
                return (h & 0x7FFFFFFF) / 2147483647f;
            }
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

    /// <summary>A tumbling wall chunk: lives briefly, shrinks out over its last moments, then goes —
    /// debris is a READ, never litter (Quest budget) and never a weapon (non-lethal law: it damages
    /// nothing; it only clatters).</summary>
    public class WallChunkDebris : MonoBehaviour
    {
        public float lifetime = 4.5f;
        private const float ShrinkWindow = 0.8f;
        private float _age;
        private Vector3 _baseScale;

        private void Start() { _baseScale = transform.localScale; }

        private void Update()
        {
            _age += Time.deltaTime;
            float left = lifetime - _age;
            if (left <= 0f) { Destroy(gameObject); return; }
            if (left < ShrinkWindow)
                transform.localScale = _baseScale * (left / ShrinkWindow);
        }
    }
}
