#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// SHARED geometry core for data-driven city worlds. Consumes a <see cref="CityLayoutDefinition"/>
    /// and builds districts, streets/walkways, canals, facades, hero interiors, the shipyard, skyline +
    /// fog, and drone spawns under a single root. This is the "World Build Kit": every city world's
    /// patcher is a thin shell that calls <see cref="Build"/>, so cloning a world = author a new layout
    /// asset + a ~40-line patcher. No hand-edited scene YAML; the build self-generates the scene.
    /// </summary>
    public static class CityBuilder
    {
        // Mechanical constants the patcher owns (never authored in the kit).
        private const float SlabThickness = 1f;
        private const float RailHeight = 1.1f;
        private const float RailThickness = 0.08f;
        private const float TierStep = 3f;          // elevated walkway lift over canals
        private const float WindowInset = 0.06f;

        private static readonly Dictionary<Color, Material> _matCache = new Dictionary<Color, Material>();

        /// <summary>Build the whole city under <paramref name="root"/>. Deterministic given kit.seed
        /// (caller should Random.InitState(kit.seed) first).</summary>
        public static void Build(Transform root, CityLayoutDefinition kit)
        {
            if (root == null || kit == null) return;
            _matCache.Clear();

            BuildSkylineAndFog(root, kit);
            BuildCanals(root, kit);

            foreach (var d in kit.districts)
                if (d != null) BuildDistrict(root, kit, d);

            foreach (var c in kit.connections)
                if (c != null) BuildConnection(root, kit, c);

            BuildShipyard(root, kit);
            BuildDroneZones(root, kit);
            BuildHazardZones(root, kit);

            _matCache.Clear();
        }

        // ── Materials (shared per color for perf) ────────────────────────────
        private static Material Mat(Color c)
        {
            if (_matCache.TryGetValue(c, out var m) && m != null) return m;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            m = new Material(shader) { name = "CityMat_" + ColorUtility.ToHtmlStringRGB(c) };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            _matCache[c] = m;
            return m;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                if (collider) col.enabled = true;
                else Object.DestroyImmediate(col);
            }
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = Mat(color);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        private static GlobalPalette Pal(CityLayoutDefinition kit, DistrictDef d)
            => (d != null && d.useOverride && d.paletteOverride != null) ? d.paletteOverride : kit.palette;

        // ── Skyline + fog ─────────────────────────────────────────────────────
        private static void BuildSkylineAndFog(Transform root, CityLayoutDefinition kit)
        {
            RenderSettings.fog = kit.fogEnabled;
            if (kit.fogEnabled)
            {
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogColor = kit.fogColor;
                RenderSettings.fogDensity = kit.fogDensity;
            }

            var skyRoot = NewChild(root, "Skyline");
            for (int i = 0; i < kit.skylineCount; i++)
            {
                float a = (i / (float)Mathf.Max(1, kit.skylineCount)) * Mathf.PI * 2f;
                float r = kit.skylineRingRadius * (0.85f + Random.value * 0.3f);
                float h = Mathf.Lerp(kit.skylineMinHeight, kit.skylineMaxHeight, Random.value);
                float w = 6f + Random.value * 10f;
                var pos = new Vector3(Mathf.Cos(a) * r, kit.walkwayHeight + h * 0.5f - SlabThickness, Mathf.Sin(a) * r);
                Cube(skyRoot, "Silhouette_" + i, pos, new Vector3(w, h, w), kit.palette.skyline, false);
            }
        }

        // ── Canals (decorative; collider stripped) ───────────────────────────
        private static void BuildCanals(Transform root, CityLayoutDefinition kit)
        {
            if (kit.canals == null || kit.canals.Count == 0) return;
            var canalRoot = NewChild(root, "Canals");
            for (int i = 0; i < kit.canals.Count; i++)
            {
                var c = kit.canals[i];
                if (c == null) continue;
                Color col = c.useOverride ? c.colorOverride : kit.palette.toxic;
                var pos = new Vector3(c.center.x, kit.walkwayHeight - c.depth, c.center.z);
                // NO collider — you can never spawn-snap onto goo; the fall-net catches drops.
                Cube(canalRoot, "Canal_" + i, pos, new Vector3(c.size.x, 0.4f, c.size.y), col, false);
            }
        }

        // ── Districts ─────────────────────────────────────────────────────────
        private static void BuildDistrict(Transform root, CityLayoutDefinition kit, DistrictDef d)
        {
            var districtRoot = NewChild(root, "District_" + d.id);
            districtRoot.localPosition = d.anchor;
            var pal = Pal(kit, d);

            // Walkable ground slab — TOP at walkwayHeight, collider ON.
            Cube(districtRoot, d.id + "_Ground",
                new Vector3(0f, kit.walkwayHeight - SlabThickness * 0.5f, 0f),
                new Vector3(d.bounds.x, SlabThickness, d.bounds.y), pal.concrete, true);

            BuildFacadeRing(districtRoot, kit, d, pal);

            if (d.landmarks != null)
                foreach (var lm in d.landmarks)
                    if (lm != null)
                        Cube(districtRoot, "Landmark_" + lm.name,
                            new Vector3(lm.localPos.x, kit.walkwayHeight + lm.height * 0.5f, lm.localPos.z),
                            new Vector3(lm.width, lm.height, lm.width), pal.building2, true);

            if (d.heroBuildings != null)
                foreach (var hb in d.heroBuildings)
                    if (hb != null) BuildHeroBuilding(districtRoot, kit, d, hb, pal);

            if (d.props != null)
                foreach (var p in d.props)
                    if (p != null) ScatterProps(districtRoot, kit, p, pal);
        }

        // Facades line the district edges with GAPS (walkable streets pass between them).
        private static void BuildFacadeRing(Transform districtRoot, CityLayoutDefinition kit, DistrictDef d, GlobalPalette pal)
        {
            float halfX = d.bounds.x * 0.5f;
            float halfZ = d.bounds.y * 0.5f;
            float tierH = 4f + Mathf.Clamp(d.heightTier, 0, 3) * 3f;
            float stride = 7f;

            // Two edges along X (north/south), two along Z (east/west). Stride leaves gaps to walk through.
            EdgeFacades(districtRoot, kit, pal, tierH, -halfX + 3f, halfX - 3f, stride, isXEdge: true, edgeOffset: halfZ - 2f);
            EdgeFacades(districtRoot, kit, pal, tierH, -halfX + 3f, halfX - 3f, stride, isXEdge: true, edgeOffset: -(halfZ - 2f));
            EdgeFacades(districtRoot, kit, pal, tierH, -halfZ + 3f, halfZ - 3f, stride, isXEdge: false, edgeOffset: halfX - 2f);
            EdgeFacades(districtRoot, kit, pal, tierH, -halfZ + 3f, halfZ - 3f, stride, isXEdge: false, edgeOffset: -(halfX - 2f));
        }

        private static void EdgeFacades(Transform districtRoot, CityLayoutDefinition kit, GlobalPalette pal,
            float baseHeight, float from, float to, float stride, bool isXEdge, float edgeOffset)
        {
            int idx = 0;
            for (float t = from; t <= to; t += stride)
            {
                if (Random.value < 0.25f) continue; // gaps = walkable streets between buildings
                float h = baseHeight * (0.7f + Random.value * 0.8f);
                float w = 3.5f + Random.value * 2f;
                Color c = (idx % 2 == 0) ? pal.building1 : pal.building2;
                Vector3 pos = isXEdge
                    ? new Vector3(t, kit.walkwayHeight + h * 0.5f, edgeOffset)
                    : new Vector3(edgeOffset, kit.walkwayHeight + h * 0.5f, t);
                var b = Cube(districtRoot, "Facade_" + (isXEdge ? "X" : "Z") + "_" + idx,
                    pos, new Vector3(isXEdge ? w : 3f, h, isXEdge ? 3f : w), c, true);
                AddFakeDepthWindows(b, pal, isXEdge);
                idx++;
            }
        }

        // Inset dark emissive-ish cubes give cheap fake interior depth on facades.
        private static void AddFakeDepthWindows(GameObject building, GlobalPalette pal, bool isXEdge)
        {
            Vector3 s = building.transform.localScale;
            int cols = Mathf.Max(1, Mathf.FloorToInt((isXEdge ? s.x : s.z) / 1.4f));
            int rows = Mathf.Max(1, Mathf.FloorToInt(s.y / 1.6f));
            float faceZ = isXEdge ? (-(s.z * 0.5f) - WindowInset) : 0f;
            float faceX = isXEdge ? 0f : (-(s.x * 0.5f) - WindowInset);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (Random.value < 0.35f) continue;
                    float u = ((c + 0.5f) / cols - 0.5f) * (isXEdge ? s.x : s.z) * 0.9f;
                    float vy = ((r + 0.5f) / rows - 0.5f) * s.y * 0.85f;
                    // Window is a child; convert to LOCAL scale (parent is scaled, so divide).
                    var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    w.name = "Win";
                    w.transform.SetParent(building.transform, false);
                    var col = w.GetComponent<Collider>();
                    if (col != null) Object.DestroyImmediate(col);
                    Vector3 worldWin = isXEdge ? new Vector3(0.9f, 0.7f, 0.12f) : new Vector3(0.12f, 0.7f, 0.9f);
                    w.transform.localScale = new Vector3(worldWin.x / s.x, worldWin.y / s.y, worldWin.z / s.z);
                    w.transform.localPosition = isXEdge
                        ? new Vector3(u / s.x, vy / s.y, faceZ / s.z)
                        : new Vector3(faceX / s.x, vy / s.y, u / s.z);
                    var rend = w.GetComponent<Renderer>();
                    if (rend != null) { rend.sharedMaterial = Mat(pal.facadeWindow); rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; }
                }
            }
        }

        // ── Hero (enterable) buildings ───────────────────────────────────────
        private static void BuildHeroBuilding(Transform districtRoot, CityLayoutDefinition kit, DistrictDef d, HeroBuildingDef hb, GlobalPalette pal)
        {
            var hbRoot = NewChild(districtRoot, "Hero_" + hb.id);
            hbRoot.localPosition = new Vector3(hb.localPos.x, kit.walkwayHeight, hb.localPos.z);

            float w = hb.footprint.x, depth = hb.footprint.y, h = hb.height, wall = 0.3f;
            Color shell = pal.building1;

            // Floor + ceiling.
            Cube(hbRoot, hb.id + "_Floor", new Vector3(0f, 0.05f, 0f), new Vector3(w, 0.1f, depth), pal.concrete, true);
            Cube(hbRoot, hb.id + "_Ceiling", new Vector3(0f, h, 0f), new Vector3(w, wall, depth), shell, true);

            // Four walls, with a door GAP on the wall nearest doorLocalPos.
            bool doorOnZNeg = Mathf.Abs(hb.doorLocalPos.z) >= Mathf.Abs(hb.doorLocalPos.x) && hb.doorLocalPos.z < 0f;
            bool doorOnZPos = Mathf.Abs(hb.doorLocalPos.z) >= Mathf.Abs(hb.doorLocalPos.x) && hb.doorLocalPos.z > 0f;
            bool doorOnXNeg = Mathf.Abs(hb.doorLocalPos.x) > Mathf.Abs(hb.doorLocalPos.z) && hb.doorLocalPos.x < 0f;
            bool doorOnXPos = Mathf.Abs(hb.doorLocalPos.x) > Mathf.Abs(hb.doorLocalPos.z) && hb.doorLocalPos.x > 0f;

            BuildWallWithMaybeGap(hbRoot, hb.id + "_Wall_Zn", new Vector3(0f, h * 0.5f, -depth * 0.5f), new Vector3(w, h, wall), shell, doorOnZNeg, true);
            BuildWallWithMaybeGap(hbRoot, hb.id + "_Wall_Zp", new Vector3(0f, h * 0.5f, depth * 0.5f), new Vector3(w, h, wall), shell, doorOnZPos, true);
            BuildWallWithMaybeGap(hbRoot, hb.id + "_Wall_Xn", new Vector3(-w * 0.5f, h * 0.5f, 0f), new Vector3(wall, h, depth), shell, doorOnXNeg, false);
            BuildWallWithMaybeGap(hbRoot, hb.id + "_Wall_Xp", new Vector3(w * 0.5f, h * 0.5f, 0f), new Vector3(wall, h, depth), shell, doorOnXPos, false);

            // Interior marker (named) for jobs/spawns; pure transform.
            if (!string.IsNullOrEmpty(hb.interiorMarkerId))
            {
                var marker = new GameObject("Marker_" + hb.interiorMarkerId);
                marker.transform.SetParent(hbRoot, false);
                marker.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            }

            // A simple interior interactable plinth for JobGiver/Mission kinds.
            if (hb.interior != InteriorKind.Empty)
            {
                Cube(hbRoot, hb.id + "_Interactable",
                    new Vector3(0f, 0.6f, depth * 0.25f), new Vector3(1f, 1.2f, 1f), pal.accent, true);
            }
        }

        private static void BuildWallWithMaybeGap(Transform parent, string name, Vector3 center, Vector3 size, Color color, bool gap, bool gapAlongX)
        {
            if (!gap)
            {
                Cube(parent, name, center, size, color, true);
                return;
            }
            // Split the wall into two panels leaving a ~1.4m doorway in the middle.
            float doorW = 1.4f;
            if (gapAlongX)
            {
                float side = (size.x - doorW) * 0.5f;
                Cube(parent, name + "_a", center + new Vector3(-(doorW * 0.5f + side * 0.5f), 0f, 0f), new Vector3(side, size.y, size.z), color, true);
                Cube(parent, name + "_b", center + new Vector3((doorW * 0.5f + side * 0.5f), 0f, 0f), new Vector3(side, size.y, size.z), color, true);
            }
            else
            {
                float side = (size.z - doorW) * 0.5f;
                Cube(parent, name + "_a", center + new Vector3(0f, 0f, -(doorW * 0.5f + side * 0.5f)), new Vector3(size.x, size.y, side), color, true);
                Cube(parent, name + "_b", center + new Vector3(0f, 0f, (doorW * 0.5f + side * 0.5f)), new Vector3(size.x, size.y, side), color, true);
            }
        }

        private static void ScatterProps(Transform districtRoot, CityLayoutDefinition kit, PropPatchDef p, GlobalPalette pal)
        {
            var patchRoot = NewChild(districtRoot, "Props_" + p.kind);
            int n = Mathf.RoundToInt(p.size.x * p.size.y * p.density * 0.05f);
            for (int i = 0; i < n; i++)
            {
                float x = p.center.x + (Random.value - 0.5f) * p.size.x;
                float z = p.center.z + (Random.value - 0.5f) * p.size.y;
                float s = 0.6f + Random.value * 0.8f;
                Cube(patchRoot, p.kind + "_" + i, new Vector3(x, kit.walkwayHeight + s * 0.5f, z), new Vector3(s, s, s), pal.rail, true);
            }
        }

        // ── Connections (streets / walkways / bridges / ramps) ───────────────
        private static void BuildConnection(Transform root, CityLayoutDefinition kit, ConnectionDef c)
        {
            var from = FindDistrict(kit, c.fromDistrictId);
            var to = FindDistrict(kit, c.toDistrictId);
            if (from == null || to == null) return;

            var connRoot = root.Find("Connections") ?? NewChild(root, "Connections");
            Vector3 a = from.anchor;
            Vector3 b = to.anchor;
            float y = kit.walkwayHeight + (c.tier > 0 ? TierStep : 0f);

            Vector3 mid = (a + b) * 0.5f; mid.y = y - SlabThickness * 0.5f;
            Vector3 delta = b - a; delta.y = 0f;
            float length = delta.magnitude;
            float angle = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;

            string name = c.kind + "_" + c.fromDistrictId + "_" + c.toDistrictId;
            var slab = Cube(connRoot, name, Vector3.zero, new Vector3(c.width, SlabThickness, length), kit.palette.catwalk, true);
            slab.transform.SetParent(connRoot, true);
            slab.transform.position = root.TransformPoint(mid);
            slab.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Railings on elevated/over-canal walkways and bridges (the catwalk-fall fix).
            if (c.kind == ConnectionKind.ElevatedWalkway || c.kind == ConnectionKind.Bridge || c.tier > 0)
                AddRailings(slab, c.width, length, kit.palette.rail);
        }

        private static void AddRailings(GameObject slab, float width, float length, Color color)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rail.name = "Railing_" + (side < 0 ? "L" : "R");
                rail.transform.SetParent(slab.transform, false);
                // Convert to local scale (parent slab is scaled).
                Vector3 ps = slab.transform.localScale;
                rail.transform.localScale = new Vector3(RailThickness / ps.x, RailHeight / ps.y, length / ps.z);
                rail.transform.localPosition = new Vector3(side * (width * 0.5f) / ps.x, (SlabThickness * 0.5f + RailHeight * 0.5f) / ps.y, 0f);
                var r = rail.GetComponent<Renderer>();
                if (r != null) { r.sharedMaterial = Mat(color); r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; }
            }
        }

        // ── Shipyard (static ship + berth) ───────────────────────────────────
        private static void BuildShipyard(Transform root, CityLayoutDefinition kit)
        {
            var s = kit.shipyard;
            if (s == null || !s.enabled) return;
            var yardRoot = NewChild(root, "Shipyard");
            yardRoot.localPosition = s.berthCenter;

            // Berth deck (walkable).
            Cube(yardRoot, "BerthDeck", new Vector3(0f, kit.walkwayHeight - SlabThickness * 0.5f, 0f),
                new Vector3(s.berthSize.x, SlabThickness, s.berthSize.y), kit.palette.metal, true);

            // Static ship placeholder (blue-collar salvage rig silhouette) — walkable-proof, not flyable.
            var ship = NewChild(yardRoot, "Ship_Static_Placeholder");
            ship.localPosition = new Vector3(s.shipLocalPos.x, kit.walkwayHeight + s.shipLocalPos.y, s.shipLocalPos.z);
            ship.localRotation = Quaternion.Euler(0f, s.shipRotationY, 0f);
            Cube(ship, "Hull", Vector3.zero, s.shipSize, kit.palette.building2, true);
            Cube(ship, "Cockpit", new Vector3(0f, s.shipSize.y * 0.4f, s.shipSize.z * 0.3f), new Vector3(s.shipSize.x * 0.7f, s.shipSize.y * 0.5f, s.shipSize.z * 0.3f), kit.palette.accent, true);
            Cube(ship, "EngineL", new Vector3(-s.shipSize.x * 0.45f, 0f, -s.shipSize.z * 0.45f), new Vector3(s.shipSize.x * 0.25f, s.shipSize.y * 0.5f, s.shipSize.z * 0.2f), kit.palette.metal, true);
            Cube(ship, "EngineR", new Vector3(s.shipSize.x * 0.45f, 0f, -s.shipSize.z * 0.45f), new Vector3(s.shipSize.x * 0.25f, s.shipSize.y * 0.5f, s.shipSize.z * 0.2f), kit.palette.metal, true);
        }

        // ── Hazards (biome mechanics — GAME_PLAN M2) ─────────────────────────
        private static void BuildHazardZones(Transform root, CityLayoutDefinition kit)
        {
            if (kit.hazards == null || kit.hazards.Count == 0) return;
            var hazardRoot = NewChild(root, "Hazards");
            foreach (var h in kit.hazards)
            {
                if (h == null) continue;
                var go = new GameObject("Hazard_" + h.id);
                go.transform.SetParent(hazardRoot, false);
                go.transform.position = new Vector3(h.center.x, kit.walkwayHeight, h.center.z);
                go.AddComponent<HazardZoneRuntime>().Init(h);
            }
        }

        // ── Drones (passive here; combat wired in MakeDrone) ─────────────────
        private static void BuildDroneZones(Transform root, CityLayoutDefinition kit)
        {
            if (kit.droneZones == null || kit.droneZones.Count == 0) return;
            var droneRoot = NewChild(root, "Drones");
            foreach (var z in kit.droneZones)
            {
                if (z == null) continue;
                for (int i = 0; i < z.count; i++)
                {
                    float a = (i / (float)Mathf.Max(1, z.count)) * Mathf.PI * 2f;
                    var pos = new Vector3(
                        z.center.x + Mathf.Cos(a) * z.radius,
                        kit.walkwayHeight + 3f + Random.value * 1.5f,
                        z.center.z + Mathf.Sin(a) * z.radius);
                    MakeDrone(droneRoot, z.id + "_" + i, pos, z.respawnDelay, z.combat, z.variantId);
                }
            }
        }

        private static void MakeDrone(Transform parent, string name, Vector3 pos, float respawnDelay, bool combat, string variantId)
        {
            var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            drone.name = name;
            drone.transform.SetParent(parent, false);
            drone.transform.position = pos;
            drone.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);
            var dr = drone.AddComponent<DroneRuntime>();
            dr.respawnDelay = respawnDelay;

            if (combat)
            {
                var cb = drone.AddComponent<DroneCombatBehavior>();
                if (!string.IsNullOrEmpty(variantId))
                {
                    var prof = Resources.Load<Ziptide.Content.DroneCombatProfile>("Enemies/" + variantId);
                    if (prof != null) cb.profile = prof;
                }
            }
        }

        // ── Small helpers ────────────────────────────────────────────────────
        private static Transform NewChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null) return existing;
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
        }
    }
}
#endif
