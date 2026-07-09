using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content.Automation;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>One authored lattice cell — the serialized truth a patcher writes and Start() builds
    /// the pure lattice from (the pure sim itself never serializes).</summary>
    [System.Serializable]
    public struct BeltCellSpec
    {
        public int x, z;
        public CellKind kind;
        public BeltDir dir;
        public string resourceId; // Source: what it emits · Sink: what the payout grants
    }

    /// <summary>
    /// HARDWIRING 4.1b / AUTOMATION_CONVEYORS — the conveyor scene translator: owns ONE
    /// <see cref="BeltLattice"/>, builds the watchable layer (belt tiles + direction chevrons +
    /// pooled item pucks lerped from lattice state), ticks the sim on a fixed step (deterministic),
    /// and pays sunk items through the ONE economy path (<see cref="RewardRouter"/>,
    /// <see cref="LedgerSource.Factory"/>) onto the live SaveSystem profile.
    ///
    /// Patch-time contract (the SalvageCache lesson): a patcher authors <see cref="cells"/> (public
    /// Author* methods) — serialized data only; ALL building happens at runtime in Start(). Visuals
    /// are a skin: the lattice is the state, pucks just show it.
    /// </summary>
    public class BeltFloorRuntime : MonoBehaviour
    {
        [Tooltip("Lattice size in cells.")]
        public int width = 8, depth = 4;
        [Tooltip("World meters per cell.")]
        public float cellSize = 0.8f;
        [Tooltip("Authored layout — the serialized truth (patchers write via Author*).")]
        public List<BeltCellSpec> cells = new List<BeltCellSpec>();

        private const float FixedStep = 1f / 30f;  // fixed sim cadence — reproducible factories
        private const float PayoutInterval = 1.0f;

        private BeltLattice _lattice;
        private float _accum;
        private float _nextPayout;
        private readonly Dictionary<BeltItem, GameObject> _pucks = new Dictionary<BeltItem, GameObject>();
        private static readonly List<BeltItem> _gone = new List<BeltItem>(); // scratch

        // ── Patch-time authoring (serialized; no scene wiring) ─────────────────────────────────
        public void AuthorBelt(int x, int z, BeltDir dir)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Belt, dir = dir });
        public void AuthorSource(int x, int z, BeltDir dir, string resourceId)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Source, dir = dir, resourceId = resourceId });
        public void AuthorSink(int x, int z, string payoutResourceId)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Sink, resourceId = payoutResourceId });

        private void Start()
        {
            _lattice = new BeltLattice(width, depth);
            foreach (var c in cells)
            {
                switch (c.kind)
                {
                    case CellKind.Belt: _lattice.PlaceBelt(c.x, c.z, c.dir); break;
                    case CellKind.Source: _lattice.PlaceSource(c.x, c.z, c.dir, c.resourceId); break;
                    case CellKind.Sink: _lattice.PlaceSink(c.x, c.z); break;
                }
            }
            BuildTiles();
            Debug.Log("ZIPTIDE: BELT_FLOOR ready cells=" + cells.Count +
                      " size=" + width + "x" + depth);
        }

        private Vector3 CellCenter(int x, int z)
            => transform.TransformPoint(new Vector3((x + 0.5f) * cellSize, 0f, (z + 0.5f) * cellSize));

        private void BuildTiles()
        {
            var tileCol = new Color(0.17f, 0.19f, 0.21f);
            var chevCol = new Color(0.35f, 0.95f, 0.75f); // salvage teal — the automation accent
            var srcCol = new Color(0.30f, 0.45f, 0.60f);
            var sinkCol = new Color(0.55f, 0.40f, 0.20f);

            foreach (var c in cells)
            {
                Vector3 at = CellCenter(c.x, c.z);
                var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = "BeltTile";
                tile.transform.SetParent(transform, true);
                tile.transform.position = at + Vector3.up * 0.05f;
                tile.transform.localScale = new Vector3(cellSize * 0.96f, 0.1f,cellSize * 0.96f);
                ItemFactory.ApplyURPColor(tile,
                    c.kind == CellKind.Source ? srcCol : c.kind == CellKind.Sink ? sinkCol : tileCol);

                if (c.kind == CellKind.Belt || c.kind == CellKind.Source)
                {
                    // Direction chevron: a flat teal bar pointing along flow — readable from above.
                    // Parented to the UNSCALED floor root (a child of the squashed tile would shear).
                    var chev = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    chev.name = "Chevron";
                    var cc = chev.GetComponent<Collider>();
                    if (cc != null) Destroy(cc);
                    chev.transform.SetParent(transform, true);
                    chev.transform.position = at + Vector3.up * 0.11f;
                    chev.transform.rotation = transform.rotation * Quaternion.Euler(0f, 90f * (int)c.dir, 0f);
                    chev.transform.localScale = new Vector3(0.14f, 0.03f, cellSize * 0.5f);
                    ItemFactory.ApplyURPColor(chev, chevCol);
                    var cr = chev.GetComponent<Renderer>();
                    if (cr != null) cr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                var tr = tile.GetComponent<Renderer>();
                if (tr != null) tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private void Update()
        {
            if (_lattice == null) return;

            _accum += Time.deltaTime;
            while (_accum >= FixedStep)
            {
                _accum -= FixedStep;
                _lattice.Tick(FixedStep);
            }

            SyncPucks();

            if (Time.time >= _nextPayout)
            {
                _nextPayout = Time.time + PayoutInterval;
                PaySunk();
            }
        }

        private void SyncPucks()
        {
            // New/moved items → pooled pucks lerped between cell centers by Progress.
            var items = _lattice.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                if (!_pucks.TryGetValue(it, out var go) || go == null)
                {
                    go = Ziptide.Core.GamePool.Get("belt_puck", BuildPuck, Vector3.zero);
                    _pucks[it] = go;
                }
                Vector3 a = CellCenter(it.X, it.Z);
                var dir = _lattice.DirAt(it.X, it.Z);
                Vector3 step = dir == BeltDir.East ? Vector3.right : dir == BeltDir.West ? Vector3.left
                             : dir == BeltDir.North ? Vector3.forward : Vector3.back;
                // Progress 0..0.5 rides into the cell center; 0.5..1 rides toward the lip.
                Vector3 pos = a + transform.TransformDirection(step) * ((it.Progress - 0.5f) * cellSize);
                pos.y = CellCenter(it.X, it.Z).y + 0.22f;
                go.transform.position = pos;
            }

            // Consumed items → release their pucks.
            _gone.Clear();
            foreach (var kv in _pucks)
            {
                bool alive = false;
                for (int i = 0; i < items.Count; i++) if (ReferenceEquals(items[i], kv.Key)) { alive = true; break; }
                if (!alive) _gone.Add(kv.Key);
            }
            foreach (var dead in _gone)
            {
                Ziptide.Core.GamePool.Release("belt_puck", _pucks[dead]);
                _pucks.Remove(dead);
            }
        }

        private static GameObject BuildPuck()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "BeltPuck";
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);
            go.transform.localScale = Vector3.one * 0.24f;
            ItemFactory.ApplyURPColor(go, new Color(0.75f, 0.62f, 0.35f)); // scrap gold
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private void PaySunk()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return;
            foreach (var c in cells)
            {
                if (c.kind != CellKind.Sink || string.IsNullOrEmpty(c.resourceId)) continue;
                int n = _lattice.DrainSunk(c.resourceId);
                if (n <= 0) continue;
                RewardRouter.Grant(profile, LedgerSource.Factory, c.resourceId, n,
                    reason: "belt_sink", relatedId: "");
                Debug.Log("ZIPTIDE: BELT_SUNK resource=" + c.resourceId + " n=" + n);
            }
        }
    }

}
