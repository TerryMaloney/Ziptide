#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE FIRST WALK — the quay between the ship's ramp and the shipyard district
    /// (HANGAR_AND_COUPLER_PASS §2.4 and §2.5).
    ///
    /// Two jobs, and only one of them is decoration.
    ///
    /// **Foreground.** The quay had a midground (the berths) and a background (the skyline) and
    /// nothing whatsoever inside arm's reach, which is the reason it read as a diagram of a shipyard
    /// rather than a shipyard. Objects the player physically passes are what supply motion parallax,
    /// and parallax is most of how a VR player feels distance at all.
    ///
    /// **The grab lesson, taught by an object rather than a line.** One crate on the walk is
    /// grabbable and worth absolutely nothing. A player who picks it up has learned the verb at zero
    /// cost and without being told; a player who ignores it has lost nothing. That is the whole
    /// design — <em>the game's first tutorial is a piece of junk</em> — and it only works if the crate
    /// really is worthless, so it carries no ItemDefinition and never enters the inventory.
    ///
    /// Placement is <see cref="ApproachClutterCore"/>'s, not this file's, because the failure mode is
    /// a soft-lock: junk in the walking lane is invisible to a player who cannot see their own feet.
    /// </summary>
    public static class ShipyardApproachAuthor
    {
        public const string RootName = "__SHIPYARD_APPROACH";
        private const string CrateName = "LooseCrate";

        private static readonly Color CrateColor = new Color(0.42f, 0.30f, 0.20f);
        private static readonly Color SpoolColor = new Color(0.26f, 0.27f, 0.29f);
        private static readonly Color ToolboxColor = new Color(0.55f, 0.42f, 0.12f);
        private static readonly Color LampAmber = new Color(1f, 0.72f, 0.32f);

        public static void Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null) return;
            var berth = kit.shipyard;
            if (berth == null || !berth.enabled) return;

            // Idempotent by name, like every other author in the bake.
            var existing = cityRoot.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);

            // THE WALK IS THE BERTH DECK, alongside your own hull — amidships to the landward edge.
            //
            // The first version ran this from the berth's LANDWARD edge to the Shipyard district's
            // SEAWARD edge, which sounds exactly right and is one metre long: those two edges are
            // adjacent and the golden bridge spans the gap between them. Nine pieces of junk would
            // have been dumped in a heap on the bridge. A derived span needs its arithmetic checked
            // against the real numbers, not just its variable names — both of those names read as
            // "the walk".
            float centreX = berth.berthCenter.x;
            float fromZ = berth.berthCenter.z - berth.berthSize.y * 0.25f;
            float toZ = berth.berthCenter.z + berth.berthSize.y * 0.5f;
            float y = kit.walkwayHeight;

            int placed = 0;
            foreach (var piece in ApproachClutterCore.Place(centreX, fromZ, toZ))
            {
                var go = Cube(root, piece.Kind + "_" + placed,
                    new Vector3(piece.Position.x, y + piece.Position.y, piece.Position.z),
                    ApproachClutterCore.Footprint(piece), ColorFor(piece.Kind), collider: true);
                // Tilt the spools and boxes a few degrees. Nothing in a working yard is square to
                // anything else, and a perfect grid of crates reads as a texture rather than as junk.
                if (piece.Kind != ClutterKind.Crate)
                    go.transform.localRotation = Quaternion.Euler(0f, (placed * 37) % 90, 0f);
                placed++;
            }

            BuildTeachingCrate(root, centreX, fromZ, toZ, y);
            int overhead = BuildGantryRoof(root, berth, y);

            Debug.Log("ZIPTIDE: APPROACH_DRESSED pieces=" + placed + " crate=1 overhead=" + overhead
                      + " walkZ=" + fromZ.ToString("F0") + ".." + toZ.ToString("F0"));
        }

        /// <summary>
        /// The one grabbable thing on the walk. Rigidbody and collider go on BEFORE the interactable,
        /// which is the ordering the other patchers learned the hard way — XRGrabInteractable caches
        /// what it finds at Awake, and a collider added afterwards is a crate you cannot pick up.
        /// </summary>
        private static void BuildTeachingCrate(Transform root, float centreX, float fromZ, float toZ, float y)
        {
            Vector3 local = ApproachClutterCore.TeachingCratePosition(centreX, fromZ, toZ);
            float s = ApproachClutterCore.TeachingCrateSize;

            // staticBatch:false — this one is the whole point: it has a Rigidbody and the player
            // picks it up. Batching a body into a combined mesh is how a "grabbable" crate becomes
            // scenery that ignores your hands.
            var go = Cube(root, CrateName, new Vector3(local.x, y + local.y, local.z),
                Vector3.one * s, CrateColor, collider: true, staticBatch: false);

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 4f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = go.AddComponent<XRGrabInteractable>();
            var so = new SerializedObject(grab);
            var movement = so.FindProperty("m_MovementType");
            if (movement != null) movement.enumValueIndex = 1;      // VelocityTracking — it has weight
            var dynamic = so.FindProperty("m_UseDynamicAttach");
            if (dynamic != null) dynamic.boolValue = true;          // grab it anywhere; it is a box
            var ease = so.FindProperty("m_AttachEaseInTime");
            if (ease != null) ease.floatValue = 0.05f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// §2.2 — the partial gantry roof. Columns, trusses, two runners and three hanging lamps over
        /// the LANDWARD half of the berth only.
        ///
        /// The half matters more than the roof does. Covering the whole berth would seal off the sky,
        /// and the skyscape is the thing this game is arguably built around — a scale win that costs
        /// the horizon is not a win. `GantryRoofCore` owns that ratio and a test holds it.
        /// </summary>
        private static int BuildGantryRoof(Transform root, ShipyardBerthDef berth, float y)
        {
            GantryRoofCore.CoveredSpan(berth.berthCenter.z, berth.berthSize.y, out float fromZ, out float toZ);
            float halfX = berth.berthSize.x * 0.5f - GantryRoofCore.EdgeInset;
            float cx = berth.berthCenter.x;
            float trussY = y + GantryRoofCore.TrussHeight;
            int built = 0;

            for (int i = 0; i < GantryRoofCore.TrussCount; i++)
            {
                float z = GantryRoofCore.TrussZ(i, fromZ, toZ);

                // Columns, one per side, and the truss that spans them.
                for (int s = -1; s <= 1; s += 2)
                {
                    Cube(root, "GantryColumn_" + i + (s < 0 ? "a" : "b"),
                        new Vector3(cx + s * halfX, y + GantryRoofCore.TrussHeight * 0.5f, z),
                        new Vector3(0.45f, GantryRoofCore.TrussHeight, 0.45f), SpoolColor, collider: true);
                    built++;
                }

                Cube(root, "GantryTruss_" + i, new Vector3(cx, trussY, z),
                    new Vector3(halfX * 2f, 0.35f, 0.5f), SpoolColor, collider: false);
                built++;

                // One lamp per truss, hung below it. Unlit-bright rather than a real light: a row of
                // point lights over a berth is how a Quest frame budget dies (the lantern route
                // learned this first).
                Cube(root, "GantryLamp_" + i, new Vector3(cx, y + GantryRoofCore.LampHeight, z),
                    new Vector3(0.5f, 0.16f, 0.5f), LampAmber, collider: false);
                built++;
            }

            // Two runners along the length, which is what turns three arches into one structure.
            for (int s = -1; s <= 1; s += 2)
            {
                Cube(root, "GantryRunner_" + (s < 0 ? "a" : "b"),
                    new Vector3(cx + s * halfX, trussY, (fromZ + toZ) * 0.5f),
                    new Vector3(0.3f, 0.3f, toZ - fromZ), SpoolColor, collider: false);
                built++;
            }

            return built;
        }

        private static Color ColorFor(ClutterKind kind)
        {
            switch (kind)
            {
                case ClutterKind.CableSpool: return SpoolColor;
                case ClutterKind.Toolbox: return ToolboxColor;
                default: return CrateColor;
            }
        }

        private static GameObject Cube(Transform parent, string name, Vector3 worldPos, Vector3 scale,
            Color color, bool collider, bool staticBatch = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = worldPos;
            go.transform.localScale = scale;

            var col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);

            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = Mat(color);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            if (staticBatch) GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            return go;
        }

        // Was a private cache; PatchMaterials is now the one cache for the whole bake, so these
        // three colours merge with any identical grey the city already made.
        private static Material Mat(Color c) => PatchMaterials.Get(c);
    }
}
#endif
