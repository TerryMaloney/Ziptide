#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// HARDWIRING 1.3e / BUILDING_INTERIORS Tier B — the furnish TRANSLATOR: turns
    /// <see cref="RoomFurnishCore"/>'s placed items into multi-part primitive furniture (LAW 6 —
    /// a cot is a frame + mattress + pillow, never one box). Creates a `Room_<i>` parent per room
    /// so InteriorCullRuntime can portal-cull room contents; each solid item carries ONE root
    /// BoxCollider (children stripped — Quest physics cost lives in collider count). Deterministic:
    /// everything derives from the furnish plan. Called by InteriorBuilder at patch time.
    /// </summary>
    public static class InteriorFurnisher
    {
        private const float FloorY = 0.15f; // interior floor plane (the walls' base)

        private static readonly Color Wood = new Color(0.45f, 0.33f, 0.22f);
        private static readonly Color WoodDark = new Color(0.32f, 0.23f, 0.15f);
        private static readonly Color Metal = new Color(0.42f, 0.45f, 0.48f);
        private static readonly Color MetalDark = new Color(0.28f, 0.30f, 0.32f);
        private static readonly Color Cloth = new Color(0.55f, 0.52f, 0.44f);
        private static readonly Color Warm = new Color(1.0f, 0.87f, 0.62f);

        /// <summary>Furnish every room under <paramref name="root"/>. Returns the per-room parent
        /// transforms (index-aligned with plan.Rooms) so the caller can group its own per-room
        /// content (lights) for portal culling.</summary>
        public static Transform[] Furnish(Transform root, InteriorPlan plan, Vector2 entry,
            BuildingStyleDefinition style, int seed)
        {
            var furnish = RoomFurnishCore.Furnish(plan, entry, seed);
            var roomParents = new Transform[plan.Rooms.Count];
            for (int i = 0; i < plan.Rooms.Count; i++)
            {
                var node = new GameObject("Room_" + i);
                node.transform.SetParent(root, false);
                roomParents[i] = node.transform;
            }

            foreach (var room in furnish)
                foreach (var item in room.Items)
                {
                    var go = new GameObject(item.Kind);
                    go.transform.SetParent(roomParents[room.RoomIndex], false);
                    go.transform.localPosition = new Vector3(item.Pos.x, FloorY, item.Pos.y);
                    go.transform.localRotation = Quaternion.Euler(0f, item.RotationDeg, 0f);
                    float height = BuildKind(go.transform, item, style);
                    if (!item.Flat && height > 0.03f)
                    {
                        var box = go.AddComponent<BoxCollider>();
                        box.center = new Vector3(0f, height * 0.5f, 0f);
                        box.size = new Vector3(item.Size.x, height, item.Size.y);
                    }
                }
            return roomParents;
        }

        /// <summary>Build the parts for one catalog kind (local: X = width, Z = depth, faces +Z).
        /// Returns the item's collider height.</summary>
        private static float BuildKind(Transform t, FurnishItem item, BuildingStyleDefinition style)
        {
            float w = item.Size.x, d = item.Size.y;
            switch (item.Kind)
            {
                case "bench":
                    Part(t, "Seat", new Vector3(0f, 0.42f, 0f), new Vector3(w, 0.06f, d), Wood);
                    Part(t, "LegL", new Vector3(-w * 0.5f + 0.08f, 0.21f, 0f), new Vector3(0.08f, 0.42f, d * 0.8f), WoodDark);
                    Part(t, "LegR", new Vector3(w * 0.5f - 0.08f, 0.21f, 0f), new Vector3(0.08f, 0.42f, d * 0.8f), WoodDark);
                    return 0.46f;

                case "coat_rack":
                    Part(t, "Base", new Vector3(0f, 0.02f, 0f), new Vector3(0.35f, 0.02f, 0.35f), MetalDark, PrimitiveType.Cylinder);
                    Part(t, "Pole", new Vector3(0f, 0.8f, 0f), new Vector3(0.05f, 0.8f, 0.05f), Metal, PrimitiveType.Cylinder);
                    for (int i = 0; i < 3; i++)
                    {
                        float a = i * 120f * Mathf.Deg2Rad;
                        Part(t, "Peg" + i, new Vector3(Mathf.Cos(a) * 0.1f, 1.5f - i * 0.06f, Mathf.Sin(a) * 0.1f),
                            new Vector3(0.16f, 0.03f, 0.03f), WoodDark);
                    }
                    return 1.6f;

                case "sign_board":
                    Part(t, "PostL", new Vector3(-w * 0.35f, 0.65f, 0f), new Vector3(0.05f, 1.3f, 0.05f), WoodDark);
                    Part(t, "PostR", new Vector3(w * 0.35f, 0.65f, 0f), new Vector3(0.05f, 1.3f, 0.05f), WoodDark);
                    Part(t, "Board", new Vector3(0f, 1.3f, 0f), new Vector3(w, 0.5f, 0.05f), style.trimColor);
                    Part(t, "Text", new Vector3(0f, 1.32f, 0.03f), new Vector3(w * 0.7f, 0.28f, 0.012f), Cloth);
                    return 1.55f;

                case "table":
                    Part(t, "Top", new Vector3(0f, 0.72f, 0f), new Vector3(w, 0.05f, d), Wood);
                    for (int i = 0; i < 4; i++)
                        Part(t, "Leg" + i, new Vector3(
                                (i % 2 == 0 ? -1f : 1f) * (w * 0.5f - 0.08f), 0.36f,
                                (i < 2 ? -1f : 1f) * (d * 0.5f - 0.08f)),
                            new Vector3(0.07f, 0.72f, 0.07f), WoodDark);
                    return 0.75f;

                case "stool":
                    Part(t, "Seat", new Vector3(0f, 0.45f, 0f), new Vector3(0.35f, 0.025f, 0.35f), Wood, PrimitiveType.Cylinder);
                    Part(t, "Leg", new Vector3(0f, 0.22f, 0f), new Vector3(0.07f, 0.22f, 0.07f), WoodDark, PrimitiveType.Cylinder);
                    return 0.5f;

                case "shelf":
                    Part(t, "UpL", new Vector3(-w * 0.5f + 0.03f, 0.9f, 0f), new Vector3(0.05f, 1.8f, d), WoodDark);
                    Part(t, "UpR", new Vector3(w * 0.5f - 0.03f, 0.9f, 0f), new Vector3(0.05f, 1.8f, d), WoodDark);
                    for (int i = 0; i < 3; i++)
                    {
                        float y = 0.4f + i * 0.5f;
                        Part(t, "Board" + i, new Vector3(0f, y, 0f), new Vector3(w - 0.08f, 0.04f, d), Wood);
                        Part(t, "Clutter" + i, new Vector3((i - 1) * w * 0.2f, y + 0.1f, 0f),
                            new Vector3(0.18f + i * 0.05f, 0.16f, d * 0.6f), TintFor(i));
                    }
                    return 1.8f;

                case "lamp":
                    Part(t, "Base", new Vector3(0f, 0.02f, 0f), new Vector3(0.28f, 0.02f, 0.28f), MetalDark, PrimitiveType.Cylinder);
                    Part(t, "Stem", new Vector3(0f, 0.65f, 0f), new Vector3(0.035f, 0.63f, 0.035f), Metal, PrimitiveType.Cylinder);
                    Part(t, "Shade", new Vector3(0f, 1.35f, 0f), new Vector3(0.22f, 0.18f, 0.22f), Warm);
                    return 1.5f;

                case "cot":
                    Part(t, "Frame", new Vector3(0f, 0.22f, 0f), new Vector3(w, 0.16f, d), WoodDark);
                    Part(t, "Mattress", new Vector3(0f, 0.35f, 0f), new Vector3(w - 0.1f, 0.1f, d - 0.1f), Cloth);
                    Part(t, "Pillow", new Vector3(-w * 0.5f + 0.28f, 0.43f, 0f), new Vector3(0.38f, 0.08f, d - 0.3f), Warm * 0.9f);
                    return 0.48f;

                case "footlocker":
                    Part(t, "Box", new Vector3(0f, 0.2f, 0f), new Vector3(w, 0.38f, d), MetalDark);
                    Part(t, "Lid", new Vector3(0f, 0.41f, 0f), new Vector3(w + 0.02f, 0.05f, d + 0.02f), Metal);
                    Part(t, "Latch", new Vector3(0f, 0.3f, d * 0.5f + 0.005f), new Vector3(0.08f, 0.1f, 0.02f), Warm * 0.7f);
                    return 0.45f;

                case "side_table":
                    Part(t, "Top", new Vector3(0f, 0.55f, 0f), new Vector3(w, 0.04f, d), Wood);
                    Part(t, "Leg", new Vector3(0f, 0.27f, 0f), new Vector3(0.06f, 0.27f, 0.06f), WoodDark, PrimitiveType.Cylinder);
                    return 0.58f;

                case "workbench":
                    Part(t, "Top", new Vector3(0f, 0.82f, 0f), new Vector3(w, 0.08f, d), WoodDark);
                    Part(t, "LegL", new Vector3(-w * 0.5f + 0.1f, 0.4f, 0f), new Vector3(0.12f, 0.8f, d * 0.85f), MetalDark);
                    Part(t, "LegR", new Vector3(w * 0.5f - 0.1f, 0.4f, 0f), new Vector3(0.12f, 0.8f, d * 0.85f), MetalDark);
                    Part(t, "Vise", new Vector3(w * 0.5f - 0.2f, 0.92f, d * 0.25f), new Vector3(0.16f, 0.14f, 0.12f), Metal);
                    Part(t, "Tool0", new Vector3(-w * 0.2f, 0.89f, 0f), new Vector3(0.3f, 0.04f, 0.08f), TintFor(0));
                    Part(t, "Tool1", new Vector3(0.05f, 0.89f, -d * 0.2f), new Vector3(0.12f, 0.05f, 0.2f), TintFor(2));
                    return 0.95f;

                case "tool_rack":
                    Part(t, "Panel", new Vector3(0f, 1.15f, -d * 0.5f + 0.03f), new Vector3(w, 1.0f, 0.05f), WoodDark);
                    for (int i = 0; i < 4; i++)
                        Part(t, "Tool" + i, new Vector3((i - 1.5f) * w * 0.22f, 1.05f - (i % 2) * 0.12f, -d * 0.5f + 0.09f),
                            new Vector3(0.05f, 0.34f + (i % 3) * 0.1f, 0.05f), i % 2 == 0 ? Metal : TintFor(i));
                    return 1.7f;

                case "parts_bin":
                    Part(t, "Bin", new Vector3(0f, 0.18f, 0f), new Vector3(w, 0.34f, d), MetalDark);
                    Part(t, "Rim", new Vector3(0f, 0.36f, 0f), new Vector3(w + 0.03f, 0.03f, d + 0.03f), Metal);
                    for (int i = 0; i < 3; i++)
                        Part(t, "Bit" + i, new Vector3((i - 1) * w * 0.22f, 0.4f, (i % 2 == 0 ? 1 : -1) * d * 0.15f),
                            new Vector3(0.09f, 0.07f, 0.09f), TintFor(i + 1));
                    return 0.42f;

                case "crate":
                    Part(t, "Box", new Vector3(0f, 0.25f, 0f), new Vector3(w, 0.5f, d), Wood);
                    Part(t, "LidTrim", new Vector3(0f, 0.51f, 0f), new Vector3(w + 0.02f, 0.03f, d + 0.02f), WoodDark);
                    Part(t, "Stencil", new Vector3(0f, 0.3f, d * 0.5f + 0.005f), new Vector3(w * 0.5f, 0.18f, 0.01f), style.trimColor);
                    return 0.53f;

                case "barrel":
                    Part(t, "Body", new Vector3(0f, 0.45f, 0f), new Vector3(w, 0.45f, d), WoodDark, PrimitiveType.Cylinder);
                    Part(t, "BandLo", new Vector3(0f, 0.2f, 0f), new Vector3(w + 0.02f, 0.02f, d + 0.02f), Metal, PrimitiveType.Cylinder);
                    Part(t, "BandHi", new Vector3(0f, 0.7f, 0f), new Vector3(w + 0.02f, 0.02f, d + 0.02f), Metal, PrimitiveType.Cylinder);
                    return 0.9f;

                case "floor_drain":
                    Part(t, "Plate", new Vector3(0f, 0.01f, 0f), new Vector3(w, 0.01f, d), MetalDark, PrimitiveType.Cylinder);
                    Part(t, "BarA", new Vector3(0f, 0.021f, -0.05f), new Vector3(w * 0.8f, 0.005f, 0.03f), Metal);
                    Part(t, "BarB", new Vector3(0f, 0.021f, 0.05f), new Vector3(w * 0.8f, 0.005f, 0.03f), Metal);
                    return 0.02f;

                default: // unknown kind: an honest crate, never an invisible nothing
                    Part(t, "Box", new Vector3(0f, 0.25f, 0f), new Vector3(w, 0.5f, d), Wood);
                    return 0.5f;
            }
        }

        private static Color TintFor(int i)
        {
            switch (i % 4)
            {
                case 0: return new Color(0.62f, 0.42f, 0.28f);
                case 1: return new Color(0.36f, 0.48f, 0.42f);
                case 2: return new Color(0.5f, 0.38f, 0.45f);
                default: return new Color(0.55f, 0.5f, 0.32f);
            }
        }

        private static void Part(Transform parent, string name, Vector3 pos, Vector3 scale,
            Color color, PrimitiveType type = PrimitiveType.Cube)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col); // one root BoxCollider per item instead
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
