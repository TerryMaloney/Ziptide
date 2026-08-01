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

        public static void Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null) return;
            var berth = kit.shipyard;
            if (berth == null || !berth.enabled) return;

            var yard = FindDistrict(kit, "Shipyard");
            if (yard == null) return;

            // Idempotent by name, like every other author in the bake.
            var existing = cityRoot.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);
            _mats.Clear();   // never hand a material instance from a previous scene to this one

            // The walk: from the berth's landward edge to the district's seaward edge. Measured off
            // the live layout rather than the design doc, so re-laying the city moves the junk with it.
            float centreX = berth.berthCenter.x;
            float fromZ = berth.berthCenter.z + berth.berthSize.y * 0.5f;
            float toZ = yard.anchor.z - yard.bounds.y * 0.5f;
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

            Debug.Log("ZIPTIDE: APPROACH_DRESSED pieces=" + placed + " crate=1"
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

            var go = Cube(root, CrateName, new Vector3(local.x, y + local.y, local.z),
                Vector3.one * s, CrateColor, collider: true);

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

        private static Color ColorFor(ClutterKind kind)
        {
            switch (kind)
            {
                case ClutterKind.CableSpool: return SpoolColor;
                case ClutterKind.Toolbox: return ToolboxColor;
                default: return CrateColor;
            }
        }

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            if (kit.districts == null) return null;
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 worldPos, Vector3 scale,
            Color color, bool collider)
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
            return go;
        }

        // Three colours, three materials — shared, because ten pieces of junk each carrying their own
        // material is ten more draw calls on a device that counts them.
        private static readonly System.Collections.Generic.Dictionary<Color, Material> _mats
            = new System.Collections.Generic.Dictionary<Color, Material>();

        private static Material Mat(Color c)
        {
            if (_mats.TryGetValue(c, out var cached) && cached != null) return cached;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var m = new Material(shader) { name = "Approach_" + ColorUtility.ToHtmlStringRGB(c) };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            _mats[c] = m;
            return m;
        }
    }
}
#endif
