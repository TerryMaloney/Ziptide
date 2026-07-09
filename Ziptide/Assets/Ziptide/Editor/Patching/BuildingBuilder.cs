#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// V2.5 H1 — renders LotPartitioner + BuildingGrammar output into a district: real multi-storey
    /// building shells with ENTERABLE doorways, replacing facade slabs. Modules resolve through the
    /// ArtModuleRegistry ("buildingModule:&lt;styleId&gt;/&lt;Module&gt;") with primitive fallback, so Picasso's
    /// kits upgrade the look without touching this file. Dormant until a district opts in via
    /// DistrictDef.buildingStyleId (style assets in Resources/BuildingStyles, seeded by
    /// BuildingStyleAuthor). Every door plants a "__DOOR" marker facing outward — the audit's
    /// BUILDING_DOOR_BLOCKED gate raycasts from it.
    /// </summary>
    public static class BuildingBuilder
    {
        /// <summary>Renderer budget per district before the audit blocks (Quest draw-call proxy).</summary>
        public const int MaxRenderersPerDistrict = 700;

        public static void Build(Transform cityRoot, DistrictDef district, float walkwayHeight, int worldSeed)
        {
            if (district == null || string.IsNullOrEmpty(district.buildingStyleId)) return;
            var style = Resources.Load<BuildingStyleDefinition>("BuildingStyles/" + district.buildingStyleId);
            if (style == null)
            {
                Debug.LogWarning("[Ziptide] BUILDING_STYLE_MISSING id=" + district.buildingStyleId
                    + " district=" + district.id + " (Resources/BuildingStyles)");
                return;
            }

            var root = new GameObject("__BUILDINGS_" + district.id);
            root.transform.SetParent(cityRoot, false);
            root.transform.position = new Vector3(district.anchor.x, walkwayHeight, district.anchor.z);

            var districtRect = new Rect(-district.bounds.x / 2f, -district.bounds.y / 2f,
                district.bounds.x, district.bounds.y);
            int seed = worldSeed * 31 + district.id.GetHashCode();
            var lots = LotPartitioner.Partition(districtRect, style.streetWidth, style.minLotArea,
                style.maxLotAspect, seed);

            int built = 0;
            foreach (var lot in lots)
            {
                var plan = BuildingGrammar.Plan(lot, style.ToData(), seed + ++built);
                if (plan == null) continue;
                RenderPlan(root.transform, lot, plan, style);
            }
        }

        private static void RenderPlan(Transform districtRoot, Lot lot, BuildingPlan plan, BuildingStyleDefinition style)
        {
            var bRoot = new GameObject("Building_" + lot.Bounds.x.ToString("F0") + "_" + lot.Bounds.y.ToString("F0"));
            bRoot.transform.SetParent(districtRoot, false);

            var cells = new Vector2Int(0, 0);
            foreach (var m in plan.Modules)
            {
                float y = m.Storey * plan.StoreyHeight;
                var pos = new Vector3(m.LocalPos.x, y, m.LocalPos.y);
                switch (m.Module)
                {
                    case BuildingModule.WallSolid:
                        Wall(bRoot.transform, plan, style, m, pos, style.wallColor, "Wall", out _);
                        break;
                    case BuildingModule.WallWindow:
                        var wall = Wall(bRoot.transform, plan, style, m, pos, style.wallColor, "WallW", out bool fromKit);
                        if (!fromKit) Inset(wall.transform, plan, style.windowColor); // kit modules own their pane
                        break;
                    case BuildingModule.Doorway:
                        Doorway(bRoot.transform, plan, style, m, pos);
                        break;
                    case BuildingModule.CornerTrim:
                        var trim = Cube(bRoot.transform, "Trim", pos + Vector3.up * (plan.StoreyHeight * 0.5f),
                            new Vector3(0.35f, plan.StoreyHeight, 0.35f), style.trimColor);
                        trim.transform.localRotation = Quaternion.Euler(0f, m.Rotation, 0f);
                        break;
                    case BuildingModule.FloorSlab:
                        cells = CellsFromPlan(plan);
                        Cube(bRoot.transform, "Floor", pos + Vector3.up * 0.075f,
                            new Vector3(cells.x * plan.ModuleWidth, 0.15f, cells.y * plan.ModuleWidth),
                            style.trimColor);
                        break;
                    case BuildingModule.RoofFlat:
                        cells = CellsFromPlan(plan);
                        Cube(bRoot.transform, "Roof", pos + Vector3.up * 0.15f,
                            new Vector3(cells.x * plan.ModuleWidth + 0.6f, 0.3f, cells.y * plan.ModuleWidth + 0.6f),
                            style.roofColor);
                        break;
                    case BuildingModule.RoofRaked:
                        cells = CellsFromPlan(plan);
                        var rake = Cube(bRoot.transform, "RoofRaked", pos + Vector3.up * 0.5f,
                            new Vector3(cells.x * plan.ModuleWidth + 0.6f, 0.3f, cells.y * plan.ModuleWidth * 1.06f),
                            style.roofColor);
                        rake.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);
                        break;
                }
            }
        }

        // Styles seen falling back to primitive walls this domain — warn ONCE per id, not per wall.
        private static readonly System.Collections.Generic.HashSet<string> _unfulfilledWarned =
            new System.Collections.Generic.HashSet<string>();

        private static GameObject Wall(Transform parent, BuildingPlan plan, BuildingStyleDefinition style,
            ModulePlacement m, Vector3 pos, Color color, string name, out bool fromKit)
        {
            string regId = "buildingModule:" + style.styleId + "/" + m.Module;
            if (Ziptide.Editor.Art.ArtModuleRegistry.TryBuild(regId, out var kit))
            {
                fromKit = true;
                kit.transform.SetParent(parent, false);
                kit.transform.localPosition = pos + Vector3.up * (plan.StoreyHeight * 0.5f);
                kit.transform.localRotation = Quaternion.Euler(0f, m.Rotation, 0f);
                return kit;
            }
            // KIT_UNFULFILLED (HARDWIRING 0.2): this style ships flat primitive walls. Warn-once so
            // an unfulfilled style is visible in every build log without drowning it.
            fromKit = false;
            if (_unfulfilledWarned.Add(regId))
                Debug.LogWarning("[Ziptide] KIT_UNFULFILLED id=" + regId + " (primitive fallback walls)");
            var wall = Cube(parent, name, pos + Vector3.up * (plan.StoreyHeight * 0.5f),
                new Vector3(plan.ModuleWidth, plan.StoreyHeight, 0.25f), color);
            wall.transform.localRotation = Quaternion.Euler(0f, m.Rotation, 0f);
            return wall;
        }

        /// <summary>Doorway = two jambs + lintel, a REAL 1.1m gap you can walk through, plus the
        /// outward-facing __DOOR marker the audit gate raycasts from.</summary>
        private static void Doorway(Transform parent, BuildingPlan plan, BuildingStyleDefinition style,
            ModulePlacement m, Vector3 pos)
        {
            float w = plan.ModuleWidth, h = plan.StoreyHeight;
            float jambW = (w - 1.1f) * 0.5f;
            var rot = Quaternion.Euler(0f, m.Rotation, 0f);

            var frame = new GameObject("DoorFrame");
            frame.transform.SetParent(parent, false);
            frame.transform.localPosition = pos;
            frame.transform.localRotation = rot;

            Cube(frame.transform, "JambL", new Vector3(-(1.1f + jambW) * 0.5f, h * 0.5f, 0f),
                new Vector3(jambW, h, 0.25f), style.wallColor);
            Cube(frame.transform, "JambR", new Vector3((1.1f + jambW) * 0.5f, h * 0.5f, 0f),
                new Vector3(jambW, h, 0.25f), style.wallColor);
            Cube(frame.transform, "Lintel", new Vector3(0f, (h + 2.1f) * 0.5f, 0f),
                new Vector3(1.1f, h - 2.1f, 0.25f), style.doorColor);

            var marker = new GameObject("__DOOR");
            marker.transform.SetParent(frame.transform, false);
            marker.transform.localPosition = new Vector3(0f, 1.0f, 0.3f); // just outside, chest height
            marker.transform.localRotation = Quaternion.identity;         // +Z = outward (frame already rotated)
        }

        private static void Inset(Transform wall, BuildingPlan plan, Color paneColor)
        {
            var pane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pane.name = "Pane";
            var col = pane.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            pane.transform.SetParent(wall, false);
            pane.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            pane.transform.localScale = new Vector3(0.55f, 0.45f, 1.1f); // pokes through both wall faces
            ItemFactory.ApplyURPColor(pane, paneColor);
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPos, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private static Vector2Int CellsFromPlan(BuildingPlan plan)
        {
            // Recover cell counts from the wall accounting (face 0 cells × face 1 cells).
            int x = 0, z = 0;
            foreach (var m in plan.Modules)
            {
                if (m.Storey != 0) continue;
                if (m.Face == 0 && m.CellIndex >= 0 && IsWall(m.Module)) x = Mathf.Max(x, m.CellIndex + 1);
                if (m.Face == 1 && m.CellIndex >= 0 && IsWall(m.Module)) z = Mathf.Max(z, m.CellIndex + 1);
            }
            return new Vector2Int(Mathf.Max(1, x), Mathf.Max(1, z));
        }

        private static bool IsWall(BuildingModule m) =>
            m == BuildingModule.WallSolid || m == BuildingModule.WallWindow || m == BuildingModule.Doorway;
    }
}
#endif
