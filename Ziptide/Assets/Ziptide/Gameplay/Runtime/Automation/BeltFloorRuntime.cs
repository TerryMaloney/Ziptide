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
        [Tooltip("Save identity (4.1f) — player edits persist under scene+floorId. Empty = this " +
                 "floor never persists (throwaway rigs).")]
        public string floorId = "";

        private const float FixedStep = 1f / 30f;  // fixed sim cadence — reproducible factories
        private const float PayoutInterval = 1.0f;

        private BeltLattice _lattice;
        private float _accum;
        private float _nextPayout;
        private readonly Dictionary<BeltItem, GameObject> _pucks = new Dictionary<BeltItem, GameObject>();
        private readonly Dictionary<int, GameObject> _cellVisuals = new Dictionary<int, GameObject>();
        private static readonly List<BeltItem> _gone = new List<BeltItem>(); // scratch

        /// <summary>Live floors — held BeltTileItems query these for ghost + placement.</summary>
        public static readonly List<BeltFloorRuntime> Active = new List<BeltFloorRuntime>();
        private void OnEnable() => Active.Add(this);
        private void OnDisable() => Active.Remove(this);

        // ── Patch-time authoring (serialized; no scene wiring) ─────────────────────────────────
        public void AuthorBelt(int x, int z, BeltDir dir)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Belt, dir = dir });
        public void AuthorSource(int x, int z, BeltDir dir, string resourceId)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Source, dir = dir, resourceId = resourceId });
        public void AuthorSink(int x, int z, string payoutResourceId)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Sink, resourceId = payoutResourceId });
        public void AuthorSplitter(int x, int z, BeltDir dir)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Splitter, dir = dir });
        /// <summary>A machine port: emits only when an adapter feeds it (empty resourceId = port).</summary>
        public void AuthorPort(int x, int z, BeltDir dir)
            => cells.Add(new BeltCellSpec { x = x, z = z, kind = CellKind.Source, dir = dir, resourceId = "" });

        /// <summary>Machine adapters (mine ports etc.) emit through here. False = blocked, keep stock.</summary>
        public bool TryEmitPort(int x, int z, string resourceId)
            => _lattice != null && _lattice.TryEmit(x, z, resourceId);

        /// <summary>The live sim — read-only consumers (4.1h conductor route tracing). Null before Start.</summary>
        public BeltLattice Lattice => _lattice;

        /// <summary>Interpolated grid coordinates → world (cell CENTER is x.0, the BeltRoute convention).</summary>
        public Vector3 GridToWorld(float gx, float gz)
            => transform.TransformPoint(new Vector3((gx + 0.5f) * cellSize, 0f, (gz + 0.5f) * cellSize));

        private void Start()
        {
            RestoreFromProfile(); // 4.1f: cells becomes authored ⊕ the player's saved overlay

            _lattice = new BeltLattice(width, depth);
            foreach (var c in cells)
            {
                switch (c.kind)
                {
                    case CellKind.Belt: _lattice.PlaceBelt(c.x, c.z, c.dir); break;
                    case CellKind.Source: _lattice.PlaceSource(c.x, c.z, c.dir, c.resourceId); break;
                    case CellKind.Sink: _lattice.PlaceSink(c.x, c.z); break;
                    case CellKind.Splitter: _lattice.PlaceSplitter(c.x, c.z, c.dir); break;
                }
            }
            BuildTiles();
            Debug.Log("ZIPTIDE: BELT_FLOOR ready cells=" + cells.Count +
                      " size=" + width + "x" + depth);
        }

        // ── Persistence (HARDWIRING 4.1f — player factories survive quit/reload) ────────────────

        private readonly HashSet<long> _authoredBeltCells = new HashSet<long>(); // pre-overlay Belt coords
        private BeltFloorState _saveState; // lazy — created on the first player edit

        private long CellKey(int x, int z) => ((long)z << 32) | (uint)x;

        /// <summary>The overlay's home: this scene's WorldState (worldId = scene name, the
        /// BeltMinePortRuntime convention).</summary>
        private BeltFloorState SaveState(bool createIfMissing)
        {
            if (_saveState != null) return _saveState;
            if (string.IsNullOrEmpty(floorId)) return null;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return null;
            var world = profile.GetWorld(gameObject.scene.name, createIfMissing);
            if (world == null) return null;
            _saveState = BeltFloorSave.GetFloor(world, floorId, createIfMissing);
            return _saveState;
        }

        /// <summary>Rebuild <see cref="cells"/> as canonical-authored + saved player overlay. The
        /// authored Belt coords are snapshotted first so RemoveBeltAt can tell an authored pick-up
        /// (remember it as removed) from a player-placed one (just drop it from the overlay).</summary>
        private void RestoreFromProfile()
        {
            _authoredBeltCells.Clear();
            foreach (var c in cells)
                if (c.kind == CellKind.Belt) _authoredBeltCells.Add(CellKey(c.x, c.z));

            var state = SaveState(createIfMissing: false);
            if (state == null || (state.placed.Count == 0 && state.removedAuthored.Count == 0))
                return; // neutral default — authored layout untouched

            var authored = new List<BeltCellRecord>(cells.Count);
            foreach (var c in cells)
                authored.Add(new BeltCellRecord
                {
                    x = c.x, z = c.z, kind = (int)c.kind, dir = (int)c.dir,
                    resourceId = c.resourceId ?? ""
                });
            var effective = BeltFloorSave.Apply(authored, state);

            cells = new List<BeltCellSpec>(effective.Count);
            foreach (var r in effective)
                cells.Add(new BeltCellSpec
                {
                    x = r.x, z = r.z, kind = (CellKind)r.kind, dir = (BeltDir)r.dir,
                    resourceId = r.resourceId
                });
            Debug.Log("ZIPTIDE: BELT_RESTORE floor=" + floorId +
                      " placed=" + state.placed.Count +
                      " removed=" + state.removedAuthored.Count);
        }

        private void PersistPlace(BeltCellSpec spec)
        {
            var state = SaveState(createIfMissing: true);
            if (state == null) return;
            BeltFloorSave.RecordPlace(state, new BeltCellRecord
            {
                x = spec.x, z = spec.z, kind = (int)spec.kind, dir = (int)spec.dir,
                resourceId = spec.resourceId ?? ""
            });
            SaveSystem.AutosaveNow("belt_edit");
        }

        private void PersistRemove(int x, int z)
        {
            bool wasAuthored = _authoredBeltCells.Contains(CellKey(x, z));
            var state = SaveState(createIfMissing: wasAuthored);
            if (state == null) return;
            BeltFloorSave.RecordRemove(state, x, z, wasAuthored);
            SaveSystem.AutosaveNow("belt_edit");
        }

        private Vector3 CellCenter(int x, int z)
            => transform.TransformPoint(new Vector3((x + 0.5f) * cellSize, 0f, (z + 0.5f) * cellSize));

        private void BuildTiles()
        {
            foreach (var c in cells) BuildCellVisual(c);
        }

        /// <summary>One cell's visual — a per-cell container so runtime removal is one Destroy.
        /// Hand-placed BELT tiles are grip-pickable (select → the belt returns to your hand).</summary>
        private void BuildCellVisual(BeltCellSpec c)
        {
            var tileCol = new Color(0.17f, 0.19f, 0.21f);
            var chevCol = new Color(0.35f, 0.95f, 0.75f); // salvage teal — the automation accent
            var srcCol = new Color(0.30f, 0.45f, 0.60f);
            var sinkCol = new Color(0.55f, 0.40f, 0.20f);

            var cellRoot = new GameObject("Cell_" + c.x + "_" + c.z);
            cellRoot.transform.SetParent(transform, false);
            _cellVisuals[c.z * width + c.x] = cellRoot;

            Vector3 at = CellCenter(c.x, c.z);
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "BeltTile";
            tile.transform.SetParent(cellRoot.transform, true);
            tile.transform.position = at + Vector3.up * 0.05f;
            tile.transform.localScale = new Vector3(cellSize * 0.96f, 0.1f, cellSize * 0.96f);
            ItemFactory.ApplyURPColor(tile,
                c.kind == CellKind.Source ? srcCol : c.kind == CellKind.Sink ? sinkCol : tileCol);

            if (c.kind == CellKind.Belt || c.kind == CellKind.Source || c.kind == CellKind.Splitter)
            {
                // Direction chevron: a flat teal bar pointing along flow — readable from above.
                // Parented to the UNSCALED cell root (a child of the squashed tile would shear).
                // Splitters get a SECOND bar on the right-hand exit — the fork reads at a glance.
                int bars = c.kind == CellKind.Splitter ? 2 : 1;
                for (int b = 0; b < bars; b++)
                {
                    var chev = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    chev.name = b == 0 ? "Chevron" : "ChevronAlt";
                    var cc = chev.GetComponent<Collider>();
                    if (cc != null) Destroy(cc);
                    chev.transform.SetParent(cellRoot.transform, true);
                    chev.transform.position = at + Vector3.up * 0.11f;
                    chev.transform.rotation = transform.rotation
                        * Quaternion.Euler(0f, 90f * ((int)c.dir + b), 0f);
                    chev.transform.localScale = new Vector3(0.14f, 0.03f, cellSize * (b == 0 ? 0.5f : 0.38f));
                    ItemFactory.ApplyURPColor(chev, chevCol);
                    var cr = chev.GetComponent<Renderer>();
                    if (cr != null) cr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            var tr = tile.GetComponent<Renderer>();
            if (tr != null) tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (c.kind == CellKind.Belt)
            {
                // Grip a placed belt to pick it back up (its riding item lifts with it — Clear's law).
                int cx = c.x, cz = c.z;
                var pick = tile.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRSimpleInteractable>();
                pick.selectEntered.AddListener(_ => RemoveBeltAt(cx, cz));
            }
        }

        // ── Hand placement (HARDWIRING 4.1c — the VR-unique verb) ───────────────────────────────

        /// <summary>World position → cell coords. False when outside this floor's grid.</summary>
        public bool TryWorldToCell(Vector3 worldPos, out int x, out int z)
        {
            Vector3 local = transform.InverseTransformPoint(worldPos);
            x = Mathf.FloorToInt(local.x / cellSize);
            z = Mathf.FloorToInt(local.z / cellSize);
            return x >= 0 && x < width && z >= 0 && z < depth && Mathf.Abs(local.y) < 2.5f;
        }

        public bool CanPlaceAt(int x, int z)
            => _lattice != null && _lattice.KindAt(x, z) == CellKind.Empty;

        /// <summary>Quantize a held tile's facing to the nearest cardinal in floor space.</summary>
        public BeltDir DirFromForward(Vector3 worldForward)
        {
            Vector3 f = transform.InverseTransformDirection(worldForward);
            if (Mathf.Abs(f.x) >= Mathf.Abs(f.z)) return f.x >= 0f ? BeltDir.East : BeltDir.West;
            return f.z >= 0f ? BeltDir.North : BeltDir.South;
        }

        /// <summary>Place a belt from the hand: snaps to the cell under <paramref name="worldPos"/>,
        /// direction from the hand's facing. False if off-grid or the cell is taken.</summary>
        public bool PlaceBeltFromHand(Vector3 worldPos, Vector3 worldForward)
        {
            if (!TryWorldToCell(worldPos, out int x, out int z) || !CanPlaceAt(x, z)) return false;
            var dir = DirFromForward(worldForward);
            _lattice.PlaceBelt(x, z, dir);
            var spec = new BeltCellSpec { x = x, z = z, kind = CellKind.Belt, dir = dir };
            cells.Add(spec);
            BuildCellVisual(spec);
            PersistPlace(spec); // 4.1f: player factories survive quit/reload
            Debug.Log("ZIPTIDE: BELT_PLACE x=" + x + " z=" + z + " dir=" + dir);
            return true;
        }

        /// <summary>Pick a placed belt back up: clears the lattice cell (any riding item lifts with
        /// it), removes the visual, and spawns a grabbable tile just above the cell.</summary>
        public void RemoveBeltAt(int x, int z)
        {
            if (_lattice == null || _lattice.KindAt(x, z) != CellKind.Belt) return;
            var lifted = _lattice.Clear(x, z);
            if (lifted != null && _pucks.TryGetValue(lifted, out var puck))
            {
                Ziptide.Core.GamePool.Release("belt_puck", puck);
                _pucks.Remove(lifted);
            }
            int idx = z * width + x;
            if (_cellVisuals.TryGetValue(idx, out var vis) && vis != null) Destroy(vis);
            _cellVisuals.Remove(idx);
            for (int i = cells.Count - 1; i >= 0; i--)
                if (cells[i].x == x && cells[i].z == z) cells.RemoveAt(i);
            PersistRemove(x, z); // 4.1f: an authored cell is remembered as removed; a placed one just leaves

            BeltTileItem.Spawn(CellCenter(x, z) + Vector3.up * 0.35f);
            Debug.Log("ZIPTIDE: BELT_PICKUP x=" + x + " z=" + z);
        }

        // ── Ghost preview (shown by the held tile; hidden on release) ───────────────────────────

        private GameObject _ghost;
        private Transform _ghostChev;
        private Renderer _ghostTileR, _ghostChevR;

        public void ShowGhost(Vector3 worldPos, Vector3 worldForward)
        {
            if (!TryWorldToCell(worldPos, out int x, out int z)) { HideGhost(); return; }
            if (_ghost == null) BuildGhost();
            bool ok = CanPlaceAt(x, z);
            var dir = DirFromForward(worldForward);
            Vector3 at = CellCenter(x, z);
            _ghost.SetActive(true);
            _ghost.transform.position = at + Vector3.up * 0.06f;
            _ghostChev.position = at + Vector3.up * 0.14f;
            _ghostChev.rotation = transform.rotation * Quaternion.Euler(0f, 90f * (int)dir, 0f);
            var col = ok ? new Color(0.35f, 0.95f, 0.75f) : new Color(0.9f, 0.3f, 0.25f);
            ItemFactory.ApplyURPColor(_ghostTileR.gameObject, col * 0.9f);
            ItemFactory.ApplyURPColor(_ghostChevR.gameObject, col);
        }

        public void HideGhost() { if (_ghost != null) _ghost.SetActive(false); }

        private void BuildGhost()
        {
            _ghost = new GameObject("BeltGhost");
            _ghost.transform.SetParent(transform, false);
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "GhostTile";
            Destroy(tile.GetComponent<Collider>());
            tile.transform.SetParent(_ghost.transform, false);
            tile.transform.localScale = new Vector3(cellSize * 0.9f, 0.02f, cellSize * 0.9f);
            _ghostTileR = tile.GetComponent<Renderer>();
            var chev = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chev.name = "GhostChevron";
            Destroy(chev.GetComponent<Collider>());
            chev.transform.SetParent(_ghost.transform, true);
            chev.transform.localScale = new Vector3(0.14f, 0.03f, cellSize * 0.5f);
            _ghostChev = chev.transform;
            _ghostChevR = chev.GetComponent<Renderer>();
            foreach (var r in _ghost.GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
