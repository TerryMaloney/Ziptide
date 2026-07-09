#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// HARDWIRING 1.3 / BUILDING_INTERIORS Tier B — walkable ground-floor interiors. Consumes what
    /// already exists: RoomPartitioner (connected rooms+corridors, tested), InteriorMeshCore (walls
    /// from the plan + the doorway entry-carve, tested), and the shell BuildingBuilder just built
    /// (its per-storey FloorSlab is the interior's floor AND ceiling — no new slabs). This file only
    /// translates rects into cubes: interior wall boxes (with colliders — players lean on them) and
    /// one warm unlit light panel per room so rooms read as LIT from inside.
    ///
    /// Deterministic per building seed. Opt-in per style (BuildingStyleDefinition.hasInteriors);
    /// the district renderer-budget gate audits the added cost on the next APK.
    /// </summary>
    public static class InteriorBuilder
    {
        private const float CorridorWidth = 1.2f;
        private const float MinRoomArea = 8f;
        private const float MaxAspect = 2.5f;
        /// <summary>Skip interiors in footprints smaller than this — a kiosk isn't a house.</summary>
        private const float MinFootprintArea = 30f;

        /// <summary>Build the ground-floor interior inside <paramref name="bRoot"/>. The footprint
        /// and doorway are derived from the plan's storey-0 modules (local space).</summary>
        public static void Build(Transform bRoot, BuildingPlan plan, BuildingStyleDefinition style, int seed)
        {
            // Footprint from the storey-0 wall ring: walls sit ON the boundary lines.
            bool any = false;
            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
            Vector2 doorPos = Vector2.zero;
            float doorRot = 0f;
            bool hasDoor = false;

            foreach (var m in plan.Modules)
            {
                if (m.Storey != 0) continue;
                bool isWall = m.Module == BuildingModule.WallSolid || m.Module == BuildingModule.WallWindow
                    || m.Module == BuildingModule.Doorway;
                if (!isWall) continue;
                any = true;
                if (m.LocalPos.x < minX) minX = m.LocalPos.x;
                if (m.LocalPos.x > maxX) maxX = m.LocalPos.x;
                if (m.LocalPos.y < minZ) minZ = m.LocalPos.y;
                if (m.LocalPos.y > maxZ) maxZ = m.LocalPos.y;
                if (m.Module == BuildingModule.Doorway)
                {
                    doorPos = m.LocalPos;
                    doorRot = m.Rotation;
                    hasDoor = true;
                }
            }
            if (!any || !hasDoor) return;

            // Interior rect: the wall ring inset by roughly half a module (walls occupy the lines)
            // plus the shell depth, so interior mass never fuses with the facade.
            float inset = plan.ModuleWidth * 0.5f + 0.3f;
            var interior = new Rect(minX + inset, minZ + inset,
                (maxX - minX) - 2f * inset, (maxZ - minZ) - 2f * inset);
            if (interior.width < RoomPartitioner.MinSide || interior.height < RoomPartitioner.MinSide) return;
            if (interior.width * interior.height < MinFootprintArea) return;

            // The doorway's INNER face: step inward against the door's outward rotation.
            Vector3 inward3 = Quaternion.Euler(0f, doorRot, 0f) * new Vector3(0f, 0f, -1f);
            var entry = new Vector2(doorPos.x + inward3.x * (inset + 0.2f),
                                    doorPos.y + inward3.z * (inset + 0.2f));
            entry.x = Mathf.Clamp(entry.x, interior.xMin + 0.1f, interior.xMax - 0.1f);
            entry.y = Mathf.Clamp(entry.y, interior.yMin + 0.1f, interior.yMax - 0.1f);

            var roomPlan = RoomPartitioner.Partition(interior, CorridorWidth, MinRoomArea, MaxAspect, seed);
            if (roomPlan.IsEmpty) return;
            InteriorMeshCore.WithEntry(roomPlan, entry, CorridorWidth * 0.9f);

            var root = new GameObject("Interior");
            root.transform.SetParent(bRoot, false);
            // 1.3d: interiors only render when the player is near (arms itself at runtime).
            root.AddComponent<InteriorCullRuntime>();

            // Interior walls: full storey height, colliding, trim-dark so they read as structure.
            Color wallCol = style.trimColor * 0.92f;
            float h = plan.StoreyHeight - 0.3f; // clear of the storey-1 floor slab
            foreach (var w in InteriorMeshCore.RasterizeWalls(interior, roomPlan))
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "IWall";
                cube.transform.SetParent(root.transform, false);
                cube.transform.localPosition = new Vector3(w.center.x, h * 0.5f + 0.15f, w.center.y);
                cube.transform.localScale = new Vector3(w.width, h, w.height);
                ItemFactory.ApplyURPColor(cube, wallCol);
                var r = cube.GetComponent<Renderer>();
                if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            // One warm light panel per room, up at the ceiling — rooms read LIT from inside and
            // through the doorway (pairs with the exterior interior-mapped windows).
            var lightCol = new Color(1.0f, 0.87f, 0.62f);
            foreach (var room in roomPlan.Rooms)
            {
                var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                panel.name = "RoomLight";
                Object.DestroyImmediate(panel.GetComponent<Collider>());
                panel.transform.SetParent(root.transform, false);
                panel.transform.localPosition = new Vector3(room.center.x, h - 0.12f, room.center.y);
                panel.transform.localScale = new Vector3(
                    Mathf.Min(1.2f, room.width * 0.4f), 0.06f, Mathf.Min(1.2f, room.height * 0.4f));
                ItemFactory.ApplyURPColor(panel, lightCol);
                var r = panel.GetComponent<Renderer>();
                if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            // Interior POIs (1.3c): salvage caches make rooms WORTH entering. Deterministic xorshift
            // (the RoomPartitioner recipe) — same building, same loot. Cap 2 per interior, distinct
            // rooms, skipping room 0 (usually nearest the entry — loot rewards going deeper).
            uint rng = seed == 0 ? 2463534242u : (uint)seed;
            int caches = 0;
            for (int i = 1; i < roomPlan.Rooms.Count && caches < 2; i++)
            {
                rng ^= rng << 13; rng ^= rng >> 17; rng ^= rng << 5;
                if ((rng & 0xFFFFFF) / (float)0x1000000 > 0.4f) continue;

                var room = roomPlan.Rooms[i];
                var cacheGo = new GameObject("SalvageCache");
                cacheGo.transform.SetParent(root.transform, false);
                cacheGo.transform.localPosition = new Vector3(room.center.x, 0.15f, room.center.y);
                rng ^= rng << 13; rng ^= rng >> 17; rng ^= rng << 5;
                double pay = 4 + System.Math.Floor(((rng & 0xFFFFFF) / (double)0x1000000) * 7); // 4..10
                cacheGo.AddComponent<SalvageCacheRuntime>().Init("scrap", pay);
                caches++;
            }

            Debug.Log("[Ziptide] INTERIOR_BUILT rooms=" + roomPlan.Rooms.Count +
                      " corridors=" + roomPlan.Corridors.Count + " caches=" + caches);
        }
    }
}
#endif
