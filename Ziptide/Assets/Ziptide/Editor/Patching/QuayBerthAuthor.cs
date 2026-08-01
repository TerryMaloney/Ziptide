#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE HANGAR WALK, BUILT (LEVEL1_SPATIAL_SCRIPT §3 — rb121's deferred beat, and the last
    /// unbuilt geometry row of the first level).
    ///
    /// Berth six had a ship in it and nothing on either side, so the harbour read as one pad in
    /// open ground. Berths one to five now run west along the quay at the script's 14 m spacing:
    /// five EMPTY slips, each with its deck, its mooring bollards, and its number told in tally
    /// bars rather than text — a six-year-old counts three bars and knows it is berth three, and
    /// nothing has to survive being rendered as small type on a Quest panel.
    ///
    /// The emptiness is the point. Walking out to the job you pass five berths that used to hold
    /// something; walking back with the joined key, the beacon thread descends them one by one to
    /// the only one still occupied, which is yours.
    ///
    /// Positions come from <see cref="QuayBerthCore"/> against the LIVE layout's berth, so the walk
    /// cannot drift away from the shipyard, and the no-overlap gap is a tested property rather than
    /// a hand-tuned number (coplanar decks would z-fight the length of the quay).
    /// Procedural stand-in geometry per the stand-in law; Tripo re-skins behind the same names.
    /// </summary>
    public static class QuayBerthAuthor
    {
        public const string RootName = "__QUAY_BERTHS";

        private const float SlabThickness = 1f;   // matches CityBuilder's berth deck
        private const float BollardHeight = 0.7f;
        private const float TallyBarWidth = 0.22f;

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

            var centres = QuayBerthCore.PadCentres(berth.berthCenter, berth.berthSize.x);
            float deckY = kit.walkwayHeight - SlabThickness * 0.5f;

            for (int i = 0; i < centres.Count; i++)
            {
                int number = QuayBerthCore.BerthNumberAt(i);
                var slip = new GameObject("Berth_" + number).transform;
                slip.SetParent(root, false);
                slip.localPosition = new Vector3(centres[i].x, 0f, centres[i].z);

                // The deck: walkable, same slab as berth six so the quay reads as one surface.
                Cube(slip, "BerthDeck", new Vector3(0f, deckY, 0f),
                    new Vector3(QuayBerthCore.PadWidth, SlabThickness, QuayBerthCore.PadDepth),
                    kit.palette.metal, collider: true);

                // Mooring bollards at the water edge — the quay's silhouette from a distance.
                float bollardZ = QuayBerthCore.PadDepth * 0.5f - 1.2f;
                float bollardX = QuayBerthCore.PadWidth * 0.5f - 1.5f;
                Bollard(slip, "Bollard_W", new Vector3(-bollardX, kit.walkwayHeight, bollardZ), kit.palette.rail);
                Bollard(slip, "Bollard_E", new Vector3(bollardX, kit.walkwayHeight, bollardZ), kit.palette.rail);

                // The number, as TALLY BARS on a plate. Countable, readable at any resolution, and
                // legible to a player who cannot read — the child-readability law applied to level
                // geometry instead of to a menu.
                float plateZ = -QuayBerthCore.PadDepth * 0.5f + 1.6f;
                Cube(slip, "BerthPlate",
                    new Vector3(0f, kit.walkwayHeight + 0.02f, plateZ),
                    new Vector3(number * TallyBarWidth * 2f + 0.6f, 0.04f, 1.0f),
                    kit.palette.concrete, collider: false);

                // Bars are siblings of the plate, not children of it: the plate is a squashed cube,
                // and a child's local position/scale would be multiplied by that squash.
                float span = (number - 1) * TallyBarWidth * 2f;
                for (int bar = 0; bar < number; bar++)
                {
                    Cube(slip, "Tally_" + (bar + 1),
                        new Vector3(-span * 0.5f + bar * TallyBarWidth * 2f,
                                    kit.walkwayHeight + 0.05f, plateZ),
                        new Vector3(TallyBarWidth, 0.05f, 0.55f),
                        kit.palette.toxic, collider: false);
                }
            }

            Debug.Log("ZIPTIDE: QUAY_BERTHS built=" + centres.Count +
                      " spacing=" + QuayBerthCore.Spacing.ToString("F0") +
                      " disjoint=" + QuayBerthCore.DecksAreDisjoint(berth.berthCenter, berth.berthSize.x));
        }

        private static void Bollard(Transform parent, string name, Vector3 pos, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos + new Vector3(0f, BollardHeight * 0.5f, 0f);
            go.transform.localScale = new Vector3(0.34f, BollardHeight * 0.5f, 0.34f);
            Paint(go, color);
        }

        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale,
            Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            var col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);
            Paint(go, color);
            return go;
        }

        /// <summary>Shared paint — was a fresh material per pad, bollard, plate and tally bar.</summary>
        private static void Paint(GameObject go, Color color) => PatchMaterials.Paint(go, color);
    }
}
#endif
