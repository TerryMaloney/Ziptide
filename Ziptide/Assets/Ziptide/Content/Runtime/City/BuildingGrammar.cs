using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>The module vocabulary a building shell is assembled from. Each id resolves through
    /// the Art Registry (docs/design/ART_REGISTRY.md) — primitive fallback until a kit exists.</summary>
    public enum BuildingModule { WallSolid, WallWindow, Doorway, FloorSlab, RoofFlat, RoofRaked, CornerTrim }

    /// <summary>One placed module in a building plan: which module, where on the perimeter grid,
    /// which storey, and which way it faces (0=S, 1=E, 2=N, 3=W in lot-local space).</summary>
    public struct ModulePlacement
    {
        public BuildingModule Module;
        public int Face;        // 0..3; FloorSlab/Roof* use -1
        public int CellIndex;   // cell along that face, 0-based; slabs use -1
        public int Storey;      // 0 = ground
        public Vector2 LocalPos;   // XZ center in lot-local meters
        public float Rotation;     // Y degrees the module faces outward
    }

    /// <summary>The pure output: everything a builder needs to render one building.</summary>
    public class BuildingPlan
    {
        public Rect Lot;
        public int Storeys;
        public float ModuleWidth;
        public float StoreyHeight;
        public bool RakedRoof;
        public int DoorFace = -1;
        public int DoorCell = -1;
        public readonly List<ModulePlacement> Modules = new List<ModulePlacement>();
    }

    /// <summary>Pure style parameters (the BotProfileData pattern — the SO mirrors this).</summary>
    public struct BuildingStyleData
    {
        public float ModuleWidth;      // wall module width, meters (grid cell)
        public float StoreyHeight;
        public int MinStoreys, MaxStoreys;
        public float WindowChance;     // 0..1 per upper wall cell
        public float RakedRoofChance;  // else flat
        public static BuildingStyleData Default => new BuildingStyleData
        { ModuleWidth = 3f, StoreyHeight = 3.2f, MinStoreys = 1, MaxStoreys = 3, WindowChance = 0.45f, RakedRoofChance = 0.35f };
    }

    /// <summary>
    /// ARCHITECTURE V2 Q2b / V2.5 H1 (PDF §4) — PURE seeded building assembly. Takes a
    /// <see cref="Lot"/> from LotPartitioner and a style, returns a <see cref="BuildingPlan"/> of
    /// socketed modules. WFC-lite by design: the single forced choice (the DOOR — lowest entropy)
    /// collapses first onto a STREET-FRONTED edge (LotPartitioner guarantees one exists), then
    /// windows/solids fill under seeded weights. The socket set is contradiction-free, so no
    /// backtracking is ever needed — "no door into a wall" is true by construction, and the audit
    /// gate re-verifies it in the baked scene. Deterministic: same lot+style+seed → identical plan.
    /// </summary>
    public static class BuildingGrammar
    {
        /// <summary>Inset of the building footprint from the lot bounds (yard/margin).</summary>
        public const float FootprintInset = 0.6f;

        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
            public int Range(int minIncl, int maxIncl) => minIncl + (int)(Next01() * 0.99999f * (maxIncl - minIncl + 1));
        }

        /// <summary>Plan one building on a lot. Returns null when the lot can't hold even a 1-cell shell.</summary>
        public static BuildingPlan Plan(Lot lot, BuildingStyleData style, int seed)
        {
            if (!lot.HasFrontage) return null; // grammar law: unfronted lots don't build
            var fp = new Rect(lot.Bounds.x + FootprintInset, lot.Bounds.y + FootprintInset,
                lot.Bounds.width - 2f * FootprintInset, lot.Bounds.height - 2f * FootprintInset);
            float w = Mathf.Max(1f, style.ModuleWidth);
            int cellsX = Mathf.FloorToInt(fp.width / w);
            int cellsZ = Mathf.FloorToInt(fp.height / w);
            if (cellsX < 1 || cellsZ < 1) return null;

            // Center the cell grid inside the footprint.
            float originX = fp.x + (fp.width - cellsX * w) * 0.5f;
            float originZ = fp.y + (fp.height - cellsZ * w) * 0.5f;

            var rng = new Rng(seed);
            var plan = new BuildingPlan
            {
                Lot = lot.Bounds,
                ModuleWidth = w,
                StoreyHeight = Mathf.Max(2.4f, style.StoreyHeight),
                Storeys = ClampStoreys(style, ref rng),
                RakedRoof = rng.Next01() < style.RakedRoofChance,
            };

            // ── The door collapses first: a fronted face, then a cell on it ──
            var frontedFaces = new List<int>(4);
            if (lot.FrontS) frontedFaces.Add(0);
            if (lot.FrontE) frontedFaces.Add(1);
            if (lot.FrontN) frontedFaces.Add(2);
            if (lot.FrontW) frontedFaces.Add(3);
            plan.DoorFace = frontedFaces[rng.Range(0, frontedFaces.Count - 1)];
            int doorFaceCells = (plan.DoorFace == 0 || plan.DoorFace == 2) ? cellsX : cellsZ;
            plan.DoorCell = rng.Range(0, doorFaceCells - 1);

            // ── Perimeter walls, storey by storey ──
            for (int storey = 0; storey < plan.Storeys; storey++)
            {
                for (int face = 0; face < 4; face++)
                {
                    int faceCells = (face == 0 || face == 2) ? cellsX : cellsZ;
                    for (int cell = 0; cell < faceCells; cell++)
                    {
                        BuildingModule m;
                        if (storey == 0 && face == plan.DoorFace && cell == plan.DoorCell)
                            m = BuildingModule.Doorway;
                        else if (storey > 0 && rng.Next01() < style.WindowChance)
                            m = BuildingModule.WallWindow;
                        else if (storey == 0 && rng.Next01() < style.WindowChance * 0.5f)
                            m = BuildingModule.WallWindow; // sparser at street level
                        else
                            m = BuildingModule.WallSolid;

                        plan.Modules.Add(new ModulePlacement
                        {
                            Module = m,
                            Face = face,
                            CellIndex = cell,
                            Storey = storey,
                            LocalPos = FaceCellCenter(face, cell, cellsX, cellsZ, originX, originZ, w),
                            Rotation = FaceRotation(face),
                        });
                    }
                }
                // Corner trims (one per corner per storey — the silhouette detail).
                for (int corner = 0; corner < 4; corner++)
                    plan.Modules.Add(new ModulePlacement
                    {
                        Module = BuildingModule.CornerTrim,
                        Face = corner,
                        CellIndex = -1,
                        Storey = storey,
                        LocalPos = CornerPos(corner, cellsX, cellsZ, originX, originZ, w),
                        Rotation = 90f * corner,
                    });
            }

            // ── Slabs: one floor per storey + the roof ──
            var center = new Vector2(originX + cellsX * w * 0.5f, originZ + cellsZ * w * 0.5f);
            for (int storey = 0; storey < plan.Storeys; storey++)
                plan.Modules.Add(new ModulePlacement
                { Module = BuildingModule.FloorSlab, Face = -1, CellIndex = -1, Storey = storey, LocalPos = center });
            plan.Modules.Add(new ModulePlacement
            {
                Module = plan.RakedRoof ? BuildingModule.RoofRaked : BuildingModule.RoofFlat,
                Face = -1,
                CellIndex = -1,
                Storey = plan.Storeys,
                LocalPos = center,
            });

            return plan;
        }

        /// <summary>Footprint size in whole cells for a lot+style (what the builder scales slabs to).</summary>
        public static Vector2Int CellCounts(Lot lot, BuildingStyleData style)
        {
            float w = Mathf.Max(1f, style.ModuleWidth);
            return new Vector2Int(
                Mathf.FloorToInt((lot.Bounds.width - 2f * FootprintInset) / w),
                Mathf.FloorToInt((lot.Bounds.height - 2f * FootprintInset) / w));
        }

        private static int ClampStoreys(BuildingStyleData s, ref Rng rng)
        {
            int min = Mathf.Max(1, s.MinStoreys);
            int max = Mathf.Max(min, s.MaxStoreys);
            return rng.Range(min, max);
        }

        private static Vector2 FaceCellCenter(int face, int cell, int cellsX, int cellsZ,
            float originX, float originZ, float w)
        {
            switch (face)
            {
                case 0: return new Vector2(originX + (cell + 0.5f) * w, originZ);                    // S edge
                case 1: return new Vector2(originX + cellsX * w, originZ + (cell + 0.5f) * w);        // E edge
                case 2: return new Vector2(originX + (cell + 0.5f) * w, originZ + cellsZ * w);        // N edge
                default: return new Vector2(originX, originZ + (cell + 0.5f) * w);                    // W edge
            }
        }

        private static Vector2 CornerPos(int corner, int cellsX, int cellsZ, float originX, float originZ, float w)
        {
            switch (corner)
            {
                case 0: return new Vector2(originX, originZ);                                  // SW
                case 1: return new Vector2(originX + cellsX * w, originZ);                     // SE
                case 2: return new Vector2(originX + cellsX * w, originZ + cellsZ * w);        // NE
                default: return new Vector2(originX, originZ + cellsZ * w);                    // NW
            }
        }

        /// <summary>Outward Y rotation for a face (module +Z points out of the building).</summary>
        public static float FaceRotation(int face)
        {
            switch (face)
            {
                case 0: return 180f; // S faces -Z
                case 1: return 90f;  // E faces +X
                case 2: return 0f;   // N faces +Z
                default: return 270f; // W faces -X
            }
        }
    }
}
