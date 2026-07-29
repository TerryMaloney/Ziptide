#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE RING CITY — the approved concentric ToxicCity from `CITY_VISUAL_SPEC.md` §1, built from
    /// <see cref="RingCityDef"/> data measured off Terry's own K1/K7 sheets.
    ///
    /// Six elements, in order outward from the middle:
    ///   1. the Tower island — the colossal leaning scrap tower on its knot of land;
    ///   2. shanty wedges divided by a canal ring and radial canals, crossed by causeways;
    ///   3. the old sea wall, breached in places;
    ///   4. the harbour wedge on the south face with its double-armed breakwater;
    ///   5. the flyable outskirts — beached wrecks, stilt villages, glowing green tide pools;
    ///   6. the gate-pillar ring on the horizon, which is a VISTA and never walkable geometry.
    ///
    /// This is the shell the city reads as from a distance and from the air. The authored districts
    /// keep providing the walkable contract content inside it; nothing existing is moved or deleted,
    /// so a world that has not opted in renders exactly as it does today.
    ///
    /// Budget discipline: one shared material per semantic slot (nine total), everything marked
    /// batching-static, shadows off, and colliders ONLY on surfaces a player is meant to stand on.
    /// Deterministic from `kit.seed` — the same layout produces the same city every build.
    /// </summary>
    public static class RingCityBuilder
    {
        private const string RootName = "__RING_CITY";

        // Semantic material slots. Adding a tenth means justifying it against the draw-call budget.
        private const string SlotLand = "Land";
        private const string SlotStone = "Stone";
        private const string SlotTenement = "Tenement";
        private const string SlotTenementAlt = "TenementAlt";
        private const string SlotMetal = "Metal";
        private const string SlotWater = "Water";
        private const string SlotGlow = "Glow";
        private const string SlotWindow = "Window";
        private const string SlotHorizon = "Horizon";

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public static int Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null || kit.rings == null || !kit.rings.enabled) return 0;

            var issues = kit.rings.Validate();
            if (issues.Count > 0)
            {
                for (int i = 0; i < issues.Count; i++)
                    Debug.LogWarning("[Ziptide] ring city skipped: " + issues[i]);
                return 0;
            }

            Materials.Clear();
            Transform existing = cityRoot.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);

            RingCityDef r = kit.rings;
            GlobalPalette pal = kit.palette;
            float y = kit.walkwayHeight;
            var rng = new System.Random(kit.seed ^ 0x21C17E);

            if (r.buildTidalFlat) BuildTidalFlat(root, r, pal, y);
            if (r.buildTowerIsland) BuildTowerIsland(root, r, pal, y);
            if (r.buildCanalRing)
            {
                BuildCanalRing(root, r, pal, y);
                BuildRadialCanals(root, r, pal, y);
                BuildCauseways(root, r, pal, y);
            }
            if (r.buildWedges) BuildWedges(root, r, pal, y, rng);
            if (r.buildSeaWall) BuildSeaWall(root, r, pal, y);
            if (r.buildHarbour) BuildHarbour(root, r, pal, y);
            if (r.buildOutskirts) BuildOutskirts(root, r, pal, y, rng);
            if (r.buildGatePillars) BuildGatePillarRing(root, r, pal, y);

            int built = root.GetComponentsInChildren<Transform>(true).Length - 1;
            Debug.Log("[Ziptide] RING_CITY built objects=" + built + " materials=" + Materials.Count
                + " outerRadius=" + r.outskirtsRadius.ToString("F0"));
            Materials.Clear();
            return built;
        }

        // ── 0. The drowned tidal flat everything stands on ───────────────────
        /// <summary>
        /// A world with no terrain has no ground past its authored districts, so every outer ring
        /// would hang in the void. The spec's own first line is "concentric rings on a drowned tidal
        /// flat" — the flat is not scenery, it is the thing the city is built on.
        ///
        /// One disc, one collider, sitting just below walkway height so authored district slabs still
        /// read as raised built pads rather than being z-fought into mud.
        /// </summary>
        private static void BuildTidalFlat(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform flat = Child(root, "TidalFlat", new Vector3(0f, y, 0f));
            float diameter = r.outskirtsRadius * 2f;
            Cylinder(flat, "Mudflat", new Vector3(0f, -0.55f, 0f),
                new Vector3(diameter, 0.5f, diameter), SlotLand, pal.concrete, collider: true);

            // A shallow water skirt beyond the flats so the horizon reads as sea, not as a cliff edge.
            Cylinder(flat, "ShallowSea", new Vector3(0f, -0.75f, 0f),
                new Vector3(diameter * 1.9f, 0.2f, diameter * 1.9f), SlotWater, pal.toxic);
        }

        // ── 1. The Tower island ──────────────────────────────────────────────
        // "Stacked mismatched decks, cranes at the crown, lean 10-15 degrees." The lean is applied to
        // the whole stack, not to each deck, so the silhouette reads as ONE leaning thing.
        private static void BuildTowerIsland(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform island = Child(root, "TowerIsland", new Vector3(0f, y, 0f));

            Cylinder(island, "IslandDeck", new Vector3(0f, -0.4f, 0f),
                new Vector3(r.islandRadius * 2f, 0.4f, r.islandRadius * 2f), SlotLand, pal.concrete,
                collider: true);

            Transform tower = Child(island, "Tower", Vector3.zero);
            tower.localRotation = Quaternion.Euler(0f, 24f, r.towerLeanDegrees);

            float deckHeight = r.towerHeight / Mathf.Max(1, r.towerDeckCount);
            for (int i = 0; i < r.towerDeckCount; i++)
            {
                // Each deck is narrower, offset and twisted: mismatched salvage, not a wedding cake.
                float t = i / (float)Mathf.Max(1, r.towerDeckCount - 1);
                float width = Mathf.Lerp(r.islandRadius * 0.75f, r.islandRadius * 0.22f, t);
                float lift = deckHeight * (i + 0.5f);
                var deck = Cube(tower, "Deck_" + i,
                    new Vector3(Mathf.Sin(i * 2.1f) * width * 0.16f, lift, Mathf.Cos(i * 1.7f) * width * 0.16f),
                    new Vector3(width, deckHeight * 0.92f, width * (0.8f + 0.2f * (i % 2))),
                    i % 2 == 0 ? SlotTenement : SlotTenementAlt,
                    i % 2 == 0 ? pal.building1 : pal.building2);
                deck.transform.localRotation = Quaternion.Euler(0f, i * 17f, 0f);

                // A band of lit windows per deck keeps the hero readable at night and at distance.
                Cube(tower, "Deck_" + i + "_Lights",
                    new Vector3(0f, lift + deckHeight * 0.18f, width * 0.42f),
                    new Vector3(width * 0.8f, deckHeight * 0.10f, 0.25f),
                    SlotWindow, pal.accent, emissive: true);
            }

            for (int c = 0; c < r.crownCraneCount; c++)
            {
                float a = (c / (float)Mathf.Max(1, r.crownCraneCount)) * Mathf.PI * 2f;
                Transform crane = Child(tower, "CrownCrane_" + c,
                    new Vector3(Mathf.Cos(a) * 3f, r.towerHeight, Mathf.Sin(a) * 3f));
                crane.localRotation = Quaternion.Euler(0f, a * Mathf.Rad2Deg, 0f);
                Cube(crane, "Mast", new Vector3(0f, 4f, 0f), new Vector3(0.6f, 8f, 0.6f), SlotMetal, pal.metal);
                Cube(crane, "Jib", new Vector3(0f, 7.6f, 5f), new Vector3(0.4f, 0.4f, 11f), SlotMetal, pal.metal);
            }
        }

        // ── 2. The canal ring and the wedges it divides ──────────────────────
        private static void BuildCanalRing(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform ring = Child(root, "CanalRing", new Vector3(0f, y, 0f));
            const int segments = 36;
            for (int i = 0; i < segments; i++)
            {
                float a = (i / (float)segments) * Mathf.PI * 2f;
                float arc = (Mathf.PI * 2f * r.canalRingRadius) / segments;
                var seg = Cube(ring, "Canal_" + i,
                    new Vector3(Mathf.Cos(a) * r.canalRingRadius, -0.35f, Mathf.Sin(a) * r.canalRingRadius),
                    new Vector3(r.canalWidth, 0.2f, arc * 1.08f), SlotWater, pal.toxic, emissive: true);
                seg.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);
            }
        }

        private static void BuildRadialCanals(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform radial = Child(root, "RadialCanals", new Vector3(0f, y, 0f));
            float inner = r.canalRingRadius;
            float outer = r.wedgeOuterRadius;
            float length = outer - inner;
            for (int i = 0; i < r.radialCanalCount; i++)
            {
                float a = (i / (float)Mathf.Max(1, r.radialCanalCount)) * Mathf.PI * 2f + 0.35f;
                float mid = (inner + outer) * 0.5f;
                var seg = Cube(radial, "Radial_" + i,
                    new Vector3(Mathf.Cos(a) * mid, -0.35f, Mathf.Sin(a) * mid),
                    new Vector3(r.canalWidth * 0.8f, 0.2f, length), SlotWater, pal.toxic, emissive: true);
                seg.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg + 90f, 0f);
            }
        }

        /// <summary>
        /// The shanty wedges: irregular stacked corrugated tenements filling the arc between the canal
        /// ring and the sea wall. Deliberately uneven — a regular ring of identical blocks reads as a
        /// stadium, and the whole point of the concept is a knot of accreted salvage.
        /// </summary>
        private static void BuildWedges(Transform root, RingCityDef r, GlobalPalette pal, float y,
            System.Random rng)
        {
            Transform wedges = Child(root, "ShantyWedges", new Vector3(0f, y, 0f));
            float inner = r.canalRingRadius + r.canalWidth * 0.7f;
            float outer = r.wedgeOuterRadius;

            for (int w = 0; w < r.wedgeCount; w++)
            {
                float wedgeCentre = (w / (float)r.wedgeCount) * Mathf.PI * 2f;
                Transform wedge = Child(wedges, "Wedge_" + w, Vector3.zero);

                const int rows = 4;
                for (int row = 0; row < rows; row++)
                {
                    float radius = Mathf.Lerp(inner, outer, (row + 0.5f) / rows);
                    // Fewer blocks on the inner rows: the arc is shorter there, and crowding the
                    // waterline is what makes a ring city read as circular rather than as a wall.
                    int perRow = 4 + row;
                    for (int b = 0; b < perRow; b++)
                    {
                        float spread = (Mathf.PI * 2f / r.wedgeCount) * 0.78f;
                        float a = wedgeCentre + Mathf.Lerp(-spread * 0.5f, spread * 0.5f,
                            (b + 0.5f) / perRow);
                        float jitterR = (float)(rng.NextDouble() - 0.5) * 6f;
                        float height = 5f + (float)rng.NextDouble() * 11f;
                        float footprint = 5f + (float)rng.NextDouble() * 3f;

                        var block = Cube(wedge, "Tenement_" + row + "_" + b,
                            new Vector3(Mathf.Cos(a) * (radius + jitterR), height * 0.5f,
                                Mathf.Sin(a) * (radius + jitterR)),
                            new Vector3(footprint, height, footprint * 0.85f),
                            (row + b) % 2 == 0 ? SlotTenement : SlotTenementAlt,
                            (row + b) % 2 == 0 ? pal.building1 : pal.building2,
                            collider: true);
                        block.transform.localRotation =
                            Quaternion.Euler(0f, -a * Mathf.Rad2Deg + (float)(rng.NextDouble() - 0.5) * 24f, 0f);

                        // Warm window light, in two or three warmths per the palette note. The old
                        // city's windows were a cold dark blue, which is why it read as unlit boxes.
                        if (rng.NextDouble() < 0.66)
                            Cube(block.transform, "Windows",
                                new Vector3(0f, 0.08f, -0.52f), new Vector3(0.62f, 0.22f, 0.06f),
                                SlotWindow, pal.accent, emissive: true);
                    }
                }
            }
        }

        private static void BuildCauseways(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform ways = Child(root, "Causeways", new Vector3(0f, y, 0f));
            for (int i = 0; i < r.causewayCount; i++)
            {
                float a = (i / (float)Mathf.Max(1, r.causewayCount)) * Mathf.PI * 2f;
                var deck = Cube(ways, "Causeway_" + i,
                    new Vector3(Mathf.Cos(a) * r.canalRingRadius, 0.12f, Mathf.Sin(a) * r.canalRingRadius),
                    new Vector3(4.5f, 0.25f, r.canalWidth * 1.5f), SlotStone, pal.concrete, collider: true);
                deck.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);

                for (int s = -1; s <= 1; s += 2)
                {
                    var rail = Cube(deck.transform, "Rail_" + (s < 0 ? "L" : "R"),
                        new Vector3(s * 0.48f, 1.6f, 0f), new Vector3(0.06f, 2.4f, 1.02f),
                        SlotMetal, pal.rail);
                    rail.transform.localRotation = Quaternion.identity;
                }
            }
        }

        // ── 3. The old sea wall, breached ────────────────────────────────────
        private static void BuildSeaWall(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform wall = Child(root, "SeaWall", new Vector3(0f, y, 0f));
            const int segments = 48;
            float harbourFrom = r.harbourAzimuthDegrees - r.harbourArcDegrees * 0.5f;
            float harbourTo = r.harbourAzimuthDegrees + r.harbourArcDegrees * 0.5f;

            // Breaches are landmarks, spaced so they never collapse into one long gap.
            var breachAt = new HashSet<int>();
            for (int b = 0; b < r.seaWallBreachCount; b++)
                breachAt.Add((int)(segments * ((b + 0.5f) / r.seaWallBreachCount) + 7) % segments);

            for (int i = 0; i < segments; i++)
            {
                float degrees = (i / (float)segments) * 360f;
                if (degrees >= harbourFrom && degrees <= harbourTo) continue;   // the harbour mouth
                if (breachAt.Contains(i)) continue;                             // a breach

                float a = degrees * Mathf.Deg2Rad;
                float arc = (Mathf.PI * 2f * r.seaWallRadius) / segments;
                var seg = Cube(wall, "Wall_" + i,
                    new Vector3(Mathf.Cos(a) * r.seaWallRadius, r.seaWallHeight * 0.5f,
                        Mathf.Sin(a) * r.seaWallRadius),
                    new Vector3(r.seaWallThickness, r.seaWallHeight, arc * 1.05f),
                    SlotStone, pal.concrete, collider: true);
                seg.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);

                // Wall-top shacks, per the spec. Sparse: they are punctuation, not a second city.
                if (i % 7 != 0) continue;
                Cube(seg.transform, "WallShack", new Vector3(0f, 0.72f, 0f),
                    new Vector3(1.6f, 0.55f, 0.5f), SlotTenementAlt, pal.building2);
            }
        }

        // ── 4. The harbour wedge ─────────────────────────────────────────────
        private static void BuildHarbour(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform harbour = Child(root, "Harbour", new Vector3(0f, y, 0f));
            float centre = r.harbourAzimuthDegrees * Mathf.Deg2Rad;

            // Quay: the working edge of the basin, and where the moored Scrapper belongs.
            var quay = Cube(harbour, "Quay",
                new Vector3(Mathf.Cos(centre) * (r.seaWallRadius - 8f), 0.1f,
                    Mathf.Sin(centre) * (r.seaWallRadius - 8f)),
                new Vector3(34f, 0.5f, 16f), SlotStone, pal.concrete, collider: true);
            quay.transform.localRotation = Quaternion.Euler(0f, -centre * Mathf.Rad2Deg, 0f);

            // Two breakwater arms curving out from the mouth — the double arm the sheets show.
            for (int arm = -1; arm <= 1; arm += 2)
            {
                Transform breakwater = Child(harbour, "Breakwater_" + (arm < 0 ? "W" : "E"), Vector3.zero);
                const int blocks = 12;
                for (int i = 0; i < blocks; i++)
                {
                    float t = (i + 0.5f) / blocks;
                    float a = centre + arm * (r.harbourArcDegrees * 0.5f * Mathf.Deg2Rad) * (1f - t * 0.45f);
                    float radius = r.seaWallRadius + t * r.breakwaterLength;
                    var block = Cube(breakwater, "Block_" + i,
                        new Vector3(Mathf.Cos(a) * radius, 1.1f, Mathf.Sin(a) * radius),
                        new Vector3(5.5f, 2.2f, 5.5f), SlotStone, pal.concrete, collider: true);
                    block.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);
                }
            }
        }

        // ── 5. The flyable outskirts ─────────────────────────────────────────
        // Mud flats with beached wrecks (squatters aboard), stilt villages, and the glowing green
        // tide pools that light the flats for a night flyover.
        private static void BuildOutskirts(Transform root, RingCityDef r, GlobalPalette pal, float y,
            System.Random rng)
        {
            Transform flats = Child(root, "Outskirts", new Vector3(0f, y, 0f));
            float inner = r.seaWallRadius + 14f;
            float outer = r.outskirtsRadius;

            for (int i = 0; i < r.beachedWreckCount; i++)
            {
                float a = (float)rng.NextDouble() * Mathf.PI * 2f;
                float radius = Mathf.Lerp(inner, outer, (float)rng.NextDouble());
                Transform wreck = Child(flats, "Wreck_" + i,
                    new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius));
                wreck.localRotation = Quaternion.Euler(0f, (float)rng.NextDouble() * 360f, 12f);

                float length = 22f + (float)rng.NextDouble() * 26f;
                Cube(wreck, "Hull", new Vector3(0f, 2.4f, 0f), new Vector3(7f, 5f, length),
                    SlotMetal, pal.rail, collider: true);
                Cube(wreck, "Superstructure", new Vector3(0f, 6.4f, -length * 0.28f),
                    new Vector3(5f, 4f, 7f), SlotMetal, pal.metal);
                // A squatter shack aboard — the dressing gold the spec calls out.
                Cube(wreck, "SquatterShack", new Vector3(1.6f, 9.1f, -length * 0.28f),
                    new Vector3(2.4f, 1.9f, 2.4f), SlotTenementAlt, pal.building2);
            }

            for (int v = 0; v < r.stiltVillageCount; v++)
            {
                float a = (v / (float)Mathf.Max(1, r.stiltVillageCount)) * Mathf.PI * 2f + 1.1f;
                float radius = Mathf.Lerp(inner, outer, 0.55f);
                Transform village = Child(flats, "StiltVillage_" + v,
                    new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius));

                for (int h = 0; h < 5; h++)
                {
                    float ox = (float)(rng.NextDouble() - 0.5) * 26f;
                    float oz = (float)(rng.NextDouble() - 0.5) * 26f;
                    Cube(village, "Stilts_" + h, new Vector3(ox, 1.6f, oz),
                        new Vector3(0.35f, 3.2f, 0.35f), SlotMetal, pal.metal);
                    Cube(village, "Hut_" + h, new Vector3(ox, 4.1f, oz),
                        new Vector3(4.2f, 2.4f, 4.2f), SlotTenementAlt, pal.building2, collider: true);
                }
                // Plank causeways linking the huts.
                Cube(village, "Planks", new Vector3(0f, 3.1f, 0f), new Vector3(26f, 0.15f, 1.4f),
                    SlotMetal, pal.catwalk, collider: true);
            }

            for (int p = 0; p < r.tidePoolCount; p++)
            {
                float a = (float)rng.NextDouble() * Mathf.PI * 2f;
                float radius = Mathf.Lerp(inner, outer, (float)rng.NextDouble());
                float size = 8f + (float)rng.NextDouble() * 14f;
                Cylinder(flats, "TidePool_" + p,
                    new Vector3(Mathf.Cos(a) * radius, 0.05f, Mathf.Sin(a) * radius),
                    new Vector3(size, 0.06f, size), SlotGlow, pal.toxic, emissive: true);
            }
        }

        // ── 6. The gate pillars on the horizon ───────────────────────────────
        /// <summary>
        /// A far-anchor VISTA, never geometry. No colliders, and deliberately placed beyond the
        /// playable radius: the pillars tie every wide shot to the Ziptide, and the illusion dies the
        /// moment a player can walk up and touch one.
        /// </summary>
        private static void BuildGatePillarRing(Transform root, RingCityDef r, GlobalPalette pal, float y)
        {
            Transform ring = Child(root, "GatePillarRing", new Vector3(0f, y, 0f));
            float centre = r.gatePillarAzimuthDegrees * Mathf.Deg2Rad;
            float spread = 26f * Mathf.Deg2Rad;

            for (int i = 0; i < r.gatePillarCount; i++)
            {
                float t = r.gatePillarCount <= 1 ? 0.5f : i / (float)(r.gatePillarCount - 1);
                float a = centre + Mathf.Lerp(-spread, spread, t);
                var pillar = Cube(ring, "Pillar_" + i,
                    new Vector3(Mathf.Cos(a) * r.gatePillarDistance, r.gatePillarHeight * 0.5f,
                        Mathf.Sin(a) * r.gatePillarDistance),
                    new Vector3(6f, r.gatePillarHeight, 6f), SlotHorizon, pal.skyline);
                pillar.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);

                // One cyan glint per pillar: the rare accent that says "this is the gate".
                Cube(pillar.transform, "Crown", new Vector3(0f, 0.46f, 0f),
                    new Vector3(1.15f, 0.06f, 1.15f), SlotGlow, new Color(0.45f, 0.95f, 0.95f),
                    emissive: true);
            }
        }

        // ── Primitives (shared material per slot, batching-static, shadows off) ──
        private static Transform Child(Transform parent, string name, Vector3 localPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            return go.transform;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPosition,
            Vector3 localScale, string slot, Color color, bool collider = false, bool emissive = false)
        {
            return Primitive(PrimitiveType.Cube, parent, name, localPosition, localScale, slot, color,
                collider, emissive);
        }

        private static GameObject Cylinder(Transform parent, string name, Vector3 localPosition,
            Vector3 localScale, string slot, Color color, bool collider = false, bool emissive = false)
        {
            // Unity's cylinder is 2 units tall, so a Y scale of 1 is 2 m. Halve it so callers can
            // think in metres like they do for cubes.
            localScale.y *= 0.5f;
            return Primitive(PrimitiveType.Cylinder, parent, name, localPosition, localScale, slot,
                color, collider, emissive);
        }

        private static GameObject Primitive(PrimitiveType type, Transform parent, string name,
            Vector3 localPosition, Vector3 localScale, string slot, Color color, bool collider,
            bool emissive)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            var existingCollider = go.GetComponent<Collider>();
            if (existingCollider != null && !collider) Object.DestroyImmediate(existingCollider);

            // ⚠ A Unity cylinder ships a CAPSULE collider, and a capsule does not scale like its mesh:
            // its radius takes the LARGER of X/Z and its height is clamped to at least twice that. A
            // 380 m wide, 0.5 m thick disc therefore becomes a 380 m TALL pill that swallows the whole
            // world — which is exactly how the tidal flat came to overlap ToxicCity's spawn
            // (SPAWN_OVERLAP_SOLID, run 30415165711). A box matches the mesh's footprint honestly, and
            // under a round visual the player never sees the difference.
            if (collider && type == PrimitiveType.Cylinder)
            {
                var capsule = go.GetComponent<CapsuleCollider>();
                if (capsule != null) Object.DestroyImmediate(capsule);
                go.AddComponent<BoxCollider>();
            }

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = MaterialFor(slot, color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            return go;
        }

        private static Material MaterialFor(string slot, Color color, bool emissive)
        {
            if (Materials.TryGetValue(slot, out Material cached) && cached != null) return cached;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return null;

            var material = new Material(shader) { name = "RingCity_" + slot };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.9f);
            }

            Materials[slot] = material;
            return material;
        }
    }
}
#endif
