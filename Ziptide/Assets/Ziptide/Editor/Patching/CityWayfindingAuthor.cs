#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE CITY'S COMPASS, BUILT (LEVEL1_SPATIAL_SCRIPT §3 — the last unbuilt wayfinding row).
    ///
    /// Toxic City had districts, a contract and a creature, and no way to read where any of it
    /// was. The objective beacon and RILL were carrying the whole load, which works right up to
    /// the moment a player turns around in a street and the city stops meaning anything.
    ///
    /// Two devices, both geometry, neither a HUD:
    ///
    /// **THE LANTERN ROUTE** — amber lanterns hang over the contract's walk and nowhere else.
    /// Lantern = the job goes this way; unlit street = you are exploring. That second half matters
    /// as much as the first: it is what makes wandering feel deliberate instead of lost.
    ///
    /// **THE SIGHTLINE TRIPLE** — from the dispatch plaza three things are visible at once: the
    /// relay mast's RED fault strobe (the errand), the north tower's lit crown (the fixed
    /// bearing), and your own ship's floodlights (home). Red, white and warm-amber, at three
    /// different heights, in three widely separated directions — so a glance is enough.
    ///
    /// Everything reads its positions off the LAYOUT the city builder is actually about to build,
    /// never off the design doc's coordinates, so the compass cannot drift away from the city.
    /// Procedural stand-in geometry per the stand-in law; Tripo re-skins behind the same names.
    /// </summary>
    public static class CityWayfindingAuthor
    {
        public const string RootName = "__CITY_WAYFINDING";

        /// <summary>The contract's walk, as district ids. Every consecutive pair MUST be an
        /// authored connection — CityWayfindingTests proves it, because a lantern leg with no
        /// street under it hangs lamps through a building.</summary>
        public static readonly string[] JobRoute =
        {
            "Dispatch",   // the contract board
            "Market",     // the combat drones
            "Plaza",      // the open middle, the north tower overhead
            "Colonnade",  // the Husk-Molter observation walk
            "CanalRow",   // the relay yard — and the thing that reads wrong
            "Dispatch",   // and back, which is what makes it a loop rather than a corridor
        };

        /// <summary>
        /// THE ARRIVAL WALK — the ramp to the contract board, and the first thirty seconds of the game.
        ///
        /// The lantern route used to start AT Dispatch, which meant the player walked the whole quay
        /// unlit and then arrived at a lit city with no idea the lamps meant anything. Lighting this
        /// leg teaches the grammar before anybody names it: you follow lamps, you find the job, and the
        /// first time you see an unlit street you already know what that means too.
        ///
        /// Kept separate from <see cref="JobRoute"/> rather than prepended, because the job route is a
        /// LOOP that must come home to Dispatch and this one is a one-way arrival. Merging them would
        /// have broken that law to save an array.
        /// </summary>
        public static readonly string[] ArrivalWalk = { "Quay", "Shipyard", "Dispatch" };

        private static readonly Color LanternAmber = new Color(1f, 0.72f, 0.32f);
        private static readonly Color FaultRed = new Color(1f, 0.24f, 0.18f);
        private static readonly Color CrownWhite = new Color(0.92f, 0.95f, 1f);
        private static readonly Color Iron = new Color(0.30f, 0.29f, 0.27f);

        private const float LanternHangHeight = 3.4f;

        /// <summary>Where the relay mast stands relative to the CanalRow anchor. NOT at the anchor:
        /// the RelayVault hero building is there (9×8 footprint), so a mast on the anchor would grow
        /// through its roof. Offset to the far side on purpose — it widens the angle between the
        /// strobe and the north tower, which is the difference between a compass and a smear.
        /// CityWayfindingTests measures the triple from THIS point, not from the anchor.</summary>
        public static readonly Vector3 RelayMastOffset = new Vector3(-6.5f, 0f, -5.5f);

        /// <summary>Build (or refresh) the compass under the city root. Idempotent.</summary>
        public static void Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null) return;

            Transform existing = cityRoot.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            Transform root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);

            var route = RouteWorldPoints(kit);
            var arrival = WorldPoints(kit, ArrivalWalk);

            // Both walks share the Dispatch corner, so the arrival's last lamp is the loop's first.
            // Merge before building rather than after: two lanterns in one spot is a z-fighting globe
            // that only shows up on device.
            var spots = WayfindingCore.MergeLanterns(
                WayfindingCore.LanternPositions(route, WayfindingCore.LanternSpacing),
                WayfindingCore.LanternPositions(arrival, WayfindingCore.LanternSpacing));

            int lanterns = BuildLanterns(root, spots, kit.walkwayHeight);
            float separation = BuildSightlineTriple(root, kit);

            Debug.Log("ZIPTIDE: WAYFINDING lanterns=" + lanterns + " legs=" + Mathf.Max(0, route.Count - 1)
                + " arrivalLegs=" + Mathf.Max(0, arrival.Count - 1)
                + " tripleSeparation=" + separation.ToString("F0"));
        }

        /// <summary>The route in world space, resolved against the districts that actually exist.
        /// A layout missing one of them simply drops that stop instead of planting lamps at the
        /// origin.</summary>
        public static List<Vector3> RouteWorldPoints(CityLayoutDefinition kit) => WorldPoints(kit, JobRoute);

        /// <summary>Any district-id walk, resolved to anchors against the layout being built.</summary>
        public static List<Vector3> WorldPoints(CityLayoutDefinition kit, IList<string> districtIds)
        {
            var points = new List<Vector3>();
            if (kit == null || districtIds == null) return points;
            foreach (string id in districtIds)
            {
                var d = FindDistrict(kit, id);
                if (d != null) points.Add(d.anchor);
            }
            return points;
        }

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            if (kit.districts == null) return null;
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
        }

        // ── The lantern route ──────────────────────────────────────────────────────────────────

        private static int BuildLanterns(Transform root, List<Vector3> spots, float walkwayHeight)
        {
            if (spots == null || spots.Count < 2) return 0;
            Transform lane = Child(root, "LanternRoute", Vector3.zero);

            for (int i = 0; i < spots.Count; i++)
            {
                Vector3 p = spots[i];
                p.y = walkwayHeight;
                BuildLantern(lane, "Lantern_" + i, p);
            }
            return spots.Count;
        }

        /// <summary>
        /// One hanging lantern: a post, a curved-over arm, and a small amber globe. Hung at 3.4 m —
        /// above the head so it never becomes an obstacle, low enough that it lights the ground
        /// rather than the sky. The globe is unlit-bright rather than a real light: fifty point
        /// lights down a street is how a Quest frame budget dies.
        /// </summary>
        private static void BuildLantern(Transform parent, string name, Vector3 worldPos)
        {
            Transform t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.position = worldPos;

            Cube(t, "Post", new Vector3(0f, LanternHangHeight * 0.5f, 0f),
                new Vector3(0.12f, LanternHangHeight, 0.12f), Iron);
            Cube(t, "Arm", new Vector3(0.35f, LanternHangHeight, 0f),
                new Vector3(0.8f, 0.09f, 0.09f), Iron);

            var globe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            globe.name = "Globe";
            globe.transform.SetParent(t, false);
            globe.transform.localPosition = new Vector3(0.7f, LanternHangHeight - 0.22f, 0f);
            globe.transform.localScale = Vector3.one * 0.32f;
            Object.DestroyImmediate(globe.GetComponent<Collider>());
            Paint(globe, LanternAmber, emissive: true);
        }

        // ── The sightline triple ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Plant the three landmarks and return the minimum angle between them as seen from the
        /// dispatch plaza. The number is logged rather than enforced here — CityWayfindingTests
        /// owns the law, so a layout change that collapses the compass fails CI instead of quietly
        /// shipping a city you cannot navigate.
        /// </summary>
        private static float BuildSightlineTriple(Transform root, CityLayoutDefinition kit)
        {
            var dispatch = FindDistrict(kit, "Dispatch");
            var canalRow = FindDistrict(kit, "CanalRow");
            var plaza = FindDistrict(kit, "Plaza");
            if (dispatch == null) return 0f;

            Vector3 eye = dispatch.anchor;
            var bearings = new List<float>();

            if (canalRow != null)
            {
                Vector3 mast = canalRow.anchor + RelayMastOffset;
                BuildRelayMast(root, mast, kit.walkwayHeight);
                bearings.Add(WayfindingCore.BearingDegrees(eye, mast));
            }
            if (plaza != null)
            {
                BuildNorthCrown(root, plaza.anchor, kit.walkwayHeight);
                bearings.Add(WayfindingCore.BearingDegrees(eye, plaza.anchor));
            }
            if (kit.shipyard != null && kit.shipyard.enabled)
            {
                BuildBerthFloodlights(root, kit.shipyard.berthCenter, kit.walkwayHeight);
                bearings.Add(WayfindingCore.BearingDegrees(eye, kit.shipyard.berthCenter));
            }

            return WayfindingCore.MinPairwiseSeparation(bearings);
        }

        /// <summary>
        /// THE RELAY MAST — 22 m of lattice with a RED fault strobe on top. Red because it is the
        /// only red light in the city: the errand announces itself, and the colour is already the
        /// story (the relay reads wrong long before anyone says so).
        /// </summary>
        private static void BuildRelayMast(Transform root, Vector3 at, float walkwayHeight)
        {
            Transform mast = Child(root, "RelayMast", new Vector3(at.x, walkwayHeight, at.z));
            for (int i = 0; i < 6; i++)
            {
                float y = 1.6f + i * 3.4f;
                float w = Mathf.Lerp(1.5f, 0.5f, i / 5f);
                Cube(mast, "Lattice_" + i, new Vector3(0f, y, 0f), new Vector3(w, 3.4f, w), Iron);
                Cube(mast, "Brace_" + i, new Vector3(0f, y, 0f), new Vector3(w * 1.35f, 0.1f, w * 1.35f),
                    new Color(0.38f, 0.36f, 0.33f));
            }
            Cube(mast, "DishArm", new Vector3(1.1f, 19f, 0f), new Vector3(2.2f, 0.12f, 0.12f), Iron);

            var strobe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            strobe.name = "FaultStrobe";
            strobe.transform.SetParent(mast, false);
            strobe.transform.localPosition = new Vector3(0f, 22f, 0f);
            strobe.transform.localScale = Vector3.one * 0.7f;
            Object.DestroyImmediate(strobe.GetComponent<Collider>());
            Paint(strobe, FaultRed, emissive: true);
            // A fault light that does not blink is a decoration. One tiny runtime, one sphere.
            strobe.AddComponent<FaultStrobeRuntime>();
        }

        /// <summary>
        /// THE NORTH TOWER'S CROWN — the plaza's tall landmark already exists as skyline; this is
        /// the lit collar that turns it into a BEARING. White, steady, and the only steady white
        /// light up high, so "north" is a thing you can check without thinking about it.
        /// </summary>
        private static void BuildNorthCrown(Transform root, Vector3 anchor, float walkwayHeight)
        {
            // Sits over the plaza's OligarchTower landmark offset (+9, +6) — the tallest thing in
            // the district and therefore the one the eye already goes to.
            Transform crown = Child(root, "NorthCrown",
                new Vector3(anchor.x + 9f, walkwayHeight + 40f, anchor.z + 6f));
            for (int i = 0; i < 4; i++)
            {
                float a = i * Mathf.PI * 0.5f;
                var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lamp.name = "CrownLamp_" + i;
                lamp.transform.SetParent(crown, false);
                lamp.transform.localPosition = new Vector3(Mathf.Cos(a) * 3.2f, 0f, Mathf.Sin(a) * 3.2f);
                lamp.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);
                Object.DestroyImmediate(lamp.GetComponent<Collider>());
                Paint(lamp, CrownWhite, emissive: true);
            }
            Cube(crown, "CrownRing", Vector3.zero, new Vector3(7f, 0.35f, 7f), Iron);
        }

        /// <summary>
        /// THE BERTH FLOODLIGHTS — two masts either side of the berth, throwing warm light down at
        /// your own hull. Warm because home is warm; low because they should read as UNDER the
        /// skyline, which is what makes "south, below the towers" a usable instruction.
        /// </summary>
        private static void BuildBerthFloodlights(Transform root, Vector3 berthCenter, float walkwayHeight)
        {
            Transform pad = Child(root, "BerthFloodlights",
                new Vector3(berthCenter.x, walkwayHeight, berthCenter.z));
            for (int s = -1; s <= 1; s += 2)
            {
                Transform mast = Child(pad, s < 0 ? "FloodMast_L" : "FloodMast_R",
                    new Vector3(s * 9f, 0f, 0f));
                Cube(mast, "Pole", new Vector3(0f, 4f, 0f), new Vector3(0.25f, 8f, 0.25f), Iron);
                Cube(mast, "Head", new Vector3(-s * 0.6f, 7.9f, 0f), new Vector3(1.4f, 0.5f, 0.9f), Iron);

                var lens = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lens.name = "Lens";
                lens.transform.SetParent(mast, false);
                lens.transform.localPosition = new Vector3(-s * 1.2f, 7.75f, 0f);
                lens.transform.localScale = new Vector3(0.5f, 0.42f, 0.8f);
                Object.DestroyImmediate(lens.GetComponent<Collider>());
                Paint(lens, LanternAmber, emissive: true);
            }
        }

        // ── helpers (FlatsSiteAuthor idiom) ────────────────────────────────────────────────────

        private static Transform Child(Transform parent, string name, Vector3 localPos)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.localPosition = localPos;
            return t;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPos, Vector3 scale,
            Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            Paint(go, color, emissive: false);
            return go;
        }

        /// <summary>URP paint with an optional emission pass — the same shape as every other
        /// patcher's local Paint (ScenePatcherSpaceLane, CityStageAPrimitiveFactory). Shadows off:
        /// these are markers, and a lantern that casts is a lantern that costs.</summary>
        private static void Paint(GameObject go, Color color, bool emissive)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (emissive && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 1.8f);
            }
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
