#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// The SLV-01 "SCRAPPER" hero hull — built to the measured spec in
    /// docs/project_art_plan/SHIP_SCAVENGER_VISUAL_SPEC.md §2/§4, which was measured off Terry's
    /// approved concepts in concepts/ship_scavenger_mk1/ (the ortho sheet outranks this file).
    ///
    /// This replaced a GENERIC fighter (wings, wingtips, tail fin, swept nacelles, radar dish, bubble
    /// canopy) that read as "Lego Star Wars" because it was simply the wrong ship. The Scrapper is a
    /// salvage tug: a truck CAB that breaks the roofline, a boxy work HULL, one oversized rear ENGINE
    /// DRUM whose back face is the signature (one big nozzle + four in a quincunx), an articulated
    /// CLAW on the LEFT flank only (asymmetry is canon), and four splayed LANDING LEGS whose stance is
    /// wider than the hull so daylight shows between leg and body in the front view.
    ///
    /// Spec §4 sequencing: silhouette first, then the back-view engine, then texture, then the claw and
    /// legs, then the cyan port. Geometry here is the silhouette+detail pass; it is Stage 1 (B+) of the
    /// two-stage ship delivery and becomes the distant/berth LOD when the Tier-C hero mesh lands.
    ///
    /// Refit contract: the named direct children below are ShipRefit's skeleton. Fuselage_Aft/Mid/Bow,
    /// Nose_Tip and DorsalSpine keep their names (they map onto real Scrapper masses); the old
    /// Wing_/WingTip_/TailFin/Exhaust_ vocabulary was remapped onto Leg_*, ClawArm_*, EngineDrum and
    /// Nozzle_* — see ShipRefit.ApplyProportions.
    /// </summary>
    public static class ShipHullBuilder
    {
        public const int MinimumHeroRenderers = 52;
        public const int MaximumHeroMaterials = 9;

        // Spec §3 palette. These are the ship's IDENTITY, so they lead; the world palette is blended in
        // at a quarter strength so themes still tint the hull without turning it into a different ship.
        private static readonly Color SpecSteel = new Color(0.42f, 0.47f, 0.53f); // weathered blue-grey
        private static readonly Color SpecSteelAlt = new Color(0.33f, 0.38f, 0.44f); // mismatched plate
        private static readonly Color SpecGunmetal = new Color(0.24f, 0.26f, 0.29f); // engine drum
        private static readonly Color SpecThroat = new Color(0.07f, 0.07f, 0.08f); // nozzle throats
        private static readonly Color SpecHazard = new Color(0.86f, 0.62f, 0.16f); // chipped amber
        private static readonly Color SpecGlass = new Color(0.20f, 0.60f, 0.66f); // teal cab interior
        private static readonly Color SpecFlood = new Color(1.00f, 0.94f, 0.80f); // warm floodlights
        private static readonly Color SpecCyan = new Color(0.25f, 0.90f, 0.95f); // THE coupler port
        private static readonly Color SpecMarker = new Color(0.95f, 0.35f, 0.20f); // marker dots

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        /// <summary>Build under ship; size is beam/height/length and +Z is bow.</summary>
        public static void Build(Transform ship, Vector3 size, GlobalPalette pal)
        {
            if (ship == null || pal == null) return;
            Materials.Clear();
            float w = Mathf.Max(3f, size.x);
            float h = Mathf.Max(2f, size.y);
            float l = Mathf.Max(7f, size.z);

            // Spec §2 works in fractions of total LENGTH measured nose→nozzle. Fraction 0 is the bow.
            float Z(float f) => l * (0.5f - f);

            Color steel = Tint(SpecSteel, pal.building2);
            Color steelAlt = Tint(SpecSteelAlt, pal.metal);
            Color gunmetal = Tint(SpecGunmetal, pal.metal);

            float hullHalfW = w * 0.425f;   // boxy: slightly wider than tall
            float hullHalfH = h * 0.5f;
            float bellyY = -hullHalfH;
            float roofY = hullHalfH;

            // ── GROUP 1 · NOSE BLOCK (0.00–0.18) — sloped down-forward, lower half of hull height ──
            Part(ship, "Fuselage_Bow", PrimitiveType.Cube,
                new Vector3(0f, -h * 0.06f, Z(0.09f)),
                new Vector3(hullHalfW * 1.72f, h * 0.62f, l * 0.18f),
                new Vector3(-6f, 0f, 0f), steel, true, "Steel");
            Part(ship, "Nose_Tip", PrimitiveType.Cube,
                new Vector3(0f, -h * 0.20f, Z(0.02f)),
                new Vector3(hullHalfW * 1.30f, h * 0.30f, l * 0.07f),
                new Vector3(-16f, 0f, 0f), steelAlt, false, "SteelAlt");
            for (int s = -1; s <= 1; s += 2)
            {
                string tag = s < 0 ? "L" : "R";
                // Hazard striping on the nose cheeks (spec §3).
                Part(ship, "NoseCheek_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 0.80f, -h * 0.10f, Z(0.07f)),
                    new Vector3(hullHalfW * 0.16f, h * 0.26f, l * 0.11f),
                    new Vector3(-6f, 0f, 0f), SpecHazard, false, "Hazard");
                Part(ship, "Headlight_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 0.52f, -h * 0.12f, Z(0.015f)),
                    new Vector3(hullHalfW * 0.30f, h * 0.11f, 0.10f),
                    Vector3.zero, SpecFlood, false, "Flood", true);
                // Nose floodlight cluster (spec §3: 2×2 warm-white panels).
                Part(ship, "NoseFlood_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 0.30f, h * 0.10f, Z(0.05f)),
                    new Vector3(hullHalfW * 0.22f, h * 0.07f, 0.09f),
                    new Vector3(-10f, 0f, 0f), SpecFlood, false, "Flood", true);
            }

            // ── GROUP 2 · CAB (0.10–0.30) — sits ON TOP; its roof is the ship's highest point ──
            float cabH = h * 0.35f;
            float cabY = roofY + cabH * 0.5f;
            float cabZ = Z(0.20f);
            Part(ship, "Cab", PrimitiveType.Cube,
                new Vector3(0f, cabY, cabZ),
                new Vector3(hullHalfW * 1.30f, cabH, l * 0.20f),
                Vector3.zero, steel, true, "Steel");
            Part(ship, "Cab_Roof", PrimitiveType.Cube,
                new Vector3(0f, cabY + cabH * 0.52f, cabZ),
                new Vector3(hullHalfW * 1.38f, h * 0.04f, l * 0.21f),
                Vector3.zero, steelAlt, false, "SteelAlt");
            // Panoramic front glass + two side-door windows, teal-lit interior (spec §2/§3).
            Part(ship, "Cab_Glass_Front", PrimitiveType.Cube,
                new Vector3(0f, cabY + cabH * 0.10f, cabZ + l * 0.101f),
                new Vector3(hullHalfW * 1.12f, cabH * 0.52f, 0.06f),
                Vector3.zero, SpecGlass, false, "Glass", true);
            for (int s = -1; s <= 1; s += 2)
            {
                string tag = s < 0 ? "L" : "R";
                Part(ship, "Cab_Glass_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 0.655f, cabY + cabH * 0.10f, cabZ - l * 0.02f),
                    new Vector3(0.06f, cabH * 0.44f, l * 0.10f),
                    Vector3.zero, SpecGlass, false, "Glass", true);
            }
            Part(ship, "Cab_LightBar", PrimitiveType.Cube,
                new Vector3(0f, cabY + cabH * 0.60f, cabZ + l * 0.07f),
                new Vector3(hullHalfW * 0.90f, h * 0.045f, 0.10f),
                Vector3.zero, SpecFlood, false, "Flood", true);
            // "Small antenna mast" (spec §2) — kept short on purpose: it sets the ship's highest
            // point, so a tall one silently inflates the hull bounds the berth and tests key off.
            Segment(ship, "Cab_Antenna", PrimitiveType.Capsule,
                new Vector3(hullHalfW * 0.50f, cabY + cabH * 0.55f, cabZ - l * 0.07f),
                new Vector3(hullHalfW * 0.56f, cabY + cabH * 1.05f, cabZ - l * 0.08f),
                0.035f, steelAlt, false, "SteelAlt");
            Part(ship, "Cab_Step", PrimitiveType.Cube,
                new Vector3(-hullHalfW * 0.86f, roofY - h * 0.10f, cabZ - l * 0.03f),
                new Vector3(hullHalfW * 0.22f, h * 0.03f, l * 0.07f),
                Vector3.zero, steelAlt, false, "SteelAlt");

            // ── GROUP 3 · MID HULL (0.18–0.65) — the working body ──
            Part(ship, "Fuselage_Mid", PrimitiveType.Cube,
                new Vector3(0f, 0f, Z(0.415f)),
                new Vector3(hullHalfW * 2f, h, l * 0.47f),
                Vector3.zero, steel, true, "Steel");
            Part(ship, "DorsalSpine", PrimitiveType.Cube,
                new Vector3(0f, roofY + h * 0.045f, Z(0.50f)),
                new Vector3(hullHalfW * 0.55f, h * 0.09f, l * 0.30f),
                Vector3.zero, steelAlt, false, "SteelAlt");
            for (int i = 0; i < 3; i++)
            {
                // Roofline machinery blocks + intake grilles (spec §2).
                float mz = Z(0.34f + i * 0.10f);
                Part(ship, "RoofMachinery_" + i, PrimitiveType.Cube,
                    new Vector3((i - 1) * hullHalfW * 0.44f, roofY + h * 0.10f, mz),
                    new Vector3(hullHalfW * 0.52f, h * 0.20f, l * 0.07f),
                    Vector3.zero, i == 1 ? gunmetal : steelAlt, false, i == 1 ? "Gunmetal" : "SteelAlt");
            }
            for (int s = -1; s <= 1; s += 2)
            {
                string tag = s < 0 ? "L" : "R";
                // Flank radiator panels + access hatches. hullHalfW IS the flank surface, so these
                // sit just proud of it — scaling by 2 here would float them off the ship entirely.
                Part(ship, "Radiator_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 1.01f, h * 0.10f, Z(0.44f)),
                    new Vector3(0.07f, h * 0.42f, l * 0.20f),
                    Vector3.zero, gunmetal, false, "Gunmetal");
                Part(ship, "Hatch_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 1.01f, -h * 0.16f, Z(0.28f)),
                    new Vector3(0.06f, h * 0.26f, l * 0.08f),
                    Vector3.zero, steelAlt, false, "SteelAlt");
                Part(ship, "HazardPatch_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 1.01f, -h * 0.34f, Z(0.55f)),
                    new Vector3(0.06f, h * 0.13f, l * 0.13f),
                    Vector3.zero, SpecHazard, false, "Hazard");
                Part(ship, "FlankFlood_" + tag, PrimitiveType.Cube,
                    new Vector3(s * hullHalfW * 0.96f, roofY - h * 0.06f, Z(0.24f)),
                    new Vector3(hullHalfW * 0.12f, h * 0.05f, 0.09f),
                    Vector3.zero, SpecFlood, false, "Flood", true);
                Part(ship, "Marker_" + tag, PrimitiveType.Sphere,
                    new Vector3(s * hullHalfW * 0.95f, roofY + h * 0.02f, Z(0.62f)),
                    Vector3.one * 0.09f, Vector3.zero, SpecMarker, false, "Marker", true);
                // Pipe runs along each flank (spec §4: thin capsules ×6).
                for (int p = 0; p < 3; p++)
                {
                    float py = -h * 0.30f + p * h * 0.16f;
                    Segment(ship, "Pipe_" + tag + p, PrimitiveType.Capsule,
                        new Vector3(s * hullHalfW * 0.99f, py, Z(0.24f)),
                        new Vector3(s * hullHalfW * 0.99f, py, Z(0.60f)),
                        0.055f, gunmetal, false, "Gunmetal");
                }
            }
            Part(ship, "CargoPod", PrimitiveType.Cube,
                new Vector3(0f, bellyY - h * 0.10f, Z(0.48f)),
                new Vector3(hullHalfW * 1.30f, h * 0.20f, l * 0.26f),
                Vector3.zero, steelAlt, true, "SteelAlt");

            // THE cyan power port — the artifact coupler. Spec §3: ONE recessed glowing socket,
            // mid-hull LEFT, its own emissive submesh so story states can drive it.
            Part(ship, "CyanPort_Recess", PrimitiveType.Cube,
                new Vector3(-hullHalfW * 1.01f, -h * 0.02f, Z(0.40f)),
                new Vector3(0.09f, h * 0.20f, l * 0.055f),
                Vector3.zero, SpecThroat, false, "Throat");
            Segment(ship, "CyanPort", PrimitiveType.Cylinder,
                new Vector3(-hullHalfW * 1.02f, -h * 0.02f, Z(0.40f)),
                new Vector3(-hullHalfW * 1.09f, -h * 0.02f, Z(0.40f)),
                0.20f, SpecCyan, false, "Cyan", true);

            // Boarding read stays on the port flank, aft of the claw so they never overlap.
            Part(ship, "BoardingDoor_Port", PrimitiveType.Cube,
                new Vector3(-hullHalfW * 1.015f, -h * 0.02f, Z(0.52f)),
                new Vector3(0.08f, h * 0.52f, l * 0.11f),
                Vector3.zero, SpecThroat, false, "Throat");
            Part(ship, "BoardingStep_Port", PrimitiveType.Cube,
                new Vector3(-hullHalfW * 1.16f, bellyY - h * 0.16f, Z(0.52f)),
                new Vector3(hullHalfW * 0.60f, h * 0.05f, l * 0.13f),
                Vector3.zero, steelAlt, true, "SteelAlt");

            // ── GROUP 4 · ENGINE CLUSTER (0.62–1.00) — the DOMINANT mass ──
            Part(ship, "Fuselage_Aft", PrimitiveType.Cube,
                new Vector3(0f, 0f, Z(0.66f)),
                new Vector3(hullHalfW * 1.76f, h * 0.86f, l * 0.10f),
                Vector3.zero, steelAlt, true, "SteelAlt");

            float drumD = h * 1.15f;                 // spec §2: engine drum ≈ 1.15 hull height
            float drumZc = Z(0.81f);
            float drumHalfL = l * 0.19f;
            Segment(ship, "EngineDrum", PrimitiveType.Cylinder,
                new Vector3(0f, 0f, drumZc + drumHalfL),
                new Vector3(0f, 0f, drumZc - drumHalfL),
                drumD, gunmetal, true, "Gunmetal");
            // Armor bands wrapping the drum.
            for (int b = 0; b < 2; b++)
            {
                float bz = Z(0.70f + b * 0.14f);
                Segment(ship, "DrumBand_" + b, PrimitiveType.Cylinder,
                    new Vector3(0f, 0f, bz + 0.06f), new Vector3(0f, 0f, bz - 0.06f),
                    drumD * 1.06f, steelAlt, false, "SteelAlt");
            }
            // Heavy collar the nozzles recess into.
            Segment(ship, "EngineCollar", PrimitiveType.Cylinder,
                new Vector3(0f, 0f, Z(0.94f)), new Vector3(0f, 0f, Z(0.985f)),
                drumD * 1.08f, gunmetal, false, "Gunmetal");

            // THE BACK-VIEW MONEY SHOT (spec §2): ONE large central nozzle + FOUR in a quincunx.
            float nozZ = Z(0.975f);
            Segment(ship, "Nozzle_Center", PrimitiveType.Cylinder,
                new Vector3(0f, 0f, nozZ + 0.30f), new Vector3(0f, 0f, nozZ - 0.30f),
                drumD * 0.43f, gunmetal, false, "Gunmetal");
            Segment(ship, "NozzleThroat_Center", PrimitiveType.Cylinder,
                new Vector3(0f, 0f, nozZ + 0.10f), new Vector3(0f, 0f, nozZ - 0.33f),
                drumD * 0.31f, SpecThroat, false, "Throat");
            string[] quad = { "TL", "TR", "BL", "BR" };
            for (int q = 0; q < 4; q++)
            {
                float qx = (q % 2 == 0 ? -1f : 1f) * drumD * 0.26f;
                float qy = (q < 2 ? 1f : -1f) * drumD * 0.26f;
                Segment(ship, "Nozzle_" + quad[q], PrimitiveType.Cylinder,
                    new Vector3(qx, qy, nozZ + 0.22f), new Vector3(qx, qy, nozZ - 0.24f),
                    drumD * 0.245f, gunmetal, false, "Gunmetal");
                Segment(ship, "NozzleThroat_" + quad[q], PrimitiveType.Cylinder,
                    new Vector3(qx, qy, nozZ + 0.06f), new Vector3(qx, qy, nozZ - 0.26f),
                    drumD * 0.17f, SpecThroat, false, "Throat");
            }
            for (int g = 0; g < 2; g++)
            {
                Part(ship, "EngineGreeble_" + g, PrimitiveType.Cube,
                    new Vector3((g == 0 ? -1f : 1f) * drumD * 0.40f, drumD * 0.34f, Z(0.75f)),
                    new Vector3(hullHalfW * 0.30f, h * 0.16f, l * 0.06f),
                    Vector3.zero, steelAlt, false, "SteelAlt");
            }
            Part(ship, "Nav_Tail", PrimitiveType.Sphere,
                new Vector3(0f, drumD * 0.56f, Z(0.86f)), Vector3.one * 0.10f,
                Vector3.zero, SpecMarker, false, "Marker", true);

            // ── GROUP 5 · CLAW ARM — LEFT flank only, shoulder ~0.12; asymmetry is canon ──
            float shoulderZ = Z(0.12f);
            Vector3 shoulder = new Vector3(-hullHalfW * 1.05f, -h * 0.12f, shoulderZ);
            Part(ship, "ClawArm_Shoulder", PrimitiveType.Cube,
                shoulder, new Vector3(hullHalfW * 0.40f, h * 0.24f, l * 0.055f),
                Vector3.zero, gunmetal, true, "Gunmetal");
            Vector3 elbow = new Vector3(-hullHalfW * 1.62f, bellyY - h * 0.16f, shoulderZ + l * 0.03f);
            Vector3 wrist = new Vector3(-hullHalfW * 1.34f, bellyY - h * 0.34f, shoulderZ + l * 0.075f);
            Segment(ship, "ClawArm_Upper", PrimitiveType.Capsule, shoulder, elbow,
                0.17f, steelAlt, false, "SteelAlt");
            Segment(ship, "ClawArm_Fore", PrimitiveType.Capsule, elbow, wrist,
                0.14f, steelAlt, false, "SteelAlt");
            Segment(ship, "ClawArm_Piston", PrimitiveType.Cylinder,
                shoulder + new Vector3(-0.05f, -0.10f, 0.06f), elbow + new Vector3(0.05f, 0.10f, -0.04f),
                0.075f, gunmetal, false, "Gunmetal");
            Segment(ship, "ClawArm_Wrist", PrimitiveType.Cylinder,
                wrist, wrist + new Vector3(0.02f, -0.14f, 0.04f), 0.20f, gunmetal, false, "Gunmetal");
            for (int f = 0; f < 3; f++)
            {
                float a = -0.55f + f * 0.55f;
                Vector3 tip = wrist + new Vector3(a * 0.28f, -h * 0.20f, 0.10f + Mathf.Abs(a) * 0.06f);
                Segment(ship, "ClawFinger_" + f, PrimitiveType.Capsule,
                    wrist + new Vector3(0f, -0.12f, 0f), tip, 0.075f, steelAlt, false, "SteelAlt");
            }

            // ── GROUP 6 · LANDING LEGS ×4 — front ~0.25, rear ~0.70, stance WIDER than the hull ──
            BuildLeg(ship, "Leg_FL", -1f, Z(0.25f), w, h, hullHalfW, bellyY, steelAlt, gunmetal);
            BuildLeg(ship, "Leg_FR", 1f, Z(0.25f), w, h, hullHalfW, bellyY, steelAlt, gunmetal);
            BuildLeg(ship, "Leg_RL", -1f, Z(0.70f), w, h, hullHalfW, bellyY, steelAlt, gunmetal);
            BuildLeg(ship, "Leg_RR", 1f, Z(0.70f), w, h, hullHalfW, bellyY, steelAlt, gunmetal);

            Debug.Log("ZIPTIDE: HERO_SHIP_HULL model=SLV-01_SCRAPPER renderers="
                + ship.GetComponentsInChildren<Renderer>(true).Length
                + " materials=" + Materials.Count
                + " size=" + size.ToString("F1"));
            Materials.Clear();
        }

        /// <summary>
        /// One hydraulic 2-segment leg (upper strut + piston) on a broad flat skid. The foot sits
        /// outboard of the hull so the front view shows daylight between leg and body — spec §2 calls
        /// that gap the "stable workhorse" read, so it is load-bearing, not decoration.
        /// </summary>
        private static void BuildLeg(Transform ship, string name, float side, float z,
            float w, float h, float hullHalfW, float bellyY, Color strutColor, Color pistonColor)
        {
            Vector3 hip = new Vector3(side * hullHalfW * 0.85f, bellyY + h * 0.06f, z);
            Vector3 knee = new Vector3(side * w * 0.68f, bellyY - h * 0.28f, z);
            Vector3 foot = new Vector3(side * w * 0.80f, bellyY - h * 0.45f, z);

            Segment(ship, name, PrimitiveType.Capsule, hip, knee, 0.19f, strutColor, false, "SteelAlt");
            Segment(ship, name + "_Piston", PrimitiveType.Cylinder, knee, foot, 0.12f,
                pistonColor, false, "Gunmetal");
            Part(ship, name + "_Skid", PrimitiveType.Cube,
                foot + new Vector3(0f, -h * 0.03f, 0f),
                new Vector3(w * 0.14f, h * 0.05f, w * 0.30f),
                Vector3.zero, strutColor, true, "SteelAlt");
            // Hazard striping on the struts (spec §3).
            Part(ship, name + "_Hazard", PrimitiveType.Cube,
                Vector3.Lerp(hip, knee, 0.55f),
                new Vector3(0.21f, h * 0.06f, 0.21f),
                Vector3.zero, SpecHazard, false, "Hazard");
        }

        /// <summary>
        /// Places a capsule/cylinder spanning two local points. Authoring legs, pipes and the claw from
        /// endpoints rather than hand-derived Euler angles keeps the mirrored left/right sides honest —
        /// sign errors in a hand-written rotation are exactly how a leg ends up bending the wrong way.
        /// </summary>
        private static GameObject Segment(Transform parent, string name, PrimitiveType primitive,
            Vector3 a, Vector3 b, float thickness, Color color, bool collider, string materialSlot,
            bool emissive = false)
        {
            Vector3 delta = b - a;
            float length = Mathf.Max(0.001f, delta.magnitude);
            Vector3 euler = Quaternion.FromToRotation(Vector3.up, delta / length).eulerAngles;
            return Part(parent, name, primitive, (a + b) * 0.5f,
                new Vector3(thickness, length * 0.5f, thickness), euler, color, collider,
                materialSlot, emissive);
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType primitive,
            Vector3 localPos, Vector3 scale, Vector3 euler, Color color, bool collider,
            string materialSlot, bool emissive = false)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.transform.localRotation = Quaternion.Euler(euler);
            Collider col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = Mat(materialSlot, color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Quest budget
            }
            return go;
        }

        /// <summary>Ship identity leads; the world palette tints it a quarter of the way, so a themed
        /// world still reads on the hull without repainting the Scrapper into a different ship.</summary>
        private static Color Tint(Color identity, Color themed)
        {
            return Color.Lerp(identity, themed, 0.25f);
        }

        private static Material Mat(string slot, Color color, bool emissive)
        {
            if (Materials.TryGetValue(slot, out Material material) && material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = "Scrapper_" + slot };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.7f);
            }
            Materials[slot] = material;
            return material;
        }
    }
}
#endif
