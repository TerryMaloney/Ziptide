#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE EXPEDITION SITE — the crashed survey skiff on the tidal flats, OUTSIDE the sea wall
    /// (⚖ Terry, 2026-07-29). Story of record: half B is not a paperweight on the Dockmaster's
    /// desk; he only has the LEAD. You take the vehicle out through the wall's breach, across the
    /// open flats, to a survey skiff that went nose-down the same week the relay started reading
    /// wrong — and half B is in its cracked cargo cage.
    ///
    /// Everything here is procedural stand-in geometry per the stand-in law: the wreck, the burn-off
    /// stack that marks it from the breach, the salvage scatter, and the rubble ramp that makes the
    /// breach obviously drivable. The half-B pickup itself is NOT built here — it is pack data
    /// (spec collectible), so the item, its flag, and its position all stay in one authored place.
    ///
    /// Geometry is placed in RING coordinates so the site always agrees with the sea wall the
    /// RingCityBuilder actually generates: the breach bearing is computed with the same formula,
    /// not guessed at.
    /// </summary>
    public static class FlatsSiteAuthor
    {
        public const string RootName = "__FLATS_EXPEDITION_SITE";

        /// <summary>Marker id the contract's lead step walks to. The GameObject is created with the
        /// literal name "Marker_flats_site" so JobDirector's scene fallback resolves it and the
        /// worldspec↔contract gate can see it in this source file.</summary>
        public const string SiteMarkerId = "flats_site";

        /// <summary>Bearing (degrees) of the site out on the flats, and its radius from the city
        /// centre — inside the outskirts ring so it stands on the generated tidal flat.</summary>
        public const float SiteAzimuthDegrees = 30f;
        public const float SiteRadius = 175f;

        /// <summary>World position of the wreck. Public so tests and the vehicle bay agree with it.</summary>
        public static Vector3 SitePosition
        {
            get
            {
                float a = SiteAzimuthDegrees * Mathf.Deg2Rad;
                return new Vector3(Mathf.Cos(a) * SiteRadius, 0f, Mathf.Sin(a) * SiteRadius);
            }
        }

        /// <summary>
        /// The bearing of the breach you drive out through. Mirrors RingCityBuilder's breach
        /// placement exactly (48 segments, evenly spaced, +7 offset) so the ramp is always AT a
        /// gap rather than against a wall — the class of mistake that turns a 3-minute drive into
        /// a player driving in circles looking for the way out.
        /// </summary>
        public static float BreachAzimuthDegrees(RingCityDef rings)
        {
            const int segments = 48;
            int count = rings != null ? Mathf.Max(1, rings.seaWallBreachCount) : 2;
            float best = 0f;
            float bestDelta = float.MaxValue;
            for (int b = 0; b < count; b++)
            {
                int seg = (int)(segments * ((b + 0.5f) / count) + 7) % segments;
                float deg = seg / (float)segments * 360f;
                // Pick the breach closest to the site's bearing — the drive should leave the city
                // pointing roughly where it's going.
                float delta = Mathf.Abs(Mathf.DeltaAngle(deg, SiteAzimuthDegrees));
                if (delta < bestDelta) { bestDelta = delta; best = deg; }
            }
            return best;
        }

        /// <summary>Build (or refresh) the site under the city root. Idempotent.</summary>
        public static void Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null) return;

            Transform existing = cityRoot.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            Transform root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);

            Vector3 site = SitePosition;
            BuildWreck(root, site);
            BuildBurnOffStack(root, site);
            BuildScatter(root, site);
            BuildBreachRamp(root, kit != null ? kit.rings : null);

            // The objective marker the contract's lead step resolves. Literal name on purpose.
            var marker = new GameObject("Marker_" + SiteMarkerId);
            marker.transform.SetParent(root, false);
            marker.transform.position = site + new Vector3(0f, 0.1f, -3f);

            Debug.Log("ZIPTIDE: FLATS_SITE built site=" + site
                + " breachDeg=" + BreachAzimuthDegrees(kit != null ? kit.rings : null).ToString("F1"));
        }

        /// <summary>
        /// The survey skiff: nose-down in the mud, spine broken, cargo cage cracked open. Built
        /// tilted rather than upright — a wreck that sits level reads as a parked ship, and the
        /// whole point is that something went wrong out here.
        /// </summary>
        private static void BuildWreck(Transform root, Vector3 site)
        {
            Transform wreck = Child(root, "SurveySkiffWreck", site);
            wreck.localRotation = Quaternion.Euler(-24f, 34f, 9f);

            Cube(wreck, "Hull", new Vector3(0f, 1.1f, 0f), new Vector3(3.2f, 1.8f, 9f),
                new Color(0.34f, 0.32f, 0.28f));
            Cube(wreck, "Nose", new Vector3(0f, 0.5f, 5.2f), new Vector3(2.2f, 1.2f, 2.6f),
                new Color(0.30f, 0.28f, 0.25f));
            Cube(wreck, "Fin_L", new Vector3(-2.1f, 1.9f, -3.2f), new Vector3(0.25f, 2.2f, 2.4f),
                new Color(0.38f, 0.30f, 0.22f));
            Cube(wreck, "Fin_R", new Vector3(2.1f, 1.9f, -3.2f), new Vector3(0.25f, 2.2f, 2.4f),
                new Color(0.38f, 0.30f, 0.22f));

            // The cage half B sits in: open ribs, not a box, so the pickup inside is visible from
            // outside and the player walks TO something they already saw.
            Transform cage = Child(wreck, "CargoCage", new Vector3(0f, 1.6f, -1.4f));
            for (int i = 0; i < 5; i++)
            {
                float x = -1.2f + i * 0.6f;
                Cube(cage, "Rib_" + i, new Vector3(x, 0f, 0f), new Vector3(0.09f, 1.5f, 0.09f),
                    new Color(0.45f, 0.42f, 0.36f));
            }
            Cube(cage, "CageFloor", new Vector3(0f, -0.75f, 0f), new Vector3(2.8f, 0.1f, 1.6f),
                new Color(0.30f, 0.29f, 0.27f));
        }

        /// <summary>
        /// THE BURN-OFF STACK — the wayfinding. The work order pins a smoke column; from the breach
        /// you steer at the smoke rather than at a waypoint on a HUD. Tapered stack plus stacked
        /// puff blocks that read as a column from a long way out.
        /// </summary>
        private static void BuildBurnOffStack(Transform root, Vector3 site)
        {
            Transform stack = Child(root, "BurnOffStack", site + new Vector3(-7f, 0f, 5f));
            Cube(stack, "Pipe", new Vector3(0f, 3f, 0f), new Vector3(0.7f, 6f, 0.7f),
                new Color(0.36f, 0.30f, 0.24f));
            Cube(stack, "Collar", new Vector3(0f, 6.1f, 0f), new Vector3(1.1f, 0.4f, 1.1f),
                new Color(0.48f, 0.36f, 0.20f));

            // The column: widening, dimming puffs. Visible over the 7 m sea wall from inside the
            // city, which is what makes "follow the wall, then the smoke" work as directions.
            for (int i = 0; i < 7; i++)
            {
                float t = i / 6f;
                float y = 7f + i * 2.6f;
                var puff = Cube(stack, "Puff_" + i,
                    new Vector3(Mathf.Sin(i * 1.3f) * (0.6f + t * 2.2f), y, Mathf.Cos(i * 0.9f) * (0.4f + t * 1.6f)),
                    Vector3.one * (1.4f + t * 3.4f),
                    Color.Lerp(new Color(0.55f, 0.45f, 0.35f), new Color(0.32f, 0.31f, 0.30f), t));
                puff.transform.localRotation = Quaternion.Euler(i * 21f, i * 37f, i * 13f);
            }
        }

        /// <summary>Salvage scatter — the site has to look like a place things fell out of.</summary>
        private static void BuildScatter(Transform root, Vector3 site)
        {
            Transform scatter = Child(root, "Scatter", site);
            for (int i = 0; i < 10; i++)
            {
                float a = i * 2.39996f;
                float r = 4f + (i % 4) * 2.6f;
                var piece = Cube(scatter, "Debris_" + i,
                    new Vector3(Mathf.Cos(a) * r, 0.18f, Mathf.Sin(a) * r),
                    new Vector3(0.5f + (i % 3) * 0.35f, 0.3f, 0.7f + (i % 2) * 0.5f),
                    new Color(0.31f + (i % 3) * 0.02f, 0.29f, 0.26f));
                piece.transform.localRotation = Quaternion.Euler(0f, i * 41f, i % 3 == 0 ? 14f : 0f);
            }
        }

        /// <summary>
        /// Rubble ramps in the breach mouth. The wall is 7 m of concrete with a missing segment;
        /// without spilled rubble the gap reads as damage rather than as the way out, and a player
        /// looking for an exit is a player not going on the expedition.
        /// </summary>
        private static void BuildBreachRamp(Transform root, RingCityDef rings)
        {
            float radius = rings != null ? rings.seaWallRadius : 126f;
            float deg = BreachAzimuthDegrees(rings);
            float a = deg * Mathf.Deg2Rad;
            Vector3 mouth = new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);

            Transform ramp = Child(root, "BreachRamp", mouth);
            ramp.localRotation = Quaternion.Euler(0f, -deg, 0f);

            // Two spill piles either side of the gap + a low graded lip through it.
            Cube(ramp, "Spill_L", new Vector3(0f, 0.6f, -4.2f), new Vector3(6f, 1.2f, 2.4f),
                new Color(0.40f, 0.39f, 0.36f));
            Cube(ramp, "Spill_R", new Vector3(0f, 0.6f, 4.2f), new Vector3(6f, 1.2f, 2.4f),
                new Color(0.40f, 0.39f, 0.36f));
            var lip = Cube(ramp, "Lip", new Vector3(0f, 0.12f, 0f), new Vector3(9f, 0.25f, 6.5f),
                new Color(0.44f, 0.43f, 0.40f));
            lip.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // ── primitives (same shape as the other authors) ────────────────────────────────────────

        private static Transform Child(Transform parent, string name, Vector3 localPos)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.localPosition = localPos;
            return t;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ItemFactory.ApplyURPColor(go, color);
            return go;
        }
    }
}
#endif
